using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // [Header("Death Settings")]
    // [SerializeField] private float deathAnimationDuration = 1.5f;

    private bool isDead = false;
    private PlayerController playerController;
    private PlayerAnimationController playerAnimationController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerAnimationController = GetComponent<PlayerAnimationController>();
    }

    public void TakeDamage()
    {
        if (isDead) return;

        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
        {
            return; // 게임 끝나면 데미지 안 받음
        }

        Die();
    }

    void Die()
    {
        isDead = true;

        Debug.Log("플레이어 사망!");

        // 플레이어 조작 비활성화
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // 사망 애니메이션 재생
        if (playerAnimationController != null)
        {
            playerAnimationController.PlayDeathAnimation();
            Debug.Log("사망 애니메이션 재생 호출");
        }

        // 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerDeath();
        }

        // 라이프 감소
        if (LifeManager.Instance != null)
        {
            LifeManager.Instance.LoseLife(gameObject);
        }
    }

    public void Respawn()
    {
        isDead = false;

        // 플레이어 조작 활성화
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // 애니메이터 리셋
        if (playerAnimationController != null)
        {
            playerAnimationController.ResetAnimation();
        }

        Debug.Log("플레이어 부활!");
    }

    public bool IsDead => isDead;
}
