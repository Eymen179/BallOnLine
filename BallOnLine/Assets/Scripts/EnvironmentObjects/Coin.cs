using UnityEngine;

public class Coin : MonoBehaviour, IInteractable
{
    public void Interact(BallController ball)
    {
        CoinManager.Instance.UpdateCoinAmount(1);
        Destroy(gameObject);
    }


}
