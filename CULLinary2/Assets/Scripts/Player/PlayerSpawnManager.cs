using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : SingletonGeneric<PlayerSpawnManager>
{
    private List<GameObject> campfires;
    private bool hasFoundCampfires = false;
    private Vector3 lastVisitedCampfirePosition;
    private Vector3 spawnOffset = Vector3.left * 5;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: get from player data if not new world
        lastVisitedCampfirePosition = Vector3.zero;
    }

    public void SpawnPlayer()
    {
        if (!hasFoundCampfires)
        {
            campfires = Campfire.GetAll();
            hasFoundCampfires = true;
        }

        if (lastVisitedCampfirePosition.Equals(Vector3.zero))
        {
            // Get nearest campfire to origin
            lastVisitedCampfirePosition = GetCampfireNearestTo(Vector3.zero).position;
        }

        Vector3 campfirePosition = lastVisitedCampfirePosition;
        Vector3 playerSpawn = campfirePosition + spawnOffset;

        gameObject.GetComponent<CharacterController>().enabled = false;
        transform.position = playerSpawn;
        gameObject.GetComponent<CharacterController>().enabled = true;
    }

    public void SetLastVisitedCampfire(Transform campfire)
    {
        lastVisitedCampfirePosition = campfire.position;
        // TODO: save to player data
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
            Debug.LogWarning("cannot find campfires");
        }

        return nearestCampfire;
    }
}
