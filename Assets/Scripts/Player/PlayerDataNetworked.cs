using System.Collections;
using System.Collections.Generic;
using Fusion;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataNetworked : NetworkBehaviour
{
    // Network data class which takes the PlayerData input provided in Entry scene and registers user 
    // with the username provided
    
    [Networked(OnChanged = nameof(UsernameChanged))]
    public string UserName { get;  set; }
    
    
    public Text _playernameEntryText;
    public static PlayerDataNetworked NetworkedDataInstance;


    [SerializeField] private DynamicScrollView _dynamicScrollView;
    
    

    private void Awake()
    {
        if (NetworkedDataInstance == null)
        {
            NetworkedDataInstance = this;
        }

    }


    public override void Spawned()
    {
        // return each joined player's name
        
        if (Object.HasStateAuthority)
        {
            _dynamicScrollView = FindObjectOfType<DynamicScrollView>();
            var userName = FindObjectOfType<PlayerData>().GetUserName();
            DealNameRpc(userName);
            _playernameEntryText.text = UserName;
            _dynamicScrollView.CreateContent(UserName, PlayerStateController.StateInstance.playerScore);
            Debug.Log(UserName + "joined from state authority");
        }

    }
    private static void UsernameChanged(Changed<PlayerDataNetworked> changed)
    {
        changed.Behaviour._playernameEntryText.text = changed.Behaviour.UserName;
    }
    
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealNameRpc(string name)
    {
        UserName = name;
    }

}
