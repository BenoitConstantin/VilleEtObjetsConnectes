using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    [SerializeField]
    new Transform transform;

    int id = -1;
    int teamId = -1;
    new string name = "";



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
    }


    public void MoveTo(Vector2 position)
    {

    }
}
