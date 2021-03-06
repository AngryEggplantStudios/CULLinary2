using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBehavior : MonoBehaviour
{
    [Header("Monster Behavior Variables")]
    [SerializeField] private float idleTimer;
    [SerializeField] private float wanderRadius;
    [SerializeField] public float wanderTimer;

    [Tooltip("The minimum distance to wander about. Needed because of the stopping distance being large makes the enemy only wander a bit before stopping.")]
    [SerializeField] private float minDist;
    [SerializeField] protected float timeBetweenAttacks;

    [Header("Test")]
    [SerializeField] public bool lineTest = false;
    // Variables

    public float goingBackToStartTimer;
    private Vector3 roamPosition;
    public bool canAttack = true;
    public Animator animator;
    public NavMeshAgent navMeshAgent;

    public float timer;
    public Vector3 startingPosition;
    //The minimum distance to stop in front of the player. Has to be equal to Stopping distance. Cannot use stopping distance directly else navmesh agent will keep bumping into player/
    public float reachedPositionDistance;
    public MonsterScript monsterScript;
    public LineRenderer lineRenderer;
    protected NavMeshPath path;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        monsterScript = GetComponent<MonsterScript>();
        lineRenderer = GetComponent<LineRenderer>();
        monsterScript.onEnemyRoaming += EnemyRoaming;
        monsterScript.onEnemyChase += EnemyChase;
        monsterScript.onEnemyIdle += EnemyIdle;
        monsterScript.onEnemyAttack += EnemyAttackPlayer;
        monsterScript.onEnemyReturn += EnemyReturn;
        reachedPositionDistance = navMeshAgent.stoppingDistance;
        startingPosition = transform.position;
        timer = wanderTimer;
        path = new NavMeshPath();
        goingBackToStartTimer = 0;
        childClassPreStartFunctions();
    }

    protected virtual void childClassPreStartFunctions()
	{
        //default do nothing, for children class to override;
	}

    public virtual void EnemyIdle()
    {
        animator.SetBool("isMoving", false);
        timer += Time.deltaTime;
        if (timer >= idleTimer)
        {
            Vector3 newPos = RandomNavSphere(startingPosition, wanderRadius, -1, minDist);
			NavMesh.CalculatePath(transform.position, newPos, NavMesh.AllAreas, path);
			navMeshAgent.SetPath(path);
			timer = 0;
            monsterScript.SetStateMachine(MonsterState.Roaming);
            roamPosition = newPos;

            if (lineTest)
            {
                Vector3[] points = new Vector3[2];
                points[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                points[1] = newPos;
                lineRenderer.SetPositions(points);
            }

        }
    }

    public virtual void EnemyRoaming()
    {
        animator.SetBool("isMoving", true);
        timer += Time.deltaTime;
        Vector3 distanceToFinalPosition = transform.position - roamPosition;
        //without this the eggplant wandering will be buggy as it may be within the Navmesh Obstacles itself
        if (timer >= wanderTimer || distanceToFinalPosition.magnitude < reachedPositionDistance)
        {
            timer = 0;
            navMeshAgent.SetDestination(gameObject.transform.position);
            animator.SetBool("isMoving", false);
            monsterScript.SetStateMachine(MonsterState.Idle);
        }
    }

    public virtual void EnemyChase(float stopChaseDistance, Vector3 playerPosition)
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


        if (directionVector <= reachedPositionDistance)
        {
            playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
            slowlyRotateToLookAt(playerPositionWithoutYOffset);
            navMeshAgent.SetDestination(gameObject.transform.position);
            // Target within attack range
            monsterScript.SetStateMachine(MonsterState.AttackTarget);
            // Add new state to attack player
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

    public virtual void EnemyAttackPlayer(Vector3 playerPosition, bool ableToMove)
    {
        Vector3 playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        slowlyRotateToLookAt(playerPositionWithoutYOffset);

        if (canAttack)
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

    public virtual IEnumerator DelayAttack(Vector3 playerPositionWithoutYOffset)
    {
        yield return new WaitForSeconds(1);
        slowlyRotateToLookAt(playerPositionWithoutYOffset);
        animator.SetBool("isMoving", false);
        animator.ResetTrigger("attack");
        animator.SetTrigger("attack");
        StartCoroutine(DelayFire());
    }

    public virtual void EnemyReturn()
    {
        goingBackToStartTimer += Time.deltaTime;
        slowlyRotateToLookAt(startingPosition);
        animator.SetBool("isMoving", true);
        navMeshAgent.SetDestination(startingPosition);
        if (lineTest)
        {
            Vector3[] points = new Vector3[2];
            points[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            points[1] = startingPosition;
            lineRenderer.SetPositions(points);
        }

        //|| goingBackToStartTimer > 4.0f
        if (Vector3.Distance(transform.position, startingPosition) <= reachedPositionDistance)
        {
            // Reached Start Position
            animator.SetBool("isMoving", false);
            monsterScript.SetStateMachine(MonsterState.Idle);
            goingBackToStartTimer = 0;
            // Reset timer for roaming / idle time
            timer = 0;
        }
    }


    protected virtual IEnumerator DelayFire()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;
    }

    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask, float minDist)
    {
        Vector2 randPos = Random.insideUnitCircle * dist;
        Vector3 randDirection = new Vector3(randPos.x, transform.position.y, randPos.y);
        while ((randDirection - origin).magnitude < minDist)
        {
            randPos = Random.insideUnitCircle * dist;
            randDirection = new Vector3(randPos.x, transform.position.y, randPos.y);
        }
        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    // Use this isntead of OnDestroy due to global enemy Detection unable to call OnDestroy on disabled
	public void DestroyObjectEvents()
	{
        monsterScript.onEnemyRoaming -= EnemyRoaming;
        monsterScript.onEnemyChase -= EnemyChase;
        monsterScript.onEnemyIdle -= EnemyIdle;
        monsterScript.onEnemyAttack -= EnemyAttackPlayer;
        monsterScript.onEnemyReturn -= EnemyReturn;
    }

	public void slowlyRotateToLookAt(Vector3 target)
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(Quaternion.LookRotation(target - transform.position).eulerAngles),
            Time.deltaTime * 3.0f);
    }


}
