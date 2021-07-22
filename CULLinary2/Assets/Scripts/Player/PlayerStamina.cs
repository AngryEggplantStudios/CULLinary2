using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private Image staminaBar;
    [SerializeField] private Text staminaText;
    [SerializeField] private float regenerationRate = 1f;
    private Coroutine regen;
    private bool canRegenerate = true;

    // Start is called before the first frame update
    private void DisplayOnUI(float currentStamina, float maxStamina)
    {
        staminaBar.fillAmount = currentStamina / maxStamina;
        staminaText.text = Mathf.FloorToInt(currentStamina) + "/" + Mathf.FloorToInt(maxStamina);
    }

    private void Awake()
    {
        float currentStamina = PlayerManager.instance ? PlayerManager.instance.currentStamina : 100f;
        float maxStamina = PlayerManager.instance ? PlayerManager.instance.maxStamina : 100f;
        DisplayOnUI(currentStamina, maxStamina);
    }

    private void Update()
    {
        if (PlayerManager.instance.currentStamina < PlayerManager.instance.maxStamina && canRegenerate)
        {
            StartCoroutine(RegenerateStamina());
            return;
        }
    }

    private IEnumerator RegenerateStamina()
    {
        while (PlayerManager.instance.currentStamina < PlayerManager.instance.maxStamina)
        {
            //Regenerate
            PlayerManager.instance.currentStamina = Mathf.Min(PlayerManager.instance.currentStamina + regenerationRate, PlayerManager.instance.maxStamina);
            float currentStamina = PlayerManager.instance.currentStamina;
            float maxStamina = PlayerManager.instance.maxStamina;
            DisplayOnUI(currentStamina, maxStamina);
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator checkRegenerate()
    {
        canRegenerate = false;
        yield return new WaitForSeconds(3f);
        canRegenerate = true;
    }

    public bool hasStamina(float staminaCost)
    {
        return PlayerManager.instance.currentStamina - staminaCost >= 0.0f;
    }

    public void reduceStamina(float staminaCost)
    {
        staminaCost = staminaCost < 0 ? 0 : staminaCost;
        PlayerManager.instance.currentStamina -= staminaCost;
        float currentStamina = PlayerManager.instance.currentStamina;
        float maxStamina = PlayerManager.instance.maxStamina;
        DisplayOnUI(currentStamina, maxStamina);
        resetStaminaRegeneration();
    }

    private void resetStaminaRegeneration()
    {
        if (regen != null)
        {
            StopCoroutine(regen);
        }

        regen = StartCoroutine(checkRegenerate());
    }

}