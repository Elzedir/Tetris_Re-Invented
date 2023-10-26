using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Shape_Controller : MonoBehaviour
{
    public Board_Controller Board { get; private set; }
    public Shape ShapeData { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int Position { get; private set; }
    public int RotationIndex { get; private set; }

    public float StepDelay = 1f;
    public float MoveDelay = 0.1f;
    public float LockDelay = 0.5f;

    public float StepTime;
    private float MoveTime;
    private float LockTime;

    public KeyBindings KeyBindings;

    public void Awake()
    {
        KeyBindings = new KeyBindings();
        KeyBindings.LoadBindings();
        KeyBindings.InitialiseKeyActions(this);
    }

    public void CreateShape(Board_Controller board, Vector3Int position, Shape data)
    {
        ShapeData = data;
        Board = board;
        Position = position;

        RotationIndex = 0;
        StepTime = Time.time + StepDelay;
        MoveTime = Time.time + MoveDelay;
        LockTime = 0f;

        if (Cells == null)
        {
            Cells = new Vector3Int[data.Cells.Length];
        }

        for (int i = 0; i < Cells.Length; i++)
        {
            Cells[i] = (Vector3Int)data.Cells[i];
        }
    }

    private void Update()
    {
        Board.Clear(this);

        LockTime += Time.deltaTime;

        foreach (var actionKey in KeyBindings.SinglePressKeyActions.Keys)
        {
            if (Input.GetKeyDown(KeyBindings.Keys[actionKey]))
            {
                KeyBindings.SinglePressKeyActions[actionKey]?.Invoke();
            }
        }

        if (Time.time > MoveTime)
        {
            foreach (var actionKey in KeyBindings.MovementPressKeyActions.Keys)
            {
                if (Input.GetKey(KeyBindings.Keys[actionKey]))
                {
                    KeyBindings.MovementPressKeyActions[actionKey]?.Invoke();
                }
            }
        }

        if (Time.time > StepTime)
        {
            TimedMove();
        }

        Board.Set(this);
    }

    public void TimedMove()
    {
        StepTime = Time.time + StepDelay;

        Move(Board.MoveDirection);

        if (LockTime >= LockDelay) Lock();
    }

    public void InstaSet()
    {
        int attempts = 0;

        while (Move(Board.MoveDirection) && attempts < 30)
        {
            attempts++;

            continue;
        }

        Lock();
    }

    private void Lock()
    {
        Board.Set(this);
        Board.ClearLines();
        Board.SpawnPiece(Board.NextShape);
    }

    public bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = Position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = Board.CanMove(this, newPosition);

        if (valid)
        {
            Position = newPosition;
            MoveTime = Time.time + MoveDelay;
            LockTime = 0f;
        }

        return valid;
    }

    public void Rotate(int direction)
    {
        int originalRotation = RotationIndex;

        RotationIndex = Wrap(RotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        if (!TestWallKicks(RotationIndex, direction))
        {
            RotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = global::ShapeData.RotationMatrix;

        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3 cell = Cells[i];

            int x, y;

            switch (ShapeData.ShapeName)
            {
                case ShapeName.I:
                case ShapeName.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            Cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < ShapeData.WallKicks.GetLength(1); i++)
        {
            Vector2Int translation = ShapeData.WallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, ShapeData.WallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }
}
