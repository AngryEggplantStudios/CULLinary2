using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : SingletonGeneric<GameTimer>
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField] private TextMeshProUGUI DayText;
    [SerializeField] private TextMeshProUGUI TimeText;

    // 0.2 of 1 minute = 10 seconds eg
    [SerializeField] private float dayLengthInMinutes;
    [Tooltip("e.g. 0.25 for 6am")]
    [Range(0, 1f)]
    [SerializeField] private float dayStartTime = 0.25f; //6am
    private static float gameTime;
    private static float timeScale;
    private static int dayNum = 1; // TODO: to get from saved data
    private int hourNum;
    private int minuteNum;
    private string timeAsString;

    private float dayEndTime = 1f; //12am

    private bool isNewDay = true; // prevent OnStartNewDay from being invoked multiple times
    private bool isRunning = false;

    public delegate void StartNewDayDelegate();
    public static event StartNewDayDelegate OnStartNewDay;
    public delegate void EndOfDayDelegate();
    public static event EndOfDayDelegate OnEndOfDay;

    void Start()
    {
        Debug.Log("set timescale to 1");
        Time.timeScale = 1;

        gameTime = dayStartTime;
        Debug.Log("start game time");
        timeScale = 24 / (dayLengthInMinutes / 60);
        DayText.text = "DAY " + dayNum;
        TimeText.text = "06:00";
    }

    private void Update()
    {
        if (Preset == null || !isRunning)
            return;

        UpdateGameTime();

        UpdateLighting(gameTime);

        // if (timeAsString == "6:00" && isNewDay)
        if (gameTime >= dayStartTime && isNewDay)
        {
            Debug.Log("start of day " + dayNum);
            isNewDay = false;
            OnStartNewDay?.Invoke();
        }

        if (gameTime >= 1f)
        {
            Debug.Log("day ended");
            OnEndOfDay?.Invoke();
            StartSceneFadeOut();
        }
    }

    private void UpdateGameTime()
    {
        gameTime += Time.deltaTime * timeScale / 86400;
        float actualTime = gameTime * 24;
        hourNum = Mathf.FloorToInt(actualTime);
        minuteNum = Mathf.FloorToInt((actualTime - (float)hourNum) * 60);
        timeAsString = hourNum + ":" + minuteNum.ToString("00");
        TimeText.text = timeAsString;
    }

    private void StartSceneFadeOut()
    {
        Pause();
        Debug.Log("StartSceneFadeOut()");
        // Time.timeScale = 0; //pause game
        //fade to black
        Debug.Log("fading scene out");
        SceneTransitionManager.instance.FadeSceneIn();
        // yield return new WaitForSecondsRealtime(1);
        // Debug.Log("waited 1 sec");
        Invoke("ShowEndOfDayMenu", 1);
        GoToNextDay();
    }

    private void ShowEndOfDayMenu()
    {
        Debug.Log("fade end");
        UIController.instance.ShowEndOfDayMenu();
    }

    public void GoToNextDay()
    {
        // triggered from continue button on the end of day menu
        // TriggerEndOfDaySequence();
        // is next day, player at 0, 0, 0   

        Debug.Log("start go to next day");
        PlayerManager.instance.currentHealth = PlayerManager.instance.maxHealth;
        PlayerManager.instance.currentStamina = PlayerManager.instance.maxStamina;

        dayNum++;
        gameTime = dayStartTime;
        DayText.text = "DAY " + dayNum;
        isNewDay = true;
        UpdateGameTime();
        UpdateLighting(gameTime);
        // Run();
        Debug.Log("end go to next day");
    }

    public void Run()
    {
        Debug.Log("running game timer");
        isRunning = true;
    }

    public void Pause()
    {
        Debug.Log("pausing game timer");
        isRunning = false;
    }

    public static float GetTime()
    {
        return gameTime;
    }

    public static int GetDayNumber()
    {
        return dayNum;
    }

    private void OnDestroy()
    {
        gameTime = 0f;
    }

    private void UpdateLighting(float timePercent)
    {
        //Set ambient and fog
        //RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        //RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        //If the directional light is set then rotate and set it's color, I actually rarely use the rotation because it casts tall shadows unless you clamp the value
        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));

            // Debug.Log("color: " + DirectionalLight.color + " | transform.localRotation: " + DirectionalLight.transform.localRotation);
        }

    }
    //Try to find a directional light to use if we haven't set one
    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;

        //Search for lighting tab sun
        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        //Search scene for light that fits criteria (directional)
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }
}