using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager_TouchToPlay : GameManagerState
{

    [SerializeField]
    string splashScreenSceneName = "SplashScreen";


    public override void OnActivation(string previousState, string info = "")
    {
        base.OnActivation(previousState, info);
        SceneManager.UnloadScene(splashScreenSceneName);
    }


}
