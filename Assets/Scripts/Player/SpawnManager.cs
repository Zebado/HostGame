using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private Transform[] spawnPointsforPlayer1, spawnPointsforPlayer2;

    private int levelIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void StartNewLevel()
    {
        levelIndex++;
    }

    public Transform GetSpawnPoint1()
    {
        return spawnPointsforPlayer1[levelIndex];
    }
    public Transform GetSpawnPoint2()
    {
        return spawnPointsforPlayer2[levelIndex];
    }
}