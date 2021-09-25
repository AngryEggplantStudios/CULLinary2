using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterScript : Monster
{

    [Header("Monster Variables")]
    [SerializeField] private float monsterHealth;
    [SerializeField] private float distanceTriggered;
    [SerializeField] private float stopChase;
    [SerializeField] private GameObject lootDropped;
    [HideInInspector] public GameObject spawner;

    [Header("UI Prefabs")]
    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] private GameObject damageCounterPrefab;
    [SerializeField] private GameObject enemyAlertPrefab;
    [SerializeField] private GameObject canvasDisplay;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSourceDamage;
    [SerializeField] private AudioClip[] stabSounds;
    [SerializeField] private AudioSource audioSourceAttack;
    [SerializeField] private AudioClip alertSound;
    [SerializeField] private AudioClip attackSound;

    [Header("Attacks")]
    [SerializeField] private MonsterAttack primaryEnemyAttack;

    // Variables
    private MonsterName monsterName;
    private bool hasAssignedName = false;
    private MonsterState currentState;
    private float currentHealth;
    private Transform playerTransform;
    private Camera playerCamera;
    private Image hpBarFull;
    private List<GameObject> uiElements = new List<GameObject>();
    private List<GameObject> damageUiElements = new List<GameObject>();
    private bool canMoveDuringAttack = true;
    private Renderer rend;
    private Color[] originalColors;
    private Color onDamageColor = Color.white;
    private Animator animator;
    // Store a reference to final damage counter when death
    private GameObject damageCounter;
    // Store refference to collider so can disable when death
    private Collider monsterCollider;

    // Events & Delegates

    public delegate void EnemyIdleDelegate();

    public delegate void EnemyRoamingDelegate();

    public delegate void EnemyChasePlayerDelegate(float stopChaseDistance, Vector3 playerPosition);

    public delegate void EnemyAttackPlayerDelegate(Vector3 playerPosition, bool ableToMove);

    public delegate void EnemyReturnDelegate();

    public event EnemyIdleDelegate onEnemyIdle;

    public event EnemyRoamingDelegate onEnemyRoaming;

    public event EnemyChasePlayerDelegate onEnemyChase;

    public event EnemyAttackPlayerDelegate onEnemyAttack;

    public event EnemyReturnDelegate onEnemyReturn;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        monsterCollider = GetComponent<Collider>();
        currentHealth = monsterHealth;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; //Temp fix
        playerCamera = playerTransform.GetComponentInChildren<Camera>();
        if (GameCanvas.instance != null)
        {
            canvasDisplay = GameCanvas.instance.gameObject;
        }
        canvasDisplay = GameObject.FindGameObjectWithTag("GameCanvas");
        GetMonsterName();
    }

    public MonsterName GetMonsterName()
    {
        if (!hasAssignedName)
        {
            foreach (MonsterName currName in MonsterName.GetValues(typeof(MonsterName)))
            {
                string currNameString = MonsterName.GetName(typeof(MonsterName), currName).ToLower();
                if (name.ToLower().Contains(currNameString))
                {
                    monsterName = currName;
                    hasAssignedName = true;
                    break;
                }
            }
        }
        return monsterName;
    }

    private void Start()
    {
        SetupHpBar();
        SetupFlash();
    }

    private void Update()
    {
        if (playerTransform == null)
		{
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform; //Temp fix
        }

        switch (currentState)
        {
            default:
            case MonsterState.Idle:
                FindTarget();
                onEnemyIdle?.Invoke();
                break;
            case MonsterState.Roaming:
                FindTarget();
                onEnemyRoaming?.Invoke();
                break;
            case MonsterState.ChaseTarget:
                onEnemyChase?.Invoke(stopChase, playerTransform.position);
                break;
            case MonsterState.AttackTarget:
                // Hack to ensure attack trigger isn't triggered
                if (this.currentHealth > 0)
                {
                    onEnemyAttack?.Invoke(playerTransform.position, canMoveDuringAttack);
                }
                break;
            case MonsterState.GoingBackToStart:
                onEnemyReturn?.Invoke();
                break;
        }

        //Need to find a better way to update this?

        Vector2 screenPos = playerCamera.WorldToScreenPoint(transform.position);
        if (screenPos != Vector2.zero)
        {
            foreach (GameObject ui in uiElements)
            {
                if (ui != null)
                {
                    ui.transform.position = screenPos;
                }
                else
                {
                    uiElements.Remove(null);
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
                    uiElements.Remove(null);
                }
            }
        }
    }

    public void SetStateMachine(MonsterState newState)
    {
        currentState = newState;
    }

    private void SetupHpBar()
    {
        GameObject hpBarObject = Instantiate(hpBarPrefab);
        hpBarFull = hpBarObject.transform.Find("hpBar_full").gameObject.GetComponent<Image>();
        uiElements.Add(hpBarObject);
        SetupUI(hpBarObject);
    }

    private void SetupFlash()
    {
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        originalColors = new Color[rend.materials.Length];
        for (int i = 0; i < rend.materials.Length; i++)
        {
            originalColors[i] = rend.materials[i].color;
        }
    }

    private void SetupUI(GameObject ui)
    {
        ui.transform.SetParent(canvasDisplay.transform);
        ui.transform.position = playerCamera.WorldToScreenPoint(transform.position);
    }

    public void FindTarget()
    {
        float dist = Vector3.Distance(playerTransform.position, transform.position);
        if (dist < distanceTriggered)
        {
            Alert();
        }
    }

    public void Alert()
    {
        currentState = MonsterState.ChaseTarget;
        GameObject enemyAlertObject = Instantiate(enemyAlertPrefab);
        SetupUI(enemyAlertObject);
        uiElements.Add(enemyAlertObject);
        audioSourceAttack.clip = alertSound;
        audioSourceAttack.Play();
    }

    public override void HandleHit(float damage)
    {
        if (currentState != MonsterState.AttackTarget)
        {
            Alert();
        }
        this.currentHealth -= damage;
        hpBarFull.fillAmount = currentHealth / monsterHealth;
        StartCoroutine(FlashOnDamage());
        SpawnDamageCounter(damage);
        audioSourceDamage.clip = stabSounds[Random.Range(0, stabSounds.Length)];
        audioSourceDamage.Play();
        if (this.currentHealth <= 0)
        {
            // Reset all triggers first to prevent interference of other animation states before deathj
            animator.ResetTrigger("attack");
            animator.SetBool("isMoving", false);
            animator.SetTrigger("death");
            // Disable collider to prevent spam hitting damage
            monsterCollider.enabled = false;
            Die();
        }
    }

    private void SpawnDamageCounter(float damage)
    {
        damageCounter = Instantiate(damageCounterPrefab);
        damageCounter.transform.GetComponentInChildren<Text>().text = damage.ToString();
        SetupUI(damageCounter);
        damageUiElements.Add(damageCounter);
    }

    public void Die()
    {
        EcosystemManager.DecreasePopulation(monsterName, 1);

        // update spawn cap for the spawner it came from
        if (spawner)
        {
            spawner.GetComponent<MonsterSpawn>().DecrementSpawnCap(1);
        }

        if (TryGetComponent<MiniBoss>(out MiniBoss miniBossScript))
        {
            miniBossScript.Die();
        }

        DungeonSpawnManager.CheckIfExtinct(monsterName);
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

    private void DropLoot()
    {
        Vector3 tempVectors = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Instantiate(lootDropped, tempVectors, Quaternion.identity);
    }

    public void attackPlayerStart()
    {
        canMoveDuringAttack = false;
        primaryEnemyAttack.attackPlayerStart();
    }

    public void attackPlayerDealDamage()
    {
        primaryEnemyAttack.attackPlayerDealDamage();
        audioSourceAttack.clip = attackSound;
        audioSourceAttack.Play();
    }

    public void attackPlayerEnd()
    {
        canMoveDuringAttack = true;
        primaryEnemyAttack.attackPlayerEnd();
    }

    public void destroyDamageNumbers()
    {
        //Death Animation too long destroy earlier
        Destroy(damageCounter);
    }

    public void monsterDeathAnimation()
    {
        DropLoot();
        foreach (GameObject uiElement in uiElements)
        {
            Destroy(uiElement);
        }
        Destroy(gameObject);
    }

    public void SetMiniBossValues(int health, float miniBossDistanceTriggered)
    {
        monsterHealth = health;
        distanceTriggered = miniBossDistanceTriggered;
    }
}
