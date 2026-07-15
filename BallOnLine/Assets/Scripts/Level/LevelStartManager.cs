using UnityEngine;
using UnityEngine.InputSystem;

public class LevelStartManager : MonoBehaviour
{
    [Header("Cinemachine Kameralarý")]
    public GameObject vcamPan;
    public GameObject vcamFollow;

    [Header("Pan (Kaydýrma) Ayarlarý")]
    public float panSpeed = 0.5f;
    private bool isPanningMode = true;
    private Vector2 lastTouchPos;

    [Header("Sýnýrlar ve Hedefler (YENÝ)")]
    [Tooltip("Kameranýn dýþarý çýkmasýný kod ile engellemek için level sýnýrýný buraya atýn.")]
    public Collider2D levelBounds;
    [Tooltip("Topu takip edecek ama ekseni kilitli kalacak sahte hedef obje.")]
    public Transform cameraFollowTarget;

    [Header("Referanslar")]
    public DrawingManager drawingManager;
    public Rigidbody2D ballRb;

    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;

        vcamPan.SetActive(true);
        vcamFollow.SetActive(false);
        isPanningMode = true;

        if (drawingManager != null) drawingManager.isGameActive = false;

        if (ballRb != null) ballRb.simulated = false;

        // --- GÜNCELLENEN KISIM ---
        if (cameraFollowTarget != null && ballRb != null && levelBounds != null)
        {
            // Sahte hedefin baþlangýç pozisyonunu topun olduðu yer olarak alýyoruz
            Vector3 targetPos = ballRb.transform.position;

            // Ancak eksen kontrolü yaparak sabit kalmasý gereken ekseni LEVEL'ÝN ORTASI yapýyoruz
            if (LevelManager.Instance.currentLevel.axis == Level.levelAxis.YAxis)
            {
                // Y ekseninde ilerleyen levelde, kamera X'te topu deðil levelin merkezini baz alýr
                targetPos.x = levelBounds.bounds.center.x;
            }
            else if (LevelManager.Instance.currentLevel.axis == Level.levelAxis.XAxis)
            {
                // X ekseninde ilerleyen levelde, kamera Y'de topu deðil levelin merkezini baz alýr
                targetPos.y = levelBounds.bounds.center.y;
            }

            // Sahte hedefi bu yeni, mükemmel ortalanmýþ konuma yerleþtir
            cameraFollowTarget.position = targetPos;
        }
    }

    private void Update()
    {
        // 1. TAKÝP MODU: Oyun baþladýysa sahte hedefi topun eksenine göre güncelle
        if (!isPanningMode)
        {
            UpdateFollowTarget();
            return;
        }

        // 2. PAN MODU: Ekrana dokunulmuyorsa iptal et
        if (Pointer.current == null) return;

        if (Pointer.current.press.wasPressedThisFrame)
        {
            lastTouchPos = Pointer.current.position.ReadValue();
        }
        else if (Pointer.current.press.isPressed)
        {
            Vector2 currentTouchPos = Pointer.current.position.ReadValue();
            Vector2 delta = currentTouchPos - lastTouchPos;
            Vector3 move = Vector3.zero;

            if (LevelManager.Instance != null && LevelManager.Instance.currentLevel != null)
            {
                if (LevelManager.Instance.currentLevel.axis == Level.levelAxis.XAxis)
                    move = new Vector3(-delta.x, 0, 0) * panSpeed * Time.deltaTime;
                else if (LevelManager.Instance.currentLevel.axis == Level.levelAxis.YAxis)
                    move = new Vector3(0, -delta.y, 0) * panSpeed * Time.deltaTime;
            }

            // Kamerayý hareket ettir
            vcamPan.transform.Translate(move);

            // DRIFTING (KAYMA) ÇÖZÜMÜ: Kameranýn Transform'unu sýnýrlar içine hapset
            if (levelBounds != null)
            {
                ClampPanCamera();
            }

            lastTouchPos = currentTouchPos;
        }
    }

    // Pan kamerasýnýn LevelBounds dýþýna çýkmasýný kesin olarak engeller
    private void ClampPanCamera()
    {
        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;

        Bounds bounds = levelBounds.bounds;

        float minX = bounds.min.x + camWidth;
        float maxX = bounds.max.x - camWidth;
        float minY = bounds.min.y + camHeight;
        float maxY = bounds.max.y - camHeight;

        // Level kameradan küçükse titremeyi önle
        if (minX > maxX) { float mid = (minX + maxX) / 2; minX = mid; maxX = mid; }
        if (minY > maxY) { float mid = (minY + maxY) / 2; minY = mid; maxY = mid; }

        Vector3 clampedPos = vcamPan.transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
        clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY);

        vcamPan.transform.position = clampedPos;
    }

    // Sahte hedefin sadece tek bir eksende topu takip etmesini saðlar
    private void UpdateFollowTarget()
    {
        if (cameraFollowTarget == null || ballRb == null) return;

        Vector3 newPos = cameraFollowTarget.position;

        if (LevelManager.Instance.currentLevel.axis == Level.levelAxis.XAxis)
        {
            newPos.x = ballRb.transform.position.x; // Sadece X'te takip et, Y sabit
        }
        else if (LevelManager.Instance.currentLevel.axis == Level.levelAxis.YAxis)
        {
            newPos.y = ballRb.transform.position.y; // Sadece Y'de takip et, X sabit
        }

        cameraFollowTarget.position = newPos;
    }

    public void StartLevelPlay()
    {
        AudioManager.Instance.PlayAudioClip("Sound_ButtonClick");

        isPanningMode = false;

        vcamPan.SetActive(false);
        vcamFollow.SetActive(true);

        if (UIManager.Instance != null && UIManager.Instance.btnStartLevel != null)
        {
            UIManager.Instance.btnStartLevel.SetActive(false);

            // Kodunda olan alt UI engelleyiciyi güvenli bir þekilde kapatýyoruz
            if (UIManager.Instance.pnlBottomUIBlocker != null)
                UIManager.Instance.pnlBottomUIBlocker.SetActive(false);
        }

        if (ballRb != null) ballRb.simulated = true;
        if (drawingManager != null) drawingManager.isGameActive = true;

        // --- EKLENEN KISIM: TÝMER'I BAÞLAT ---
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StartTimer();
        }

        //Zaman Tablosu
        UIManager.Instance.pnlTimeTable.SetActive(false);
        UIManager.Instance.btnTimeTable.gameObject.SetActive(false);
    }
}