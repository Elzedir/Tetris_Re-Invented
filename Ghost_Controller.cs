using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost_Controller : MonoBehaviour
{
    public Tile tile;
    [HideInInspector] public Board_Controller _board;
    public Shape_Controller _activeShape;

    public Tilemap Tilemap { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int Position { get; private set; }

    private void Awake()
    {
        _board = GameObject.Find("Board").GetComponent<Board_Controller>();
        _activeShape = _board.GetComponent<Shape_Controller>();
        Tilemap = GetComponentInChildren<Tilemap>();
        Cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        Refresh();
        PlaceGhost();
        Set();
    }

    private void Refresh()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3Int tilePosition = Cells[i] + Position;
            Tilemap.SetTile(tilePosition, null);
        }

        for (int i = 0; i < Cells.Length; i++)
        {
            Cells[i] = _activeShape.Cells[i];
        }
    }

    private void PlaceGhost()
    {
        Vector3Int newPosition = _activeShape.Position;

        _board.Clear(_activeShape);

        int attempts = 0;

        while (_board.CanMove(_activeShape, newPosition) && attempts < 30)
        {
            Debug.Log(newPosition);
            newPosition.x += _board.MoveDirection.x;
            newPosition.y += _board.MoveDirection.y;
            Position = newPosition;
            attempts++;
        }

        Position = new Vector3Int (Position.x - _board.MoveDirection.x, Position.y - _board.MoveDirection.y, 0);

        _board.Set(_activeShape);
    }

    private void Set()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3Int tilePosition = Cells[i] + Position;
            Tilemap.SetTile(tilePosition, tile);
        }
    }
}
