using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWorldSpace : MonoBehaviour
{
    public Transform FollowThis;
 
    void Update()
    {
        if (FollowThis == null || Camera.main == null) { return; }
        this.transform.position = Camera.main.WorldToScreenPoint(FollowThis.position);
    }

    void OnEnable()
    {
        if (gameObject.activeInHierarchy) { Update(); }
    }
}
