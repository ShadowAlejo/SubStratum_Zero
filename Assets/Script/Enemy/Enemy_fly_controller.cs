// ====================== FlyingEnemy_Controller.cs ======================
using UnityEngine;

public class FlyingEnemy_Controller : EnemyControllerBase
{
    private Collider2D col;
    private Vector2 direccionMovimiento;

    protected override void InicializarComponentes()
    {
        base.InicializarComponentes();
        col = GetComponent<Collider2D>();

        // Vuelo normal: gravedad en 0
        rb.gravityScale = 0f;
    }

    protected override void DetenerMovimiento()
    {
        horizontalDirection = 0f;
        direccionMovimiento = Vector2.zero;
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
            DetenerMovimiento();
        }
    }

    private void CalcularDireccionHaciaJugador()
    {
        direccionMovimiento = (player.position - transform.position).normalized;

        if (direccionMovimiento.x > 0.1f)
            horizontalDirection = 1f;
        else if (direccionMovimiento.x < -0.1f)
            horizontalDirection = -1f;
        else
            horizontalDirection = 0f;
    }

    protected override void AplicarMovimiento()
    {
        if (!EstaBloqueado())
        {
            rb.velocity = direccionMovimiento * speed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    protected override void Morir()
    {
        base.Morir();

        // Reactivar la gravedad para que caiga y congelar el movimiento lateral
        rb.gravityScale = 2f;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        // Ignorar colisión física con el Player (pero sigue chocando con el suelo)
        DesactivarColisionConJugador();
    }

    private void DesactivarColisionConJugador()
    {
        if (col == null) return;

        Collider2D playerCol = player != null ? player.GetComponent<Collider2D>() : null;

        if (playerCol != null)
        {
            Physics2D.IgnoreCollision(col, playerCol, true);
        }
    }
}