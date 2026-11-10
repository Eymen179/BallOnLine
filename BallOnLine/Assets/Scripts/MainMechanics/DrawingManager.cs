using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DrawingManager : MonoBehaviour
{
    public static DrawingManager Instance { get; private set; }

    [Header("Line Settings")]
    public GameObject linePrefab;
    private GameObject currentLine;
    public static List<GameObject> lines = new List<GameObject>();

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;
    private Rigidbody2D currentRb;
    public List<Vector2> fingerPositions;

    public float minPointDistance = 0.25f;

    [Header("Ink Settings")]
    public int lineCount = 0;
    public static float inkAmount;
    public static float maxInkAmount = 100f;
    public float inkDecreaseRate = -0.2f;
    public TextMeshProUGUI inkAmountText;

    [Header("Game State")]
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
        // 1. Oyun baþlamadýysa çizimi engelle
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

        if (Pointer.current == null) return;

        bool isPressedDown = Pointer.current.press.wasPressedThisFrame;
        bool isHeldDown = Pointer.current.press.isPressed;
        bool isReleased = Pointer.current.press.wasReleasedThisFrame;

        Vector2 pointerScreenPos = Pointer.current.position.ReadValue();

        // 2. Çizim Baþlangýcý (UI kilidi kaldýrýldý)
        if (isPressedDown && inkAmount > 0f)
        {
            CreateLine(pointerScreenPos);
            UpdateInkAmount(inkDecreaseRate);

            if (AudioManager.Instance != null) AudioManager.Instance.StartDrawingSound();
        }
        // 3. Çizim Devamý
        else if (isHeldDown && inkAmount > 0f && currentLine != null)
        {
            Vector2 tempFingerPos = Camera.main.ScreenToWorldPoint(pointerScreenPos);

            if (fingerPositions.Count > 0 && Vector2.Distance(tempFingerPos, fingerPositions[fingerPositions.Count - 1]) > minPointDistance)
            {
                UpdateLine(tempFingerPos);
                UpdateInkAmount(inkDecreaseRate);
            }

            if (AudioManager.Instance != null)
            {
                float calculatedPitch = 1f;

                if (LevelManager.Instance.currentLevel.axis == Level.levelAxis.YAxis)
                {
                    float normalizedY = Mathf.InverseLerp(-4f, 4f, tempFingerPos.x);
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
        // 4. Çizim Bitiþi (Býrakýldýðý an yerçekimini devreye sok)
        else if ((isReleased || inkAmount <= 0f) && currentLine != null)
        {
            FinishCurrentLine();

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
        currentRb = currentLine.GetComponent<Rigidbody2D>();

        // Çizim esnasýnda koordinat kaymasýný önlemek için standart dünya uzayýnda baþlar
        if (lineRenderer != null)
        {
            lineRenderer.useWorldSpace = true;
        }

        // Çizerken fizik hareket etmesin
        if (currentRb != null)
        {
            currentRb.bodyType = RigidbodyType2D.Kinematic;
            currentRb.linearVelocity = Vector2.zero;
            currentRb.angularVelocity = 0f;
        }

        fingerPositions.Clear();

        Vector2 startPos = Camera.main.ScreenToWorldPoint(screenPos);
        fingerPositions.Add(startPos);
        fingerPositions.Add(startPos);

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, fingerPositions[0]);
            lineRenderer.SetPosition(1, fingerPositions[1]);
        }

        if (edgeCollider != null)
        {
            edgeCollider.points = fingerPositions.ToArray();
        }
    }

    void UpdateLine(Vector2 newFingerPos)
    {
        if (LevelManager.Instance.currentLevel.axis == Level.levelAxis.XAxis)
        {
            newFingerPos.x = Mathf.Clamp(newFingerPos.x, -50f, 100f);
        }
        else if (LevelManager.Instance.currentLevel.axis == Level.levelAxis.YAxis)
        {
            newFingerPos.x = Mathf.Clamp(newFingerPos.x, -11f, 11f);
        }

        fingerPositions.Add(newFingerPos);

        if (lineRenderer != null)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, newFingerPos);
        }

        if (edgeCollider != null)
        {
            edgeCollider.points = fingerPositions.ToArray();
        }
    }

    private void FinishCurrentLine()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.currentLevel != null)
        {
            // Ýlgili levelde çizgilere yerçekimi etki edecekse
            if (LevelManager.Instance.currentLevel.isLineRigidbodyActive && currentRb != null)
            {
                // 1. Geometrik merkezi (aðýrlýk merkezini) hesapla
                Vector2 centroid = Vector2.zero;
                for (int i = 0; i < fingerPositions.Count; i++)
                {
                    centroid += fingerPositions[i];
                }
                if (fingerPositions.Count > 0)
                {
                    centroid /= fingerPositions.Count;
                }

                // 2. Çizgiyi objenin yerel merkezine taþýyýp Transform'u oraya yerleþtir (Kaymayý önler)
                currentLine.transform.position = centroid;

                Vector2[] localColliderPoints = new Vector2[fingerPositions.Count];
                for (int i = 0; i < fingerPositions.Count; i++)
                {
                    Vector2 localPt = fingerPositions[i] - centroid;
                    localColliderPoints[i] = localPt;
                    lineRenderer.SetPosition(i, localPt);
                }

                // Görseli yerel uzaya alarak GameObject ile birlikte düþmesini saðla
                lineRenderer.useWorldSpace = false;
                edgeCollider.points = localColliderPoints;

                currentRb.centerOfMass = Vector2.zero;
                currentRb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        currentLine = null;
        currentRb = null;
    }

    public void UpdateDrawingProgressBar()
    {
        if (UIManager.Instance != null && UIManager.Instance.inkAmountBar != null)
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

        if (inkAmount > maxInkAmount)
        {
            inkAmount = maxInkAmount;
        }
    }
}