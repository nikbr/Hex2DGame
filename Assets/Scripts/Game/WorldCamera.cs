using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCamera : Observer
{
    public WorldCamera(GameActivity context){

    }


    public void Update(GameActivity context){
        Camera.main.transform.position = context.gameModel.GameCameraPos;
        context.UpdateCameraCornerPositions();
    }
}
