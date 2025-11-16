using System.Collections;
using UnityEngine;

public class PackageSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private ObjectPool packagePool;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int totalPackagesToSpawn = 15;
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private int initialSpawnCount = 3;

    [Header("Package Types")]
    [SerializeField] private PackageData[] packageTypes; // 여러 종류의 PackageData
    [SerializeField] private float rarePackageChance = 0.2f; // 20% 확률로 특별 패키지

    private int currentSpawnedCount = 0;

    void Start()
    {
        StartCoroutine(DelayedSpawn());
    }

    IEnumerator DelayedSpawn()
    {
        yield return null;
        SpawnInitialPackages();
    }

    // 게임 시작 시 초기 패키지 스폰
    void SpawnInitialPackages()
    {
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnPackage();
        }
    }

    // 패키지 1개 스폰
    public void SpawnPackage()
    {
        if (currentSpawnedCount >= totalPackagesToSpawn)
        {
            Debug.Log("모든 패키지 스폰 완료!");
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPoint();
        GameObject package = packagePool.Get();

        if (package != null)
        {
            package.transform.position = spawnPosition;
            package.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            Package packageScript = package.GetComponent<Package>();
            if (packageScript != null)
            {
                packageScript.ResetPackage();

                // ===== 랜덤으로 PackageData 선택 =====
                if (packageTypes != null && packageTypes.Length > 0)
                {
                    PackageData selectedData = GetRandomPackageData();
                    packageScript.SetPackageData(selectedData);
                }
                // ====================================
            }

            currentSpawnedCount++;
            Debug.Log($"Package Spawned: {currentSpawnedCount}/{totalPackagesToSpawn}");
        }
    }

    PackageData GetRandomPackageData()
    {
        float randomValue = Random.value;

        // 20% 확률로 특별 패키지 (ExpressPackage 또는 VIPPackage)
        if (randomValue < rarePackageChance && packageTypes.Length > 1)
        {
            // 일반 패키지(0번)를 제외한 나머지 중 랜덤 선택
            int rareIndex = Random.Range(1, packageTypes.Length);
            return packageTypes[rareIndex];
        }
        else
        {
            // 일반 패키지
            return packageTypes[0];
        }
    }

    // 랜덤 스폰 포인트 위치 반환
    Vector3 GetRandomSpawnPoint()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("스폰 포인트가 설정되지 않았습니다!");
            return Vector3.zero;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex].position;
    }

    // 패키지 수집 후 호출 (GameManager에서 호출)
    public void OnPackageCollected()
    {
        // 2초 후 다시 스폰
        StartCoroutine(RespawnAfterDelay());
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnPackage();
    }
}

