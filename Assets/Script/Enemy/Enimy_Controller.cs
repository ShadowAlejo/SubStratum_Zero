// ====================== Enimy_Controller.cs ======================
using UnityEngine;

public class Enimy_Controller : EnemyControllerBase
{
    protected override void DetenerMovimiento()
    {
        horizontalDirection = 0f;
        anim.SetFloat("speedX", 0f);
    }

    protected override void ActualizarPersecucionJugador()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            CalcularDireccionHaciaJugador();
            OrientarSprite();
        }
        else
        {
            horizontalDirection = 0f;
        }
    }

    private void CalcularDireccionHaciaJugador()
    {
        float distanceX = player.position.x - transform.position.x;

        if (distanceX > 0.1f)
            horizontalDirection = 1f;
        else if (distanceX < -0.1f)
            horizontalDirection = -1f;
        else
            horizontalDirection = 0f;
    }

    protected override void AplicarMovimiento()
    {
        if (!EstaBloqueado())
        {
            rb.velocity = new Vector2(horizontalDirection * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    protected override void Morir()
    {
        base.Morir();

        // Desactiva el colisionador para que el jugador no sufra daño al tocar el cadáver
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Evita que el enemigo caiga al vacío tras perder su colisionador
        rb.isKinematic = true;
    }
}