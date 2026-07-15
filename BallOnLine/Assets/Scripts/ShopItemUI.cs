using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [Header("Data")]
    public ShopItemSO itemData;

    [Header("Shared Icons (Ortak Görseller)")]
    public Image imgBall;
    public Image imgLocked;

    [Header("5 State Buttons (Hiyerarþideki Butonlar)")]
    public GameObject btnUsing;        // Açýk Mavi (Kullanýmda)
    public GameObject btnUse;          // Koyu Mavi (Kullanýmda deðil)
    public GameObject btnLocked;       // Gri (Star isteyen)
    public GameObject btnCanBuy;       // Yeþil (Coin yeterli)
    public GameObject btnCanNotBuy;    // Kýrmýzý (Coin yetersiz)

    [Header("Texts (Fiyat ve Yýldýz Yazýlarý)")]
    public TextMeshProUGUI txtStarRequirement; // BtnLocked'ýn içindeki Text
    public TextMeshProUGUI txtCostCanBuy;      // BtnCanBuy'ýn içindeki Text
    public TextMeshProUGUI txtCostCannotBuy;   // BtnCanNotBuy'ýn içindeki Text

    private void Awake()
    {
        // Sadece etkileþime girilebilen (Yeþil ve Koyu Mavi) butonlara týklanma özelliði ekliyoruz.
        // Böylece Inspector'dan tek tek OnClick atamakla uðraþmayacaksýn.
        if (btnCanBuy != null)
        {
            Button btnGreen = btnCanBuy.GetComponent<Button>();
            if (btnGreen != null) btnGreen.onClick.AddListener(OnButtonClicked);
        }

        if (btnUse != null)
        {
            Button btnDarkBlue = btnUse.GetComponent<Button>();
            if (btnDarkBlue != null) btnDarkBlue.onClick.AddListener(OnButtonClicked);
        }
    }

    public void Setup(ShopItemState state)
    {
        // 1. Ortak Verileri Doldur
        if (imgBall != null) imgBall.sprite = itemData.shopImage;
        if (txtStarRequirement != null) txtStarRequirement.text = itemData.requiredStars.ToString();
        if (txtCostCanBuy != null) txtCostCanBuy.text = itemData.coinCost.ToString();
        if (txtCostCannotBuy != null) txtCostCannotBuy.text = itemData.coinCost.ToString();

        // 2. Önce Tüm Butonlarý Gizle (Temizlik)
        if (btnLocked != null) btnLocked.SetActive(false);
        if (btnCanNotBuy != null) btnCanNotBuy.SetActive(false);
        if (btnCanBuy != null) btnCanBuy.SetActive(false);
        if (btnUsing != null) btnUsing.SetActive(false);
        if (btnUse != null) btnUse.SetActive(false);

        // 3. Duruma (State) Göre Sadece Ýlgili Ýkonu ve Butonu Aç
        switch (state)
        {
            case ShopItemState.Locked_Gray:
                imgBall.gameObject.SetActive(false);
                imgLocked.gameObject.SetActive(true);
                if (btnLocked != null) btnLocked.SetActive(true);
                break;

            case ShopItemState.CannotBuy_Red:
                imgBall.gameObject.SetActive(true);
                imgLocked.gameObject.SetActive(false);
                if (btnCanNotBuy != null) btnCanNotBuy.SetActive(true);
                break;

            case ShopItemState.CanBuy_Green:
                imgBall.gameObject.SetActive(true);
                imgLocked.gameObject.SetActive(false);
                if (btnCanBuy != null) btnCanBuy.SetActive(true);
                break;

            case ShopItemState.Equipped_LightBlue:
                imgBall.gameObject.SetActive(true);
                imgLocked.gameObject.SetActive(false);
                if (btnUsing != null) btnUsing.SetActive(true);
                break;

            case ShopItemState.Owned_DarkBlue:
                imgBall.gameObject.SetActive(true);
                imgLocked.gameObject.SetActive(false);
                if (btnUse != null) btnUse.SetActive(true);
                break;
        }
    }

    private void OnButtonClicked()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick");

        BallShopManager.Instance.OnShopItemClicked(this);
    }
}

// (ShopItemState enum'u zaten ayný kalsýn)
public enum ShopItemState
{
    Locked_Gray,
    CannotBuy_Red,
    CanBuy_Green,
    Equipped_LightBlue,
    Owned_DarkBlue
}