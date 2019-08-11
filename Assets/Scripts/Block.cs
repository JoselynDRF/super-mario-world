using UnityEngine;

public class Block : MonoBehaviour {
  
	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Mario") {
			this.gameObject.layer = 8;
		} else {
			this.gameObject.layer = 13;
		}
	}

}
