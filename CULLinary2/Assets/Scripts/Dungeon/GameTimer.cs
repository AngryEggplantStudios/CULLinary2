using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : SingletonGeneric<GameTimer>
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField] private TextMeshProUGUI DayText;
    [SerializeField] private GameObject DayIcon;
    [SerializeField] private GameObject NightIcon;
    [SerializeField] private TextMeshProUGUI TimeText;

    // 0.2 of 1 minute = 10 seconds eg
    [SerializeField] private float dayLengthInMinutes;
    [SerializeField, Range(0, 1), Tooltip("e.g. 0.25 for 6am")] private float sunrise;
    [SerializeField, Range(0, 1), Tooltip("e.g. 0.75 for 6pm")] private float sunset;
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
        if (sunrise > sunset) { Debug.LogError("Sunrise is after Sunset!"); }

        // Debug.Log("set timescale to 1");
        Time.timeScale = 1;

        gameTime = (float)System.Math.Round(sunrise, 2);
        timeScale = 24 / (dayLengthInMinutes / 60);

        DayText.text = "DAY " + dayNum;
        UpdateTimerText();
    }

    private void Update()
    {
        if (Preset == null || !isRunning)
            return;

        gameTime += Time.deltaTime * timeScale / 86400;
        if (gameTime > 1f)
        {
            gameTime = 1f;
        }

        UpdateTimerText();
        UpdateLighting(gameTime);

        if (gameTime > sunrise && isNewDay)
        {
            isNewDay = false;
            OnStartNewDay?.Invoke();
        }

        if (gameTime >= dayEndTime)
        {
            // Debug.Log("day ended");
            OnEndOfDay?.Invoke();
            StartSceneFadeOut();
        }
    }

    private void UpdateTimerText()
    {
        float actualTime = gameTime * 24;
        hourNum = Mathf.FloorToInt(actualTime) % 24;
        minuteNum = Mathf.FloorToInt((actualTime - (float)hourNum) * 60) % 60;
        timeAsString = hourNum + ":" + minuteNum.ToString("00");
        TimeText.text = timeAsString;

        if (gameTime < sunrise || gameTime > sunset)
        {
            DayIcon.SetActive(false);
            NightIcon.SetActive(true);
        }
        else
        {
            DayIcon.SetActive(true);
            NightIcon.SetActive(false);
        }
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
        GoToNextDay();
    }

    public void GoToNextDay()
    {
        // happens after end of day screen is shown

        // reset player health and teleport player to origin for now
        SpecialEventManager.instance.ClearCurrentEvents();
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerHealth>().RestoreToFull();
        player.GetComponent<PlayerStamina>().RestoreToFull();
        player.transform.position = new Vector3(0f, 0f, 0f); // TODO: go back to last saved campfire

        // no choice find clown by tag because clown might not be around at start of GameTimer.
        GameObject bossClown = GameObject.FindWithTag("ClownBoss");
        if (bossClown)
        {
            bossClown.GetComponent<ClownController>().DeSpawnClown();
        }

        // change time to next day
        gameTime = (float)System.Math.Round(sunrise, 2);
        dayNum++;
        DayText.text = "DAY " + dayNum;
        isNewDay = true;

        UpdateTimerText();
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