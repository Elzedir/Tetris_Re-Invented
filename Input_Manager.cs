using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input_Manager : MonoBehaviour
{
    public static Input_Manager Instance;

    KeyBindings _keyBindings; public KeyBindings KeyBindings {  get { return _keyBindings; } }
    Dictionary<ActionKey, Action> _singlePressKeyActions;
    Dictionary<ActionKey, Action> _continuousPressKeyActions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _keyBindings = new KeyBindings();
        _keyBindings.LoadBindings();
        InitialiseKeyActions();
    }

    public void InitialiseKeyActions()
    {
        _singlePressKeyActions = new Dictionary<ActionKey, Action>
        {
            { ActionKey.Escape, HandleEscapePressed },
            { ActionKey.Move_Up, HandleUpPressed },
            { ActionKey.Move_Left, HandleLeftPressed },
            { ActionKey.Move_Down, HandleDownPressed },
            { ActionKey.Move_Right, HandleRightPressed },
            { ActionKey.Space, HandleSpacePressed }
        };

        _continuousPressKeyActions = new Dictionary<ActionKey, Action>
        {
            
        };
    }

    public void Update()
    {
        foreach (var actionKey in _singlePressKeyActions.Keys)
        {
            if (Input.GetKeyDown(_keyBindings.Keys[actionKey]))
            {
                _singlePressKeyActions[actionKey]?.Invoke();
            }
        }

        foreach (var actionKey in _continuousPressKeyActions.Keys)
        {
            if (Input.GetKey(_keyBindings.Keys[actionKey]))
            {
                _continuousPressKeyActions[actionKey]?.Invoke();
            }
        }
    }

    public void HandleUpPressed()
    {
        
    }

    public void HandleLeftPressed()
    {

    }
    public void HandleDownPressed()
    {

    }
    public void HandleRightPressed()
    {

    }

    public void HandleEscapePressed()
    {
        //if (Menu_RightClick.Instance.enabled)
        //{
        //    Menu_RightClick.Instance.RightClickMenuClose();
        //}

        //Manager_Menu.Instance.HandleEscapePressed();
    }

    public void HandleSpacePressed()
    {
        // Speed up the game
    }
}

public enum ActionKey
{
    Move_Up, Move_Left, Move_Down, Move_Right,
    Escape, Space,
}

[Serializable]
public class KeyBindings
{
    public Dictionary<ActionKey, KeyCode> Keys = new();

    public KeyBindings()
    {
        Keys.Add(ActionKey.Move_Up, KeyCode.UpArrow);
        Keys.Add(ActionKey.Move_Left, KeyCode.LeftArrow);
        Keys.Add(ActionKey.Move_Down, KeyCode.DownArrow);
        Keys.Add(ActionKey.Move_Right, KeyCode.RightArrow);

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
}
