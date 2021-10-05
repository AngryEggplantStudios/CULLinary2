using System;
using UnityEngine;

using SRand = System.Random;

// A script to vary the colour of a monster randomly
public class MonsterVaryColour : MonoBehaviour
{
    public Renderer monsterRenderer;
    // Custom colours for the tint
    public Color[] tintColours;

    private static SRand rand = new SRand();

    public void Awake()
    {
        GenerateRandomColour();
    }

    public void GenerateRandomColour()
    {
        if (tintColours == null || tintColours.Length == 0)
        {
            Debug.Log("MonsterVaryColour: No tints applied.");
            return;
        }

        Color original = monsterRenderer.material.color;
        Color tint = tintColours[rand.Next(tintColours.Length)];
        Color newColour =  Color.Lerp(original, tint, (float) rand.NextDouble());
        monsterRenderer.material.SetColor("_BaseColor", newColour);
    }
}
