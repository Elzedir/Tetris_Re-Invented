using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Preview_Controller : MonoBehaviour
{
    Board_Controller _board;
    Shape_Controller _activeShape;

    public Tilemap Tilemap { get; private set; }
    public Vector3Int[] Cells { get; private set; }

    private void Awake()
    {
        _board = GameObject.Find("Board").GetComponent<Board_Controller>();
        _activeShape = _board.GetComponent<Shape_Controller>();
        Tilemap = GetComponentInChildren<Tilemap>();
        Cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        Refresh(_board.NextShape);
        Set();
    }

    private void Refresh(Shape data)
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3Int tilePosition = Cells[i];
            Tilemap.SetTile(tilePosition, null);
        }

        if (Cells == null)
        {
            Cells = new Vector3Int[data.Cells.Length];
        }

        for (int i = 0; i < Cells.Length; i++)
        {
            Cells[i] = (Vector3Int)data.Cells[i];
        }
    }

    private void Set()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3Int tilePosition = Cells[i];
            Tilemap.SetTile(tilePosition, _board.CurrentTile);
        }
    }

}
