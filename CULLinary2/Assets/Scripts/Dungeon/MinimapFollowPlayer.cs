using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is required because when driving,
// the player is disabled but we do not want to
// attack the minimap to the player and end up
// disabling the minimap as well
public class MinimapFollowPlayer : MonoBehaviour
{
    public float zCoordinate = 0.0f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = player.position;
    }
}
