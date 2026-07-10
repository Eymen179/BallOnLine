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
        // Mevcut sahneyi tekrar yükler
        SceneController.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }
    //-------------------------------------------------------------------------------
    /*Win Menu*/
    public void Button_NextLevel()
    {
        int nextLevelNum = LevelManager.Instance.currentLevel.levelIndex + 1;

        // Þimdilik 25 level planladýðýn için sýnýr koyuyoruz
        if (nextLevelNum <= 25 && nextLevelNum > 0)
        {
            //SceneController.Instance.LoadScene("Level" + nextLevelNum);
            SceneController.Instance.LoadScene("Level_" + nextLevelNum);

        }
        else
        {
            // 25. level bitince ana menüye veya "Yeni leveller yakýnda" ekranýna atabilir
            SceneController.Instance.LoadScene("MainMenu");
        }
    }
    /*Pause Menu - Win Menu - Death Menu*/
    public void Button_BackToMainMenu()
    {
        // Ana menü sahnesinin adýnýn "MainMenu" olduðunu varsayýyorum
        SceneController.Instance.LoadScene("MainMenu");
    }
    /*In-Game UI*/
    public void Button_Pause()
    {
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
