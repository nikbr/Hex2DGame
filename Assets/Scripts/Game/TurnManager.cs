using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TurnManager : MonoBehaviour
{
    private int currentTurn = 1;
    private int totalFactions = 0;
    public FactionModel[] factionsModel;
    [SerializeField] public GameActivity gameActivity;
    public Button nextTurn;
    public Text buttonText;

    void Start()
    {
        totalFactions = gameActivity.gameModel.PLAYERS_NUM;
        factionsModel = new FactionModel[totalFactions];
        nextTurn.onClick.AddListener(delegate {
            currentTurn++;
        });
    }

    void Update()
    {
        RunTurn();
    }

    private void RunTurn(){
        buttonText.text = currentTurn.ToString();        
    }
}
