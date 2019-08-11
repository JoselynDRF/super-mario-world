using UnityEngine;

public class Shell : MonoBehaviour {

	public float bounceForce = 500f;
	public float velShell;
	public float impulse;
	public bool rebound;
	public Transform bouncePosition;
	public float radioShell;
	public LayerMask floor;
	public LayerMask objectRebound;

	public GameObject Mario;
	public GameObject MarioFoot;

	public Transform ShellFoot;
	public bool inFloorShell;

	public bool getShell;

	Animator animator;
	Rigidbody2D rb;

	// Start is called before the first frame update
	void Start() {
		animator = GetComponent <Animator>();
		rb = GetComponent <Rigidbody2D>();
	}

	// Update is called once per frame
	void FixedUpdate() {
		velShell = rb.velocity.x;
		rebound = Physics2D.OverlapCircle(bouncePosition.position, radioShell, objectRebound);

		if (velShell != 0) {
			impulse = velShell;

			if (rebound && impulse > 0) {
				this.rb.AddForce(new Vector2(-bounceForce, 0));
			}

			if (rebound && impulse < 0) {
				this.rb.AddForce(new Vector2(bounceForce, 0));
			}
		}
	}
}
