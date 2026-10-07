using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MedievalObstacle : MonoBehaviour
{
    public enum ObstacleType { StrayDog, StaticCrate }
    [SerializeField] private ObstacleType type = ObstacleType.StrayDog;
    [SerializeField] private float stunTime = 1.2f;

    [Header("Dash Across Behavior (วิ่งตัดหน้า)")]
    [SerializeField] private bool triggerOnApproach = true; // ตรวจจับเมื่อผู้เล่นเข้าใกล้
    [SerializeField] private float triggerDistance = 4.0f;   // ระยะตรวจจับ (เมื่อเข้าใกล้ถึงระยะนี้จะพุ่ง)
    [SerializeField] private Vector2 dashDirection = Vector2.left; // ทิศทางที่พุ่งตัดหน้า (เช่น ซ้ายหรือขวา)
    [SerializeField] private float dashSpeed = 6.0f;         // ความเร็วในการวิ่งพุ่ง
    [SerializeField] private float dashDistance = 5.0f;      // ระยะทางที่วิ่งข้ามถนน

    private Transform playerTransform;
    private Vector2 startPos;
    private Vector2 targetPos;
    private bool hasTriggered = false;
    private bool isMoving = false;

    private void Start()
    {
        startPos = transform.position;
        targetPos = startPos + (dashDirection.normalized * dashDistance);

        // ค้นหาตำแหน่งผู้เล่นอัตโนมัติ
        HorseCourierController player = FindAnyObjectByType<HorseCourierController>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (type != ObstacleType.StrayDog) return;

        // เมื่อผู้เล่นเข้าใกล้ในระยะ triggerDistance จะเริ่มวิ่งพุ่งตัดหน้า
        if (triggerOnApproach && !hasTriggered && playerTransform != null)
        {
            float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distToPlayer <= triggerDistance)
            {
                hasTriggered = true;
                isMoving = true;
            }
        }

        // กำลังวิ่งข้ามถนน
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, dashSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, targetPos) < 0.05f)
            {
                isMoving = false; // วิ่งถึงอีกฝั่งแล้วหยุด
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HorseCourierController rider = other.GetComponent<HorseCourierController>();
        if (rider != null && !rider.IsInAir())
        {
            rider.ApplyStumble(stunTime);
        }
    }

    // วาดเส้นและระยะตรวจจับในหน้าจอ Scene ให้มองเห็นง่ายๆ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
        Gizmos.color = Color.red;
        Vector3 target = transform.position + (Vector3)(dashDirection.normalized * dashDistance);
        Gizmos.DrawLine(transform.position, target);
    }
}
