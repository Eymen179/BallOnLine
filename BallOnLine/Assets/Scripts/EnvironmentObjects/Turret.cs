using UnityEngine;
using DG.Tweening; // DOTween kütüphanesi
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
        while (true)
        {
            yield return new WaitForSeconds(LevelManager.Instance.currentLevel.fireInterval);
            Fire();
        }
    }

    void Fire()
    {
        if (barrel == null) return;

        // EÐER ÇOK HIZLI ATEÞ EDÝLÝYORSA: Önceki animasyonu ezmemesi için DOTween'i durdurup namluyu sýfýrla
        barrel.DOKill();
        barrel.localPosition = originalBarrelPos;

        // --- GERÝ TEPME ANÝMASYONU ---
        Sequence recoilSeq = DOTween.Sequence();

        recoilSeq.Append(barrel.DOLocalMoveX(originalBarrelPos.x + recoilDistance, recoilDuration * 0.3f)
                 .SetEase(Ease.OutExpo));
        recoilSeq.Append(barrel.DOLocalMoveX(originalBarrelPos.x, recoilDuration * 0.7f)
                 .SetEase(Ease.OutSine));

        // --- MERMÝ ÜRETÝMÝ VE HAREKETÝ ---
        if (projectilePrefab != null && firePoint != null)
        {
            // Mermiyi namlu ucunda (firePoint) üret
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // Mermiye fiziksel bir hýz ver (Level.cs içindeki shootingSpeed'i çekiyoruz)
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null && LevelManager.Instance != null && LevelManager.Instance.currentLevel != null)
            {
                // firePoint.right = Namlunun baktýðý yön (X ekseni)
                rb.linearVelocity = firePoint.up * LevelManager.Instance.currentLevel.shootingSpeed;
            }

            // RAM Tasarrufu: Mermi bir yere çarpmazsa 5 saniye sonra sahneden silinsin
            Destroy(bullet, 5f);
        }
    }

    private void OnDestroy()
    {
        if (barrel != null) barrel.DOKill();
    }
}