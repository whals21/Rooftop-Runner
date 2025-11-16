using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static AudioManager Instance { get; private set; }

    [Header("Background Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private float musicVolume = 0.5f;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip packageCollectSound;
    [SerializeField] private AudioClip grappleShootSound;
    [SerializeField] private AudioClip gameWinSound;
    [SerializeField] private AudioClip gameLoseSound;
    [SerializeField] private AudioClip trampolineBounceSound;
    [SerializeField] private AudioClip sniperShootSound;
    [SerializeField] private AudioClip bulletHitSound;
    [SerializeField] private AudioClip playerDeathSound;
    [SerializeField] private float sfxVolume = 0.7f;

    // Audio Sources
    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // AudioSource 컴포넌트 생성
        SetupAudioSources();
    }

    void Start()
    {
        // 배경음악 자동 재생
        PlayBackgroundMusic();
    }

    void SetupAudioSources()
    {
        // 배경음악용 AudioSource
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;

        // 효과음용 AudioSource
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
            Debug.Log("배경음악 재생 시작");
        }
        else
        {
            Debug.LogWarning("배경음악이 할당되지 않았습니다. Inspector에서 AudioManager에 배경음악을 할당하세요.");
        }
    }

    // ===== 효과음 재생 메서드 =====

    public void PlayPackageCollect()
    {
        PlaySFX(packageCollectSound, "패키지 수집");
    }

    public void PlayGrappleShoot()
    {
        PlaySFX(grappleShootSound, "그래플 발사");
    }

    public void PlayGameWin()
    {
        PlaySFX(gameWinSound, "게임 승리");
    }

    public void PlayGameLose()
    {
        PlaySFX(gameLoseSound, "게임 패배");
    }

    public void PlayTrampolineBounce()
    {
        PlaySFX(trampolineBounceSound, "트램펄린 튕김");
    }

    public void PlaySniperShoot()
    {
        PlaySFX(sniperShootSound, "저격 발사");
    }

    public void PlayBulletHit()
    {
        PlaySFX(bulletHitSound, "총알 명중");
    }

    public void PlayPlayerDeath()
    {
        PlaySFX(playerDeathSound, "플레이어 사망");
    }

    // 범용 효과음 재생 (외부에서도 사용 가능)
    public void PlaySFX(AudioClip clip, string soundName = "효과음")
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
            Debug.Log($"[AudioManager] {soundName} 재생");
        }
        else if (clip == null)
        {
            Debug.LogWarning($"[AudioManager] {soundName} AudioClip이 할당되지 않았습니다.");
        }
    }

    // ===== 볼륨 조절 =====

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    // 배경음악 일시정지/재개
    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }

    // 배경음악 정지
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
}
