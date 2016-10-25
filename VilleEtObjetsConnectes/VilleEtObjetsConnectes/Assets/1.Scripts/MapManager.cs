using UnityEngine;
using System.Collections;
using EquilibreGames;

public class MapManager : MonoBehaviour {


    // 0 aucune équipe n'a la case
    // 1 l'équipe 1 à la case
    // 2 l'équipe 2 à la case

    [SerializeField]
    int width = 25;

    [SerializeField]
    int length = 25;

    [SerializeField]
    TeamScriptableObject teamScriptableObject;

    int[] bitMap;
    GameObject[] flowers;



    void Awake()
    {
        bitMap = new int[width * length];
        flowers = new GameObject[width * length];
    }



    void Conquere(Vector2 position, int teamId)
    {
        int index = (int)(position.y / length * width + position.x / width);

        bitMap[index] = teamId;

        if(flowers[index] != null)
        {
            ObjectPool.Instance.Pool(flowers[index]);
            ObjectPool.Instance.GetFromPool(teamScriptableObject.teamInfos[teamId-1].flower).transform.position = new Vector3(index*width,0,index*length);
        }
    }


}
