using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Newspaper Issue", menuName = "Newspaper/Issues")]
public class NewsIssue : ScriptableObject
{
    public int issueId;
    public string headlines;
    public string subhead;
    public Sprite mainImage;
    public Sprite imageBackground;

    public int[] recipesUnlocked;
    public MonsterName[] enemiesUnlocked;
}
