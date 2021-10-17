using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : PlayerInteractable
{
    public SpherePlayerCollider spCollider;
    public GameObject[] spawnOnOpen;
    public GameObject[] destroyOnOpen;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    public override SpherePlayerCollider GetCollider()
    {
        return spCollider;
    }

    public override void OnPlayerEnter()
    {
    }

    public override void OnPlayerInteract()
    {
        animator.enabled = true;
        foreach (GameObject obj in spawnOnOpen)
        {
            Instantiate(obj, transform.position, transform.rotation);
        }
        foreach (GameObject obj in destroyOnOpen)
        {
            Destroy(obj);
        }
    }

    public override void OnPlayerLeave()
    {
    }
}
