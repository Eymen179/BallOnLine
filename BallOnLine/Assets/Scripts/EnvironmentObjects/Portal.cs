using UnityEngine;

public class Portal : MonoBehaviour, IInteractable
{
    public void Interact(BallController ball)
    {
        Destroy(ball.gameObject);

        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StopTimer();
        }

        UIManager.Instance.OpenPanel(UIManager.Instance.pnlWinMenu);
        UIManager.Instance.txtResultTime.text = UIManager.Instance.txtTimer.text;

        // --- GÜNCELLENEN KISIM ---
        float finalTime = TimerManager.Instance.GetElapsedTime();

        // Þu anki level numarasýný ve eski rekoru al
        int currentLevelNum = LevelManager.Instance.currentLevel.levelIndex;
        int previousStars = PlayerPrefs.GetInt("LevelStars_" + currentLevelNum, 0);

        // UI'da yýldýzlarý göster (Eski rekoru da metoda gönderiyoruz ki hangilerini uçuracaðýný bilsin)
        int earnedStars = UIManager.Instance.CalculateAndShowStars(finalTime, previousStars);

        // Kazanýlan yýldýzlarý StarManager'a yolla
        if (StarManager.Instance != null)
        {
            StarManager.Instance.SaveLevelStars(currentLevelNum, earnedStars);
        }
        // -------------------------

        UIManager.Instance.txtTimer.gameObject.SetActive(false);
        UIManager.Instance.starCounter.SetActive(true);

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (currentLevelNum >= unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevelNum + 1);
            PlayerPrefs.Save();
        }
    }
}