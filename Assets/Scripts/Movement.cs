using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour {

  // Movement variables
  public float velX = 0.03f;
  public float movX;
  public float currentPosition;
  public bool lookingRight;

  // Jump variables
  public float jumpingForce = 100f;
  public Transform foot;
  public float radioFoot = 0.08f;
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

  // Run variables
  public bool run;

  // Turbo variables
  public bool turbo;

  // Turbo jump variables
  public bool turboJump;

  // Shell variables
  public float kick = 500f;
  public Transform hand;
  public float radioHand = 0.07f;
  public LayerMask shell;
  public bool getShell;
  public GameObject Shell;
  public GameObject Mario;

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
        lookingRight = true;
      }

      if (inputX < 0) {
        movX = transform.position.x + (inputX * velX);
        transform.position = new Vector3(movX, transform.position.y, 0);
        transform.localScale = new Vector3(-1, 1, 1);
        movX = currentPosition;
        lookingRight = false;
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

      if (Input.GetKeyDown(KeyCode.X) && !isDown) {
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

    // Run
    if (inputX > 0 || inputX < 0) {
      if (Input.GetKey(KeyCode.Z)) {
        run = true;
        velX = 0.06f;
        animator.SetBool("run", true);
        StartCoroutine(Turbo());
      } else {
        run = false;
        velX = 0.03f;
        turbo = false;
        animator.SetBool("run", false);
      }
    }

    if (inputX == 0) {
      animator.SetBool("run", false);
      animator.SetBool("turbo", false);
      animator.SetBool("turboJump", false);
    }

    // Turbo
    if (inputX > 0 || inputX < 0) {
      if (turbo) {
        animator.SetBool("turbo", true);
      } else {
        animator.SetBool("turbo", false);
      }
    }

    // Turbo jump
    if (inputX > 0 || inputX < 0) {
      if (turbo && Input.GetKey(KeyCode.X)) {
        animator.SetBool("turboJump", true);
      } else {
        animator.SetBool("turboJump", false);
      }
    }

    // Shell
    getShell = Physics2D.OverlapCircle(hand.position, radioHand, shell);
    if (getShell && lookingRight) {
      if (Input.GetKey(KeyCode.Z)) {
        Shell.transform.parent = Mario.transform;
        Shell.GetComponent<Rigidbody2D>().gravityScale = 0;
        Shell.GetComponent<Rigidbody2D>().isKinematic = true;
      } else {
        Shell.GetComponent<Rigidbody2D>().AddForce(new Vector2(kick, 0));
        Shell.transform.parent = null;
        Shell.GetComponent<Rigidbody2D>().gravityScale = 2;
        Shell.GetComponent<Rigidbody2D>().isKinematic = false;
      }
    }

    if (getShell && !lookingRight) {
      if (Input.GetKey(KeyCode.Z)) {
        Shell.transform.parent = Mario.transform;
        Shell.GetComponent<Rigidbody2D>().gravityScale = 0;
        Shell.GetComponent<Rigidbody2D>().isKinematic = true;
      } else {
        Shell.GetComponent<Rigidbody2D>().AddForce(new Vector2(kick*-1, 0));
        Shell.transform.parent = null;
        Shell.GetComponent<Rigidbody2D>().gravityScale = 2;
        Shell.GetComponent<Rigidbody2D>().isKinematic = false;
      }
    }
  }

  // Coroutines
  public IEnumerator WaitTime() {
    yield return new WaitForSeconds(0.3f);
    skid = 0;
    right = 0;
    left = 0;
    animator.SetBool("skid", false);
  }

  public IEnumerator Turbo() {
    yield return new WaitForSeconds(0.5f);

    if (run) {
      velX = 0.15f;
      turbo = true;
    } else {
      StopCoroutine(Turbo());
    }
  }
}
