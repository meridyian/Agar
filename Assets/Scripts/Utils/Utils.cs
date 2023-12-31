using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils 
{
    public static Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(Random.Range(0, 30), 0f, Random.Range(0, 30) * 0.7f);
    }



    public static string GetRandomName()
    {
        string[] names = { "Eddy", "Freddy", "Paddy", "Buddy", "Herkel" };

        return names[Random.Range(0, names.Length)] + Random.Range(1, 100);
    }
}
