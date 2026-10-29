using UnityEngine;

public class ItemSalud : MonoBehaviour
{
    [Header("Configuración de Curación")]
    [SerializeField] private int cantidadCuracion = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que entra al Trigger tiene la etiqueta "Player"
        if (collision.CompareTag("player"))
        {
            PlayerController2D jugador = collision.GetComponent<PlayerController2D>();

            if (jugador != null)
            {
                // Solo se consume si el jugador está vivo y no tiene la vida al máximo
                if (!jugador.estaMuerto && jugador.VidasActuales < jugador.vidasMaximas)
                {
                    jugador.Curar(cantidadCuracion);
                    Destroy(gameObject); // Destruye el ítem al ser consumido
                }
            }
        }
    }
}