using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class HorseCourierController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float boostMultiplier = 1.5f;

    [Header("Jump / Dodge")]
    [SerializeField] private float jumpDuration = 0.6f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private Vector2 movementInput;
    private bool isBoosting = false;
    private bool isJumping = false;
    private bool isStunned = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isStunned) return;

        // รับค่าปุ่มเดิน
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(moveX, moveY).normalized;

        // สั่งให้ Animator หยุดเล่นเมื่อไม่ได้กดเดิน (ยืนนิ่ง)
        if (anim != null)
        {
            bool isMoving = movementInput.sqrMagnitude > 0.01f;
            anim.speed = isMoving ? 1f : 0f; // ถ้าหยุดเดินให้เฟรมหยุดนิ่ง ถ้าเดินให้ขยับขาปกติ
        }

        // พลิกตัวละคร ซ้าย/ขวา อัตโนมัติ (Flip X)
        if (spriteRenderer != null)
        {
            if (moveX > 0.1f) spriteRenderer.flipX = false; // เดินขวา
            else if (moveX < -0.1f) spriteRenderer.flipX = true; // เดินซ้าย
        }

        // กดเร่งความเร็ว
        isBoosting = Input.GetKey(KeyCode.LeftShift);

        // กดกระโดด
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            StartCoroutine(JumpRoutine());
        }
    }

    private void FixedUpdate()
    {
        if (isStunned || rb == null) return;

        float currentSpeed = moveSpeed * (isBoosting ? boostMultiplier : 1f);
        rb.linearVelocity = movementInput * currentSpeed;
    }

    private IEnumerator JumpRoutine()
    {
        isJumping = true;
        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * 1.25f;

        yield return new WaitForSeconds(jumpDuration);

        transform.localScale = originalScale;
        isJumping = false;
    }

    public void ApplyStumble(float duration)
    {
        if (!isStunned)
        {
            StartCoroutine(StumbleRoutine(duration));
        }
    }

    private IEnumerator StumbleRoutine(float duration)
    {
        isStunned = true;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    public bool IsInAir()
    {
        return isJumping;
    }

    // เก็บค่าความเร็วเดิม
    private float defaultSpeed = -1f;

    // ฟังก์ชันปรับตัวคูณความเร็ว (เช่น ส่งค่า 0.65f = ชะลอความเร็วลง 35%)
    public void SetSpeedModifier(float multiplier)
    {
        if (defaultSpeed < 0f) defaultSpeed = moveSpeed;
        moveSpeed = defaultSpeed * multiplier;
    }

    // ฟังก์ชันคืนค่าความเร็วเดิม
    public void ResetSpeed()
    {
        if (defaultSpeed > 0f)
        {
            moveSpeed = defaultSpeed;
        }
    }
}