using UnityEngine;
using UnityEngine.UI;

public class GrappleController : MonoBehaviour
{
    [Header("Grapple Settings")]
    [SerializeField] private float grappleRange = 30f;
    [SerializeField] private float grappleSpeed = 20f;
    [SerializeField] private float grappleDuration = 1f;
    [SerializeField] private LayerMask grappleLayer;

    [Header("References")]
    [SerializeField] private LineRenderer ropeLine;
    [SerializeField] private Transform grappleOrigin; // 로프 시작 위치
    [SerializeField] private Camera playerCamera;

    [Header("UI")]
    [SerializeField] private RectTransform crosshair;

    // Grapple 상태
    private bool isGrappling = false;
    private Vector3 grapplePoint;
    private float grappleTimer = 0f;

    // Components
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 카메라 자동 찾기
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        // LineRenderer 설정
        if (ropeLine != null)
        {
            ropeLine.enabled = false;
            ropeLine.positionCount = 10;
        }

        // Grapple Origin 자동 설정 (플레이어 중심)
        if (grappleOrigin == null)
        {
            grappleOrigin = transform;
        }
    }

    void Update()
    {
        // 게임이 Playing 상태일 때만 그래플 입력 처리
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
        {
            // 게임이 끝나면 그래플 중단
            if (isGrappling)
            {
                StopGrapple();
            }
            return;
        }

        UpdateCrosshair();
        HandleGrappleInput();

        if (isGrappling)
        {
            ExecuteGrapple();
        }
    }

    void UpdateCrosshair()
    {
        if (crosshair == null) return;

        crosshair.position = Input.mousePosition;

        // 마우스 방향으로 레이캐스트
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 갈고리 가능한 오브젝트에 마우스가 올라가면 색상 변경
        Image crosshairImage = crosshair.GetComponent<Image>();
        if (Physics.Raycast(ray, out hit, grappleRange, grappleLayer))
        {
            crosshairImage.color = Color.green; // 갈고리 가능 (초록)
        }
        else
        {
            crosshairImage.color = Color.white; // 기본 (흰색)
        }
    }

    void OnDrawGizmos()
    {
        if (playerCamera == null || !Application.isPlaying) return;

        // 마우스 방향 레이 시각화
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, grappleRange, grappleLayer))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(ray.origin, hit.point);
            Gizmos.DrawSphere(hit.point, 0.5f); // 갈고리 지점 표시
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray.origin, ray.direction * grappleRange);
        }
    }

    void HandleGrappleInput()
    {
        // 마우스 우클릭으로 갈고리 발사
        if (Input.GetMouseButtonDown(1) && !isGrappling)
        {
            StartGrapple();
        }

        // 갈고리 취소 (우클릭 떼기 또는 Space)
        if ((Input.GetMouseButtonUp(1) || Input.GetKeyDown(KeyCode.Space)) && isGrappling)
        {
            StopGrapple();
        }
    }

    void StartGrapple()
    {
        // 마우스 위치를 월드 좌표로 변환
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Grapple 가능한 오브젝트 감지
        if (Physics.Raycast(ray, out hit, grappleRange, grappleLayer))
        {
            grapplePoint = hit.point;
            isGrappling = true;
            grappleTimer = 0f;

            // LineRenderer 활성화
            if (ropeLine != null)
            {
                ropeLine.enabled = true;
            }

            // 그래플 발사 사운드 재생
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayGrappleShoot();
            }

            Debug.Log("Grapple Start: " + hit.collider.name);
        }
        else
        {
            Debug.Log("갈고리 범위 밖이거나 감지 불가능한 오브젝트입니다.");
        }
    }

    void ExecuteGrapple()
    {
        grappleTimer += Time.deltaTime;

        // 플레이어를 갈고리 지점으로 당기기
        Vector3 direction = (grapplePoint - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, grapplePoint);

        // 가까워지면 멈춤
        if (distance > 2f)
        {
            controller.Move(direction * grappleSpeed * Time.deltaTime);
        }
        else
        {
            StopGrapple();
        }

        // 로프 시각화
        UpdateRopeLine();

        // 일정 시간 후 자동 종료
        if (grappleTimer >= grappleDuration)
        {
            StopGrapple();
        }
    }

    void UpdateRopeLine()
    {
        if (ropeLine != null && isGrappling)
        {
            int segments = ropeLine.positionCount;
            Vector3 startPos = grappleOrigin.position;
            Vector3 endPos = grapplePoint;

            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)(segments - 1);
                Vector3 point = Vector3.Lerp(startPos, endPos, t);

                // 중간에 아래로 처지는 효과 (포물선)
                float sag = Mathf.Sin(t * Mathf.PI) * 2f; // 2f = 처짐 정도
                point.y -= sag;

                ropeLine.SetPosition(i, point);
            }
        }
    }

    void StopGrapple()
    {
        isGrappling = false;
        grappleTimer = 0f;

        // LineRenderer 비활성화
        if (ropeLine != null)
        {
            ropeLine.enabled = false;
        }

        Debug.Log("Grapple Stop");
    }

    // Gizmo로 갈고리 범위 시각화
    void OnDrawGizmosSelected()
    {
        if (playerCamera == null) return;

        Gizmos.color = Color.green;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Gizmos.DrawRay(ray.origin, ray.direction * grappleRange);
    }

    // 외부에서 갈고리 상태 확인용
    public bool IsGrappling()
    {
        return isGrappling;
    }
}
