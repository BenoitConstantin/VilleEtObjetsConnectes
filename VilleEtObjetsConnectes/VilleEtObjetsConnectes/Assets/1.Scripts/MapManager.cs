using UnityEngine;
using System.Collections;
using EquilibreGames;

public class MapManager : MonoBehaviour {


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


    void Update()
    {
        foreach(Player p in GameManager.Instance.Players)
        {
            Conquere(new Vector2(p.transform.position.x, p.transform.position.z), p.TeamId);
        }
    }


    void Conquere(Vector2 position, int teamId)
    {
        float caseWidth = mapWidth / gridWidth;
        float caseHeight = mapHeight / gridLength;

        int index = ((int)(position.y/(caseHeight)) * gridWidth) + ((int)(position.x/(caseWidth))) + ((int)(gridLength*gridWidth/2f));

        bitMap[index] = teamId;

        if(flowers[index] != null)
        {
            ObjectPool.Instance.Pool(flowers[index]);
        }
        GameObject flower = ObjectPool.Instance.GetFromPool(teamScriptableObject.teamInfos[teamId - 1].flower);
        flower.transform.position = new Vector3((position.x * (caseWidth/2f)) /*- (caseWidth) /2f*/ , 0, (position.y * (caseHeight/2f)) /*- (caseHeight) / 2f*/);
        flowers[index] = flower;

    }


}
