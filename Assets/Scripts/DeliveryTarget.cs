using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeliveryTarget : MonoBehaviour
{
    [SerializeField] private GameObject activeIndicator; // ใส่รูปลูกศรชี้เป้าหมายบนหลังคาบ้าน
    private bool isActiveTarget = false;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void SetTargetActive(bool active)
    {
        isActiveTarget = active;
        if (activeIndicator != null) activeIndicator.SetActive(active);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActiveTarget && other.GetComponent<HorseCourierController>() != null)
        {
            DeliveryGameManager.Instance.CompleteTargetDelivery(this);
        }
    }
}
