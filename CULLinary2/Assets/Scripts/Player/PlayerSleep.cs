using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSleep : MonoBehaviour
{
    [SerializeField] private GameObject sceneTransition;

    void Start()
    {
        GameTimer.OnEndOfDay += Sleep;
    }

    private void Sleep()
    {
        Debug.Log("Time to sleep");
        // sceneTransition.SetActive(true);
        transform.position = new Vector3(0, 0, 0);
    }

    void OnDestroy()
    {
        GameTimer.OnEndOfDay -= Sleep;
    }

}
