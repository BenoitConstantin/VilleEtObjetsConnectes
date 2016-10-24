using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager_TouchToPlay : GameManagerState
{
    [SerializeField]
    string touchPlaySceneName = "TouchToPlay";

    public override void OnActivation(string previousState, string info = "")
    {
        base.OnActivation(previousState, info);
    }

    public override void OnDesactivation(string nextState, string info = "")
    {
        base.OnDesactivation(nextState, info);
        SceneManager.UnloadScene(touchPlaySceneName);
    }


}
