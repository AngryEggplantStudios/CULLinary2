using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckEnabler : MonoBehaviour
{
    public GameObject truck;

    private bool hasEnabled = false;
    void Update()
    {
        if (!hasEnabled)
        {
            truck.GetComponent<CarController>().enabled = true;
            truck.GetComponent<CarController>().AddOnCollisionAction(decel =>
                Debug.Log("Ouch! " + decel));
            hasEnabled = true;
        }
    }
}
