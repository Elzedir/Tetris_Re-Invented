using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

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
        for (int i = 0; i < shape.Cells.Length; i++)
        {
            Vector3Int tilePosition = shape.Cells[i] + position;

            if (!Bounds.Contains((Vector2Int)tilePosition))
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
        ClearX();
        ClearY();
        
    }

    void ClearX()
    {
        int xAttempts = 0;
        int startColumn;
        int endColumn;

        if (MoveDirection.x < 0)
        {
            startColumn = Bounds.xMin;
            endColumn = Bounds.xMax - 1;

            while (startColumn < endColumn && xAttempts < 300 && MoveDirection.x != 0)
            {
                if (ColumnIsFull(startColumn)) ColumnClear(startColumn);
                else startColumn++;
                xAttempts++;
            }
        }
        else
        {
            startColumn = Bounds.xMax - 1;
            endColumn = Bounds.xMin;

            while (startColumn >= endColumn && xAttempts < 300 && MoveDirection.x != 0)
            {
                if (ColumnIsFull(startColumn)) ColumnClear(startColumn);
                else startColumn--;
                xAttempts++;
            }
        }

        Debug.Log($"XAttempts: {xAttempts}");
    }

    void ClearY()
    {
        int yAttempts = 0;
        int startRow;
        int endRow;

        if (MoveDirection.y < 0)
        {
            startRow = Bounds.yMin;
            endRow = Bounds.yMax - 1;

            while (startRow < endRow && yAttempts < 300 && MoveDirection.y != 0)
            {
                if (RowIsFull(startRow)) RowClear(startRow);
                else startRow++;
                yAttempts++;
            }
        }
        else
        {
            startRow = Bounds.yMax - 1;
            endRow = Bounds.yMin;

            while (startRow > endRow && yAttempts < 300 && MoveDirection.y != 0)
            {
                if (RowIsFull(startRow)) RowClear(startRow);
                else startRow--;
                yAttempts++;
            }
        }

        Debug.Log($"YAttempts: {yAttempts}");
    }

    public bool ColumnIsFull(int column)
    {
        if (MoveDirection.x < 0)
        {
            for (int row = Bounds.yMin; row <= Bounds.yMax - 1; row++)
            {
                Vector3Int position = new Vector3Int(column, row, 0);

                if (!BoardTilemap.HasTile(position)) return false;
            }
        }
        else
        {
            for (int row = Bounds.yMax - 1; row >= Bounds.yMin; row--)
            {
                Vector3Int position = new Vector3Int(column, row, 0);
                if (!BoardTilemap.HasTile(position)) return false;
            }
        }
        
        return true;
    }

    public bool RowIsFull(int row)
    {
        if (MoveDirection.y < 0)
        {
            for (int column = Bounds.xMin; column < Bounds.xMax - 1; column++)
            {
                Vector3Int position = new Vector3Int(column, row, 0);

                if (!BoardTilemap.HasTile(position)) return false;
            }
        }
        else
        {
            for (int column = Bounds.xMax - 1; column >= Bounds.xMin; column--)
            {
                Vector3Int position = new Vector3Int(column, row, 0);

                if (!BoardTilemap.HasTile(position)) return false;
            }
        }
        

        return true;
    }

    public void ColumnClear(int column)
    {
        if (MoveDirection.x < 0)
        {
            for (int row = Bounds.yMin; row < Bounds.yMax - 1; row++)
            {
                Vector3Int position = new Vector3Int(column, row, 0);
                Debug.Log(position);
                BoardTilemap.SetTile(position, null);
            }

            while (column < Bounds.xMax - 1)
            {
                for (int row = Bounds.yMin; row < Bounds.yMax - 1; row++)
                {
                    Vector3Int position = new Vector3Int(column - (int)Mathf.Sign(MoveDirection.x), row, 0);
                    TileBase above = BoardTilemap.GetTile(position);

                    position = new Vector3Int(column, row, 0);
                    BoardTilemap.SetTile(position, above);
                }

                column++;
            }
        }
        else
        {
            for (int row = Bounds.yMax - 1; row >= Bounds.yMin; row--)
            {
                Vector3Int position = new Vector3Int(column, row, 0);
                BoardTilemap.SetTile(position, null);
            }

            while (column >= Bounds.xMin)
            {
                for (int row = Bounds.yMax - 1; row >= Bounds.yMin; row--)
                {
                    Vector3Int position = new Vector3Int(column - (int)Mathf.Sign(MoveDirection.x), row, 0);
                    TileBase above = BoardTilemap.GetTile(position);

                    position = new Vector3Int(column, row, 0);
                    BoardTilemap.SetTile(position, above);
                }

                column--;
            }
        }
    }

    public void RowClear(int row)
    {
        if (MoveDirection.y < 0)
        {
            for (int column = Bounds.xMin; column < Bounds.xMax - 1; column++)
            {
                Vector3Int position = new Vector3Int(column, row, 0);
                BoardTilemap.SetTile(position, null);
            }

            while (row < Bounds.yMax - 1)
            {
                for (int column = Bounds.xMin; column < Bounds.xMax - 1; column++)
                {
                    Vector3Int position = new Vector3Int(column, row - (int)Mathf.Sign(MoveDirection.y), 0);
                    TileBase aboveTile = BoardTilemap.GetTile(position);

                    position = new Vector3Int(column, row, 0);
                    BoardTilemap.SetTile(position, aboveTile);
                }

                row++;
            }
        }
        else
        {
            for (int column = Bounds.xMax - 1; column >= Bounds.xMin; column--)
            {
                Vector3Int position = new Vector3Int(column, row, 0);
                BoardTilemap.SetTile(position, null);
            }

            while (row >= Bounds.yMin)
            {
                for (int column = Bounds.xMax - 1; column >= Bounds.xMin; column--)
                {
                    Vector3Int position = new Vector3Int(column, row - (int)Mathf.Sign(MoveDirection.y), 0);
                    TileBase aboveTile = BoardTilemap.GetTile(position);

                    position = new Vector3Int(column, row, 0);
                    BoardTilemap.SetTile(position, aboveTile);
                }

                row--;
            }
        }
    }
}
