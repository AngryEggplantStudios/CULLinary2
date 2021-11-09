using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{

    [SerializeField] private Transform playerSpawnPosition;

    public Vector3 GetPosition()
    {
        return playerSpawnPosition.position;
    }
}
