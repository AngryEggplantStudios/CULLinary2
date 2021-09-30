using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private Image staminaCircleImage;
    [SerializeField] private GameObject staminaCircle;
    [SerializeField] private float regenerationRate = 1f;
    [SerializeField] private float pauseBeforeRegen = 1.5f;
    [SerializeField] private Animator staminaCircleAnimator;
    [SerializeField] private float thresholdStamina = 0.25f;
    [SerializeField] private GameObject canvasDisplay;
    [SerializeField] private Camera playerCamera;

    private Coroutine regenerationCoroutine;
    private bool flashIsActivated = false;
    private Color originalFlashColor;
    private Color deactivatedFlashColor = new Color(255, 0, 0, 0);
    private Color dangerCircleFlashColor;
    private WaitForSeconds timeTakenRegen = new WaitForSeconds(0.05f);
    private GameObject parentGameObject;

    private void Awake()
    {
        /*
        originalFlashColor = outlineFlash.effectColor;
        outlineFlash.effectColor = deactivatedFlashColor;
        dangerCircleFlashColor = outlineCircleFlash.effectColor;
        outlineCircleFlash.effectColor = deactivatedFlashColor;
        parentGameObject = outlineCircleFlash.gameObject;
        */
    }

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
        //staminaText.text = Mathf.FloorToInt(currentStamina) + "/" + Mathf.FloorToInt(maxStamina);
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

    private void Update()
    {
        /*
        if (PlayerManager.instance.currentStamina / PlayerManager.instance.maxStamina < thresholdStamina && !flashIsActivated)
        {
            flashIsActivated = true;
            StartCoroutine(flashBar());
        }
        else if (PlayerManager.instance.currentStamina / PlayerManager.instance.maxStamina >= thresholdStamina)
        {
            flashIsActivated = false;
        }

/*        Vector2 screenPos = playerCamera.WorldToScreenPoint(transform.position);
        screenPos.x = screenPos.x + 40f;
        screenPos.y = screenPos.y + 50f;
        if (screenPos != Vector2.zero)
        {
            staminaCircle.transform.position = screenPos;
        }*/

    }

    private void SetupUI(GameObject ui)
    {
        ui.transform.SetParent(canvasDisplay.transform);
        ui.transform.position = playerCamera.WorldToScreenPoint(transform.position);
    }

    /* private IEnumerator flashBar()
    {
        while (PlayerManager.instance.currentStamina / PlayerManager.instance.maxStamina < thresholdStamina)
        {
            outlineFlash.effectColor = originalFlashColor;
            outlineCircleFlash.effectColor = dangerCircleFlashColor;
            yield return new WaitForSeconds(0.5f);
            outlineFlash.effectColor = deactivatedFlashColor;
            outlineCircleFlash.effectColor = deactivatedFlashColor;
            yield return new WaitForSeconds(0.5f);
        }
    } */

    private IEnumerator checkRegenerate()
    {
        yield return new WaitForSeconds(pauseBeforeRegen);
        while (PlayerManager.instance.currentStamina < (PlayerManager.instance.maxStamina))
        {
            //StartCoroutine(WaitOneSecond());
            PlayerManager.instance.currentStamina = Mathf.Min(PlayerManager.instance.currentStamina + regenerationRate, PlayerManager.instance.maxStamina);
            float currentStamina = PlayerManager.instance.currentStamina;
            float maxStamina = PlayerManager.instance.maxStamina;
            DisplayOnUI(currentStamina, maxStamina);
            yield return timeTakenRegen;
        }
        staminaCircle.SetActive(false); ;
    }

    public bool hasStamina(float staminaCost)
    {
        return PlayerManager.instance.currentStamina - staminaCost >= 0.0f;
    }

    public void reduceStamina(float staminaCost)
    {
        staminaCost = staminaCost < 0 ? 0 : staminaCost;
        PlayerManager.instance.currentStamina = Mathf.Max(0f, PlayerManager.instance.currentStamina - staminaCost);
        float currentStamina = PlayerManager.instance.currentStamina;
        float maxStamina = PlayerManager.instance.maxStamina;
        DisplayOnUI(currentStamina, maxStamina);
        staminaCircleAnimator.SetBool("flashing", PlayerManager.instance.currentStamina / PlayerManager.instance.maxStamina < thresholdStamina);
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