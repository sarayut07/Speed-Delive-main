using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TimeBonusItem : MonoBehaviour
{
    [SerializeField] private float bonusSeconds = 10f;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<HorseCourierController>() != null)
        {
            DeliveryGameManager.Instance.AddTimeBonus(bonusSeconds);
            Destroy(gameObject);
        }
    }
}
