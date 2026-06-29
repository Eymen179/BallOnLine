using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Singleton Kurulumu
    public static LevelManager Instance { get; private set; }

    [Header("Current Level Data")]
    [Tooltip("Ýçinde bulunduðumuz levelin Scriptable Object dosyasýný buraya sürükleyin.")]
    public Level currentLevel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}