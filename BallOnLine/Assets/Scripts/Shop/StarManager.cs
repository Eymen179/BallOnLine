using UnityEngine;

public class StarManager : MonoBehaviour
{
    public static StarManager Instance;

    [HideInInspector] public int totalStars;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Oyuna girildiðinde toplam yýldýz miktarýný hafýzadan çek
            totalStars = PlayerPrefs.GetInt("TotalStars", 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

    }

    // Portal scriptinden çaðrýlacak metot
    public void SaveLevelStars(int levelIndex, int earnedStars)
    {
        // Bu levelde daha önce alýnmýþ en yüksek yýldýzý bul
        int previousStars = PlayerPrefs.GetInt("LevelStars_" + levelIndex, 0);

        // Eðer yeni kazanýlan yýldýz, eskisinden büyükse:
        if (earnedStars > previousStars)
        {
            // --- YENÝ EKLENEN: YILDIZ BAÞINA COIN ÖDÜLÜ SÝSTEMÝ ---
            int coinReward = 0;

            // Döngü SADECE "yeni" kazanýlan yýldýzlar için çalýþýr.
            // Örn: Eskiden 1 yýldýzý varsa (previousStars = 1), döngü 2'den baþlar.
            for (int i = previousStars + 1; i <= earnedStars; i++)
            {
                if (i == 1) coinReward += 15;
                else if (i == 2) coinReward += 15;
                else if (i == 3) coinReward += 20;
            }

            // CoinManager üzerinden ödülü ver (Zaten kendi içinde PlayerPrefs'e kaydeder)
            if (coinReward > 0 && CoinManager.Instance != null)
            {
                CoinManager.Instance.UpdateCoinAmount(coinReward, false);
                Debug.Log($"Level {levelIndex}'den yeni kazanýlan yýldýzlar için {coinReward} Coin eklendi!");
            }
            // ------------------------------------------------------

            // Sadece aradaki farký toplam yýldýza ekle (Örn: 1'di 3 oldu, fark 2)
            int diff = earnedStars - previousStars;
            totalStars += diff;

            // Hafýzaya kaydet
            PlayerPrefs.SetInt("TotalStars", totalStars);
            PlayerPrefs.SetInt("LevelStars_" + levelIndex, earnedStars); // Bu levelin yeni rekoru
            PlayerPrefs.Save();

            // Oyun içi UI güncelleyici
            /*if (UIManager.Instance != null && UIManager.Instance.txtStarAmount != null)
            {
                UIManager.Instance.txtStarAmount.text = totalStars.ToString();
            }*/
        }
    }
}