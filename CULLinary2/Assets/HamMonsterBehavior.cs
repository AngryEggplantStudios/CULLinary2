using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamMonsterBehavior : MonsterBehavior
{
    [SerializeField] private HamAttack hamAttackScript;

    public override void EnemyAttackPlayer(Vector3 playerPosition, bool ableToMove)
    {
        Vector3 playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        Debug.Log(hamAttackScript.getCanDealDamage());
        if (hamAttackScript.getCanDealDamage())
		{
            slowlyRotateToLookAt(playerPositionWithoutYOffset);
		}

        if (canAttack == true)
        {
            canAttack = false;
            StartCoroutine(DelayAttack(playerPositionWithoutYOffset));
        }
        float directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);
        if (directionVector > navMeshAgent.stoppingDistance && ableToMove)
        {
            // Target within attack range
            monsterScript.SetStateMachine(MonsterState.ChaseTarget);
        }
    }
}
