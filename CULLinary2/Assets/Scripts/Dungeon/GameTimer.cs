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

    [Header("Daily News")]
    [SerializeField] private GameObject newspaper;
    [SerializeField] private GameObject hudToHide;
    [Header("Daily Music")]
    [SerializeField] public AudioSource chillBgm;
    [SerializeField] public AudioSource hypeBgm;
    [SerializeField] private AudioClip[] chillMusic;
    [SerializeField] private AudioClip[] hypeMusic;
    [SerializeField] private int numOfTracks;
    [Header("Boss Music")]
    [SerializeField] public AudioClip bossMusic;

    private static float gameTime;
    private static float timeScale;
    private static int dayNum = 1;
    private int hourNum;
    private int minuteNum;
    private string timeAsString;

    private float dayEndTime = 1f; //12am

    private bool isNewDay = true; // prevent OnStartNewDay from being invoked multiple times
    private bool isRunning = false;
    private bool isEndOfDay = false;

    private NewspaperDetails newspaperDets;

    public delegate void StartNewDayDelegate();
    public static event StartNewDayDelegate OnStartNewDay;
    public delegate void BeforeStartNewDayDelegate();
    // public static event BeforeStartNewDayDelegate OnBeforeStartNewDay; // invoked right before OnStartNewDay; for enabling certain populations
    public delegate void StartNightDelegate();
    public static event StartNightDelegate OnStartNight;
    public delegate void BeforeStartNightDelegate(); //for enabling Mushy
    public delegate void EndOfDayDelegate();
    public static event EndOfDayDelegate OnEndOfDay;

    private int currentIssueNumber = 1;
    private bool showRandomNews = false;
    private bool hasActivatedMushroom = false;

    void Start()
    {
        dayNum = PlayerManager.instance != null ? PlayerManager.instance.currentDay : 1;
        currentIssueNumber = PlayerManager.instance != null ? PlayerManager.instance.currentNewspaperIssue : 1;
        newspaperDets = newspaper.GetComponent<NewspaperDetails>();

        if (sunrise > sunset) { Debug.LogError("Sunrise is after Sunset!"); }

        // Debug.Log("set timescale to 1");
        Time.timeScale = 1;

        gameTime = (float)System.Math.Round(startOfDay, 2);
        timeScale = 24 / (dayLengthInMinutes / 60);

        DayText.text = "DAY " + dayNum;
        UpdateTimedObjects();

        OnStartNewDay += () =>
        {
            // Reset isEndOfDay flag
            isEndOfDay = false;

            // enable unlocked monsters
            foreach (MonsterName mn in PlayerManager.instance.unlockedMonsters)
            {
                EcosystemManager.EnablePopulation(mn);
            }
        };
    }

    private void Update()
    {
        // // For debug
        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     ShowEndOfDayMenu();
        //     return;
        // }

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

    // Starts the day after the newspaper is closed
    public void CloseNewspaperAndStartDay()
    {
        newspaper.SetActive(false);
        hudToHide.SetActive(true);
        UIController.instance.isNewspaperOpen = false;
        Time.timeScale = 1;

        // OnBeforeStartNewDay?.Invoke();
        OnStartNewDay?.Invoke();

        //Open orders at start of the day
        UIController.instance.ToggleOrders();
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
            NewsIssue currentNews = showRandomNews
                ? DatabaseLoader.GetRandomNewsIssue()
                : DatabaseLoader.GetOrderedNewsIssueById(currentIssueNumber);

            if (currentNews == null)
            {
                Debug.Log("No newspaper for " + currentIssueNumber + " found");
                // OnBeforeStartNewDay?.Invoke();
                OnStartNewDay?.Invoke();
            }
            else
            {
                // Set up today's music, minus 1 to start at day 1
                int musicTrack = (dayNum - 1) % numOfTracks;
                chillBgm.clip = chillMusic[musicTrack];
                hypeBgm.clip = hypeMusic[musicTrack];
                chillBgm.Play();
                hypeBgm.Play();

                Time.timeScale = 0;
                newspaperDets.UpdateNewspaperIssueUI(currentNews);
                UIController.instance.isNewspaperOpen = true;
                UIController.instance.HandleUIActiveChange(true);
                newspaper.SetActive(true);
                hudToHide.SetActive(false);
                // when newspaper is closed, CloseNewspaperAndStartDay is called
            }
            showRandomNews = false;
        }

        if (!hasActivatedMushroom && gameTime >= sunset && gameTime < dayEndTime)
        {
            hasActivatedMushroom = true;
            StartCoroutine(invokeMushy());
        }

        if (gameTime >= dayEndTime)
        {
            hasActivatedMushroom = false;
            OnEndOfDay?.Invoke();
            if (DrivingManager.instance.IsPlayerInVehicle())
            {
                DrivingManager.instance.HandlePlayerLeaveVehicle(true);
            }
            StartSceneFadeOut();
        }
    }

    private IEnumerator invokeMushy()
    {
        OnStartNight?.Invoke();
        yield return null;
    }

    public bool GetIsMushroomActive()
    {
        return hasActivatedMushroom;
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
        Time.timeScale = 0;
        isEndOfDay = true;
        // Reset BGM
        EnemyAggressionManager.Instance.Reset();
        UIController.instance.ShowEndOfDayMenu();
        // Increment newspaper for next day
        NewsIssue currentNews = DatabaseLoader.GetOrderedNewsIssueById(currentIssueNumber + 1);
        if (currentNews)
        {
            currentIssueNumber++;
            PlayerManager.instance.currentNewspaperIssue = currentIssueNumber;
            DoProgression(currentNews);
        }
        //Restore health here
        GoToNextDay();
    }

    public bool WasEndOfDayCalled()
    {
        return isEndOfDay;
    }

    public void PlayBossMusic()
    {
        chillBgm.clip = bossMusic;
        hypeBgm.clip = bossMusic;
        chillBgm.Play();
        hypeBgm.Play();
    }

    public void StopBgm()
    {
        chillBgm.Stop();
        hypeBgm.Stop();
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
        player.GetComponent<PlayerHealth>().DestroyAllDamageCounter();
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

        SaveGame();
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

    // Shows a random newspaper issue the next day
    public void ShowRandomNews()
    {
        showRandomNews = true;
    }

    // Uses the newspaper that is shown at the start of the next day
    // and applies the changes to the game
    private void DoProgression(NewsIssue ni)
    {
        foreach (int id in ni.recipesUnlocked)
        {
            PlayerManager.instance.unlockedRecipesList.Add(id);
        }
        foreach (MonsterName mn in ni.enemiesUnlocked)
        {
            PlayerManager.instance.unlockedMonsters.Add(mn);
        }
        if (ni.recipesUnlocked.Length > 0)
        {
            RecipeManager.instance.UpdateUnlockedRecipes();
        }
        if (CreatureDexManager.instance != null)
        {
            CreatureDexManager.instance.UpdateCreatureSlots();
        }
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