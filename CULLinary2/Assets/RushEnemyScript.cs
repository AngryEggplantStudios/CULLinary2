using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RushEnemyScript : MonoBehaviour
{
    [SerializeField] private EnemyScript enemyScript;
    [SerializeField] private RushEnemyAttack enemyAttack;
    [SerializeField] private float distanceTriggered;

    // Variables for idle
    [SerializeField] private float idleTimer;
    [SerializeField] private float wanderRadius;
    [SerializeField] private float wanderTimer;
    [SerializeField] private float minDist;
    [SerializeField] private float distToCharge;

    public LineRenderer debugLine;
    // Variables for goingBackToStart
    private float goingBackToStartTimer;
    private float presetSpeed = 30.0f;
    private float presetAccel = 90.0f;
    private float chargingSpeed = 300.0f;
    private float chargingAccel = 1500.0f;
    // Variables for roaming
    private Vector3 roamPosition;

    // Variables for attacking
    private bool canAttack = true;
    [SerializeField] private float timeBetweenAttacks;

    private Animator animator;
    private NavMeshAgent agent;
    private NavMeshHit navHit;
    private Transform player;
    private float timer;
    private Vector3 startingPosition;
    private Vector3? chargeDirection;
    private bool amInAttackState;
    [Tooltip("The minimum distance to stop. Has to be equal to Stopping distance. Cannot use stopping distance directly else navmesh agent will keep bumping into player/")]
    private float reachedPositionDistance;
    // Start is called before the first frame update
    void Start()
    {
        // Get Variables from EnemyScript
        agent = enemyScript.getNavMeshAgent();
        animator = enemyScript.getAnimator();
        player = enemyScript.getPlayerReference();
        enemyScript.onEnemyRoaming += EnemyRoaming;
        enemyScript.onEnemyChase += EnemyChase;
        enemyScript.onEnemyIdle += EnemyIdle;
        enemyScript.onEnemyAttack += EnemyAttackPlayer;
        enemyScript.onEnemyReturn += EnemyReturn;
        reachedPositionDistance = agent.stoppingDistance;
        amInAttackState = false;
        startingPosition = transform.position;
        timer = wanderTimer;
        goingBackToStartTimer = 0;
        enemyAttack = GetComponentInChildren<RushEnemyAttack>();
    }

    private void EnemyIdle()
    {
        animator.SetBool("isMoving", false);
        timer += Time.deltaTime;
        enemyScript.FindTarget();
        if (timer >= idleTimer)
        {
            Vector3 newPos = enemyScript.RandomNavSphere(startingPosition, wanderRadius, -1, minDist);
            agent.SetDestination(newPos);
            var points = new Vector3[2];

            points[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            points[1] = newPos;
            debugLine.SetPositions(points);
            timer = 0;
            enemyScript.setStateMachine(State.Roaming);
            roamPosition = newPos;
        }
    }

    private void EnemyRoaming()
    {
        animator.SetBool("isMoving", true);
        timer += Time.deltaTime;
        enemyScript.FindTarget();
        Vector3 distanceToFinalPosition = transform.position - roamPosition;
        //without this the eggplant wandering will be buggy as it may be within the Navmesh Obstacles itself
        if (timer >= wanderTimer || distanceToFinalPosition.magnitude < reachedPositionDistance)
        {
            timer = 0;
            animator.SetBool("isMoving", false);
            enemyScript.setStateMachine(State.Idle);
        }
    }

    private void EnemyChase()
    {
        Vector3 playerPositionWithoutYOffset = new Vector3(player.position.x, transform.position.y, player.position.z);
        animator.SetBool("isMoving", true);
        float directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);


        if (directionVector <= 90.0f)
        {
            //transform.LookAt(playerPositionWithoutYOffset);
            // Target within attack range
            enemyScript.setStateMachine(State.AttackTarget);
            // Add new state to attack player
        }
        else if (directionVector > enemyScript.getStopChaseDistance())
        {
            // Too far, stop chasing
            enemyScript.setStateMachine(State.GoingBackToStart);
        }
        else
        {
            // Between 90 and 140, assuming direction vector is 140
            agent.SetDestination(playerPositionWithoutYOffset);
        }


    }

    private void EnemyAttackPlayer()
    {
        Vector3 playerPositionWithoutYOffset = new Vector3(player.position.x, transform.position.y, player.position.z);
        animator.SetBool("isMoving", false);
        animator.ResetTrigger("attack");
        if (canAttack == true)
        {
            animator.SetTrigger("attack");
            amInAttackState = true;
            canAttack = false;
            StartCoroutine(DelayFire());
        }

        if (!enemyAttack.attackStarted())
		{
            Debug.Log("Setting charge diirection to null"); 
            amInAttackState = false;
            chargeDirection = null;
            //agent.speed = presetSpeed;
            //agent.acceleration = presetAccel;
/*            enemyScript.setStateMachine(State.Idle);*/
        }
        var points = new Vector3[2];

        bool canStartCharging = enemyAttack.getCanDealDamage();
        if (canStartCharging && !chargeDirection.HasValue)
		{
            agent.speed = chargingSpeed;
            agent.acceleration = chargingAccel;
            Debug.Log("StartCharging");
            //Initialize Start charging player
            //chargeDirection = playerPositionWithoutYOffset;
            chargeDirection = new Vector3(playerPositionWithoutYOffset.x, playerPositionWithoutYOffset.y, playerPositionWithoutYOffset.z) - transform.position;
            Vector3 unitVectorForChargePosition = Vector3.Normalize(chargeDirection.Value);
            
            chargeDirection = unitVectorForChargePosition * distToCharge;
            chargeDirection = chargeDirection + transform.position;

            //charge towards nearest obstacle if hit
            bool blocked = NavMesh.Raycast(transform.position, chargeDirection.Value, out navHit, NavMesh.AllAreas);

            if (blocked)
            {
                //charge to the limit of nearest obstacle
                chargeDirection = new Vector3(navHit.position.x - 0.01f, playerPositionWithoutYOffset.y, navHit.position.z - 0.01f);
            }
			points[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			points[1] = chargeDirection.Value;
            //transform.position = chargeDirection.Value;
			agent.SetDestination(chargeDirection.Value);
            debugLine.SetPositions(points);
        }
        else if (canStartCharging && chargeDirection.HasValue)
		{
            //Destination of charge already set, continue charging
            agent.SetDestination(chargeDirection.Value);
        } 
        else
		{
            // am revving up to charge, do nothing
		}
        if (!chargeDirection.HasValue)
        {
            //means not charging
            transform.LookAt(playerPositionWithoutYOffset);
            float directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);
            //cooling down cant move
            if (directionVector >= enemyScript.getStopChaseDistance())
            {
                // Target within attack range
                Debug.Log("sTOP cHASING Tt");
                enemyScript.setStateMachine(State.GoingBackToStart);
            }
        }
    }

    private void EnemyReturn()
    {
        goingBackToStartTimer += Time.deltaTime;
        animator.SetBool("isMoving", true);
        float reachedPositionDistance = 1.0f;
        transform.LookAt(startingPosition);
        agent.SetDestination(startingPosition);
        if (Vector3.Distance(transform.position, startingPosition) <= reachedPositionDistance || goingBackToStartTimer > 4.0f)
        {
            // Reached Start Position
            animator.SetBool("isMoving", false);
            enemyScript.setStateMachine(State.Idle);
            goingBackToStartTimer = 0;
            // Reset timer for roaming / idle time
            timer = 0;
        }
    }


    private IEnumerator DelayFire()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;
    }


}
