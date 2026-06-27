using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string sceneName)
    {
        if(string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
    public void LoadCurrentLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void LoadNextLevel(string sceneName)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
