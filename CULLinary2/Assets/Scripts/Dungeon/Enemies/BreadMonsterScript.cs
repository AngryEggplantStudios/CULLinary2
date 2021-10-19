using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BreadMonsterScript : MonsterBehavior
{

    public override void EnemyChase(float stopChaseDistance, Vector3 playerPosition)
    {
        // stop form moving
        navMeshAgent.SetDestination(gameObject.transform.position);
        Vector3 playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        slowlyRotateToLookAt(playerPositionWithoutYOffset);
        float directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);
        if (directionVector >= stopChaseDistance)
        {
            // Target within attack range
            monsterScript.SetStateMachine(MonsterState.Idle);
        }
        else
		{
            monsterScript.SetStateMachine(MonsterState.AttackTarget);
		}
    }

    public override void EnemyAttackPlayer(Vector3 playerPosition, bool ableToMove)
    {
        Vector3 playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        slowlyRotateToLookAt(playerPositionWithoutYOffset);

        if (canAttack == true)
        {
            canAttack = false;
            StartCoroutine(DelayAttack(playerPositionWithoutYOffset));
        }
        float directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);
        if (directionVector >= monsterScript.getStopChase())
        {
            // Target within attack range
            monsterScript.SetStateMachine(MonsterState.Idle);
        }
    }

}