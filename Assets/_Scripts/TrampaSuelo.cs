using UnityEngine;

public class TrampaSuelo : MonoBehaviour
{
    [Header("Configuración de Daño")]
    public float cantidadDano = 20f;
    public float tiempoEntreDano = 1.5f;

    // Aquí guardaremos la hora exacta en la que puede volver a hacer daño
    private float tiempoProximoDano = 0f;

    // Solo usamos Stay. Se activa nada más rozar y mientras te mantengas encima
    void OnCollisionStay2D(Collision2D collision)
    {
        // Comprobamos si el reloj del juego ya ha superado el tiempo de espera
        if (Time.time >= tiempoProximoDano)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null && !player.esInvulnerable)
            {
                player.RecibirDano(cantidadDano);

                // Calculamos la hora exacta para el siguiente golpe sumando 1.5 segundos
                tiempoProximoDano = Time.time + tiempoEntreDano;
            }
        }
    }
}