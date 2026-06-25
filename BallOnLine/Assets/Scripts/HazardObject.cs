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
        }
    }
    public void IsSpikeTrigger(bool isTrigger)
    {
        if (spikeCol != null)
        {
            spikeCol.isTrigger = isTrigger;
        }
    }
    public enum HazardType
    {
        Spike,
        Projectile
    }
}