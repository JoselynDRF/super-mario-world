using UnityEngine;
using System.Collections;

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

  // Fall out variables
  public float fallDown;
  Rigidbody2D rb;

  // Skid variables
  public int skid;
  public int right;
  public int left;

  // Run variables
  public bool run;

  // Turbo variables
  public bool turbo;
  public int turboCounter;

  // Turbo jump variables
  public bool turboJump;

  // Shell variables
  public float kick = 500f;
  public Transform hand;
  public float radioHand = 0.07f;
  public LayerMask shell;
  public bool getShell;
  public GameObject Shell;

  // Animations
  Animator animator;

  void Awake() {
    animator = GetComponent <Animator>();
    rb = GetComponent <Rigidbody2D>();
  }

  // Start is called before the first frame update
  void Start() { }

  // Update is called once per frame
  void FixedUpdate() {
    float inputX = Input.GetAxis("Horizontal");                               // Almacena el movimiento en el eje X
		movX = transform.position.x + (inputX * velX);                            // movX será igual a mi posición en X + el movimiento en el eje X * velX
		currentPosition = movX;                                                   // Almacena la posición actual

    // MOVE
    if (!isDown && !lookUp) { 
      if (inputX > 0) {                                                       // Si la velocidad en el eje X es menor que 0
        transform.position = new Vector3(movX, transform.position.y, 0);      // Mi posicion = movX, la posición que tenga en Y, 0
        transform.localScale = new Vector3(1, 1, 1);                          // La escala original si me muevo a la derecha
        lookingRight = true;                                                  // Establece que estoy mirando a la derecha
        animator.SetBool("skid", false);                                      // Le indica al animador que estoy derrapando
      }

      if (inputX < 0) {
        transform.position = new Vector3(movX, transform.position.y, 0);
        transform.localScale = new Vector3(-1, 1, 1);                         // La escala inversa si me muevo a la izquierda
        lookingRight = false;
        animator.SetBool("skid", false);
      }
    }

    if (inputX != 0) {
      animator.SetFloat("velX", 1);
    } else {
      animator.SetFloat("velX", 0);
    }

    // JUMP
    inFloor = Physics2D.OverlapCircle(foot.position, radioFoot, floor);       // Si estoy tocando el suelo será true

    if (inFloor) {
      animator.SetBool("inFloor", true);
	    animator.SetBool("turboJump", false);

      if (Input.GetKeyDown(KeyCode.C) && !isDown) {                           // Si pulso la tecla C y no estoy agachado
        GetComponent <Rigidbody2D>().AddForce(new Vector2(0, jumpingForce));  // Accede a la velocidad del Rigidbody2D y le añado la fuerza vertical establecida en jumpingForce
        animator.SetBool("inFloor", false);
      }
    } else {
      animator.SetBool("inFloor", false);
    }

    // CROUCH
    if (inFloor && Input.GetKey(KeyCode.DownArrow)) {
      isDown = true;
      animator.SetBool("isDown", true);
    } else {
      isDown = false;
      animator.SetBool("isDown", false);
    }

    // LOOK UP
    if (inputX == 0) {
      if (inFloor && Input.GetKey(KeyCode.UpArrow)) {
        lookUp = true;
        animator.SetBool("lookUp", true);
      } else {
        lookUp = false;
        animator.SetBool("lookUp", false);
      }
    }

    // FALL DOWN
    fallDown = rb.velocity.y;                                                 // Establece que la caída es igual a la velocidad en el eje Y

    if (fallDown != 0 || fallDown == 0) {
      animator.SetFloat("velY", fallDown);
    }

    // SKID
    if (inputX == 0) {
      StartCoroutine(WaitTime());                                             // Llama a la corutina WaitTime()
    }

    // Skid right --> left
    if (inputX > 0.5f) {
      right = 1;
      skid = 1;
    }

    if (skid == 1 && Input.GetKey(KeyCode.LeftArrow)) {
      animator.SetBool("skid", true);
      animator.SetBool("turbo", false);
      StartCoroutine(WaitTime());
      StopCoroutine(Turbo());
    }

    // Skid left --> right
    if (inputX < 0) {
      left = 1;
      skid = -1;
    }

    if (skid == -1 && Input.GetKey(KeyCode.RightArrow)) {
      animator.SetBool("skid", true);
      animator.SetBool("turbo", false);
      StartCoroutine(WaitTime());
    }

    // RUN
    if (inputX != 0) {
      if (Input.GetKey(KeyCode.X)) {
        run = true;
        velX = 0.06f;
        animator.SetBool("run", true);
      } else {
        velX = 0.03f;
        run = false;
        animator.SetBool("run", false);
        turboCounter = 0;
      }
    } else {                                                                  // Si no me estoy moviendo reestablece los valores del código y del animador por defecto
	    run = false;
			animator.SetBool("run", false);
			animator.SetBool("skid", false);
			turboCounter = 0;
    }

    // TURBO
 		if (Input.GetKey(KeyCode.X) && run && inFloor) {
			StartCoroutine(Turbo());
		} else {
			turbo = false;
			animator.SetBool ("turbo",false);
			StopAllCoroutines();                                                    // Detener las corrutinas
		}

    // TURBO JUMP
    if (inputX > 0 || inputX < 0) {
   		if (turbo && Input.GetKeyDown(KeyCode.C)) {
				animator.SetBool("turboJump", true);
			}
    }

    // SHELL
    getShell = Physics2D.OverlapCircle(hand.position, radioHand, shell);      // Cuando la mano entre en el radio de colisión con la concha

    if (getShell && lookingRight) {                                           // Si estoy en el radio de colisión y estoy mirando a la derecha
      if (Input.GetKey(KeyCode.X)) {
        Shell.transform.parent = this.transform;                              // Establece que la concha sea hija de Mario para que tengan el mismo movimiento
        Shell.GetComponent<Rigidbody2D>().gravityScale = 0;                   // Desactiva la gravedad de la concha
        Shell.GetComponent<Rigidbody2D>().isKinematic = true;                 // Activa kinematic para que no le afecten las fuerzas
      } else {
        Shell.GetComponent<Rigidbody2D>().AddForce(new Vector2(kick, 0));     // Añade una fuerza al eje X para que simule la patada a la concha
        Shell.transform.parent = null;                                        // La concha pasa a ser padre
        Shell.GetComponent<Rigidbody2D>().gravityScale = 3;                   // Activa la gravedad de la concha y la establece en 3
        Shell.GetComponent<Rigidbody2D>().isKinematic = false;                // Desactiva kinematic para que le vuelvan a afectar las fuerzas
      }
    }

    if (getShell && !lookingRight) {                                          // Si estoy en el radio de colisión y no estoy mirando a la derecha
      if (Input.GetKey(KeyCode.X)) {
        Shell.transform.parent = this.transform;
        Shell.GetComponent<Rigidbody2D>().gravityScale = 0;
        Shell.GetComponent<Rigidbody2D>().isKinematic = true;
      } else {
        Shell.GetComponent<Rigidbody2D>().AddForce(new Vector2(kick*(-1), 0));
        Shell.transform.parent = null;
        Shell.GetComponent<Rigidbody2D>().gravityScale = 3;
        Shell.GetComponent<Rigidbody2D>().isKinematic = false;
      }
    }
  }

  // Coroutines
  public IEnumerator WaitTime() {                                             // Establecer un tiempo de espera desde el cambio de dirección hasta que se reestablezcan los valores
    yield return new WaitForSeconds(0.3f);                                    // Esperar 0.3 "segundos"
    skid = 0;
    right = 0;
    left = 0;
    animator.SetBool("skid", false);
  }

  public IEnumerator Turbo() {                                                // Activar el turbo
    while (turboCounter <= 15) {
      yield return new WaitForSeconds(1.5f);
      turboCounter++;
    }

		turbo = true;
		animator.SetBool("turbo",true);
		velX = 0.12f;
		StopAllCoroutines();
  }
}
