
using UnityEngine;

public static class Utils 
{
    public static Vector3 GetRandomSpawnPosition(float radius)
    {
        return new Vector3(Random.Range(-50, 50), radius/2f, Random.Range(-50,50) );
    }

    
    // to create bots with names
    public static string GetRandomBotName()
    {
        string[] names = { "Eddy", "Freddy", "Paddy", "Buddy", "Herkel" };

        return names[Random.Range(0, names.Length)] + Random.Range(1, 100);
    }
}
