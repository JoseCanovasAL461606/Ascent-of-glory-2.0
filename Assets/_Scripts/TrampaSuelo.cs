using UnityEngine;

public class TrampaSuelo : MonoBehaviour
{
    [Header("Configuración de Daño")]
    public float cantidadDano = 20f;
    public float tiempoEntreDano = 1.5f;

    
    private float tiempoProximoDano = 0f;

    
    void OnCollisionStay2D(Collision2D collision)
    {
       
        if (Time.time >= tiempoProximoDano)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null && !player.esInvulnerable)
            {
                player.RecibirDano(cantidadDano);

                
                tiempoProximoDano = Time.time + tiempoEntreDano;
            }
        }
    }
}