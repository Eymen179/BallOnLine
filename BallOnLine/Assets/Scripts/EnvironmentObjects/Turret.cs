using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Turret : MonoBehaviour
{
    [Header("Referanslar")]
    [Tooltip("Geri tepecek olan namlu (Barrel) objesini buraya sürükleyin.")]
    public Transform barrel;

    [Tooltip("Merminin namludan çýkacaðý nokta (Boþ obje)")]
    public Transform firePoint;

    [Tooltip("Ateþlenecek mermi (Projectile) Prefab'ý")]
    public GameObject projectilePrefab;

    [Header("Animasyon (Juice) Ayarlarý")]
    public float recoilDistance = 0.3f;
    public float recoilDuration = 0.2f;

    // --- YENÝ EKLENEN DEÐÝÞKEN ---
    [Header("Ateþleme Ayarlarý")]
    [Tooltip("Oyun baþladýðýnda bu taretin ne kadar geç ateþe baþlayacaðý (Saniye)")]
    public float initialDelay = 0f;
    // ----------------------------

    private Vector3 originalBarrelPos;

    void Start()
    {
        if (barrel != null)
        {
            originalBarrelPos = barrel.localPosition;
        }

        StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        // --- YENÝ EKLENEN KISIM ---
        // Eðer Inspector üzerinden bir gecikme deðeri verildiyse, sonsuz döngüye girmeden önce bir defaya mahsus bekle.
        if (initialDelay > 0f)
        {
            yield return new WaitForSeconds(initialDelay);
        }
        // --------------------------

        while (true)
        {
            yield return new WaitForSeconds(LevelManager.Instance.currentLevel.fireInterval);
            Fire();
        }
    }

    void Fire()
    {
        if (barrel == null) return;

        barrel.DOKill();
        barrel.localPosition = originalBarrelPos;

        Sequence recoilSeq = DOTween.Sequence();

        recoilSeq.Append(barrel.DOLocalMoveX(originalBarrelPos.x + recoilDistance, recoilDuration * 0.3f)
                 .SetEase(Ease.OutExpo));
        recoilSeq.Append(barrel.DOLocalMoveX(originalBarrelPos.x, recoilDuration * 0.7f)
                 .SetEase(Ease.OutSine));

        if (projectilePrefab != null && firePoint != null)
        {
            AudioManager.Instance.PlayFireAudio(transform.position);

            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null && LevelManager.Instance != null && LevelManager.Instance.currentLevel != null)
            {
                rb.linearVelocity = firePoint.up * LevelManager.Instance.currentLevel.shootingSpeed;
            }

            Destroy(bullet, 5f);
        }
    }

    private void OnDestroy()
    {
        if (barrel != null) barrel.DOKill();
    }
}