using UnityEngine;
using System.Collections;

public class ArqueroEnemigo : MonoBehaviour
{
    [Header("Configuración del Enemigo")]
    public float rangoDeteccion = 8f;
    public float tiempoEntreDisparos = 2.5f;
    public float tiempoTensarArco = 0.6f; 

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
        
        GameObject goJugador = GameObject.FindGameObjectWithTag("Player");
        if (goJugador != null) jugador = goJugador.transform;

        contadorDisparo = tiempoEntreDisparos;
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

       
        if (distancia <= rangoDeteccion)
        {
           
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
       
        if (anim != null) anim.SetTrigger("Atacar");

       
        yield return new WaitForSeconds(tiempoTensarArco);

       
        if (jugador != null && prefabFlecha != null && puntoDeDisparo != null)
        {
           
            Vector2 direccion = (jugador.position - puntoDeDisparo.position).normalized;
            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

            
            Quaternion rotacionFlecha = Quaternion.Euler(0, 0, angulo);

            Instantiate(prefabFlecha, puntoDeDisparo.position, rotacionFlecha);
        }
    }

    void Voltear()
    {
        mirandoDerecha = !mirandoDerecha;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}
