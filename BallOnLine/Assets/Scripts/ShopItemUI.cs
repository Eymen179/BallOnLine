using DTT.UI.ProceduralUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [Header("Data")]
    public ShopItemSO itemData; // Bu panele ait topun verisi

    [Header("UI Elements")]
    public Image imgShopItem;
    public Image imgLocked;
    public Button btnBuyAndUse;

    [Header("Button Contents (Texts & Icons)")]
    public TextMeshProUGUI txtAction; // "Use" veya "Using" yazacak text
    public GameObject starIconGroup; // Ýçinde yýldýz ikonu ve text'i olan obje
    public GameObject coinIconGroup; // Ýçinde coin ikonu ve text'i olan obje
    public TextMeshProUGUI txtCoinCost;
    public TextMeshProUGUI txtStarRequirement;

    private GradientEffect buttonGradient;
    /*public Gradient grayGradient; // Gri Gradient
    public Gradient redGradient; // Kýrmýzý Gradient
    public Gradient greenGradient; // Yeþil Gradient
    public Gradient lightBlueGradient; // Açýk Mavi Gradient
    public Gradient darkBlueGradient; // Koyu Mavi Gradient*/

    private void Awake()
    {
        buttonGradient = btnBuyAndUse.GetComponent<GradientEffect>();
        // Butona týklandýðýnda Manager'a haber ver
        btnBuyAndUse.onClick.AddListener(OnButtonClicked);
    }

    public void Setup(ShopItemState state)
    {
        // 1. Temel Görselleri Ayarla
        imgShopItem.sprite = itemData.shopImage;

        starIconGroup.SetActive(false);
        coinIconGroup.SetActive(false);
        txtAction.gameObject.SetActive(false);

        // 2. Duruma (State) Göre 5 Farklý Tasarým
        switch (state)
        {
            case ShopItemState.Locked_Gray:
                imgShopItem.gameObject.SetActive(false);
                imgLocked.gameObject.SetActive(true);
                btnBuyAndUse.interactable = false;
                SetDynamicGradient(new Color(0.6f, 0.6f, 0.6f), new Color(0.3f, 0.3f, 0.3f)); // Gri
                //SetButtonColor(grayGradient); // Gri Gradient
                starIconGroup.SetActive(true);
                txtStarRequirement.text = itemData.requiredStars.ToString();
                break;

            case ShopItemState.CannotBuy_Red:
                imgShopItem.gameObject.SetActive(true);
                imgLocked.gameObject.SetActive(false);
                btnBuyAndUse.interactable = false;
                SetDynamicGradient(new Color(1f, 0.2f, 0.3f), new Color(0.6f, 0f, 0.1f)); // Kýrmýzý
                //SetButtonColor(redGradient); // Kýrmýzý Gradient
                coinIconGroup.SetActive(true);
                txtCoinCost.text = itemData.coinCost.ToString();
                break;

            case ShopItemState.CanBuy_Green:
                imgShopItem.gameObject.SetActive(true);
                imgLocked.gameObject.SetActive(false);
                btnBuyAndUse.interactable = true;
                SetDynamicGradient(new Color(0.2f, 0.9f, 0.2f), new Color(0f, 0.5f, 0f)); // Yeþil
                //SetButtonColor(greenGradient); // Yeþil Gradient
                coinIconGroup.SetActive(true);
                txtCoinCost.text = itemData.coinCost.ToString();
                break;

            case ShopItemState.Equipped_LightBlue:
                imgShopItem.gameObject.SetActive(true);
                imgLocked.gameObject.SetActive(false);
                btnBuyAndUse.interactable = false; // Kullanýlan topa tekrar týklanamaz
                SetDynamicGradient(new Color(0.2f, 0.8f, 1f), new Color(0f, 0.4f, 0.8f)); // Açýk Mavi
                //SetButtonColor(lightBlueGradient); // Açýk Mavi Gradient
                txtAction.gameObject.SetActive(true);
                txtAction.text = "Using";
                break;

            case ShopItemState.Owned_DarkBlue:
                imgShopItem.gameObject.SetActive(true);
                imgLocked.gameObject.SetActive(false);
                btnBuyAndUse.interactable = true; // DÜZELTME: Seçebilmek için aktif olmalý
                SetDynamicGradient(new Color(0.1f, 0.2f, 0.8f), new Color(0f, 0f, 0.4f)); // Koyu Mavi
                //SetButtonColor(darkBlueGradient); // Koyu Mavi Gradient
                txtAction.gameObject.SetActive(true);
                txtAction.text = "Use";
                break;
        }
    }

    // YENÝ EKLENEN DÝNAMÝK GRADÝENT METODU
    private void SetDynamicGradient(Color topColor, Color bottomColor)
    {
        if (buttonGradient != null)
        {
            Gradient g = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0].color = topColor; colorKeys[0].time = 0f;
            colorKeys[1].color = bottomColor; colorKeys[1].time = 1f;

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0].alpha = 1f; alphaKeys[0].time = 0f;
            alphaKeys[1].alpha = 1f; alphaKeys[1].time = 1f;

            g.SetKeys(colorKeys, alphaKeys);
            buttonGradient.Gradient = g;

            // Efekti kapa aç yaparak anýnda ekrana yansýmasýný zorlar
            buttonGradient.enabled = false;
            buttonGradient.enabled = true;
        }
    }
    /*private void SetButtonColor(Gradient gradient)
    {
        // NOT: DTT Procedural UI kullanýyorsan, bu kýsmý o asset'in Gradient deðiþtirme koduyla güncellemelisin.
        if (buttonGradient != null)
        {
            buttonGradient.Gradient = gradient;
            buttonGradient.enabled = false; // Efekti kapa aç yaparak yenilemeye zorla
            buttonGradient.enabled = true;
        }
    }*/

    private void OnButtonClicked()
    {
        BallShopManager.Instance.OnShopItemClicked(this);
    }
}

public enum ShopItemState
{
    Locked_Gray,
    CannotBuy_Red,
    CanBuy_Green,
    Equipped_LightBlue,
    Owned_DarkBlue
}