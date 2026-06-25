using UnityEngine;

public class InkBall : MonoBehaviour, IInteractable
{
    public void Interact(BallController ball)
    {
        // LevelManager üzerinden veri çekimi
        DrawingManager.inkAmount += LevelManager.Instance.currentLevel.inkBallAmount;

        if (DrawingManager.Instance != null)
        {
            DrawingManager.Instance.UpdateDrawingProgressBar();
        }

        Destroy(gameObject);
    }
}