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
            // Kalkan durumunu bir deðiþkene alalým ki kod daha okunaklý olsun
            bool hasShield = ButtonSkillManager.Instance.isShieldActive;

            if (!hasShield)
            {
                // --- KALKAN YOKSA ---
                if (hazardType == HazardType.Spike)
                {
                    IsSpikeTrigger(true);
                }

                // Kalkan yoksa top her halükarda ölür
                ball.Die();

                // Eðer objemiz mermiyse, topu patlattýktan sonra mermi de yok olsun
                // (Ölü topun içinden hayalet gibi geçip gitmemesi için)
                if (hazardType == HazardType.Projectile)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                // --- KALKAN VARSA ---
                if (hazardType == HazardType.Spike)
                {
                    // Diken katýlaþýr, top üzerinde rahatça gezer
                    IsSpikeTrigger(false);
                }
                else if (hazardType == HazardType.Projectile)
                {
                    // Mermi kalkanlý topa çarptý!
                    // Top ölmez (ball.Die çalýþmaz), sadece mermi duvara çarpmýþ gibi yok olur!
                    Destroy(gameObject);
                }
            }
        }
    }

    public void IsSpikeTrigger(bool isTrigger)
    {
        if (spikeCol != null)
        {
            spikeCol.isTrigger = isTrigger;
        }
    }

    // --- DUVARA/ZEMÝNE ÇARPMA KONTROLÜ ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hazardType == HazardType.Projectile)
        {
            if (collision.GetComponent<BallController>() == null)
            {
                if (!collision.isTrigger)
                {
                    Destroy(gameObject);
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