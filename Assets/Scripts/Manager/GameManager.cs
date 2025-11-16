using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private int targetPackageCount = 15;
    [SerializeField] private float timeLimit = 180f; // 3분

    [Header("References")]
    [SerializeField] private PackageSpawner packageSpawner;

    [Header("Sky Colors")]
    [SerializeField] private Color morningColor = new Color(0.5f, 0.7f, 1f); // 파란색
    [SerializeField] private Color eveningColor = new Color(1f, 0.5f, 0.2f); // 주황색
    [SerializeField] private Light directionalLight;

    // 게임 상태
    public enum GameState { Playing, Cleared, GameOver }
    private GameState currentState = GameState.Playing;

    // 게임 데이터
    private int collectedCount = 0;
    private float remainingTime;
    private float bestTime = 999f;
    private float gameTime = 0f;
    private int totalScore = 0;

    // Properties
    public int CollectedCount => collectedCount;
    public int TargetCount => targetPackageCount;
    public float RemainingTime => remainingTime;
    public GameState CurrentState => currentState;
    public float BestTime => bestTime;
    public float GameTime => gameTime;
    public int TotalScore => totalScore;

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        remainingTime = timeLimit;
        currentState = GameState.Playing;
        gameTime = 0f;

        Debug.Log($"Game Start! Target: {targetPackageCount} packages in {timeLimit} seconds");
        bestTime = PlayerPrefs.GetFloat("BestTime", 999f);
        Debug.Log($"현재 최고 기록: {bestTime:F2}초");

        // Directional Light 자동 찾기
        if (directionalLight == null)
        {
            directionalLight = FindObjectOfType<Light>();
        }
    }

    void Update()
    {
        if (currentState != GameState.Playing) return;

        UpdateTimer();
        gameTime += Time.deltaTime;
    }

    void UpdateTimer()
    {
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            GameOver();
        }
    }

    // 패키지 수집 시 호출
    public void CollectPackage(int score = 1)
    {
        if (currentState != GameState.Playing) return;

        collectedCount++;
        totalScore += score; // 점수 누적

        Debug.Log($"Collected: {collectedCount}/{targetPackageCount}, Score: +{score}");

        if (packageSpawner != null)
        {
            packageSpawner.OnPackageCollected();
        }

        UpdateSkyColor();
        CheckClearCondition();
    }

    void CheckClearCondition()
    {
        if (collectedCount >= targetPackageCount)
        {
            GameClear();
        }
    }

    void GameClear()
    {
        currentState = GameState.Cleared;
        float clearTime = timeLimit - remainingTime; // 걸린 시간

        if (clearTime < bestTime)
        {
            bestTime = clearTime;
            PlayerPrefs.SetFloat("BestTime", bestTime);
            PlayerPrefs.Save();
            Debug.Log($"🎉 신기록! {bestTime:F2}초");
        }
        Debug.Log("🎉 Game Clear!");

        // 승리 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameWin();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowClearPanel(clearTime, bestTime);
        }

        // 마우스 커서 표시 (버튼 클릭을 위해)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 시간 정지
        Time.timeScale = 0f;
    }

    void GameOver()
    {
        currentState = GameState.GameOver;
        Debug.Log("⏰ Time Over! Game Over!");

        // 패배 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameLose();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverPanel();
        }

        // 마우스 커서 표시 (버튼 클릭을 위해)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 시간 정지
        Time.timeScale = 0f;
    }

    // 재시작
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 게임 종료
    public void QuitGame()
    {
        Time.timeScale = 1f; // 시간 복원
        Debug.Log("Quit Game");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    void UpdateSkyColor()
    {
        float progress = (float)collectedCount / targetPackageCount;

        // 아침 → 저녁으로 색상 변화
        Color skyColor = Color.Lerp(morningColor, eveningColor, progress);

        // Ambient Light 색상 변경
        RenderSettings.ambientLight = skyColor;

        // Directional Light 색상 변경
        if (directionalLight != null)
        {
            directionalLight.color = skyColor;
        }
    }
}