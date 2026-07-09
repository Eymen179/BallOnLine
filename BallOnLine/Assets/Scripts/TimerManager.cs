using UnityEngine;
using System; // TimeSpan kullanýmý için zorunlu kütüphane

public class TimerManager : MonoBehaviour
{
    // Singleton yapýsý (Her yerden kolayca ulaþabilmek için)
    public static TimerManager Instance;

    private float elapsedTime = 0f;
    private bool isTimerRunning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            // Zamaný Frame bazýnda güvenli bir þekilde artýr
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    // LevelStartManager'dan çaðýrýlacak
    public void StartTimer()
    {
        isTimerRunning = true;
    }

    // Portal'dan çaðýrýlacak
    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isTimerRunning = false;
        UpdateTimerUI();
    }

    // Win panelinde süreyi kontrol etmek için dýþarýdan çaðýracaðýz
    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    private void UpdateTimerUI()
    {
        // UIManager'daki txtTimer referansýný kullanarak UI'ý güncelliyoruz
        if (UIManager.Instance != null && UIManager.Instance.txtTimer != null)
        {
            TimeSpan time = TimeSpan.FromSeconds(elapsedTime);

            // mm: Dakika (2 hane)
            // ss: Saniye (2 hane)
            // ff: Salise / Yüzdelik saniye (2 hane)
            UIManager.Instance.txtTimer.text = time.ToString(@"mm\:ss\:ff");
        }
    }
}