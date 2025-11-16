using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float lifetime = 5f;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffect;

    void Start()
    {
        // 일정 시간 후 자동 삭제
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어 명중
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }

            // 명중 사운드
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBulletHit();
            }

            // 명중 이펙트
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            Debug.Log("[Bullet] 플레이어 명중!");

            // 총알 제거
            Destroy(gameObject);
        }
        // 지형 충돌
        else if (other.CompareTag("Ground") || other.CompareTag("Building"))
        {
            // 명중 이펙트
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            // 총알 제거
            Destroy(gameObject);
        }
    }
}
