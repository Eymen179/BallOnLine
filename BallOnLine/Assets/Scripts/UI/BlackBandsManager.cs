using UnityEngine;

public class BlackBandsManager : MonoBehaviour
{
    [Header("Bant Referanslarý")]
    public RectTransform topBand;    // Üst siyah bant (Çentik için)
    public RectTransform bottomBand; // Alt siyah bant (Home butonu/çizgisi için)

    private Rect lastSafeArea = new Rect(0, 0, 0, 0);

    void Start()
    {
        ApplyBands();
    }

    void Update()
    {
        if (lastSafeArea != Screen.safeArea)
        {
            ApplyBands();
        }
    }

    void ApplyBands()
    {
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;

        // Ekran oranlarýný 0 ile 1 arasýna çeviriyoruz (Týpký SafeArea.cs'deki mantýk)
        float safeAreaTopY = (safeArea.y + safeArea.height) / Screen.height;
        float safeAreaBottomY = safeArea.y / Screen.height;

        // --- ÜST BANT ---
        // Ekranýn en üstünden (1), Güvenli alanýn bittiði yere kadar (safeAreaTopY) uzat
        if (topBand != null)
        {
            topBand.anchorMin = new Vector2(0, safeAreaTopY);
            topBand.anchorMax = new Vector2(1, 1);
            topBand.offsetMin = Vector2.zero; // Sol ve Alt boþluklarý sýfýrla
            topBand.offsetMax = Vector2.zero; // Sað ve Üst boþluklarý sýfýrla
        }

        // --- ALT BANT ---
        // Ekranýn en altýndan (0), Güvenli alanýn baþladýðý yere kadar (safeAreaBottomY) uzat
        if (bottomBand != null)
        {
            bottomBand.anchorMin = new Vector2(0, 0);
            bottomBand.anchorMax = new Vector2(1, safeAreaBottomY);
            bottomBand.offsetMin = Vector2.zero;
            bottomBand.offsetMax = Vector2.zero;
        }
    }
}