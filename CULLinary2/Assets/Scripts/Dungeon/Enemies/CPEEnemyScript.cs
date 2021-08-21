using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CPEEnemyScript : MonoBehaviour
{
    [SerializeField] private EnemyScript enemyScript;
    [SerializeField] private float distanceTriggered;

    // Variables for idle
    [SerializeField] private float idleTimer;
    [SerializeField] private float wanderRadius;
    [SerializeField] private float wanderTimer;

    // Variables for goingBackToStart
    private float goingBackToStartTimer;

    // Variables for roaming
    private Vector3 roamPosition;

    // Variables for attacking
    private bool canAttack = true;
    [SerializeField] private float timeBetweenAttacks;

    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;
    private float timer;
    private Vector3 startingPosition;


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

        startingPosition = transform.position;
        timer = wanderTimer;
        goingBackToStartTimer = 0;
    }

    private void EnemyIdle()
    {
        animator.SetBool("isMoving", false);
        timer += Time.deltaTime;
        enemyScript.FindTarget();
        if (timer >= idleTimer)
        {
            Vector3 newPos = enemyScript.RandomNavSphere(startingPosition, wanderRadius, -1);
            agent.SetDestination(newPos);
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
        if (timer >= wanderTimer || distanceToFinalPosition.magnitude < 0.5f)
        {
            timer = 0;
            enemyScript.setStateMachine(State.Idle);
        }
    }

    private void EnemyChase()
    {
        Vector3 playerPositionWithoutYOffset = new Vector3(player.position.x, transform.position.y, player.position.z);
        animator.SetBool("isMoving", true);
        float directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);
        if (directionVector <= agent.stoppingDistance)
        {
            transform.LookAt(playerPositionWithoutYOffset);
            // Target within attack range
            enemyScript.setStateMachine(State.AttackTarget);
            // Add new state to attack player
        }
        else
        {
            // Debug.Log("Setting destination");
            agent.SetDestination(playerPositionWithoutYOffset);
        }

        if (Vector3.Distance(transform.position, player.position) > enemyScript.getStopChaseDistance() + 0.1f)
        {
            // Too far, stop chasing
            enemyScript.setStateMachine(State.GoingBackToStart);
        }
    }

    private void EnemyAttackPlayer()
    {
        Vector3 playerPositionWithoutYOffset = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(playerPositionWithoutYOffset);
        animator.SetBool("isMoving", false);
        animator.ResetTrigger("attack");
        if (canAttack == true)
        {
            animator.SetTrigger("attack");
            canAttack = false;
            StartCoroutine(DelayFire());
        }
        float directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);
        if (directionVector > agent.stoppingDistance && enemyScript.getCanMoveDuringAttack())
        {
            // Target within attack range
            enemyScript.setStateMachine(State.ChaseTarget);
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
