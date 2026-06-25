using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private InputActionReference restart;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(restart.action.WasPressedThisFrame())
        {
            Debug.Log("Restart triggered");
            // Restart logic here
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
