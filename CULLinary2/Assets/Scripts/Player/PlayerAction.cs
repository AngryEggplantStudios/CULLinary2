using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private bool isInvoking = false;

    public void SetIsInvoking(bool b)
    {
        this.isInvoking = b;
    }

    public bool GetIsInvoking()
    {
        return this.isInvoking;
    }
}