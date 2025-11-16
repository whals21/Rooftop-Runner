using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance { get; private set; }

    [Header("Life Settings")]
    [SerializeField] private int maxLives = 3;
    private int currentLives;

    [Header("Respawn Settings")]
    [SerializeField] private Vector3 respawnPosition = new Vector3(0, 2, 0);
    [SerializeField] private float respawnDelay = 1.5f;

    public int CurrentLives => currentLives;
    public int MaxLives => maxLives;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentLives = maxLives;
        Debug.Log($"게임 시작! 라이프: {currentLives}/{maxLives}");
    }

    public void LoseLife(GameObject player)
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
        {
            return; // 게임이 끝났으면 라이프 감소 안 함
        }

        currentLives--;
        Debug.Log($"라이프 감소! 남은 라이프: {currentLives}/{maxLives}");

        // UI 업데이트
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateLives(currentLives, maxLives);
        }

        if (currentLives <= 0)
        {
            // 게임 오버
            GameOver();
        }
        else
        {
            // 리스폰
            StartCoroutine(RespawnPlayer(player));
        }
    }

    System.Collections.IEnumerator RespawnPlayer(GameObject player)
    {
        yield return new WaitForSeconds(respawnDelay);

        // 플레이어 위치 초기화
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = respawnPosition;
            controller.enabled = true;
        }
        else
        {
            player.transform.position = respawnPosition;
        }

        // PlayerHealth 리셋
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Respawn();
        }

        Debug.Log("플레이어 리스폰!");
    }

    void GameOver()
    {
        Debug.Log("💀 모든 라이프 소진! Game Over!");

        if (GameManager.Instance != null)
        {
            // GameManager의 GameOver 호출하지 않고 직접 처리
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOverPanel();
            }

            // 마우스 커서 표시
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // 패배 사운드
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayGameLose();
            }

            // 시간 정지
            Time.timeScale = 0f;
        }
    }

    public void ResetLives()
    {
        currentLives = maxLives;
    }
}
