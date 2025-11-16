using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float bounceForce = 15f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem bounceEffect;
    [SerializeField] private AudioClip bounceSound;

    void OnTriggerEnter(Collider other)
    {
        // 플레이어가 트램펄린에 닿았는지 체크
        if (other.CompareTag("Player"))
        {
            Bounce(other.gameObject);
        }
    }

    void Bounce(GameObject player)
    {
        // PlayerController 찾기
        PlayerController playerController = player.GetComponent<PlayerController>();

        if (playerController != null)
        {
            // PlayerController에 점프 강제 적용
            playerController.ForceJump(bounceForce);
            Debug.Log("Trampoline Bounce!");

            // 이펙트
            if (bounceEffect != null)
            {
                bounceEffect.Play();
            }

            // 사운드 (AudioManager 우선 사용)
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayTrampolineBounce();
            }
            else if (bounceSound != null)
            {
                AudioSource.PlayClipAtPoint(bounceSound, transform.position);
            }
        }
    }
}
