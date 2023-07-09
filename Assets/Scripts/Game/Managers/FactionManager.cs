using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactionManager : MonoBehaviour
{

         private GameActivity gameActivity;

    public int totalFactions = 0;
    public FactionModel[] factionsModel;
    public int humanPlayerID = 0;
    public Text woodText;
    public Text oreText;
    public Text goldText;
    public Text gemsText;
    public Text mercuryText;
    public Text sulfurText;


    void Awake(){
        gameActivity = GameObject.FindObjectOfType<GameActivity>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Init factions");
            InitializeFactions();
            UpdateResourcesView();
          
        

    }

    private void InitializeFactions(){
        totalFactions = gameActivity.gameModel.PLAYERS_NUM;
        factionsModel = new FactionModel[totalFactions];
        for(int id = 0; id < factionsModel.Length; id++){
            factionsModel[id] = new FactionModel(id, gameActivity);
        }
        factionsModel[humanPlayerID].SetToHuman();
    }

    private void UpdateResourcesView(){
        woodText.text = "Wood: " + factionsModel[humanPlayerID].wood;
        oreText.text = "Ore: " + factionsModel[humanPlayerID].ore;
        goldText.text = "Gold: " + factionsModel[humanPlayerID].gold;
        gemsText.text = "Gems: " + factionsModel[humanPlayerID].gems;
        mercuryText.text = "Mercury: " + factionsModel[humanPlayerID].mercury;
        sulfurText.text = "Sulfur: " + factionsModel[humanPlayerID].sulfur;
    }

    public void RunTurn(){
        for(int id = 0;id<totalFactions;id++){
            factionsModel[id].ExecuteTurn(gameActivity);
        }
        UpdateResourcesView();
        //Debug.Log("BRUHG "+objectsThatFinishedTurn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
