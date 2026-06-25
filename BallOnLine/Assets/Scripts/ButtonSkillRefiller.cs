using TMPro;
using UnityEngine;

public class ButtonSkillRefiller : MonoBehaviour, IInteractable
{
    public SkillType skillType = SkillType.Freeze;

    public TextMeshProUGUI txtRefillAmount;
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

        // Yazý güncellemesini deðerler atandýktan SONRA yapmalýyýz
        txtRefillAmount.text = "+" + refillAmount.ToString();
    }

    public void Interact(BallController ball)
    {
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