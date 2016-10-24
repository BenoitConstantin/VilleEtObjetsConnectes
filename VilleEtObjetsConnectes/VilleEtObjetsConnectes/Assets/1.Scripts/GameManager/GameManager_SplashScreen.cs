using UnityEngine;
using System.Collections;

public class GameManager_SplashScreen : GameManagerState
{



    public override void OnActivation(string previousState, string info = "")
    {
        base.OnActivation(previousState, info);

        DontDestroyOnLoad(gameManager.gameObject);
    }

}
