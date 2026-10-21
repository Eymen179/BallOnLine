using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Level Verisi")]
    [Tooltip("Yeni levelin Scriptable Object dosyasý")]
    public Level levelData;

    [Header("Referanslar (Sahnede Var Olanlar)")]
    public Transform platformsParent;      // Hiyerarþideki 'Platforms' objesi
    public Transform ballTransform;        // 'Ball' objesi
    public Transform portalTransform;      // 'Portal' objesi
    public BoxCollider2D levelBoundsCollider; // 'LevelBounds' üzerindeki BoxCollider2D

    [Header("Görsel & Materyal Ayarlarý")]
    public Sprite wallSprite;              // Düz kare/dikdörtgen sprite
    public Material platformMaterial;      // Neon duvar materyali

    [Header("Tünel Boyut Ayarlarý")]
    public float levelLength = 40f;
    public float corridorWidth = 8f;
    public float wallThickness = 2f;

    [Header("Ýç Platform Üretim Ayarlarý")]
    [Tooltip("Platform ýzgara hücre boyutu (Metre cinsinden önerilen: 2f)")]
    public float cellSize = 2f;
    [Tooltip("Boþ alanlara dörtgen platform koyulma sýklýðý (1.0 = Arka planý tamamen doldurur)")]
    [Range(0.15f, 1.0f)] public float platformDensity = 1.0f;

    [Header("Yol Kývrým ve Çýkýntý Ayarlarý")]
    [Tooltip("Yolun zikzak/kývrým yapma þiddeti (0 = Dümdüz yol, 1 = Duvarlara çarpan tam zikzak)")]
    [Range(0f, 1f)] public float pathWinding = 0.6f;

    [Tooltip("Yol boyunca oluþacak zikzak/dalga sayýsý")]
    [Range(1, 8)] public int windingSegments = 3;

    [Tooltip("Duvardan yolun içine doðru uzanacak blok çýkýntýlarýnýn (engellerin) sýklýðý")]
    [Range(0f, 1f)] public float protrusionChance = 0.35f;

    [Tooltip("Yolun güvenlik geniþliði (1 = Dar ve bol çýkýntýlý, 2 = Geniþ ve ferah)")]
    [Range(1, 2)] public int pathMargin = 1;

    // Grid durumlarý
    private enum CellType { Empty, CorePath, PathMargin, Rectangle }
    private CellType[,] grid;
    private int gridCols;
    private int gridRows;

    [ContextMenu("Bölüm Ýskeletini Oluþtur")]
    public void GenerateScaffolding()
    {
        if (levelData == null)
        {
            Debug.LogError("Lütfen önce bir Level ScriptableObject atayýn!");
            return;
        }

        ClearAllPlatforms();

        if (levelData.axis == Level.levelAxis.YAxis)
        {
            GenerateVerticalLevel();
        }
        else
        {
            GenerateHorizontalLevel();
        }

        LevelManager lm = FindFirstObjectByType<LevelManager>();
        if (lm != null) lm.currentLevel = levelData;

        Debug.Log($"<color=green>[LevelGenerator]</color> {levelData.levelName} baþarýyla oluþturuldu ve yolu doðrulandý!");
    }

    private void GenerateVerticalLevel()
    {
        float halfWidth = corridorWidth / 2f;
        float halfLength = levelLength / 2f;

        // Dýþ Duvarlar
        CreateWall("Wall_Left", new Vector2(-halfWidth - (wallThickness / 2f), 0), new Vector2(wallThickness, levelLength));
        CreateWall("Wall_Right", new Vector2(halfWidth + (wallThickness / 2f), 0), new Vector2(wallThickness, levelLength));
        CreateWall("Wall_Top", new Vector2(0, halfLength + (wallThickness / 2f)), new Vector2(corridorWidth + (wallThickness * 2), wallThickness));
        CreateWall("Wall_Bottom", new Vector2(0, -halfLength - (wallThickness / 2f)), new Vector2(corridorWidth + (wallThickness * 2), wallThickness));

        // --- TOP VE PORTAL KONUMLANDIRMA (Y Ekseni) ---
        float safeX = halfWidth - 2f;
        Vector3 ballPos = new Vector3(Random.Range(-safeX, safeX), halfLength - 3f, 0);
        Vector3 portalPos = new Vector3(Random.Range(-safeX, safeX), -halfLength + 3f, 0);

        if (ballTransform != null) ballTransform.position = ballPos;
        if (portalTransform != null) portalTransform.position = portalPos;

        // --- ÝÇ PLATFORM DOLDURMA, KIVRIMLAR VE ÇIKINTILAR ---
        GenerateInteriorGrid(ballPos, portalPos, isVertical: true);

        // LevelBounds (Senin özel ayarlarýn korundu)
        /*
         * ----------------
         * Y ekseni
         * X -> X + 9f (corridorWidth + 9f)
         * Y -> Y + 2f (levelLength + 2f)
         * levelLength = 40 (ort)
         * corridorWidth = 6
         * wallThickness = 2
         * ------------------
         * X ekseni
         * X -> X + 2f (levelLength + 2f)
         * Y -> Y + 3f 
         * levelLength = 40 (ort)
         * corridorWidth = 12
         * wallThickness = 2
         */
        if (levelBoundsCollider != null)
        {
            levelBoundsCollider.offset = Vector2.zero;
            levelBoundsCollider.size = new Vector2(corridorWidth + 9f, levelLength + 2f);
        }
    }

    private void GenerateHorizontalLevel()
    {
        float halfLength = levelLength / 2f;
        float halfWidth = corridorWidth / 2f;

        // Dýþ Duvarlar
        CreateWall("Wall_Top", new Vector2(0, halfWidth + (wallThickness / 2f)), new Vector2(levelLength, wallThickness));
        CreateWall("Wall_Bottom", new Vector2(0, -halfWidth - (wallThickness / 2f)), new Vector2(levelLength, wallThickness));
        CreateWall("Wall_Left", new Vector2(-halfLength - (wallThickness / 2f), 0), new Vector2(wallThickness, corridorWidth + (wallThickness * 2)));
        CreateWall("Wall_Right", new Vector2(halfLength + (wallThickness / 2f), 0), new Vector2(wallThickness, corridorWidth + (wallThickness * 2)));

        // --- TOP VE PORTAL KONUMLANDIRMA (X Ekseni) ---
        float safeY = halfWidth - 2f;
        float ballY = Random.Range(-safeY, safeY);
        float portalY = Random.Range(-safeY, safeY);

        float ballX = -halfLength + Random.Range(3f, 6f);

        float minDistance = levelLength * 0.5f;
        float maxDistance = (halfLength - 3f) - ballX;
        float chosenDistance = Random.Range(minDistance, Mathf.Max(minDistance, maxDistance));
        float portalX = Mathf.Clamp(ballX + chosenDistance, -halfLength + minDistance, halfLength - 3f);

        Vector3 ballPos = new Vector3(ballX, ballY, 0);
        Vector3 portalPos = new Vector3(portalX, portalY, 0);

        if (ballTransform != null) ballTransform.position = ballPos;
        if (portalTransform != null) portalTransform.position = portalPos;

        // --- ÝÇ PLATFORM DOLDURMA, KIVRIMLAR VE ÇIKINTILAR ---
        GenerateInteriorGrid(ballPos, portalPos, isVertical: false);

        // LevelBounds (Senin özel ayarlarýn korundu)
        /*
         * ----------------
         * Y ekseni
         * X -> X + 9f (corridorWidth + 9f)
         * Y -> Y + 2f (levelLength + 2f)
         * levelLength = 40 (ort)
         * corridorWidth = 6
         * wallThickness = 2
         * ------------------
         * X ekseni
         * X -> X + 2f (levelLength + 2f)
         * Y -> Y + 5f 
         * levelLength = 40 (ort)
         * corridorWidth = 12
         * wallThickness = 3
         */
        if (levelBoundsCollider != null)
        {
            levelBoundsCollider.offset = Vector2.zero;
            levelBoundsCollider.size = new Vector2(levelLength + 2f, corridorWidth + 4f);
        }
    }

    // =========================================================================
    // --- YORDAMSAL ÝÇ PLATFORM, KIVRIM VE ÇIKINTI MOTORU ---
    // =========================================================================
    private void GenerateInteriorGrid(Vector3 ballPos, Vector3 portalPos, bool isVertical)
    {
        float totalWidth = isVertical ? corridorWidth : levelLength;
        float totalHeight = isVertical ? levelLength : corridorWidth;

        gridCols = Mathf.Max(3, Mathf.FloorToInt(totalWidth / cellSize));
        gridRows = Mathf.Max(3, Mathf.FloorToInt(totalHeight / cellSize));

        grid = new CellType[gridCols, gridRows];

        Vector2Int startCell = WorldToGrid(ballPos, totalWidth, totalHeight);
        Vector2Int targetCell = WorldToGrid(portalPos, totalWidth, totalHeight);

        // 1. KIVRIMLI ZÝKZAK YOLU OY
        CarveWindingPath(startCell, targetCell, isVertical);

        // 2. BOÞ ALANLARI PLATFORM ÝLE DOLDUR (platformDensity)
        for (int c = 0; c < gridCols; c++)
        {
            for (int r = 0; r < gridRows; r++)
            {
                if (grid[c, r] == CellType.Empty)
                {
                    if (Random.value < platformDensity)
                    {
                        grid[c, r] = CellType.Rectangle;
                    }
                }
            }
        }

        // 3. YOL KENARLARINA ÇIKINTILAR (PROTRUSIONS) EKLE
        if (protrusionChance > 0f)
        {
            AddPathProtrusions(startCell, targetCell);
        }

        // 4. BELÝRLENEN TÜM RECTANGLE HÜCRELERÝNÝ DÜNYAYA ÇÝZ
        Vector2 blockSize = new Vector2(cellSize, cellSize);
        for (int c = 0; c < gridCols; c++)
        {
            for (int r = 0; r < gridRows; r++)
            {
                if (grid[c, r] == CellType.Rectangle)
                {
                    Vector2 worldCenter = GridToWorld(c, r, totalWidth, totalHeight);
                    CreateInternalRectangle($"Platform_Box_{c}_{r}", worldCenter, blockSize);
                }
            }
        }

        // 5. YOLU DOÐRULA
        bool isPathValid = ValidatePathExists(startCell, targetCell);
        if (isPathValid)
        {
            Debug.Log("<color=cyan>[A* / BFS Doðrulayýcý]</color> Kývrýmlý yol ve çýkýntýlar baþarýyla test edildi, tünel açýk!");
        }
    }

    // Slalom / zikzak rotasýný oluþturan fonksiyon
    private void CarveWindingPath(Vector2Int start, Vector2Int target, bool isVertical)
    {
        List<Vector2Int> waypoints = new List<Vector2Int>();
        waypoints.Add(start);

        if (isVertical)
        {
            float midCol = (gridCols - 1) / 2f;
            float maxOffset = (gridCols - 1) / 2f;

            for (int i = 1; i <= windingSegments; i++)
            {
                int r = Mathf.RoundToInt(Mathf.Lerp(start.y, target.y, (float)i / (windingSegments + 1)));
                int side = (i % 2 == 1) ? -1 : 1; // Bir sola, bir saða savrul
                int c = Mathf.RoundToInt(midCol + (side * maxOffset * pathWinding));
                c = Mathf.Clamp(c, 0, gridCols - 1);

                waypoints.Add(new Vector2Int(c, r));
            }
        }
        else
        {
            float midRow = (gridRows - 1) / 2f;
            float maxOffset = (gridRows - 1) / 2f;

            for (int i = 1; i <= windingSegments; i++)
            {
                int c = Mathf.RoundToInt(Mathf.Lerp(start.x, target.x, (float)i / (windingSegments + 1)));
                int side = (i % 2 == 1) ? 1 : -1; // Bir yukarý, bir aþaðý savrul
                int r = Mathf.RoundToInt(midRow + (side * maxOffset * pathWinding));
                r = Mathf.Clamp(r, 0, gridRows - 1);

                waypoints.Add(new Vector2Int(c, r));
            }
        }

        waypoints.Add(target);

        // Noktalarý birbirine baðlayarak çekirdek yolu ve koruma marjýný oy
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            ConnectWaypoints(waypoints[i], waypoints[i + 1]);
        }
    }

    private void ConnectWaypoints(Vector2Int p1, Vector2Int p2)
    {
        Vector2Int cur = p1;
        while (cur != p2)
        {
            MarkPathCell(cur);

            if (cur.x != p2.x && (cur.y == p2.y || Random.value < 0.5f))
            {
                cur.x += (p2.x > cur.x) ? 1 : -1;
            }
            else if (cur.y != p2.y)
            {
                cur.y += (p2.y > cur.y) ? 1 : -1;
            }
            else
            {
                cur.x += (p2.x > cur.x) ? 1 : -1;
            }
        }
        MarkPathCell(p2);
    }

    private void MarkPathCell(Vector2Int cell)
    {
        grid[cell.x, cell.y] = CellType.CorePath;

        // Yolun kenar marjýný iþaretle
        for (int x = -pathMargin; x <= pathMargin; x++)
        {
            for (int y = -pathMargin; y <= pathMargin; y++)
            {
                int nx = cell.x + x;
                int ny = cell.y + y;
                if (nx >= 0 && nx < gridCols && ny >= 0 && ny < gridRows)
                {
                    if (grid[nx, ny] == CellType.Empty)
                    {
                        grid[nx, ny] = CellType.PathMargin;
                    }
                }
            }
        }
    }

    // Yol kenarlarýna güvenli çýkýntý (engel) ekleyen fonksiyon
    private void AddPathProtrusions(Vector2Int start, Vector2Int target)
    {
        List<Vector2Int> candidates = new List<Vector2Int>();

        for (int c = 0; c < gridCols; c++)
        {
            for (int r = 0; r < gridRows; r++)
            {
                // Sadece yolun kýyýsýndaki koruma marjý hücreleri çýkýntý adayýdýr
                if (grid[c, r] == CellType.PathMargin)
                {
                    // Top ve portalýn doðuþ noktalarýný doðrudan týkama
                    if (Vector2Int.Distance(new Vector2Int(c, r), start) > 1.5f &&
                        Vector2Int.Distance(new Vector2Int(c, r), target) > 1.5f)
                    {
                        candidates.Add(new Vector2Int(c, r));
                    }
                }
            }
        }

        // Adaylarý karýþtýr
        for (int i = 0; i < candidates.Count; i++)
        {
            int rnd = Random.Range(i, candidates.Count);
            Vector2Int temp = candidates[i];
            candidates[i] = candidates[rnd];
            candidates[rnd] = temp;
        }

        foreach (var pos in candidates)
        {
            if (Random.value < protrusionChance)
            {
                // Çýkýntýyý geçici olarak yerleþtir
                grid[pos.x, pos.y] = CellType.Rectangle;

                // Yolu kapatýp kapatmadýðýný anlýk BFS ile kontrol et
                if (!ValidatePathExists(start, target))
                {
                    // Týkadýysa geri al!
                    grid[pos.x, pos.y] = CellType.PathMargin;
                }
            }
        }
    }

    // =========================================================================
    // --- FÝZÝKSEL OBJE OLUÞTURUCULAR ---
    // =========================================================================
    private void CreateInternalRectangle(string objName, Vector2 localPos, Vector2 size)
    {
        GameObject block = new GameObject(objName);
        if (platformsParent != null) block.transform.SetParent(platformsParent);

        block.transform.localPosition = localPos;
        block.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer sr = block.AddComponent<SpriteRenderer>();
        sr.sprite = wallSprite;
        sr.material = platformMaterial;
        sr.drawMode = SpriteDrawMode.Simple;

        BoxCollider2D col = block.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
    }

    private void CreateWall(string wallName, Vector2 localPos, Vector2 size)
    {
        GameObject wall = new GameObject(wallName);
        if (platformsParent != null) wall.transform.SetParent(platformsParent);

        wall.transform.localPosition = localPos;
        wall.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
        sr.sprite = wallSprite;
        sr.material = platformMaterial;
        sr.drawMode = SpriteDrawMode.Simple;

        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
    }

    // =========================================================================
    // --- MATEMATÝK VE BFS DOÐRULAMA FONKSÝYONLARI ---
    // =========================================================================
    private Vector2Int WorldToGrid(Vector3 worldPos, float totalWidth, float totalHeight)
    {
        float x = Mathf.Clamp(worldPos.x + (totalWidth / 2f), 0, totalWidth - 0.1f);
        float y = Mathf.Clamp(worldPos.y + (totalHeight / 2f), 0, totalHeight - 0.1f);
        int c = Mathf.FloorToInt(x / cellSize);
        int r = Mathf.FloorToInt(y / cellSize);
        return new Vector2Int(Mathf.Clamp(c, 0, gridCols - 1), Mathf.Clamp(r, 0, gridRows - 1));
    }

    private Vector2 GridToWorld(int c, int r, float totalWidth, float totalHeight)
    {
        float wx = (c * cellSize) + (cellSize / 2f) - (totalWidth / 2f);
        float wy = (r * cellSize) + (cellSize / 2f) - (totalHeight / 2f);
        return new Vector2(wx, wy);
    }

    private bool ValidatePathExists(Vector2Int start, Vector2Int target)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            Vector2Int cur = queue.Dequeue();
            if (cur == target) return true;

            for (int i = 0; i < 4; i++)
            {
                int nx = cur.x + dx[i];
                int ny = cur.y + dy[i];
                Vector2Int next = new Vector2Int(nx, ny);

                if (nx >= 0 && nx < gridCols && ny >= 0 && ny < gridRows)
                {
                    if (!visited.Contains(next) && grid[nx, ny] != CellType.Rectangle)
                    {
                        visited.Add(next);
                        queue.Enqueue(next);
                    }
                }
            }
        }
        return false;
    }

    private void ClearAllPlatforms()
    {
        if (platformsParent == null) return;

        for (int i = platformsParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(platformsParent.GetChild(i).gameObject);
        }
    }
}