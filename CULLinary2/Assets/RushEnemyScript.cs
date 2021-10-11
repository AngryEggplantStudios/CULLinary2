using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RushEnemyScript : MonoBehaviour
{
    private enum RushEnemyState
    {
        AttackStarting,
        AttackChasing,
        AttackChargingUp,
        AttackCharging,
        AttackCoolingDown
    }
    private RushEnemyState currentState;

    [SerializeField] private MonsterScript enemyScript;
    [SerializeField] private RushEnemyAttack enemyAttack;

    // Variables for idle
    [SerializeField] private float idleTimer;
    [SerializeField] private float wanderRadius;
    [SerializeField] private float wanderTimer;
    [SerializeField] private float minDistanceToWander;
    [SerializeField] private float distToCharge;
    [SerializeField] Rigidbody rb;

    public LineRenderer debugLine;
    // Variables for goingBackToStart
    private float goingBackToStartTimer;
    // Variables for roaming
    private Vector3 roamPosition;
    [Header("Test")]
    [SerializeField] private bool lineTest = false;

    // Variables for attacking
    private bool canAttack = true;
    [SerializeField] private float timeBetweenAttacks;

    private Animator animator;
    private NavMeshAgent agent;
    private NavMeshHit navHit;
    private float timer;
    private Vector3 startingPosition;
    private Vector3? chargeDirection;
    [Tooltip("The minimum distance to stop. Has to be equal to Stopping distance. Cannot use stopping distance directly else navmesh agent will keep bumping into player/")]
    private float reachedPositionDistance;
    private float stopChaseDistance;
    private Vector3 roamingPosition;
    private bool attackCollided = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get Variables from EnemyScript
        stopChaseDistance = enemyScript.getStopChaseDistance();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyScript = GetComponent<MonsterScript>();
        debugLine = GetComponent<LineRenderer>();
        enemyScript.onEnemyRoaming += EnemyRoaming;
        enemyScript.onEnemyChase += EnemyChase;
        enemyScript.onEnemyIdle += EnemyIdle;
        enemyScript.onEnemyAttack += EnemyAttackPlayer;
        enemyScript.onEnemyReturn += EnemyReturn;
        reachedPositionDistance = agent.stoppingDistance;
        startingPosition = transform.position;
        timer = wanderTimer;
        goingBackToStartTimer = 0;
        enemyAttack = GetComponentInChildren<RushEnemyAttack>();
    }

    private void EnemyIdle()
    {
        animator.SetBool("isMoving", false);
        timer += Time.deltaTime;
        if (timer >= idleTimer)
        {
			Vector3 newPos = RandomNavSphere(startingPosition, wanderRadius, -1, minDistanceToWander);
			agent.SetDestination(newPos);
			timer = 0;
            roamPosition = newPos;
            transform.LookAt(roamPosition);
            enemyScript.SetStateMachine(MonsterState.Roaming);

            if (lineTest)
            {
                Vector3[] points = new Vector3[2];
                points[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                points[1] = newPos;
                debugLine.SetPositions(points);
            }

        }
    }

    private void EnemyRoaming()
    {
        animator.SetBool("isMoving", true);
        timer += Time.deltaTime;
        Vector3 distanceToFinalPosition = transform.position - roamPosition;
        //without this the eggplant wandering will be buggy as it may be within the Navmesh Obstacles itself
        if (timer >= wanderTimer || distanceToFinalPosition.magnitude < reachedPositionDistance)
        {
            timer = 0;
            agent.SetDestination(gameObject.transform.position);
            animator.SetBool("isMoving", false);
            enemyScript.SetStateMachine(MonsterState.Idle);
        }
    }

    // moving without using navmesh
    private void movingForward(float moveSpeed)
	{
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    //Enemy Chase should never be called in Rush enemy. This is because the charge Attack distance is extremely huge, 
    //so chase will have to be even bigger and this results in infinite chasing. Instead chase is implemented in Charge
    private void EnemyChase(float stopChaseDistance, Vector3 playerPosition)
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
        else
        {
            agent.SetDestination(playerPositionWithoutYOffset);
        }

        if (Vector3.Distance(transform.position, playerPosition) > stopChaseDistance + 0.1f)
        {
            // Too far, stop chasing
            enemyScript.SetStateMachine(MonsterState.GoingBackToStart);
        }
    }
    /**
    When charging enemy is chasing the player, use Navmesh, when charging the player, use rigidbody to navigate
    4 states:
    State 1: Chasing player
    State 2: Stop + Rev up charge
    State 3: Charge
    State 4: End the charge and cool down, while checking if player out of position / range
    **/
    private void EnemyAttackPlayer(Vector3 playerPosition, bool ableToMove)
	{
        var points = new Vector3[2];
        Vector3 playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        //animator.ResetTrigger("attack");
        switch (currentState)
        {
            default:
            case RushEnemyState.AttackStarting:
                agent.enabled = true;
                slowlyRotateToLookAt(playerPositionWithoutYOffset);
                if (canAttack == true)
                {
                    //State 1
                    //animator.SetTrigger("attack");
                    canAttack = false;
                    //need attackEnded to tell when it has stopped charging and is cooling down.
                    StartCoroutine(DelayFire());
                    animator.SetBool("collide", false);
                    currentState = RushEnemyState.AttackChasing;
                }
                break;
            case RushEnemyState.AttackChasing: //2 s
                animator.SetBool("isMoving", true);
                agent.enabled = true;
                agent.SetDestination(playerPositionWithoutYOffset);
                //enable navmes to follow the player
                break;
            case RushEnemyState.AttackChargingUp: // 2s 
                if (!chargeDirection.HasValue)
                {
                    animator.SetBool("isMoving", false);
                    agent.enabled = false;
                    //Disable navmesh to charge past obstacles
                    chargeDirection = new Vector3(playerPositionWithoutYOffset.x, playerPositionWithoutYOffset.y, playerPositionWithoutYOffset.z) - transform.position;
                    Vector3 unitVectorForChargePosition = Vector3.Normalize(chargeDirection.Value);

                    chargeDirection = unitVectorForChargePosition * distToCharge;
                    chargeDirection = chargeDirection + transform.position;
                    //charge towards nearest obstacle if hit
                    bool blocked = NavMesh.Raycast(transform.position, chargeDirection.Value, out navHit, NavMesh.AllAreas);
                    Debug.Log(blocked);
                    debugLine.enabled = true;
                    if (blocked)
                    {
                        //charge to the limit of nearest obstacle
                        attackCollided = true;
                        chargeDirection = new Vector3(navHit.position.x - 1f, playerPositionWithoutYOffset.y, navHit.position.z - 1f);
                    }
                    points[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    points[1] = chargeDirection.Value;
                    //transform.position = chargeDirection.Value;
                    transform.LookAt(chargeDirection.Value);
                    movingForward(40.0f);
                    debugLine.SetPositions(points);
                }
                break;
            case RushEnemyState.AttackCharging:
                if (chargeDirection.HasValue)
                {
                    animator.ResetTrigger("attack");
                    animator.SetTrigger("attack");
                    agent.enabled = false;
                    //Destination of charge already set, continue charging
                    transform.LookAt(chargeDirection.Value);
                    movingForward(40.0f);
                    // State 3
                    float distanceToFinalChargePosition = Vector3.Distance(transform.position, chargeDirection.Value);
                    if (distanceToFinalChargePosition <= reachedPositionDistance)
                    {
                        animator.ResetTrigger("attack");
                        if (attackCollided)
						{
                            attackCollided = false;
                            animator.SetBool("collide", true);
						} else 
                        {
                            animator.SetBool("isMoving", false);
                        }
                        // reached the chargePosition
                        enemyScript.attackPlayerEnd();
                        currentState = RushEnemyState.AttackCoolingDown;
                    }
                }
                break;
            case RushEnemyState.AttackCoolingDown:
                chargeDirection = null;
                debugLine.enabled = false;
                animator.SetBool("isMoving", false);
				// State 4
				//means not charging
				//transform.LookAt(playerPositionWithoutYOffset);
				float directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);
                //cooling down cant move
                //Debug.Log(directionVector);
                if (directionVector >= stopChaseDistance)
                {
                    // Target not within attack range go back to start, else if not maintain in attack state
                    currentState = RushEnemyState.AttackStarting;
                    agent.enabled = true;
                    enemyScript.SetStateMachine(MonsterState.GoingBackToStart);
                }
                break;
        }
    }

    private IEnumerator DelayFire()
    {
        yield return new WaitForSeconds(3.5f);
        currentState = RushEnemyState.AttackChargingUp;
        yield return new WaitForSeconds(0.75f);
        enemyScript.attackPlayerStart();
        currentState = RushEnemyState.AttackCharging;
        yield return new WaitForSeconds(2);
        canAttack = true;
        currentState = RushEnemyState.AttackStarting;
    }
    /*    private void EnemyAttackPlayer(Vector3 playerPosition, bool ableToMove)
        {
            Vector3 playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
            animator.ResetTrigger("attack");
            if (canAttack == true)
            {
                //State 1
                Debug.Log("Setting attack trigger");
                animator.SetTrigger("attack");
                amInAttackState = true;
                canAttack = false;
                //need attackEnded to tell when it has stopped charging and is cooling down.
                StartCoroutine(DelayFire());
            }

            if (!amInAttackState)
            {
                //State 4
                //AttackEnded
                Debug.Log("Setting charge diirection to null");
                chargeDirection = null;
                //agent.speed = presetSpeed;
                //agent.acceleration = presetAccel;
                enemyScript.setStateMachine(State.Idle);
            }
            var points = new Vector3[2];

            // old function based on animation events; no longer use this anymore reenable when new animation is in
            //bool canStartCharging = enemyAttack.getCanDealDamage()
            if (amInAttackState && !chargeDirection.HasValue)
            {
                //State 2
                agent.enabled = false;
                agent.speed = chargingSpeed;
                agent.acceleration = chargingAccel; // previous navmesh code
                Debug.Log("StartCharging");
                //Initialize Start charging player
                //chargeDirection = playerPositionWithoutYOffset;
                chargeDirection = new Vector3(playerPositionWithoutYOffset.x, playerPositionWithoutYOffset.y, playerPositionWithoutYOffset.z) - transform.position;
                Vector3 unitVectorForChargePosition = Vector3.Normalize(chargeDirection.Value);

                chargeDirection = unitVectorForChargePosition * distToCharge;
                chargeDirection = chargeDirection + transform.position;
                // add additional logic to adjust the position of potato
                //charge towards nearest obstacle if hit
                bool blocked = NavMesh.Raycast(transform.position, chargeDirection.Value, out navHit, NavMesh.AllAreas);
                if (blocked)
                {
                    //charge to the limit of nearest obstacle
                    chargeDirection = new Vector3(navHit.position.x - 1f, playerPositionWithoutYOffset.y, navHit.position.z - 1f);
                }
                points[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                points[1] = chargeDirection.Value;
                //transform.position = chargeDirection.Value;
                transform.LookAt(chargeDirection.Value);
                movingForward(40.0f);
                debugLine.SetPositions(points);
            }
            else if (amInAttackState && chargeDirection.HasValue)
            {
                //State 3
                agent.enabled = false;
                Debug.Log("In middle of charging");
                //Destination of charge already set, continue charging
                transform.LookAt(chargeDirection.Value);
                movingForward(40.0f);
                    //agent.SetDestination(chargeDirection.Value);
                else
                {
                    // State 2
                    agent.enabled = true;
                    agent.SetDestination(playerPositionWithoutYOffset);
                    //enable navmes to follow the player?
                    Debug.Log("DoingNothing");
                    // am revving up to charge, do nothing
                }
                if (chargeDirection.HasValue)
                {
                    // State 3
                    float distanceToFinalChargePosition = Vector3.Distance(transform.position, chargeDirection.Value);
                    if (distanceToFinalChargePosition <= reachedPositionDistance)
                    {
                        // reached the chargePosition
                        amInAttackState = false;
                    }
                }
                else
                {
                    // State 4
                    //means not charging
                    transform.LookAt(playerPositionWithoutYOffset);
                    float directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);
                    //cooling down cant move
                    if (directionVector >= stopChaseDistance + 0.1f)
                    {
                        // Target not within attack range go back to start, else if not maintain in attack state
                        Debug.Log("sTOP cHASING Tt");
                        enemyScript.SetStateMachine(MonsterState.GoingBackToStart);
                    }
                }
            }
        }*/
    private void EnemyReturn()
    {
        goingBackToStartTimer += Time.deltaTime;
        slowlyRotateToLookAt(startingPosition);
        animator.SetBool("isMoving", true);
        agent.SetDestination(startingPosition);

        if (lineTest)
        {
            Vector3[] points = new Vector3[2];
            points[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            points[1] = startingPosition;
            debugLine.SetPositions(points);
        }

        //|| goingBackToStartTimer > 4.0f
        if (Vector3.Distance(transform.position, startingPosition) <= reachedPositionDistance)
        {
            // Reached Start Position
            animator.SetBool("isMoving", false);
            enemyScript.SetStateMachine(MonsterState.Idle);
            goingBackToStartTimer = 0;
            // Reset timer for roaming / idle time
            timer = 0;
        }
    }



    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask, float minDistanceToWander)
    {
        Vector2 randPos = Random.insideUnitCircle * dist;
        Vector3 randDirection = new Vector3(randPos.x, transform.position.y, randPos.y);
        while ((randDirection - origin).magnitude < minDistanceToWander)
        {
            randPos = Random.insideUnitCircle * dist;
            randDirection = new Vector3(randPos.x, transform.position.y, randPos.y);
        }
        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    private void slowlyRotateToLookAt(Vector3 target)
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(Quaternion.LookRotation(target - transform.position).eulerAngles),
            Time.deltaTime * 6.0f);
    }
}
