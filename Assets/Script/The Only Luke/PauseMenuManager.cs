using UnityEngine;
using UnityEngine.SceneManagement; 

public class PauseMenuManager : MonoBehaviour
{
    public static bool GameIsPaused = false; 
    public GameObject pauseMenuUI; 
    
    // --- TAMBAHAN BARU: Slot buat masukin Panel Journal ---
    public GameObject journalUI; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); 
        
        // --- TAMBAHAN BARU: Pastiin Jurnal juga ikut ketutup pas di-resume ---
        if (journalUI != null) journalUI.SetActive(false); 
        
        Time.timeScale = 1f; 
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true); 
        Time.timeScale = 0f; 
        GameIsPaused = true;
    }

    // --- FUNGSI INI UDAH DI-UPDATE ---
    public void BukaJournal()
{
    // Debug: Cek apakah script beneran nemu objeknya
    if (journalUI != null)
    {
        Debug.Log("Jurnal ketemu! Lagi nyoba aktifin: " + journalUI.name);
        pauseMenuUI.SetActive(false); 
        journalUI.SetActive(true);    
    }
    else
    {
        Debug.LogError("WADUH! Variable journalUI di PauseMenuManager masih KOSONG (null). Tolong cek Inspector GameManager lagi!");
    }
}

    // --- INI FUNGSI BARU BUAT TOMBOL KEMBALI ---
    public void TutupJournal()
    {
        journalUI.SetActive(false);   // Sembunyiin layar jurnal
        pauseMenuUI.SetActive(true);  // Balikin tombol menu pause
    }

    public void KeMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); 
    }
}