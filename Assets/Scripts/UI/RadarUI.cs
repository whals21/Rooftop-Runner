using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RadarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform radarPanel;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject packageDotPrefab; // 패키지 표시 점 프리팹

    [Header("Settings")]
    [SerializeField] private float radarRange = 50f; // 레이더 범위
    [SerializeField] private float radarRadius = 100f; // UI 레이더 반지름 (픽셀)
    [SerializeField] private RectTransform scanLine; // 회전하는 스캔 라인
    [SerializeField] private Color normalPackageColor = Color.yellow;
    [SerializeField] private Color rarePackageColor = Color.red;

    // 패키지 점들을 관리
    private Dictionary<GameObject, RectTransform> packageDots = new Dictionary<GameObject, RectTransform>();
    private List<GameObject> dotPool = new List<GameObject>();

    void Update()
    {
        UpdateRadar();
        RotateScanLine();
    }

    void UpdateRadar()
    {
        // 모든 활성 패키지 찾기
        GameObject[] packages = GameObject.FindGameObjectsWithTag("Package");
        HashSet<GameObject> activePackages = new HashSet<GameObject>();

        foreach (GameObject pkg in packages)
        {
            if (!pkg.activeInHierarchy) continue;

            activePackages.Add(pkg);

            // 패키지가 레이더 범위 안에 있는지 체크
            float distance = Vector3.Distance(player.position, pkg.transform.position);
            if (distance <= radarRange)
            {
                UpdatePackageDot(pkg, distance);
            }
            else
            {
                // 범위 밖이면 점 숨기기
                if (packageDots.ContainsKey(pkg))
                {
                    packageDots[pkg].gameObject.SetActive(false);
                }
            }
        }

        // 더 이상 존재하지 않는 패키지의 점 제거
        List<GameObject> toRemove = new List<GameObject>();
        foreach (var kvp in packageDots)
        {
            if (!activePackages.Contains(kvp.Key))
            {
                kvp.Value.gameObject.SetActive(false);
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var pkg in toRemove)
        {
            packageDots.Remove(pkg);
        }
    }

    void UpdatePackageDot(GameObject package, float distance)
    {
        // 패키지에 대한 점이 없으면 생성
        if (!packageDots.ContainsKey(package))
        {
            RectTransform dot = GetDotFromPool();
            packageDots[package] = dot;
        }

        RectTransform dotTransform = packageDots[package];
        dotTransform.gameObject.SetActive(true);

        // 플레이어 기준 상대 위치 계산
        Vector3 direction = package.transform.position - player.position;
        direction.y = 0; // 수평면만 고려

        // 2D 평면 투영 (X, Z)
        Vector2 radarPos = new Vector2(direction.x, direction.z);

        // 레이더 범위에 맞게 스케일 조정
        float normalizedDistance = distance / radarRange;
        radarPos = radarPos.normalized * (normalizedDistance * radarRadius);

        // 점 위치 설정 (레이더 패널 중심 기준)
        dotTransform.anchoredPosition = radarPos;

        // 패키지 종류에 따라 색상 변경 (선택)
        Image dotImage = dotTransform.GetComponent<Image>();
        if (dotImage != null)
        {
            // PackageData 확인하여 색상 설정 (고급)
            Package pkgScript = package.GetComponent<Package>();
            // 기본은 노란색, 특별 패키지는 빨간색 등
            dotImage.color = normalPackageColor;
        }

        float dotScale = Mathf.Lerp(1.2f, 0.6f, normalizedDistance); // 가까울수록 크게
        dotTransform.localScale = Vector3.one * dotScale;
    }

    RectTransform GetDotFromPool()
    {
        // 비활성화된 점 재사용
        foreach (var dot in dotPool)
        {
            if (!dot.activeInHierarchy)
            {
                return dot.GetComponent<RectTransform>();
            }
        }

        // 풀에 없으면 새로 생성
        GameObject newDot = Instantiate(packageDotPrefab, radarPanel);
        dotPool.Add(newDot);
        return newDot.GetComponent<RectTransform>();
    }

    void RotateScanLine()
    {
        if (scanLine != null)
        {
            scanLine.Rotate(0, 0, -90f * Time.deltaTime); // 초당 90도 회전
        }
    }
}