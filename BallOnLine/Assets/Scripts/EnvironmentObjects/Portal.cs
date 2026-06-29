using UnityEngine;

public class Portal : MonoBehaviour, IInteractable
{
    public void Interact(BallController ball)
    {
        Destroy(ball.gameObject);

        UIManager.Instance.pnlWinMenu.SetActive(true);

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
