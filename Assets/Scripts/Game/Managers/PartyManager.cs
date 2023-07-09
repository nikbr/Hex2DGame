using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
            private GameActivity gameActivity;
            private FactionManager factionManager;
            public Dictionary<int, IParty> warbandsModel = new Dictionary<int, IParty>();
            public Dictionary<int, IParty> townsModel = new Dictionary<int, IParty>();
            public Queue<int> destroyedPartyIds = new Queue<int>();
            public int nextId = 0;

    void Awake(){
        gameActivity = GameObject.FindObjectOfType<GameActivity>();
        factionManager = GameObject.FindObjectOfType<FactionManager>();
    }


    // Start is called before the first frame update
    void Start()
    {
        GiveEachFactionStartingParties();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int TotalParties(){
        return warbandsModel.Count + townsModel.Count;
    }

    public void RunTurn(){
        foreach (KeyValuePair<int, IParty> warbandModel in warbandsModel)
        {
            warbandModel.Value.ExecuteTurn(gameActivity);
        }
        foreach (KeyValuePair<int, IParty> townModel in townsModel)
        {
            townModel.Value.ExecuteTurn(gameActivity);
        }
        
    }

    private void GiveEachFactionStartingParties(){
        for(int id = 0;id<factionManager.factionsModel.Length;id++){
            CreateTownForFaction(id);
            CreateWarbandForFaction(id);
           
        }
    }

    private void CreateTownForFaction(int factionId){
        int id = ChooseNextPartyId();
        townsModel.Add(id, new TownModel(id, factionId, gameActivity));
        factionManager.factionsModel[factionId].parties.Add(id);

        // Debug.Log("Created town " + id + " for faction " + factionId);
    }

    private void CreateWarbandForFaction(int factionId){
        int id = ChooseNextPartyId();
        warbandsModel.Add(id, new WarbandModel(id, factionId, gameActivity));
        factionManager.factionsModel[factionId].parties.Add(id);
       // Debug.Log("Created warband " + id + " for faction " + factionId);
    }

    private int ChooseNextPartyId(){
        if(destroyedPartyIds.Count==0){
            int result = nextId;
            nextId++;
            return result;
        }else{
            return destroyedPartyIds.Dequeue();
        }
    }
}
