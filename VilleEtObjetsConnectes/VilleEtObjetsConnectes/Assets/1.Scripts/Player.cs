using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Player : MonoBehaviour {

    [SerializeField]
    new Transform transform;

    [SerializeField]
    string playerName = "";

    public string PlayerName
    {
        get { return playerName; }
    }


    [SerializeField]
    int id = -1;

    [SerializeField]
    int teamId = -1;

    [SerializeField]
    Text nameText;

    [SerializeField]
    MeshRenderer[] meshRenderers;

    [SerializeField]
    TrailRenderer trailRenderer;

    [SerializeField]
    TeamScriptableObject teamScriptableObject;

    [SerializeField]
    Material greyMaterial;

    #region Movement variable
    Vector2 targetPos;
    float targetTime;
    #endregion

    public Vector2 NormalizedPosition { get; private set; }

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

        playerName = _name;
        nameText.text = _name;
        nameText.color = teamScriptableObject.teamInfos[_teamId].color;

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    public void MoveTo(Vector2 position, Vector2 normalizedPosition, float deltaTime, bool directPos = false)
    {
        NormalizedPosition = normalizedPosition;

        if (targetPos != default(Vector2) && !targetPos.Equals(position) && !directPos)
            targetPos = position;
        else
            transform.position = new Vector3(position.x, transform.position.y, position.y);

        targetTime = deltaTime;
    }


    void Update()
    {
        //Debug.Log(targetTime);
        if(targetPos != default(Vector2))
            transform.position = Vector3.Lerp(transform.position,new Vector3(targetPos.x, transform.position.y,targetPos.y), 1f/targetTime *Time.deltaTime);
    }

    public void ValidPosition(bool isValid)
    {
        if (!isValid)
        {
            foreach(MeshRenderer meshRenderer in meshRenderers)
                 meshRenderer.material = greyMaterial;
        }
        else
        {
            foreach (MeshRenderer meshRenderer in meshRenderers)
                meshRenderer.material = teamScriptableObject.teamInfos[teamId].material;

            trailRenderer.material = teamScriptableObject.teamInfos[teamId].trailMaterial;
        }
    }
}
