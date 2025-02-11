using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("Creates a grid of size cellCount x cellCount.")]
    [Range(3, 80)] public int cellCount = 5;
    public GameObject cellPrefab;
    public GameObject background;
    public TMP_Text score;
    public TMP_InputField cellCountInput;
    public float cellSpacing = 0.1f; // Spacing between cells

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
            if (parsedCellCount < 3) parsedCellCount = 3;
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

        // Clear the existing grid if any
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

        // Calculate offset to position the grid in the center of the screen
        float offset = (cellCount - 1) / 2f;

        for (int i = 0; i < cellCount; i++)
        {
            for (int j = 0; j < cellCount; j++)
            {
                // Calculate position based on i and j values
                Vector3 pos = new Vector3((i - offset) * (cellSize + cellSpacing), (j - offset) * (cellSize + cellSpacing) + 1, 0);
                GameObject cellObj = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                cellObj.transform.localScale = Vector3.one * cellSize;
                Cell cell = cellObj.GetComponent<Cell>();
                cell.Init(i, j, this);
                grid[i, j] = cell;
            }
        }
    }

    // Function to calculate cell size based on screen dimensions
    float DetermineCellSize()
    {
        Camera cam = Camera.main;
        // Screen height for an orthographic camera (in world units)
        float height = cam.orthographicSize * 2;
        // Screen width
        float width = height * cam.aspect;

        // Spacing value between cells (using the cellSpacing defined in GridManager)
        float spacing = cellSpacing;

        // Since there are cellCount cells and cellCount-1 gaps, calculate the available width/height:
        float availableWidth = width - (cellCount - 1) * spacing;
        float availableHeight = height - (cellCount - 1) * spacing;

        float wideScreenvalue;
        if (width > height)
        {
            wideScreenvalue = 10f;
            // Background adjustments commented out
        }
        else
        {
            wideScreenvalue = 0.5f;
            // Background adjustments commented out
        }

        // Since the grid is square, cellSize is calculated based on the smallest available area:
        float cellSize = Mathf.Min(availableWidth - wideScreenvalue, availableHeight - wideScreenvalue / 2f) / cellCount;
        return cellSize;
    }

    // Called when a cell is clicked
    public void OnCellClicked(Cell clickedCell)
    {
        // Do nothing if the cell is already marked (has an X)
        if (clickedCell.IsMarked)
            return;

        clickedCell.Mark(); // Place the X marker
    }

    // Checks for consecutive X markers horizontally and vertically when an X is placed
    internal void CheckAndClearMatches(Cell cell)
    {
        int i = cell.x;
        int j = cell.y;

        // Horizontal check
        List<Cell> horizontalMatches = new List<Cell>();
        horizontalMatches.Add(cell);
        // Check to the left
        for (int a = i - 1; a >= 0; a--)
        {
            if (grid[a, j].IsMarked)
                horizontalMatches.Add(grid[a, j]);
            else
                break;
        }
        // Check to the right
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

        // Vertical check
        List<Cell> verticalMatches = new List<Cell>();
        verticalMatches.Add(cell);
        // Check downward
        for (int b = j - 1; b >= 0; b--)
        {
            if (grid[i, b].IsMarked)
                verticalMatches.Add(grid[i, b]);
            else
                break;
        }
        // Check upward
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
            executionCount = 0; // Reset the counter.
        }
    }
}
