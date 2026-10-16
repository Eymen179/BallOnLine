using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Scriptable Objects/Shop Item")]
public class ShopItemSO : ScriptableObject
{
    [Header("Genel Maðaza Bilgileri")]
    public string itemID;          // Benzersiz ID (Örn: "Ball1", "Trail1", "Line1")
    public Sprite shopImage;       // Markette (ImgBall) görünecek ikon (Boþ býrakýlýrsa mevcut ikon korunur)
    public int requiredStars;      // Kilidin açýlmasý için gereken yýldýz
    public int coinCost;           // Satýn alma bedeli

    [Header("Temel Görünüm (Ball / Trail / Line Ortak)")]
    public Material shopItemMaterial; // Atanacak materyal (Farklý þekiller için desenli materyal)
    public Color shopItemColor = Color.white; // Tek renk kullanýlacaksa ana renk

    [Header("Geliþmiþ Renk Geçiþi (Trail & Line Ýçin)")]
    [Tooltip("Ýþaretlenirse tek renk (shopItemColor) yerine aþaðýdaki Gradient kullanýlýr.")]
    public bool useCustomGradient = false;
    public Gradient itemGradient;

    [Header("Þekil ve Boyut Ayarlarý (Trail & Line Ýçin)")]
    [Tooltip("Farklý þekilli/desenli dokularýn sünmemesi için 'Tile' seçilmelidir. Düz renklerde 'Stretch' kalabilir.")]
    public LineTextureMode textureMode = LineTextureMode.Stretch;

    [Tooltip("Ýþaretlenirse aþaðýdaki özel kalýnlýk eðrisi ve süre uygulanýr. Ýþaretlenmezse prefabýn kendi ayarý korunur.")]
    public bool useCustomCurveAndTime = false;
    public AnimationCurve widthCurve = AnimationCurve.Linear(0f, 0.5f, 1f, 0f);
    public float trailTime = 0.75f;

    // --- YARDIMCI METOT 1: TrailRenderer'a Ayarlarý Uygula ---
    public void ApplyToTrailRenderer(TrailRenderer trailRenderer)
    {
        if (trailRenderer == null) return;

        // 1. Materyal (Eðer SO içinde özel bir materyal atandýysa deðiþtir, yoksa mevcut olaný koru)
        if (shopItemMaterial != null)
        {
            trailRenderer.material = shopItemMaterial;
        }

        // 2. Doku Modu (Þekilli trail'lerin tekrar etmesi için Tile / Stretch ayarý)
        trailRenderer.textureMode = textureMode;

        // 3. Renk veya Gradient Ayarý
        if (useCustomGradient && itemGradient != null)
        {
            trailRenderer.colorGradient = itemGradient;
        }
        else
        {
            // Tek renk seçildiyse: Baþlangýçta tam renk, kuyruk ucunda þeffaf (Alpha = 0) bir Gradient oluþtur
            Gradient simpleGradient = new Gradient();
            simpleGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(shopItemColor, 0.0f), new GradientColorKey(shopItemColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(shopItemColor.a, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            trailRenderer.colorGradient = simpleGradient;
        }

        // 4. Özel Kalýnlýk Eðrisi ve Uzunluk (Süre)
        if (useCustomCurveAndTime)
        {
            trailRenderer.widthCurve = widthCurve;
            trailRenderer.time = trailTime;
        }
    }

    // --- YARDIMCI METOT 2: Ýleride Line Shop Ýçin LineRenderer'a Ayarlarý Uygula ---
    public void ApplyToLineRenderer(LineRenderer lineRenderer)
    {
        if (lineRenderer == null) return;

        if (shopItemMaterial != null)
        {
            lineRenderer.material = shopItemMaterial;
        }

        lineRenderer.textureMode = textureMode;

        if (useCustomGradient && itemGradient != null)
        {
            lineRenderer.colorGradient = itemGradient;
        }
        else
        {
            lineRenderer.startColor = shopItemColor;
            lineRenderer.endColor = shopItemColor;
        }

        if (useCustomCurveAndTime)
        {
            lineRenderer.widthCurve = widthCurve;
        }
    }
}