using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class GameSetter : MonoBehaviour
{
    
    // Class for connecting Network
    
    // Network objects
    [SerializeField] private NetworkRunner networkRunnerPrefab = null;
    private NetworkRunner runnerInstance;
    
    // Data related attributes
    [SerializeField] private PlayerData playerDataPrefab;
    [SerializeField] private InputField userName = null;
    [SerializeField] private Text usernamePlaceHolder = null;
    [SerializeField] private string gameSceneName = null;

    public bool isNameProvided;
    [SerializeField] private Text enterNamePopup;

    
    // ok but can be done with new input system
    public void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            StartSharedSession();
        }
    }
    
    // HANDLE USERNAME AND CALL STARTGAME
    public void StartSharedSession()
    {
       
        var playerData = FindObjectOfType<PlayerData>();
        
        if (playerData == null)
        {
            playerData = Instantiate(playerDataPrefab);
        }
        
        if (string.IsNullOrWhiteSpace(userName.text))
        {
            isNameProvided = false;
            enterNamePopup.gameObject.SetActive(true);
        }
        else
        {
            isNameProvided = true;
            playerData.SetUserName(userName.text);
            StartGame(GameMode.Shared, gameSceneName);
        }

    }
    
    // NETWORK
    private async void StartGame(GameMode mode, string sceneName)
    {
        runnerInstance = FindObjectOfType<NetworkRunner>();
        if (runnerInstance == null)
        {
            runnerInstance = Instantiate(networkRunnerPrefab);
        }

        runnerInstance.ProvideInput = true;
        var startGameArgs = new StartGameArgs
        {
            GameMode = mode
        };
        await runnerInstance.StartGame(startGameArgs);
        runnerInstance.SetActiveScene(sceneName);
    }
    
    // assigned to input field OnValueChanged no need to use in this way but looks pretty 
    public void TogglePopup()
    {
        if (!isNameProvided) 
            enterNamePopup.gameObject.SetActive(false);
    }
}
