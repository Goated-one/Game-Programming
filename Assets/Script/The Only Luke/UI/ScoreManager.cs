using UnityEngine;
using TMPro; 

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("Pengaturan Skor")]
    public int skorSaatIni = 0;
    public TextMeshProUGUI teksSkorUI; 

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateTeksSkor();
    }

    public void TambahSkor(int jumlah)
    {
        skorSaatIni += jumlah;
        UpdateTeksSkor();
    }

    void UpdateTeksSkor()
    {
        if (teksSkorUI != null)
        {
            teksSkorUI.text = "Skor: " + skorSaatIni;
        }
    }
}