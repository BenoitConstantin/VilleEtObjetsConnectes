using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager_End : GameManagerState {

    [SerializeField]
    string gameSceneName = "Game";


    public override void OnActivation(string previousState, string info = "")
    {
        base.OnActivation(previousState, info);

        var mapManager = GameObject.FindObjectOfType(typeof(MapManager)) as MapManager;
        mapManager.ScoreScreen.SetActive(true);
        this.stateMachine.ChangeState("TouchToPlay");
    }

    public override void OnDesactivation(string nextState, string info = "")
    {
        base.OnDesactivation(nextState, info);
        SceneManager.UnloadScene(gameSceneName);
    }

}
