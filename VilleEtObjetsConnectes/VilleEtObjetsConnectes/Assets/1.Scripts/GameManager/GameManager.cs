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


        IEnumerator Start()
    {
        Debug.Log("LocationService : " + Input.location.isEnabledByUser);

        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
                yield break;

            // Start service before querying location
            Input.location.Start();

            // Wait until service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1)
            {
                print("Timed out");
                yield break;
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                print("Unable to determine device location");
                yield break;
            }
            else
            {
                // Access granted and location value could be retrieved
                print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            }
        }


    public string ServerAddress
    {
        get { return serverAddress; }
    }


    public void GetPlayers()
    {
        StartCoroutine(GetPlayersCoroutine(new WWW(serverAddress + "/player/")));
    }

    int lastUpdate = -1;
    IEnumerator GetPlayersCoroutine(WWW request)
    {
        Debug.Log("Request sended");
        yield return request;

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

             players[i].Init(JNode[i]["id"].AsInt, JNode[i]["team"].AsInt, JNode[i]["name"]);
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
        WWWForm form = new WWWForm();
        form.AddField("", "");

        WWW request = new WWW(serverAddress + "/reset/", form);
        yield return request;

        Debug.Log(request.error);
        if(!string.IsNullOrEmpty(request.error) && !request.error.Equals("Null"))
        {
            StartCoroutine(UnLockMatchCoroutine());
        }

    }

    IEnumerator LockMatchCoroutine()
    {
        WWWForm formData = new WWWForm();

        while (Input.location.status == LocationServiceStatus.Initializing)
            yield return null;

        Vector2 gpsLocation = GPSLocation();

        formData.AddField("x", gpsLocation.x.ToString());
        formData.AddField("y", gpsLocation.y.ToString());

        WWW request = new WWW(serverAddress + "/start/", formData);
        yield return request;

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
        Debug.Log("Location service : " + Input.location.status);
        return new Vector2(Input.location.lastData.longitude, Input.location.lastData.latitude);
    }

    void OnDestroy()
    {
        Input.location.Stop();
    }
}
