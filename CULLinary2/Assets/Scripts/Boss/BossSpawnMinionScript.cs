using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossSpawnMinionScript : Monster
{
    public NavMeshAgent agent;

    private enum State
    {
        ChaseTarget,
        AttackTarget,
        ShootingTarget,
    }

    [SerializeField] private float maxHealth;
    [SerializeField] private float timeBetweenAttacks;

    private GameObject hpBar;
    private Image hpBarFull;

    [Header("UI Prefabs")]
    [SerializeField] private GameObject hpBar_prefab;
    [SerializeField] private GameObject damageCounter_prefab;
    [SerializeField] private GameObject enemyAlert_prefab;
    [SerializeField] private GameObject canvasDisplay;

    private List<GameObject> uiList = new List<GameObject>();
    private List<GameObject> damageUiElements = new List<GameObject>();

    [SerializeField] private float wanderRadius;

    [SerializeField] private AudioSource audioSourceDamage;
    [SerializeField] private AudioClip[] stabSounds;
    [SerializeField] private AudioSource audioSourceAttack;
    [SerializeField] private AudioClip alertSound;
    [SerializeField] private AudioClip attackSound;

    private float health;
    private Animator animator;
    private Camera cam;
    private State state;
    private Transform player;
    private MonsterAttack refScript;
    private bool canAttack = true;
    private Renderer rend;
    private Color[] originalColors;
    private Color onDamageColor = Color.white;
    private bool canMoveDuringAttack = true;
    private Collider monsterCollider;
    // Store a reference to final damage counter when death
    private GameObject damageCounter;
    private bool deathCoroutine = false;

    private void Awake()
    {
        state = State.ChaseTarget;
        health = maxHealth;
    }
    public void destroyDamageNumbers()
    {
        //Death Animation too long destroy earlier
        Destroy(damageCounter);
    }

    private void Start()
    {
        GameObject attackRadius = gameObject.transform.Find("AttackRadius").gameObject;
        refScript = attackRadius.GetComponent<MonsterAttack>();
        animator = GetComponentInChildren<Animator>();
        SetupFlash();
        if (GameCanvas.instance != null)
        {
            canvasDisplay = GameCanvas.instance.gameObject;
        }
        canvasDisplay = GameObject.FindGameObjectWithTag("GameCanvas");
        // Make sure player exists (finished loading) before running these
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cam = player.GetComponentInChildren<Camera>();
        if (GameCanvas.instance != null)
        {
            canvasDisplay = GameCanvas.instance.gameObject;
        }
        SetupHpBar();
        GameObject enemyAlertObject = Instantiate(enemyAlert_prefab);
        SetupUI(enemyAlertObject);
        uiList.Add(enemyAlertObject);
        audioSourceAttack.clip = alertSound;
        audioSourceAttack.Play();
        monsterCollider = GetComponent<Collider>();

    }

    private void SetupFlash()
    {
        rend = GetComponentInChildren<Renderer>();
        originalColors = new Color[rend.materials.Length];
        for (int i = 0; i < rend.materials.Length; i++)
        {
            originalColors[i] = rend.materials[i].color;
        }
    }

    private void SetupHpBar()
    {
        hpBar = Instantiate(hpBar_prefab);
        hpBarFull = hpBar.transform.Find("hpBar_full").gameObject.GetComponent<Image>();
        uiList.Add(hpBar);
        SetupUI(hpBar);
    }

    private void SetupUI(GameObject ui)
    {
        ui.transform.SetParent(canvasDisplay.transform);
        ui.transform.position = cam.WorldToScreenPoint(transform.position);
    }

    private void Update()
    {
        Vector3 playerPositionWithoutYOffset = new Vector3(player.position.x, transform.position.y, player.position.z);
        float directionVector;
        switch (state)
        {
            default:
            case State.ChaseTarget:
                checkIfDead();
                animator.SetBool("isMoving", true);
                directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);
                if (directionVector <= agent.stoppingDistance)
                {
                    transform.LookAt(playerPositionWithoutYOffset);
                    slowlyRotateToLookAt(playerPositionWithoutYOffset);
                    agent.SetDestination(gameObject.transform.position);

                    // Target within attack range
                    state = State.AttackTarget;
                    // Add new state to attack player
                }
                else
                {
                    agent.SetDestination(playerPositionWithoutYOffset);

                }
                break;
            case State.AttackTarget:
                checkIfDead();
                transform.LookAt(playerPositionWithoutYOffset);
                animator.SetBool("isMoving", false);
                animator.ResetTrigger("attack");
                if (canAttack == true)
                {
                    canAttack = false;
                    StartCoroutine(DelayAttack(playerPositionWithoutYOffset));
                }
                directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);
                if (directionVector > agent.stoppingDistance && canMoveDuringAttack)
                {
                    // Target within attack range
                    state = State.ChaseTarget;
                }
                break;
        }

        // Set UI to current position
        Vector2 screenPos = cam.WorldToScreenPoint(transform.position);
        if (screenPos != Vector2.zero)
        {
            foreach (GameObject ui in uiList)
            {
                if (ui != null)
                {
                    ui.transform.position = screenPos;
                }
                else
                {
                    uiList.Remove(null);
                }
            }
            foreach (GameObject ui in damageUiElements)
            {
                if (ui != null)
                {
                    ui.transform.position = screenPos;
                }
                else
                {
                    damageUiElements.Remove(null);
                }
            }
        }


    }

    private IEnumerator DelayAttack(Vector3 playerPositionWithoutYOffset)
    {
        yield return new WaitForSeconds(1);
        slowlyRotateToLookAt(playerPositionWithoutYOffset);
        animator.SetBool("isMoving", false);
        animator.ResetTrigger("attack");
        animator.SetTrigger("attack");
        StartCoroutine(DelayFire());
    }

    private void checkIfDead()
    {
        if (this.health < 0)
        {
            if (!deathCoroutine)
            {
                // need this due to buggy triggers for death animation: Attack animation may be triggered immediately after death
                Debug.Log("Calling death");
                deathCoroutine = true;
                DieAnimation();
            }
        }
    }

    private IEnumerator DelayFire()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;
    }


    public override void HandleHit(float damage)
    {
        this.health -= damage;
        hpBarFull.fillAmount = health / maxHealth;
        StartCoroutine(FlashOnDamage());
        SpawnDamageCounter(damage);
        audioSourceDamage.clip = stabSounds[Random.Range(0, stabSounds.Length)];
        audioSourceDamage.Play();

        if (this.health <= 0)
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("death");
            // Disable collider to prevent spam hitting damage
            monsterCollider.enabled = false;
        }
    }

    private void SpawnDamageCounter(float damage)
    {
        damageCounter = Instantiate(damageCounter_prefab);
        damageCounter.transform.GetComponentInChildren<Text>().text = damage.ToString();
        SetupUI(damageCounter);
        damageUiElements.Add(damageCounter);
    }

    public void DieAnimation()
    {
        animator.ResetTrigger("attack");
        animator.SetTrigger("death");
        // Disable collider to prevent spam hitting damage
        monsterCollider.enabled = false;
    }

    public void monsterDeathAnimation()
    {
        Debug.Log("Calling Death Trigger");
        foreach (GameObject uiElement in uiList)
        {
            Debug.Log("Removing Trigger");
            Destroy(uiElement);
        }
        foreach (GameObject uiElement in damageUiElements)
        {
            Destroy(uiElement);
        }
        Destroy(gameObject);
    }

    private IEnumerator FlashOnDamage()
    {
        for (var i = 0; i < rend.materials.Length; i++)
        {
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
            rend.materials[i].color = originalColors[i];
        }
    }


    public void attackPlayerStart()
    {
        canMoveDuringAttack = false;
        refScript.attackPlayerStart();
    }

    public void attackPlayerDealDamage()
    {
        refScript.attackPlayerDealDamage();
        audioSourceAttack.clip = attackSound;
        audioSourceAttack.Play();
    }


    public void attackPlayerEnd()
    {
        canMoveDuringAttack = true;
        refScript.attackPlayerEnd();
    }

    private void slowlyRotateToLookAt(Vector3 target)
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(Quaternion.LookRotation(target - transform.position).eulerAngles),
            Time.deltaTime * 3.0f);
    }
}