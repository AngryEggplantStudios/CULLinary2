using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image staminaCircleImage;
    [SerializeField] private GameObject staminaCircle;
    [SerializeField] private Animator staminaCircleAnimator;

    [Header("Variables")]
    [SerializeField] private float regenerationRate = 1f;
    [SerializeField] private float pauseBeforeRegen = 1.5f;
    [SerializeField] private float thresholdStamina = 0.25f;
    private Coroutine regenerationCoroutine;
    private WaitForSeconds timeTakenRegen = new WaitForSeconds(0.05f);
    private void Start()
    {
        float currentStamina = PlayerManager.instance ? PlayerManager.instance.currentStamina : 100f;
        float maxStamina = PlayerManager.instance ? PlayerManager.instance.maxStamina : 100f;
        DisplayOnUI(currentStamina, maxStamina);
        staminaCircle.SetActive(false);
    }

    private void DisplayOnUI(float currentStamina, float maxStamina)
    {
        staminaCircleImage.fillAmount = currentStamina / maxStamina;
    }

    public void SetStaminaCircleActive(bool setActive)
    {
        staminaCircle.SetActive(true);
    }

    public void RestoreToFull()
    {
        PlayerManager.instance.currentStamina = PlayerManager.instance.maxStamina;
        DisplayOnUI(PlayerManager.instance.currentStamina, PlayerManager.instance.maxStamina);
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
        return PlayerManager.instance.currentStamina - staminaCost >= 0.0f;
    }

    public void ReduceStamina(float staminaCost)
    {
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
}