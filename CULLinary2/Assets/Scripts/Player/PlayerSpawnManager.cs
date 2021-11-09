using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : SingletonGeneric<PlayerSpawnManager>
{
    private List<GameObject> campfires;
    private bool hasFoundCampfires = false;
    // private Vector3 lastVisitedCampfirePosition = Vector3.zero;
    private GameObject lastVisitedCampfire;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SpawnPlayer()
    {
        if (!hasFoundCampfires)
        {
            campfires = Campfire.GetAll();
            hasFoundCampfires = true;
        }

        if (!lastVisitedCampfire)
        {
            // Get nearest campfire to origin
            lastVisitedCampfire = GetCampfireNearestTo(Vector3.zero).gameObject;
        }

        Vector3 spawnPosition = lastVisitedCampfire.GetComponent<PlayerSpawn>().GetPosition();

        gameObject.GetComponent<CharacterController>().enabled = false;
        transform.position = spawnPosition;
        gameObject.GetComponent<CharacterController>().enabled = true;
    }

    public void SetLastVisitedCampfire(Transform campfire)
    {
        lastVisitedCampfire = campfire.gameObject;
    }

    public Transform GetCampfireNearestTo(Vector3 givenPosition)
    {
        // Get campfire nearest to givenPosition, using squared distance
        Transform nearestCampfire = null;
        float nearestDistSqr = Mathf.Infinity;

        if (campfires.Count != 0)
        {
            foreach (GameObject campfire in campfires)
            {
                if (campfire != null)
                {
                    Vector3 directionToCampfire = campfire.transform.position - givenPosition;
                    float sqrDist = directionToCampfire.sqrMagnitude;
                    if (sqrDist < nearestDistSqr)
                    {
                        nearestDistSqr = sqrDist;
                        nearestCampfire = campfire.transform;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("PlayerSpawnManager: cannot find campfires");
        }

        return nearestCampfire;
    }
}
