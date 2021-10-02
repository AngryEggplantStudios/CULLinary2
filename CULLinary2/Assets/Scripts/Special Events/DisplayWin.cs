using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DisplayWin : SingletonGeneric<DisplayWin>
{
    // Called when the clown dies
    [SerializeField] private UnityEvent whenClownKilledCallback;

    public void DisplayUI()
    {
    }
}
