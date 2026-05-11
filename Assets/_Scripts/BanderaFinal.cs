using UnityEngine;
using TMPro;

public class BanderaFinal : MonoBehaviour
{
    public GameObject cartelVictoria; 
    
    public TextMeshProUGUI textoPuntosFinales; 

    private bool jugadorCerca = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;
           
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = false;
           
        }
    }

    void Update()
    {
        
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            FinalizarJuego();
        }
    }

    void FinalizarJuego()
    {
     

       
        if (cartelVictoria != null)
        {
            cartelVictoria.SetActive(true);

            
            if (textoPuntosFinales != null)
            {
                textoPuntosFinales.text = "Puntuación final: " + GameManager.instance.puntosTotales;
            }
        }

      
        Time.timeScale = 0f;
    }
}