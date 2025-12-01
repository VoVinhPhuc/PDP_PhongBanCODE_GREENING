using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 
using System.Collections; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public int targetTrees = 5;      
    public float timeLimit = 180f;   
    public float rainStartTime = 120f; 

    [Header("Current State")]
    public float elapsedTime = 0f;
    public int currentTrees = 0;
    public bool isGameOver = false;
    public bool isGameStarted = false; 
    private bool hasShownWarning = false; // Kiểm tra đã hiện cảnh báo chưa

    [Header("Audio Settings (NEW)")]
    public AudioSource musicSource;   // Nguồn phát nhạc nền (Loop)
    public AudioSource sfxSource;     // Nguồn phát âm thanh nút (One Shot)
    
    public AudioClip normalMusic;     // Nhạc nền bình thường
    public AudioClip intenseMusic;    // Nhạc kịch tính (30s cuối)
    public AudioClip clickSound;      // Tiếng bấm nút
    public AudioClip winSound;        // Tiếng thắng (Tùy chọn)
    public AudioClip loseSound;       // Tiếng thua (Tùy chọn)

    [Header("UI Panels")]
    public GameObject startPanel;    
    public GameObject tutorialPanel; 
    public GameObject winPanel;      
    public GameObject losePanel;     

    [Header("UI Text")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI seedText;
    public TextMeshProUGUI loseReasonText; 
    public TextMeshProUGUI warningText; 

    [Header("Environment")]
    public GameObject rainParticle;
    public GameObject floodWater; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 0; 
        isGameStarted = false;
        hasShownWarning = false;

        // --- SETUP AUDIO ---
        if (musicSource != null && normalMusic != null)
        {
            musicSource.clip = normalMusic;
            musicSource.loop = true; // Lặp lại liên tục
            musicSource.Play();
        }
        // ------------------

        if(warningText) warningText.gameObject.SetActive(false);

        if(startPanel) startPanel.SetActive(true);
        if(tutorialPanel) tutorialPanel.SetActive(false);
        if(winPanel) winPanel.SetActive(false);
        if(losePanel) losePanel.SetActive(false);
        if(floodWater) floodWater.SetActive(false); 

        UpdateUI();
    }

    void Update()
    {
        if (!isGameStarted || isGameOver) return;

        elapsedTime += Time.unscaledDeltaTime; 
        float timeRemaining = timeLimit - elapsedTime;
        
        // --- LOGIC 30 GIÂY CUỐI: CẢNH BÁO & ĐỔI NHẠC ---
        if (timeRemaining <= 30f && timeRemaining > 0f && !hasShownWarning)
        {
            StartCoroutine(ShowWarningText());
            SwitchToIntenseMusic(); // Gọi hàm đổi nhạc
            hasShownWarning = true; 
        }
        // -----------------------------------------------

        if (elapsedTime >= rainStartTime)
        {
            if(rainParticle && !rainParticle.activeSelf) rainParticle.SetActive(true);
            if(floodWater && !floodWater.activeSelf) floodWater.SetActive(true);
        }

        if (timeRemaining <= 0)
        {
            TriggerGameOver(false, "Hết thời gian! Nước lũ đã nhấn chìm tất cả.");
        }

        UpdateUI(timeRemaining);
    }

    // --- HÀM ĐỔI NHẠC KỊCH TÍNH ---
    void SwitchToIntenseMusic()
    {
        if (musicSource != null && intenseMusic != null)
        {
            musicSource.Stop();
            musicSource.clip = intenseMusic; // Đổi sang bài nhạc căng thẳng
            musicSource.Play();
        }
    }

    // --- HÀM PHÁT TIẾNG CLICK (Gắn vào nút bấm) ---
    public void PlayButtonSound()
    {
        if (sfxSource != null && clickSound != null)
        {
            sfxSource.PlayOneShot(clickSound); // Phát 1 lần, không cắt ngang nhạc nền
        }
    }

    IEnumerator ShowWarningText()
    {
        if (warningText != null)
        {
            warningText.text = "CHỈ CÒN 30 GIÂY. Nhanh lên!"; 
            warningText.gameObject.SetActive(true); 
            warningText.color = Color.red; 
            yield return new WaitForSeconds(5f); 
            warningText.gameObject.SetActive(false); 
        }
    }

    // --- CÁC HÀM NÚT BẤM (Đã thêm PlayButtonSound) ---

    public void Button_PressStart()
    {
        PlayButtonSound(); // Phát tiếng
        if(startPanel) startPanel.SetActive(false);
        if(tutorialPanel) tutorialPanel.SetActive(true);
    }

    public void Button_ConfirmTutorial() 
    {
        PlayButtonSound(); // Phát tiếng
        if(tutorialPanel) tutorialPanel.SetActive(false);
        StartGameplayLogic();
    }

    public void Button_QuitGame()
    {
        PlayButtonSound(); // Phát tiếng
        Debug.Log("Đang thoát game..."); 
        Application.Quit(); 
    }

    private void StartGameplayLogic()
    {
        isGameStarted = true;
        Time.timeScale = 1; 
    }

    public void AddScore()
    {
        currentTrees++;
        UpdateUI();
        if (currentTrees >= targetTrees)
        {
            TriggerGameOver(true, "Chúc mừng! Bạn đã phủ xanh đồi trọc!");
        }
    }

    public void UpdateSeedUI(int count)
    {
        if (seedText != null) seedText.text = "SEED: " + count;
    }

    public void TriggerGameOver(bool isWin, string message = "")
    {
        if (isGameOver) return; 
        isGameOver = true;
        Time.timeScale = 0; 
        musicSource.Stop(); // Dừng nhạc nền

        if(warningText) warningText.gameObject.SetActive(false);

        if (isWin)
        {
            if(winPanel) winPanel.SetActive(true);
            if(sfxSource && winSound) sfxSource.PlayOneShot(winSound); // Âm thanh thắng
        }
        else
        {
            if(losePanel) losePanel.SetActive(true);
            if(loseReasonText) loseReasonText.text = message;
            if(sfxSource && loseSound) sfxSource.PlayOneShot(loseSound); // Âm thanh thua
        }
    }

    void UpdateUI(float timeDisplay = 0)
    {
        if(scoreText) scoreText.text = $"{currentTrees} / {targetTrees}";
        
        if(timerText)
        {
            if (!isGameStarted) timeDisplay = timeLimit;
            if(timeDisplay < 0) timeDisplay = 0;
            
            int minutes = Mathf.FloorToInt(timeDisplay / 60F);
            int seconds = Mathf.FloorToInt(timeDisplay % 60F);
            timerText.text = $"{minutes:00}:{seconds:00}";

            if (timeDisplay <= 30) timerText.color = Color.red;
            else timerText.color = Color.white;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}