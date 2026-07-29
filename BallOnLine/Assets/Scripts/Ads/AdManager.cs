using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    [Header("AdMob Test ID'leri")]
    private string bannerId = "ca-app-pub-3940256099942544/6300978111";
    private string interstitialId = "ca-app-pub-3940256099942544/1033173712";

    private BannerView bannerView;
    private InterstitialAd interstitialAd;

    private int portalEntryCount = 0;

    // YENÝ: Reklam kapandýktan sonra çalýþtýrýlacak eylemi tutan deðiþken
    private System.Action onInterstitialClosed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus => { });
        LoadInterstitialAd();
    }

    public void ShowBanner()
    {
        if (bannerView != null) bannerView.Destroy();
        bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
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
        InterstitialAd.Load(interstitialId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null) return;
            interstitialAd = ad;

            // Oyuncu reklamý kapattýðýnda çalýþacak kýsým güncellendi
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                if (onInterstitialClosed != null)
                {
                    onInterstitialClosed.Invoke(); // Sahneyi geçirme komutunu þimdi çalýþtýr!
                    onInterstitialClosed = null; // Hafýzayý temizle
                }
                LoadInterstitialAd(); // Arkadan yenisini yükle
            };
        });
    }

    // YENÝ: Metot artýk içine bir "Action" (Görev) kabul ediyor
    public void ShowInterstitialIfTime(System.Action onComplete)
    {
        portalEntryCount++;

        if (portalEntryCount >= 3)
        {
            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                onInterstitialClosed = onComplete; // Kapanýnca ne olacaðýný hafýzaya al
                interstitialAd.Show(); // Reklamý göster
                portalEntryCount = 0;
            }
            else
            {
                LoadInterstitialAd();
                onComplete?.Invoke(); // Ýnternet yoksa reklamý beklemeden direkt geç
            }
        }
        else
        {
            onComplete?.Invoke(); // Reklam sýrasý gelmediyse direkt geç
        }
    }
}