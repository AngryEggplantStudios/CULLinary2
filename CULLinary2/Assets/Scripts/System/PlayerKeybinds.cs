using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeybindAction
{
    Run,
    Interact,
    OpenInventory,
    OpenRecipeBook,
    Trash,
    CloseMenu,
    StatsMenu
}

/// <summary>
/// Player keybinds for actions
/// </summary>
public class PlayerKeybinds
{
    private static Dictionary<KeybindAction, KeyCode> keybinds = new Dictionary<KeybindAction, KeyCode>
    {
        { KeybindAction.Interact, KeyCode.F },
        { KeybindAction.OpenInventory, KeyCode.I },
        { KeybindAction.OpenRecipeBook, KeyCode.R },
        { KeybindAction.Trash, KeyCode.Delete },
        { KeybindAction.CloseMenu, KeyCode.Escape },
        { KeybindAction.StatsMenu, KeyCode.Q },
        { KeybindAction.Run, KeyCode.LeftShift }
    };

    /// <summary>
    /// Returns a bool value based on whether the action was triggered by the player.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool WasTriggered(KeybindAction key)
    {
        return Input.GetKeyDown(keybinds[key]);
    }

    public static KeyCode GetKeybind(KeybindAction action)
    {
        return keybinds[action];
    }

    /// <summary>
    /// Changes keybind of an action given a keycode by the player. Returns true if the action is allowed, else false.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="keyCode"></param>
    /// <returns></returns>
    public static bool ChangeKeybind(KeybindAction key, KeyCode keyCode)
    {
        if (keybinds.ContainsValue(keyCode)) //Check if keycode is already being used by another action
        {
            return false;
        }
        keybinds[key] = keyCode;
        return true;
    }
}
