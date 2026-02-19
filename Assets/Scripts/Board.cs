using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public TetrisManager tetrisManager;
    public Piece prefabPiece;
    public Tilemap tilemap;
    public TetronimoData[] tetronimos;
    public Vector2Int boardSize;
    public Vector2Int startPosition;

    private Piece activePiece;

    

    // Getters for board sides
    int left
    {
        get { return -boardSize.x / 2; }
    }

    int right
    {
        get { return boardSize.x / 2; }
    }

    int top
    {
        get { return boardSize.y / 2; }
    }

    int bottom
    {
        get { return -boardSize.y / 2; }
    }

    // Spawn piece on script start
    private void Start()
    {
        SpawnPiece();
    }

    
    public void SpawnPiece()
    {
        // Instatiate new tetronimo
        activePiece = Instantiate(prefabPiece);

        // Set tetronimo to random shape
        Tetronimo t = (Tetronimo)Random.Range(0, tetronimos.Length);

        // Initialize and set new tetronimo piece
        activePiece.Initialize(this, t);

        CheckEndGame();

        Set(activePiece);
    }
    void CheckEndGame()
    {
        if (!IsPositionValid(activePiece, activePiece.position))
        {
         //if no valid position gameover
            tetrisManager.SetGameOver(true);
        } 
    }
    public void UpdateGameOver()
    {
        if (!tetrisManager.GameOver)
        {
            ResetBoard();
        }
    }

    void ResetBoard()
    {
       Piece[] foundPieces = FindObjectsByType<Piece>(FindObjectsSortMode.None);

        foreach (Piece piece in foundPieces)Destroy(piece.gameObject);

        activePiece = null;

        tilemap.ClearAllTiles();

        SpawnPiece();

    }

 public void Clear(Piece piece)
    {
        // Loop through all tetronimo cells and set tiles to null
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            tilemap.SetTile(cellPosition, null);
        }
    }

    // Color piece tiles
    public void Set(Piece piece)
    {
        // Loop through all tetronimo sells and set tiles accordingly
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            tilemap.SetTile(cellPosition, piece.data.tile);
        }
    }

    // Check if line is full
    bool IsLineFull(int y)
    {
        // Loop from left of row to right
        for (int x = left; x < right; x++)
        {
            // Create new position
            Vector3Int cellPosition = new Vector3Int(x, y);

            // Return false if there are any cells that are empty
            if (!tilemap.HasTile(cellPosition))
            {
                return false;
            }
        }

        // Else return true
        return true;
    }

    // Destroy all pieces on a given line
    void DestroyLine(int y)
    {
        Debug.Log($"Destroy Line: {y}");

        // Loop from left to right of row
        for (int x = left; x < right; x++)
        {
            // Set all tiles in row to null
            Vector3Int cellPosition = new Vector3Int(x, y);
            tilemap.SetTile(cellPosition, null);
        }
    }

    // Shift all rows down from a given row
    void ShiftRowsDown(int clearedRow)
    {
        // From the cleared row to the top of the board
        for (int y = clearedRow + 1; y < top; y++)
        {
            Debug.Log($"Shift down {clearedRow + 1}");

            // From left to right of the row
            for (int x = left; x < right; x++)
            {
                // Get current cell position
                Vector3Int cellPosition = new Vector3Int(x, y);

                // Save temp tile
                TileBase currentTile = tilemap.GetTile(cellPosition);

                // Clear tile
                tilemap.SetTile(cellPosition, null);

                // Move temp tile down
                cellPosition.y--;

                // Set tiles
                tilemap.SetTile(cellPosition, currentTile);
            }
        }
    }

    public void CheckBoard()
    {
        // Create new list of destroyed lines
        List<int> destroyedLines = new List<int>();

        // Bottom to Top
        for (int y = bottom; y < top; y++)
        {
            // Check if line is full, if so destroy line and add line y value to destroyed lines list
            if (IsLineFull(y))
            {
                DestroyLine(y);
                destroyedLines.Add(y);
            }
        }

        int rowsShiftedDown = 0;
        // For every line in destroyed lines list, shift all rows above down
        foreach (int y in destroyedLines)
        {
            ShiftRowsDown(y - rowsShiftedDown);
            rowsShiftedDown++;
        }

        // Update Score
        int score = tetrisManager.CalculateScore(destroyedLines.Count);
        tetrisManager.ChangeScore(score);
    }

    // Returns if a position is valid for tetronimo movement
    public bool IsPositionValid(Piece piece, Vector2Int position)
    {
        // Loop through all cells in the tetronimo
        for (int i = 0; i < piece.cells.Length;  i++)
        {
            // Get cell position
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + position);

            // Bounds check
            if (cellPosition.x < left || cellPosition.x >= right ||
                cellPosition.y < bottom || cellPosition.y >= top)
                return false;
            
            // Return false if this position is invalid
            if (tilemap.HasTile(cellPosition))
            {
                return false;
            }
        }

        return true;
    }
}
