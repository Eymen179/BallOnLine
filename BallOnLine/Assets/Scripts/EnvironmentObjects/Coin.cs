using UnityEngine;

public class Coin : MonoBehaviour, IInteractable
{
    public void Interact(BallController ball)
    {
        AudioManager.Instance.PlayAudioClip("Sound_CoinPickup");

        CoinManager.Instance.UpdateCoinAmount(1);
        Destroy(gameObject);
    }


}
