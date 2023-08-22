using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataNetworked : NetworkBehaviour
{
    [Networked(OnChanged = nameof(UsernameChanged))]
    public string UserName { get;  set; }
    
    public Text _playernameEntryText;
    public static PlayerDataNetworked NetworkedDataInstance;

    private void Awake()
    {
        if (NetworkedDataInstance == null)
        {
            NetworkedDataInstance = this;
        }
    }


    public override void Spawned()
    {
        _playernameEntryText.text = UserName;
        Debug.Log(UserName + "joined outside haststate authority");
        if (Object.HasStateAuthority)
        {
            var userName = FindObjectOfType<PlayerData>().GetUserName();
            DealNameRpc(userName);
            _playernameEntryText.text = UserName;
            Debug.Log(UserName + "joined  haststate authority");
        }

    }
    private static void UsernameChanged(Changed<PlayerDataNetworked> changed)
    {
        changed.Behaviour.UserName = changed.Behaviour._playernameEntryText.text;
        Debug.Log(changed.Behaviour.UserName + "joined from networked data");
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealNameRpc(string name)
    {
        UserName = name;
    }

}
