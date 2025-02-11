using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Ayarları")]
    [Tooltip("cellCount x cellCount boyutunda grid oluşturulur.")]
    [Range(3, 80)] public int cellCount = 5;
    public GameObject cellPrefab;
    public GameObject background;
    public TMP_Text score;
    public TMP_InputField cellCountInput;
    public float cellSpacing = 0.1f; // Hücreler arası mesafe

    private Cell[,] grid;

    private int executionCount = 0;
    private int variableToIncrement = 0;


    private const float DEFAULT_CELL_SPACING = 0.1f;

    private void Start()
    {
        GenerateGrid();
    }
    [ContextMenu("Rebuild")]
    public void GenerateGrid()
    {
        if (int.TryParse(cellCountInput.text, out int parsedCellCount))
        {
            cellCount = parsedCellCount;
        }
        else
        {
            cellCount = 5;
        }

        cellSpacing = DEFAULT_CELL_SPACING;

        if (cellCount > 18 && cellCount <= 30)
            cellSpacing /= 2;
        else if (cellCount > 30)
            cellSpacing /= 4;
        else
            cellSpacing = DEFAULT_CELL_SPACING;

        // Var olan grid varsa temizle
        if (grid != null)
        {
            foreach (Cell cell in grid)
            {
                if (cell != null)
                    Destroy(cell.gameObject);
            }
        }

        grid = new Cell[cellCount, cellCount];
        float cellSize = DetermineCellSize();

        // Grid’i ekranın ortasında konumlandırmak için offset hesaplaması
        float offset = (cellCount - 1) / 2f;

        for (int i = 0; i < cellCount; i++)
        {
            for (int j = 0; j < cellCount; j++)
            {
                // Pozisyon hesaplaması: i ve j değerlerine göre
                Vector3 pos = new Vector3((i - offset) * (cellSize + cellSpacing), (j - offset) * (cellSize + cellSpacing) + 1, 0);
                GameObject cellObj = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                cellObj.transform.localScale = Vector3.one * cellSize;
                Cell cell = cellObj.GetComponent<Cell>();
                cell.Init(i, j, this);
                grid[i, j] = cell;
            }
        }
    }

    // Hücrelerin boyutunu ekran boyutuna göre hesaplayan fonksiyon
    float DetermineCellSize()
    {
        Camera cam = Camera.main;
        // Ortografik kamera için ekran yüksekliği (world units)
        float height = cam.orthographicSize * 2;
        // Ekran genişliği
        float width = height * cam.aspect;

        // Hücreler arası boşluk değeri (örneğin GridManager'da tanımlı bir cellSpacing varsa onu kullanabilirsiniz)
        float spacing = cellSpacing; // cellSpacing, grid oluşturulurken kullanılan spacing değeri olsun.

        // cellCount hücre, cellCount-1 aralık olduğundan kullanılabilir toplam genişlik/yükseklik:
        float availableWidth = width - (cellCount - 1) * spacing;
        float availableHeight = height - (cellCount - 1) * spacing;

        float wideScreenvalue;
        if (width > height)
        {
            wideScreenvalue = 10f;

            //background.GetComponent<RectTransform>().position = new Vector3(background.transform.position.x, background.transform.position.y, background.transform.position.z);
            // background.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);
        }
        else
        {
            wideScreenvalue = 0.5f;
            //background.GetComponent<RectTransform>().position = new Vector3(background.transform.position.x, background.transform.position.x + 50f, background.transform.position.z);
            //background.transform.localScale = new Vector3(1, 1, 1);
        }


        // Grid kare olduğundan, en küçük kullanılabilir alanı baz alarak cellSize hesaplanır:
        float cellSize = Mathf.Min(availableWidth - wideScreenvalue, availableHeight - wideScreenvalue / 2f) / cellCount;
        return cellSize;
    }

    // Hücreye tıklandığında çağrılır
    public void OnCellClicked(Cell clickedCell)
    {

        // Eğer hücre zaten işaretliyse (X varsa) hiçbir işlem yapma
        if (clickedCell.IsMarked)
            return;

        clickedCell.Mark(); // X işaretini yerleştir
    }

    // X yerleştirildiğinde yatay ve dikey olarak ardışık X'leri kontrol eder
    internal void CheckAndClearMatches(Cell cell)
    {
        int i = cell.x;
        int j = cell.y;

        // Yatay kontrol
        List<Cell> horizontalMatches = new List<Cell>();
        horizontalMatches.Add(cell);
        // Sol tarafa kontrol
        for (int a = i - 1; a >= 0; a--)
        {
            if (grid[a, j].IsMarked)
                horizontalMatches.Add(grid[a, j]);
            else
                break;
        }
        // Sağ tarafa kontrol
        for (int a = i + 1; a < cellCount; a++)
        {
            if (grid[a, j].IsMarked)
                horizontalMatches.Add(grid[a, j]);
            else
                break;
        }
        if (horizontalMatches.Count >= 3)
        {
            foreach (Cell c in horizontalMatches)
            {
                c.Clear();
            }
        }

        // Dikey kontrol
        List<Cell> verticalMatches = new List<Cell>();
        verticalMatches.Add(cell);
        // Aşağıya kontrol
        for (int b = j - 1; b >= 0; b--)
        {
            if (grid[i, b].IsMarked)
                verticalMatches.Add(grid[i, b]);
            else
                break;
        }
        // Yukarıya kontrol
        for (int b = j + 1; b < cellCount; b++)
        {
            if (grid[i, b].IsMarked)
                verticalMatches.Add(grid[i, b]);
            else
                break;
        }
        if (verticalMatches.Count >= 3)
        {
            foreach (Cell c in verticalMatches)
            {
                c.Clear();
            }
        }
    }
    public void ScoreCount()
    {
        executionCount++;

        if (executionCount >= 3)
        {
            variableToIncrement++;
            score.text = "SCORE:" + variableToIncrement;
            executionCount = 0; // Sayaç sıfırlanır.

        }
    }
}
