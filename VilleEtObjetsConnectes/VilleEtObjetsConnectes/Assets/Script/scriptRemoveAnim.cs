using UnityEngine;
using System.Collections;

public class scriptRemoveAnim : MonoBehaviour {

	Animator an;

	// Use this for initialization
	void Start () {
		an = gameObject.GetComponent<Animator>();
		Destroy (an,3);
	}
	

}
