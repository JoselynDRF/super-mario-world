using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

  // Movement variables
  public float velX = 0.1f;
  public float movX;
  public float currentPosition;

  // Jump variables
  public float jumpingForce = 300f;
  public Transform foot;
  public float radioFoot;
  public LayerMask floor;
  public bool inFloor;

  // Down variables
  public bool isDown;

  // Look Up variables
  public bool lookUp;

  // Animations
  Animator animator;

  void Awake() {
    animator = GetComponent <Animator>();
  }

  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void FixedUpdate() {
    float inputX = Input.GetAxis("Horizontal");

    if (!isDown && !lookUp) {
      if (inputX > 0) {
        movX = transform.position.x + (inputX * velX);
        transform.position = new Vector3(movX, transform.position.y, 0);
        transform.localScale = new Vector3(1, 1, 1);
        movX = currentPosition;
      }

      if (inputX < 0) {
        movX = transform.position.x + (inputX * velX);
        transform.position = new Vector3(movX, transform.position.y, 0);
        transform.localScale = new Vector3(-1, 1, 1);
        movX = currentPosition;
      }
    }

    if (inputX != 0 && inFloor) {
      animator.SetFloat("velX", 1);
    } else {
      animator.SetFloat("velX", 0);
    }

    inFloor = Physics2D.OverlapCircle(foot.position, radioFoot, floor);

    if (inFloor) {
      animator.SetBool("inFloor", true);

      if (Input.GetKeyDown(KeyCode.Space) && !isDown) {
        GetComponent <Rigidbody2D>().AddForce (new Vector2(0, jumpingForce));
        animator.SetBool("inFloor", false);
      }
    } else {
      animator.SetBool("inFloor", false);
    }

    // Crouch
    if (inFloor && Input.GetKey(KeyCode.DownArrow)) {
      animator.SetBool("isDown", true);
      isDown = true;
    } else {
      animator.SetBool("isDown", false);
      isDown = false;
    }

    // Look up
    if (inputX == 0) {
      if (inFloor && Input.GetKey(KeyCode.UpArrow)) {
        animator.SetBool("lookUp", true);
        lookUp = true;
      } else {
        animator.SetBool("lookUp", false);
        lookUp = false;
      }
    }
  }
}
