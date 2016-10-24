using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager_SplashScreen : GameManagerState
{

    [SerializeField]
    float timeOnSplashScreen = 1.5f;

    [SerializeField]
    string touchPlaySceneName = "TouchToPlay";

    [SerializeField]
    string splashScreenSceneName = "SplashScreen";

    float timer = -1;

    public override void OnActivation(string previousState, string info = "")
    {
        base.OnActivation(previousState, info);

        DontDestroyOnLoad(gameManager.gameObject);

        timer = Time.time;
        SceneManager.LoadSceneAsync(touchPlaySceneName, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += GoToTouchToPlay;
    }

    void GoToTouchToPlay(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(GoToTouchToPlayCoroutine());
    }

    IEnumerator GoToTouchToPlayCoroutine()
    {
        yield return new WaitForSeconds(Mathf.Clamp((timeOnSplashScreen - (Time.time - timer)),0, timeOnSplashScreen));

        this.stateMachine.ChangeState("TouchToPlay");
    }

    public override void OnDesactivation(string nextState, string info = "")
    {
        base.OnDesactivation(nextState, info);
        SceneManager.UnloadScene(splashScreenSceneName);
        SceneManager.sceneLoaded -= GoToTouchToPlay;
    }
}
