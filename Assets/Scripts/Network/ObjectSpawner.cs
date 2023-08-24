using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;
using Fusion;

public class ObjectSpawner : SimulationBehaviour, ISpawned
{
    // class to manage spawn processes in scene
    
    // prefabs to spawn
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject BotPrefab;
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private GameObject obstaclePrefab;
    
    // bot attributes
    private const int desiredNumberofBots = 7;
    
    // control attributes
    private bool isFoodSpawned = false;
    private bool isObstacleSpawned = false;
    private bool isBotSpawned = false;

    
    // to hold spawned bots
    public List<NetworkObject> botsList = new List<NetworkObject>();
    
    
    // spawn foods, you might make them static later on

    public void SpawnFood()
    {
        for (int i = 0; i < 350; i++)
        {
            NetworkObject spawnedFood = Runner.Spawn(foodPrefab, Utils.GetRandomSpawnPosition(foodPrefab.transform.localScale.x) , Quaternion.identity);
        }

        isFoodSpawned = true;
    }
    
    public void SpawnObstacle()
    {
        for (int i = 0; i < 150; i++)
        {

            NetworkObject spawnedFood = Runner.Spawn(obstaclePrefab, Utils.GetRandomSpawnPosition(obstaclePrefab.transform.localScale.x) , Quaternion.identity);
        }

        isObstacleSpawned = true;
    }
    
    // check the current number of players you have, if it is not enough spawn bots
    public void SpawnBots()
    {
        int numberOfBotsToSpawn = desiredNumberofBots - Runner.SessionInfo.PlayerCount - botsList.Count;
        for (int i = 0; i < numberOfBotsToSpawn; i++)
        {
            NetworkObject spawnedBots = Runner.Spawn(BotPrefab, Utils.GetRandomSpawnPosition(BotPrefab.transform.localScale.x), Quaternion.identity);
            Debug.Log("username : " + Utils.GetRandomBotName());
        }

        isBotSpawned = true;
    }

    
    /*
    public void InitializeBeforeBotSpawn(NetworkRunner runner, NetworkObject networkObject)
    {
        //networkObject.GetComponent<PlayerStateController>().isBot = true;
    }
    */
    
    public void Spawned()
    {
        SpawnPlayer(Runner.LocalPlayer);
        
    }

    public void SpawnPlayer(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(playerPrefab, new Vector3(0, 0.5f, 0), Quaternion.identity);
            
            if(!isFoodSpawned)
                SpawnFood();
            if(!isObstacleSpawned)
                SpawnObstacle();
            if(!isBotSpawned)
                SpawnBots();
        }
    }
}
