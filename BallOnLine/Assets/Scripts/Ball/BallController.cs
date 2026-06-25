using UnityEngine;

public class BallController : MonoBehaviour
{
    public void ChangeSize(float multiplier)
    {
        transform.localScale *= multiplier;
    }

    public void Die()
    {
        // Ölüm efekti, oyunu durdurma veya restart paneli tetiklemeleri
        Debug.Log("Top Patladý!");
        gameObject.SetActive(false);

        UIManager.Instance.pnlDeathMenu.SetActive(true);
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