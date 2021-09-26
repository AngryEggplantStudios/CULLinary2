using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandleHit : Monster
{
    [SerializeField] private ClownController parentController;

    public override void HandleHit(float damage)
    {
        parentController.HandleHit(damage);
    }
}
