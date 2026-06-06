using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Wajib ada buat fitur tunggu (Coroutine)

public class MainMenuManager : MonoBehaviour
{
    [Header("Pengaturan Scene")]
    public string namaSceneLevel = "Level1"; 

    [Header("Pengaturan Transisi")]
    public Animator transisiAnim; // Tarik objek Panel hitam ke sini
    public float durasiTunggu = 1f; // Sesuaikan sama durasi animasi FadeOut lu

    public void MainkanGame()
    {
        // Jalankan sistem pindah scene yang pakai delay
        StartCoroutine(ProsesPindahScene());
    }

    IEnumerator ProsesPindahScene()
    {
        // 1. Trigger animasi biar layar jadi hitam
        if (transisiAnim != null) transisiAnim.SetTrigger("MulaiHitam");

        // 2. Tunggu sampai animasinya beres nutup layar
        yield return new WaitForSeconds(durasiTunggu);

        // 3. Pindah Scene!
        Time.timeScale = 1f; 
        SceneManager.LoadScene(namaSceneLevel);
    }

    public void KeluarGame()
    {
        Debug.Log("Game ditutup!");
        Application.Quit();
    }
}