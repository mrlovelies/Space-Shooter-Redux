using System;
using System.Collections;
using System.Collections.Generic;
using Proyecto26;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseHandler : MonoBehaviour
{
    
    private const string projectId = "space-shooter-redux-4565b-default-rtdb"; // You can find this in your Firebase project settings
    private static readonly string databaseURL = $"https://{projectId}.firebaseio.com/";
    private static Text _recordScoreMessage;
    private static GameObject _nameInputInteractionsContainer;

    public static void PostScore(Score score)
    {
        string scoreID = Guid.NewGuid().ToString("N");
        _recordScoreMessage = GameObject.FindGameObjectWithTag("Name_Input_Message_Text").GetComponent<Text>();
        if (_recordScoreMessage == null) Debug.LogError("_recordScoreMessage::DatabaseHandler is NULL");
        
        RestClient.Put<Score>($"{databaseURL}scoreboard/{scoreID}.json", score).Then(response =>
        {
            _recordScoreMessage.text = "Your score has been recorded";
            Debug.Log("The score was successfully uploaded");
        });
    }
}
