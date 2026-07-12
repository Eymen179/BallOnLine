using UnityEngine;

public class Portal : MonoBehaviour, IInteractable
{
    public void Interact(BallController ball)
    {
        Destroy(ball.gameObject);

        // --- EKLENEN KISIM: TÝMER'I DURDUR ---
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StopTimer();
        }

        UIManager.Instance.OpenPanel(UIManager.Instance.pnlWinMenu);
        UIManager.Instance.txtResultTime.text = UIManager.Instance.txtTimer.text;

        // --- EKLENEN KISIM: YILDIZLARI HESAPLA ---
        float finalTime = TimerManager.Instance.GetElapsedTime();
        UIManager.Instance.CalculateAndShowStars(finalTime);
        // -----------------------------------------

        UIManager.Instance.txtTimer.gameObject.SetActive(false);
        UIManager.Instance.starCounter.SetActive(true);

        // KAYIT SÝSTEMÝ: Þu anki levelin numarasýný al
        int currentLevelNum = LevelManager.Instance.currentLevel.levelIndex;
        // Sistemde kayýtlý olan "Açýk Level" numarasýný al (Yoksa varsayýlan 1'dir)
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // Eðer oyuncu, kilidini açtýðý en son leveli bitirdiyse, bir sonrakini aç
        if (currentLevelNum >= unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevelNum + 1);
            PlayerPrefs.Save(); // Telefona kaydet
        }
    }

}
