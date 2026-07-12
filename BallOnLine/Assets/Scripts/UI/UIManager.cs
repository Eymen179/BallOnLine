using DG.Tweening;
using System;
using TMPro;
using TMPro.EditorUtilities;
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
    public TextMeshProUGUI txtStarAmount;
    public GameObject starCounter;

    public Slider inkAmountBar;

    [Header("In Game UI - Bottom")]
    public TextMeshProUGUI txtFreezeAmount;
    public TextMeshProUGUI txtShieldAmount;

    public GameObject pnlBottomUIBlocker;

    [Header("Before Level Start")]
    public GameObject btnStartLevel;

    [Header("Level Finish")]
    public Image star1;
    public Image star2;
    public Image star3;
    public GameObject pnlTimeTable;
    public Button btnTimeTable;

    public TextMeshProUGUI txtResultTime;
    [Header("Time Table Texts")]
    public TextMeshProUGUI txtTarget3Stars;
    public TextMeshProUGUI txtTarget2Stars;
    public TextMeshProUGUI txtTarget1Star;


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
        if(pnlBottomUIBlocker != null)
        {
            pnlBottomUIBlocker.SetActive(true);
        }

        star1.gameObject.SetActive(false);
        star2.gameObject.SetActive(false);
        star3.gameObject.SetActive(false);

        starCounter.SetActive(false);
        txtTimer.gameObject.SetActive(true);

        // Level baþladýðýnda TimeTable'daki hedef süre yazýlarýný otomatik doldur
        if (LevelManager.Instance.currentLevel != null)
        {
            var level = LevelManager.Instance.currentLevel;

            if (txtTarget3Stars != null)
                txtTarget3Stars.text = TimeSpan.FromSeconds(level.timeForThreeStars).ToString(@"mm\:ss\:ff");

            if (txtTarget2Stars != null)
                txtTarget2Stars.text = TimeSpan.FromSeconds(level.timeForTwoStars).ToString(@"mm\:ss\:ff");

            if (txtTarget1Star != null)
                txtTarget1Star.text = TimeSpan.FromSeconds(level.timeForOneStar).ToString(@"mm\:ss\:ff");
        }
    }

    // Portal scriptinden çaðýracaðýmýz Yýldýz Hesaplama Metodu
    public void CalculateAndShowStars(float finalTime)
    {
        var level = LevelManager.Instance.currentLevel;

        // Önce her ihtimale karþý tüm dolu yýldýzlarý kapatalým (Zaten baþtan kapalý ama garanti olsun)
        star1.gameObject.SetActive(false);
        star2.gameObject.SetActive(false);
        star3.gameObject.SetActive(false);

        // Þartlarý kontrol edip uygun yýldýzlarý aktif ediyoruz
        if (finalTime <= level.timeForThreeStars)
        {
            // 3 Yýldýz
            star1.gameObject.SetActive(true);
            star2.gameObject.SetActive(true);
            star3.gameObject.SetActive(true);
        }
        else if (finalTime <= level.timeForTwoStars)
        {
            // 2 Yýldýz (Soldan ve ortadan 2 tanesi)
            star1.gameObject.SetActive(true);
            star2.gameObject.SetActive(true);
        }
        else if (finalTime <= level.timeForOneStar)
        {
            // 1 Yýldýz (Sadece en soldaki)
            star1.gameObject.SetActive(true);
        }
        // Eðer süre timeForOneStar'dan da büyükse hiçbiri SetActive(true) olmaz, 0 yýldýz alýr.
    }

    // Paneli yaylanarak (OutBack) açar
    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero; // Önce 0 boyutuna al

        // 0.4 saniyede yaylanarak 1 (orijinal) boyutuna getir
        // SetUpdate(true) komutu, oyun dondurulsa bile (TimeScale = 0) animasyonun oynamasýný saðlar
        panel.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    // Paneli içine çökerek (InBack) kapatýr
    public void ClosePanel(GameObject panel)
    {
        // Önce 0.3 saniyede 0 boyutuna küçült, iþlem bitince (OnComplete) objeyi tamamen kapat
        panel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
        {
            panel.SetActive(false);
        });
    }
}
