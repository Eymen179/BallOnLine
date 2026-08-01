using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource audioSource;
    [SerializeField] private List<AudioClip> audioSounds;

    // --- SES KONTROL DEÐÝÞKENÝ ---
    public bool isSoundOn = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Oyuna girildiðinde oyuncunun önceki ses tercihini hatýrla (Varsayýlan 1 = Açýk)
            isSoundOn = PlayerPrefs.GetInt("SoundSetting", 1) == 1;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudioClip(string audioName)
    {
        // Eðer ses kapalýysa metottan direkt çýk, çalma
        if (!isSoundOn) return;

        foreach (AudioClip clip in audioSounds)
        {
            if (clip.name == audioName)
            {
                audioSource.PlayOneShot(clip);
                break; // Sesi bulduk, döngüyü boþuna devam ettirme!
            }
        }
    }

    // Metoda taretin nerede olduðunu (Vector3) parametre olarak ekliyoruz
    public void PlayFireAudio(Vector3 sourcePosition)
    {
        if (!isSoundOn) return;

        AudioClip fireClip = audioSounds.Find(clip => clip.name == "Sound_TurretFire");

        if (fireClip != null)
        {
            // Belirtilen koordinatta anlýk bir 3D ses objesi oluþturur ve çalar
            AudioSource.PlayClipAtPoint(fireClip, sourcePosition);
        }
    }

    // Ana menüden (MainMenuManager'dan) çaðrýlacak metot
    public void ToggleSound(bool soundState)
    {
        isSoundOn = soundState;

        // Tercihi hafýzaya kaydet (Açýksa 1, Kapalýysa 0)
        PlayerPrefs.SetInt("SoundSetting", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();

        // Eðer ses kapatýldýysa ve o an arka planda uzayan bir ses varsa anýnda sustur
        if (!isSoundOn && audioSource != null)
        {
            audioSource.Stop();
        }
    }
}