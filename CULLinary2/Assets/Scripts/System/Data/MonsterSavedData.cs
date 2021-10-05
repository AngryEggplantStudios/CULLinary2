[System.Serializable]
public class MonsterSavedData
{
    public MonsterName monsterName;
    public PopulationLevel populationLevel;

    public MonsterSavedData(MonsterName monsterName, PopulationLevel populationLevel)
    {
        this.monsterName = monsterName;
        this.populationLevel = populationLevel;
    }
}