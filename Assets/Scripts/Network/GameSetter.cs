using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GameSetter : MonoBehaviour
{
    [SerializeField] private NetworkRunner networkRunnerPrefab = null;
    //[SerializeField] private PlayerData playerDataPrefab;

    //[SerializeField] private InputField userName = null;
    //[SerializeField] private Text usernamePlaceHolder = null;

    [SerializeField] private string gameSceneName = null;

    private NetworkRunner runnerInstance;
    public GameObject joinButton;



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

    public void StartSharedSession()
    {
        StartGame(GameMode.Shared, gameSceneName);
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
        //The await keyword suggests that this method call might involve asynchronous operations
        await runnerInstance.StartGame(startGameArgs);
        runnerInstance.SetActiveScene(sceneName);
    }
}
