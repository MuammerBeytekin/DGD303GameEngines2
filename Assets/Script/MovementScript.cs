using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float rotationSpeed = 10f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Camera")]
    public Transform cam; // Sahnedeki kamerayý buraya sürükle-býrak

    [Header("Stretch Settings")]
    public float stretchSpeed = 5f; // Geçiþ hýzý
    public Vector3 normalScale = Vector3.one; // Normal boy
    public Vector3 tallScale = new Vector3(0.7f, 2f, 0.7f); // Uzayýnca incel
    public Vector3 shortScale = new Vector3(1.5f, 0.5f, 1.5f); // Kýsalýnca geniþler

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cam == null && Camera.main != null)
            cam = Camera.main.transform;
    }

    void Update()
    {
        GroundCheck();
        Move();
        Jump();
        ApplyGravity();
        StretchCharacter();
    }

    void GroundCheck()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(horizontal, 0, vertical).normalized;

        if (move.magnitude >= 0.1f)
        {
            // Kamera yönüne göre hareket yönü hesapla
            Vector3 camForward = cam.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 camRight = cam.right;
            camRight.y = 0;
            camRight.Normalize();

            Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;

            // Karakterin yönünü hareket yönüne döndür
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Yürüme/koþma hýzý
            float speed = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? runSpeed : walkSpeed;

            controller.Move(moveDir * speed * Time.deltaTime);
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void StretchCharacter()
    {
        Vector3 targetScale = normalScale;

        if (Input.GetKey(KeyCode.Alpha1)) // 1 tuþu: uzasýn ve incelsin
        {
            targetScale = tallScale;
        }
        else if (Input.GetKey(KeyCode.Alpha2)) // 2 tuþu: kýsalsýn ve geniþlesin
        {
            targetScale = shortScale;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * stretchSpeed);
    }
}
