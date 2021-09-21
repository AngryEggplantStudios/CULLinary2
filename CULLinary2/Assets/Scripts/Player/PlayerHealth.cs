using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject damageCounter_prefab;
    [SerializeField] private Image healthBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private float invincibilityDurationSeconds;
    [SerializeField] private GameObject model;
    [SerializeField] private Outline outlineFlash;
    private bool flashIsActivated = false;

    //To be ported over to somewhere
    [SerializeField] private GameObject canvasDisplay;
    [SerializeField] private Camera cam;
    [SerializeField] private float thresholdHealth = 0.25f;
    [SerializeField] private Renderer rend;
    private Color originalFlashColor;
    private Color deactivatedFlashColor = new Color(255, 0, 0, 0);
    private bool isInvincible = false;

    private Color[] originalColors;
    private Color onDamageColor = Color.white;
    private float invincibilityDeltaTime = 0.025f;
    private Animator animator;

    private void DisplayOnUI(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
        healthText.text = Mathf.CeilToInt(currentHealth) + "/" + Mathf.CeilToInt(maxHealth);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        SetupIFrame();
        //originalFlashColor = outlineFlash.effectColor;
        //outlineFlash.effectColor = deactivatedFlashColor;
    }

    private void Start()
    {
        float currentHealth = PlayerManager.instance ? PlayerManager.instance.currentHealth : 200f;
        float maxHealth = PlayerManager.instance ? PlayerManager.instance.maxHealth : 200f;
        DisplayOnUI(currentHealth, maxHealth);
    }

    private void Update()
    {
        if (PlayerManager.instance.currentHealth / PlayerManager.instance.maxHealth < thresholdHealth && !flashIsActivated)
        {
            flashIsActivated = true;
            StartCoroutine(FlashBar());
        }
        else if (PlayerManager.instance.currentHealth / PlayerManager.instance.maxHealth >= thresholdHealth)
        {
            flashIsActivated = false;
        }
    }

    private IEnumerator FlashBar()
    {
        while (PlayerManager.instance.currentHealth / PlayerManager.instance.maxHealth < thresholdHealth)
        {
            //outlineFlash.effectColor = originalFlashColor;
            yield return new WaitForSeconds(0.5f);
            //outlineFlash.effectColor = deactivatedFlashColor;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public bool IncreaseHealth(float health)
    {
        health = health < 0 ? 0 : Mathf.CeilToInt(health);
        PlayerManager.instance.currentHealth = Mathf.Min(PlayerManager.instance.currentHealth + health, PlayerManager.instance.maxHealth);
        float currentHealth = PlayerManager.instance.currentHealth;
        float maxHealth = PlayerManager.instance.maxHealth;
        DisplayOnUI(currentHealth, maxHealth);
        return true;
    }

    public bool HandleHit(float damage)
    {
        damage = damage < 0 ? 0 : Mathf.CeilToInt(damage); //Guard check
        if (isInvincible)
        {
            return false;
        }

        PlayerManager.instance.currentHealth = Mathf.Max(0f, PlayerManager.instance.currentHealth - damage);
        float currentHealth = PlayerManager.instance.currentHealth;
        float maxHealth = PlayerManager.instance.maxHealth;
        DisplayOnUI(currentHealth, maxHealth);
        SpawnDamageCounter(damage);
        audioSource.Play();
        animator.SetTrigger("isTakingDamage");

        if (PlayerManager.instance.currentHealth <= 0f)
        {
            Debug.Log("You are dead.");
            return true;
        }
        StartCoroutine(BecomeTemporarilyInvincible());
        return true;
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

    private void SpawnDamageCounter(float damage)
    {
        GameObject damageCounter = Instantiate(damageCounter_prefab);
        damageCounter.transform.GetComponentInChildren<Text>().text = damage.ToString();
        damageCounter.transform.SetParent(canvasDisplay.transform);
        damageCounter.transform.position = cam.WorldToScreenPoint(transform.position);
    }

    public void KnockbackPlayer(Vector3 positionOfEnemy)
    {
        //ToImplementKnockback
        //StartCoroutine(KnockCoroutine(positionOfEnemy));
        /*Vector3 forceDirection = transform.position - positionOfEnemy;
        forceDirection.y = 0;
        Vector3 force = forceDirection.normalized;*/
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

    private IEnumerator BecomeTemporarilyInvincible()
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

}
