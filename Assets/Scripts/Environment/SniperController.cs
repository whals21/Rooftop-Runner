using UnityEngine;

public class SniperController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Sniper Settings")]
    [SerializeField] private float aimDuration = 0.3f; // 조준 시간
    [SerializeField] private float cooldownTime = 3f; // 재장전 시간
    [SerializeField] private float detectionRange = 50f; // 탐지 범위

    [Header("Laser Settings")]
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private Color laserColor = Color.red;
    [SerializeField] private float laserWidth = 0.05f;

    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 100f;
    [SerializeField] private Transform bulletSpawnPoint;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;

    // State
    private enum SniperState { Idle, Aiming, Cooldown }
    private SniperState currentState = SniperState.Idle;
    private float stateTimer = 0f;

    void Start()
    {
        // 플레이어 자동 찾기
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // LaserLine 설정
        if (laserLine != null)
        {
            laserLine.enabled = false;
            laserLine.startColor = laserColor;
            laserLine.endColor = laserColor;
            laserLine.startWidth = laserWidth;
            laserLine.endWidth = laserWidth;
            laserLine.positionCount = 2;
        }

        // BulletSpawnPoint 자동 설정
        if (bulletSpawnPoint == null)
        {
            bulletSpawnPoint = transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // 게임이 Playing 상태가 아니면 작동 안 함
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
        {
            if (laserLine != null)
            {
                laserLine.enabled = false;
            }
            return;
        }

        // 플레이어가 죽었으면 작동 안 함
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null && playerHealth.IsDead)
        {
            if (laserLine != null)
            {
                laserLine.enabled = false;
            }
            return;
        }

        // 플레이어 거리 체크
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange)
        {
            currentState = SniperState.Idle;
            if (laserLine != null)
            {
                laserLine.enabled = false;
            }
            return;
        }

        // State Machine
        switch (currentState)
        {
            case SniperState.Idle:
                StartAiming();
                break;

            case SniperState.Aiming:
                UpdateAiming();
                break;

            case SniperState.Cooldown:
                UpdateCooldown();
                break;
        }
    }

    void StartAiming()
    {
        currentState = SniperState.Aiming;
        stateTimer = 0f;

        // 레이저 활성화
        if (laserLine != null)
        {
            laserLine.enabled = true;
        }

        Debug.Log("[Sniper] 조준 시작!");
    }

    void UpdateAiming()
    {
        stateTimer += Time.deltaTime;

        // 저격수를 플레이어 방향으로 회전
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // 레이저 업데이트
        UpdateLaser();

        // 조준 시간이 지나면 발사
        if (stateTimer >= aimDuration)
        {
            Shoot();
        }
    }

    void UpdateLaser()
    {
        if (laserLine == null || player == null) return;

        Vector3 startPos = bulletSpawnPoint.position;
        Vector3 endPos = player.position + Vector3.up * 1f; // 플레이어 중심

        laserLine.SetPosition(0, startPos);
        laserLine.SetPosition(1, endPos);
    }

    void Shoot()
    {
        Debug.Log("[Sniper] 발사!");

        // 총구 이펙트
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // 발사 사운드
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySniperShoot();
        }

        // 총알 생성
        if (bulletPrefab != null)
        {
            Vector3 direction = (player.position + Vector3.up * 1f - bulletSpawnPoint.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(direction));

            // 총알에 속도 부여
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed;
            }
        }

        // 쿨다운 시작
        currentState = SniperState.Cooldown;
        stateTimer = 0f;

        // 레이저 비활성화
        if (laserLine != null)
        {
            laserLine.enabled = false;
        }
    }

    void UpdateCooldown()
    {
        stateTimer += Time.deltaTime;

        if (stateTimer >= cooldownTime)
        {
            currentState = SniperState.Idle;
        }
    }

    void OnDrawGizmosSelected()
    {
        // 탐지 범위 시각화
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
