using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Yeni Input System kütüphanesi eklendi

public class DrawingManager : MonoBehaviour
{
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
    public static float inkAmount = 100f;
    public float inkDecreaseRate = -0.2f; // Çizim sýrasýnda mürekkep azalým hýzý
    public TextMeshProUGUI inkAmountText;

    [Header("Game State")]
    // Butona basýldýðýnda bu deðeri GameManager veya UI üzerinden 'true' yapmalýsýn.
    public bool isGameActive = false;

    private void Start()
    {

    }

    void Update()
    {
        // Level donuk durumdayken çizim yapýlmasýný engeller
        if (!isGameActive) return;

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
        Vector2 pointerScreenPos = Pointer.current.position.ReadValue();

        // Parmaðýmýzla bir kere bastýðýmýzda yapýlacak iþlemler
        if (isPressedDown && inkAmount > 0f)
        {
            CreateLine(pointerScreenPos);
            UpdateInkAmount(inkDecreaseRate);

            //Ses
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
        // Çizimin ekran dýþýna taþmasýný engelleme
        newFingerPos.x = Mathf.Clamp(newFingerPos.x, -11f, 11f);
        fingerPositions.Add(newFingerPos);

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newFingerPos);

        edgeCollider.points = fingerPositions.ToArray();
    }

    public void UpdateDrawingProgressBar()
    {
        if (UIManager.Instance.inkAmountBar != null)
        {
            UIManager.Instance.inkAmountBar.value = inkAmount / 100f;
        }
    }

    public void UpdateInkAmount(float addedAmount)
    {
        inkAmount += addedAmount;

        if(inkAmount > 100f)
        {
            inkAmount = 100f;
        }
    }
    void InkLimitControl()
    {

    }
}