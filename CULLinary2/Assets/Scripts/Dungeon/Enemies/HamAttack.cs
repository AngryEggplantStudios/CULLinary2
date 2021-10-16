using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamAttack  : MonsterAttack
{
    private SphereCollider attackCollider;
    private PlayerHealth healthScript;
    private bool canDealDamage;
    [SerializeField] private GameObject parentGO;
    [SerializeField] private float angleFromForwardVectorToHit;
    [SerializeField] private GameObject canvasObject;
    private Transform playerTransform;
    private Vector3 playerPosition;
    private bool canMove;

    private void Awake()
    {
        attackCollider = gameObject.GetComponent<SphereCollider>();
        canDealDamage = false;
        canMove = true;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
	{
        //SetLineRendererForDebug();
    }

    public bool getCanDealDamage()
	{
        return canMove;
	}

    private bool checkIsPlayerWIthinCone()
	{
        playerPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z) - transform.position;
        Vector3 forwardDirection = parentGO.transform.forward;
        float angleBetweenTwoVectors = Vector3.Angle(playerPosition, forwardDirection);
        return angleBetweenTwoVectors <= angleFromForwardVectorToHit;
    }

    public override void attackPlayerStart()
    {
        attackCollider.enabled = true;
        canMove = false;
        canvasObject.SetActive(true);
    }

    public override void attackPlayerDealDamage()
    {
        canDealDamage = true;
        ScreenShake.Instance.Shake(0.4f, 1f, 0.2f, 1f);
    }

    public override void attackPlayerEnd()
    {
        //Destroy(selectionCircleActual.gameObject);
        attackCollider.enabled = false;
        canDealDamage = false;
        canMove = true;
        canvasObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (canDealDamage && playerHealth != null)
        {
            if (checkIsPlayerWIthinCone())
            {
                playerHealth.HandleHit(attackDamage);
            }
        }
    }
}
