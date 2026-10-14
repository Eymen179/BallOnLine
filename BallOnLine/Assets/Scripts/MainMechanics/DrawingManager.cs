using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem; // Yeni Input System kütüphanesi eklendi
using UnityEngine.UI;
using UnityEngine.EventSystems; // --- 1. YENÝ EKLENDÝ: UI Tespiti Ýçin ---

public class DrawingManager : MonoBehaviour
{
    public static DrawingManager Instance { get; private set; }

    [Header("Line Settings")]
    public GameObject linePrefab;
    private GameObject currentLine;
    public static List<GameObject> lines = new List<GameObject>();

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;
    public List<Vector2> fingerPositions;

    public float minPointDistance = 0.25f; // Bu deðeri Inspector'dan line kalýnlýðýna göre artýrýp azaltabilirsin.

    [Header("Ink Settings")]
    public int lineCount = 0;
    public static float inkAmount;
    public static float maxInkAmount = 100f;
    public float inkDecreaseRate = -0.2f; // Çizim sýrasýnda mürekkep azalým hýzý
    public TextMeshProUGUI inkAmountText;

    [Header("Game State")]
    // Butona basýldýðýnda bu deðeri GameManager veya UI üzerinden 'true' yapmalýsýn.
    public bool isGameActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        maxInkAmount = LevelManager.Instance.currentLevel.maxInkAmount;
        inkAmount = maxInkAmount;
    }

    void Update()
    {
        // Level donuk durumdayken çizim yapýlmasýný engeller
        if (!isGameActive)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.StopDrawingSound();
            return;
        }

        UpdateDrawingProgressBar();
        if (inkAmountText != null)
        {
            inkAmountText.text = "% " + Mathf.CeilToInt(inkAmount).ToString();
        }

        // Eðer sistemde herhangi bir iþaretçi (fare, dokunmatik vb.) yoksa metodu sonlandýr
        if (Pointer.current == null) return;

        // Yeni Input System ile basýlma ve pozisyon verilerini okuma
        bool isPressedDown = Pointer.current.press.wasPressedThisFrame;
        bool isHeldDown = Pointer.current.press.isPressed;
        bool isReleased = Pointer.current.press.wasReleasedThisFrame;

        Vector2 pointerScreenPos = Pointer.current.position.ReadValue();


        bool isPointerOverUI = false;
        if (EventSystem.current != null)
        {
            isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
        }

        // Parmaðýmýzla bir kere bastýðýmýzda yapýlacak iþlemler
        if (isPressedDown && inkAmount > 0f)
        {
            CreateLine(pointerScreenPos);
            UpdateInkAmount(inkDecreaseRate);

            // Çizim sesini baþlat
            if (AudioManager.Instance != null) AudioManager.Instance.StartDrawingSound();
        }
        // Parmaðýmýzý basýlý tuttuðumuzda yapýlacak iþlemler
        else if (isHeldDown && inkAmount > 0f)
        {
            Vector2 tempFingerPos = Camera.main.ScreenToWorldPoint(pointerScreenPos);

            // Çizgiler arasýndaki mesafe kontrolü (Çok sýk nokta eklememek için)
            if (fingerPositions.Count > 0 && Vector2.Distance(tempFingerPos, fingerPositions[fingerPositions.Count - 1]) > minPointDistance)
            {
                UpdateLine(tempFingerPos);
                UpdateInkAmount(inkDecreaseRate);
            }

            // --- YENÝ EKLENEN PITCH MATEMATÝÐÝ ---
            if (AudioManager.Instance != null)
            {
                float calculatedPitch = 1f; // Standart pitch (Normal ses)

                if (LevelManager.Instance.currentLevel.axis == Level.levelAxis.YAxis)
                {
                    // InverseLerp: -11'deyken 0 verir, 11'deyken 1 verir.
                    float normalizedY = Mathf.InverseLerp(-4f, 4f, tempFingerPos.x);

                    // Sonra bunu 0.5 (Tok/Kalýn) ile 1.5 (Tiz) arasýna çeviriyoruz.
                    calculatedPitch = Mathf.Lerp(0.5f, 1.5f, normalizedY);
                }
                else if (LevelManager.Instance.currentLevel.axis == Level.levelAxis.XAxis)
                {
                    float normalizedX = Mathf.InverseLerp(-7.5f, 7.5f, tempFingerPos.y);
                    calculatedPitch = Mathf.Lerp(0.5f, 1.5f, normalizedX);
                }

                AudioManager.Instance.UpdateDrawingPitch(calculatedPitch);
            }
        }
        // Mürekkep bittiyse veya parmak çekildiyse sesi durdur
        else if (isReleased || inkAmount <= 0f)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.StopDrawingSound();
        }
    }

    void CreateLine(Vector2 screenPos)
    {
        lineCount++;
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lines.Add(currentLine);

        lineRenderer = currentLine.GetComponent<LineRenderer>();
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();

        fingerPositions.Clear();

        // Ekrana ilk dokunulan noktayý Dünya (World) koordinatlarýna çevirme
        Vector2 startPos = Camera.main.ScreenToWorldPoint(screenPos);
        fingerPositions.Add(startPos);
        fingerPositions.Add(startPos);

        lineRenderer.SetPosition(0, fingerPositions[0]);
        lineRenderer.SetPosition(1, fingerPositions[1]);

        edgeCollider.points = fingerPositions.ToArray();
    }

    void UpdateLine(Vector2 newFingerPos)
    {
        if(LevelManager.Instance.currentLevel.axis == Level.levelAxis.XAxis)
        {
            newFingerPos.x = Mathf.Clamp(newFingerPos.x, -3.5f, 100f);
        }
        else if(LevelManager.Instance.currentLevel.axis == Level.levelAxis.YAxis)
        {
            newFingerPos.x = Mathf.Clamp(newFingerPos.x, -11f, 11f);
        }
        // Çizimin ekran dýþýna taþmasýný engelleme
        fingerPositions.Add(newFingerPos);

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newFingerPos);

        edgeCollider.points = fingerPositions.ToArray();
    }

    public void UpdateDrawingProgressBar()
    {
        if (UIManager.Instance.inkAmountBar != null)
        {
            UIManager.Instance.inkAmountBar.value = inkAmount / maxInkAmount;
        }
    }

    public void UpdateInkAmount(float addedAmount)
    {
        if (LevelManager.Instance.currentLevel.isInkLimited)
        {
            inkAmount += addedAmount;
        }

        if(inkAmount > maxInkAmount)
        {
            inkAmount = maxInkAmount;
        }
    }
}