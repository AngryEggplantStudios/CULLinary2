using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossHamMonsterBehaviour : BossRushEnemyScript
{
    [SerializeField] private HamAttack hamAttackScript;

    protected override void EnemyAttackPlayer(Vector3 playerPosition, bool ableToMove)
    {
        Vector3 playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        ableToMove = hamAttackScript.getCanMove();
        if (ableToMove)
        {
            slowlyRotateToLookAt(playerPositionWithoutYOffset);
        }

        if (canAttack == true)
        {
            agent.SetDestination(gameObject.transform.position);
            canAttack = false;
            StartCoroutine(DelayAttack(playerPositionWithoutYOffset));
        }
        if (!ableToMove)
        {
            agent.SetDestination(gameObject.transform.position);
        }
        float directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);
        if (directionVector > agent.stoppingDistance && ableToMove)
        {
            // Target within attack range
            enemyScript.SetStateMachine(MonsterState.ChaseTarget);
        }
    }

    protected override void EnemyChase(float stopChaseDistance, Vector3 playerPosition)
    {
        Vector3 playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        animator.SetBool("isMoving", true);
        float directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);

        if (lineTest)
        {
            Vector3[] points = new Vector3[2];
            points[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            points[1] = playerPositionWithoutYOffset;
            debugLine.SetPositions(points);
        }


        if (directionVector <= reachedPositionDistance)
        {
            playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
            slowlyRotateToLookAt(playerPositionWithoutYOffset);
            agent.SetDestination(gameObject.transform.position);
            // Target within attack range
            enemyScript.SetStateMachine(MonsterState.AttackTarget);
            // Add new state to attack player
        }
        else if (!hamAttackScript.getCanMove())
        {
            agent.SetDestination(gameObject.transform.position);
            // Target within attack range
            enemyScript.SetStateMachine(MonsterState.AttackTarget);
        }
        else
        {
            agent.SetDestination(playerPositionWithoutYOffset);
        }
    }

    public virtual IEnumerator DelayAttack(Vector3 playerPositionWithoutYOffset)
    {
        yield return new WaitForSeconds(1);
        slowlyRotateToLookAt(playerPositionWithoutYOffset);
        animator.SetBool("isMoving", false);
        animator.ResetTrigger("attack");
        animator.SetTrigger("attack");
        StartCoroutine(DelayFire());
    }

    protected override IEnumerator DelayFire()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;
    }

    // need a seprate method coz ham needs to stop moving earlier than all other mobs
    public void hamStopMoving()
    {
        hamAttackScript.hamAttackStopMoving();
    }
}
