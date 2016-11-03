using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToTouchToPlay : MonoBehaviour {

	public void LaunchTouchToPlay()
    {
        SceneManager.LoadScene("TouchToPlay");
    }
}
