﻿using System.Collections;
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

  // Skid variables
  public int skid;
  public int right;
  public int left;

  // Animations
  Animator animator;

  // Fall out variables
  Rigidbody2D rb;
  public float fallDown;

  void Awake() {
    animator = GetComponent <Animator>();
    rb = GetComponent <Rigidbody2D>();
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

    // Fall down
    fallDown = rb.velocity.y;

    if (fallDown != 0 || fallDown == 0) {
      animator.SetFloat("velY", fallDown);
    }

    // Skid
    if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)) {
      StartCoroutine(WaitTime());
    }

    // Skid right --> left
    if (inputX > 0.5f) {
      right = 1;
    }

    if (right == 1) {
      skid = 1;
    }

    if (skid == 1 && Input.GetKey(KeyCode.LeftArrow)) {
      animator.SetBool("skid", true);
      StartCoroutine(WaitTime());
    }

    // Skid left --> right
    if (inputX < 0) {
      left = 1;
    }

    if (left == 1) {
      skid = -1;
    }

    if (skid == -1 && Input.GetKey(KeyCode.RightArrow)) {
      animator.SetBool("skid", true);
      StartCoroutine(WaitTime());
    }  
  }

  // Coroutine
  public IEnumerator WaitTime() {
    yield return new WaitForSeconds(0.3f);
    skid = 0;
    right = 0;
    left = 0;
    animator.SetBool("skid", false);
  }
}
