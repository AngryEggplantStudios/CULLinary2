using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialGameTimer : SingletonGeneric<TutorialGameTimer>
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField] private TextMeshProUGUI DayText;
    [SerializeField] private GameObject[] DayObjects;
    [SerializeField] private GameObject[] NightObjects;
    [SerializeField] private TextMeshProUGUI TimeText;
    [SerializeField] private Slider DayProgress;

    // 0.2 of 1 minute = 10 seconds eg
    [SerializeField] private float dayLengthInMinutes;
    [SerializeField, Range(0, 1), Tooltip("e.g. 0.25 for 6am")] private float sunrise;
    [SerializeField, Range(0, 1), Tooltip("e.g. 0.75 for 6pm")] private float sunset;
    [SerializeField, Range(0, 1), Tooltip("e.g. 0.33 for 8am")] private float startOfDay;

    [Header("Daily News")]
    [SerializeField] private GameObject hudToHide;

    private static float gameTime;
    private static float timeScale;
    private static int dayNum = 0;
    private int hourNum;
    private int minuteNum;
    private string timeAsString;

    private float dayEndTime = 1f; //12am

    private bool isNewDay = true; // prevent OnStartNewDay from being invoked multiple times
    private bool isRunning = false;


    public delegate void StartNewDayDelegate();
    public static event StartNewDayDelegate OnStartNewDay;
    public delegate void BeforeStartNewDayDelegate();
    // public static event BeforeStartNewDayDelegate OnBeforeStartNewDay; // invoked right before OnStartNewDay; for enabling certain populations
    public delegate void StartNightDelegate();
    public static event StartNightDelegate OnStartNight;
    public delegate void BeforeStartNightDelegate(); //for enabling Mushy
    public delegate void EndOfDayDelegate();
    public static event EndOfDayDelegate OnEndOfDay;


    void Start()
    {
        dayNum = 0;
        if (sunrise > sunset) { Debug.LogError("Sunrise is after Sunset!"); }

        // Debug.Log("set timescale to 1");
        Time.timeScale = 1;

        gameTime = (float)System.Math.Round(startOfDay, 2);
        timeScale = 24 / (dayLengthInMinutes / 60);

        DayText.text = "DAY " + dayNum;
        UpdateTimedObjects();
        UpdateLighting();
    }

    private void Update()
    {
        if (Preset == null || !isRunning)
            return;
        gameTime += Time.deltaTime * timeScale / 86400;
        ClampTimeAndUpdateObjects();
    }

    // Returns the number of game minutes passed since the last frame update
    public float GameMinutesPassedSinceLastUpdate()
    {
        return Time.deltaTime * timeScale / 60;
    }

    // Skip ahead by a certain number of minutes
    // 
    // Note: Number of minutes is in game time, not actual time!!
    // 
    // Action toRunWhenSkipping will be executed when the 
    // screen fades in, and before the screen fades out
    public void SkipTime(int minutes, Action toRunWhenSkipping)
    {
        if (gameTime < 1.0f)
        {
            SceneTransitionManager.instance.FadeInAndFadeOut(() =>
            {
                AddMinutes(minutes);
                toRunWhenSkipping();
            });
        }
    }

    // Adds a certain number of minutes to the clock
    private void AddMinutes(int minutes)
    {
        gameTime += (float)minutes / 1440;
        ClampTimeAndUpdateObjects();
    }

    private void ClampTimeAndUpdateObjects()
    {
        if (gameTime > 1f)
        {
            gameTime = 1f;
        }

        UpdateTimedObjects();
        UpdateLighting();

        if (gameTime > startOfDay && isNewDay)
        {
            isNewDay = false;
            OnStartNewDay?.Invoke();
        }

        if (gameTime >= dayEndTime)
        {
            OnEndOfDay?.Invoke();
            StartSceneFadeOut();
        }
    }

    private void UpdateTimedObjects()
    {
        // Text
        float actualTime = gameTime * 24;
        hourNum = Mathf.FloorToInt(actualTime) % 24;
        minuteNum = Mathf.FloorToInt((actualTime - (float)hourNum) * 60) % 60;
        timeAsString = hourNum + ":" + minuteNum.ToString("00");
        TimeText.text = timeAsString;

        // Objects
        bool isNight = (gameTime < sunrise || gameTime > sunset);
        foreach (GameObject obj in DayObjects)
        {
            obj.SetActive(!isNight);
        }
        foreach (GameObject obj in NightObjects)
        {
            obj.SetActive(isNight);
        }

        // Slider
        DayProgress.value = (gameTime - startOfDay) / (1 - startOfDay);
    }

    private void StartSceneFadeOut()
    {
        Pause(); // TODO: pause entire game using timeScale
                 //TODO JESS use the scene transition manager to fade in the dialogue for restarting the day
                 //SceneTransitionManager.instance.FadeInImage();
                 //Invoke("ShowEndOfDayMenu", 1);
                 //TODO JESS Call all the events before restarting the day and calling run()

        StartCoroutine(TutorialUIController.instance.ShowRestartTutorialPanel());
        // Time.timeScale = 0;
        // RestartDay();
        // Run();
    }

    // public void RestartDay()
    // {
    //     // GameObject player = GameObject.FindWithTag("Player");
    //     // happens after end of day screen is shown
    //     // reset player health and teleport player to origin for now
    //     // player.GetComponent<PlayerHealth>().RestoreToFull();
    //     // player.GetComponent<PlayerHealth>().DestroyAllDamageCounter();
    //     // player.GetComponent<PlayerStamina>().RestoreToFull();
    //     //BuffManager.instance.ClearBuffManager();
    //     // player.GetComponent<CharacterController>().enabled = false;
    //     // //Spawn at the tutorial campfire
    //     // player.transform.position = new Vector3(-83.0f, 0f, 53.0f);
    //     // player.GetComponent<CharacterController>().enabled = true;

    //     // change time to next day
    //     gameTime = (float)System.Math.Round(startOfDay, 2);
    //     DayText.text = "DAY " + dayNum;
    //     isNewDay = true;

    //     UpdateTimedObjects();
    //     UpdateLighting();

    //     Run();
    // }

    public void Run()
    {
        isRunning = true;
        UpdateLighting();
        UpdateTimedObjects();
    }

    public void Pause()
    {
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

    public void UpdateLighting()
    {
        float timePercent = gameTime;
        // Set ambient and fog
        RenderSettings.ambientSkyColor = Preset.AmbientSkyColor.Evaluate(timePercent);
        RenderSettings.ambientEquatorColor = Preset.AmbientEquatorColor.Evaluate(timePercent);
        RenderSettings.ambientGroundColor = Preset.AmbientGroundColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        //If the directional light is set then rotate and set it's color, I actually rarely use the rotation because it casts tall shadows unless you clamp the value
        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3(TimeToSunAngle(timePercent), -110f, 0));
        }
    }

    private float TimeToSunAngle(float time)
    {
        // No sun
        if (time < sunrise || time > sunset) { return -90; }

        // Remap from 20 to 160
        return (time - sunrise) / (sunset - sunrise) * (160 - 20) + 20;
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