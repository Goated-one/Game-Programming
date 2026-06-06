using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PintuKeluar : MonoBehaviour
{
    public enum TipePintu { PindahScene, TamatGame }
    
    [Header("Pengaturan Pintu")]
    [Tooltip("Pilih fungsi pintu ini di Inspector")]
    public TipePintu fungsiPintu;

    [Header("Khusus Fungsi: Pindah Scene")]
    public string namaSceneTujuan = "Level2";
    public Animator transisiAnim; // Tarik Panel hitam CanvasTransisi lu ke sini
    public float durasiTunggu = 1f;

    [Header("Khusus Fungsi: Tamat Game")]
    public GameObject panelTamatUI; // Tarik UI Panel Tamat lu ke sini

    private bool sudahTersentuh = false;

    void Start()
    {
        // Pastikan panel tamat ngumpet pas game baru mulai
        if (panelTamatUI != null) panelTamatUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !sudahTersentuh)
        {
            sudahTersentuh = true;

            // Cek fungsi pintu ini lewat sistem dropdown yang lu pilih
            if (fungsiPintu == TipePintu.PindahScene)
            {
                StartCoroutine(ProsesPindahScene());
            }
            else if (fungsiPintu == TipePintu.TamatGame)
            {
                MunculinLayarTamat();
            }
        }
    }

    IEnumerator ProsesPindahScene()
    {
        // Panggil layar hitam buat fade out (mirip kayak MainMenuManager)
        if (transisiAnim != null) transisiAnim.SetTrigger("MulaiHitam");

        yield return new WaitForSeconds(durasiTunggu);

        Time.timeScale = 1f;
        SceneManager.LoadScene(namaSceneTujuan);
    }

    void MunculinLayarTamat()
    {
        if (panelTamatUI != null) panelTamatUI.SetActive(true);
        Time.timeScale = 0f; // Pause game biar THE ONLY LUKE berhenti gerak
    }

    // Fungsi ini bisa lu pasang di tombol "Kembali ke Main Menu" yang ada di layar tamat
    public void BalikKeMainMenu()
    {
        Time.timeScale = 1f; // Wajib balikin waktu normal sebelum pindah
        SceneManager.LoadScene("MainMenu"); // Pastikan nama scene Main Menu lu bener
    }
}