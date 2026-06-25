using UnityEngine;

public class BallScaler : MonoBehaviour, IInteractable
{
    public ScaleType scaleType = ScaleType.Grow; // Inspector'dan ayarlanabilir

    public float sizeMultiplier = 1.5f; // Inspector'dan ayarlanabilir

    private void Start()
    {
        // currentLevel yerine LevelManager.Instance.currentLevel kullanýyoruz
        switch (scaleType)
        {
            case ScaleType.Grow:
                sizeMultiplier = LevelManager.Instance.currentLevel.magnifyMultiplier;
                break;
            case ScaleType.Shrink:
                sizeMultiplier = LevelManager.Instance.currentLevel.shrinkMultiplier;
                break;
        }
    }

    public void Interact(BallController ball)
    {
        ball.ChangeSize(sizeMultiplier);
        Destroy(gameObject);
    }

    public enum ScaleType
    {
        Grow,
        Shrink
    }
}