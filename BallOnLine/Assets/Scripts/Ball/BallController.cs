using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Effects")]
    public GameObject explosionPrefab;

    // --- YENÝ EKLENEN KISIM: Oyun baþladýðýnda seçili skini kuþanma ---
    private void Start()
    {
        // SkinManager sahnede var mý kontrol et
        if (SkinManager.Instance != null)
        {
            // Kayýtlý olan skini getir
            ShopItemSO equippedSkin = SkinManager.Instance.GetEquippedBallSkin();

            if (equippedSkin != null)
            {
                // Topun kendi üzerindeki Renderer'ý bul ve materyalini/rengini deðiþtir
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.material = equippedSkin.shopItemMaterial;
                    spriteRenderer.material.color = equippedSkin.shopItemColor;
                }
            }
        }
    }
    // ------------------------------------------------------------------

    public void ChangeSize(float multiplier)
    {
        if(multiplier < 1f)
        {
            AudioManager.Instance.PlayAudioClip("Sound_Shrinker");
        }
        else
        {
            AudioManager.Instance.PlayAudioClip("Sound_Magnifyer");
        }
            transform.localScale *= multiplier;
    }

    public void Die()
    {
        // Ölüm efekti, oyunu durdurma veya restart paneli tetiklemeleri
        Debug.Log("Top Patladý!");

        if (VibrationManager.Instance != null) VibrationManager.Instance.Vibrate();

        AudioManager.Instance.PlayAudioClip("Sound_BallExplosion");

        // --- PATLAMA EFEKTÝ ---
        if (explosionPrefab != null)
        {
            // 1. Patlamayý topun tam olduðu noktada (transform.position) yarat
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // 2. Patlama parçacýklarýnýn rengini, topumuzun o anki rengine eþitle (Market uyumu!)
            ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main; // Particle System'in ana ayarlarýna ulaþýyoruz
                main.startColor = GetComponent<SpriteRenderer>().material.color;
            }
        }
        // ----------------------

        gameObject.SetActive(false);

        UIManager.Instance.OpenPanel(UIManager.Instance.pnlDeathMenu);
    }

    // ÇARPIÞMA KONTROLÜ (Sadece 4 satýr!)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Çarptýðýmýz objede IInteractable arayüzü var mý?
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)
        {
            // Varsa, etkileþimi baþlat ve kendimi (this) gönder
            interactable.Interact(this);
        }
    }
}