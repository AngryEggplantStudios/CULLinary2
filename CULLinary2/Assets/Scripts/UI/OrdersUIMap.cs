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
        SetIconPos(playerBody, navArrow, false);
    }

    protected override Vector3 GetCentrePointOfMap()
    {
        return new Vector3(width / 2, height / 2, 0);
    }
}
