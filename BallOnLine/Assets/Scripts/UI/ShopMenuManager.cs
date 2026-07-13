using DTT.UI.ProceduralUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuManager : MonoBehaviour
{
    public static ShopMenuManager Instance;

    [Header("Top UI Elements")]
    public TextMeshProUGUI txtTotalCoins; // Sahnedeki Coin metnini sürükle
    public TextMeshProUGUI txtTotalStars; // Sahnedeki Toplam Yýldýz metnini sürükle

    [Header("Shop Scroll Views")]
    public GameObject ballShop_ScrollView;
    public GameObject trailShop_ScrollView;
    public GameObject lineShop_ScrollView;

    public Button button_ballShop;
    public Button button_trailShop;
    public Button button_lineShop;

    public TextMeshProUGUI txtComingSoon;

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
        // UI Güncellemesi (Yöneticilerden güncel bilgileri çek)
        if (CoinManager.Instance != null && txtTotalCoins != null)
        {
            txtTotalCoins.text = CoinManager.Instance.coinAmount.ToString();
        }
        if (StarManager.Instance != null && txtTotalStars != null)
        {
            txtTotalStars.text = StarManager.Instance.totalStars.ToString();
        }

        ScrollViewsettings(ballShop_ScrollView, true);
        ScrollViewsettings(trailShop_ScrollView, false);
        ScrollViewsettings(lineShop_ScrollView, false);

        ButtonSettings(button_ballShop, 1f);
        ButtonSettings(button_lineShop, 0.5f);
        ButtonSettings(button_trailShop, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Button_BallShop()
    {
        ScrollViewsettings(ballShop_ScrollView, true);
        ScrollViewsettings(trailShop_ScrollView, false);
        ScrollViewsettings(lineShop_ScrollView, false);

        ButtonSettings(button_ballShop, 1f);
        ButtonSettings(button_lineShop, 0.5f);
        ButtonSettings(button_trailShop, 0.5f);

        txtComingSoon.gameObject.SetActive(false);
    }
    public void Button_TrailShop()
    {
        ScrollViewsettings(ballShop_ScrollView, false);
        ScrollViewsettings(trailShop_ScrollView, true);
        ScrollViewsettings(lineShop_ScrollView, false);

        ButtonSettings(button_ballShop, 0.5f);
        ButtonSettings(button_lineShop, 0.5f);
        ButtonSettings(button_trailShop, 1f);

        txtComingSoon.gameObject.SetActive(true);
    }
    public void Button_LineShop()
    {
        ScrollViewsettings(ballShop_ScrollView, false);
        ScrollViewsettings(trailShop_ScrollView, false);
        ScrollViewsettings(lineShop_ScrollView, true);

        ButtonSettings(button_ballShop, 0.5f);
        ButtonSettings(button_lineShop, 1f);
        ButtonSettings(button_trailShop, 0.5f);

        txtComingSoon.gameObject.SetActive(true);
    }
    public void Button_BackToMainMenu()
    {
        SceneController.Instance.LoadScene("MainMenu");
    }

    //Yardimci metotlar
    private void ScrollViewsettings(GameObject selectedScrollView, bool isActive)
    {
        if(selectedScrollView != null)
        {
            selectedScrollView.SetActive(isActive);
        }
    }
    private void ButtonSettings(Button selectedButton, float alpha)
    {
        if(selectedButton != null)
        {
            selectedButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
        }
    }
}
