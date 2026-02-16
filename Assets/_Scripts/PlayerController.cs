using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float velocidadMovimiento = 8f;
    public float fuerzaSalto = 5f;

    [Header("Detecci�n de Suelo")]
    public Transform piesPosicion;      // Un punto invisible en los pies
    public float radioDeteccion = 0.3f; // Qu� tan grande es el detector
    public LayerMask queEsSuelo;        // Qu� capas cuentan como suelo

    private Rigidbody2D rb;
    private float inputHorizontal;
    private bool estaEnSuelo;

    void Start()
    {
        // Obtenemos el componente de f�sicas autom�ticamente
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. INPUT (Detectar teclas)
        inputHorizontal = Input.GetAxisRaw("Horizontal"); // A/D o Flechas (-1 a 1)

        // 2. SALTAR (Solo si estamos en el suelo y pulsamos Espacio)
        if (Input.GetButtonDown("Jump") && estaEnSuelo)
        {
            Saltar();
        }
    }

    void FixedUpdate()
    {
        // 3. MOVIMIENTO (F�sicas)
        // Mantenemos la velocidad vertical (ca�da) y cambiamos la horizontal
        rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, rb.linearVelocity.y);

        // 4. DETECTAR SI TOCAMOS SUELO
        estaEnSuelo = Physics2D.OverlapCircle(piesPosicion.position, radioDeteccion, queEsSuelo);
    }

    void Saltar()
    {
        // Reseteamos la velocidad vertical para que el salto sea siempre igual
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        // Aplicamos fuerza hacia arriba
        rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
    }

    // Esto dibuja un circulo rojo en el editor para ver el detector de suelo
    private void OnDrawGizmos()
    {
        if (piesPosicion != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(piesPosicion.position, radioDeteccion);
        }
    }
}