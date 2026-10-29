using UnityEngine;

public class atack_player : MonoBehaviour
{
    [Header("Ataque Frontal")]
    [SerializeField] private Transform controlador_disparo;
    [SerializeField] private GameObject bullet;

    [Header("Ataque Hacia Arriba")]
    [SerializeField] private Transform controlador_up_disparo;
    [SerializeField] private GameObject up_bullet;

    private PlayerController2D playerController;

    private void Awake()
    {
        // Obtenemos la referencia al controlador principal para saber si estamos en el suelo
        playerController = GetComponent<PlayerController2D>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            // Solo dispara hacia arriba si presiona arriba Y el jugador ESTÁ en el suelo
            if (Input.GetAxisRaw("Vertical") > 0 && playerController.EnSuelo)
            {
                DispararArriba();
            }
            else
            {
                Disparar();
            }
        }
    }

    private void Disparar()
    {
        if (controlador_disparo != null && bullet != null)
        {
            Instantiate(bullet, controlador_disparo.position, controlador_disparo.rotation);
        }
    }

    private void DispararArriba()
    {
        if (controlador_up_disparo != null && up_bullet != null)
        {
            Instantiate(up_bullet, controlador_up_disparo.position, controlador_up_disparo.rotation);
        }
    }
}