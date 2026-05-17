using UnityEngine;

public class FlechaDirigida : MonoBehaviour
{
    public float velocidad = 12f;
    public float cantidadDano = 12.5f;
    public float tiempoDeVida = 5f;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        rb.linearVelocity = transform.right * velocidad;

        Destroy(gameObject, tiempoDeVida);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
      
        if (collision.GetComponent<ArqueroEnemigo>() != null || collision.GetComponent<FlechaDirigida>() != null)
            return;

        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            if (player.esInvulnerable) return; 
            player.RecibirDano(cantidadDano);
        }

        Destroy(gameObject);
    }
}
