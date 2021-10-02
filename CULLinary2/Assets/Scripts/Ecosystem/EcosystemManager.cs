using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcosystemManager : MonoBehaviour
{
    // to share among all the populations
    [SerializeField] private int numDaysBetweenLevelIncrease = 1; // num days it takes to increase pop level naturally (for endangered, vulnerable and normal (50% chance))
    [SerializeField] private int numDaysToIncreaseFromExtinct = 2; // num days it takes to increase pop level naturally from extinct

    private static List<Population> populations = new List<Population> {
        new Population(MonsterName.Potato, 20, 40, PopulationLevel.Normal),
        new Population(MonsterName.Corn, 20, 40, PopulationLevel.Normal),
        new Population(MonsterName.Eggplant, 30, 60, PopulationLevel.Normal)
    };

    // Statistics for number of monsters killed
    private static int numOfMonstersKilledToday = 0;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        foreach (Population pop in populations)
        {
            pop.SetNumDaysBetweenLevelIncrease(numDaysBetweenLevelIncrease);
            pop.SetNumDaysToIncreaseFromExtinct(numDaysToIncreaseFromExtinct);
        }
    }

    void Start()
    {
        foreach (Population pop in populations)
        {
            Debug.Log(string.Format("{0} population level: {1} ({2})", pop.GetName(), pop.GetLevel(), pop.GetCurrentNumber()));
        }
        GameTimer.OnStartNewDay += CheckNaturalPopulationIncrease;
        GameTimer.OnStartNewDay += () => {
            EcosystemManager.numOfMonstersKilledToday = 0;
        };
    }

    private void CheckNaturalPopulationIncrease()
    {
        if (GameTimer.GetDayNumber() > 1)
        {
            // don't increase if it's the first day
            foreach (Population pop in populations)
            {
                pop.CheckNaturalPopulationIncrease();
                Debug.Log(string.Format("{0} population level: {1} ({2})", pop.GetName(), pop.GetLevel(), pop.GetCurrentNumber()));
                if (pop.IsOverpopulated() && !pop.HasSpawnedMiniboss())
                {
                    SpawnMiniBoss(pop);
                }
            }

            DungeonSpawnManager.UpdateSpawners();
        }
    }

    private void SpawnMiniBoss(Population pop)
    {
        List<GameObject> spawners = DungeonSpawnManager.GetSpawners(pop.GetName());
        if (spawners.Count > 0)
        {
            GameObject randomSpawner = spawners[Random.Range(0, spawners.Count)];
            randomSpawner.GetComponent<MonsterSpawn>().SpawnMiniBoss();
            // Debug.Log("spawning miniboss for " + pop.GetName() + " at " + randomSpawner.transform.position);
            pop.SetHasSpawnedMiniboss(true);
        }
    }

    public static void ResetPopulationToNormal(MonsterName name)
    {
        Population pop = GetPopulation(name);
        pop.ResetToNormal();
    }

    public static Population GetPopulation(MonsterName monsterName)
    {
        for (int i = 0; i < populations.Count; i++)
        {
            MonsterName populationName = populations[i].GetName();
            if (monsterName == populationName)
            {
                return populations[i];
            }
        }

        return null;
    }

    public static void IncreasePopulation(MonsterName name, int value)
    {
        Population pop = GetPopulation(name);
        if (pop != null)
        {
            pop.IncreaseBy(value);
        }
    }

    public static void DecreasePopulation(MonsterName name, int value)
    {
        EcosystemManager.numOfMonstersKilledToday += value;
        Population pop = GetPopulation(name);
        if (pop != null)
        {
            pop.DecreaseBy(value);
        }
    }

    public static void SetExtinct(MonsterName name)
    {
        Population pop = GetPopulation(name);
        pop.SetExtinct();
    }
    
    // Gets the amount of monsters killed today so far
    public static int GetNumberOfMonstersKilledToday()
    {
        return EcosystemManager.numOfMonstersKilledToday;
    }

    public void OnDestroy()
    {
        GameTimer.OnStartNewDay -= CheckNaturalPopulationIncrease;
    }

}
