using TMPro;
using UnityEngine;

public class ButtonSkillRefiller : MonoBehaviour, IInteractable
{
    public SkillType skillType = SkillType.Freeze;

    private int refillAmount = 1;

    private void Start()
    {
        // Deðerleri LevelManager üzerinden okuyoruz
        switch (skillType)
        {
            case SkillType.Freeze:
                refillAmount = LevelManager.Instance.currentLevel.FreezeRefillCount;
                break;
            case SkillType.Shield:
                refillAmount = LevelManager.Instance.currentLevel.ShieldRefillCount;
                break;
        }
    }

    public void Interact(BallController ball)
    {
        AudioManager.Instance.PlayAudioClip("Sound_RefillerPickup");
        switch (skillType)
        {
            case SkillType.Freeze:
                ButtonSkillManager.Instance.UpdateFreezeCount(refillAmount);
                break;
            case SkillType.Shield:
                ButtonSkillManager.Instance.UpdateShieldCount(refillAmount);
                break;
        }

        Destroy(gameObject);
    }

    public enum SkillType
    {
        Freeze,
        Shield
    }
}