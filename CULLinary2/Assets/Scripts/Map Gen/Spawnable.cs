using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Spawnable
{
    public bool isSpawnable;
    public string name;
    public GameObject[] prefabs;
    public int density;
    public float intersectRadius;
    public bool removeCollider;
}