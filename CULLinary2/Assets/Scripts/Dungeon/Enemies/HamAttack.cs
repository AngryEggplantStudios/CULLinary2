using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamAttack  : MonsterAttack
{
    private SpriteRenderer attackSprite;
    private SphereCollider attackCollider;
    private PlayerHealth healthScript;
    private bool canDealDamage;
    private bool playerWithinAttackCone;
    [SerializeField] private LineRenderer LR1;
    [SerializeField] private GameObject parentGameObjectHam;

    private void Awake()
    {
        attackSprite = gameObject.GetComponent<SpriteRenderer>();
        attackCollider = gameObject.GetComponent<SphereCollider>();
        canDealDamage = false;
        playerWithinAttackCone = false;
    }

	private void Update()
	{
        SetLineRendererForDebug();
    }

    private void SetLineRendererForDebug()
	{
        //Debug.Log("Update is being called");
        Vector3 forwardDirection = transform.forward;
        Vector3 LeftHalfVector = Quaternion.AngleAxis(60, Vector3.up) * forwardDirection;
        Vector3 RightHalfVector = Quaternion.AngleAxis(-60, Vector3.up) * forwardDirection;
        Vector3[] points = new Vector3[2];
        points[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        points[1] = forwardDirection;
        LR1.SetPositions(points);

    }

    public override void attackPlayerStart()
    {
        attackSprite.enabled = true;
        attackCollider.enabled = true;
    }

    public override void attackPlayerDealDamage()
    {
        canDealDamage = true;
        ScreenShake.Instance.Shake(0.4f, 1f, 0.2f, 1f);
    }

    public override void attackPlayerEnd()
    {
        attackSprite.enabled = false;
        //Destroy(selectionCircleActual.gameObject);
        attackCollider.enabled = false;
        canDealDamage = false;
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (canDealDamage && playerHealth != null)
        {
            if (playerWithinAttackCone)
            {
                playerWithinAttackCone = false;
                playerHealth.HandleHit(attackDamage);
            }
        }
    }
}
