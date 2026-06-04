using UnityEngine;
using TMPro; 
using System.Collections; 
// TAMBAHAN: Wajib dipanggil untuk pindah scene / level
using UnityEngine.SceneManagement; 

public class TutorFSM : MonoBehaviour
{
    public enum State { Idle, Explain, WaitAnswer, Feedback }
    [Header("FSM Settings")]
    public State currentState;

    [Header("Player Settings")]
    public Transform player;
    public float range = 5f; 

    // ================= UI SETTINGS =================
    [Header("UI Components (TMP)")]
    public GameObject panel; 
    public TextMeshProUGUI questionText; 
    public TMP_InputField answerInput;  
    public TextMeshProUGUI feedbackText; 
    
    // TAMBAHAN: UI Text untuk skor dan jumlah soal
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI questionCountText; 

    // ================= AUDIO SETTINGS =================
    [Header("Audio Settings")]
    public AudioSource audioSource;   
    public AudioClip correctSound;    
    public AudioClip wrongSound;  
    public AudioClip bgm;    

    // ================= GAMEPLAY SETTINGS (BARU) =================
    [Header("Gameplay Variables")]
    private int score = 0; 
    private int totalAnswered = 0; 
    public int maxQuestion = 5; 

    // ================= DATA SOAL =================
    [System.Serializable]
    public class QuestionData
    {
        public string question;
        public string answer;
    }

    [Header("Question Bank")]
    public QuestionData[] questions; 
    private int currentIndex;        
    private int lastIndex = -1;      

    void Start()
    {
        currentState = State.Idle;
        
        if (panel != null) panel.SetActive(false); 

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // Set skor awal jadi 0 saat mulai
        if (scoreText != null) scoreText.text = "Score: 0";

        // <--- TAMBAHAN UNTUK PLAY BGM --->
        if (audioSource != null && bgm != null)
        {
            audioSource.clip = bgm;
            audioSource.loop = true; // Biar musiknya ngulang terus (looping)
            audioSource.Play();
        }
    }

    void Update()
    {
        // Perbaikan dari chat sebelumnya: Tutup panel otomatis saat menjauh
        if (currentState == State.Idle && IsNear())
        {
            ChangeState(State.Explain);
        }
        else if (currentState != State.Idle && !IsNear())
        {
            StopAllCoroutines(); 
            if (panel != null) panel.SetActive(false);
            currentState = State.Idle;
        }
    }

    void ChangeState(State newState)
    {
        currentState = newState;
        switch (newState)
        {
            case State.Explain:
                StartCoroutine(EnterExplain());
                break;
            case State.WaitAnswer:
                break;
            case State.Feedback:
                StartCoroutine(BackToIdle());
                break;
        }
    }

    IEnumerator EnterExplain()
    {
        if (panel != null) panel.SetActive(true);
        yield return null; 

        currentIndex = GetRandomQuestionIndex();
        
        if (questionText != null && questions.Length > 0)
        {
            questionText.text = questions[currentIndex].question;
        }

        // TAMBAHAN: Update UI teks urutan soal
        if (questionCountText != null)
        {
            questionCountText.text = "Question: " + (totalAnswered + 1) + "/" + maxQuestion;
        }

        if (answerInput != null) answerInput.text = "";
        if (feedbackText != null) feedbackText.text = "";

        ChangeState(State.WaitAnswer);
    }

    public void SubmitAnswer()
    {
        if (currentState != State.WaitAnswer) return;
        if (answerInput == null || feedbackText == null) return;

        string answer = answerInput.text;
        
        // TAMBAHAN: Tambah hitungan total soal yang udah dijawab
        totalAnswered++; 

        if (answer == "")
        {
            feedbackText.text = "Isi jawaban dulu!";
            return;
        }

        if (answer == questions[currentIndex].answer)
        {
            feedbackText.text = "Benar!";
            
            // TAMBAHAN: Tambah skor dan update UI Text-nya
            score++; 
            if (scoreText != null) scoreText.text = "Score: " + score; 

            if (audioSource != null && correctSound != null) audioSource.PlayOneShot(correctSound);
        }
        else
        {
            feedbackText.text = "Salah!";
            if (audioSource != null && wrongSound != null) audioSource.PlayOneShot(wrongSound);
        }

        ChangeState(State.Feedback);
    }

    IEnumerator BackToIdle()
    {
        yield return new WaitForSeconds(2f); 
        if (panel != null) panel.SetActive(false);
        
        // TAMBAHAN: Cek apakah udah 5 soal atau belum
        if (totalAnswered >= maxQuestion)
        {
            CheckGameResult();
        }
        else
        {
            ChangeState(State.Idle);
        }
    }

    // TAMBAHAN: Fungsi Evaluasi Akhir
    void CheckGameResult()
    {
        if (score >= 4)
        {
            feedbackText.text = "Naik ke Level 2!";
            Debug.Log("Naik ke Level 2");
            SceneManager.LoadScene("P14_Level2"); 
        }
        else
        {
            feedbackText.text = "Game Over";
            Debug.Log("Game Over");
            
            // Reset game dari awal
            totalAnswered = 0;
            score = 0;
            if (scoreText != null) scoreText.text = "Score: 0";
            
            ChangeState(State.Idle);
        }
    }

    int GetRandomQuestionIndex()
    {
        if (questions.Length <= 1) return 0;
        int newIndex;
        do { newIndex = Random.Range(0, questions.Length); } while (newIndex == lastIndex); 
        lastIndex = newIndex;
        return newIndex;
    }

    bool IsNear()
    {
        if (player == null) return false; 
        return Vector2.Distance(transform.position, player.position) < range;
    }
}