using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public Button playButton;
   public void Start(){
      Debug.Log("started");
      Debug.Log(playButton);
      playButton.onClick.AddListener(delegate{
         Loader.Load(Loader.Scene.GameMap);
      });

   }
}