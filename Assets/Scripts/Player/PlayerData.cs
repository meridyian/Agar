using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public string userName;

    public static PlayerData Instance;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


    public void SetUserName(string _userName)
    {
        userName = _userName;
    }

    public string GetUserName()
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return GetRandomUserName();
        }
        return userName;
    }

    public string GetRandomUserName()
    {
        var rngPlayerNumber = Random.Range(0, 7777);
        return $"Player {rngPlayerNumber.ToString("0000")}";
    }
    




}
