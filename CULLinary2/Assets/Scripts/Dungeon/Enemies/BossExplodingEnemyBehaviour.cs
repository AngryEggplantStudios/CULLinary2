using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossExplodingEnemyBehaviour : BossRushEnemyScript
{
    [Tooltip("The speed to chase the player.")]
    [SerializeField] private float chasingSpeed;
    [SerializeField] private float chasingAccel;
 
    private Renderer rend;
    private Color[] originalColors;
    private Texture[] originalTextures;
    private Color onDamageColor = Color.white;
    private Texture[] texturesForFlash;
    private bool chasingCouroutineStarted = false;
    private bool canStartExploding = false;
    private bool mustStartExploding = false;
    private bool canSetDestination = true;
    private NavMeshPath path;

	protected override void childClassPreStartFunctions()
	{
        path = new NavMeshPath();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
    }
    protected override void EnemyChase(float stopChaseDistance, Vector3 playerPosition)
    {
        // Set speed here to be fast boi
        agent.speed = chasingSpeed;
        agent.acceleration = chasingAccel;
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
            debugLine.SetPositions(points);
        }
        playerPositionWithoutYOffset = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);

        // reachedPositionDistance or timercoroutine has completed. Make sure stopping distance is close enuf and stop chasing, stay in place and start exploding
        if (directionVector <= reachedPositionDistance && canStartExploding || mustStartExploding)
        {
            slowlyRotateToLookAt(playerPositionWithoutYOffset);
            agent.SetDestination(gameObject.transform.position);
            // Target within attack range
            enemyScript.SetStateMachine(MonsterState.AttackTarget);
            // Add new state to attack player
        }
        else
        {
            if (canSetDestination)
            {
                Debug.Log(agent + "TOmatoa");
                NavMesh.CalculatePath(transform.position, playerPositionWithoutYOffset, NavMesh.AllAreas, path);
                canSetDestination = false;
                StartCoroutine(DelayFindingPlayer());
                agent.SetPath(path);
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
    protected override void EnemyAttackPlayer(Vector3 playerPosition, bool ableToMove)
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

    protected IEnumerator DelayAttack(Vector3 playerPositionWithoutYOffset)
    {
        originalColors = enemyScript.GetOriginalColors();
        originalTextures = enemyScript.GetOriginalTextures();
        texturesForFlash = enemyScript.GetTexturesForFlash();
        StartCoroutine(FlashOnExplosion());
        animator.SetBool("isMoving", false);
        // Start Coroutine to flash indicating explosions
        yield return new WaitForSeconds(0.001f);
        slowlyRotateToLookAt(playerPositionWithoutYOffset);
        animator.ResetTrigger("attack");
        animator.SetTrigger("attack");
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
