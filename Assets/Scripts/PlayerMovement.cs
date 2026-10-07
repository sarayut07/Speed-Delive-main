using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    private float currentSpeed;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        bool isMoving = movement.sqrMagnitude > 0;
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);

        currentSpeed = (isShiftPressed && isMoving) ? runSpeed : walkSpeed;

        if (animator != null)
        {
            if (isMoving)
            {
                animator.SetFloat("MoveX", movement.x);
                animator.SetFloat("MoveY", movement.y);

                // พลิกด้านรูปภาพเฉพาะตอนเดินซ้าย
                // ในส่วน Update() เมื่อมีการเคลื่อนที่แกน X
                if (movement.x < -0.01f)
                 {
                     spriteRenderer.flipX = true;  // หันซ้าย
                 }
                else if (movement.x > 0.01f)
                 {
                    spriteRenderer.flipX = false; // หันขวา
                 }
            }

            animator.SetFloat("Speed", movement.sqrMagnitude);
            animator.SetBool("isRunning", isShiftPressed && isMoving);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
    }
}