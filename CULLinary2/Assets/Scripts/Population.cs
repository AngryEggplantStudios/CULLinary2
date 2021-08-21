using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population
{
    private EnemyName name;
    private int lowerBound;
    private int upperBound; // to determine if there's overpopulation
    private int optimalLevel;
    private int current;

    public Population(EnemyName name, int lowerBound, int upperBound, int curr)
    {
        this.name = name;
        this.lowerBound = lowerBound;
        this.upperBound = upperBound;
        this.current = curr;
        this.optimalLevel = lowerBound + Mathf.RoundToInt((upperBound - lowerBound) / 2);
    }

    public EnemyName GetName()
    {
        return this.name;
    }

    public int GetCurrentNumber()
    {
        return this.current;
    }

    public void DecreaseBy(int value)
    {
        this.current -= value;
        if (this.current < 0)
        {
            this.current = 0;
        }
    }

    public void IncreaseBy(int value)
    {
        this.current += value;
    }

    // population level category? eg Overpopulated, Normal, Vulnerable, Endangered, Extinct?
    public PopulationCategory GetCategory()
    {
        if (this.current > this.upperBound)
        {
            return PopulationCategory.Overpopulated;
        }
        else if (this.optimalLevel <= this.current && this.current <= this.upperBound)
        {
            return PopulationCategory.Normal;
        }
        else if (this.lowerBound <= this.current && this.current < this.optimalLevel)
        {
            return PopulationCategory.Vulnerable;
        }
        else if (this.current < this.lowerBound && this.current != 0)
        {
            return PopulationCategory.Endangered;
        }
        else
        {
            return PopulationCategory.Extinct;
        }

    }

    public bool IsOverpopulated()
    {
        return GetCategory() == PopulationCategory.Overpopulated;
    }

}

