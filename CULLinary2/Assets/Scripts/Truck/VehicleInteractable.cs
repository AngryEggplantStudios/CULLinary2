using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleInteractable : PlayerInteractable
{
    public SpherePlayerCollider spCollider;

    public override SpherePlayerCollider GetCollider()
    {
        return spCollider;
    }

    public override void OnPlayerEnter()
    { }
    
    public override void OnPlayerInteract()
    {
        DrivingManager.instance.HandlePlayerEnterVehicle();
    }

    public override void OnPlayerLeave()
    { }
}
