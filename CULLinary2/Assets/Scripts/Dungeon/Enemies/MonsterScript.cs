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
    [SerializeField] private int additionalSpawning = 0;
    [HideInInspector] public GameObject spawner;

    [Header("UI Prefabs")]
    [SerializeField] public GameObject hpBarPrefab;
    [SerializeField] private GameObject damageCounterPrefab;
    [SerializeField] protected GameObject enemyAlertPrefab;
    [SerializeField] private GameObject canvasDisplay;

    [Header("Particle Prefabs")]
    [SerializeField] private GameObject onDeathParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSourceDamage;
    [SerializeField] private AudioClip[] stabSounds;
    [SerializeField] protected AudioSource audioSourceAttack;
    [SerializeField] protected AudioClip alertSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Attacks")]
    [SerializeField] private MonsterAttack primaryEnemyAttack;

    [Header("Flash On Damage")]
    [SerializeField] private Color onDamageColor = Color.white;
    [SerializeField] private Texture[] texturesForFlash;

    [Header("Fade out on Death")]
    [SerializeField] private Material[] transparentMaterials;

    // Variables
    // private EnemyDetection globalEnemyDetection;
    private MonsterName monsterName;
    private bool hasAssignedName = false;
    public MonsterState currentState;
    private float currentHealth;
    public Transform playerTransform;
    private Camera playerCamera;
    private Image hpBarFull;
    protected List<GameObject> uiElements = new List<GameObject>();
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
    private bool wasAggressive = false;
    private GameObject hpBarUi;
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

        /* globalEnemyDetection = EnemyDetection.Instance.GetComponentInChildren<EnemyDetection>();
        globalEnemyDetection.AddToListOfEnemies(this); */
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
        RemoveUiElements();
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform; //Temp fix
        }

        if (checkIfDead()) return;

        bool aggressive = false;

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
                aggressive = true;
                onEnemyChase?.Invoke(stopChase, playerTransform.position);
                break;

            case MonsterState.AttackTarget:
                aggressive = true;
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

        // UI
        if (aggressive)
        {
            if (!wasAggressive)
            {
                wasAggressive = aggressive;
                if (monsterName != MonsterName.Bread)
                {
                    EnemyAggressionManager.Instance.Add(1);
                }
                foreach (GameObject ui in uiElements)
                {
                    if (ui != null)
                    {
                        ui.SetActive(true);
                    }
                    else
                    {
                        uiElements.Remove(null);
                    }
                }
            }
            else if (CheckIfPlayerGoingToMoveAway())
            {
                RemoveUiElements();
            }

            Vector2 screenPos = playerCamera.WorldToScreenPoint(transform.position);
            if (screenPos != Vector2.zero)
            {
                foreach (GameObject ui in uiElements)
                {
                    if (ui != null)
                    {
                        ui.SetActive(true);
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
        else if (wasAggressive)
        {
            wasAggressive = aggressive;
            if (monsterName != MonsterName.Bread)
			{
                EnemyAggressionManager.Instance.Add(-1);
            }
            RemoveUiElements();
        }
    }

    public void DeactivateAggression()
    {
        wasAggressive = false;
        EnemyAggressionManager.Instance.Add(-1);
        RemoveUiElements();
    }

    public bool checkIfDead()
    {
        if (this.currentHealth <= 0)
        {
            if (!deathCoroutine)
            {
                //globalEnemyDetection.RemoveFromListOfEnemies(this);
                // need this due to buggy triggers for death animation: Attack animation may be triggered immediately after death
                deathCoroutine = true;
                DieAnimation();
            }

            return true;
        }
        return false;
    }

    public void SetStateMachine(MonsterState newState)
    {
        currentState = newState;
    }

    // Checks if the player is going to be teleported away in a moment
    private bool CheckIfPlayerGoingToMoveAway()
    {
        return PlayerHealth.instance.WasDeathCalled() ||
            (GameTimer.instance != null && GameTimer.instance.WasEndOfDayCalled());
    }

    private void RemoveUiElements()
    {
        foreach (GameObject ui in uiElements)
        {
            if (ui != null)
            {
                ui.SetActive(false);
            }
            else
            {
                uiElements.Remove(null);
            }
        }
    }

    private void SetupHpBar()
    {
        GameObject hpBarObject = Instantiate(hpBarPrefab);
        hpBarFull = hpBarObject.transform.Find("hpBar_full").gameObject.GetComponent<Image>();
        uiElements.Add(hpBarObject);
        hpBarUi = hpBarObject;
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

    protected void SetupUI(GameObject ui)
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

    public virtual void Alert()
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

    public void DieAnimation()
    {
        // Reset all triggers first to prevent interference of other animation states before deathj
        animator.ResetTrigger("attack");
        animator.SetBool("isMoving", false);
        animator.SetTrigger("death");

        // Disable collider to prevent spam hitting damage
        monsterCollider.enabled = false;

        // Death audio
        audioSourceAttack.clip = deathSound;
        audioSourceAttack.volume = 0.7f;
        audioSourceAttack.pitch = Random.Range(1f, 2f);
        audioSourceAttack.Play();

        // Handle aggression
        if (wasAggressive)
        {
            wasAggressive = false;
            EnemyAggressionManager.Instance.Add(-1);
        }
    }

    public void Die()
    {
        //Update Stats
        PlayerManager.instance.enemiesCulled++;
        EcosystemManager.DecreasePopulation(monsterName, 1);

        // update spawn cap for the spawner it came from
        if (spawner)
        {
            spawner.GetComponent<MonsterSpawn>().DecrementSpawnCap(1);
        }
        else
        {
            if (MonsterBaseSpawning.instance != null)
            {
                MonsterBaseSpawning.instance.UpdateNumOfAliveMonsters(monsterName, -1);
            }

            // Debug.Log("no spawner");
        }

        if (TryGetComponent<MiniBoss>(out MiniBoss miniBoss))
        {
            miniBoss.Die();
        }
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
        if (TryGetComponent<MiniBoss>(out MiniBoss miniBoss))
        {
            miniBoss.DropLoot();
            return;
        }

        Instantiate(lootDropped, transform.position, Quaternion.identity);
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

    public void destroyHpBarUi()
    {
        uiElements.Remove(hpBarUi);
        Destroy(hpBarUi);
    }

    public void monsterDeathAnimation()
    {
        Instantiate(onDeathParticles, transform.position, transform.rotation);
        DropLoot();
        foreach (GameObject uiElement in uiElements)
        {
            Destroy(uiElement);
        }
        // Cheat as Tomato has seperate script
        if (TryGetComponent(out MonsterBehavior getMonsterBehav))
        {
            getMonsterBehav.DestroyObjectEvents();
        }
        else if (TryGetComponent(out RushEnemyScript rushEnemy))
        {
            rushEnemy.DestroyObjectEvents();
        }
        else
        {
            gameObject.GetComponent<ExplodingMonsterBehaviour>().DestroyObjectEvents();
        }
        Destroy(gameObject);
    }

    public void SetMiniBossValues(int health, float miniBossDistTriggered, float miniBossStopChase, float damageMultiplier)
    {
        currentHealth = health;
        monsterHealth = health;
        distanceTriggered = miniBossDistTriggered;
        stopChase = miniBossStopChase;
        primaryEnemyAttack.attackDamage = Mathf.RoundToInt(primaryEnemyAttack.attackDamage * damageMultiplier);
    }

    public int GetAdditionalSpawning()
    {
        return additionalSpawning;
    }

    public Color[] GetOriginalColors()
    {
        return originalColors;
    }

    public Texture[] GetOriginalTextures()
    {
        return originalTextures;
    }

    public Texture[] GetTexturesForFlash()
    {
        return texturesForFlash;
    }

    public float GetStopChase()
    {
        return this.stopChase;
    }

    public GameObject GetLoot()
    {
        return lootDropped;
    }

    public Vector3 GetPlayerPosition()
    {
        return this.playerTransform.position;
    }

    public bool IsFarEnoughFromPlayer(Vector3 playerPosition, float globalRadius)
    {
        Vector3 dist = playerPosition - transform.position;
        float sqredMagnitude = Vector3.SqrMagnitude(dist);
        return sqredMagnitude >= globalRadius * globalRadius;
    }

    public void SetActiveMonster(bool toActivate)
    {
        this.gameObject.SetActive(toActivate);
    }

    public float getStopChaseDistance()
    {
        return this.stopChase;
    }
}
