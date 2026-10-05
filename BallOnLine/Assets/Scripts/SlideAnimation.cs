using UnityEngine;
using DG.Tweening;

public class SlideAnimation : MonoBehaviour
{
    [Header("Animasyon Ayarlarý")]
    [Tooltip("Ýkonun gideceði yön ve mesafe. Y ekseninde kaydýrma için (0, -100, 0) gibi bir deðer girebilirsin.")]
    public Vector3 moveOffset = new Vector3(0f, -50f, 0f);

    [Tooltip("Git-gel hareketinin bir tur süresi.")]
    public float duration = 0.8f;

    private void Start()
    {
        // Mevcut pozisyonu al ve belirlediðimiz offset kadar ileri taþý.
        // SetLoops(-1, LoopType.Yoyo) sayesinde sonsuza kadar baþlangýç ve bitiþ noktasý arasýnda gidip gelir.
        transform.DOLocalMove(transform.localPosition + moveOffset, duration)
                 .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo)
                 .SetUpdate(true); // Eðer oyunu duraklatýrsan animasyonun donmamasý için
    }

    private void OnDestroy()
    {
        // Obje silindiðinde (veya level baþladýðýnda gizlendiðinde) arkada çalýþan Tween'i temizle
        transform.DOKill();
    }
}