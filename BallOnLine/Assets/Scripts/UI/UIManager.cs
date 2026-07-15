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

    [Header("Animated Flying Stars")]
    public Image animatedStar1;
    public Image animatedStar2;
    public Image animatedStar3;

    [Header("Animated Flying Coins")]
    public GameObject animatedCoin1;
    public GameObject animatedCoin2;
    public GameObject animatedCoin3;

    // Coinlerin orijinal konumlarýný tutacaðýmýz deðiþkenler
    private Vector2 coin1OrigPos;
    private Vector2 coin2OrigPos;
    private Vector2 coin3OrigPos;

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
        txtFreezeAmount.text = "x" + ButtonSkillManager.freezeCount.ToString();
        txtShieldAmount.text = "x" + ButtonSkillManager.shieldCount.ToString();

        // --- YENÝ EKLENEN KISIM: Sahne açýldýðýnda verileri çeker ---
        if (CoinManager.Instance != null && txtCoinAmount != null)
        {
            txtCoinAmount.text = CoinManager.Instance.coinAmount.ToString();
        }

        if (StarManager.Instance != null && txtStarAmount != null)
        {
            txtStarAmount.text = StarManager.Instance.totalStars.ToString();
        }
        // -------------------------------------------------------------

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

        if (pnlBottomUIBlocker != null)
        {
            pnlBottomUIBlocker.SetActive(true);
        }

        star1.gameObject.SetActive(false);
        star2.gameObject.SetActive(false);
        star3.gameObject.SetActive(false);

        starCounter.SetActive(false);
        txtTimer.gameObject.SetActive(true);

        // Coinlerin UI içindeki orijinal (tasarladýðýn) pozisyonlarýný kaydet
        if (animatedCoin1 != null) coin1OrigPos = animatedCoin1.GetComponent<RectTransform>().anchoredPosition;
        if (animatedCoin2 != null) coin2OrigPos = animatedCoin2.GetComponent<RectTransform>().anchoredPosition;
        if (animatedCoin3 != null) coin3OrigPos = animatedCoin3.GetComponent<RectTransform>().anchoredPosition;

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

    // Metoda "int previousStars" parametresi eklendi
    public int CalculateAndShowStars(float finalTime, int previousStars)
    {
        var level = LevelManager.Instance.currentLevel;

        star1.gameObject.SetActive(false);
        star2.gameObject.SetActive(false);
        star3.gameObject.SetActive(false);

        if (animatedStar1 != null) animatedStar1.gameObject.SetActive(false);
        if (animatedStar2 != null) animatedStar2.gameObject.SetActive(false);
        if (animatedStar3 != null) animatedStar3.gameObject.SetActive(false);

        // BUNLARI EKLE:
        if (animatedCoin1 != null) animatedCoin1.SetActive(false);
        if (animatedCoin2 != null) animatedCoin2.SetActive(false);
        if (animatedCoin3 != null) animatedCoin3.SetActive(false);

        int earnedStars = 0;

        if (finalTime <= level.timeForThreeStars) earnedStars = 3;
        else if (finalTime <= level.timeForTwoStars) earnedStars = 2;
        else if (finalTime <= level.timeForOneStar) earnedStars = 1;

        if (earnedStars > 0)
        {
            star1.transform.localScale = Vector3.zero;
            star2.transform.localScale = Vector3.zero;
            star3.transform.localScale = Vector3.zero;

            if (earnedStars >= 1) star1.gameObject.SetActive(true);
            if (earnedStars >= 2) star2.gameObject.SetActive(true);
            if (earnedStars >= 3) star3.gameObject.SetActive(true);

            Sequence starSequence = DOTween.Sequence();
            starSequence.SetUpdate(true);

            starSequence.AppendInterval(0.4f);

            if (earnedStars >= 1)
            {
                // Büyüme animasyonu baþladýðý an (.OnStart) sesi çal
                starSequence.Append(star1.transform.DOScale(2f, 0.4f).SetEase(Ease.OutBack)
                    .OnStart(() => AudioManager.Instance.PlayAudioClip("Sound_StarAppear")));
            }

            if (earnedStars >= 2)
            {
                starSequence.AppendInterval(0.5f);
                starSequence.Append(star2.transform.DOScale(2f, 0.4f).SetEase(Ease.OutBack)
                    .OnStart(() => AudioManager.Instance.PlayAudioClip("Sound_StarAppear")));
            }

            if (earnedStars >= 3)
            {
                starSequence.AppendInterval(0.5f);
                starSequence.Append(star3.transform.DOScale(2f, 0.4f).SetEase(Ease.OutBack)
                    .OnStart(() => AudioManager.Instance.PlayAudioClip("Sound_StarAppear")));
            }

            // Orijinal yýldýzlarýn açýlmasý bittiðinde uçma metodunu çaðýr (Eski rekoru da gönderiyoruz)
            starSequence.OnComplete(() =>
            {
                StartFlyingStarsAnim(previousStars, earnedStars);
            });
        }

        return earnedStars;
    }

    private void StartFlyingStarsAnim(int previousStars, int earnedStars)
    {
        float delay = 0f;

        for (int i = previousStars + 1; i <= earnedStars; i++)
        {
            if (i == 1) FlySingleStar(animatedStar1, star1.transform.position, delay);
            if (i == 2) FlySingleStar(animatedStar2, star2.transform.position, delay);
            if (i == 3) FlySingleStar(animatedStar3, star3.transform.position, delay);

            delay += 0.2f;
        }

        // --- YENÝ EKLENEN KISIM ---
        // Yýldýz animasyonlarý bittikten sonra (0.6f saniye uçuþ süreleri var) Coin'leri uçur
        float coinStartDelay = delay + 0.6f;
        StartFlyingCoinsAnim(previousStars, earnedStars, coinStartDelay);
    }

    private void StartFlyingCoinsAnim(int previousStars, int earnedStars, float startDelay)
    {
        float coinDelay = startDelay;

        // Týpký yýldýzlar gibi sadece YENÝ KAZANILAN ödülleri fýrlatýr
        for (int i = previousStars + 1; i <= earnedStars; i++)
        {
            int coinValue = (i == 3) ? 20 : 15; // 1. ve 2. yýldýz 15, 3. yýldýz 20 coin verir

            if (i == 1) FlySingleCoin(animatedCoin1, coin1OrigPos, coinDelay, coinValue);
            if (i == 2) FlySingleCoin(animatedCoin2, coin2OrigPos, coinDelay, coinValue);
            if (i == 3) FlySingleCoin(animatedCoin3, coin3OrigPos, coinDelay, coinValue);

            coinDelay += 0.2f;
        }
    }

    private void FlySingleCoin(GameObject animCoin, Vector2 origAnchoredPos, float delay, int coinValueToAdd)
    {
        if (animCoin == null || txtCoinAmount == null) return;

        DOVirtual.DelayedCall(delay, () =>
        {
            animCoin.SetActive(true);

            // Baþlangýç pozisyonunu (senin tasarladýðýn yer) ve boyutunu sýfýrla
            RectTransform rect = animCoin.GetComponent<RectTransform>();
            rect.anchoredPosition = origAnchoredPos;
            animCoin.transform.localScale = Vector3.one;

            // txtCoinAmount'a (Sað üstteki Coin sayacý) doðru uç ve küçül
            animCoin.transform.DOMove(txtCoinAmount.transform.position, 0.6f).SetEase(Ease.InBack).SetUpdate(true);
            animCoin.transform.DOScale(0f, 0.6f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
            {
                animCoin.SetActive(false);

                // --- COIN SAYAÇ GÜNCELLEMESÝ VE JUICE EFEKTÝ ---
                if (int.TryParse(txtCoinAmount.text, out int currentVisualCount))
                {
                    AudioManager.Instance.PlayAudioClip("Sound_CoinIncreased");

                    currentVisualCount += coinValueToAdd;
                    txtCoinAmount.text = currentVisualCount.ToString();

                    // Coin içeri girdiðinde sayacýn zýplamasý (Punch efekti)
                    txtCoinAmount.transform.DOKill(true);
                    txtCoinAmount.transform.localScale = Vector3.one;
                    txtCoinAmount.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f).SetUpdate(true);
                }
            });
        }).SetUpdate(true);
    }

    private void FlySingleStar(Image animStar, Vector3 startPos, float delay)
    {
        if (animStar == null || txtStarAmount == null) return;

        DOVirtual.DelayedCall(delay, () =>
        {
            animStar.gameObject.SetActive(true);
            animStar.transform.position = startPos;
            animStar.transform.localScale = Vector3.one;

            // Sayaca doðru uç ve küçül
            animStar.transform.DOMove(txtStarAmount.transform.position, 0.6f).SetEase(Ease.InBack).SetUpdate(true);
            animStar.transform.DOScale(0f, 0.6f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
            {
                animStar.gameObject.SetActive(false);

                // --- SAYAÇ GÜNCELLEMESÝ (Animasyon bitince çalýþýr) ---
                if (int.TryParse(txtStarAmount.text, out int currentVisualCount))
                {
                    AudioManager.Instance.PlayAudioClip("Sound_StarIncreased");

                    currentVisualCount++; // Ekranda gördüðümüz sayýyý 1 artýr
                    txtStarAmount.text = currentVisualCount.ToString();

                    // Yýldýz sayaca girdiðinde sayacýn zýplamasý (Punch) için Juice efekti:
                    txtStarAmount.transform.DOKill(true); // Çakýþmalarý önler
                    txtStarAmount.transform.localScale = Vector3.one;
                    txtStarAmount.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f).SetUpdate(true);
                }
            });
        }).SetUpdate(true);
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
    // --- YENÝ EKLENEN KISIM ---
    // Bu metot, sahne deðiþtiðinde veya oyun kapatýldýðýnda Unity tarafýndan otomatik olarak çaðrýlýr.
    private void OnDestroy()
    {
        // Sahnede yarým kalmýþ, havada uçan veya bekleyen TÜM DOTween animasyonlarýný anýnda iptal eder.
        // Böylece obje yok olduðunda DOTween arkasýndan aðlamaz ve konsolda hata vermez.
        DOTween.KillAll();
    }
}
