using UnityEngine;
using System.Collections;

public class GameManager_Game : GameManagerState
{

    [SerializeField]
    Vector2 initialGamePosition = Vector2.zero;

    public override void OnActivation(string previousState, string info = "")
    {
        base.OnActivation(previousState, info);

        Player[] players = GameManager.Instance.Players;

        for (int i =0; i < players.Length; i++)
        {
            players[i].MoveTo(initialGamePosition, 0, true);
        }
    }


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
