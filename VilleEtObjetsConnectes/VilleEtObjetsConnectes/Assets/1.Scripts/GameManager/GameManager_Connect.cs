using UnityEngine;
using System.Collections;

public class GameManager_Connect : GameManagerState
{



    void Update()
    {
        gameManager.GetPlayers();
    }

}
