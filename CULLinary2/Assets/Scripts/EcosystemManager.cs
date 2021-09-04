using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcosystemManager : MonoBehaviour
{
    // to share among all the populations
    [SerializeField] private int numDaysBetweenLevelIncrease = 1; // num days it takes to increase pop level naturally (for endangered, vulnerable and normal (50% chance))
    [SerializeField] private int numDaysToIncreaseFromExtinct = 2; // num days it takes to increase pop level naturally from extinct

    private static List<Population> populations = new List<Population> {
        new Population(EnemyName.Potato, 5, 10, PopulationLevel.Normal),
        new Population(EnemyName.Corn, 5, 10, PopulationLevel.Normal),
        new Population(EnemyName.Eggplant, 5, 10, PopulationLevel.Normal)
    };

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
        GameTimer.OnStartNewDay += CheckNaturalPopulationIncrease;
        foreach (Population pop in populations)
        {
            Debug.Log(string.Format("{0} population level: {1}", pop.GetName(), pop.GetLevel()));
        }
    }

    private void CheckNaturalPopulationIncrease()
    {
        if (GameTimer.GetDayNumber() > 1)
        {
            // don't increase if it's the first day
            foreach (Population pop in populations)
            {
                pop.CheckNaturalPopulationIncrease();
                if (pop.IsOverpopulated() && !pop.HasSpawnedMiniboss())
                {
                    SpawnMiniBoss(pop);
                }
                Debug.Log(string.Format("{0} population level: {1}", pop.GetName(), pop.GetLevel()));
            }

            DungeonSpawnManager.UpdateSpawners();

        }
    }

    private void SpawnMiniBoss(Population pop)
    {
        List<GameObject> spawners = DungeonSpawnManager.GetSpawners(pop.GetName());
        GameObject randomSpawner = spawners[Random.Range(0, spawners.Count)];
        randomSpawner.GetComponent<DungeonSpawn>().SpawnMiniBoss();
        Debug.Log("spawning miniboss for " + pop.GetName() + " at " + randomSpawner.transform.position);
        pop.SetHasSpawnedMiniboss(true);
    }

    public static void ResetPopulationToNormal(EnemyName name)
    {
        Population pop = GetPopulation(name);
        pop.ResetToNormal();
    }

    public static Population GetPopulation(EnemyName enemyName)
    {
        for (int i = 0; i < populations.Count; i++)
        {
            EnemyName populationName = populations[i].GetName();
            if (enemyName == populationName)
            {
                return populations[i];
            }
        }

        return null;
    }

    public static void IncreasePopulation(EnemyName name, int value)
    {
        Population pop = GetPopulation(name);
        if (pop != null)
        {
            pop.IncreaseBy(value);
        }
    }

    public static void DecreasePopulation(EnemyName name, int value)
    {
        Population pop = GetPopulation(name);
        if (pop != null)
        {
            pop.DecreaseBy(value);
        }
    }

    public static void SetExtinct(EnemyName name)
    {
        Population pop = GetPopulation(name);
        pop.SetExtinct();
    }

}
