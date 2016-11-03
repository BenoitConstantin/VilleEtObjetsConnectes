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

    private float _Width;
    private float _Height;
    private float _Shear_amount;

    void Awake()
    {
        _Width = borderRightTopGPS.x - borderLeftTopGPS.x;
        _Height = borderRightTopGPS.y - borderRightBotGPS.y;
        _Shear_amount = borderLeftTopGPS.x - borderLeftBotGPS.x;
    }

    // Returns a vector normalized. If any value is not between 0 and 1, the point isn't on the area.
    private Vector2 _GetNormalizedCoordinates(Vector2 p)
    {
        Vector2 pt_offset = p - borderLeftTopGPS;
        Vector2 scaled = new Vector2();
        scaled.y = -pt_offset.y / _Height;

        var offsetX = _Shear_amount * Mathf.Abs(scaled.y);
        scaled.x = (pt_offset.x + offsetX) / _Width;
        scaled.y = Mathf.Abs(scaled.y);
        return scaled;
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
                    var normalizedVector = _GetNormalizedCoordinates(new Vector2(JNode[i]["x"].AsFloat, JNode[i]["y"].AsFloat));

                    var newPosition = new Vector2();
                    newPosition.x = -8 + normalizedVector.x * 15;
                    newPosition.y = -8 + (1 - normalizedVector.y) * 14;
                    //normalizedVector.x = Mathf.Lerp(-8, 7, normalizedVector.x);
                    //normalizedVector.y = Mathf.Lerp(-8, 6, 1-Mathf.Abs(normalizedVector.y));


                    //Apply transformation matrix to GPS coordonate
                    p.MoveTo(newPosition, normalizedVector, deltaTime/1000);
                }
            }
        } 
    }

}
