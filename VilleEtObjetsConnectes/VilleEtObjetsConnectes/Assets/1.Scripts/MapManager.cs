using UnityEngine;
using System.Collections;
using EquilibreGames;
using SimpleJSON;
using Newtonsoft;
using UnityEngine.UI;

public class MapManager : Singleton<MapManager> {


    // 0 aucune équipe n'a la case
    // 1 l'équipe 1 à la case
    // 2 l'équipe 2 à la case

    [SerializeField]
    int gridWidth = 25;

    [SerializeField]
    int gridLength = 25;

    [SerializeField]
    float mapWidth;

    [SerializeField]
    float mapHeight;


    [SerializeField]
    TeamScriptableObject teamScriptableObject;

    [SerializeField]
    float sendingTime = 0.25f;


    [SerializeField]
    Vector2 borderLeftTop;

    [SerializeField]
    Vector2 borderRightTop;

    [SerializeField]
    Vector2 borderLeftBot;

    [SerializeField]
    Vector2 borderRightBot;

    [SerializeField]
    AudioSource backgroundMusic;

    [SerializeField]
    AudioSource team1Music;

    [SerializeField]
    AudioSource team2Music;

    [SerializeField]
    AnimationCurve audioCurve;

    [SerializeField]
    Image timerFillingImage;

    [Header("Score")]
    [SerializeField]
    Text ScoreTeamA;

    [SerializeField]
    Text ScoreTeamB;

    [SerializeField]
    public GameObject ScoreScreen;

    [SerializeField]
    GameObject TeamA;

    [SerializeField]
    GameObject TeamB;

    Vector2 translation;
    Vector2 columnTransformationMatrix1;
    Vector2 columnTransformationMatrix2;


    public int[] bitMap;
    GameObject[] flowers;

    float timer = -1;
    bool lastSendingIsReturned = true;

    float maxTime;

    void Awake()
    {
        bitMap = new int[gridWidth * gridLength];

        for (int i = 0; i < bitMap.Length; i++)
            bitMap[i] = -1;

        flowers = new GameObject[gridWidth * gridLength];


        //Fing the matrix transformation that send ortho-normalized coordonate to the mapCoordonate
        translation = -borderLeftTop;

        Vector2 columnTransformationMatrix1 = borderLeftTop - borderLeftBot;
        Vector2 columnTransformationMatrix2 = borderRightBot - borderLeftBot;
    }

    private void MapManager_OnEndGame()
    {
        ScoreScreen.SetActive(true);
    }

    void Start()
    {
        _InitMatch();
        backgroundMusic.Play();
        team1Music.Play();
        team2Music.Play();
    }

    private void _InitMatch()
    {
        GameManager.Instance.LockMatch();

        GameManager.Instance.coordonateRequest.StartRequest();

        GameManager.Instance.gameTimer = GameManager.Instance.matchDuration;

        Player[] players = GameManager.Instance.Players;

        for (int i = 0; i < players.Length; i++)
        {
            players[i].MoveTo(new Vector2(-99,-99), new Vector2(-99, -99), 0, true);
            players[i].gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (Time.time > timer && lastSendingIsReturned)
        {
            SendBitMap();
            timer = Time.time + sendingTime;
            lastSendingIsReturned = false;
        }

        timerFillingImage.fillAmount = 1 - GameManager.Instance.gameTimer / GameManager.Instance.matchDuration;

        float team1Score;
        float team2Score;
        GetMapConquer(out team1Score, out team2Score);

        float total = team1Score + team2Score;
        float deltaScore = team1Score - team2Score;
        _UpdateScoreScreen(team1Score, team2Score);

        if(total != 0)
        {
            //team1Music.volume = audioCurve.Evaluate(0.5f + deltaScore);
            //team2Music.volume = audioCurve.Evaluate(0.5f - deltaScore);
            team1Music.volume = audioCurve.Evaluate(team1Score/total);
            team2Music.volume = audioCurve.Evaluate(team2Score/total);
        }
    }

    private void _UpdateScoreScreen(float team1Score, float team2Score)
    {
        var scoreAFloor = Mathf.FloorToInt(team1Score*100);
        var scoreBFloor = Mathf.FloorToInt(team2Score*100);

        if (scoreAFloor == 50)
        {
            ScoreTeamA.text = "50.1%";
            ScoreTeamB.text = "49.9%";
        }
        else
        {
            ScoreTeamA.text = scoreAFloor + "%";
            ScoreTeamB.text = scoreBFloor + "%";
        }

        if (team1Score > team2Score)
        {
            TeamA.SetActive(true);
            TeamB.SetActive(false);
        }
        else
        {
            TeamA.SetActive(false);
            TeamB.SetActive(true);
        }
    }

    public void Conquer(Vector2 normalizedPosition, int teamId)
    {        
        if (normalizedPosition.x > 1 || normalizedPosition.y > 1 || normalizedPosition.x < 0 || normalizedPosition.y < 0)
            return;
        int index = (int)(normalizedPosition.y*(gridLength-1))*gridWidth + (int)(normalizedPosition.x*gridWidth);

        bitMap[index] = teamId;
        var x = index % gridWidth;
        var y = index / gridWidth;

        if (flowers[index] != null)
        {
            if(flowers[index].name != teamScriptableObject.teamInfos[teamId].flower)
            {
                Debug.Log("Pool");
                ObjectPool.Instance.Pool(flowers[index]);
                Debug.Log("Alloc change =>" + teamScriptableObject.teamInfos[teamId].flower);
                GameObject flower = ObjectPool.Instance.GetFromPool(teamScriptableObject.teamInfos[teamId].flower);
                //flower.transform.position = new Vector3(((int)(position.x / caseWidth)) * (caseWidth), 0, ((int)(position.y / caseHeight)) * (caseHeight));

                flower.transform.position = new Vector3(borderLeftTop.x + (x * mapWidth/gridWidth), 0, borderLeftTop.y - (y * mapHeight/gridLength));

                flowers[index] = flower;
            }
        }
        else
        {
            GameObject flower = ObjectPool.Instance.GetFromPool(teamScriptableObject.teamInfos[teamId].flower);
            //flower.transform.position = new Vector3(((int)(position.x / caseWidth)) * (caseWidth), 0, ((int)(position.y / caseHeight)) * (caseHeight));
            flower.transform.position = new Vector3(borderLeftTop.x + (x * mapWidth / gridWidth), 0, borderLeftTop.y - (y * mapHeight / gridLength));
            Debug.Log("Alloc new =>" + teamScriptableObject.teamInfos[teamId].flower + "=>" + flower.transform.position + "=>" + normalizedPosition);
            flowers[index] = flower;
        }
    }

    void SendBitMap()
    {
        StartCoroutine(SendBitMapCoroutine());
    }

    [System.Serializable]
    public class SerializedBitMap
    {
        public int[] bitMap;

        public SerializedBitMap(int[] _bitMap)
        {
            bitMap = _bitMap;
        }
    }

    IEnumerator SendBitMapCoroutine()
    {

        WWWForm formData = new WWWForm();
        formData.AddField("map", Newtonsoft.Json.JsonConvert.SerializeObject(bitMap));

        WWW request = new WWW(GameManager.Instance.ServerAddress + "/map/", formData);
        yield return request;

        lastSendingIsReturned = true;
    }


    public void GetMapConquer(out float percentTeam1, out float percentTeam2)
    {
        float cpt1 = 0;
        float cpt2 = 0;

        for(int i =0; i < bitMap.Length; i++)
        {
            if (bitMap[i] == 0)
                cpt1++;
            else if (bitMap[i] == 1)
                cpt2++;
        }

        percentTeam1 = cpt1 / (float) bitMap.Length;
        percentTeam2 = cpt2 / (float) bitMap.Length;

    }

}
