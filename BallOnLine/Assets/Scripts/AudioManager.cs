using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Main Sound Settings")]
    public AudioSource audioSource;
    [SerializeField] private List<AudioClip> audioSounds;

    [Header("Drawing Sound Settings")]
    public AudioSource drawingAudioSource; // Çizim için KENDÝNE ÖZEL yeni bir kanal yaratýyoruz
    public AudioClip drawingLoopClip;      // Çalýnacak loop neon sesi

    [Tooltip("Çizim sesinin maksimum yüksekliði (0 ile 1 arasý)")]
    public float maxDrawingVolume = 0.35f; // --- 2. YENÝ EKLENDÝ ---

    // --- SES KONTROL DEÐÝÞKENÝ ---
    public bool isSoundOn = true;
    private bool isDrawingSoundFadingOut = false; // --- 3. YENÝ EKLENDÝ ---

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

    // DrawingManager, parmak basýlýyken bu metodu çaðýracak
    public void StartDrawingSound()
    {
        if (!isSoundOn || drawingAudioSource == null || drawingLoopClip == null) return;

        isDrawingSoundFadingOut = false;
        // Eðer ses zaten çalmýyorsa baþlat
        if (!drawingAudioSource.isPlaying)
        {
            drawingAudioSource.clip = drawingLoopClip;
            drawingAudioSource.loop = true; // Sesi sonsuz döngüye al
            drawingAudioSource.volume = 0f;
            drawingAudioSource.Play();
        }

        // DOTween ile 0.08 saniyede (hissedilemeyecek kadar kýsa bir sürede) sesi yumuþakça aç
        drawingAudioSource.DOFade(maxDrawingVolume, 0.05f).SetUpdate(true);
    }

    // Parmaðýn eksenine göre sesin tizliðini (Pitch) ayarlar
    public void UpdateDrawingPitch(float pitchValue)
    {
        if (drawingAudioSource != null && drawingAudioSource.isPlaying)
        {
            // Pitch genelde 0.5 (kalýn) ile 1.5 (tiz) veya 2.0 arasýnda çok güzel duyulur.
            drawingAudioSource.pitch = pitchValue;
        }
    }

    // Parmak ekrandan çekildiðinde sesi durdurur
    public void StopDrawingSound()
    {
        // Ses zaten kýsýlýyorsa kodu tekrar çalýþtýrma
        if (drawingAudioSource != null && drawingAudioSource.isPlaying && !isDrawingSoundFadingOut)
        {
            isDrawingSoundFadingOut = true;
            drawingAudioSource.DOKill();

            // 0.08 saniyede sesi pürüzsüzce sýfýra indir, sýfýr olunca sesi tamamen durdur
            drawingAudioSource.DOFade(0f, 0.05f).SetUpdate(true).OnComplete(() =>
            {
                drawingAudioSource.Stop();
                isDrawingSoundFadingOut = false;
            });
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