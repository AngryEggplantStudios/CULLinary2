using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWorldSpace : MonoBehaviour
{
    public Transform FollowThis;
 
    void Update()
    {
        if (FollowThis != null)
        {
            if (Camera.main == null)
            {
                // May be driving
                if (DrivingManager.instance != null &&
                    DrivingManager.instance.IsPlayerInVehicle())
                {
                    this.transform.position = DrivingManager
                        .instance
                        .GetTruckCamera()
                        .WorldToScreenPoint(FollowThis.position);
                }
            }
            else
            {
                this.transform.position = Camera
                    .main
                    .WorldToScreenPoint(FollowThis.position);
            }
        }
    }

    void OnEnable()
    {
        if (gameObject.activeInHierarchy) { Update(); }
    }
}
