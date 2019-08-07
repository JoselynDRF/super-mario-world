using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

  // Movement variables
  public float velX = 0.1f;
  public float movX;
  public float inputX;

  // Jump variables
  public float jumpingForce = 300f;
  public Transform foot;
  public float radioFoot;
  public LayerMask floor;
  public bool inFloor;

  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void FixedUpdate() {
    float inputX = Input.GetAxis("Horizontal");

    if (inputX > 0) {
      movX = transform.position.x + (inputX * velX);
      transform.position = new Vector3(movX, transform.position.y, 0);
      transform.localScale = new Vector3(1, 1, 1);
    }

    if (inputX < 0) {
      movX = transform.position.x + (inputX * velX);
      transform.position = new Vector3(movX, transform.position.y, 0);
      transform.localScale = new Vector3(-1, 1, 1);
    }

    inFloor = Physics2D.OverlapCircle(foot.position, radioFoot, floor);

    if (inFloor && Input.GetKeyDown(KeyCode.Space)) {
      GetComponent <Rigidbody2D>().AddForce (new Vector2(0, jumpingForce));
    }
  }
}
