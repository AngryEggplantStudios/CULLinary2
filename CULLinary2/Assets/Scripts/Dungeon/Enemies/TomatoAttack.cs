using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomatoAttack : MonsterAttack
{
    [SerializeField] MonsterScript tomato;
    [SerializeField] public GameObject prefabForExplosion;
    [SerializeField] int radiusAroundOrigin;
    private SphereCollider attackCollider;
    private bool canDealDamage;
    private void Awake()
    {
        attackCollider = gameObject.GetComponent<SphereCollider>();
        canDealDamage = false;
    }

    public override void attackPlayerStart()
    {
        attackCollider.enabled = true;
    }

    public override void attackPlayerDealDamage()
    {
        canDealDamage = true;
    }

    public override void attackPlayerEnd()
    {
        attackCollider.enabled = false;
        canDealDamage = false;
        Vector3 positionToInstantiate = FindPositionInRadiusAround(0f, 90f);
        Instantiate(prefabForExplosion, positionToInstantiate, Quaternion.identity);
        positionToInstantiate = FindPositionInRadiusAround(140f, 220f);
        Instantiate(prefabForExplosion, positionToInstantiate, Quaternion.identity);
        positionToInstantiate = FindPositionInRadiusAround(270f, 360f);
        Instantiate(prefabForExplosion, positionToInstantiate, Quaternion.identity);
        EnemyAggressionManager.Instance.Add(-1);
    }      

    private Vector3 FindPositionInRadiusAround(float startDegree, float endDegree)
    {
        float randomAngle = Random.Range(startDegree, endDegree);
        float xPos = transform.position.x + radiusAroundOrigin * Mathf.Cos(randomAngle * Mathf.PI * 2 / 360);
        float zPos = transform.position.z + radiusAroundOrigin * Mathf.Sin(randomAngle * Mathf.PI * 2 / 360);

        return new Vector3(xPos, transform.position.y, zPos);
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (canDealDamage && playerHealth != null)
        {
            playerHealth.HandleHit(attackDamage);
        }
    }
}
