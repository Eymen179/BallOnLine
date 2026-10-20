using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    public MainMenuManager mainMenuManager;

    public string comingSoonSceneName = "Level_26";
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

    public void LoadScene(string sceneName)
    {
        if(sceneName == comingSoonSceneName)
        {
            if(mainMenuManager != null)
            {
                mainMenuManager.txtComingSoon.gameObject.SetActive(true);
            }
            return;
        }
        if (string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
