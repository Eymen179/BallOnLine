using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuManager : MonoBehaviour
{
    [Header("Top UI Elements")]
    public TextMeshProUGUI txtTotalCoins; // Sahnedeki Coin metnini sürükle
    public TextMeshProUGUI txtTotalStars; // Sahnedeki Toplam Yýldýz metnini sürükle

    [Header("Tüm Level Butonlarý (Sýrasýyla 1'den 25'e)")]
    public Button[] levelButtons;

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

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNum = i + 1;

            levelButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = levelNum.ToString();

            // --- YILDIZ AKTÝFLEÞTÝRME SÝSTEMÝ ---
            // Bu level için daha önceden kaydedilmiþ rekor yýldýzý bul
            int earnedStars = PlayerPrefs.GetInt("LevelStars_" + levelNum, 0);

            int starIndex = 0;
            // Butonun içindeki (child) tüm objeleri tara
            foreach (Transform child in levelButtons[i].transform)
            {
                // Eðer obje senin ayarladýðýn tag'e sahipse
                if (child.CompareTag("LevelStar"))
                {
                    // Alýnan yýldýz sayýsýna kadar olanlarý aç, gerisini kapa
                    if (starIndex < earnedStars)
                        child.gameObject.SetActive(true);
                    else
                        child.gameObject.SetActive(false);

                    starIndex++;
                }
            }
            // ------------------------------------

            if (levelNum <= unlockedLevel)
            {
                levelButtons[i].interactable = true;
                levelButtons[i].GetComponentInChildren<TextMeshProUGUI>().alpha = 1f;
                levelButtons[i].onClick.AddListener(() => LoadLevel(levelNum));
            }
            else
            {
                levelButtons[i].interactable = false;
                levelButtons[i].GetComponentInChildren<TextMeshProUGUI>().alpha = 0.3f;
            }
        }
    }

    private void LoadLevel(int levelIndex)
    {
        SceneController.Instance.LoadScene("Level_" + levelIndex);
    }

    public void Button_BackToMain()
    {
        SceneController.Instance.LoadScene("MainMenu");
    }
}