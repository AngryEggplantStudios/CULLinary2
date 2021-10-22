using UnityEngine;

public class HamMonsterBehavior : MonsterBehavior
{
    [SerializeField] private HamAttack hamAttackScript;

    public override void EnemyAttackPlayer(Vector3 playerPosition, bool ableToMove)
    {
        Vector3 playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        ableToMove = hamAttackScript.getCanMove();
        if (ableToMove)
		{
            slowlyRotateToLookAt(playerPositionWithoutYOffset);
		}

        if (canAttack == true)
        {
            navMeshAgent.SetDestination(gameObject.transform.position);
            canAttack = false;
            StartCoroutine(DelayAttack(playerPositionWithoutYOffset));
        }
        if (!ableToMove)
		{
            navMeshAgent.SetDestination(gameObject.transform.position);
        }
        float directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);
        if (directionVector > navMeshAgent.stoppingDistance && ableToMove)
        {
            // Target within attack range
            monsterScript.SetStateMachine(MonsterState.ChaseTarget);
        }
    }

    public override void EnemyChase(float stopChaseDistance, Vector3 playerPosition)
    {
        Vector3 playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        animator.SetBool("isMoving", true);
        float directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);

        if (lineTest)
        {
            Vector3[] points = new Vector3[2];
            points[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            points[1] = playerPositionWithoutYOffset;
            lineRenderer.SetPositions(points);
        }


        if (directionVector <= reachedPositionDistance )
        {
            playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
            slowlyRotateToLookAt(playerPositionWithoutYOffset);
            navMeshAgent.SetDestination(gameObject.transform.position);
            // Target within attack range
            monsterScript.SetStateMachine(MonsterState.AttackTarget);
            // Add new state to attack player
        } 
        else if (!hamAttackScript.getCanMove())
		{
            navMeshAgent.SetDestination(gameObject.transform.position);
            // Target within attack range
            monsterScript.SetStateMachine(MonsterState.AttackTarget);
        }
        else
        {
            navMeshAgent.SetDestination(playerPositionWithoutYOffset);
        }

        if (Vector3.Distance(transform.position, playerPosition) > stopChaseDistance + 0.1f)
        {
            // Too far, stop chasing
            monsterScript.SetStateMachine(MonsterState.GoingBackToStart);
        }
    }

    // need a seprate method coz ham needs to stop moving earlier than all other mobs
    public void hamStopMoving()
	{
        hamAttackScript.hamAttackStopMoving();
    }
}
