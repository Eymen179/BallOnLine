using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour
{
    [Header("Hareket Noktalarý")]
    public Transform point1; // Baþlangýç noktasý
    public Transform point2; // Hedef nokta

    [Header("Ayarlar")]
    public float moveDuration = 2f; // Bir noktadan diðerine gitme süresi
    public Ease moveEase = Ease.Linear; // Hareketin ivmesi (Linear = Sabit hýz)

    private void Start()
    {
        // Güvenlik kontrolü
        if (point1 == null || point2 == null)
        {
            Debug.LogWarning("MovingPlatform: Lütfen Point1 ve Point2 objelerini atayýn!");
            return;
        }

        // Platformu oyun baþladýðýnda kesin olarak birinci noktaya oturt
        transform.position = point1.position;

        // --- DOTWEEN HAREKET SÝSTEMÝ ---
        // point2'ye doðru git, iþlemi sonsuza kadar (-1) git-gel (Yoyo) þeklinde tekrarla
        transform.DOMove(point2.position, moveDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(moveEase)
            .SetUpdate(UpdateType.Fixed); // Fizik motoruyla %100 uyumlu çalýþmasý için
    }

    private void OnDestroy()
    {
        // Obje yok edilirse veya sahne deðiþirse DOTween'i temizle (Hata almamak için)
        transform.DOKill();
    }
}