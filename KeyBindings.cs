using System.Collections.Generic;
using System;
using UnityEngine;

public enum ActionKey
{
    Move_Up, Move_Left, Move_Down, Move_Right,
    Rotate_Left, Rotate_Right,
    Space, Escape
}

[Serializable]
public class KeyBindings
{
    Shape_Controller _activeShape;
    public Dictionary<ActionKey, KeyCode> Keys { get; private set; } = new();
    public Dictionary<ActionKey, Action> SinglePressKeyActions { get; private set; }
    public Dictionary<ActionKey, Action> MovementPressKeyActions { get; private set; }

    public KeyBindings()
    {
        Keys.Add(ActionKey.Move_Up, KeyCode.UpArrow);
        Keys.Add(ActionKey.Move_Left, KeyCode.LeftArrow);
        Keys.Add(ActionKey.Move_Down, KeyCode.DownArrow);
        Keys.Add(ActionKey.Move_Right, KeyCode.RightArrow);

        Keys.Add(ActionKey.Rotate_Left, KeyCode.A);
        Keys.Add(ActionKey.Rotate_Right, KeyCode.D);

        Keys.Add(ActionKey.Escape, KeyCode.Escape);
        Keys.Add(ActionKey.Space, KeyCode.Space);
    }

    public void RebindKey(ActionKey action, KeyCode newKey)
    {
        if (Keys.ContainsKey(action))
        {
            Keys[action] = newKey;
        }

        SaveBindings();
    }

    public void SaveBindings()
    {
        foreach (var key in Keys)
        {
            PlayerPrefs.SetInt(key.Key.ToString(), (int)key.Value);
        }

        PlayerPrefs.Save();
    }

    public void LoadBindings()
    {
        foreach (ActionKey key in Enum.GetValues(typeof(ActionKey)))
        {
            string keyString = key.ToString();

            if (PlayerPrefs.HasKey(keyString))
            {
                Keys[key] = (KeyCode)PlayerPrefs.GetInt(keyString);
            }
        }
    }

    public void InitialiseKeyActions(Shape_Controller shapeController)
    {
        _activeShape = shapeController;

        SinglePressKeyActions = new Dictionary<ActionKey, Action>
        {
            { ActionKey.Escape, HandleEscapePressed },
            { ActionKey.Space, HandleSpacePressed },
            { ActionKey.Rotate_Left, HandleRotateLeftPressed },
            { ActionKey.Rotate_Right, HandleRotateRightPressed }
        };

        MovementPressKeyActions = new Dictionary<ActionKey, Action>
        {
            { ActionKey.Move_Up, HandleMoveUpPressed },
            { ActionKey.Move_Down, HandleMoveDownPressed },
            { ActionKey.Move_Left, HandleMoveLeftPressed },
            { ActionKey.Move_Right, HandleMoveRightPressed },
        };
    }

    void HandleMoveUpPressed()
    {
        if (_activeShape.Move(Vector2Int.up))
        {
            _activeShape.StepTime = Time.time + _activeShape.StepDelay;
        }
    }

    void HandleMoveDownPressed()
    {
        if (_activeShape.Move(Vector2Int.down))
        {
            _activeShape.StepTime = Time.time + _activeShape.StepDelay;
        }
    }
    void HandleMoveLeftPressed()
    {
        _activeShape.Move(Vector2Int.left);
    }
    void HandleMoveRightPressed()
    {
        _activeShape.Move(Vector2Int.right);
    }

    void HandleEscapePressed()
    {
        //if (Menu_RightClick.Instance.enabled)
        //{
        //    Menu_RightClick.Instance.RightClickMenuClose();
        //}

        //Manager_Menu.Instance.HandleEscapePressed();
    }

    void HandleSpacePressed()
    {
        _activeShape.InstaSet();
    }

    void HandleRotateLeftPressed()
    {
        _activeShape.Rotate(-1);
    }

    void HandleRotateRightPressed()
    {
        _activeShape.Rotate(1);
    }
}