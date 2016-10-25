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
            Vector3 position = p.transform.position;

            if (!Physics.Raycast(position, Vector3.down, Mathf.Infinity))
            {
                p.ValidPosition(false);
            }
            else
            {
                p.ValidPosition(true);
                Conquere(new Vector2(position.x, position.z), p.TeamId);
            }
        }
    }


    void Conquere(Vector2 position, int teamId)
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


}
