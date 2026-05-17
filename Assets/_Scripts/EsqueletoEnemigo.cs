using UnityEngine;

public class EsqueletoEnemigo : MonoBehaviour
{
    [Header("Velocidades")]
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4f;

    [Header("Detección (Bastón de ciego)")]
    public Transform controladorSuelo;
    public float distanciaRayoSuelo = 1.0f; 
    public float distanciaMuro = 0.3f;      
    public LayerMask queEsSuelo;

    [Header("Combate")]
    public float rangoVision = 6f;
    public float rangoAtaque = 1.2f;
    public float cantidadDano = 20f;
    public float tiempoEntreAtaques = 1.5f;
    public float duracionAnimacionAtaque = 0.5f;

    private Transform jugador;
    private Rigidbody2D rb;
    private Animator anim;
    private bool mirandoDerecha = true;
    private float contadorAtaque;
    private bool estaAtacando = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        GameObject goJugador = GameObject.FindGameObjectWithTag("Player");
        if (goJugador != null) jugador = goJugador.transform;
    }

    void Update()
    {
        if (jugador == null || estaAtacando) return;

        contadorAtaque -= Time.deltaTime;

       
        bool haySuelo = Physics2D.Raycast(controladorSuelo.position, Vector2.down, distanciaRayoSuelo, queEsSuelo);

        Vector2 direccionMiro = mirandoDerecha ? Vector2.right : Vector2.left;

       
        bool chocaPared = Physics2D.Raycast(controladorSuelo.position + Vector3.up * 0.6f, direccionMiro, distanciaMuro, queEsSuelo);

        float distanciaJugador = Vector2.Distance(transform.position, jugador.position);

        if (distanciaJugador <= rangoAtaque)
        {
            
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (contadorAtaque <= 0)
            {
                Atacar();
            }
        }
        else if (distanciaJugador <= rangoVision)
        {
            
            if (jugador.position.x > transform.position.x && !mirandoDerecha) Voltear();
            else if (jugador.position.x < transform.position.x && mirandoDerecha) Voltear();

            direccionMiro = mirandoDerecha ? Vector2.right : Vector2.left;

           
            if (haySuelo && !chocaPared)
            {
                rb.linearVelocity = new Vector2(direccionMiro.x * velocidadPersecucion, rb.linearVelocity.y);
            }
            else
            {
               
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
        else
        {
           
            if (!haySuelo || chocaPared)
            {
                Voltear();
                direccionMiro = mirandoDerecha ? Vector2.right : Vector2.left;
            }

            rb.linearVelocity = new Vector2(direccionMiro.x * velocidadPatrulla, rb.linearVelocity.y);
        }

        
        if (anim != null && velocidadPatrulla > 0)
        {
            float velocidadActual = Mathf.Abs(rb.linearVelocity.x);
            float multi = velocidadActual / velocidadPatrulla;
            anim.SetFloat("Multiplicador", multi);
        }
    }

    void Atacar()
    {
        estaAtacando = true;
        contadorAtaque = tiempoEntreAtaques;

       
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        
        if (anim != null) anim.SetFloat("Multiplicador", 0f);
        if (anim != null) anim.SetTrigger("Atacar");

        PlayerController scriptJugador = jugador.GetComponent<PlayerController>();
        if (scriptJugador != null && !scriptJugador.esInvulnerable)
        {
            scriptJugador.RecibirDano(cantidadDano);
        }

        Invoke(nameof(FinAtaque), duracionAnimacionAtaque);
    }

    void FinAtaque()
    {
        estaAtacando = false;
    }

    void Voltear()
    {
        mirandoDerecha = !mirandoDerecha;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoVision);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);

        if (controladorSuelo != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(controladorSuelo.position, controladorSuelo.position + Vector3.down * distanciaRayoSuelo);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(controladorSuelo.position + Vector3.up * 0.6f, controladorSuelo.position + Vector3.up * 0.6f + (mirandoDerecha ? Vector3.right : Vector3.left) * distanciaMuro);
        }
    }
}