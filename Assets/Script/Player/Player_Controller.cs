using UnityEngine;
using System.Collections;

public class PlayerController2D : MonoBehaviour
{
    [Header("Sistema de Vida")]
    [SerializeField] public int vidasMaximas = 5;
    private int vidasActuales;
    public bool estaMuerto { get; private set; }
    public int VidasActuales => vidasActuales;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Detección de Suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Daño e Invulnerabilidad")]
    [SerializeField] private float tiempoInvulnerabilidad = 1f;
    [SerializeField] private float fuerzaReboteX = 5f;
    [SerializeField] private float fuerzaReboteY = 5f;

    [Header("Combate")]
    [SerializeField] private float tiempoAnimacionAtaque = 0.3f;

    private Rigidbody2D rb;
    private Animator anim;
    private atack_player scriptDisparo;
    private float horizontalInput;
    private float verticalInput;
    private bool isGrounded;
    private bool recibiendoDanio;
    private bool estaAtacando;

    public bool EnSuelo => isGrounded;

    // ===================== CICLO DE VIDA UNITY =====================

    private void Awake()
    {
        ObtenerComponentes();
        RestaurarVidaCompleta();
    }

    private void Update()
    {
        if (estaMuerto) return;

        ActualizarDeteccionSuelo();
        ActualizarEstadoScriptDisparo();

        if (recibiendoDanio) return;

        

        LeerInputMovimiento();
        ActualizarAnimacionVelocidadHorizontal();
        OrientarPersonajeSegunMovimiento();
        ProcesarEntradaAtaque();
        ProcesarEntradaSalto();
    }

    private void FixedUpdate()
    {
        if (estaMuerto) return;
        MoverHorizontalmente();
    }

    private void OnDrawGizmosSelected()
    {
        DibujarGizmoDeteccionSuelo();
    }

    // ===================== INICIALIZACIÓN =====================

    private void ObtenerComponentes()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        scriptDisparo = GetComponent<atack_player>();
    }

    private void RestaurarVidaCompleta()
    {
        vidasActuales = vidasMaximas;
    }

    // ===================== DETECCIÓN DE SUELO =====================

    private void ActualizarDeteccionSuelo()
    {
        isGrounded = DetectarContactoConSuelo();
        ActualizarAnimacionSuelo();
    }

    private bool DetectarContactoConSuelo()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void ActualizarAnimacionSuelo()
    {
        anim.SetBool("enSuelo", isGrounded);
    }

    private void ActualizarEstadoScriptDisparo()
    {
        if (scriptDisparo != null)
        {
            scriptDisparo.enabled = !estaMuerto;
        }
    }

    private void DibujarGizmoDeteccionSuelo()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    // ===================== INPUT DE MOVIMIENTO =====================

    private void LeerInputMovimiento()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void ActualizarAnimacionVelocidadHorizontal()
    {
        anim.SetFloat("velocidadX", horizontalInput);
    }

    // ===================== ROTACIÓN DEL PERSONAJE =====================

    private void OrientarPersonajeSegunMovimiento()
    {
        if (horizontalInput > 0)
        {
            MirarHaciaDerecha();
        }
        else if (horizontalInput < 0)
        {
            MirarHaciaIzquierda();
        }
    }

    private void MirarHaciaDerecha()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    private void MirarHaciaIzquierda()
    {
        transform.eulerAngles = new Vector3(0, 180, 0);
    }

    // ===================== SALTO =====================

    private void ProcesarEntradaSalto()
    {
        if (SeSolicitaSalto())
        {
            AplicarImpulsoDeSalto();
        }
    }

    private bool SeSolicitaSalto()
    {
        return Input.GetKeyDown(KeyCode.Space) && isGrounded;
    }

    private void AplicarImpulsoDeSalto()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    // ===================== MOVIMIENTO HORIZONTAL =====================

    private void MoverHorizontalmente()
    {
        if (recibiendoDanio) return;
        AplicarVelocidadHorizontal();
    }

    private void AplicarVelocidadHorizontal()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }

    // ===================== ATAQUE =====================

    private void ProcesarEntradaAtaque()
    {
        if (!SeSolicitaAtaque()) return;

        if (EsAtaqueSuperior())
        {
            StartCoroutine(EjecutarAtaqueSuperior());
        }
        else
        {
            StartCoroutine(EjecutarAtaqueNormal());
        }
    }

    private bool SeSolicitaAtaque()
    {
        return Input.GetButtonDown("Fire1") && !estaAtacando;
    }

    private bool EsAtaqueSuperior()
    {
        return verticalInput > 0 && isGrounded;
    }

    private IEnumerator EjecutarAtaqueNormal()
    {
        IniciarEstadoAtaque("atacando");
        yield return new WaitForSeconds(tiempoAnimacionAtaque);
        FinalizarEstadoAtaque("atacando");
    }

    private IEnumerator EjecutarAtaqueSuperior()
    {
        IniciarEstadoAtaque("up_atack");
        yield return new WaitForSeconds(tiempoAnimacionAtaque);
        FinalizarEstadoAtaque("up_atack");
    }

    private void IniciarEstadoAtaque(string parametroAnimacion)
    {
        estaAtacando = true;
        anim.SetBool(parametroAnimacion, true);
    }

    private void FinalizarEstadoAtaque(string parametroAnimacion)
    {
        anim.SetBool(parametroAnimacion, false);
        estaAtacando = false;
    }

    // ===================== RECIBIR DAÑO =====================

    public void RecibirDanio(Vector2 posicionEnemigo, int cantDanio)
    {
        if (recibiendoDanio || estaMuerto) return;

        DescontarVida(cantDanio);

        if (VidaAgotada())
        {
            Morir();
        }
        else
        {
            IniciarSecuenciaDeDanio(posicionEnemigo);
        }
    }

    // ===================== CURACIÓN =====================

    public void Curar(int cantidad)
    {
        if (estaMuerto) return;

        vidasActuales += cantidad;

        // Evita que la vida supere el máximo permitido
        if (vidasActuales > vidasMaximas)
        {
            vidasActuales = vidasMaximas;
        }
    }

    private void DescontarVida(int cantDanio)
    {
        vidasActuales -= cantDanio;
    }

    private bool VidaAgotada()
    {
        return vidasActuales <= 0;
    }

    private void IniciarSecuenciaDeDanio(Vector2 posicionEnemigo)
    {
        ActivarEstadoDanio();
        DetenerMovimientoFisico();
        AplicarReboteDanio(posicionEnemigo);
        StartCoroutine(TemporizarFinDanio());
    }

    private void ActivarEstadoDanio()
    {
        recibiendoDanio = true;
        anim.SetBool("recibeDanio", true);
    }

    private void DetenerMovimientoFisico()
    {
        rb.velocity = Vector2.zero;
    }

    private void AplicarReboteDanio(Vector2 posicionEnemigo)
    {
        float direccionX = CalcularDireccionRebote(posicionEnemigo);
        Vector2 rebote = new Vector2(direccionX * fuerzaReboteX, fuerzaReboteY);
        rb.AddForce(rebote, ForceMode2D.Impulse);
    }

    private float CalcularDireccionRebote(Vector2 posicionEnemigo)
    {
        return transform.position.x < posicionEnemigo.x ? -1f : 1f;
    }

    private IEnumerator TemporizarFinDanio()
    {
        yield return new WaitForSeconds(tiempoInvulnerabilidad);
        DesactivaDanio();
    }

    public void DesactivaDanio()
    {
        recibiendoDanio = false;
        anim.SetBool("recibeDanio", false);
    }

    // ===================== MUERTE =====================

    private void Morir()
    {
        estaMuerto = true;
        DetenerMovimientoFisico();
        ReiniciarAnimacionesDeMovimiento();
        LimpiarAnimacionesDeAccion();
        ActivarAnimacionDeMuerte();
        DesactivarScriptDisparo();
    }

    private void ReiniciarAnimacionesDeMovimiento()
    {
        anim.SetFloat("velocidadX", 0f);
        anim.SetBool("enSuelo", true);
    }

    private void LimpiarAnimacionesDeAccion()
    {
        anim.SetBool("recibeDanio", false);
        anim.SetBool("atacando", false);
        anim.SetBool("up_atack", false);
    }

    private void ActivarAnimacionDeMuerte()
    {
        anim.SetBool("muerto", true);
        anim.Play("die");
    }

    private void DesactivarScriptDisparo()
    {
        if (scriptDisparo != null)
        {
            scriptDisparo.enabled = false;
        }
    }
}