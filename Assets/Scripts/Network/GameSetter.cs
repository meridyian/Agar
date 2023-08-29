using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    public GameObject joinButton;
    public bool isNameProvided;

    [SerializeField] private Text enterNamePopup;

    
    public void Start()
    {
        // call the function that tracks join button
        ListenJoinButton();
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            StartSharedSession();
        }
    }

    public void ListenJoinButton()
    {
        //adds a listener to the onClick event of the Button component. When the button is clicked,
        //the code within the lambda expression (() => StartSharedSession()) will be executed.
        joinButton.GetComponent<Button>().onClick.AddListener((() => StartSharedSession()));
    }
    
    
    
    public void SetPlayerData()
    {
        // saves the userName data provided by user to playerData object
        
        var playerData = FindObjectOfType<PlayerData>();
        if (playerData == null)
        {
            playerData = Instantiate(playerDataPrefab);
        }

        if (string.IsNullOrWhiteSpace(userName.text))
        {
            isNameProvided = false;
        }
        else
        {
            isNameProvided = true;
            playerData.SetUserName(userName.text);

        }
    }
    

    public void StartSharedSession()
    {
        SetPlayerData();
        if(isNameProvided)
            StartGame(GameMode.Shared, gameSceneName);
        else
        {
            enterNamePopup.gameObject.SetActive(true);
        }

    }

    private async void StartGame(GameMode mode, string sceneName)
    {
        runnerInstance = FindObjectOfType<NetworkRunner>();
        if (runnerInstance == null)
        {
            runnerInstance = Instantiate(networkRunnerPrefab);
        }

        runnerInstance.ProvideInput = true;

        //Here, a new instance of StartGameArgs is created
        var startGameArgs = new StartGameArgs
        {
            GameMode = mode
        };
        
        //The await keyword suggests that this method call might involve asynchronous operations those waiting
        //for another functions to be executed, for safety reasons (waiting the fusion to be connected)
        await runnerInstance.StartGame(startGameArgs);
        runnerInstance.SetActiveScene(sceneName);
    }

    
    // assign it to input field
    public void TogglePopup()
    {
        if (!isNameProvided) 
            enterNamePopup.gameObject.SetActive(false);

    }
}
