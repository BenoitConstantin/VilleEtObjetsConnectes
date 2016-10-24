using UnityEngine;
using System.Collections;

public class GameManager_Game : GameManagerState
{


    /// <summary>
    /// Can't launch Tutorial in game state
    /// </summary>
    /// <returns></returns>
    public override bool LaunchTutorial()
    {
        return false;
    }

    /// <summary>
    /// Can't launch game if the game is started
    /// </summary>
    /// <returns></returns>
    public override bool LaunchGame()
    {
        return false;
    }
}
