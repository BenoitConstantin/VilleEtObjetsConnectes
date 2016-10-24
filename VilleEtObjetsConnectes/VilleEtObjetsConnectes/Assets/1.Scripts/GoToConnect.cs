using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToConnect : MonoBehaviour {
    
	public void LaunchConnection()
    {
        GameManager.Instance.LaunchConnect();
    } 
}
