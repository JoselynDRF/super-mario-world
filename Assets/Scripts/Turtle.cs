using UnityEngine;

public class Turtle : MonoBehaviour {

	public GameObject initialPosition;
	public GameObject endPosition;
	public float velTurtle = 0.05f;
	public bool lookingRight = true;

	Animator animator;

	// Start is called before the first frame update
	void Start() {
		 animator = GetComponent <Animator>();

		if (lookingRight) {
			transform.position = initialPosition.transform.position;
		} else {
			transform.position = endPosition.transform.position;
		}
	}

	// Update is called once per frame
	void FixedUpdate() {
		if (lookingRight) {
			transform.position = Vector3.MoveTowards(transform.position, endPosition.transform.position, velTurtle);
			animator.SetFloat("velX", velTurtle);

			if (transform.position == endPosition.transform.position) {
				lookingRight = false;
				GetComponent<SpriteRenderer>().flipX = true;
	
			}
		}

		if (!lookingRight) {
			transform.position = Vector3.MoveTowards(transform.position, initialPosition.transform.position, velTurtle);
			animator.SetFloat("velX", velTurtle);

			if (transform.position == initialPosition.transform.position) {
				lookingRight = true;
				GetComponent<SpriteRenderer>().flipX = false;
		
			}
		}

	}
}
