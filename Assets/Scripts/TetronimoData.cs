using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetronimo { I, O, T, J, L, S, Z, Custom}

[Serializable]
public struct TetronimoData
{
    public Tetronimo tetronimo;
    public Vector2Int[] cells;
    public Tile tile;
}
