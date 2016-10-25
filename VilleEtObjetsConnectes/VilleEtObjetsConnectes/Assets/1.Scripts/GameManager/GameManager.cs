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

    public CoordonateRequest coordonateRequest;

    public Player[] Players
    {
        get { return players; }
    }

    LocationService lService;

    void Start()
    {
        lService = new LocationService();
        lService.Start(0.1f);
    }


    public string ServerAddress
    {
        get { return serverAddress; }
    }


    public void GetPlayers()
    {
        StartCoroutine(GetPlayersCoroutine(new WWWS(serverAddress + "/player/")));
    }

    int lastUpdate = -1;
    IEnumerator GetPlayersCoroutine(WWWS request)
    {
        yield return request.next();

        Debug.Log(request.text);

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
                ObjectPool.Instance.Pool(players[i].gameObject);

            players = new Player[length];
        }

        for (int i = 0; i < length; i++)
        {
            //Instantiate new players
            if (players[i] == null)
                players[i] = ObjectPool.Instance.GetFromPool("Player").GetComponent<Player>();

             players[i].Init(JNode["id"].AsInt, JNode["team"].AsInt, JNode["name"]);
            players[i].gameObject.SetActive(false);
        }
    }


    public void UnLockMatch()
    {
        StartCoroutine(UnLockMatchCoroutine());
    }

    public void LockMatch()
    {
        StartCoroutine(LockMatchCoroutine());
    }

    IEnumerator UnLockMatchCoroutine()
    {
        WWWS request = new WWWS(GameManager.Instance.serverAddress + "/reset/", "POST");
        yield return request.next();

        Debug.Log(request.error);
        if(!string.IsNullOrEmpty(request.error) && !request.error.Equals("Null"))
        {
            StartCoroutine(UnLockMatchCoroutine());
        }

    }

    IEnumerator LockMatchCoroutine()
    {
        WWWS request = new WWWS(serverAddress + "/start/", "POST");
        yield return request.next();

        if (!string.IsNullOrEmpty(request.error)  && !request.error.Equals("Null"))
        {
            StartCoroutine(LockMatchCoroutine());
        }
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

    public Vector2 GPSLocation()
    {
        return new Vector2(lService.lastData.longitude, lService.lastData.latitude);
    }

    void OnDestroy()
    {
        lService.Stop();
    }
}
