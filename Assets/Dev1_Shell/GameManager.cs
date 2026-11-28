using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Screens")]
    public GameObject titleScreen;
    public GameObject[] lockPanels; // Size 3: Lock1, Lock2, Lock3
    public GameObject winScreen;
    public GameObject loseScreen;

    [Header("HUD & End Text")]
    public GameObject hudBar;
    public TMP_Text timerText;
    public TMP_Text stageText;
    public TMP_Text winSummaryText;

    [Header("Audio & Particles")]
    public AudioSource sfxSource;
    public AudioClip clickClip;
    public AudioClip errorClip;
    public AudioClip unlockClip;
    public ParticleSystem confettiParticles;

    [Header("Game Settings")]
    public float timeLimit = 45f;

    private int currentLockIndex = -1;
    private float timeRemaining;
    private bool isPlaying = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ShowOnly(titleScreen);
        hudBar.SetActive(false);
    }

    void Update()
    {
        if (!isPlaying) return;

        timeRemaining -= Time.deltaTime;
        timerText.text = $"MOM RETURNS IN: {Mathf.Max(0f, timeRemaining):F1}s";

        if (timeRemaining <= 0f)
        {
            TriggerLose();
        }
    }

    public void StartGame()
    {
        PlayClick();
        timeRemaining = timeLimit;
        currentLockIndex = 0;
        isPlaying = true;
        hudBar.SetActive(true);
        LoadCurrentLock();
    }

    public void CompleteCurrentLock()
    {
        if (!isPlaying) return;

        if (sfxSource != null && unlockClip != null) sfxSource.PlayOneShot(unlockClip);
        if (confettiParticles != null) confettiParticles.Play();

        currentLockIndex++;

        if (currentLockIndex >= lockPanels.Length)
        {
            TriggerWin();
        }
        else
        {
            LoadCurrentLock();
        }
    }

    private void LoadCurrentLock()
    {
        ShowOnly(lockPanels[currentLockIndex]);
        stageText.text = $"LOCK {currentLockIndex + 1} / {lockPanels.Length}";
    }

    private void TriggerWin()
    {
        isPlaying = false;
        hudBar.SetActive(false);
        if (winSummaryText != null)
        {
            winSummaryText.text = $"COOKIE SECURED WITH {timeRemaining:F1}s TO SPARE!";
        }
        ShowOnly(winScreen);
    }

    private void TriggerLose()
    {
        isPlaying = false;
        hudBar.SetActive(false);
        PlayError();
        ShowOnly(loseScreen);
    }

    private void ShowOnly(GameObject target)
    {
        titleScreen.SetActive(target == titleScreen);
        winScreen.SetActive(target == winScreen);
        loseScreen.SetActive(target == loseScreen);

        foreach (GameObject panel in lockPanels)
        {
            if (panel != null) panel.SetActive(panel == target);
        }
    }

    public void PlayClick()
    {
        if (sfxSource != null && clickClip != null) sfxSource.PlayOneShot(clickClip);
    }

    public void PlayError()
    {
        if (sfxSource != null && errorClip != null) sfxSource.PlayOneShot(errorClip);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}