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
        if (spriteRenderer != null && spriteSucia != null) spriteRenderer.sprite = spriteSucia;
    }

    public void Interactuar(PlayerController player)
    {
        if (!estaLimpia)
        {
            contadorLimpieza += 0.5f;

            if (contadorLimpieza >= tiempoLimpiezaNecesario)
            {
                estaLimpia = true;
                if (spriteRenderer != null && spriteLimpia != null) spriteRenderer.sprite = spriteLimpia;

                // --- ACTUALIZADO CON TUS NUEVOS OBJETOS ---
                string[] posiblesObjetos = { "Lata", "Muelle", "Burbuja" };
                string objetoGanado = posiblesObjetos[Random.Range(0, posiblesObjetos.Length)];

                player.RecogerObjetoBasura(objetoGanado);
                Debug.Log("¡Encontraste: " + objetoGanado + "!");
            }
        }
    }
}