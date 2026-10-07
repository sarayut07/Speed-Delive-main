using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StairZone : MonoBehaviour
{
    [Range(0.1f, 1f)]
    [Tooltip("ความเร็วขณะอยู่บนบันได (0.65 = ช้าลง 35%)")]
    [SerializeField] private float speedMultiplier = 0.65f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        HorseCourierController player = other.GetComponent<HorseCourierController>();
        if (player != null)
        {
            player.SetSpeedModifier(speedMultiplier);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        HorseCourierController player = other.GetComponent<HorseCourierController>();
        if (player != null)
        {
            player.ResetSpeed();
        }
    }
}
