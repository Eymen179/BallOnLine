using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("In Game Menus")]
    public GameObject pnlPauseMenu;
    public GameObject pnlDeathMenu;
    public GameObject pnlWinMenu;

    [Header("In Game UI - Top")]
    public TextMeshProUGUI txtTimer;
    public TextMeshProUGUI txtCoinAmount;
    public Slider inkAmountBar;

    [Header("In Game UI - Bottom")]
    public TextMeshProUGUI txtFreezeAmount;
    public TextMeshProUGUI txtShieldAmount;

    [Header("Before Level Start")]
    public GameObject btnStartLevel;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        txtFreezeAmount.text = "x" +  ButtonSkillManager.freezeCount.ToString();
        txtShieldAmount.text = "x" + ButtonSkillManager.shieldCount.ToString();

        txtCoinAmount.text = "0";

        if (LevelManager.Instance.currentLevel != null)
        {
            if (LevelManager.Instance.currentLevel.isInkLimited)
            {
                inkAmountBar.gameObject.SetActive(true);
            }
            else
            {
                inkAmountBar.gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
