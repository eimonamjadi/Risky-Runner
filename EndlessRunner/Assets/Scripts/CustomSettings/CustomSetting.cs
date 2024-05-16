using UnityEngine;

[CreateAssetMenu(fileName = "New_CustomSettings", menuName = "CustomeSetting")]
public class CustomSetting : ScriptableObject
{
    [HideInInspector] public static int speedCount = 5;
    public CustomSettingData Data;
}

[System.Serializable]
public class CustomSettingData
{
    public CustomSettingData(bool assigned)
    {
        IsAssigned = assigned;
    }
    /// <summary>
    /// Default the 'IsAssigned' field to true
    /// </summary>
    /// <param name="speeds"></param>
    /// <param name="rep"></param>
    /// <param name="noise"></param>
    /// <param name="obstacles"></param>
    public CustomSettingData(int[] speeds, int rep, float noise, int obstacles)
    {
        IsAssigned = true;
        Speeds = speeds;
        NumRepetitions = rep;
        NoiseLevel = noise;
        MaxObstacles = obstacles;
    }

    public int[] Speeds;
    public int NumRepetitions;
    [Range(0f, 100f)] public float NoiseLevel;
    [Min(1)] public int MaxObstacles;
    public bool IsAssigned = false;
}