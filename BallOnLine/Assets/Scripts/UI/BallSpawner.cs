using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 4f;

    [Tooltip("Boþ býrakýrsan alt objelerdeki (Child) TrailRenderer'ý otomatik bulur.")]
    public TrailRenderer ballTrail;

    void Start()
    {        
        StartCoroutine(SpawnRoutine());
    }

    System.Collections.IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnBall();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnBall()
    {
        if (ballPrefab == null || spawnPoints == null || spawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedPoint = spawnPoints[randomIndex];

        // 1. Topun Kopyasýný Sahnede Oluþtur
        GameObject newBall = Instantiate(ballPrefab, selectedPoint.position, selectedPoint.rotation);
        ballTrail = newBall.GetComponentInChildren<TrailRenderer>();

        // 2. Kopyanýn materyalini SkinManager'dan alýp deðiþtir (sharedMaterial YERÝNE sadece material)
        if (SkinManager.Instance != null)
        {
            ShopItemSO equippedSkin = SkinManager.Instance.GetEquippedBallSkin();
            if (equippedSkin != null)
            {
                SpriteRenderer spriteRenderer = newBall.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.material = equippedSkin.shopItemMaterial;
                    spriteRenderer.material.color = equippedSkin.shopItemColor;
                }
            }

            // 2. Seçili ÝZ (Trail) Skinini Uygula
            ShopItemSO equippedTrail = SkinManager.Instance.GetEquippedTrailSkin();
            
            if (equippedTrail != null && ballTrail != null)
            {
                equippedTrail.ApplyToTrailRenderer(ballTrail);
            }
        }
    }
}