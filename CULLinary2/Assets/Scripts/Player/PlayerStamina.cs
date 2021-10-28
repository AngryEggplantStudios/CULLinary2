using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : SingletonGeneric<PlayerStamina>
{
    [Header("References")]
    [SerializeField] private Image staminaCircleImage;
    [SerializeField] private GameObject staminaCircle;
    [SerializeField] private Animator staminaCircleAnimator;

    [Header("Variables")]
    [SerializeField] private float regenerationRate = 1f;
    [SerializeField] private float pauseBeforeRegen = 1.5f;
    [SerializeField] private float thresholdStamina = 0.25f;
    private Coroutine regenerationCoroutine = null; 
    private WaitForSeconds timeTakenRegen = new WaitForSeconds(0.05f);
    private bool isUnlimitedStamina = false;
    private bool isHalfStamina = false;

    public override void Awake()
    {
        base.Awake();
        float currentStamina = PlayerManager.instance ? PlayerManager.instance.currentStamina : 100f;
        float maxStamina = PlayerManager.instance ? PlayerManager.instance.maxStamina : 100f;
        DisplayOnUI(currentStamina, maxStamina);
        staminaCircle.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(checkRegenerate());
    }

    private void OnDisable()
    {
        StopCoroutine(SetStaminaCircleActiveForTime());
        if (regenerationCoroutine != null)
        {
            StopCoroutine(regenerationCoroutine);
        }
    }

    private void DisplayOnUI(float currentStamina, float maxStamina)
    {
        staminaCircleImage.fillAmount = currentStamina / maxStamina;
    }

    public void SetStaminaCircleActive(bool b)
    {
        staminaCircle.SetActive(b);
    }

    public void RestoreToFull()
    {
        PlayerManager.instance.currentStamina = PlayerManager.instance.maxStamina;
        DisplayOnUI(PlayerManager.instance.currentStamina, PlayerManager.instance.maxStamina);
    }

    public IEnumerator ToggleUnlimitedStamina(float duration)
    {
        isUnlimitedStamina = true;
        yield return new WaitForSeconds(duration);
        isUnlimitedStamina = false;
    }

    private IEnumerator checkRegenerate()
    {
        yield return new WaitForSeconds(pauseBeforeRegen);
        while (PlayerManager.instance.currentStamina < (PlayerManager.instance.maxStamina))
        {
            PlayerManager.instance.currentStamina = Mathf.Min(PlayerManager.instance.currentStamina + regenerationRate, PlayerManager.instance.maxStamina);
            float currentStamina = PlayerManager.instance.currentStamina;
            float maxStamina = PlayerManager.instance.maxStamina;
            DisplayOnUI(currentStamina, maxStamina);
            yield return timeTakenRegen;
        }
        staminaCircle.SetActive(false); ;
    }

    public bool HasStamina(float staminaCost)
    {
        if (isUnlimitedStamina)
        {
            return true;
        }
        return PlayerManager.instance.currentStamina - staminaCost >= 0.0f;
    }

    public bool IncreaseStamina(float increase)
    {
        StartCoroutine(SetStaminaCircleActiveForTime());
        PlayerManager.instance.currentStamina = Mathf.Min(PlayerManager.instance.currentStamina + increase, PlayerManager.instance.maxStamina);
        float currentStamina = PlayerManager.instance.currentStamina;
        float maxStamina = PlayerManager.instance.maxStamina;
        DisplayOnUI(currentStamina, maxStamina);
        return true;
    }

    public IEnumerator SetStaminaCircleActiveForTime()
    {
        SetStaminaCircleActive(true);
        yield return new WaitForSeconds(3f);
        SetStaminaCircleActive(false);
    }

    public void ReduceStamina(float staminaCost)
    {
        if (isUnlimitedStamina)
        {
            staminaCost = 0;
        }
        staminaCost = staminaCost < 0 ? 0 : staminaCost;
        PlayerManager.instance.currentStamina = Mathf.Max(0f, PlayerManager.instance.currentStamina - staminaCost);
        float currentStamina = PlayerManager.instance.currentStamina;
        float maxStamina = PlayerManager.instance.maxStamina;
        DisplayOnUI(currentStamina, maxStamina);
        staminaCircleAnimator.SetBool("flashing", PlayerManager.instance.currentStamina / PlayerManager.instance.maxStamina < thresholdStamina);
        ResetStaminaRegeneration();
    }

    public void ResetStaminaRegeneration()
    {
        if (regenerationCoroutine != null)
        {
            StopCoroutine(regenerationCoroutine);
        }
        regenerationCoroutine = StartCoroutine(checkRegenerate());
    }
    public void ClearBuffs()
    {
        isUnlimitedStamina = false;
    }
}