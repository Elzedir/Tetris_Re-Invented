using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Shape_Controller : MonoBehaviour
{
    Vector3 _initialDirection;
    Tilemap _tilemap;
    Tilemap _borderTilemap;
    List<Vector3Int> _currentShape;
    Tile _tile;
    float _timer;

    Coroutine _playerMoveCoroutine;
    bool _playerMoveCoroutineRunning;
    public bool ShapePlaced;

    public void CreateShape(Vector3 initialDirection, Tilemap tilemap, List<Vector3Int> initialShape, Tile tile)
    {
        _borderTilemap = Level_Manager.Instance.BorderTilemap;
        _tile = tile;
        _initialDirection = initialDirection;
        _tilemap = tilemap;
        _currentShape = new List<Vector3Int>(initialShape); // Initialize with the shape's initial positions
    }

    public void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= Level_Manager.Instance.SpawnTime)
        {
            MoveShape(_initialDirection);
            _timer = 0;
        }

        PlayerMoveShape();
    }

    void MoveShape(Vector3 initialDirection)
    {
        Vector3Int intDirection = new Vector3Int((int)_initialDirection.x, (int)_initialDirection.y, 0);

        if (!CanMove(intDirection))
        {
            if (intDirection.y < 0)
            {
                ShapePlaced = true;
            }
            return;
        }

        for (int i = 0; i < _currentShape.Count; i++)
        {
            _tilemap.SetTile(_currentShape[i], null);

            _currentShape[i] += intDirection;
        }

        foreach (Vector3Int pos in _currentShape)
        {
            _tilemap.SetTile(pos, _tile);
        }
    }

    void PlayerMoveShape()
    {
        Vector3Int moveDirection = Vector3Int.zero;

        if (Input.GetKey(Input_Manager.Instance.KeyBindings.Keys[ActionKey.Move_Up])) moveDirection.y = 1;
        if (Input.GetKey(Input_Manager.Instance.KeyBindings.Keys[ActionKey.Move_Down])) moveDirection.y = -1;
        if (Input.GetKey(Input_Manager.Instance.KeyBindings.Keys[ActionKey.Move_Left])) moveDirection.x = -1;
        if (Input.GetKey(Input_Manager.Instance.KeyBindings.Keys[ActionKey.Move_Right])) moveDirection.x = 1;

        if (moveDirection != Vector3Int.zero && !_playerMoveCoroutineRunning)
        {
            _playerMoveCoroutine = StartCoroutine(MoveKeyHeld(moveDirection));
            MoveShape(moveDirection);
        }
    }

    IEnumerator MoveKeyHeld(Vector3Int direction)
    {
        _playerMoveCoroutineRunning = true;

        ActionKey _directionKey = ActionKey.Move_Down;

        if (direction.x == 1) _directionKey = ActionKey.Move_Right;
        else if (direction.x == -1) _directionKey = ActionKey.Move_Left;
        else if (direction.y == 1) _directionKey = ActionKey.Move_Up;
        else if (direction.y == -1) _directionKey = ActionKey.Move_Down;

        MoveShape(direction);
        yield return new WaitForSeconds(1f);

        while (Input.GetKey(Input_Manager.Instance.KeyBindings.Keys[_directionKey]))
        {
            if (CanMove(direction))
            {
                MoveShape(direction);
            }

            yield return new WaitForSeconds(0.2f);
        }

        _playerMoveCoroutineRunning = false;
    }

    bool CanMove(Vector3Int direction)
    {
        foreach (Vector3Int pos in _currentShape)
        {
            Vector3Int nextPos = pos + direction;

            if (_borderTilemap.GetTile(nextPos) != null)
            {
                return false;
            }
        }

        return true;
    }
}
