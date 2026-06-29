using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenuManager : MonoBehaviour
{
    public void Button_RestartLevel()
    {
        // Mevcut sahneyi tekrar yükler
        SceneController.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }
    //-------------------------------------------------------------------------------
    public void Button_NextLevel()
    {
        int nextLevelNum = LevelManager.Instance.currentLevel.levelIndex + 1;

        // Þimdilik 25 level planladýðýn için sýnýr koyuyoruz
        if (nextLevelNum <= 25)
        {
            //SceneController.Instance.LoadScene("Level" + nextLevelNum);
            SceneController.Instance.LoadScene("TestScene " + nextLevelNum);

        }
        else
        {
            // 25. level bitince ana menüye veya "Yeni leveller yakýnda" ekranýna atabilir
            SceneController.Instance.LoadScene("MainMenu");
        }
    }

    public void Button_BackToMainMenu()
    {
        // Ana menü sahnesinin adýnýn "MainMenu" olduðunu varsayýyorum
        SceneController.Instance.LoadScene("MainMenu");
    }
}
