using UnityEngine;

public class BallController : MonoBehaviour
{
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
        transform.localScale *= multiplier;
    }

    public void Die()
    {
        // Ölüm efekti, oyunu durdurma veya restart paneli tetiklemeleri
        Debug.Log("Top Patladý!");
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