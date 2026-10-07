using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target to Follow")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // กล้อง 2D แกน Z ต้องเป็น -10 เสมอ

    private void LateUpdate()
    {
        if (target == null) return;

        // คำนวณตำแหน่งเป้าหมายที่ต้องการให้กล้องไปอยู่
        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, offset.z);

        // เลื่อนกล้องตามอย่างนุ่มนวลด้วย Lerp
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}
