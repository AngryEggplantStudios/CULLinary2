using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObstacleManager : MonoBehaviour
{
    public Transform cam;
    public Transform player;
    public Transform truckCam;
    public Transform driveableTruck;

    private Transform currentCam;
    private Transform currentEntity;
    private List<CameraObstacle> blocking;
    private List<CameraObstacle> transparent;

    void Start()
    {
        currentCam = cam;
        currentEntity = player;
        blocking = new List<CameraObstacle>();
        transparent = new List<CameraObstacle>();
    }

    void Update()
    {
        if (DrivingManager.instance != null && DrivingManager.instance.IsPlayerInVehicle())
        {
            currentCam = truckCam;
            currentEntity = driveableTruck;
        }
        else
        {
            currentCam = cam;
            currentEntity = player;
        }

        blocking.Clear();

        float dist = Vector3.Magnitude(currentCam.position - currentEntity.position);

        Ray rayF = new Ray(currentCam.position, currentEntity.position - currentCam.position);
        Ray rayB = new Ray(currentEntity.position, currentCam.position - currentEntity.position);

        var hitsF = Physics.RaycastAll(rayF, dist);
        var hitsB = Physics.RaycastAll(rayB, dist);

        foreach (var hit in hitsF)
        {
            if (hit.collider.gameObject.TryGetComponent(out CameraObstacle obstacle))
            {
                if (!blocking.Contains(obstacle))
                {
                    blocking.Add(obstacle);
                }
            }
        }

        foreach (var hit in hitsB)
        {
            if (hit.collider.gameObject.TryGetComponent(out CameraObstacle obstacle))
            {
                if (!blocking.Contains(obstacle))
                {
                    blocking.Add(obstacle);
                }
            }
        }

        // If transparent but not blocking, make solid
        for (int i = transparent.Count - 1; i >= 0; i--)
        {
            CameraObstacle obj = transparent[i];

            if (!blocking.Contains(obj))
            {
                obj.ShowSolid();
                transparent.Remove(obj);
            }
        }

        // If blocking but still solid, make transparent
        for (int i = 0; i < blocking.Count; i++)
        {
            CameraObstacle obj = blocking[i];

            if (!transparent.Contains(obj))
            {
                obj.ShowTransparent();
                transparent.Add(obj);
            }
        }
    }
}
