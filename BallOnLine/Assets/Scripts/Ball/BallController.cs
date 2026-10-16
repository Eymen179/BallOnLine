using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Effects & References")]
    public GameObject explosionPrefab;
    [Tooltip("Boþ býrakýrsan alt objelerdeki (Child) TrailRenderer'ý otomatik bulur.")]
    public TrailRenderer ballTrail;

    private int currentSizeStep = 0;

    private void Start()
    {
        // Eðer Inspector'dan atanmadýysa alt objedeki Trail'i otomatik bul
        if (ballTrail == null)
        {
            ballTrail = GetComponentInChildren<TrailRenderer>();
        }

        if (SkinManager.Instance != null)
        {
            // 1. Seçili TOP (Ball) Skinini Uygula
            ShopItemSO equippedBall = SkinManager.Instance.GetEquippedBallSkin();
            if (equippedBall != null)
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    if (equippedBall.shopItemMaterial != null)
                        spriteRenderer.material = equippedBall.shopItemMaterial;

                    spriteRenderer.material.color = equippedBall.shopItemColor;
                }
            }

            // 2. Seçili ÝZ (Trail) Skinini Uygula
            ShopItemSO equippedTrail = SkinManager.Instance.GetEquippedTrailSkin();
            if (equippedTrail != null && ballTrail != null)
            {
                equippedTrail.ApplyToTrailRenderer(ballTrail);
            }
        }
    }

    public void ChangeSize(float multiplier)
    {
        if (multiplier < 1f) // Küçültücü
        {
            AudioManager.Instance.PlayAudioClip("Sound_Shrinker");

            if (currentSizeStep > -2)
            {
                currentSizeStep--;
                transform.localScale *= multiplier;

                // Top küçüldüðünde arkasýndaki izin kalýnlýðý da orantýlý küçülsün
                if (ballTrail != null) ballTrail.widthMultiplier *= multiplier;
            }
        }
        else // Büyütücü
        {
            AudioManager.Instance.PlayAudioClip("Sound_Magnifyer");

            if (currentSizeStep < 2)
            {
                currentSizeStep++;
                transform.localScale *= multiplier;

                // Top büyüdüðünde arkasýndaki izin kalýnlýðý da orantýlý büyüsün
                if (ballTrail != null) ballTrail.widthMultiplier *= multiplier;
            }
        }
    }

    public void Die()
    {
        Debug.Log("Top Patladý!");

        if (VibrationManager.Instance != null) VibrationManager.Instance.Vibrate();

        AudioManager.Instance.PlayAudioClip("Sound_BallExplosion");

        DrawingManager.Instance.isGameActive = false;

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = GetComponent<SpriteRenderer>().material.color;
            }
        }

        gameObject.SetActive(false);

        UIManager.Instance.OpenPanel(UIManager.Instance.pnlDeathMenu);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)
        {
            interactable.Interact(this);
        }
    }
}