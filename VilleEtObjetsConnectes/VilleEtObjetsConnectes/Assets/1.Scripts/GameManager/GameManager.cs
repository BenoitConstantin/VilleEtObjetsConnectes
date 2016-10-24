using UnityEngine;
using System.Collections;
using EquilibreGames;
using SimpleJSON;

public class GameManager : Singleton<GameManager> {

    [SerializeField][Tooltip("The server address to request on")]
    string serverAddress;


    [SerializeField]
    StateMachine stateMachine;

    [SerializeField]
    Player[] players;

    [SerializeField]
    GameObject playerPrefab;

    public Player[] Players
    {
        get { return players; }
    }



    public string ServerAddress
    {
        get { return serverAddress; }
    }


    public void GetPlayers()
    {
        StartCoroutine(GetPlayersCoroutine(new WWW(GameManager.Instance.ServerAddress + "/player/")));
    }

    int lastUpdate = -1;
    IEnumerator GetPlayersCoroutine(WWW request)
    {
        yield return request;

        Debug.Log(request);

        JSONNode JNode = JSON.Parse(request.text);

        //The response is too old
        if (JNode["time"].AsInt < lastUpdate)
            yield return null;

        lastUpdate = JNode["time"].AsInt;

        JNode = JNode["players"];
        int length = JNode.Count;

        if (players == null || length != players.Length)
        {
            //Clear all players
            for (int i = 0; i < players.Length; i++)
                Destroy(players[i]);

            players = new Player[length];
        }

        for (int i = 0; i < length; i++)
        {
            //Instantiate new players
            if (players[i] == null)
                players[i] = Instantiate(playerPrefab).GetComponent<Player>();

             players[i].gameObject.SetActive(false);

             players[i].Init(JNode["id"].AsInt, JNode["team"].AsInt, JNode["name"]);
        }
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


    public bool LaunchConnect()
    {
        return ((GameManagerState)(this.stateMachine.CurrentState)).LaunchConnect();
    }
}
