using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcosystemManager : SingletonGeneric<EcosystemManager>
{
    // to share among all the populations
    [SerializeField] private int numDaysBetweenLevelIncrease = 1; // num days it takes to increase pop level naturally (for endangered, vulnerable and normal (50% chance))
    [SerializeField] private int numDaysToIncreaseFromExtinct = 2; // num days it takes to increase pop level naturally from extinct
    [Header("Enable populations for testing?")]
    [SerializeField] private bool enableAllPopAtStart = true;
    private static List<Population> populations = new List<Population>();

    // Statistics for number of monsters killed
    private static int numOfMonstersKilledToday = 0;

    public void InstantiateEcosystem()
    {
        List<MonsterData> monsterList = DatabaseLoader.GetAllMonsters();
        foreach (MonsterData monsterData in monsterList)
        {
            // Debug.Log("instantiating ecosystem with " + monsterData.monsterName);
            populations.Add(new Population(monsterData.monsterName, monsterData.lowerBound, monsterData.upperBound, PlayerManager.instance.GetPopulationLevelByMonsterName(monsterData.monsterName)));
        }

        foreach (Population pop in populations)
        {
            pop.SetNumDaysBetweenLevelIncrease(numDaysBetweenLevelIncrease);
            pop.SetNumDaysToIncreaseFromExtinct(numDaysToIncreaseFromExtinct);
            pop.SetEnabled(enableAllPopAtStart);
            // Debug.Log(string.Format("instantiate ecosystem: {0} population level: {1} ({2})", pop.GetName(), pop.GetLevel(), pop.GetCurrentNumber()));
        }

        GameTimer.OnStartNewDay += CheckNaturalPopulationIncrease;
        GameTimer.OnStartNewDay += () =>
        {
            EcosystemManager.numOfMonstersKilledToday = 0;
        };
    }

    public static void SaveEcosystemPopulation()
    {
        foreach (Population population in populations)
        {
            PlayerManager.instance.SetPopulationLevelByMonsterName(population.GetName(), population.GetLevel());
        }
    }

    private void CheckNaturalPopulationIncrease()
    {
        if (GameTimer.GetDayNumber() > 1)
        {
            // don't increase if it's the first day
            foreach (Population pop in populations)
            {
                if (!pop.IsEnabled()) // Don't increase population level if is disabled
                {
                    Debug.Log(pop.GetName() + " is not enabled");
                    continue;
                }
                pop.CheckNaturalPopulationIncrease();
                Debug.Log(string.Format("check pop increase: {0} population level: {1} ({2})", pop.GetName(), pop.GetLevel(), pop.GetCurrentNumber()));
                if (pop.IsOverpopulated() && !pop.HasSpawnedMiniboss())
                {
                    SpawnMiniBoss(pop);
                }
            }

            // DungeonSpawnManager.UpdateSpawners();
        }
    }

    private void SpawnMiniBoss(Population pop)
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        GameObject nearestSpawner = DungeonSpawnManager.instance.GetSpawnerNearestTo(player.position, pop.GetName());
        if (nearestSpawner)
        {
            nearestSpawner.GetComponent<MonsterSpawn>().SpawnMiniBoss();
            pop.SetHasSpawnedMiniboss(true);
            Debug.Log("spawning miniboss for " + pop.GetName() + " at " + nearestSpawner.transform.position);
        }
    }

    public static void OnMiniBossDeath(MonsterName name)
    {
        Population pop = GetPopulation(name);
        if (pop.IsOverpopulated())
        {
            Debug.Log("resetting population to normal for " + name);
            pop.ResetToNormal();
        }
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

    public static void EnablePopulation(MonsterName name)
    {
        // Enable population to let its population increase naturally and monsters to spawn
        Population pop = GetPopulation(name);
        pop.SetEnabled(true);
    }

    public static bool GetIsEnabled(MonsterName name)
    {
        Population pop = GetPopulation(name);
        return pop.IsEnabled();
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
