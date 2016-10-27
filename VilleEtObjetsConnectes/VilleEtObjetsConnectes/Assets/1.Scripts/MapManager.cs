using UnityEngine;
using System.Collections;
using EquilibreGames;

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

    int[] bitMap;
    GameObject[] flowers;



    void Awake()
    {
        bitMap = new int[gridWidth * gridLength];
        flowers = new GameObject[gridWidth * gridLength];
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
