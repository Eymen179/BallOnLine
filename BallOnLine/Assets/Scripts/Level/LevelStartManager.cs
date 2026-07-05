using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // Yeni Input System

public class LevelStartManager : MonoBehaviour
{
    [Header("Cinemachine Kameralarý")]
    public GameObject vcamPan;
    public GameObject vcamFollow;

    [Header("Pan (Kaydýrma) Ayarlarý")]
    public float panSpeed = 0.5f;
    private bool isPanningMode = true;
    private Vector2 lastTouchPos;

    [Header("Referanslar")]
    public DrawingManager drawingManager;
    public Rigidbody2D ballRb;

    private void Start()
    {
        // Baþlangýç durumu: Ýnceleme Modu aktif, Takip Modu kapalý
        vcamPan.SetActive(true);
        vcamFollow.SetActive(false);
        isPanningMode = true;

        if(drawingManager != null)
        {
            drawingManager.isGameActive = false;
        }
        // Topun fiziðini donduruyoruz (Aþaðý düþmemesi için)
        if (ballRb != null)
        {
            ballRb.simulated = false;
        }
    }

    private void Update()
    {
        // Eðer oyun baþladýysa veya ekrana dokunulmuyorsa pan iþlemini iptal et
        if (!isPanningMode || Pointer.current == null) return;

        // Ekrana ilk dokunulan kare
        if (Pointer.current.press.wasPressedThisFrame)
        {
            lastTouchPos = Pointer.current.position.ReadValue();
        }
        // Ekrana basýlý tutulup kaydýrýldýðý anlar
        else if (Pointer.current.press.isPressed)
        {
            Vector2 currentTouchPos = Pointer.current.position.ReadValue();
            Vector2 delta = currentTouchPos - lastTouchPos;
            Vector3 move = Vector3.zero;

            if (LevelManager.Instance != null && LevelManager.Instance.currentLevel != null 
                && LevelManager.Instance.currentLevel.axis == Level.levelAxis.XAxis)
                move = new Vector3(-delta.x, 0, 0) * panSpeed * Time.deltaTime;
            else if(LevelManager.Instance.currentLevel.axis == Level.levelAxis.YAxis)
                move = new Vector3(0, -delta.y, 0) * panSpeed * Time.deltaTime;

            // vcamPan objesinin transform'unu hareket ettiriyoruz. Sýnýrlarý Confiner2D koruyacak.
            vcamPan.transform.Translate(move);

            lastTouchPos = currentTouchPos;
        }
    }

    // Bu metodu UI'daki "Leveli Baþlat" butonunun OnClick eventine baðlayacaðýz.
    public void StartLevelPlay()
    {
        isPanningMode = false;

        // 1. KAMERA GEÇÝÞÝ
        // vcamPan'i kapatýp vcamFollow'u açtýðýmýzda, Cinemachine otomatik olarak
        // eski kameranýn olduðu yerden topun olduðu yere pürüzsüzce kayacaktýr.
        vcamPan.SetActive(false);
        vcamFollow.SetActive(true);

        // 2. UI GÝZLEME
        if (UIManager.Instance != null && UIManager.Instance.btnStartLevel != null)
        {
            UIManager.Instance.btnStartLevel.SetActive(false);

            UIManager.Instance.pnlBottomUIBlocker.SetActive(false); // Alt UI engelleyiciyi kaldýr
        }

        // 3. FÝZÝKLERÝ VE ÇÝZÝMÝ AKTÝF ETME
        if (ballRb != null)
        {
            ballRb.simulated = true; // Top yerçekimine kapýlýp düþmeye baþlar
        }

        if (drawingManager != null)
        {
            drawingManager.isGameActive = true; // Çizim mekaniði kullanýma açýlýr
        }
    }
}