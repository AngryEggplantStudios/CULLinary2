using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossRushEnemyScript : MonoBehaviour
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
    [SerializeField] protected bool lineTest = false;

    // Variables for attacking
    protected bool canAttack = true;
    [SerializeField] protected float timeBetweenAttacks;

    protected Animator animator;
    protected NavMeshAgent agent;
    private NavMeshHit navHit;
    private float timer;
    private Vector3 startingPosition;
    private Vector3? chargeDirection;
    [Tooltip("The minimum distance to stop. Has to be equal to Stopping distance. Cannot use stopping distance directly else navmesh agent will keep bumping into player/")]
    protected float reachedPositionDistance;
    private float stopChaseDistance;
    private Vector3 roamingPosition;
    private bool attackCollided = false;
    public BossEnemyScript enemyScript;

    // Start is called before the first frame update
    void Start()
    {
        // Get Variables from EnemyScript
        stopChaseDistance = enemyScript.getStopChaseDistance();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyScript = GetComponent<BossEnemyScript>();
        debugLine = GetComponent<LineRenderer>();
        enemyScript.onEnemyChase += EnemyChase;
        enemyScript.onEnemyAttack += EnemyAttackPlayer;
        reachedPositionDistance = agent.stoppingDistance;
        startingPosition = transform.position;
        timer = wanderTimer;
        goingBackToStartTimer = 0;
        childClassPreStartFunctions();
    }

    protected virtual void childClassPreStartFunctions()
    {
        //default do nothing, for children class to override;
        enemyAttack = GetComponentInChildren<RushEnemyAttack>();
    }

    // moving without using navmesh
    private void movingForward(float moveSpeed)
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    //Enemy Chase should never be called in Rush enemy. This is because the charge Attack distance is extremely huge, 
    //so chase will have to be even bigger and this results in infinite chasing. Instead chase is implemented in Charge
    protected virtual void EnemyChase(float stopChaseDistance, Vector3 playerPosition)
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
    }
    /**
    When charging enemy is chasing the player, use Navmesh, when charging the player, use rigidbody to navigate
    4 states:
    State 1: Chasing player
    State 2: Stop + Rev up charge
    State 3: Charge
    State 4: End the charge and cool down, while checking if player out of position / range
    **/
    protected virtual void EnemyAttackPlayer(Vector3 playerPosition, bool ableToMove)
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
                        }
                        else
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
                    enemyScript.SetStateMachine(MonsterState.ChaseTarget);
                }
                break;
        }
    }

    protected virtual IEnumerator DelayFire()
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

    public void DestroyObjectEvents()
    {
        enemyScript.onEnemyChase -= EnemyChase;
        enemyScript.onEnemyAttack -= EnemyAttackPlayer;
    }

    protected void slowlyRotateToLookAt(Vector3 target)
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(Quaternion.LookRotation(target - transform.position).eulerAngles),
            Time.deltaTime * 6.0f);
    }
}
