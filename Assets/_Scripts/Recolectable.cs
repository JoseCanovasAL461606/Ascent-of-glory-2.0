using UnityEngine;

public class Recolectable : MonoBehaviour
{
    public int puntosQueDa = 100;
    

    private bool jugadorCerca = false;
    private bool yaPuntuado = false;

    
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
       
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E) && !yaPuntuado)
        {
            GameManager.instance.SumarPuntos(puntosQueDa);
            yaPuntuado = true;

           
        }
    }
}