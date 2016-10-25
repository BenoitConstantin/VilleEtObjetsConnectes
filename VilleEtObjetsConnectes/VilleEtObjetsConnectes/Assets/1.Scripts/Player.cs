using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Player : MonoBehaviour {

    [SerializeField]
    new Transform transform;


    [SerializeField]
    new string name = "";

    [SerializeField]
    int id = -1;

    [SerializeField]
    int teamId = -1;

    [SerializeField]
    Text nameText;

    #region Movement variable
    Vector2 targetPos;
    float targetTime;
    #endregion

    public int Id
    {
        get { return id; }
    }

    public int TeamId
    {
        get { return teamId; }
    }

    public void Init(int _id, int _teamId, string _name)
    {
        id = _id;
        teamId = _teamId;

        name = _name;
        nameText.text = _name;
    }




    public void MoveTo(Vector2 position, float deltaTime, bool directPos = false)
    {
        if (targetPos != default(Vector2) && !targetPos.Equals(position) && !directPos)
        {
            targetPos = position;
        }
        else
        {
            transform.position = new Vector3(position.x, transform.position.y, position.y);
        }

        targetTime = deltaTime;
    }


    void Update()
    {
        //Debug.Log(targetTime);
        if(targetPos != default(Vector2))
        {
            transform.position = Vector3.Lerp(transform.position,new Vector3(targetPos.x, transform.position.y,targetPos.y), 1f/targetTime *Time.deltaTime);
        }
    }
}
