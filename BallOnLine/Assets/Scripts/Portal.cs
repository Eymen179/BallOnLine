using UnityEngine;

public class Portal : MonoBehaviour, IInteractable
{
    public void Interact(BallController ball)
    {
        Destroy(ball.gameObject);

        UIManager.Instance.pnlWinMenu.SetActive(true);
    }

}
