using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Spawnable
{
    public string name;
    public bool isSpawnable;
    public GameObject[] prefabs;
    public int density;
    public float intersectRadius;
    public bool removeCollider;
    public bool ignoreSelfIntersection;
}