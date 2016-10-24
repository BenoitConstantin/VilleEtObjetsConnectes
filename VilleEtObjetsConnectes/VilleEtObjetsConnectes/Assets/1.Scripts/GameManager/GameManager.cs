using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> {

    [SerializeField][Tooltip("The server address to request on")]
    string serverAddress;


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
}
