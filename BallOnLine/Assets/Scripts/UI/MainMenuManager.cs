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

    public Button buttonSoundSettings;
    public Button buttonVibrationSettings;

    private int soundButtonCounter = 0;
    private int vibrationButtonCounter = 0;
    void Start()
    {
        if(buttonSoundSettings != null)
        {
            ButtonCounter(0, spriteSoundOn, spriteSoundOff, true);
        }
        if(buttonVibrationSettings != null)
        {
            ButtonCounter(0, spriteVibrationOn, spriteVibrationOff, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //-------------------------------------------------------------------------------
    public void Button_Start()
    {
        // Kaydedilmiþ son açýk leveli al (Hiç oynanmamýþsa 1 gelir)
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // Ýlgili sahneyi yükle
        //SceneController.Instance.LoadScene("Level" + unlockedLevel);
        SceneController.Instance.LoadScene("TestScene " + unlockedLevel);
    }
    public void Button_Levels()
    {
        SceneController.Instance.LoadScene("LevelMenu");
    }
    public void Button_Sound()
    {
        soundButtonCounter++;
        ButtonCounter(soundButtonCounter, spriteSoundOn, spriteSoundOff, true);
    }
    public void Button_Vibration()
    {
        vibrationButtonCounter++;
        ButtonCounter(vibrationButtonCounter, spriteVibrationOn, spriteVibrationOff, false);
    }
    public void Button_PrivacyPolicy()
    {

    }

    private void ButtonCounter(int counter, Sprite spriteOn, Sprite spriteOff, bool isSound)
    {
        if(counter % 2 == 0) //Açma durumu
        {
            if(isSound)
            {
                buttonSoundSettings.GetComponent<Image>().sprite = spriteOn;
            }
            else
            {
                buttonVibrationSettings.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 512f);
                buttonVibrationSettings.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 512f);

                buttonVibrationSettings.GetComponent<Image>().sprite = spriteOn;
            }
        }
        else //Kapama durumu
        {
            if(isSound)
            {
                buttonSoundSettings.GetComponent<Image>().sprite = spriteOff;
            }
            else
            {
                buttonVibrationSettings.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 290f);
                buttonVibrationSettings.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 470f);

                buttonVibrationSettings.GetComponent<Image>().sprite = spriteOff;
            }
        }
    }
}
