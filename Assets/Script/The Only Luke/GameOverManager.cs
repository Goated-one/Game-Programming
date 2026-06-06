using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Wajib dipanggil buat pakai Coroutine (jeda waktu)

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    [Header("Pengaturan UI")]
    public GameObject panelGameOver;
    public CanvasGroup panelCanvasGroup; // Tambahan buat ngatur transparansi (Fade)
    public float durasiFade = 1.5f;      // Berapa detik efek fadenya berlangsung?

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (panelGameOver != null) panelGameOver.SetActive(false);
    }

    public void MunculkanGameOver()
    {
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
            
            // Hentikan waktu game biar musuh ikut diam
            Time.timeScale = 0f; 

            // Mulai jalankan animasi fadenya
            if (panelCanvasGroup != null)
            {
                StartCoroutine(ProsesFadeIn());
            }
        }
    }

    // --- FUNGSI ANIMASI FADE IN ---
    IEnumerator ProsesFadeIn()
    {
        float waktuBerjalan = 0f;

        // Set Alpha jadi 0 (100% transparan / hilang) di detik pertama
        panelCanvasGroup.alpha = 0f;

        while (waktuBerjalan < durasiFade)
        {
            // PENTING: Pakai unscaledDeltaTime karena waktu game lagi berhenti (0)
            waktuBerjalan += Time.unscaledDeltaTime;
            
            // Rumus nyari nilai transisi yang mulus dari 0 ke 1
            panelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, waktuBerjalan / durasiFade);
            
            yield return null; // Tunggu ke frame layar selanjutnya
        }

        // Pastikan pas selesai, Alpha-nya mentok di 1 (Solid)
        panelCanvasGroup.alpha = 1f;
    }

    public void UlangiLevel()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void KembaliKeMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 
    }
}