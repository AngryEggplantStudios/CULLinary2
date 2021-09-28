using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdersUIMap : Minimap
{
    public RectTransform mapRectangle;
    private bool onEnableFlag = false;
    public override void Update()
    { 
        if (onEnableFlag)
        {
            // if (!hasInstantiatedIcons)
            // {
            //     // Need to instantiate in Update to get
            //     // the width and height of the map
            //     InstantiateMinimapIcons();
            //     Debug.Log("Width is " + width);
            //     Debug.Log("Height is " + height);
            // }
            // CheckIfPlayerHasMoved();
            // onEnableFlag = false;
        }
    }

    void OnEnable()  
    {
        if (!hasInstantiatedIcons)
        {
            // Need to instantiate in Update to get
            // the width and height of the map
            InstantiateMinimapIcons();
            Debug.Log("Width is " + width);
            Debug.Log("Height is " + height);
        }
        CheckIfPlayerHasMoved();
        onEnableFlag = false;
    }

    protected override float GetMapWidth()
    {
        return mapRectangle.rect.width;
    }

    protected override float GetMapHeight()
    {
        return mapRectangle.rect.height;
    }
}
