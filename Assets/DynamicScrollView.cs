using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class DynamicScrollView : NetworkBehaviour
{
    
    [SerializeField] private Transform scrollViewContent;

    [SerializeField] private GameObject joinedTextPrefab;
    
    
    public void CreateContent(string playername)
    {
        GameObject newPlayerJoined = Instantiate(joinedTextPrefab, scrollViewContent);
        Text joinedText = newPlayerJoined.GetComponent<Text>();
        joinedText.text =  playername + " is in the housee !!" ;
        
    }
}
