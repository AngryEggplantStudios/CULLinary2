using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcosystemManager : MonoBehaviour
{
    private List<Population> populations = new List<Population> {
        new Population(EnemyName.Potato, 5, 50, 45),
        new Population(EnemyName.Corn, 5, 50, 35),
        new Population(EnemyName.Eggplant, 5, 50, 35)
    };

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public Population GetPopulation(EnemyName enemyName)
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

    public void IncreasePopulation(EnemyName name, int value)
    {
        Population pop = GetPopulation(name);
        if (pop != null)
        {
            pop.IncreaseBy(value);
        }
    }

    public void DecreasePopulation(EnemyName name, int value)
    {
        Population pop = GetPopulation(name);
        if (pop != null)
        {
            pop.DecreaseBy(value);
        }
    }

}
