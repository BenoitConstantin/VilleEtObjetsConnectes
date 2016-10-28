using UnityEngine;
using System.Collections;
using SimpleJSON;

public class CoordonateRequest : MonoBehaviour {


    [SerializeField]
    float timeBetween2Update = 0.25f;


    [SerializeField]
    Vector2 borderLeftTopGPS;

    [SerializeField]
    Vector2 borderRightTopGPS;

    [SerializeField]
    Vector2 borderLeftBotGPS;

    [SerializeField]
    Vector2 borderRightBotGPS;


    float timer = -1;
    long lastUpdate = -1;


    Vector2 translation;
    Vector2 columnTransformationMatrix1;
    Vector2 columnTransformationMatrix2;

    void Awake()
    {
        //Fing the matrix transformation that send GPS coordonate to the ortho-normalized base
        translation = -borderLeftTopGPS;


        //from ortho-normalized to GPS
        columnTransformationMatrix1 = borderLeftTopGPS - borderLeftBotGPS;
        columnTransformationMatrix2 = borderRightBotGPS - borderLeftBotGPS;

        //from GPD to ortho-normalized
        float det = (columnTransformationMatrix1.x * columnTransformationMatrix2.y - columnTransformationMatrix1.y * columnTransformationMatrix2.x);

        columnTransformationMatrix1 = 1f / det * new Vector2(columnTransformationMatrix2.y, -columnTransformationMatrix1.y);
        columnTransformationMatrix2 = 1f / det * new Vector2(-columnTransformationMatrix2.x, columnTransformationMatrix1.x);
    }



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
                    //Apply transformation matrix to GPS coordonate
                    p.MoveTo(new Vector2((JNode[i]["x"].AsFloat* columnTransformationMatrix1.x + JNode[i]["y"].AsFloat* columnTransformationMatrix2.x), (JNode[i]["x"].AsFloat * columnTransformationMatrix1.y + JNode[i]["y"].AsFloat * columnTransformationMatrix2.y)) + translation, deltaTime/1000);
                }
            }
        } 
    }

}
