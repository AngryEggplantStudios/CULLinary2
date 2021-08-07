using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : SingletonGeneric<GameTimer>
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    // 0.2 of 1 minute = 10 seconds eg
    [SerializeField] private float dayLengthInMinutes;
    private static float gameTime;
    private static float timeScale;
    private int dayNum;
    
    void Start()
    {
        gameTime = 0f;
        timeScale = 24 / (dayLengthInMinutes / 60);
        Debug.Log("Starting class");
    }

    private void Update()
    {
        if (Preset == null)
            return;

        gameTime += Time.deltaTime * timeScale / 86400;
        //Debug.Log(gameTime);
        UpdateLighting(gameTime);
        if (gameTime > 1)
        {
            dayNum++;
            gameTime -= 1;
            Debug.Log(dayNum);
        }
    }

    public static float GetTime()
    {
        return gameTime;
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