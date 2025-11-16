using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // 플레이어

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 3, -7);
    [SerializeField] private float smoothSpeed = 0.125f;

    [Header("Rotation Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minVerticalAngle = -20f;
    [SerializeField] private float maxVerticalAngle = 60f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // 마우스 커서 숨기기 (선택사항)
        // Cursor.lockState = CursorLockMode.Locked;

        if (target == null)
        {
            Debug.LogError("Camera Target이 설정되지 않았습니다!");
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 게임이 Playing 상태일 때만 마우스 입력 처리
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameManager.GameState.Playing)
        {
            // 마우스 입력으로 카메라 회전
            RotateCamera();
        }

        // 카메라 위치 계산 (게임 종료 후에도 플레이어 따라감)
        FollowTarget();
    }

    void RotateCamera()
    {
        // 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Y축 회전 (좌우)
        rotationY += mouseX;

        // X축 회전 (상하) - 제한
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        // 플레이어도 Y축 회전 적용 (캐릭터가 바라보는 방향)
        target.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    void FollowTarget()
    {
        // 목표 위치 계산 (오프셋 + 회전 적용)
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        // 부드럽게 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 카메라가 플레이어를 바라보도록
        transform.LookAt(target.position + Vector3.up * 1.5f); // 플레이어 중심보다 약간 위
    }
}