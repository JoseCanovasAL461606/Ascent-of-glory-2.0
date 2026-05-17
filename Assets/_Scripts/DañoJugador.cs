using System.Collections;
using UnityEngine;

public class DanoJugador : MonoBehaviour
{
    private SpriteRenderer miDibujo;

    void Start()
    {
    
        miDibujo = GetComponent<SpriteRenderer>();
    }

   
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            StartCoroutine(EfectoParpadeo()); 
        }
    }

    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo"))
        {
            StartCoroutine(EfectoParpadeo());
        }
    }

 
    IEnumerator EfectoParpadeo()
    {
      
        miDibujo.color = Color.red;

       
        yield return new WaitForSeconds(0.2f);

      
        miDibujo.color = Color.white;
    }
}
