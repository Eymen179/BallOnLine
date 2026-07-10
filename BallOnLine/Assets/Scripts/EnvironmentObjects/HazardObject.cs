using UnityEngine;

public class HazardItem : MonoBehaviour, IInteractable
{
    public HazardType hazardType = HazardType.Spike;

    private PolygonCollider2D spikeCol;

    private void Start()
    {
        spikeCol = GetComponent<PolygonCollider2D>();
    }
    public void Interact(BallController ball)
    {
        if (ButtonSkillManager.Instance != null)
        {
            if (!ButtonSkillManager.Instance.isShieldActive)
            {
                if (hazardType == HazardType.Spike)
                {
                    IsSpikeTrigger(true);
                }
                ball.Die();
            }
            else
            {
                if(hazardType == HazardType.Spike)
                {
                    IsSpikeTrigger(false);
                }
            }
            // --- YENÝ EKLENEN KISIM ---
            // Eðer objemiz mermiyse, topa çarptýðý an (kalkan olsa da olmasa da) yok olsun.
            if (hazardType == HazardType.Projectile)
            {
                ball.Die();
            }
            // -------------------------
        }
    }
    public void IsSpikeTrigger(bool isTrigger)
    {
        if (spikeCol != null)
        {
            spikeCol.isTrigger = isTrigger;
        }
    }

    // --- DUVARA/ZEMÝNE ÇARPMA KONTROLÜ (YENÝ EKLENDÝ) ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Sadece mermi ise bu kontrolü yapýyoruz (Dikenlerin duvarda durmasý gerekir)
        if (hazardType == HazardType.Projectile)
        {
            // 1. Çarptýðýmýz obje top DEÐÝLSE (Çünkü topla çarpýþmayý zaten üstteki Interact() hallediyor)
            if (collision.GetComponent<BallController>() == null)
            {
                // 2. Çarptýðýmýz obje bir Trigger DEÐÝLSE (Yani katý bir zemin veya duvar ise)
                // Bu sayede mermi LevelBounds'a, Altýnlara veya Portala çarpýnca yanlýþlýkla yok olmaz!
                if (!collision.isTrigger)
                {
                    // Duvara çarptýðý anda mermiyi yok et
                    Destroy(gameObject);

                    // ÝPUCU: Ýleride buraya "duvara çarpma partikül efekti" (Instantiate) ekleyebilirsin.
                }
            }
        }
    }

    public enum HazardType
    {
        Spike,
        Projectile
    }
}