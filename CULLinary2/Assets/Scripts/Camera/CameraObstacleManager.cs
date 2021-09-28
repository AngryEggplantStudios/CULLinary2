using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObstacleManager : MonoBehaviour
{
    public Transform camera;
    public Transform player;

    private List<CameraObstacle> blocking;
    private List<CameraObstacle> transparent;

    void Start()
    {
        blocking = new List<CameraObstacle>();
        transparent = new List<CameraObstacle>();
    }

    void Update()
    {
        blocking.Clear();

        float dist = Vector3.Magnitude(camera.position - player.position);

        Ray rayF = new Ray(camera.position, player.position - camera.position);
        Ray rayB = new Ray(player.position, camera.position - player.position);

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
