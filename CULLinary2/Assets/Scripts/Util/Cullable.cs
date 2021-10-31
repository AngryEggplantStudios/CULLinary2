using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cullable : MonoBehaviour
{
    void Start()
    {
        ObjectCuller.Instance.Add(this.gameObject);
    }

    void OnDestroy()
    {
        ObjectCuller.Instance.Remove(this.gameObject);
    }
}
