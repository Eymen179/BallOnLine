using TMPro;
using UnityEngine;

public class ButtonSkillRefiller: MonoBehaviour, IInteractable
{
    public SkillType skillType = SkillType.Freeze;

    public int refillAmount = 1;
    public TextMeshProUGUI txtRefillAmount;

    private void Start()
    {
        txtRefillAmount.text = "+" + refillAmount.ToString();
    }
    public void Interact(BallController ball)
    {
        switch (skillType)
        {
            case SkillType.Freeze:
                ButtonSkillManager.Instance.UpdateFreezeAmount(refillAmount);
                break;
            case SkillType.Shield:
                ButtonSkillManager.Instance.UpdateShieldAmount(refillAmount);
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