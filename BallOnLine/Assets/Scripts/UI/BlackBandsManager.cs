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
        // Ekran döndürme veya simülatör deðiþikliklerinde anlýk tepki vermesi için
        if (lastSafeArea != Screen.safeArea)
        {
            ApplyBands();
        }
    }

    void ApplyBands()
    {
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;

        // Üstteki güvensiz alanýn (çentik) piksel cinsinden yüksekliði
        float topOffset = Screen.height - (safeArea.y + safeArea.height);

        // Alttaki güvensiz alanýn piksel cinsinden yüksekliði
        float bottomOffset = safeArea.y;

        // Bantlarýn yüksekliklerini, hesaplanan bu piksel boþluklarýna eþitliyoruz
        if (topBand != null)
        {
            topBand.sizeDelta = new Vector2(topBand.sizeDelta.x, topOffset);
        }

        if (bottomBand != null)
        {
            bottomBand.sizeDelta = new Vector2(bottomBand.sizeDelta.x, bottomOffset);
        }
    }
}