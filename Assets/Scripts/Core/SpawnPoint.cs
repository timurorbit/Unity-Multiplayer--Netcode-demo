using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> spawnPonts = new List<SpawnPoint>();

    private void OnEnable()
    {
        spawnPonts.Add(this);
    }

    public static Vector3 GetRandomSpawnPos()
    {
        if (spawnPonts.Count == 0)
        {
            return Vector3.zero;
        }
        return spawnPonts[Random.Range(0, spawnPonts.Count)].transform.position;
    }

    private void OnDisable()
    {
        spawnPonts.Remove(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1);
    }
}

