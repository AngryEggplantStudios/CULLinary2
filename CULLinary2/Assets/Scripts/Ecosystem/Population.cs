using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population
{
    private MonsterName name;
    private int lowerBound;
    private int upperBound; // to determine if there's overpopulation
    private int optimalNumber;
    private int currentNumber;
    private PopulationLevel level;
    private bool isEnabled = false; // if not enabled, pop level does not increase and monsters do not spawn
    private int dayEnabled = 0; // keep track of the day it was enabled, for checking population increase
    private const float chanceOfOverpopulation = 0.75f;
    private const float overpopulationMultiplier = 1.5f;
    private int numDaysBetweenLevelIncrease = 1; // num days it takes to increase pop level naturally (for endangered, vulnerable and normal (50% chance))
    private int numDaysLeftToIncreaseLevel; // counter to checking next increase in level
    private int numDaysToIncreaseFromExtinct; // num days it takes to increase pop level naturally from extinct
    private bool hasSpawnedMiniboss = false;

    public Population(MonsterName name, int lowerBound, int upperBound, int curr)
    {
        this.name = name;
        this.lowerBound = lowerBound;
        this.upperBound = upperBound;
        this.currentNumber = curr;
        this.optimalNumber = Mathf.RoundToInt(Mathf.Lerp(lowerBound, upperBound, 0.5f));
        this.numDaysLeftToIncreaseLevel = numDaysBetweenLevelIncrease;
        this.numDaysToIncreaseFromExtinct = numDaysBetweenLevelIncrease + 1;
        SetLevelBasedOnCurrentNumber();
    }

    public Population(MonsterName name, int lowerBound, int upperBound, PopulationLevel level)
    {
        this.name = name;
        this.lowerBound = lowerBound;
        this.upperBound = upperBound;
        this.optimalNumber = Mathf.RoundToInt(Mathf.Lerp(lowerBound, upperBound, 0.5f));
        this.numDaysLeftToIncreaseLevel = numDaysBetweenLevelIncrease;
        this.numDaysToIncreaseFromExtinct = numDaysBetweenLevelIncrease + 1;
        SetLevel(level);
        // Debug.Log("current number for " + name + " : " + currentNumber);
    }

    public MonsterName GetName()
    {
        return this.name;
    }

    public int GetCurrentNumber()
    {
        return this.currentNumber;
    }

    public bool IsEnabled()
    {
        return isEnabled;
    }

    public int GetDayEnabled()
    {
        return dayEnabled;
    }

    public void SetEnabled(bool value)
    {
        isEnabled = value;
        dayEnabled = GameTimer.GetDayNumber();
        // if (value) { Debug.Log("enabled population: " + name); }
    }

    private void SetCurrentNumberBasedOnLevel()
    {
        int min = 0;
        int max = 0;
        switch (level)
        {
            case PopulationLevel.Overpopulated:
                currentNumber = Mathf.RoundToInt(upperBound * overpopulationMultiplier);
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
        SetLevelBasedOnCurrentNumber();
    }

    public void DecreaseBy(int value)
    {
        if (currentNumber > 0)
        {
            currentNumber -= value;
            SetLevelBasedOnCurrentNumber();
        }
    }

    public void IncreaseLevel()
    {
        // runs when new day is started and today is the day to increase pop level
        // natural population increase for endangered, vulnerable and normal only
        switch (level)
        {
            case PopulationLevel.Vulnerable:
                SetLevel(PopulationLevel.Normal);
                // Debug.Log("increased pop level for " + name + " to normal");
                break;
            case PopulationLevel.Endangered:
                SetLevel(PopulationLevel.Vulnerable);
                // Debug.Log("increased pop level for " + name + " to vulnerable");
                break;
            case PopulationLevel.Normal:
                // randomly increase to overpopulated
                if (WillBecomeOverpopulated())
                {
                    SetLevel(PopulationLevel.Overpopulated);
                    // Debug.Log("increased pop level for " + name + " to overpopulated");
                }
                break;
            case PopulationLevel.Extinct:
                SetLevel(PopulationLevel.Endangered);
                break;
            case PopulationLevel.Overpopulated:
                // do nothing
                // Debug.Log("pop level remains at " + level + " for " + name);
                break;
        }
    }

    private bool WillBecomeOverpopulated()
    {
        float randomValue = Random.value;
        return randomValue <= chanceOfOverpopulation;
    }

    public void SetLevelBasedOnCurrentNumber()
    {
        if (this.currentNumber > this.upperBound)
        {
            level = PopulationLevel.Overpopulated;
        }
        else if (this.optimalNumber <= this.currentNumber && this.currentNumber <= this.upperBound)
        {
            level = PopulationLevel.Normal;
        }
        else if (this.lowerBound <= this.currentNumber && this.currentNumber < this.optimalNumber)
        {
            level = PopulationLevel.Vulnerable;
        }
        else if (this.currentNumber < this.lowerBound && this.currentNumber != 0)
        {
            level = PopulationLevel.Endangered;
        }
        else
        {
            level = PopulationLevel.Extinct;
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

    public PopulationLevel GetLevel()
    {
        return level;
    }

    private void SetLevel(PopulationLevel level)
    {
        this.level = level;
        SetCurrentNumberBasedOnLevel();
        if (this.level == PopulationLevel.Extinct)
        {
            numDaysLeftToIncreaseLevel = numDaysToIncreaseFromExtinct;
        }
    }

    public void ResetToNormal()
    {
        // was overpopulated, miniboss killed
        SetLevel(PopulationLevel.Normal);
        hasSpawnedMiniboss = false;
    }

    public void SetExtinct()
    {
        SetLevel(PopulationLevel.Extinct);
        Debug.Log(string.Format("{0} went extinct :(", name));
    }

    public void SetNumDaysBetweenLevelIncrease(int numDays)
    {
        numDaysBetweenLevelIncrease = numDays;
        if (this.level != PopulationLevel.Extinct)
        {
            numDaysLeftToIncreaseLevel = numDays;
        }
    }

    public void SetNumDaysToIncreaseFromExtinct(int numDays)
    {
        numDaysToIncreaseFromExtinct = numDays;
        if (this.level == PopulationLevel.Extinct)
        {
            numDaysLeftToIncreaseLevel = numDays;
        }
    }

    public bool HasSpawnedMiniboss()
    {
        return hasSpawnedMiniboss;
    }

    public void SetHasSpawnedMiniboss(bool value)
    {
        hasSpawnedMiniboss = value;
    }

    public int GetBaseSpawningNumber()
    {
        return currentNumber / 2;
    }

}
