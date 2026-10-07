using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    [Tooltip("เวลาเริ่มต้น (วินาที) เช่น 60 วินาที = 1 นาที")]
    [SerializeField] private float timeRemaining = 60f;
    private bool isTimerRunning = true;

    void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                isTimerRunning = false;
                DisplayTime(timeRemaining);
                TimeUp();
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        // คำนวณเป็น นาที และ วินาที
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // แสดงผลเป็นรูปแบบ 00:00
        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void TimeUp()
    {
        Debug.Log("หมดเวลาแล้ว!");
        // ใส่โค้ดจบเกม หรือแจ้งเตือนภารกิจล้มเหลวตรงนี้ได้ในอนาคต
    }
}
