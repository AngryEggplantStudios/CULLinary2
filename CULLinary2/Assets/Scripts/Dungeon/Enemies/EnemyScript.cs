using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyScript : Enemy
{
    public NavMeshAgent agent;



    [SerializeField] private float maxHealth;
    [SerializeField] private float distanceTriggered;
    [SerializeField] private float stopChase;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float collideDamage;

    [SerializeField] private GameObject hpBar_prefab;
    private GameObject hpBar;
    private Image hpBarFull;

    [SerializeField] private GameObject damageCounter_prefab;
    [SerializeField] private GameObject enemyAlert_prefab;
    private List<GameObject> uiList = new List<GameObject>();

    [System.Serializable] private class LootTuple
    {
        [SerializeField] private GameObject loot;
        [SerializeField] private int ratio;

        public LootTuple(GameObject loot, int ratio)
        {
            this.loot = loot;
            this.ratio = ratio;
        }

        public GameObject GetLoot()
        {
            return loot;
        }

        public int GetRatio()
        {
            return ratio;
        }
    }

    [SerializeField] private LootTuple[] lootTuples;
    
    [SerializeField] private AudioSource audioSourceDamage;
    [SerializeField] private AudioClip[] stabSounds;
    [SerializeField] private AudioSource audioSourceAttack;
    [SerializeField] private AudioClip alertSound;
    [SerializeField] private AudioClip attackSound;

    private Vector3 startingPosition;
    private Vector3 roamPosition;
    private float timer;
    private float goingBackToStartTimer;
    private float dist;
    private float health;
    private Animator animator;
    private Camera cam;
    private State state;
    private Transform player;
    private GameObject lootDropped;
    private EnemyAttack refScript;
    private bool canAttack = true;
    private Renderer rend;
    private Color[] originalColors;
    private Color onDamageColor = Color.white;
    private bool canMoveDuringAttack = true;

    public delegate void EnemyIdleDelegate();

    public delegate void EnemyRoamingDelegate();

    public delegate void EnemyChasePlayerDelegate(Vector3 playerPosition);

    public event EnemyIdleDelegate onEnemyIdle;

    public event EnemyRoamingDelegate onEnemyRoaming;

    public event EnemyChasePlayerDelegate onEnemyChase;

    private void Awake()
    {
        state = State.Idle;
        health = maxHealth;
    }

    private void Start()
    {
        startingPosition = transform.position;
        GameObject attackRadius = gameObject.transform.Find("AttackRadius").gameObject;
        refScript = attackRadius.GetComponent <EnemyAttack>();
        animator = GetComponentInChildren<Animator>();
        goingBackToStartTimer = 0;
        SetupFlash();
        SetupLoot();

        // Make sure player exists (finished loading) before running these
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cam = player.GetComponentInChildren<Camera>();
        SetupHpBar();
    }

    public Animator getAnimator()
	{
        return this.animator;
	}

    public float getStopChaseDistance()
    {
        return this.stopChase;
    }
    public NavMeshAgent getNavMeshAgent()
    {
        return agent;
    }

    public Transform getPlayerReference()
	{
        return this.player;
	}

    public void setStateMachine(State newState)
	{
        state = newState;
	}

    private void SetupFlash()
    {
        rend = GetComponentInChildren<Renderer>();
        originalColors = new Color[rend.materials.Length];
        for (int i = 0; i < rend.materials.Length; i++) {
            originalColors[i] = rend.materials[i].color;
        }
    }

    private void SetupLoot()
    {
        int currentWeight = 0;
        Dictionary<GameObject, int> dropTuples = new Dictionary<GameObject, int>();
        foreach (var loot in lootTuples)
        {
            currentWeight += loot.GetRatio();
            dropTuples.Add(loot.GetLoot(), currentWeight);
        }
        int randomWeight = Random.Range(1, currentWeight + 1);
        foreach (var tpl in dropTuples)
        {
            if (randomWeight <= tpl.Value)
            {
                lootDropped = tpl.Key;
                return;
            }
        }
        lootDropped = lootTuples[0].GetLoot();
        return;
    }

    private void SetupHpBar()
    {
        hpBar = Instantiate(hpBar_prefab);
        hpBarFull = hpBar.transform.Find("hpBar_full").gameObject.GetComponent<Image>();
        SetupUI(hpBar);
    }

    private void SetupUI(GameObject ui)
    {
        //To Figure out a proper UI
        //ui.transform.SetParent(GameObject.FindObjectOfType<InventoryUI>().transform);
        EnemyManager.attachToUIForHp(ui);
        ui.transform.position = cam.WorldToScreenPoint(transform.position);
        uiList.Add(ui);
    }

    private void Update()
    {
        Vector3 playerPositionWithoutYOffset = new Vector3(player.position.x, transform.position.y, player.position.z);
        float directionVector;
        switch (state)
        {
            default:
            case State.Idle:
                onEnemyIdle?.Invoke();
                break;
            case State.Roaming:
                onEnemyRoaming?.Invoke();
                break;
            case State.ChaseTarget:
                onEnemyChase?.Invoke(playerPositionWithoutYOffset);
                break;
            case State.AttackTarget:
                transform.LookAt(playerPositionWithoutYOffset);
                animator.SetBool("isMoving", false);
                animator.ResetTrigger("attack");
                if (canAttack == true)
                {
                    animator.SetTrigger("attack");
                    canAttack = false;
                    StartCoroutine(DelayFire());
                }
                directionVector = Vector3.Distance(transform.position, playerPositionWithoutYOffset);
                if (directionVector > agent.stoppingDistance && canMoveDuringAttack)
                {
                    // Target within attack range
                    state = State.ChaseTarget;
                }

                break;
            case State.GoingBackToStart:
                goingBackToStartTimer += Time.deltaTime;
                animator.SetBool("isMoving", true);
                float reachedPositionDistance = 1.0f;
                transform.LookAt(startingPosition);
                agent.SetDestination(startingPosition);
                if (Vector3.Distance(transform.position, startingPosition) <= reachedPositionDistance || goingBackToStartTimer > 4.0f)
                {
                    // Reached Start Position
                    animator.SetBool("isMoving", false);
                    state = State.Idle;
                    goingBackToStartTimer = 0;
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
        }

        
    }

    private IEnumerator DelayFire()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;
    }

    private void FindTarget()
    {
        dist = Vector3.Distance(player.position, transform.position);
        if (dist <= distanceTriggered)
        {
            Alert();
        }
    }

    public void Alert()
    {
        timer = 0;
        state = State.ChaseTarget;
        
        SetupUI(Instantiate(enemyAlert_prefab));
        audioSourceAttack.clip = alertSound;
        audioSourceAttack.Play();
    }

    public override void HandleHit(float damage)
    {
      if (state != State.AttackTarget)
      {
          Alert();
      }
      this.health -= damage;
      hpBarFull.fillAmount = health/maxHealth;
      StartCoroutine(FlashOnDamage());
      SpawnDamageCounter(damage);
      audioSourceDamage.clip = stabSounds[Random.Range(0, stabSounds.Length)];
      audioSourceDamage.Play();

      if (this.health <= 0)
      {
          Die();
      }
    }

    private void SpawnDamageCounter(float damage)
    {
        GameObject damageCounter = Instantiate(damageCounter_prefab);
        damageCounter.transform.GetComponentInChildren<Text>().text = damage.ToString();
        SetupUI(damageCounter);
    }

    private void Die()
    {
/*        if (PlayerManager.instance != null)
        {
            PlayerManager.noOfMobsCulled++;
        }*/
        DropLoot();
        Destroy(hpBar);
        Destroy(gameObject);
    }

    private IEnumerator FlashOnDamage()
    {
        for (var i = 0; i < rend.materials.Length; i++) {
            rend.materials[i].color = onDamageColor;
        }

        float duration = 0.1f;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }

        for (var i = 0; i < rend.materials.Length; i++) {
            rend.materials[i].color = originalColors[i];
        }
    }

    private void DropLoot()
    {
        Instantiate(lootDropped, transform.position, Quaternion.identity);
    }

    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector2 randPos = Random.insideUnitCircle * dist;
        Vector3 randDirection = new Vector3(randPos.x, transform.position.y, randPos.y);
        while ((randDirection - origin).magnitude < 5.0f)
        {
            randPos = Random.insideUnitCircle * dist;
            randDirection = new Vector3(randPos.x, transform.position.y, randPos.y);
        }
        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
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
}