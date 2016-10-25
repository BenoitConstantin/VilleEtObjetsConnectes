using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToGame : MonoBehaviour {

	public void LaunchGame()
    {
        GameManager.Instance.LaunchGame();
    }
}
