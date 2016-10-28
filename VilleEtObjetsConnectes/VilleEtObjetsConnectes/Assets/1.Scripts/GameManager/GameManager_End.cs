using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager_End : GameManagerState {

    [SerializeField]
    string gameSceneName = "Game";


    public override void OnActivation(string previousState, string info = "")
    {
        base.OnActivation(previousState, info);
        this.stateMachine.ChangeState("TouchToPlay");
        SceneManager.LoadScene("TouchToPlay");
    }

    public override void OnDesactivation(string nextState, string info = "")
    {
        base.OnDesactivation(nextState, info);
        SceneManager.UnloadScene(gameSceneName);
    }

}
