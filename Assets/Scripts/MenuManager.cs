using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static bool isPaused = false;

    [Header("Paneles de UI")]
    public GameObject panelMenuInicio;
    public GameObject panelPausa;
    public GameObject panelHUDJuego;

    void Start()
    {
        // Al iniciar, mostrar menú de inicio y congelar el tiempo del juego
        panelMenuInicio.SetActive(true);
        panelPausa.SetActive(false);
        panelHUDJuego.SetActive(false);
        Time.timeScale = 0f; 
        isPaused = true;
    }

    void Update()
    {
        // Activar pausa con la tecla Escape si el menú de inicio está cerrado
        if (Input.GetKeyDown(KeyCode.Escape) && !panelMenuInicio.activeSelf)
        {
            if (isPaused) ReanudarJuego();
            else PausarJuego();
        }
    }

    public void IniciarPartida()
    {
        panelMenuInicio.SetActive(false);
        panelHUDJuego.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PausarJuego()
    {
        panelPausa.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ReanudarJuego()
    {
        panelPausa.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void ReiniciarEscena()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
} //