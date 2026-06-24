using UnityEngine;
using UnityEngine.InputSystem; 

public class BanderaFinal : MonoBehaviour
{
    [Header("Conexión con la UI Toolkit")]
    public MenuVictoria menuVictoria; 

    private ControlesJugador controles;
    private bool jugadorCerca = false;

    void Awake()
    {
        controles = new ControlesJugador();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;

          
            controles.Jugador.Interactuar.performed += AlInteractuar;
            controles.Enable();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DesactivarControles();
        }
    }

    private void OnDisable()
    {
        
        DesactivarControles();
    }

    private void DesactivarControles()
    {
        if (jugadorCerca)
        {
            jugadorCerca = false;
            controles.Jugador.Interactuar.performed -= AlInteractuar;
            controles.Disable();
        }
    }

   
    private void AlInteractuar(InputAction.CallbackContext contexto)
    {
        FinalizarJuego();
    }

    void FinalizarJuego()
    {
        
        DesactivarControles();

        if (menuVictoria != null)
        {
           
            int puntosFinales = GameManager.instance.puntosTotales;
            menuVictoria.MostrarVictoria(puntosFinales);
        }
        else
        {
           
            Time.timeScale = 0f;
        }
    }
}