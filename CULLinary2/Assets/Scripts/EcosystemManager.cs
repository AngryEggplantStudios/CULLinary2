using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcosystemManager : MonoBehaviour
{
    private static List<Population> populations = new List<Population> {
        new Population(EnemyName.Potato, 5, 20, PopulationLevel.Endangered),
        new Population(EnemyName.Corn, 5, 20, PopulationLevel.Endangered),
        new Population(EnemyName.Eggplant, 5, 20, PopulationLevel.Endangered)
    };

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        GameTimer.OnStartNewDay += CheckNaturalPopulationIncrease;
    }

    private void CheckNaturalPopulationIncrease()
    {
        if (GameTimer.GetDayNumber() > 1)
        {
            // don't increase if it's the first day
            foreach (Population pop in populations)
            {
                pop.CheckNaturalPopulationIncrease();
            }
            DungeonSpawnManager.UpdateLocalSpawnCap();
        }
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

}
