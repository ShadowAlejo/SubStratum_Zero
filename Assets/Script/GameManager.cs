using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Referencias del Jugador")]
    [Tooltip("Arrastra el GameObject del Jugador aquí")]
    [SerializeField] private PlayerController2D jugador;
    
    [Header("Paneles de Interfaz")]
    [Tooltip("Arrastra el Game_Over_Panel aquí")]
    [SerializeField] private GameObject gameOverPanel;

    [Tooltip("Arrastra el Nivel_Completo_Panel aquí")]
    [SerializeField] private GameObject nivelCompletoPanel;

    private bool juegoTerminado = false;

    void Start()
    {
        // Aseguramos que ambos paneles estén ocultos al iniciar el nivel
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (nivelCompletoPanel != null) nivelCompletoPanel.SetActive(false);
    }

    void Update()
    {
        // Si el juego ya terminó (ganamos o perdimos), no hacemos más comprobaciones
        if (juegoTerminado) return;

        // Condición de Derrota: El jugador muere
        if (jugador != null && jugador.estaMuerto)
        {
            ActivarGameOver();
            return; 
        }

        // Condición de Victoria: Revisar estado de los enemigos
        ComprobarVictoria();
    }

    private void ComprobarVictoria()
    {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemigos.Length == 0)
        {
            ActivarNivelCompleto();
            return;
        }

        foreach (GameObject enemigo in enemigos)
        {
            EnemyControllerBase scriptEnemigo = enemigo.GetComponent<EnemyControllerBase>();

            // Objeto con tag "Enemy" pero sin script reconocido, o enemigo vivo: no hay victoria aún
            if (scriptEnemigo == null || !scriptEnemigo.estaMuerto)
            {
                return;
            }
        }

        ActivarNivelCompleto();
    }

    private void ActivarGameOver()
    {
        juegoTerminado = true;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    private void ActivarNivelCompleto()
    {
        juegoTerminado = true;
        if (nivelCompletoPanel != null) nivelCompletoPanel.SetActive(true);
    }

    // Función que usarán tanto el botón "Reintentar" como "Jugar_De_Nuevo"
    public void ReintentarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Función para los botones "Salir"
    public void SalirAlMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}