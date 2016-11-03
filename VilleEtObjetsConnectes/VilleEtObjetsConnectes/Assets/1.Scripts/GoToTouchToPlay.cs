using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToTouchToPlay : MonoBehaviour {

    // i wont change the name of the method, but it do not launch TouchToPlay, it toggles score screen
	public void LaunchTouchToPlay()
    {
        var MapManager = GameObject.FindObjectOfType(typeof(MapManager)) as MapManager;
        //MapManager.ScoreScreen.SetActive(!MapManager.ScoreScreen.activeInHierarchy);

        var canvas = MapManager.ScoreScreen.GetComponentInChildren<CanvasGroup>();

        if (canvas.alpha == 0)
            canvas.alpha = 1;
        else
            canvas.alpha = 0;
    }
}
