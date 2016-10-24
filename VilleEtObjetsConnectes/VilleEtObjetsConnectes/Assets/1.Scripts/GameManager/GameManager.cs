using UnityEngine;
using System.Collections;
using EquilibreGames;

public class GameManager : Singleton<GameManager> {

    [SerializeField][Tooltip("The server address to request on")]
    string serverAddress;


    [SerializeField]
    StateMachine stateMachine;

    [SerializeField]
    Player[] players;


    public Player[] Players
    {
        get { return players; }
    }



    public string ServerAddress
    {
        get { return serverAddress; }
    }


    void GetPlayers()
    {

    }


    void LockMatch()
    {

    }

    void UnLockMatch()
    {

    }


    public bool LaunchTutorial()
    {
        return ((GameManagerState)(this.stateMachine.CurrentState)).LaunchTutorial();
    }

    public bool LaunchGame()
    {
        return ((GameManagerState)(this.stateMachine.CurrentState)).LaunchGame();
    }
}
