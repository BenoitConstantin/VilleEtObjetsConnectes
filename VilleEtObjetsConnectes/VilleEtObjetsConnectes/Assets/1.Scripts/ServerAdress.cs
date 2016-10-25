using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ServerAdress : MonoBehaviour {
    Text addr;
	// Use this for initialization
	void Start () {
        addr = GetComponent<Text>();
        addr.text = "Adresse du serveur : " + GameManager.Instance.ServerAddress;
	}
}
