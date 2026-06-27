using UnityEngine;

[System.Serializable]
public class EnemySpawnConfig
{
    public EnemyData EnemyConfig; 
    public int Count;
}

[CreateAssetMenu(fileName = "NewWave", menuName = "Spawner/WaveData")]
public class WaveData : ScriptableObject
{
    public string WaveName = "Волна 1";
    public EnemySpawnConfig[] EnemiesInWave; 
    public float SpawnInterval = 1f;  
    public float Duration = 60f;
}
