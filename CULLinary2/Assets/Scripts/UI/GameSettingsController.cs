using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class GameSettingsController : SingletonGeneric<GameSettingsController>
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Text currentBgValue;
    [SerializeField] private TMP_Text currentSfxValue;
    [SerializeField] private Slider bgSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullscreenToggle;
    private float maxVol = 20f;
    private float minVol = -30f;

    public Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }

        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        audioMixer.GetFloat("BG_Vol", out float bgFloat);
        SetBGVolume(bgFloat);
        bgSlider.value = bgFloat;
        audioMixer.GetFloat("SFX_Vol", out float sfxFloat);
        sfxSlider.value = sfxFloat;
        SetSFXVolume(sfxFloat);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        fullscreenToggle.isOn = Screen.fullScreen;

    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetBGVolume(float volume)
    {
        if (Mathf.Floor(volume) <= minVol)
        {
            audioMixer.SetFloat("BG_Vol", -80f);
            currentBgValue.text = "0";
            return;
        }
        else if (Mathf.Floor(volume) >= maxVol)
        {
            audioMixer.SetFloat("BG_Vol", 20f);
            currentBgValue.text = "100";
            return;
        }
        audioMixer.SetFloat("BG_Vol", volume);
        currentBgValue.text = Mathf.RoundToInt((volume - minVol) / (maxVol - minVol) * 100).ToString();
    }

    public void SetSFXVolume(float volume)
    {
        if (Mathf.RoundToInt(volume) <= minVol)
        {
            audioMixer.SetFloat("SFX_Vol", -80f);
            currentSfxValue.text = "0";
            return;
        }
        else if (Mathf.Floor(volume) >= maxVol)
        {
            audioMixer.SetFloat("SFX_Vol", 20f);
            currentBgValue.text = "100";
            return;
        }
        audioMixer.SetFloat("SFX_Vol", volume);
        currentSfxValue.text = Mathf.RoundToInt((volume - minVol) / (maxVol - minVol) * 100).ToString();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

}
