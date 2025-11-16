using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 사용

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("In-Game UI")]
    [SerializeField] private TextMeshProUGUI collectedText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI livesText;

    [Header("Panels")]
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private TextMeshProUGUI clearTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Clear Panel Buttons")]
    [SerializeField] private Button clearRestartButton;
    [SerializeField] private Button clearQuitButton;

    [Header("GameOver Panel Buttons")]
    [SerializeField] private Button gameOverRestartButton;
    [SerializeField] private Button gameOverQuitButton;

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
        SetupButtons();
    }

    void SetupButtons()
    {
        // Clear Panel 버튼 연결
        if (clearRestartButton != null)
        {
            clearRestartButton.onClick.AddListener(() => {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.RestartGame();
                }
            });
        }

        if (clearQuitButton != null)
        {
            clearQuitButton.onClick.AddListener(() => {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.QuitGame();
                }
            });
        }

        // GameOver Panel 버튼 연결
        if (gameOverRestartButton != null)
        {
            gameOverRestartButton.onClick.AddListener(() => {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.RestartGame();
                }
            });
        }

        if (gameOverQuitButton != null)
        {
            gameOverQuitButton.onClick.AddListener(() => {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.QuitGame();
                }
            });
        }
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (GameManager.Instance == null) return;

        // 점수 업데이트
        UpdateScore(GameManager.Instance.CollectedCount, GameManager.Instance.TargetCount);

        // 타이머 업데이트
        UpdateTimer(GameManager.Instance.RemainingTime);

        // 라이프 업데이트
        if (LifeManager.Instance != null)
        {
            UpdateLives(LifeManager.Instance.CurrentLives, LifeManager.Instance.MaxLives);
        }
    }

    void UpdateScore(int collected, int total)
    {
        if (collectedText != null)
        {
            collectedText.text = $"📦 수집: {collected}/{total}";
        }
    }

    void UpdateTimer(float timeInSeconds)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60);
            timerText.text = $"⏰ 남은시간: {minutes}:{seconds:00}";

            // 시간이 30초 미만이면 빨간색으로 경고
            if (timeInSeconds < 30f)
            {
                timerText.color = Color.red;
            }
            else
            {
                timerText.color = Color.yellow;
            }
        }
    }

    public void UpdateLives(int current, int max)
    {
        if (livesText != null)
        {
            livesText.text = $"라이프: {current}";

            // 라이프가 1개 이하면 빨간색으로 경고
            if (current <= 1)
            {
                livesText.color = Color.red;
            }
            else
            {
                livesText.color = Color.white;
            }
        }
    }

    public void ShowClearPanel(float clearTime, float bestTime)
    {
        if (clearPanel != null)
        {
            clearPanel.SetActive(true);

            // 클리어 시간 표시
            if (clearTimeText != null)
            {
                int minutes = Mathf.FloorToInt(clearTime / 60);
                int seconds = Mathf.FloorToInt(clearTime % 60);
                clearTimeText.text = $"클리어 타임: {minutes}:{seconds:00}";
            }

            // 최고 기록 표시
            if (bestTimeText != null)
            {
                int minutes = Mathf.FloorToInt(bestTime / 60);
                int seconds = Mathf.FloorToInt(bestTime % 60);
                bestTimeText.text = $"최고 기록: {minutes}:{seconds:00}";
            }
        }
    }
    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}