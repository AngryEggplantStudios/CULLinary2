using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A helper script to set the correct value in UI Controller
public class SetUIControllerActiveTab : MonoBehaviour
{
    public int thisUiPage = 0;

    void OnEnable()
    {
        if (UIController.instance)
        {
            UIController.instance.SetCurrentUiPage(thisUiPage);   
        }
    }
}
