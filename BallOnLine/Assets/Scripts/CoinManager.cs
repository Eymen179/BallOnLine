using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [HideInInspector]public int coinAmount;
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
        coinAmount = PlayerPrefs.GetInt("CoinAmount", 0);

        if (SceneManager.GetActiveScene().name != "MainMenu" && SceneManager.GetActiveScene().name != "LevelMenu")
        {
            UIManager.Instance.txtCoinAmount.text = coinAmount.ToString();
        }

    }
    public void UpdateCoinAmount(int amount)
    {
        coinAmount += amount;
        PlayerPrefs.SetInt("CoinAmount", coinAmount);
        UIManager.Instance.txtCoinAmount.text = coinAmount.ToString();
    }
}
