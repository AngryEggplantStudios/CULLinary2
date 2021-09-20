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
    private static float gameTime;
    private static float timeScale;
    private static int dayNum = 1;
    private int hourNum;
    private int minuteNum;
    private string timeAsString;

    public delegate void StartNewDayDelegate();
    public static event StartNewDayDelegate OnStartNewDay;
    private bool isNewDay = true; // prevent OnStartNewDay from being invoked multiple times

    void Start()
    {
        gameTime = 0.25f; //day starts at 6 am 
        timeScale = 24 / (dayLengthInMinutes / 60);
        DayText.text = "DAY " + dayNum;
        TimeText.text = "06:00";
    }

    private void Update()
    {
        if (Preset == null)
            return;
        gameTime += Time.deltaTime * timeScale / 86400;
        float actualTime = gameTime * 24;
        hourNum = Mathf.FloorToInt(actualTime);
        minuteNum = Mathf.FloorToInt((actualTime - (float)hourNum) * 60);
        timeAsString = hourNum + ":" + minuteNum.ToString("00");
        TimeText.text = timeAsString;
        // Debug.Log("current time: " + timeAsString);

        if (timeAsString == "6:00" && isNewDay)
        {
            //Debug.Log("start of new day");
            isNewDay = false;
            OnStartNewDay?.Invoke();
        }

        UpdateLighting(gameTime);
        if (gameTime > 1)
        {
            dayNum++;
            gameTime -= 1;
            DayText.text = "DAY " + dayNum;
            isNewDay = true;
        }
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
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        //If the directional light is set then rotate and set it's color, I actually rarely use the rotation because it casts tall shadows unless you clamp the value
        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
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