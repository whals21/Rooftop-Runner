using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = -35f;
    [SerializeField] private float maxFallSpeed = -50f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Effects")]
    [SerializeField] private ParticleSystem landingEffect;

    private bool wasGrounded = false;

    // Components
    private CharacterController controller;

    // Movement variables
    private Vector3 velocity;
    private bool isGrounded;
    private bool canDoubleJump;

    // Public getter for other scripts
    public bool IsGrounded => isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // groundLayer를 Everything으로 설정 (나중에 Inspector에서 조정)
        groundLayer = ~0;
    }

    void Update()
    {
        CheckGround();

        // 게임이 Playing 상태일 때만 입력 처리
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameManager.GameState.Playing)
        {
            Move();
            Jump();
        }

        ApplyGravity();
        CheckLanding();
    }

    void CheckGround()
    {
        // 캐릭터 발 위치에서 Raycast로 지면 체크
        Vector3 spherePosition = transform.position - new Vector3(0, 1f, 0);
        isGrounded = Physics.CheckSphere(spherePosition, groundCheckDistance, groundLayer);

        // 땅에 닿으면 더블점프 리셋
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 약간의 downward force
            canDoubleJump = true;
        }
    }

    void Move()
    {
        // WASD 입력 받기
        float horizontal = Input.GetAxis("Horizontal"); // A, D
        float vertical = Input.GetAxis("Vertical");     // W, S

        // 이동 방향 계산
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;

        // 달리기 체크 (Left Shift)
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= sprintMultiplier;
        }

        // CharacterController로 이동
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    void Jump()
    {
        // Space 키 입력 체크
        if (Input.GetButtonDown("Jump"))
        {
            // 지면에 있으면 일반 점프
            if (isGrounded)
            {
                velocity.y = jumpForce;
                canDoubleJump = true; // 더블점프 가능하게 설정
            }
            // 공중에 있고 더블점프 가능하면
            else if (canDoubleJump)
            {
                velocity.y = jumpForce;
                canDoubleJump = false; // 더블점프 사용
                Debug.Log("Double Jump!");
            }
        }
    }

    void CheckLanding()
    {
        // 이전 프레임에는 공중이었고, 현재 프레임에 착지했다면
        if (!wasGrounded && isGrounded)
        {
            // 착지 파티클 재생
            if (landingEffect != null)
            {
                landingEffect.Play();
            }

            Debug.Log("착지!");
        }

        // 현재 상태 저장 (다음 프레임에서 비교용)
        wasGrounded = isGrounded;
    }

    void ApplyGravity()
    {
        // 중력 적용
        velocity.y += gravity * Time.deltaTime;

        // 최대 낙하 속도 제한 (터미널 속도)
        if (velocity.y < maxFallSpeed)
        {
            velocity.y = maxFallSpeed;
        }

        // Y축 이동 적용
        controller.Move(velocity * Time.deltaTime);
    }

    // Gizmos로 Ground Check 시각화 (Scene View에서만 보임)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 spherePosition = transform.position - new Vector3(0, 1f, 0);
        Gizmos.DrawWireSphere(spherePosition, groundCheckDistance);
    }

    // 외부에서 점프 강제 적용 (트램펄린용)
    public void ForceJump(float force)
    {
        velocity.y = force;
    }
}
