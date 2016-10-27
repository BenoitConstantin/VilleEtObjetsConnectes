using UnityEngine;
using System.Collections;
using SimpleJSON;

public class CoordonateRequest : MonoBehaviour {


    [SerializeField]
    float timeBetween2Update = 0.25f;

    [SerializeField]
    float mapScale = 0.01f;

    float timer = -1;
    long lastUpdate = -1;

    public void StartRequest()
    {
        timer = Time.time + timeBetween2Update;
        this.enabled = true;
    }

    public void StopRequest()
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
        StartCoroutine(AllocateCoordonates(new WWW(GameManager.Instance.ServerAddress +"/player/"), GameManager.Instance.Players));
    }

    IEnumerator AllocateCoordonates(WWW request, Player[] players)
    {
        yield return (request);
        Debug.Log(request.text);

       JSONNode JNode =  JSON.Parse(request.text);

        long time = long.Parse(JNode["time"]);

        //The response is too old
        if (time < lastUpdate)
            yield return null;

        float deltaTime = time - lastUpdate;

        lastUpdate = time;

        JNode = JNode["players"];
        int length = JNode.Count;

        for(int i =0; i < length; i++)
        {
            foreach(Player p in players)
            {
                if(p.Id == JNode[i]["id"].AsInt)
                {
                    p.MoveTo(new Vector2(mapScale*JNode[i]["x"].AsFloat, mapScale * JNode[i]["y"].AsFloat), deltaTime/1000);
                }
            }
        } 
    }

}
