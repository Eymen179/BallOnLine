using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenuManager : MonoBehaviour
{

    public DrawingManager drawingManager;
    public Rigidbody2D ballRb;

    private int button_TimeTableCounter = 0;
    private void Start()
    {
        
    }
    /*Pause Menu - Win Menu - Death Menu*/
    public void Button_RestartLevel()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick2");

        // Mevcut sahneyi tekrar yükler
        SceneController.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }
    //-------------------------------------------------------------------------------
    /*Win Menu*/
    public void Button_NextLevel()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick2");

        int nextLevelNum = LevelManager.Instance.currentLevel.levelIndex + 1;

        // Hangi sahneye gidileceðini string olarak belirliyoruz
        string nextSceneName = (nextLevelNum <= 25 && nextLevelNum > 0) ? "Level_" + nextLevelNum : "MainMenu";

        // --- REKLAM VE SAHNE GEÇÝÞ KONTROLÜ (GÜNCELLENDÝ) ---
        if (AdManager.Instance != null)
        {
            // Sahne yükleme iþlemini direkt çaðýrmak yerine, AdManager'a "Görev" olarak veriyoruz.
            AdManager.Instance.ShowInterstitialIfTime(() =>
            {
                // Bu kod bloðu sadece reklam bittiðinde (veya sýra gelmediyse anýnda) çalýþýr!
                SceneController.Instance.LoadScene(nextSceneName);
            });
        }
        else
        {
            // Eðer sistemde AdManager yoksa (örneðin test yaparken silmiþsen) direkt yükle
            SceneController.Instance.LoadScene(nextSceneName);
        }
    }
    /*Pause Menu - Win Menu - Death Menu*/
    public void Button_BackToMainMenu()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick2");

        // Ana menü sahnesinin adýnýn "MainMenu" olduðunu varsayýyorum
        SceneController.Instance.LoadScene("MainMenu");
    }
    /*In-Game UI*/
    public void Button_Pause()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick");

        UIManager.Instance.OpenPanel(UIManager.Instance.pnlPauseMenu);

        if (drawingManager != null)
        {
            drawingManager.isGameActive = false;
        }
        // Topun fiziðini donduruyoruz (Aþaðý düþmemesi için)
        if (ballRb != null)
        {
            ballRb.simulated = false;
        }
        // --- EKLENEN KISIM: TÝMER'I DURAKLAT ---
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StopTimer();
        }
    }
    /*Pause Menu*/
    public void Button_Continue()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick2");

        UIManager.Instance.ClosePanel(UIManager.Instance.pnlPauseMenu);

        if (drawingManager != null)
        {
            drawingManager.isGameActive = true;
        }
        // Topun fiziðini tekrar aktif ediyoruz
        if (ballRb != null)
        {
            ballRb.simulated = true;
        }
        // --- EKLENEN KISIM: TÝMER'I DEVAM ETTÝR ---
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StartTimer();
        }
    }
    public void Button_TimeTable()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick2");

        button_TimeTableCounter++;
        if(button_TimeTableCounter % 2 == 1)
        {
            UIManager.Instance.pnlTimeTable.SetActive(true);
        }
        else
        {
            UIManager.Instance.pnlTimeTable.SetActive(false);
        }
    }
}
