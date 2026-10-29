using UnityEngine;
using UnityEngine.UI;

public class Barra_Vida : MonoBehaviour
{
    [SerializeField] private Image rellenoBarraVida;
    private PlayerController2D playerController;
    private float vidaMaxima;

    void Start()
    {
        // Encuentra al PlayerController2D independientemente del nombre del GameObject
        playerController = GameObject.Find("player").GetComponent<PlayerController2D>();

        if (playerController != null)
        {
            vidaMaxima = playerController.vidasMaximas;
        }
        else
        {
            Debug.LogError("Barra_Vida: No se encontró ningún PlayerController2D en la escena.");
        }
    }

    void Update()
    {
        if (playerController == null || vidaMaxima <= 0) return;

        // Actualiza el relleno de la barra de vida (funciona para daño y curación)
        rellenoBarraVida.fillAmount = (float)playerController.VidasActuales / vidaMaxima;
    }
}