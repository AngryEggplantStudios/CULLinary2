using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameTimer : SingletonGeneric<GameTimer>
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
        dayNum = PlayerManager.instance != null ? PlayerManager.instance.currentDay : 1;

        if (sunrise > sunset) { Debug.LogError("Sunrise is after Sunset!"); }

        // Debug.Log("set timescale to 1");
        Time.timeScale = 1;

        gameTime = (float)System.Math.Round(startOfDay, 2);
        timeScale = 24 / (dayLengthInMinutes / 60);

        DayText.text = "DAY " + dayNum;
        UpdateTimedObjects();
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
        UpdateLighting(gameTime);

        if (gameTime > startOfDay && isNewDay)
        {
            isNewDay = false;
            if (dayNum == 2)
            {
                EcosystemManager.EnablePopulation(MonsterName.Potato);
            }
            OnStartNewDay?.Invoke();
        }

        if (gameTime >= dayEndTime)
        {
            OnEndOfDay?.Invoke();
            if (DrivingManager.instance.IsPlayerInVehicle())
            {
                DrivingManager.instance.HandlePlayerLeaveVehicle();
            }
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
        SceneTransitionManager.instance.FadeInImage();
        Invoke("ShowEndOfDayMenu", 1);
    }

    private void ShowEndOfDayMenu()
    {
        UIController.instance.ShowEndOfDayMenu();
        //Restore health here
        GoToNextDay();
        SaveGame();
    }

    public void SaveGame()
    {
        PlayerManager.instance.currentDay = dayNum;
        EcosystemManager.SaveEcosystemPopulation();
        PlayerManager.instance.SaveData(InventoryManager.instance.itemListReference);
    }

    public void GoToNextDay()
    {
        GameObject player = GameObject.FindWithTag("Player");
        // happens after end of day screen is shown
        // reset player health and teleport player to origin for now
        SpecialEventManager.instance.ClearCurrentEvents();
        player.GetComponent<PlayerHealth>().RestoreToFull();
        player.GetComponent<PlayerStamina>().RestoreToFull();
        BuffManager.instance.ClearBuffManager();
        PlayerSpawnManager.instance.SpawnPlayer();

        // no choice find clown by tag because clown might not be around at start of GameTimer.
        GameObject bossClown = GameObject.FindWithTag("ClownBoss");
        if (bossClown)
        {
            bossClown.GetComponentInChildren<ClownController>().DeSpawnClown();
        }

        // change time to next day
        gameTime = (float)System.Math.Round(startOfDay, 2);
        dayNum++;
        DayText.text = "DAY " + dayNum;
        isNewDay = true;

        UpdateTimedObjects();
        UpdateLighting(gameTime);
    }

    public void Run()
    {
        isRunning = true;
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

    private void UpdateLighting(float timePercent)
    {
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