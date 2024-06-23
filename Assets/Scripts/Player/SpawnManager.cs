using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private Transform[] spawnPoints;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Transform GetSpawnPoint1()
    {
        return spawnPoints[0];
    }
    public Transform GetSpawnPoint2()
    {
        return spawnPoints[1];
    }
}