using UnityEngine;
using UnityEngine.UI;

public class LevelMenuManager : MonoBehaviour
{
    [Header("Tüm Level Butonlarý (Sýrasýyla 1'den 25'e)")]
    public Button[] levelButtons;

    void Start()
    {
        // Oyuncunun açtýðý son leveli PlayerPrefs'ten alýyoruz
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNum = i + 1; // Diziler 0'dan, leveller 1'den baþlar

            if (levelNum <= unlockedLevel)
            {
                // Level açýksa butonu aktif et ve dinleyici ekle
                levelButtons[i].interactable = true;

                // Butona týklandýðýnda LoadLevel metodunu o levelin numarasýyla çalýþtýr
                levelButtons[i].onClick.AddListener(() => LoadLevel(levelNum));
            }
            else
            {
                // Level kilitliyse butonu soluk ve týklanamaz yap
                levelButtons[i].interactable = false;
            }
        }
    }
    //-------------------------------------------------------------------------------
    private void LoadLevel(int levelIndex)
    {
        // "Level1", "Level2" isim standartlarýna göre sahneyi yükler
        //SceneController.Instance.LoadScene("Level" + levelIndex);
        SceneController.Instance.LoadScene("TestScene " + levelIndex);
    }

    public void Button_BackToMain()
    {
        SceneController.Instance.LoadScene("MainMenu");
    }
}