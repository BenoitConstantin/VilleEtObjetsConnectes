using UnityEngine;
using System.Collections;

public class ScriptPersonnage : MonoBehaviour {

	public GameObject PrefabHerbe;
	public float waitTime = 5; 
	bool go;
	bool go2;


	void Start () {
		go = true;
		go2 = true;
	}

	void OnTriggerStay (Collider other) {
		


		if (other.tag == "Herbe") {
			go2 = false;
			//print (other.name);
		} 

		else {
			go2 = true;
		}
	}

	void OnTriggerExit (Collider other) {



		if (other.tag == "Herbe") {
			go2 = true;
			//print (other.name);
		} 

	}

	/*void OnTriggerExit (Collider other) {
		if (other.tag == "Herbe") {
			go2 = true;
		}
	}*/

	void Update () {
		if (go == true && go2 == true) {
			Instantiate(PrefabHerbe, new Vector3 (transform.position.x,0.4f,transform.position.z), Quaternion.Euler(new Vector3 (0,Random.Range(0,360),0)));
			go = false;
			StartCoroutine(Example());
		}
	}

	IEnumerator Example() {
		yield return new WaitForSeconds(waitTime);
		go = true;
	}

}
