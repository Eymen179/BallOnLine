using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [HideInInspector] public int coinAmount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Sadece veriyi çekiyoruz, UI atamasýný diðer scriptler kendi Start'ýnda yapacak
            coinAmount = PlayerPrefs.GetInt("CoinAmount", 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

    }

    // "bool updateUI = true" eklendi. Varsayýlan olarak UI'ý günceller.
    public void UpdateCoinAmount(int amount, bool updateUI = true)
    {
        coinAmount += amount;
        PlayerPrefs.SetInt("CoinAmount", coinAmount);
        PlayerPrefs.Save();

        // Sadece updateUI true ise ekrandaki yazýyý deðiþtir
        if (updateUI && UIManager.Instance != null && UIManager.Instance.txtCoinAmount != null)
        {
            UIManager.Instance.txtCoinAmount.text = coinAmount.ToString();
        }
    }

    public void Button_DebugCoinResetter()
    {
        UpdateCoinAmount(-coinAmount); // Coin miktarýný sýfýrla
    }
}