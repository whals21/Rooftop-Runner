using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;

    // Animator Parameter 이름 (문자열 대신 Hash 사용 - 최적화)
    private int speedHash;
    private int isJumpingHash;
    private int isFallingHash;
    private int deathHash;

    // 현재 이동 속도
    private Vector3 lastPosition;
    private float currentSpeed = 0f;

    // 낙하 감지용
    private float lastYPosition;
    private float verticalVelocity;

    // 사망 상태
    private bool isDead = false;

    void Start()
    {
        // 컴포넌트 자동 찾기
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        // Animator Parameter Hash 미리 계산
        speedHash = Animator.StringToHash("Speed");
        isJumpingHash = Animator.StringToHash("IsJumping");
        isFallingHash = Animator.StringToHash("IsFalling");
        deathHash = Animator.StringToHash("Death");

        lastPosition = transform.position;
        lastYPosition = transform.position.y;
    }

    void Update()
    {
        if (!isDead)  // ← 사망 상태가 아닐 때만 업데이트
        {
            UpdateAnimationParameters();
        }
    }

    void UpdateAnimationParameters()
    {
        // 현재 이동 속도 계산
        float distance = Vector3.Distance(transform.position, lastPosition);
        currentSpeed = distance / Time.deltaTime;
        lastPosition = transform.position;

        // Speed 파라미터 업데이트 (0 ~ 2 범위로 정규화)
        float normalizedSpeed = Mathf.Clamp01(currentSpeed / 5f); // 5 = moveSpeed
        animator.SetFloat(speedHash, normalizedSpeed * 2f); // 0~2

        // PlayerController의 isGrounded 상태 가져오기
        bool isGrounded = CheckGrounded();
  
        // IsJumping 파라미터 업데이트
        animator.SetBool(isJumpingHash, !isGrounded);

        // IsFalling 파라미터 업데이트
        // Y축 속도 계산 (이전 프레임과 현재 프레임의 Y 위치 차이)
        verticalVelocity = (transform.position.y - lastYPosition) / Time.deltaTime;
        lastYPosition = transform.position.y;

        // 낙하 중인지 체크: 공중에 있으면서 아래로 떨어지고 있을 때
        // bool isFalling = !isGrounded && verticalVelocity < -1f; // -1f는 임계값
        // animator.SetBool(isFallingHash, isFalling);
    }

    bool CheckGrounded()
    {
        Vector3 spherePosition = transform.position - new Vector3(0, 1f, 0);
        return Physics.CheckSphere(spherePosition, 0.2f);
    }

    /// <summary>
    /// 사망 애니메이션 재생
    /// </summary>
    public void PlayDeathAnimation()
    {
        if (animator != null)
        {
            isDead = true;  // ← 사망 플래그 설정
            //animator.SetTrigger(deathHash);
            animator.SetTrigger("Death");
            Debug.Log("Death 트리거 설정 + 파라미터 업데이트 중단");
        }
    }

    /// <summary>
    /// 애니메이터 리셋 (리스폰 시 사용)
    /// </summary>
    public void ResetAnimation()
    {
        if (animator != null)
        {
            isDead = false;
            animator.Rebind();
        }
    }

    /// <summary>
    /// Animator에 특정 파라미터가 있는지 확인
    /// </summary>
    private bool HasParameter(string paramName)
    {
        if (animator == null) return false;

        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
}