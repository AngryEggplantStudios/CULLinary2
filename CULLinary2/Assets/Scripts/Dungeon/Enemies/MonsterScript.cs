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
    [SerializeField] private int additionalSpawningNumbers = 0;
    [HideInInspector] public GameObject spawner;

    [Header("UI Prefabs")]
    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] private GameObject damageCounterPrefab;
    [SerializeField] private GameObject enemyAlertPrefab;
    [SerializeField] private GameObject canvasDisplay;

    [Header("Particle Prefabs")]
    [SerializeField] private GameObject onDeathParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSourceDamage;
    [SerializeField] private AudioClip[] stabSounds;
    [SerializeField] private AudioSource audioSourceAttack;
    [SerializeField] private AudioClip alertSound;
    [SerializeField] private AudioClip attackSound;

    [Header("Attacks")]
    [SerializeField] private MonsterAttack primaryEnemyAttack;

    [Header("Flash On Damage")]
    [SerializeField] private Color onDamageColor = Color.white;
    [SerializeField] private Texture[] texturesForFlash;

    [Header("Fade out on Death")]
    [SerializeField] private Material[] transparentMaterials;

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
    private Texture[] originalTextures;
    private Animator animator;
    // Store a reference to final damage counter when death
    private GameObject damageCounter;
    // Store refference to collider so can disable when death
    private Collider monsterCollider;
    private bool deathCoroutine = false;
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
                checkIfDead();
                FindTarget();
                onEnemyIdle?.Invoke();
                break;
            case MonsterState.Roaming:
                checkIfDead();
                FindTarget();
                onEnemyRoaming?.Invoke();
                break;
            case MonsterState.ChaseTarget:
                checkIfDead();
                onEnemyChase?.Invoke(stopChase, playerTransform.position);
                break;
            case MonsterState.AttackTarget:
                checkIfDead();
                // Hack to ensure attack trigger isn't triggered
                if (this.currentHealth > 0)
                {
                    onEnemyAttack?.Invoke(playerTransform.position, canMoveDuringAttack);
                }
                break;
            case MonsterState.GoingBackToStart:
                checkIfDead();
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

    public void checkIfDead()
    {
        if (this.currentHealth <= 0)
        {
            if (!deathCoroutine)
            {
                // need this due to buggy triggers for death animation: Attack animation may be triggered immediately after death
                deathCoroutine = true;
                DieAnimation();
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
        originalTextures = new Texture[rend.materials.Length];
        for (int i = 0; i < rend.materials.Length; i++)
        {
            originalColors[i] = rend.materials[i].color;
            originalTextures[i] = rend.materials[i].GetTexture("_BaseMap");
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
            DieAnimation();
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

    private void DieAnimation()
    {
        // Reset all triggers first to prevent interference of other animation states before deathj
        animator.ResetTrigger("attack");
        animator.SetBool("isMoving", false);
        animator.SetTrigger("death");
        // Disable collider to prevent spam hitting damage
        monsterCollider.enabled = false;
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
    }

    private IEnumerator FadeOut(float duration)
    {
        float elapsed = 0;
        Color clearWhite = new Color(1, 1, 1, 0);

        if (rend.materials.Length == transparentMaterials.Length)
        {
            rend.materials = transparentMaterials;
        }
        else
        {
            Debug.LogWarning("Death fade out has the wrong number of transparent materials assigned");
        }

        while (elapsed < duration)
        {
            for (var i = 0; i < rend.materials.Length; i++)
            {
                rend.materials[i].color = Color.Lerp(originalColors[i], clearWhite, elapsed / duration);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        for (var i = 0; i < rend.materials.Length; i++)
        {
            rend.materials[i].color = Color.clear;
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
        Instantiate(onDeathParticles, transform.position, transform.rotation);
        DropLoot();
        foreach (GameObject uiElement in uiElements)
        {
            Destroy(uiElement);
        }
        Destroy(gameObject);
    }

    public void SetMiniBossValues(int health, float miniBossDistTriggered, float miniBossStopChase)
    {
        currentHealth = health;
        monsterHealth = health;
        distanceTriggered = miniBossDistTriggered;
        stopChase = miniBossStopChase;
    }

    public int GetAdditionalSpawningNumber()
    {
        return additionalSpawningNumbers;
    }
}
