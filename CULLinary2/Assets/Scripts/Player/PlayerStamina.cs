using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private GameObject outLine;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Text staminaText;

    private float maxStamina = 100.0f;
    private float currStamina;
    private float staminaConsumed = 0.1f; //0.01f
    private WaitForSeconds timeTakenRegen = new WaitForSeconds(0.05f);
    private Coroutine regen;
    private Image stmBarFull;
    private float coroutineFlash = 0.1f;
    private GameObject flashyOutline;
    private Coroutine regenZero;

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

    public bool hasStamina(float staminaCost)
    {
        return currStamina - staminaConsumed >= 0.0f;
    }

    public void reduceStamina(float staminaCost)
    {
        staminaCost = staminaCost < 0 ? 0 : staminaCost;
        currStamina -= staminaCost;

    }

    //Will only be called when hasEnoughStamina
    public void useStamina()
    {
        currStamina = currStamina - staminaConsumed;
        stmBarFull.fillAmount = currStamina / maxStamina;
        resetStaminaRegen();
    }

    private IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(1);

        while (currStamina < maxStamina)
        {
            currStamina = currStamina + maxStamina / 100;
            stmBarFull.fillAmount = currStamina / maxStamina;
            yield return timeTakenRegen;
        }
    }

    private void resetStaminaRegen()
    {
        if (regen != null)
        {
            StopCoroutine(regen);
        }

        regen = StartCoroutine(RegenStamina());
    }

}