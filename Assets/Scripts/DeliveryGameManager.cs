using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliveryGameManager : MonoBehaviour
{
  public static DeliveryGameManager Instance { get; private set; }

    [Header("Level Time Settings")]
    [SerializeField] private float initialTimeLimit = 90f;
    private float currentTime;
    private float elapsedTime;
    private bool isGameActive;

    [Header("3-Star Thresholds (Elapsed Time)")]
    [SerializeField] private float threeStarTime = 50f;
    [SerializeField] private float twoStarTime = 75f;

    [Header("Delivery Route (Ordered Targets)")]
    [SerializeField] private List<DeliveryTarget> deliveryTargets = new List<DeliveryTarget>();
    private int currentTargetIndex = 0;

    [Header("Scoring & Multiplier")]
    private int score = 0;
    private int money = 0;
    private int streak = 0;
    private bool hadCollision = false;

    [Header("UI Elements (Assign in Inspector)")]
    [SerializeField] private Slider timerBar;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI streakText;
    [SerializeField] private TextMeshProUGUI targetIndicatorText;

    [Header("Result Screen UI")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] private TextMeshProUGUI rewardBreakdownText;
    [SerializeField] private TextMeshProUGUI totalMoneyText;

    public bool IsGameActive => isGameActive;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadSaveData();
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (!isGameActive) return;

        currentTime -= Time.deltaTime;
        elapsedTime += Time.deltaTime;

        if (timerBar != null)
        {
            timerBar.value = Mathf.Clamp01(currentTime / initialTimeLimit);
        }

        if (timerText != null)
        {
            timerText.text = $"{Mathf.Max(0, currentTime):00.0}s";
        }

        if (currentTime <= 0f)
        {
            GameOver(false);
        }
    }

    public void StartGame()
    {
        currentTime = initialTimeLimit;
        elapsedTime = 0f;
        score = 0;
        streak = 0;
        currentTargetIndex = 0;
        hadCollision = false;
        isGameActive = true;

        if (resultPanel != null) resultPanel.SetActive(false);
        UpdateUI();
        ActivateCurrentTarget();
    }

    private void ActivateCurrentTarget()
    {
        for (int i = 0; i < deliveryTargets.Count; i++)
        {
            deliveryTargets[i].SetTargetActive(i == currentTargetIndex);
        }

        if (targetIndicatorText != null)
        {
            if (currentTargetIndex < deliveryTargets.Count)
                targetIndicatorText.text = $"จุดหมาย: บ้านที่ {currentTargetIndex + 1}/{deliveryTargets.Count}";
            else
                targetIndicatorText.text = "ภารกิจเสร็จสิ้น!";
        }
    }

    public void CompleteTargetDelivery(DeliveryTarget target)
    {
        if (!isGameActive || target != deliveryTargets[currentTargetIndex]) return;

        streak++;
        int streakBonus = streak * 15;
        score += 100 + streakBonus;

        currentTargetIndex++;
        UpdateUI();

        if (currentTargetIndex >= deliveryTargets.Count)
        {
            GameOver(true);
        }
        else
        {
            ActivateCurrentTarget();
        }
    }

    public void AddTimeBonus(float bonusSeconds)
    {
        if (!isGameActive) return;
        currentTime = Mathf.Min(initialTimeLimit, currentTime + bonusSeconds);
        score += 50;
        UpdateUI();
    }

    public void RegisterObstacleHit()
    {
        streak = 0;
        hadCollision = true;
        UpdateUI();
    }

    private void GameOver(bool victory)
    {
        isGameActive = false;

        if (victory)
        {
            int baseReward = 150;
            int timeBonusMoney = Mathf.FloorToInt(currentTime * 2f);
            float currentBest = PlayerPrefs.GetFloat("Medieval_BestTime", 9999f);
            bool isNewRecord = elapsedTime < currentBest;

            if (isNewRecord)
            {
                PlayerPrefs.SetFloat("Medieval_BestTime", elapsedTime);
                currentBest = elapsedTime;
            }

            int recordBonus = isNewRecord ? Mathf.FloorToInt(baseReward * 0.2f) : 0;
            int earnedMoney = baseReward + timeBonusMoney + recordBonus;

            money += earnedMoney;
            PlayerPrefs.SetInt("Medieval_Gold", money);
            PlayerPrefs.Save();

            DisplayResults(true, elapsedTime, currentBest, earnedMoney, timeBonusMoney, recordBonus, isNewRecord);
        }
        else
        {
            DisplayResults(false, elapsedTime, PlayerPrefs.GetFloat("Medieval_BestTime", 0f), 0, 0, 0, false);
        }
    }

    private void DisplayResults(bool win, float time, float best, int totalReward, int timeBonus, int recordBonus, bool newRecord)
    {
        if (resultPanel == null) return;
        resultPanel.SetActive(true);

        finalTimeText.text = $"เวลา: {time:00.00} วิ";
        bestTimeText.text = $"สถิติดีที่สุด: {best:00.00} วิ {(newRecord ? "<color=yellow>(NEW RECORD!)</color>" : "")}";

        if (win)
        {
            int stars = 1;
            if (time <= threeStarTime) stars = 3;
            else if (time <= twoStarTime) stars = 2;

            starsText.text = new string('★', stars) + new string('☆', 3 - stars);
            rewardBreakdownText.text = $"รางวัลพื้นฐาน: +150G\nโบนัสเวลาคงเหลือ: +{timeBonus}G\nโบนัสสถิติใหม่: +{recordBonus}G";
            totalMoneyText.text = $"รวมได้รับ: +{totalReward} Gold (สะสม: {money}G)";
        }
        else
        {
            starsText.text = "หมดเวลา (FAILED)";
            rewardBreakdownText.text = "ส่งของไม่ทันตามกำหนด";
            totalMoneyText.text = $"ทองสะสม: {money}G";
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
        if (streakText != null) streakText.text = streak > 1 ? $"Streak x{streak}!" : "";
    }

    private void LoadSaveData()
    {
        money = PlayerPrefs.GetInt("Medieval_Gold", 0);
    }  
}
