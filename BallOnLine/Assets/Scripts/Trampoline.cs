using UnityEngine;
using DG.Tweening; // DOTween kütüphanesi

public class Trampoline : MonoBehaviour
{
    [Header("Animasyon Ayarlarý")]
    [Tooltip("Trambolinin çarpma anýnda ne kadar içeri göçeceði")]
    public float punchDistance = 0.3f;

    [Tooltip("Animasyonun toplam süresi")]
    public float duration = 0.4f;

    [Tooltip("Ýleri-geri titreme (yaylanma) sayýsý")]
    public int vibrato = 5;

    [Tooltip("Esneklik hissiyatý (0 ile 1 arasý)")]
    public float elasticity = 0.5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Çarpan obje top mu diye kontrol ediyoruz
        BallController ball = collision.gameObject.GetComponent<BallController>();

        if (ball != null)
        {
            // Olasý pozisyon kaymalarýný önlemek için, eðer halihazýrda çalan bir 
            // trambolin animasyonu varsa onu anýnda bitirip orijinal konumuna döndürürüz.
            transform.DOComplete();

            // 1. YÖNÜ BULMA:
            // GetContact(0).normal bize çarpýþma yüzeyinden topa doðru (dýþarý) bakan vektörü verir.
            // Trambolinin topun baskýsýyla 'içeri' çökmesini istediðimiz için bu vektörün tersini (-normal) alýyoruz.
            Vector2 impactNormal = collision.GetContact(0).normal;
            Vector3 punchDirection = (Vector3)(-impactNormal) * punchDistance;

            // 2. DOTWEEN ÝLE YAYLANMA (PUNCH) ANÝMASYONU:
            // Belirlenen yönde ileri fýrlayýp geri sekerek yaylanma hissi verir.
            transform.DOPunchPosition(punchDirection, duration, vibrato, elasticity);
        }
    }
}