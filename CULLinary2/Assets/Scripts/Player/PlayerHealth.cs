using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : SingletonGeneric<PlayerHealth>
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject canvasDisplay;
    [SerializeField] private AudioSource audioSource;

    [Header("UI References")]
    [SerializeField] private GameObject damageCounter_prefab;
    [SerializeField] private Image healthBar;
    [SerializeField] private Animator healthBarAnimator;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Renderer rend;
    [SerializeField] private GameObject healIcon;

    [Header("Variables")]
    [SerializeField] private float invincibilityDurationSeconds;
    [SerializeField] private float thresholdHealth = 0.25f;

    [Header("Drowning Event")]
    [SerializeField] private float minimumHeightToStartDrowning;
    [SerializeField] private float drowningDamage;
    [SerializeField] private GameObject drowningAlert_prefab;

    private bool isInvincible = false;
    private bool isDrowningActivated = false;
    // Check if player is close to campfire
    private bool isCloseToCampfire = false;
    private Color[] originalColors;
    private Color onDamageColor = Color.white;
    private float invincibilityDeltaTime = 0.025f;
    private Animator animator;
    private bool deathIsCalled = false;
    private bool isInvincibleByBuff = false;
    private bool isInvincibleByDash = false;
    private bool isClownActive = false;
    private List<GameObject> playerDamagedNumbers = new List<GameObject>();

    public override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        SetupIFrame();
    }

    private void Start()
    {
        float currentHealth = PlayerManager.instance ? PlayerManager.instance.currentHealth : 200f;
        float maxHealth = PlayerManager.instance ? PlayerManager.instance.maxHealth : 200f;
        DisplayOnUI(currentHealth, maxHealth);
    }

    private void DisplayOnUI(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
        healthText.text = Mathf.CeilToInt(currentHealth) + "/" + Mathf.CeilToInt(maxHealth);
    }

    private void Update()
    {
        // Check if height of player here as less references needed than checking height in player locomotion;
        if (transform.position.y < minimumHeightToStartDrowning && !isDrowningActivated && PlayerManager.instance.currentHealth > 0)
        {
            /*
            //Drowning animation
            isDrowningActivated = true;
            StartCoroutine(StartDrowning()); 
            */
            HandleHit(PlayerManager.instance.maxHealth * 0.25f, true);
        }

        //Flash health bar if below 25%
        float currentHealthAsPercentage = PlayerManager.instance.currentHealth / PlayerManager.instance.maxHealth;
        healthBarAnimator.SetBool("flashing", currentHealthAsPercentage < thresholdHealth);

        //Check if player is dead
        if (PlayerManager.instance.currentHealth <= 0f && !deathIsCalled)
        {
            BuffManager.instance.ClearBuffManager();
            deathIsCalled = true;
            Die();
        }
        else if (!deathIsCalled)
        {
            if (isCloseToCampfire)
            {
                IncreaseHealth(GameTimer.instance.GameMinutesPassedSinceLastUpdate() * (PlayerManager.instance
                    ? PlayerManager.instance.campfireRegenerationRate
                    : 0.0f
                ));
            }
        }
    }

    private void OnDisable()
    {
        isInvincibleByDash = false;
        isInvincible = false;
        // Reset flash
        for (var k = 0; k < rend.materials.Length; k++)
        {
            rend.materials[k].color = originalColors[k];
        }
        ScreenFlash.Instance.ResetScreenColour();
    }

    public bool IncreaseHealth(float health)
    {
        // health = health < 0 ? 0 : Mathf.CeilToInt(health);
        PlayerManager.instance.currentHealth = Mathf.Min(PlayerManager.instance.currentHealth + health, PlayerManager.instance.maxHealth);
        float currentHealth = PlayerManager.instance.currentHealth;
        float maxHealth = PlayerManager.instance.maxHealth;
        DisplayOnUI(currentHealth, maxHealth);
        return true;
    }

    public bool WasDeathCalled()
    {
        return deathIsCalled;
    }

    public void RestoreToFull()
    {
        deathIsCalled = false;
        PlayerManager.instance.currentHealth = PlayerManager.instance.maxHealth;
        DisplayOnUI(PlayerManager.instance.currentHealth, PlayerManager.instance.maxHealth);
    }

    public IEnumerator MakePlayerInvincibleByBuff(float duration)
    {
        isInvincibleByBuff = true;
        yield return new WaitForSeconds(duration);
        isInvincibleByBuff = false;
    }

    public bool HandleHit(float damage, bool drowning = false)
    {
        damage = damage < 0 ? 0 : Mathf.CeilToInt(damage); //Guard check
        if (isInvincible)
        {
            return false;
        }
        if (isInvincibleByBuff)
        {
            SpawnDamageCounter("0");
            return false;
        }
        if (isInvincibleByDash)
        {
            SpawnDamageCounter("DODGE");
            return false;
        }

        int evasionChance = PlayerManager.instance.evasionChance + PlayerManager.instance.evasionBonus;

        if (evasionChance > 0 && Random.Range(0, 100) < evasionChance && !drowning)
        {
            SpawnDamageCounter("MISS");
            return false;
        }

        if (PlayerManager.instance.currentHealth <= 0f)
        {
            return true;
        }
        PlayerManager.instance.currentHealth = Mathf.Max(0f, PlayerManager.instance.currentHealth - damage);
        float currentHealth = PlayerManager.instance.currentHealth;
        float maxHealth = PlayerManager.instance.maxHealth;
        DisplayOnUI(currentHealth, maxHealth);
        SpawnDamageCounter(damage.ToString());
        if (drowning) { SpawnDrowningAlert(); }
        ScreenFlash.Instance.Flash(0.4f, 0.4f, 0.1f, 0.4f);
        ScreenShake.Instance.Shake(0.4f, 0.4f, 0.1f, 0.4f);
        audioSource.Play();
        //AnimTakeDamage();



        StartCoroutine(BecomeTemporarilyInvincibleWithFlash(invincibilityDurationSeconds));
        return true;
    }

    // Rest for 1 hour (hard-coded)
    public void HandlePlayerRest()
    {
        if (isCloseToCampfire && !isClownActive)
        {
            int restMinutes = 60;
            GameTimer.instance.SkipTime(restMinutes, () =>
                IncreaseHealth(restMinutes * (PlayerManager.instance
                    ? PlayerManager.instance.campfireRegenerationRate
                    : 0.0f
                ))
            );
        }
        else
        {
            Debug.Log("PlayerHealth.cs: Trying to rest when not close to campfire! Or clown is active.");
        }
    }

    // Called when the player enters a campfire to heal
    public void OnPlayerEnterCampfire()
    {
        isCloseToCampfire = true;
        healIcon.SetActive(true);
    }

    // Called when the player leaves a campfire to stop healing
    public void OnPlayerLeaveCampfire()
    {
        isCloseToCampfire = false;
        healIcon.SetActive(false);
    }

    private void AnimTakeDamage()
    {
        animator.SetTrigger("isTakingDamage");
    }

    private void Die()
    {
        animator.SetTrigger("isDead");
        UIController.instance.ShowDeathMenu();
    }

    private void SetupIFrame()
    {
        if (rend != null)
        {
            originalColors = new Color[rend.materials.Length];
            for (var i = 0; i < rend.materials.Length; i++)
            {
                originalColors[i] = rend.materials[i].color;
            }
        }
    }

    private void SpawnDamageCounter(string damageText)
    {
        GameObject damageCounter = Instantiate(damageCounter_prefab);
        damageCounter.transform.GetComponentInChildren<Text>().text = damageText.ToString();
        damageCounter.transform.SetParent(canvasDisplay.transform);
        damageCounter.transform.position = cam.WorldToScreenPoint(transform.position);
        playerDamagedNumbers.Add(damageCounter);
    }

    public void DestroyDamageCounter(GameObject damageNum)
	{
        if (playerDamagedNumbers.Contains(damageNum))
		{
            playerDamagedNumbers.Remove(damageNum);
        }
    }
    public void DestroyAllDamageCounter()
    {
        foreach (GameObject damageCounter in playerDamagedNumbers)
		{
            Destroy(damageCounter);
		}
    }

    private void SpawnDrowningAlert()
    {
        GameObject drowningAlert = Instantiate(drowningAlert_prefab);
        drowningAlert.transform.SetParent(canvasDisplay.transform);
        Vector3 pos = cam.WorldToScreenPoint(transform.position);
        pos.y += 50;
        drowningAlert.transform.position = pos;
    }

    public IEnumerator BecomeTemporarilyInvincibleByDash(float invincibilityDurationSeconds)
    {
        isInvincibleByDash = true;
        yield return new WaitForSeconds(invincibilityDurationSeconds);
        isInvincibleByDash = false;
    }

    public IEnumerator BecomeTemporarilyInvincibleWithFlash(float invincibilityDurationSeconds)
    {
        isInvincible = true;
        bool isFlashing = false;
        for (float i = 0; i < invincibilityDurationSeconds; i += invincibilityDeltaTime)
        {
            // Alternate between 0 and 1 scale to simulate flashing
            if (isFlashing)
            {
                for (var k = 0; k < rend.materials.Length; k++)
                {
                    rend.materials[k].color = onDamageColor;
                }
            }
            else
            {
                for (var k = 0; k < rend.materials.Length; k++)
                {
                    rend.materials[k].color = originalColors[k];
                }
            }
            isFlashing = !isFlashing;
            yield return new WaitForSeconds(invincibilityDeltaTime);
        }
        isInvincible = false;
    }

    public void ClearBuffs()
    {
        isInvincibleByBuff = false;
    }

    public void SetIsClownActive(bool isActive)
	{
        isClownActive = isActive;
	}
}

/*
    public void KnockbackPlayer(Vector3 positionOfEnemy)
    {
        //ToImplementKnockback
        //StartCoroutine(KnockCoroutine(positionOfEnemy));
        Vector3 forceDirection = transform.position - positionOfEnemy;
        forceDirection.y = 0;
        Vector3 force = forceDirection.normalized;
        //dpl.KnockBack(force, 50, 3, true);
    }


    private IEnumerator KnockCoroutine(Vector3 positionOfEnemy)
    {
        Vector3 forceDirection = transform.position - positionOfEnemy;
        Vector3 force = forceDirection.normalized;
        gameObject.GetComponent<Rigidbody>().velocity = force * 4;
        yield return new WaitForSeconds(0.3f);
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

        /* private IEnumerator StartDrowning()
    {
        Debug.Log("Start drowning");
        PlayerManager.instance.currentHealth -= drowningDamage;
        DisplayOnUI(PlayerManager.instance.currentHealth, PlayerManager.instance.maxHealth);
        SpawnDamageCounter(drowningDamage);
        audioSource.Play();
        yield return new WaitForSeconds(0.5f);
        isDrowningActivated = false;
    } */
