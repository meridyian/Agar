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
    //[SerializeField] private GameObject obstaclePrefab;
    
    // bot attributes
    private const int desiredNumberofPlayers = 3;
    
    // control attributes
    private bool isFoodSpawned = false;
    private bool isObstacleSpawned = false;
    
    // to hold spawned bots
    public List<NetworkObject> botsList = new List<NetworkObject>();
    
    
    
    // spawn foods, you might make them static later on

    public void SpawnFood()
    {
        for (int i = 0; i < 100; i++)
        {
            // belki bi gameobje altınd atoplarsın sonra
            NetworkObject spawnedFood = Runner.Spawn(foodPrefab, Utils.GetRandomSpawnPosition(), Quaternion.identity);
            //spawnedFood.GetComponent<Rigidbody>().isKinematic = true;
            spawnedFood.transform.position = Utils.GetRandomSpawnPosition();
        }

        isFoodSpawned = true;
    }
    
    // check the current number of players you have, if it is not enough spawn bots

    public void SpawnBots()
    {
        int numberOfBotsToSpawn = desiredNumberofPlayers - Runner.SessionInfo.PlayerCount - botsList.Count;
        for (int i = 0; i < numberOfBotsToSpawn; i++)
        {
            NetworkObject spawnedBots = Runner.Spawn(BotPrefab, Utils.GetRandomSpawnPosition(), Quaternion.identity,null, InitializeBeforeBotSpawn);
        }
    }

    


    public void InitializeBeforeBotSpawn(NetworkRunner runner, NetworkObject networkObject)
    {
        networkObject.GetComponent<PlayerStateController>().isBot = true;
    }
    
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
        }
    }
}
