using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; 
using DG.Tweening; // EKLENDÝ: DOTween kütüphanesi

public class Menu_DrawingManager : MonoBehaviour
{
    public static Menu_DrawingManager Instance { get; private set; }

    [Header("Line Settings")]
    public GameObject linePrefab;
    private GameObject currentLine;
    public static List<GameObject> lines = new List<GameObject>();

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;
    public List<Vector2> fingerPositions;

    public float minPointDistance = 0.25f; 

    [Header("Ink Settings")]
    public int lineCount = 0;
    public static float inkAmount;
    public static float maxInkAmount = 100f;
    public float inkDecreaseRate = -0.2f; 
    public TextMeshProUGUI inkAmountText;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    private void Start()
    {
        maxInkAmount = 100;
        inkAmount = maxInkAmount;
    }

    void Update()
    {
        if (inkAmountText != null)
        {
            inkAmountText.text = "% " + Mathf.CeilToInt(inkAmount).ToString();
        }

        if (Pointer.current == null) return;

        // Yeni Input System ile basýlma, basýlý tutma ve BIRAKILMA (YENÝ) verilerini okuma
        bool isPressedDown = Pointer.current.press.wasPressedThisFrame;
        bool isHeldDown = Pointer.current.press.isPressed;
        bool isReleased = Pointer.current.press.wasReleasedThisFrame; // EKLENDÝ
        
        Vector2 pointerScreenPos = Pointer.current.position.ReadValue();

        // 1. Parmaðýmýzla bir kere bastýðýmýzda
        if (isPressedDown && inkAmount > 0f)
        {
            CreateLine(pointerScreenPos);
        }
        // 2. Parmaðýmýzý basýlý tuttuðumuzda
        else if (isHeldDown && inkAmount > 0f && currentLine != null)
        {
            Vector2 tempFingerPos = Camera.main.ScreenToWorldPoint(pointerScreenPos);

            if (fingerPositions.Count > 0 && Vector2.Distance(tempFingerPos, fingerPositions[fingerPositions.Count - 1]) > minPointDistance)
            {
                UpdateLine(tempFingerPos);
            }
        }
        // 3. EKLENEN KISIM: Parmaðýmýzý ekrandan kaldýrdýðýmýzda
        else if (isReleased && currentLine != null)
        {
            // Çizim bittiði an erime/yok olma iþlemini baþlatýyoruz
            FadeAndDestroyLine(currentLine);
            
            // Yeni bir çizgi çizilene kadar currentLine'ý boþa alýyoruz
            currentLine = null; 
        }
    }

    void CreateLine(Vector2 screenPos)
    {
        //AudioManager.Instance.PlayAudioClip("Sound_Drawing");

        lineCount++;
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lines.Add(currentLine);

        lineRenderer = currentLine.GetComponent<LineRenderer>();
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();

        fingerPositions.Clear();

        Vector2 startPos = Camera.main.ScreenToWorldPoint(screenPos);
        fingerPositions.Add(startPos);
        fingerPositions.Add(startPos);

        lineRenderer.SetPosition(0, fingerPositions[0]);
        lineRenderer.SetPosition(1, fingerPositions[1]);

        edgeCollider.points = fingerPositions.ToArray();
    }

    void UpdateLine(Vector2 newFingerPos)
    {
        newFingerPos.x = Mathf.Clamp(newFingerPos.x, -11f, 11f);
        fingerPositions.Add(newFingerPos);

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newFingerPos);

        edgeCollider.points = fingerPositions.ToArray();
    }

    // --- GÜNCELLENEN METOT ---
    void FadeAndDestroyLine(GameObject lineObj)
    {
        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        if (lr != null)
        {
            // DOFade yerine, Shader'a özel eklediðimiz "_Alpha" deðiþkenini DOFloat ile 0'a indiriyoruz.
            lr.material.DOFloat(0f, "_Alpha", 5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                // 5 Saniyelik erime iþlemi bittiðinde:
                lines.Remove(lineObj);
                Destroy(lineObj);
            });
        }
    }
}