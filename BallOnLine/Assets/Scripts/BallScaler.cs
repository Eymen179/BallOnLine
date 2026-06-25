using UnityEngine;

public class BallScaler : MonoBehaviour, IInteractable
{
    public ScaleType scaleType = ScaleType.Grow; // Inspector'dan ayarlanabilir

    public float sizeMultiplier = 1.5f; // Inspector'dan ayarlanabilir

    public void Interact(BallController ball)
    {
        // 1. Topu büyüt
        ball.ChangeSize(sizeMultiplier);

        // 2. Efekt oynat (Opsiyonel)
        // ParticleManager.PlayGrowEffect(transform.position);

        // 3. Kendini yok et (veya Object Pool'a geri gönder)
        Destroy(gameObject);
    }

    public enum ScaleType
    {
        Grow,
        Shrink
    }
}