using System.Collections; // Coroutine (IEnumerator) kullanabilmek için gerekli
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [Header("Spawn Ayarlarý")]
    public GameObject ballPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 4f; // Time between spawns

    void Start()
    {
        // Oyun (veya menü) baþladýðýnda sonsuz spawn döngüsünü baþlatýyoruz
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // Obje sahnede var olduðu sürece sonsuza kadar çalýþacak bir döngü
        while (true)
        {
            SpawnBall();

            // Belirlenen saniye kadar bekle, sonra döngünün baþýna dön
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnBall()
    {
        // Güvenlik kontrolü: Prefab veya spawn noktalarý boþsa hata vermesini engelle
        if (ballPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("BallSpawner: Prefab veya Spawn Noktalarý eksik!");
            return;
        }

        // 0 ile dizinin uzunluðu arasýnda rastgele bir sayý (index) seç
        int randomIndex = Random.Range(0, spawnPoints.Length);

        // Rastgele seçilen noktayý al
        Transform selectedPoint = spawnPoints[randomIndex];

        // Topu o noktanýn pozisyonunda üret
        Instantiate(ballPrefab, selectedPoint.position, selectedPoint.rotation);
    }
}