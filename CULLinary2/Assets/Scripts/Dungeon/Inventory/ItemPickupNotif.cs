using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupNotif : MonoBehaviour
{
    public float destroyTime = 2f;
    void Start()
    {
        StartCoroutine(WaitAndDestroy());
    }

    // Wait 2 real seconds before destroying
    // Needed to destroy the notification when game is paused
    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSecondsRealtime(destroyTime);
        Destroy(gameObject);
    }
}
