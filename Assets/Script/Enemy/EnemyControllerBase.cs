// ====================== EnemyControllerBase.cs ======================
using UnityEngine;
using System.Collections;

public abstract class EnemyControllerBase : MonoBehaviour
{
    [Header("Sistema de Vida")]
    [SerializeField] private int vidasMaximas = 3;
    private int vidasActuales;
    public bool estaMuerto { get; protected set; }

    [Header("Configuración Base")]
    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;
    public int damageAmount = 1;
    [SerializeField] protected bool miraDerechaPorDefecto = false;

    [Header("Combate")]
    [SerializeField] protected float tiempoAnimacionAtaque = 0.5f;
    [SerializeField] protected float tiempoAnimacionDanio = 0.4f;

    protected Rigidbody2D rb;
    protected Animator anim;
    protected float horizontalDirection;
    protected bool estaAtacando;
    protected bool recibiendoDanio;
    protected PlayerController2D scriptJugador;

    protected virtual void Start()
    {
        InicializarComponentes();
        vidasActuales = vidasMaximas;

        if (player != null)
        {
            scriptJugador = player.GetComponent<PlayerController2D>();
        }
    }

    protected virtual void Update()
    {
        if (estaMuerto) return;

        if (EstaBloqueado())
        {
            DetenerMovimiento();
            return;
        }

        ActualizarPersecucionJugador();
        anim.SetFloat("speedX", horizontalDirection);
    }

    protected virtual void FixedUpdate()
    {
        if (estaMuerto) return;
        AplicarMovimiento();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (estaMuerto) return;
        GestionarColisionConJugador(collision);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (estaMuerto) return;
        GestionarImpactoProyectil(other);
    }

    // ---------- Inicialización ----------

    protected virtual void InicializarComponentes()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // ---------- Estado / bloqueo ----------

    protected bool EstaBloqueado()
    {
        bool jugadorMuerto = scriptJugador != null && scriptJugador.estaMuerto;
        return recibiendoDanio || estaAtacando || jugadorMuerto || estaMuerto;
    }

    // ---------- Métodos específicos de cada tipo de enemigo ----------

    protected abstract void DetenerMovimiento();
    protected abstract void ActualizarPersecucionJugador();
    protected abstract void AplicarMovimiento();

    protected void OrientarSprite()
    {
        Vector3 currentScale = transform.localScale;
        if (horizontalDirection != 0)
        {
            float signo = miraDerechaPorDefecto ? horizontalDirection : -horizontalDirection;
            currentScale.x = Mathf.Abs(currentScale.x) * signo;
            transform.localScale = currentScale;
        }
    }

    // ---------- Colisiones y Daño ----------

    protected void GestionarColisionConJugador(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerController2D playerController = collision.gameObject.GetComponent<PlayerController2D>();

        if (playerController != null && !estaAtacando && !recibiendoDanio && !playerController.estaMuerto)
        {
            playerController.RecibirDanio(transform.position, damageAmount); // <-- corregido
            StartCoroutine(RutinaAtaque());
        }
    }

    protected void GestionarImpactoProyectil(Collider2D other)
    {
        if (other.GetComponent<bullet>() != null)
        {
            if (!recibiendoDanio && !estaMuerto)
            {
                vidasActuales--;

                if (vidasActuales <= 0)
                {
                    Morir();
                }
                else
                {
                    StartCoroutine(RutinaDanio());
                }
            }

            Destroy(other.gameObject);
        }
    }

    // La muerte común se resuelve aquí; cada subclase agrega su comportamiento físico propio.
    protected virtual void Morir()
    {
        estaMuerto = true;
        rb.velocity = Vector2.zero;

        anim.SetFloat("speedX", 0f);
        anim.SetBool("recbiendoDanio", false);
        anim.SetBool("enemyAtack", false);

        gameObject.tag = "Untagged";

        anim.SetBool("muerto", true);
        anim.Play("die");

        Destroy(gameObject, 5f);
    }

    // ---------- Corrutinas ----------

    protected IEnumerator RutinaAtaque()
    {
        estaAtacando = true;
        anim.SetBool("enemyAtack", true);
        yield return new WaitForSeconds(tiempoAnimacionAtaque);
        anim.SetBool("enemyAtack", false);
        estaAtacando = false;
    }

    protected IEnumerator RutinaDanio()
    {
        recibiendoDanio = true;
        anim.SetBool("recbiendoDanio", true);

        yield return new WaitForSeconds(tiempoAnimacionDanio);

        anim.SetBool("recbiendoDanio", false);
        recibiendoDanio = false;
    }
}