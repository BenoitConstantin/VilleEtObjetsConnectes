using UnityEngine;
using System.Collections;
using SimpleJSON;

public class CoordonateRequest : MonoBehaviour {

    /* public class Coordonate ///Il faut que le serveur renvoie une requête JSON avec une liste de coordonnées.
     {
         int id;
         int x;
         int y;
     }*/

    [SerializeField]
    float timeBetween2Update = 0.25f;

    float timer = -1;

    void Start()
    {
        timer = Time.time + timeBetween2Update;
        this.enabled = true;
    }

    void Stop()
    {
        timer = -1;
        this.enabled = false;
    }


     void Update()
    {
        if (Time.time > timer)
        {
            RequestCoordonates();
            timer = Time.time + timeBetween2Update;
        }
    }


    void RequestCoordonates()
    {
        StartCoroutine(AllocateCoordonates(new WWW(GameManager.Instance.ServerAddress +"/player"), GameManager.Instance.Players));
    }

    IEnumerator AllocateCoordonates(WWW request, Player[] players)
    {
        yield return (request);
        Debug.Log(request.text);

       JSONNode JNode =  JSON.Parse(request.text);

        int length = JNode.Count;

        for(int i =0; i < length; i++)
        {
            foreach(Player p in GameManager.Instance.Players)
            {
                if(p.Id == JNode[i]["id"].AsInt)
                {
                    p.MoveTo(new Vector2(JNode[i]["x"].AsFloat, JNode[i]["y"].AsFloat), timeBetween2Update);
                }
            }
        } 
    }

}
