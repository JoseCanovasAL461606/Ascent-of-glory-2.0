using UnityEngine;

public class BasuraInteractuable : MonoBehaviour, IInteractable
{
    [Header("Configuración de Limpieza")]
    public float tiempoLimpiezaNecesario = 1.5f;

    [Header("Imágenes de la Basura")]
    public Sprite spriteSucia;
    public Sprite spriteLimpia;

    private float contadorLimpieza = 0f;
    private bool estaLimpia = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Al empezar, siempre mostramos la imagen de la basura sucia
        if (spriteRenderer != null && spriteSucia != null)
        {
            spriteRenderer.sprite = spriteSucia;
        }
    }

    public void Interactuar(PlayerController player)
    {
        // Si aún no está limpia, permitimos interactuar
        if (!estaLimpia)
        {
            Debug.Log("Limpiando la basura...");
            contadorLimpieza += 0.5f;

            if (contadorLimpieza >= tiempoLimpiezaNecesario)
            {
                estaLimpia = true; // Marcamos que ya se ha limpiado

                // Cambiamos a la imagen limpia
                if (spriteRenderer != null && spriteLimpia != null)
                {
                    spriteRenderer.sprite = spriteLimpia;
                }

                // Damos la recompensa
                string[] posiblesObjetos = { "Pieza Metal", "Tornillo", "Cable" };
                string objetoGanado = posiblesObjetos[Random.Range(0, posiblesObjetos.Length)];

                player.RecogerObjetoBasura(objetoGanado);
                Debug.Log("¡Has terminado de limpiar la basura y encontrado: " + objetoGanado + "!");
            }
        }
        else
        {
            Debug.Log("Esta zona ya está completamente limpia.");
        }
    }
}