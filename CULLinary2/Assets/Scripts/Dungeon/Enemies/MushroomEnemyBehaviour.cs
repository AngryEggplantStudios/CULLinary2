using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MushroomEnemyBehaviour : MonsterBehavior
{
	public enum MushroomState 
    { 
        FindingPositionToRunTo,
        Running,
        ChilingAfterRunning
    }

    [SerializeField] protected float lengthOfRelaxTime;
    [SerializeField] protected float runAwayRadius;
    [SerializeField] protected float minRunAwayRadius;
    [SerializeField] protected float runningAwaySpeed;
    [SerializeField] protected float runningAwayAccel;
    private bool isRunningAwayCoroutineRunning = false;
    private Vector3? pointToRunAwayTo;
    private MushroomState currentMushState;
    private float originalRunningSpeed;
    private float originalRunningAccel;

	protected override void childClassPreStartFunctions()
	{
        originalRunningAccel = navMeshAgent.acceleration;
        originalRunningSpeed = navMeshAgent.speed;
	}

	// find places away from player to run towards and stops there
	public override void EnemyChase(float stopChaseDistance, Vector3 playerPosition)
    {
        // stop form moving
        Vector3 playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        float directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);
        // if in middle stop 
        navMeshAgent.SetDestination(gameObject.transform.position);

        if (directionVector >= stopChaseDistance + 0.1f)
        {
            // Too far, stop chasing
            monsterScript.SetStateMachine(MonsterState.Roaming);
        }
        else
        {
            monsterScript.SetStateMachine(MonsterState.AttackTarget);
        }
    }

    public override void EnemyAttackPlayer(Vector3 playerPosition, bool ableToMove)
    {
        Vector3 playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        // here canAttack is finishedRunning 2 parts, cooldown for 3 seconds, and start finding target
        switch (currentMushState)
		{
            default:
            case MushroomState.FindingPositionToRunTo:
                float directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);
                // use stopChase to stopRunningAway
                if (directionVector >= monsterScript.GetStopChase())
                {
                    // fully reset canAttack, safe from player, can go back
                    resetAllVariables();
                    monsterScript.SetStateMachine(MonsterState.Idle);
                }
                else
                {
                    if (!pointToRunAwayTo.HasValue)
                    {
                        //start finding position to move away to and start moving away.
                        Vector3 centralPositionFromPlayer = (transform.position - playerPositionWithoutYOffset).normalized;
                        Vector3 finalCenterOfRadius = transform.position + new Vector3(centralPositionFromPlayer.x * 4, transform.position.y, centralPositionFromPlayer.z * 4);
                        Vector3 newPos = SampleRandomPosition(finalCenterOfRadius, runAwayRadius, -1, minRunAwayRadius);
                        newPos.y = transform.position.y;
                        pointToRunAwayTo = newPos;

                        if (lineTest)
                        {
                            Vector3[] points = new Vector3[2];
                            points[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                            points[1] = newPos;
                            lineRenderer.SetPositions(points);
                        }
                        slowlyRotateToLookAt(pointToRunAwayTo.Value);
                        navMeshAgent.acceleration = runningAwayAccel;
                        navMeshAgent.speed = runningAwaySpeed;
                        currentMushState = MushroomState.Running;
                    }
                }
                break;
            case MushroomState.Running:
                animator.ResetTrigger("attack");
                animator.SetTrigger("attack");
                slowlyRotateToLookAt(pointToRunAwayTo.Value);
                NavMesh.CalculatePath(transform.position, pointToRunAwayTo.Value, NavMesh.AllAreas, path);
                navMeshAgent.SetPath(path);
                float distanceToFinalPosition = Vector3.Distance(transform.position, pointToRunAwayTo.Value);
                if (distanceToFinalPosition < reachedPositionDistance + 5.0f)
                {
                    currentMushState = MushroomState.ChilingAfterRunning;
                    animator.ResetTrigger("attack");
					animator.SetBool("isMoving", false);
				}
				break;
            case MushroomState.ChilingAfterRunning:
                if (!isRunningAwayCoroutineRunning)
                {
                    StartCoroutine(coroutineToStartRunningAway());
                }
                break;
        }


    }

    private void resetAllVariables()
	{
        Debug.Log("Reset all variables called");
        pointToRunAwayTo = null;
        if (isRunningAwayCoroutineRunning)
		{
            StopCoroutine(coroutineToStartRunningAway());
		}
        navMeshAgent.acceleration = originalRunningAccel;
        navMeshAgent.speed = originalRunningSpeed;
        currentMushState = MushroomState.FindingPositionToRunTo;
    }

    private IEnumerator coroutineToStartRunningAway()
	{
        isRunningAwayCoroutineRunning = true;
        yield return new WaitForSeconds(lengthOfRelaxTime);
        isRunningAwayCoroutineRunning = false;
        currentMushState = MushroomState.FindingPositionToRunTo;
        pointToRunAwayTo = null;
	}

    private Vector3 SampleRandomPosition(Vector3 origin, float dist, int layermask, float minDist)
    {
        Vector2 randPos = Random.insideUnitCircle * dist;
        Vector3 randDirection = new Vector3(randPos.x, transform.position.y, randPos.y);
        bool hasFoundPos = false;
        NavMeshHit navHit;
        while (!hasFoundPos)
		{

            while ((randDirection - origin).magnitude < minDist)
            {
                randPos = Random.insideUnitCircle * dist;
                randDirection = new Vector3(randPos.x, transform.position.y, randPos.y);
            }
            randDirection += origin;

            hasFoundPos = NavMesh.SamplePosition(randDirection, out navHit, 1, layermask);
            if (hasFoundPos)
			{
                return navHit.position;
            }
        }
        return randDirection;
    }
}
