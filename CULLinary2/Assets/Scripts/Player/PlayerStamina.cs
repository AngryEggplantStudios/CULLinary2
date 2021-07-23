using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private Image staminaBar;
    [SerializeField] private Text staminaText;
    [SerializeField] private float regenerationRate = 1f;
    [SerializeField] private Outline outlineFlash;
    [SerializeField] private float thresholdStamina = 0.25f;
    private Coroutine regenerationCoroutine;
    private bool flashIsActivated = true;
    private bool canRegenerate = true;
    private bool rateBool = true;
    private Color originalFlashColor;
    private Color deactivatedFlashColor = new Color(255, 0, 0, 0);

    private void Awake()
    {
        float currentStamina = PlayerManager.instance ? PlayerManager.instance.currentStamina : 100f;
        float maxStamina = PlayerManager.instance ? PlayerManager.instance.maxStamina : 100f;
        DisplayOnUI(currentStamina, maxStamina);
        originalFlashColor = outlineFlash.effectColor;
        outlineFlash.effectColor = deactivatedFlashColor;
    }

    private void DisplayOnUI(float currentStamina, float maxStamina)
    {
        staminaBar.fillAmount = currentStamina / maxStamina;
        staminaText.text = Mathf.FloorToInt(currentStamina) + "/" + Mathf.FloorToInt(maxStamina);
    }

    private void Update()
    {
        if (PlayerManager.instance.currentStamina < PlayerManager.instance.maxStamina && canRegenerate)
        {
            RegenerateStamina();
            return;
        }
        if (PlayerManager.instance.currentStamina / PlayerManager.instance.maxStamina < thresholdStamina && !flashIsActivated)
        {

            flashIsActivated = true;
            StartCoroutine(flashBar());
        }
        else if (PlayerManager.instance.currentStamina / PlayerManager.instance.maxStamina >= thresholdStamina)
        {
            flashIsActivated = false;
        }
    }

    private IEnumerator flashBar()
    {
        while (PlayerManager.instance.currentStamina / PlayerManager.instance.maxStamina < thresholdStamina)
        {
            outlineFlash.effectColor = originalFlashColor;
            yield return new WaitForSeconds(0.5f);
            outlineFlash.effectColor = deactivatedFlashColor;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void RegenerateStamina()
    {
        if (rateBool)
        {
            StartCoroutine(WaitOneSecond());
            PlayerManager.instance.currentStamina = Mathf.Min(PlayerManager.instance.currentStamina + regenerationRate, PlayerManager.instance.maxStamina);
            float currentStamina = PlayerManager.instance.currentStamina;
            float maxStamina = PlayerManager.instance.maxStamina;
            DisplayOnUI(currentStamina, maxStamina);
        }
    }

    private IEnumerator WaitOneSecond()
    {
        rateBool = false;
        yield return new WaitForSeconds(1f);
        rateBool = true;
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

    public void resetStaminaRegeneration()
    {
        if (regenerationCoroutine != null)
        {
            StopCoroutine(regenerationCoroutine);
        }

        regenerationCoroutine = StartCoroutine(checkRegenerate());
    }

}