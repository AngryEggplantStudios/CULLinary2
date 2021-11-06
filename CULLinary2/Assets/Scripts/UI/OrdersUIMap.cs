using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdersUIMap : Minimap
{
    public RectTransform mapRectangle;

    public override void Update()
    { }

    void OnEnable()  
    {
        TryToSetPlayerBody();
        if (!hasInstantiatedIcons)
        {
            InstantiateMinimapIcons();
        }
        UpdateUI();
    }

    protected override float GetMapWidth()
    {
        return mapRectangle.rect.width;
    }

    protected override float GetMapHeight()
    {
        return mapRectangle.rect.height;
    }

    protected override void SetPlayerIconPos()
    {
        // Edge case where player gets into truck without opening the Orders UI before
        // We set the transform to the truck's transform
        if (DrivingManager.instance != null && DrivingManager.instance.IsPlayerInVehicle() && !hasSetPlayerBody)
        {
            playerBody = DrivingManager.instance.GetTruckTransform();
        }
        SetIconPos(playerBody, navArrow, false);
    }

    protected override Vector3 GetCentrePointOfMap()
    {
        return new Vector3(width / 2, height / 2, 0);
    }
}
