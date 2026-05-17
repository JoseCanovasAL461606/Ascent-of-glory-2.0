using UnityEngine;

public class ArbolInteractuable : MonoBehaviour, IInteractable
{
    [Header("Configuración del Árbol")]
    public float tiempoRiegoNecesario = 1.5f;
    public int frutasDisponibles = 3;

    [Header("Imágenes del Árbol")]
    public Sprite spriteSeco;
    public Sprite spriteConHojas;

    private float contadorRiego = 0f;
    private bool estaRegado = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = spriteSeco;
        }
    }

    public void Interactuar(PlayerController player)
    {
      
        if (!estaRegado)
        {
            Debug.Log("Regando el árbol seco...");
            contadorRiego += 0.5f;

            if (contadorRiego >= tiempoRiegoNecesario)
            {
                estaRegado = true; 

              
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = spriteConHojas;
                }

                DarFrutaDirecta(player);
                Debug.Log("¡El árbol ha revivido y la fruta está en tu inventario!");
            }
        }
       
        else if (frutasDisponibles > 0)
        {
            DarFrutaDirecta(player);
            Debug.Log("Fruta añadida al inventario.");
        }
        else
        {
            Debug.Log("El árbol ya no tiene más frutos.");
        }
    }

    void DarFrutaDirecta(PlayerController player)
    {
        
        player.RecogerFruta();
        frutasDisponibles--;
    }
}