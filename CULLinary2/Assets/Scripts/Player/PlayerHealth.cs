using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
  [SerializeField] private AudioSource audioSource;
  [SerializeField] private GameObject damageCounter_prefab;
  [SerializeField] private GameObject gameOverUI;
  [SerializeField] private Image healthBar;
  [SerializeField] private Text healthText;
  [SerializeField] private float invincibilityDurationSeconds;
  [SerializeField] private GameObject model;
  private bool isInvincible = false;
  private float currHealth;
  private Renderer rend;
  private Color[] originalColors;
  private Color onDamageColor = Color.white;
  private float invincibilityDeltaTime = 0.025f;

  private void Awake() {
    int currentHealth = PlayerManager.instance ? PlayerManager.instance.currentHealth : 200;
    int maxHealth = PlayerManager.instance ? PlayerManager.instance.maxHealth : 200;
    healthBar.fillAmount = currentHealth / maxHealth;
    healthText.text = currentHealth + "/" + maxHealth;
    this.currHealth = currentHealth;
  }

  public bool HandleHit(int damage) {
    if (isInvincible) {
      return false;
    }
    PlayerManager.instance.currentHealth -= damage;
    healthBar.fillAmount = PlayerManager.instance.currentHealth / PlayerManager.instance.maxHealth;
    healthText.text = PlayerManager.instance.currentHealth + "/" + PlayerManager.instance.maxHealth;
    //Spawn damage counter
    //Play Audio
    //If health is 0, play game over scene and invoke game over event
    SpawnDamageCounter(damage);
    audioSource.Play();

    if (this.currHealth <= 0)
    {
      //gameOverUI.GetComponent<GameManager>().GameOver();
      //Die();
      Debug.Log("Ded");
    }
    StartCoroutine(BecomeTemporarilyInvincible());
    return true;
  }

  public float GetHealth()
  {
    return this.currHealth;
  }

  private void SetupFlash()
  {
    rend = model.GetComponentInChildren<Renderer>();
    if (rend)
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
    //damageCounter.transform.SetParent(GameObject.FindObjectOfType<InventoryUI>().transform);
    //damageCounter.transform.position = cam.WorldToScreenPoint(transform.position);
    Debug.Log("I'm damaged by " + damage);
  }

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
