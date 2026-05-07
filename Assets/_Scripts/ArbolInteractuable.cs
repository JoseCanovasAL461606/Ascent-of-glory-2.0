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

        // Al empezar, el árbol siempre está seco
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = spriteSeco;
        }
    }

    public void Interactuar(PlayerController player)
    {
        // 1. FASE DE RIEGO (Si el árbol está seco)
        if (!estaRegado)
        {
            Debug.Log("Regando el árbol seco...");
            contadorRiego += 0.5f;

            if (contadorRiego >= tiempoRiegoNecesario)
            {
                estaRegado = true; // El árbol revive

                // Cambiamos la imagen
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = spriteConHojas;
                }

                DarFrutaDirecta(player);
                Debug.Log("¡El árbol ha revivido y la fruta está en tu inventario!");
            }
        }
        // 2. FASE DE RECOLECCIÓN (Si ya tiene hojas)
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
        // Esto añade la fruta internamente al inventario del jugador sin instanciar nada en el mundo
        player.RecogerFruta();
        frutasDisponibles--;
    }
}