using UnityEngine;

public class KeyScript : MonoBehaviour {
	public string PlayerTag = "Player";
	public GameObject Door;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider other){
		if(other.transform.gameObject.tag == PlayerTag){
			GetKey();
		}
	}
	void GetKey(){
        NOTLonely_Door.DoorScript doorScript = Door.GetComponent<NOTLonely_Door.DoorScript> ();
		doorScript.keySystem.isUnlock = true;
		Destroy (gameObject);
	}
}
