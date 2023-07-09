using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TurnManager : MonoBehaviour
{
    public Button nextTurn;
    public Text buttonText;
    private bool running=false;
        public int totalTurnObjects=1;

    private FactionManager factionManager;
    private GameActivity gameActivity;
    private PartyManager partyManager;
    
    void Awake(){
        gameActivity = GameObject.FindObjectOfType<GameActivity>();
        factionManager = GameObject.FindObjectOfType<FactionManager>();
        partyManager = GameObject.FindObjectOfType<PartyManager>();
        
    }

    void Start()
    {

        nextTurn.onClick.AddListener(delegate {
             if (gameActivity.objectsThatFinishedTurn==0){
              //  Debug.Log("Running Turn" );
                factionManager.RunTurn();
                partyManager.RunTurn();
                totalTurnObjects = factionManager.totalFactions + partyManager.TotalParties();
            }
        });
        

       // Debug.Log("totalFactions " + totalFactions + " totalTurnObjects " + totalTurnObjects);
        buttonText.text = gameActivity.currentTurn.ToString();
        Debug.Log("Total factions check " + factionManager.totalFactions);
        totalTurnObjects = factionManager.totalFactions + partyManager.TotalParties();
        
    }

    

    void Update()
    {
      //  Debug.Log("objects that finished " + objectsThatFinishedTurn + " totalTurnObjects " + totalTurnObjects);
        if(gameActivity.objectsThatFinishedTurn>=totalTurnObjects){
                gameActivity.objectsThatFinishedTurn=0;
                gameActivity.currentTurn++;
                buttonText.text = gameActivity.currentTurn.ToString(); 
               // Debug.Log("Changing turn");
            }
    }

    

    
}
