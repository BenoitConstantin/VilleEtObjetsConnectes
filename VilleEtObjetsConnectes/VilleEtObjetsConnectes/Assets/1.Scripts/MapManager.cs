using UnityEngine;
using System.Collections;
using EquilibreGames;
using SimpleJSON;
using Newtonsoft;

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

    int[] bitMap;
    GameObject[] flowers;

    float timer = -1;
    bool lastSendingIsReturned = true;

    void Awake()
    {
        bitMap = new int[gridWidth * gridLength];
        flowers = new GameObject[gridWidth * gridLength];
    }


    void Update()
    {
        if (Time.time > timer && lastSendingIsReturned)
        {
            SendBitMap();
            timer = Time.time + sendingTime;
            lastSendingIsReturned = false;
        }
    }


    public void Conquer(Vector2 position, int teamId)
    {
        float caseWidth = mapWidth / gridWidth;
        float caseHeight = mapHeight / gridLength;

        int index = ((int)(position.y/(caseHeight)) * gridWidth) + ((int)(position.x/(caseWidth))) + ((int)(gridLength*gridWidth/2f)) ;

        bitMap[index] = teamId;

        if(flowers[index] != null)
        {
            ObjectPool.Instance.Pool(flowers[index]);
        }
        GameObject flower = ObjectPool.Instance.GetFromPool(teamScriptableObject.teamInfos[teamId - 1].flower);
        flower.transform.position = new Vector3( ((int) (position.x/caseWidth)) * (caseWidth) , 0,  ((int) (position.y / caseHeight)) * (caseHeight));
        flowers[index] = flower;
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
        string[][] formData = new string[1][];


        formData[0] = new string[] { "map", Newtonsoft.Json.JsonConvert.SerializeObject(bitMap)};

        WWWS request = new WWWS(GameManager.Instance.ServerAddress + "/map/", "POST", formData);
        yield return request.next();

        lastSendingIsReturned = true;
    }


    public void GetMapConquer(out int percentTeam1, out int percentTeam2)
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
