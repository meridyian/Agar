using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicScrollView : MonoBehaviour
{
    
    [SerializeField] private Transform scrollViewContent;

    [SerializeField] private GameObject scoreTextPrefab;
    
    
    public void CreateContent(string playername, float playerSize)
    {
        GameObject newPlayerScore = Instantiate(scoreTextPrefab, scrollViewContent);
        Text scoreText = newPlayerScore.GetComponent<Text>();
        scoreText.text = "name : " + playername + ", Size : " + playerSize ;
        
    }
}
