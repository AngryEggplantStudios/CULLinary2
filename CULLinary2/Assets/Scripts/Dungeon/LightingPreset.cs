using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Scriptables/Lighting Preset", order = 1)]
public class LightingPreset : ScriptableObject
{
    public Gradient AmbientSkyColor;
    public Gradient AmbientEquatorColor;
    public Gradient AmbientGroundColor;
    public Gradient DirectionalColor;
    public Gradient FogColor;

}
