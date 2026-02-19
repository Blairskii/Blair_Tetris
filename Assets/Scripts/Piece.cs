using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    // Set variables
    public TetronimoData data;
    public Board board;
    public Vector2Int[] cells;

    public Vector2Int position;

    // Audio
    public AudioClip moveClip;
    public AudioClip rotateClip;
    public AudioClip dropClip;
    AudioSource audioSource;

    // Freeze bool for preventing piece manipulation
    bool freeze = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Initializes cell data for a given shape
    public void Initialize(Board board, Tetronimo tetronimo)
    {
        // Get board reference
        this.board = board;

        // Search and assign tetronimo data
        for (int i = 0; i < board.tetronimos.Length; i++)
        {
            if (board.tetronimos[i].tetronimo == tetronimo)
            {
                this.data = board.tetronimos[i];
                break;
            }
        }

        // Create copy of cell locations
        cells = new Vector2Int[data.cells.Length];

        for (int i = 0; i < data.cells.Length; i++)
        {
            cells[i] = data.cells[i];
        }

        // Set position to start position
        position = board.startPosition;
    }

    private void Update()
    {
        // If the piece is frozen, break out of update and prevent rest of loop from running
        if (freeze) return;

        // Clear the board every frame
        board.Clear(this);

        // Hard drop has priority
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
        else
        {
            // Left / Right movement
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (Move(Vector2Int.left))
                {
                    PlayOneShotSafe(moveClip);
                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (Move(Vector2Int.right))
                {
                    PlayOneShotSafe(moveClip);
                }
            }

            // Downward movement
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (Move(Vector2Int.down))
                {
                    PlayOneShotSafe(moveClip);
                }
            }

            // Left / Right Rotation
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Rotate(1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Rotate(-1);
            }
        }

        // Set board with this tetronimo
        board.Set(this);

        // TBD -- DEBUG------------------------------------------------------------------------
        if (Input.GetKeyDown(KeyCode.P))
        {
            board.CheckBoard();
        }

        // Check board after setting
        if (freeze)
        {
            // check board
            board.CheckBoard();

            // update score

            // spawn piece
            board.SpawnPiece();
        }
    }

    void HardDrop()
    {
        // Play one-shot for hard drop once
        PlayOneShotSafe(dropClip);

        // Algorithm: repeatedly move down until we cant
        while(Move(Vector2Int.down))
        {
            // do nothing move is called
        }

        freeze = true;
    }

    // Rotate piece in a given direction
    void Rotate(int direction)
    {
        // Copy cells incase we need to revert back to original
        Vector2Int[] originalCells = new Vector2Int[cells.Length];

        for (int i = 1; i < cells.Length; i++)
        {
            originalCells[i] = cells[i];
        }

        ApplyRotation(direction);

        bool rotationSucceeded = false;

        // Check position after rotation
        if (!board.IsPositionValid(this, position))
        {
            // If position is not valid, attempt wall kick
            if (!TryWallKicks())
            {
                // Revert rotation if wall kicks fail
                RevertRotation(originalCells);
            }
            else
            {
                rotationSucceeded = true;
                Debug.Log("Wall kick succeeded");
            }
        }
        else
        {
            rotationSucceeded = true;
            Debug.Log("Rotation successful");
        }

        if (rotationSucceeded)
        {
            PlayOneShotSafe(rotateClip);
        }
    }

    void RevertRotation(Vector2Int[] originalCells)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = originalCells[i];
        }
    }

    bool TryWallKicks()
    {
        List<Vector2Int> wallKickOffsets = new List<Vector2Int>()
        {
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.down,
            new Vector2Int(-1, -1),
            new Vector2Int(1, -1)
        };

        if (data.tetronimo == Tetronimo.I)
        {
            wallKickOffsets.Add(2 * Vector2Int.left);
            wallKickOffsets.Add(2 * Vector2Int.right);
        }

        foreach(Vector2Int offset in wallKickOffsets)
        {
            if (Move(offset))
            {
                return true;
            }
        }

        return false;
    }

    void ApplyRotation(int direction)
    {
        // Create new rotation variable
        Quaternion rotation = Quaternion.Euler(0, 0, 90 * direction);

        bool isSpecial = data.tetronimo == Tetronimo.I || data.tetronimo == Tetronimo.O || data.tetronimo == Tetronimo.Custom;
        // Loop through all cell positions 
        for (int i = 0; i < cells.Length; i++)
        {
            // Convert cell location to vec3 for quaternions
            Vector3 cellPosition = new Vector3(cells[i].x, cells[i].y);

            // Fix origin
            if (isSpecial)
            {
                cellPosition.x -= 0.5f;
                cellPosition.y -= 0.5f;
            }

            // Get result
            Vector3 result = rotation * cellPosition;

            // Apply change
            if (isSpecial)
            {
                cells[i].x = Mathf.CeilToInt(result.x);
                cells[i].y = Mathf.CeilToInt(result.y);
            }
            else
            {
                cells[i].x = Mathf.RoundToInt(result.x);
                cells[i].y = Mathf.RoundToInt(result.y);
            }
        }
    }

    // Move piece 
    bool Move(Vector2Int translation)
    {
        // Create temp position
        Vector2Int newPosition = position;

        // Add translation to temp position
        newPosition += translation;

        // Store if new position is valid
        bool positionValid = board.IsPositionValid(this, newPosition);

        // If position is valid set position
        if (positionValid)
        {
            position = newPosition;
        }

        // Return true or false
        return positionValid;
    }

    void PlayOneShotSafe(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
