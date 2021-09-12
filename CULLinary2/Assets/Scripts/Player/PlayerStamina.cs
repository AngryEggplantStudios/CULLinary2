using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image staminaCircle;
    [SerializeField] private Text staminaText;
    [SerializeField] private float regenerationRate = 1f;
    [SerializeField] private Outline outlineFlash;
    [SerializeField] private Image outlineCircleFlash;
    [SerializeField] private float thresholdStamina = 0.25f;
    [SerializeField] private GameObject canvasDisplay;
    [SerializeField] private Camera playerCamera;


    private Coroutine regenerationCoroutine;
    private bool flashIsActivated = false;
    private Color originalFlashColor;
    private Color deactivatedFlashColor = new Color(255, 0, 0, 0);
    private Color dangerCircleFlashColor = Color.red;
    private Color originalCircleFlashColor = Color.white;
    private Image outlineCircleColor;
    private WaitForSeconds timeTakenRegen = new WaitForSeconds(0.05f);

    private void Awake()
    {
        originalFlashColor = outlineFlash.effectColor;
        outlineFlash.effectColor = deactivatedFlashColor;
    }

    private void Start()
    {
        float currentStamina = PlayerManager.instance ? PlayerManager.instance.currentStamina : 100f;
        float maxStamina = PlayerManager.instance ? PlayerManager.instance.maxStamina : 100f;
        outlineCircleColor = outlineCircleFlash.GetComponent<Image>();
        DisplayOnUI(currentStamina, maxStamina);
        SetupUI(staminaCircle.gameObject);
        SetStaminaCircleActive(false);
    }

    private void DisplayOnUI(float currentStamina, float maxStamina)
    {
        // for now keep both active
        staminaBar.fillAmount = currentStamina / maxStamina;
        staminaCircle.fillAmount = currentStamina / maxStamina;
        staminaText.text = Mathf.FloorToInt(currentStamina) + "/" + Mathf.FloorToInt(maxStamina);
    }

    public void SetStaminaCircleActive(bool setActive)
    {
        staminaCircle.gameObject.SetActive(setActive);
    }

    private void Update()
    {
        if (PlayerManager.instance.currentStamina / PlayerManager.instance.maxStamina < thresholdStamina && !flashIsActivated)
        {
            flashIsActivated = true;
            StartCoroutine(flashBar());
        }
        else if (PlayerManager.instance.currentStamina / PlayerManager.instance.maxStamina >= thresholdStamina)
        {
            flashIsActivated = false;
        }

        Vector2 screenPos = playerCamera.WorldToScreenPoint(transform.position);
        screenPos.x = screenPos.x + 40f;
        screenPos.y = screenPos.y + 50f;
        if (screenPos != Vector2.zero)
        {
            staminaCircle.transform.position = screenPos;
        }
    }

    private void SetupUI(GameObject ui)
    {
        ui.transform.SetParent(canvasDisplay.transform);
        ui.transform.position = playerCamera.WorldToScreenPoint(transform.position);
    }

    private IEnumerator flashBar()
    {
        while (PlayerManager.instance.currentStamina / PlayerManager.instance.maxStamina < thresholdStamina)
        {
            outlineFlash.effectColor = originalFlashColor;
            outlineCircleColor.color = dangerCircleFlashColor;
            yield return new WaitForSeconds(0.5f);
            outlineFlash.effectColor = deactivatedFlashColor;
            outlineCircleColor.color = originalCircleFlashColor;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator checkRegenerate()
    {
        yield return new WaitForSeconds(2f);
        while (PlayerManager.instance.currentStamina < (PlayerManager.instance.maxStamina))
        {
            //StartCoroutine(WaitOneSecond());
            PlayerManager.instance.currentStamina = Mathf.Min(PlayerManager.instance.currentStamina + regenerationRate, PlayerManager.instance.maxStamina);
            float currentStamina = PlayerManager.instance.currentStamina;
            float maxStamina = PlayerManager.instance.maxStamina;
            DisplayOnUI(currentStamina, maxStamina);
            yield return timeTakenRegen;
        }
        SetStaminaCircleActive(false);
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