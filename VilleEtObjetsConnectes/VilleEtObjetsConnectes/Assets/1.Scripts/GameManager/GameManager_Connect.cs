using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager_Connect : GameManagerState
{

    [SerializeField]
    string connectSceneName = "Connect";

    [SerializeField]
    float timeBetween2Update = 0.5f;

    float timer = -1;

    public override void OnActivation(string previousState, string info = "")
    {
        base.OnActivation(previousState, info);

        SceneManager.LoadScene(connectSceneName);
    }

    void Update()
    {
        if (Time.time > timer)
        {
            gameManager.GetPlayers();
            timer = Time.time + timeBetween2Update;
        }
    }

}
