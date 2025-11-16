using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DirectionIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform arrowImage;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private Transform player;
    [SerializeField] private Camera playerCamera;

    [Header("Settings")]
    [SerializeField] private float edgeMargin = 50f;

    private Transform nearestPackage;

    void Update()
    {
        FindNearestPackage();

        if (nearestPackage != null)
        {
            UpdateArrowPosition();
        }
        else
        {
            HideArrow();
        }
    }

    void FindNearestPackage()
    {
        GameObject[] packages = GameObject.FindGameObjectsWithTag("Package");
        float minDistance = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject pkg in packages)
        {
            if (!pkg.activeInHierarchy) continue;

            float distance = Vector3.Distance(player.position, pkg.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = pkg.transform;
            }
        }

        nearestPackage = nearest;
    }

    void UpdateArrowPosition()
    {
        Vector3 screenPos = playerCamera.WorldToScreenPoint(nearestPackage.position);

        // 화면 밖이면 화살표 표시
        if (IsOffScreen(screenPos))
        {
            ShowArrow(screenPos);
        }
        else
        {
            HideArrow();
        }
    }

    bool IsOffScreen(Vector3 screenPos)
    {
        return screenPos.z < 0 ||
               screenPos.x < 0 || screenPos.x > Screen.width ||
               screenPos.y < 0 || screenPos.y > Screen.height;
    }

    void ShowArrow(Vector3 screenPos)
    {
        arrowImage.gameObject.SetActive(true);

        // 화면 가장자리로 클램프
        screenPos.x = Mathf.Clamp(screenPos.x, edgeMargin, Screen.width - edgeMargin);
        screenPos.y = Mathf.Clamp(screenPos.y, edgeMargin, Screen.height - edgeMargin);

        arrowImage.position = screenPos;

        // 화살표 회전 (패키지 방향)
        Vector3 direction = nearestPackage.position - player.position;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        arrowImage.rotation = Quaternion.Euler(0, 0, -angle);

        // 거리 표시
        if (distanceText != null)
        {
            float distance = Vector3.Distance(player.position, nearestPackage.position);
            distanceText.text = $"{distance:F0}m";
        }
    }

    void HideArrow()
    {
        arrowImage.gameObject.SetActive(false);
    }
}
