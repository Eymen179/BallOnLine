using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    [Header("Çalýþma Modu")]
    [Tooltip("Geliþtirme yaparken veya kapalý testteyken TRUE olsun. Yayýna (0.2.0) çýkarken tiki kaldýrýn (FALSE)!")]
    public bool isTestMode = true;

    [Header("Gerçek AdMob Reklam Birimi ID'leri")]
    [SerializeField] private string realBannerId = "ca-app-pub-BURAYA_GERCEK_BANNER_ID";
    [SerializeField] private string realInterstitialId = "ca-app-pub-BURAYA_GERCEK_GECIS_ID";

    // Google'ýn resmi, ban riski olmayan evrensel test birim ID'leri
    private const string testBannerId = "ca-app-pub-3940256099942544/6300978111";
    private const string testInterstitialId = "ca-app-pub-3940256099942544/1033173712";

    // Hangi ID'nin kullanýlacaðýný moda göre otomatik seçen özellikler (Property)
    private string CurrentBannerId => isTestMode ? testBannerId : realBannerId;
    private string CurrentInterstitialId => isTestMode ? testInterstitialId : realInterstitialId;

    private BannerView bannerView;
    private InterstitialAd interstitialAd;

    private int portalEntryCount = 0;

    // Reklam kapandýktan sonra çalýþtýrýlacak eylemi tutan deðiþken
    private System.Action onInterstitialClosed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus => { });
        LoadInterstitialAd();
    }

    public void ShowBanner()
    {
        if (bannerView != null) bannerView.Destroy();

        // Aktif moda uygun ID otomatik çekilir
        bannerView = new BannerView(CurrentBannerId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
    }

    public void HideBanner()
    {
        if (bannerView != null) bannerView.Destroy();
    }

    public void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        AdRequest request = new AdRequest();

        // Aktif moda uygun ID otomatik çekilir
        InterstitialAd.Load(CurrentInterstitialId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null) return;
            interstitialAd = ad;

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                if (onInterstitialClosed != null)
                {
                    onInterstitialClosed.Invoke();
                    onInterstitialClosed = null;
                }
                LoadInterstitialAd(); // Sonraki gösterim için yenisini hazýrla
            };
        });
    }

    public void ShowInterstitialIfTime(System.Action onComplete)
    {
        portalEntryCount++;

        if (portalEntryCount >= 3)
        {
            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                onInterstitialClosed = onComplete;
                interstitialAd.Show();
                portalEntryCount = 0;
            }
            else
            {
                LoadInterstitialAd();
                onComplete?.Invoke(); // Reklam hazýr deðilse sahne geçiþini engelleme
            }
        }
        else
        {
            onComplete?.Invoke(); // 3 bölüme ulaþýlmadýysa doðrudan devam et
        }
    }
}