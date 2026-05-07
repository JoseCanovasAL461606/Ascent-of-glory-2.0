using UnityEngine;
using System.Collections;

public class ArqueroEnemigo : MonoBehaviour
{
    [Header("Configuración del Enemigo")]
    public float rangoDeteccion = 8f;
    public float tiempoEntreDisparos = 2.5f;
    public float tiempoTensarArco = 0.6f; // Ajusta esto para que la flecha salga en el fotograma correcto

    [Header("Referencias")]
    public GameObject prefabFlecha;
    public Transform puntoDeDisparo;

    private Transform jugador;
    private Animator anim;
    private float contadorDisparo;
    private bool mirandoDerecha = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        // Buscamos automáticamente al objeto que tenga la etiqueta "Player"
        GameObject goJugador = GameObject.FindGameObjectWithTag("Player");
        if (goJugador != null) jugador = goJugador.transform;

        contadorDisparo = tiempoEntreDisparos;
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Si el jugador está dentro del rango
        if (distancia <= rangoDeteccion)
        {
            // Girar hacia el jugador
            if (jugador.position.x > transform.position.x && !mirandoDerecha) Voltear();
            else if (jugador.position.x < transform.position.x && mirandoDerecha) Voltear();

            contadorDisparo -= Time.deltaTime;

            if (contadorDisparo <= 0f)
            {
                StartCoroutine(RutinaDisparo());
                contadorDisparo = tiempoEntreDisparos;
            }
        }
    }

    IEnumerator RutinaDisparo()
    {
        // 1. Iniciamos la animación de disparar
        if (anim != null) anim.SetTrigger("Atacar");

        // 2. Esperamos a que el dibujo termine de tensar la cuerda
        yield return new WaitForSeconds(tiempoTensarArco);

        // 3. ¡Disparamos! (Comprobamos si el jugador sigue vivo por si acaso)
        if (jugador != null && prefabFlecha != null && puntoDeDisparo != null)
        {
            // Calculamos el ángulo exacto hacia el jugador
            Vector2 direccion = (jugador.position - puntoDeDisparo.position).normalized;
            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

            // Inclinamos la flecha en esa dirección
            Quaternion rotacionFlecha = Quaternion.Euler(0, 0, angulo);

            Instantiate(prefabFlecha, puntoDeDisparo.position, rotacionFlecha);
        }
    }

    void Voltear()
    {
        mirandoDerecha = !mirandoDerecha;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // Para ver el rango dibujado en la escena de Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}
