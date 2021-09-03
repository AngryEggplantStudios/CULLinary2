using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterScript : Enemy
{

    [Header("Monster Variables")]
    [SerializeField] private float monsterHealth;
    [SerializeField] private float distanceTriggered;
    [SerializeField] private float stopChase;
    [SerializeField] private GameObject lootDropped;

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
    [SerializeField] private EnemyAttack primaryEnemyAttack;

    // Variables
    private MonsterState currentState;
    private float currentHealth;
    private Transform playerTransform;
    private Camera playerCamera;
    private Image hpBarFull;
    private List<GameObject> uiElements = new List<GameObject>();
    private bool canMoveDuringAttack = true;
    private Renderer rend;
    private Color[] originalColors;
    private Color onDamageColor = Color.white;

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
        currentHealth = monsterHealth;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; //Temp fix
        playerCamera = playerTransform.GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        SetupHpBar();
        SetupFlash();
    }

    private void Update()
    {
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
                onEnemyAttack?.Invoke(playerTransform.position, canMoveDuringAttack);
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
        uiElements.Add(ui);
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
        SetupUI(Instantiate(enemyAlertPrefab));
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
            Die();
        }
    }

    private void SpawnDamageCounter(float damage)
    {
        GameObject damageCounter = Instantiate(damageCounterPrefab);
        damageCounter.transform.GetComponentInChildren<Text>().text = damage.ToString();
        SetupUI(damageCounter);
    }

    public void Die()
    {
        DropLoot();
        foreach (GameObject uiElement in uiElements)
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

    private void DropLoot()
    {
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


}
