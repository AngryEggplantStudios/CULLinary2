using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ExplodingMonsterBehaviour : MonoBehaviour
{
    [Header("Monster Behavior Variables")]
    [SerializeField] private float idleTimer;
    [SerializeField] private float wanderRadius;
    [SerializeField] private float wanderTimer;

    [Tooltip("The minimum distance to wander about. Needed because of the stopping distance being large makes the enemy only wander a bit before stopping.")]
    [SerializeField] private float minDist;
    [SerializeField] private float timeBetweenAttacks;

    [Header("Test")]
    [SerializeField] public bool lineTest = false;

    [Tooltip("The speed to chase the player.")]
    [SerializeField] private float chasingSpeed;
    [SerializeField] private float chasingAccel;
    private bool chasingCouroutineStarted = false;
    private bool canStartExploding = false;
    private float goingBackToStartTimer;
    private Vector3 roamPosition;
    public bool canAttack = true;
    public Animator animator;
    public NavMeshAgent navMeshAgent;
    private Renderer rend;
    private Color[] originalColors;
    private Texture[] originalTextures;
    private Color onDamageColor = Color.white;
    private Texture[] texturesForFlash;

    private float timer;
    private Vector3 startingPosition;
    //The minimum distance to stop in front of the player. Has to be equal to Stopping distance. Cannot use stopping distance directly else navmesh agent will keep bumping into player/
    public float reachedPositionDistance;
    public MonsterScript monsterScript;
    public LineRenderer lineRenderer;
    private bool mustStartExploding = false;
    private bool canSetDestination = true;
    private NavMeshPath path;

    private void Start()
	{
        chasingCouroutineStarted = false;
        navMeshAgent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
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
        goingBackToStartTimer = 0;
        rend = GetComponentInChildren<SkinnedMeshRenderer>();

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

    public virtual void EnemyIdle()
    {
        monsterScript.checkIfDead();
        animator.SetBool("isMoving", false);
        timer += Time.deltaTime;
        if (timer >= idleTimer)
        {
            Vector3 newPos = RandomNavSphere(startingPosition, wanderRadius, -1, minDist);
            navMeshAgent.SetDestination(newPos);
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

    public void EnemyChase(float stopChaseDistance, Vector3 playerPosition)
    {
        // Set speed here to be fast boi
        navMeshAgent.speed = chasingSpeed;
        navMeshAgent.acceleration = chasingAccel;
        // if timer coroutine not started yet start here
        if (chasingCouroutineStarted != true)
		{
            chasingCouroutineStarted = true;
            StartCoroutine(ChasingTimerBeforeStartExploding());
		}
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
        playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);

        // reachedPositionDistance or timercoroutine has completed. Make sure stopping distance is close enuf and stop chasing, stay in place and start exploding
        if (directionVector <= reachedPositionDistance && canStartExploding || mustStartExploding)
        {
            slowlyRotateToLookAt(playerPositionWithoutYOffset);
            navMeshAgent.SetDestination(gameObject.transform.position);
            // Target within attack range
            monsterScript.SetStateMachine(MonsterState.AttackTarget);
            // Add new state to attack player
        }
        else
        {
            if (canSetDestination)
			{
                NavMesh.CalculatePath(transform.position, playerPositionWithoutYOffset, NavMesh.AllAreas, path);
                canSetDestination = false;
                StartCoroutine(DelayFindingPlayer());
                navMeshAgent.SetPath(path);
            }
        }
    }

    private IEnumerator DelayFindingPlayer()
	{
        yield return new WaitForSeconds(0.25f);
        canSetDestination = true;
	}

    // Chases for n seconds before start exploding attack
    private IEnumerator ChasingTimerBeforeStartExploding()
	{
        yield return new WaitForSeconds(3.0f);
        canStartExploding = true;
        yield return new WaitForSeconds(2.0f);
        mustStartExploding = true;
    }

    //explode here
    public void EnemyAttackPlayer(Vector3 playerPosition, bool ableToMove)
    {
        Vector3 playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        slowlyRotateToLookAt(playerPositionWithoutYOffset);

        if (canAttack == true)
        {
            canAttack = false;
            // start exploding delay of 1.5 s
            StartCoroutine(DelayAttack(playerPositionWithoutYOffset));
        }
    }

    public IEnumerator DelayAttack(Vector3 playerPositionWithoutYOffset)
    {
        originalColors = monsterScript.GetOriginalColors();
        originalTextures = monsterScript.GetOriginalTextures();
        texturesForFlash = monsterScript.GetTexturesForFlash();
        StartCoroutine(FlashOnExplosion());
        animator.SetBool("isMoving", false);
        // Start Coroutine to flash indicating explosions
        yield return new WaitForSeconds(0.001f);
        slowlyRotateToLookAt(playerPositionWithoutYOffset);
        animator.ResetTrigger("attack");
        animator.SetTrigger("attack");
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


    private IEnumerator DelayFire()
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

    private void OnDestroy()
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

    private IEnumerator FlashOnExplosion()
    {
        while (true)
		{
            for (var i = 0; i < rend.materials.Length; i++)
            {
                Texture flashTex = null;
                if (texturesForFlash != null && i < texturesForFlash.Length)
                {
                    flashTex = texturesForFlash[i];
                }
                rend.materials[i].SetTexture("_BaseMap", flashTex);
                rend.materials[i].color = onDamageColor;
            }

            float duration = 0.1f;
            while (duration > 0)
            {
                duration -= Time.deltaTime;
                yield return null;
            }

            for (var i = 0; i < rend.materials.Length; i++)
            {
                rend.materials[i].SetTexture("_BaseMap", originalTextures[i]);
                rend.materials[i].color = originalColors[i];
            }

            duration = 0.1f;
            while (duration > 0)
            {
                duration -= Time.deltaTime;
                yield return null;
            }
        }

   
    }

}
