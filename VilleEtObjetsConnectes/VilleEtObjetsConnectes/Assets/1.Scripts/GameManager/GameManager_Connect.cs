using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager_Connect : GameManagerState
{

    [SerializeField]
    string connectSceneName = "Connect";

    public override void OnActivation(string previousState, string info = "")
    {
        base.OnActivation(previousState, info);

        SceneManager.LoadScene(connectSceneName);
    }

    void Update()
    {
        gameManager.GetPlayers();
    }

}
