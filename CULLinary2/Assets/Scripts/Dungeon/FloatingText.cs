using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float destroyTime = 1f;
    private PlayerHealth healthPlayer;
    void Start()
    {
        healthPlayer = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<PlayerHealth>();
        Destroy(gameObject, destroyTime);
    }

	private void OnDestroy()
	{
        healthPlayer.DestroyDamageCounter(gameObject);
	}
}
