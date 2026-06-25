using UnityEngine;

public class HazardItem : MonoBehaviour, IInteractable
{
    public HazardType hazardType = HazardType.Spike;

    private ButtonSkillManager buttonSkillManager;

    private void Start()
    {
        buttonSkillManager = FindAnyObjectByType<ButtonSkillManager>();
    }
    public void Interact(BallController ball)
    {
        if (buttonSkillManager != null)
        {
            if (!buttonSkillManager.isShieldActive)
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
        PolygonCollider2D spikeCol = gameObject.GetComponent<PolygonCollider2D>();
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