using UnityEngine;

public class Package : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PackageData packageData; // ScriptableObject 참조

    [Header("Settings")]
    [SerializeField] private float magnetRange = 3f;
    [SerializeField] private float magnetSpeed = 5f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem collectEffect;
    [SerializeField] private AudioClip collectSound;

    // Components
    private Transform player;
    private bool isCollected = false;
    private Rigidbody rb;
    private Renderer meshRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<Renderer>();

        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // ===== PackageData 적용 =====
        if (packageData != null)
        {
            ApplyPackageData();
        }
        // ===========================
    }

    void ApplyPackageData()
    {
        // Material 색상 변경
        if (meshRenderer != null)
        {
            meshRenderer.material.color = packageData.packageColor;
        }

        // Rigidbody 무게 적용
        if (rb != null)
        {
            rb.mass = packageData.weight;
        }

        Debug.Log($"Package Type: {packageData.packageName}, Score: {packageData.scoreValue}");
    }

    void Update()
    {
        if (isCollected || player == null) return;

        // 자력 효과
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < magnetRange)
        {
            MagnetEffect();
        }
    }

    void MagnetEffect()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * magnetSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            Collect();
        }
    }

    void Collect()
    {
        isCollected = true;

        // ===== PackageData의 점수 사용 =====
        if (GameManager.Instance != null)
        {
            if (packageData != null)
            {
                // 패키지 종류에 따라 다른 점수 획득
                GameManager.Instance.CollectPackage(packageData.scoreValue);
                Debug.Log($"{packageData.packageName} 수집! +{packageData.scoreValue}점");
            }
            else
            {
                GameManager.Instance.CollectPackage(); // 기본 수집
            }
        }
        // ===================================

        // 이펙트 재생
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // 사운드 재생 (AudioManager 사용)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPackageCollect();
        }
        // 로컬 사운드가 있으면 추가 재생 (선택사항)
        else if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // 오브젝트 비활성화
        gameObject.SetActive(false);
    }

    public void ResetPackage()
    {
        isCollected = false;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        gameObject.SetActive(true);
    }

    // 외부에서 PackageData 설정 (PackageSpawner에서 사용)
    public void SetPackageData(PackageData data)
    {
        packageData = data;
        if (packageData != null && meshRenderer != null)
        {
            ApplyPackageData();
        }
    }
}