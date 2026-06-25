using UnityEngine;

public class InkBall : MonoBehaviour, IInteractable
{
    private DrawingManager drawingManager;

    private void Start()
    {
        drawingManager = FindAnyObjectByType<DrawingManager>();
    }
    public void Interact(BallController ball)
    {
        DrawingManager.inkAmount += 10f;
        if (drawingManager != null)
        {
            drawingManager.UpdateDrawingProgressBar();
        }
        Destroy(gameObject);
    }
}
