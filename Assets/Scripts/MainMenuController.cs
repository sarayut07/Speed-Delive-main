using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // ฟังก์ชันสำหรับกดเริ่มเกม
    public void StartGame()
    {
        // โหลดฉากเกมที่ชื่อว่า Speedy Dlive
        SceneManager.LoadScene("Speedy Dlive");
    }

    // ฟังก์ชันสำหรับกดออกจากเกม (ใช้ได้ตอน Build เป็นตัวเกมจริง)
    public void QuitGame()
    {
        Application.Quit();
    }
}
