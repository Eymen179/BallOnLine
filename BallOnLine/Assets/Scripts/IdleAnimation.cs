using UnityEngine;
using DG.Tweening; // DOTween kütüphanesi zorunlu

public class IdleAnimation : MonoBehaviour
{
    [Header("Hover (Yukarý-Aþaðý Süzülme)")]
    public bool enableHover = true;
    [Tooltip("Obje Y ekseninde ne kadar yukarý çýkýp insin?")]
    public float hoverDistance = 0.5f;
    public float hoverDuration = 2f;
    public Ease hoverEase = Ease.InOutSine;

    [Header("Scale (Nefes Alma / Nabýz)")]
    public bool enableScale = false;
    [Tooltip("Obje orijinal boyutunun yüzde kaçýna çýksýn? (Örn: 1.1 = %10 büyüme)")]
    public float scaleMultiplier = 1.1f;
    public float scaleDuration = 1.5f;
    public Ease scaleEase = Ease.InOutSine;

    [Header("Rotation (Sallanma)")]
    public bool enableRotation = false;
    [Tooltip("Hangi eksende kaç derece dönsün? (2D için genelde Z ekseni kullanýlýr)")]
    public Vector3 rotationAngle = new Vector3(0, 0, 15f);
    public float rotationDuration = 2f;
    public Ease rotationEase = Ease.InOutSine;

    // Baþlangýç deðerlerini hafýzada tutuyoruz
    private Vector3 startPos;
    private Vector3 startScale;
    private Vector3 startRot;

    void Start()
    {
        // Objenin sahnede konulduðu ilk pozisyonlarý referans al
        startPos = transform.localPosition;
        startScale = transform.localScale;
        startRot = transform.localEulerAngles;

        // Açýk olan animasyonlarý baþlat
        if (enableHover) StartHover();
        if (enableScale) StartScale();
        if (enableRotation) StartRotation();
    }

    void StartHover()
    {
        // Yoyo ile gidip geri gelmesini, -1 ile sonsuza kadar tekrarlamasýný saðlýyoruz
        transform.DOLocalMoveY(startPos.y + hoverDistance, hoverDuration)
            .SetEase(hoverEase)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void StartScale()
    {
        transform.DOScale(startScale * scaleMultiplier, scaleDuration)
            .SetEase(scaleEase)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void StartRotation()
    {
        transform.DOLocalRotate(startRot + rotationAngle, rotationDuration)
            .SetEase(rotationEase)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        // ÇOK ÖNEMLÝ: Obje yok edildiðinde (top objeyi aldýðýnda) DOTween'i temizle.
        // Yoksa konsolda "Missing Reference" hatasý alýrsýn.
        transform.DOKill();
    }
}