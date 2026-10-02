using System.Collections; // Coroutine için eklendi
using DG.Tweening;        // Animasyonlar için eklendi
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
        SceneController.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }
    //-------------------------------------------------------------------------------
    /*Win Menu*/
    public void Button_NextLevel()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick2");

        int nextLevelNum = LevelManager.Instance.currentLevel.levelIndex + 1;
        string nextSceneName = (nextLevelNum <= 25 && nextLevelNum > 0) ? "Level_" + nextLevelNum : "MainMenu";

        if (AdManager.Instance != null)
        {
            AdManager.Instance.ShowInterstitialIfTime(() =>
            {
                SceneController.Instance.LoadScene(nextSceneName);
            });
        }
        else
        {
            SceneController.Instance.LoadScene(nextSceneName);
        }
    }
    /*Pause Menu - Win Menu - Death Menu*/
    public void Button_BackToMainMenu()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick2");
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
        if (ballRb != null)
        {
            ballRb.simulated = false;
        }
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StopTimer();
        }
    }

    /*Pause Menu*/
    public void Button_Continue()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick2");

        // Paneli kapatýyoruz ama oyunu anýnda baþlatmýyoruz
        UIManager.Instance.ClosePanel(UIManager.Instance.pnlPauseMenu);

        // Geri sayým döngüsünü tetikliyoruz
        StartCoroutine(ResumeCountdownRoutine());
    }

    // --- EKLENEN KISIM: DEVAM ETME GERÝ SAYIMI ---
    private IEnumerator ResumeCountdownRoutine()
    {
        if (UIManager.Instance != null && UIManager.Instance.txtCountdown != null)
        {
            UIManager.Instance.txtCountdown.gameObject.SetActive(true);

            // 3'ten 1'e doðru geri sayým
            for (int i = 3; i > 0; i--)
            {
                UIManager.Instance.txtCountdown.text = i.ToString();

                UIManager.Instance.txtCountdown.transform.localScale = Vector3.zero;
                // SetUpdate(true) ekleyerek, eðer ileride timeScale kullanýrsan animasyonun donmamasýný garantiye alýyoruz
                UIManager.Instance.txtCountdown.transform.DOScale(7f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);

                yield return new WaitForSeconds(0.5f);
            }

            // Süre bitince GO! yazýsý
            UIManager.Instance.txtCountdown.text = "GO!";
            UIManager.Instance.txtCountdown.transform.localScale = Vector3.zero;
            UIManager.Instance.txtCountdown.transform.DOScale(7f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
        }

        // --- ASIL OYUN DEVAMI ---
        if (drawingManager != null)
        {
            drawingManager.isGameActive = true;
        }
        // Topun fiziðini tekrar aktif ediyoruz
        if (ballRb != null)
        {
            ballRb.simulated = true;
        }
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StartTimer();
        }

        // GO yazýsýný ekranda 0.7 saniye tutup kapatýyoruz
        yield return new WaitForSeconds(0.7f);

        if (UIManager.Instance != null && UIManager.Instance.txtCountdown != null)
        {
            UIManager.Instance.txtCountdown.gameObject.SetActive(false);
        }
    }

    public void Button_TimeTable()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick2");

        button_TimeTableCounter++;
        if (button_TimeTableCounter % 2 == 1)
        {
            UIManager.Instance.pnlTimeTable.SetActive(true);
        }
        else
        {
            UIManager.Instance.pnlTimeTable.SetActive(false);
        }
    }
}