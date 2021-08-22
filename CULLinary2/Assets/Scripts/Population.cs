using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population
{
    private EnemyName name;
    private int lowerBound;
    private int upperBound; // to determine if there's overpopulation
    private int optimalNumber;
    private int currentNumber;
    private PopulationLevel level;
    private const float chanceOfOverpopulation = 0.5f;
    private const int numDaysBetweenLevelIncrease = 1;
    private int numDaysLeftToIncreaseLevel = numDaysBetweenLevelIncrease;

    public Population(EnemyName name, int lowerBound, int upperBound, int curr)
    {
        this.name = name;
        this.lowerBound = lowerBound;
        this.upperBound = upperBound;
        this.currentNumber = curr;
        this.optimalNumber = Mathf.RoundToInt(Mathf.Lerp(lowerBound, upperBound, 0.5f));
        this.level = GetLevel();
    }

    public Population(EnemyName name, int lowerBound, int upperBound, PopulationLevel level)
    {
        this.name = name;
        this.lowerBound = lowerBound;
        this.upperBound = upperBound;
        this.optimalNumber = Mathf.RoundToInt(Mathf.Lerp(lowerBound, upperBound, 0.5f));
        this.level = level;
        SetCurrentNumberBasedOnLevel();
        // Debug.Log("current number for " + name + " : " + currentNumber);
    }

    public EnemyName GetName()
    {
        return this.name;
    }

    public int GetCurrentNumber()
    {
        return this.currentNumber;
    }

    private void SetCurrentNumberBasedOnLevel()
    {
        int min = 0;
        int max = 0;

        switch (level)
        {
            case PopulationLevel.Overpopulated:
                currentNumber = Mathf.RoundToInt(upperBound * 1.5f); // how much more to increase?
                break;
            case PopulationLevel.Normal:
                min = this.optimalNumber;
                max = this.upperBound;
                currentNumber = Mathf.RoundToInt(Mathf.Lerp(min, max, 0.5f));
                break;
            case PopulationLevel.Vulnerable:
                min = this.lowerBound;
                max = this.optimalNumber - 1;
                currentNumber = Mathf.RoundToInt(Mathf.Lerp(min, max, 0.5f));
                break;
            case PopulationLevel.Endangered:
                min = 1;
                max = this.lowerBound - 1;
                currentNumber = Mathf.RoundToInt(Mathf.Lerp(min, max, 0.5f));
                break;
            case PopulationLevel.Extinct:
                currentNumber = 0;
                break;
        }
    }

    public void IncreaseBy(int value)
    {
        this.currentNumber += value;
    }

    public void DecreaseBy(int value)
    {
        this.currentNumber -= value;
        if (this.currentNumber < 0)
        {
            this.currentNumber = 0;
        }
    }

    public void IncreaseLevel()
    {
        // runs when new day is started and today is the day to increase pop level
        // natural population increase for endangered, vulnerable and normal only
        switch (level)
        {
            case PopulationLevel.Vulnerable:
                level = PopulationLevel.Normal;
                SetCurrentNumberBasedOnLevel();
                // Debug.Log("increased pop level for " + name + " to normal");
                break;
            case PopulationLevel.Endangered:
                level = PopulationLevel.Vulnerable;
                SetCurrentNumberBasedOnLevel();
                // Debug.Log("increased pop level for " + name + " to vulnerable");
                break;
            case PopulationLevel.Normal:
                // randomly increase to overpopulated
                if (WillBecomeOverpopulated())
                {
                    level = PopulationLevel.Overpopulated;
                    SetCurrentNumberBasedOnLevel();
                    // Debug.Log("increased pop level for " + name + " to overpopulated");
                }
                else
                {
                    // Debug.Log("did not increase pop level to overpopulated for " + name);
                }
                break;
            case PopulationLevel.Overpopulated:
            case PopulationLevel.Extinct:
                // do nothing
                // Debug.Log("pop level remains at " + level + " for " + name);
                break;
        }
    }

    private bool WillBecomeOverpopulated()
    {
        float randomValue = Random.value;
        return randomValue >= chanceOfOverpopulation;
    }

    public PopulationLevel GetLevel()
    {
        if (this.currentNumber > this.upperBound)
        {
            return PopulationLevel.Overpopulated;
        }
        else if (this.optimalNumber <= this.currentNumber && this.currentNumber <= this.upperBound)
        {
            return PopulationLevel.Normal;
        }
        else if (this.lowerBound <= this.currentNumber && this.currentNumber < this.optimalNumber)
        {
            return PopulationLevel.Vulnerable;
        }
        else if (this.currentNumber < this.lowerBound && this.currentNumber != 0)
        {
            return PopulationLevel.Endangered;
        }
        else
        {
            return PopulationLevel.Extinct;
        }
    }

    public bool IsOverpopulated()
    {
        return GetLevel() == PopulationLevel.Overpopulated;
    }

    public void CheckNaturalPopulationIncrease()
    {
        // invoked when new day is started
        numDaysLeftToIncreaseLevel--;

        if (numDaysLeftToIncreaseLevel == 0)
        {
            IncreaseLevel();
            numDaysLeftToIncreaseLevel = numDaysBetweenLevelIncrease;
        }
    }

    public void ResetToNormal()
    {
        this.level = PopulationLevel.Normal;
        SetCurrentNumberBasedOnLevel();
    }

}
