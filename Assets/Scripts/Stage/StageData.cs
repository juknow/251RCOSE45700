using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Stage/StageData", order = 1)]
public class StageData : ScriptableObject
{
    public string stageName;            // ¿¹: "Stage 1"
    public StageType stageType;         // Normal or Boss
    public int totalWaves;
    public float waveInterval;
    public int enemiesPerWave;
    public GameObject[] enemyPrefabs;
}