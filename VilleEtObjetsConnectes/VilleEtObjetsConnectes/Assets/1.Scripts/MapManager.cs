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


    Vector2 translation;
    Vector2 columnTransformationMatrix1;
    Vector2 columnTransformationMatrix2;


    int[] bitMap;
    GameObject[] flowers;

    float timer = -1;
    bool lastSendingIsReturned = true;

    float maxTime;

    GameObject gameManager;

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

        gameManager = GameObject.Find("GameManager");

        backgroundMusic.Play();
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

        float deltaScore = team1Score - team2Score;

        team1Music.volume = audioCurve.Evaluate(0.5f + deltaScore);
        team2Music.volume = audioCurve.Evaluate(0.5f - deltaScore);
    }


    public void Conquer(Vector2 position, int teamId)
    {
        /*position.x = (position.x * columnTransformationMatrix1.x + position.y * columnTransformationMatrix2.x);
        position.y = (position.x * columnTransformationMatrix1.y + position.y * columnTransformationMatrix2.y);
        position += translation;

        float caseWidth = mapWidth / gridWidth;
        float caseHeight = mapHeight / gridLength;

        int index = ((int)(position.y/(caseHeight)) * gridWidth) + ((int)(position.x/(caseWidth))) + ((int)(gridLength*gridWidth/2f)) ;
        */
        Vector2 normalizedPosition = new Vector2((position.x - borderLeftTop.x) / mapWidth, -(position.y - borderLeftTop.y) / mapHeight); //gameManager.GetComponent<CoordonateRequest>().GetNormalizedCoordinates(position);
        if (normalizedPosition.x > 1 || normalizedPosition.y > 1)
            return;
        int index = (int)(normalizedPosition.y*gridLength)*gridWidth + (int)(normalizedPosition.x*gridWidth);

        bitMap[index] = teamId;

        if(flowers[index] != null)
        {
            if(flowers[index].name != teamScriptableObject.teamInfos[teamId].flower)
            {
                Debug.Log("Pool");
                ObjectPool.Instance.Pool(flowers[index]);
                Debug.Log("Alloc change =>" + teamScriptableObject.teamInfos[teamId].flower);
                GameObject flower = ObjectPool.Instance.GetFromPool(teamScriptableObject.teamInfos[teamId].flower);
                //flower.transform.position = new Vector3(((int)(position.x / caseWidth)) * (caseWidth), 0, ((int)(position.y / caseHeight)) * (caseHeight));
                flower.transform.position = new Vector3(borderLeftTop.x + normalizedPosition.x * mapWidth,0, borderLeftTop.y - normalizedPosition.y * mapHeight);

                flowers[index] = flower;
            }
        }
        else
        {
            GameObject flower = ObjectPool.Instance.GetFromPool(teamScriptableObject.teamInfos[teamId].flower);
            //flower.transform.position = new Vector3(((int)(position.x / caseWidth)) * (caseWidth), 0, ((int)(position.y / caseHeight)) * (caseHeight));
            flower.transform.position = new Vector3(borderLeftTop.x + normalizedPosition.x * mapWidth, 0, borderLeftTop.y - normalizedPosition.y * mapHeight);
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
        int cpt1 = 0;
        int cpt2 = 0;

        for(int i =0; i < bitMap.Length; i++)
        {
            if (bitMap[i] == 0)
                cpt1++;
            else if (bitMap[i] == 1)
                cpt2++;
        }

        percentTeam1 = cpt1 / bitMap.Length;
        percentTeam2 = cpt2 / bitMap.Length;

    }

}
