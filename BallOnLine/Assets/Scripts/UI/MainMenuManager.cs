using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Settings Images")]
    public Sprite spriteSoundOn;
    public Sprite spriteSoundOff;

    public Sprite spriteVibrationOn;
    public Sprite spriteVibrationOff;

    public Image buttonSoundSettings;
    public Image buttonVibrationSettings;

    private int soundButtonCounter = 0;
    private int vibrationButtonCounter = 0;

    public TextMeshProUGUI txtComingSoon;
    public string comingSoonSceneName = "Level_26";

    void Start()
    {
        soundButtonCounter = PlayerPrefs.GetInt("SoundButtonCounter", 0);
        vibrationButtonCounter = PlayerPrefs.GetInt("VibrationButtonCounter", 0);

        if (buttonSoundSettings != null)
        {
            ButtonCounter(soundButtonCounter, spriteSoundOn, spriteSoundOff, true);
        }
        if(buttonVibrationSettings != null)
        {
            ButtonCounter(vibrationButtonCounter, spriteVibrationOn, spriteVibrationOff, false);
        }

        txtComingSoon.gameObject.SetActive(false);

        // Ana menü açýlýnca banner reklamý göster
        if (AdManager.Instance != null) AdManager.Instance.ShowBanner();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //-------------------------------------------------------------------------------
    public void Button_Start()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick");

        // Kaydedilmiþ son açýk leveli al (Hiç oynanmamýþsa 1 gelir)
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        string unlockedLevelString = "Level_" + unlockedLevel;

        if(unlockedLevelString != comingSoonSceneName)
        {
            // Oyuna girerken alt bant reklamýný gizle
            if (AdManager.Instance != null) AdManager.Instance.HideBanner();
        }
        // Ýlgili sahneyi yükle
        //SceneController.Instance.LoadScene("Level" + unlockedLevel);
        SceneController.Instance.LoadScene(unlockedLevelString);
    }
    public void Button_Levels()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick");

        SceneController.Instance.LoadScene("LevelMenu");
    }
    public void Button_Sound()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick");

        soundButtonCounter++;
        PlayerPrefs.SetInt("SoundButtonCounter", soundButtonCounter);

        ButtonCounter(soundButtonCounter, spriteSoundOn, spriteSoundOff, true);
    }
    public void Button_Vibration()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick");

        vibrationButtonCounter++;
        PlayerPrefs.SetInt("VibrationButtonCounter", vibrationButtonCounter);

        ButtonCounter(vibrationButtonCounter, spriteVibrationOn, spriteVibrationOff, false);
    }
    public void Button_Shop()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick");

        SceneController.Instance.LoadScene("ShopMenu");
    }
    public void Button_PrivacyPolicy()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick");

    }

    private void ButtonCounter(int counter, Sprite spriteOn, Sprite spriteOff, bool isSound)
    {
        if(counter % 2 == 0) //Açma durumu
        {
            if(isSound)
            {
                buttonSoundSettings.GetComponent<Image>().sprite = spriteOn;

                // --- AudioManager'a yeni durumu bildir ---
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.ToggleSound(true);
                }
            }
            else
            {
                buttonVibrationSettings.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 512f);
                buttonVibrationSettings.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 512f);

                buttonVibrationSettings.GetComponent<Image>().sprite = spriteOn;

                // --- VIBRATION MANAGER'A BÝLDÝR ---
                if (VibrationManager.Instance != null)
                {
                    VibrationManager.Instance.ToggleVibration(true);
                }
            }
        }
        else //Kapama durumu
        {
            if(isSound)
            {
                buttonSoundSettings.GetComponent<Image>().sprite = spriteOff;

                // --- AudioManager'a yeni durumu bildir ---
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.ToggleSound(false);
                }
            }
            else
            {
                buttonVibrationSettings.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 290f);
                buttonVibrationSettings.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 470f);

                buttonVibrationSettings.GetComponent<Image>().sprite = spriteOff;

                // --- VIBRATION MANAGER'A BÝLDÝR ---
                if (VibrationManager.Instance != null)
                {
                    VibrationManager.Instance.ToggleVibration(false);
                }
            }
        }
    }
}
