using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Board_Controller : MonoBehaviour
{
    public Tilemap BoardTilemap { get; private set; }
    public List<Tile> AllTiles { get; private set; } = new();
    public Tile CurrentTile { get; private set; }
    public Shape CurrentShape { get; private set; }
    public Shape NextShape { get; private set; }
    public Shape_Controller ActiveShape { get; private set; }
    public List<Shape> AllShapes { get; private set; } = new();
    public Vector2Int BoardSize { get; private set; }
    public Vector3Int[] SpawnPositions;
    public Vector2Int MoveDirection;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-BoardSize.x / 2, -BoardSize.y / 2);
            return new RectInt(position, BoardSize);
        }
    }

    private void Awake()
    {
        BoardTilemap = GetComponentInChildren<Tilemap>();
        ActiveShape = GetComponentInChildren<Shape_Controller>();

        Vector2 xSizeFloat = GameObject.Find("Grid").GetComponent<SpriteRenderer>().size;
        BoardSize = new Vector2Int(Mathf.FloorToInt(xSizeFloat.x), Mathf.FloorToInt(xSizeFloat.y));

        foreach (ShapeName shape in Enum.GetValues(typeof(ShapeName)))
        {
            Shape newShape = new Shape();
            newShape.ShapeName = shape;
            newShape.Create();
            AllShapes.Add(newShape);
        }

        Tile[] tiles = Resources.LoadAll<Tile>("Tiles/Standard");

        foreach (Tile tile in tiles)
        {
            AllTiles.Add(tile);
        }
    }

    private void Start()
    {
        if (MoveDirection == new Vector2Int(0, 0))
        {
            MoveDirection = Vector2Int.down;
        }

        StartGame();
    }

    void StartGame()
    {
        CurrentShape = AllShapes[UnityEngine.Random.Range(0, AllShapes.Count)];

        SpawnPiece(CurrentShape);
    }

    public void SpawnPiece(Shape shapeToSpawn)
    {
        CurrentTile = AllTiles[UnityEngine.Random.Range(0, AllTiles.Count)];
        Vector3Int spawnPosition = SpawnPositions[UnityEngine.Random.Range(0, SpawnPositions.Length)];
        ActiveShape.CreateShape(this, spawnPosition, shapeToSpawn);

        if (CanMove(ActiveShape, spawnPosition))
        {
            Set(ActiveShape);

            CurrentShape = NextShape;
            NextShape = AllShapes[UnityEngine.Random.Range(0, AllShapes.Count)];
        }
        else
        {
            GameOver();
        }
    }

    public void Update()
    {
        if (MoveDirection == new Vector2Int(0, 0))
        {
            MoveDirection = Vector2Int.down;
        }
    }

    public void GameOver()
    {
        BoardTilemap.ClearAllTiles();
    }

    public void Set(Shape_Controller shape)
    {
        for (int i = 0; i < shape.Cells.Length; i++)
        {
            Vector3Int tilePosition = shape.Cells[i] + shape.Position;
            BoardTilemap.SetTile(tilePosition, CurrentTile);
        }
    }

    public void Clear(Shape_Controller shape)
    {
        for (int i = 0; i < shape.Cells.Length; i++)
        {
            Vector3Int tilePosition = shape.Cells[i] + shape.Position;
            BoardTilemap.SetTile(tilePosition, null);
        }
    }

    public bool CanMove(Shape_Controller shape, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < shape.Cells.Length; i++)
        {
            Vector3Int tilePosition = shape.Cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if (BoardTilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (RowIsFull(row))
            {
                RowClear(row);
            }
            else
            {
                row++;
            }
        }

        int column = bounds.xMin;

        while (column < bounds.xMax)
        {
            if (ColumnIsFull(column))
            {
                ColumnClear(column);
            }
            else
            {
                column++;
            }
        }
    }

    public bool RowIsFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!BoardTilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    public bool ColumnIsFull(int column)
    {
        RectInt bounds = Bounds;

        for (int row = bounds.yMin; row < bounds.yMax; row++)
        {
            Vector3Int position = new Vector3Int(column, row, 0);

            if (!BoardTilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    public void RowClear(int row)
    {
        if (Mathf.Sign(MoveDirection.y) == 0) return;

        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            BoardTilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + Mathf.RoundToInt(1 * Mathf.Sign(MoveDirection.y)), 0);
                TileBase above = BoardTilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                BoardTilemap.SetTile(position, above);
            }

            row++;
        }
    }

    public void ColumnClear(int column)
    {
        if (Mathf.Sign(MoveDirection.x) == 0) return;

        RectInt bounds = Bounds;

        for (int row = bounds.yMin; row < bounds.yMax; row++)
        {
            Vector3Int position = new Vector3Int(column, row, 0);
            BoardTilemap.SetTile(position, null);
        }

        while (column < bounds.xMax)
        {
            for (int row = bounds.yMin; row < bounds.yMax; row++)
            {
                Vector3Int position = new Vector3Int(column, row + Mathf.RoundToInt(1 * Mathf.Sign(MoveDirection.x)), 0);
                TileBase above = BoardTilemap.GetTile(position);

                position = new Vector3Int(column, row, 0);
                BoardTilemap.SetTile(position, above);
            }

            column++;
        }
    }

    void ChangeTile(Tile tile)
    {
        CurrentTile = tile;
    }
}
