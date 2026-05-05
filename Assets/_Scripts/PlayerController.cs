using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float velocidadMovimiento = 8f;
    public float fuerzaSalto = 10f;

    [Header("Detección de Suelo y Pared")]
    public Transform piesPosicion;
    public float radioDeteccion = 0.3f;
    public LayerMask queEsSuelo;
    public Transform detectorPared;
    public LayerMask queEsPared;

    [Header("Interacción e Inventario")]
    public float distanciaInteraccion = 1.5f;
    public LayerMask queEsInteractuable;

    // --- REDUCIDO A 2 HUECOS ---
    public List<string> slotsComida = new List<string> { "", "" };
    public List<string> slotsObjetos = new List<string> { "", "" };

    [Header("Imágenes para la Interfaz")]
    public Sprite iconoManzana;
    public Sprite iconoPiezaMetal;
    public Sprite iconoTornillo;
    public Sprite iconoCable;

    [Header("Supervivencia")]
    public float vidaMaxima = 100f;
    public float vidaActual = 100f;
    public float resistenciaMaxima = 100f;
    public float resistenciaActual = 100f;
    public float gastoEscalada = 20f;
    public float gastoSaltoPared = 15f;

    [Header("Ayudas de Salto")]
    public float tiempoBufferSalto = 0.2f;
    private float contadorBufferSalto;
    public float tiempoCoyote = 0.15f;
    private float contadorCoyote;

    private ProgressBar uiHealthBar;
    private ProgressBar uiStaminaBar;

    // --- REDUCIDO A 2 ELEMENTOS ---
    private VisualElement[] iconosInventarioComida = new VisualElement[2];
    private VisualElement[] iconosInventarioObjetos = new VisualElement[2];

    private Rigidbody2D rb;
    private Animator anim;
    private float inputHorizontal;
    private float inputVertical;
    private bool estaEnSuelo;
    private bool estaEnPared;
    private bool estaEscalando;
    private bool mirandoDerecha = true;
    private float contadorCooldownAgarre;
    private float contadorAireEstable;

    void OnEnable()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) uiDocument = FindFirstObjectByType<UIDocument>();
        if (uiDocument != null)
        {
            VisualElement root = uiDocument.rootVisualElement;
            uiHealthBar = root.Q<ProgressBar>("HealthBar");
            uiStaminaBar = root.Q<ProgressBar>("StaminaBar");

            // --- BUCLE REDUCIDO A 2 ---
            for (int i = 0; i < 2; i++)
            {
                VisualElement slotComida = root.Q<VisualElement>($"Slot_Comida_{i + 1}");
                if (slotComida != null) iconosInventarioComida[i] = slotComida.Q<VisualElement>("Icon");

                VisualElement slotObjeto = root.Q<VisualElement>($"Slot_Objeto_{i + 1}");
                if (slotObjeto != null) iconosInventarioObjetos[i] = slotObjeto.Q<VisualElement>("Icon");
            }
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        vidaActual = vidaMaxima;
        resistenciaActual = resistenciaMaxima;
    }

    void Update()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");

        if (contadorCooldownAgarre > 0f) contadorCooldownAgarre -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.E))
        {
            float direccion = mirandoDerecha ? 1f : -1f;
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(0.5f, 1f), 0f, new Vector2(direccion, 0f), distanciaInteraccion, queEsInteractuable);

            if (hit.collider != null)
            {
                IInteractable objeto = hit.collider.GetComponent<IInteractable>();
                if (objeto != null) objeto.Interactuar(this);
            }
        }

        // --- SOLO TECLAS 1 y 2 AHORA ---
        if (Input.GetKeyDown(KeyCode.Alpha1)) UsarSlotComida(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UsarSlotComida(1);

        ManejarResistencia();
        ManejarSaltoYEscala();
        ActualizarUI();
        ManejarAnimaciones();
        ManejarGiro();
    }

    void ManejarResistencia()
    {
        float topeResistencia = (vidaActual / vidaMaxima) * resistenciaMaxima;
        if (estaEnSuelo && !estaEscalando)
            resistenciaActual = Mathf.MoveTowards(resistenciaActual, topeResistencia, 50f * Time.deltaTime);
    }

    void ManejarSaltoYEscala()
    {
        if (estaEnSuelo) contadorCoyote = tiempoCoyote; else contadorCoyote -= Time.deltaTime;
        if (Input.GetButtonDown("Jump")) contadorBufferSalto = tiempoBufferSalto; else contadorBufferSalto -= Time.deltaTime;

        if (estaEnPared && Input.GetKey(KeyCode.LeftShift) && resistenciaActual > 0 && contadorCooldownAgarre <= 0f)
        {
            estaEscalando = true;
            resistenciaActual -= gastoEscalada * Time.deltaTime;
        }
        else estaEscalando = false;

        if (contadorBufferSalto > 0f)
        {
            if (estaEnSuelo || contadorCoyote > 0f) Saltar();
            else if (estaEscalando) SaltarDesdePared();
        }
    }

    void ActualizarUI()
    {
        if (uiHealthBar != null)
        {
            uiHealthBar.value = vidaActual;
            int porcVida = Mathf.Clamp(Mathf.RoundToInt((vidaActual / vidaMaxima) * 100f), 0, 100);
            uiHealthBar.title = $"{porcVida}%";
        }
        if (uiStaminaBar != null)
        {
            uiStaminaBar.highValue = (vidaActual / vidaMaxima) * resistenciaMaxima;
            uiStaminaBar.value = resistenciaActual;
            int porcStam = Mathf.Clamp(Mathf.RoundToInt((resistenciaActual / resistenciaMaxima) * 100f), 0, 100);
            uiStaminaBar.title = $"{porcStam}%";
        }

        // --- BUCLE REDUCIDO A 2 ---
        for (int i = 0; i < 2; i++)
        {
            if (iconosInventarioComida[i] != null)
            {
                if (slotsComida[i] == "Fruta") iconosInventarioComida[i].style.backgroundImage = new StyleBackground(iconoManzana);
                else iconosInventarioComida[i].style.backgroundImage = null;
            }

            if (iconosInventarioObjetos[i] != null)
            {
                if (slotsObjetos[i] == "Pieza Metal")
                    iconosInventarioObjetos[i].style.backgroundImage = new StyleBackground(iconoPiezaMetal);
                else if (slotsObjetos[i] == "Tornillo")
                    iconosInventarioObjetos[i].style.backgroundImage = new StyleBackground(iconoTornillo);
                else if (slotsObjetos[i] == "Cable")
                    iconosInventarioObjetos[i].style.backgroundImage = new StyleBackground(iconoCable);
                else
                    iconosInventarioObjetos[i].style.backgroundImage = null;
            }
        }
    }

    void UsarSlotComida(int index)
    {
        if (slotsComida[index] == "Fruta")
        {
            vidaActual = Mathf.Clamp(vidaActual + 20f, 0, vidaMaxima);
            slotsComida[index] = "";
            Debug.Log("Has comido una fruta. Vida restaurada.");
        }
    }

    public void RecogerFruta()
    {
        for (int i = 0; i < slotsComida.Count; i++)
        {
            if (slotsComida[i] == "") { slotsComida[i] = "Fruta"; break; }
        }
    }

    public void RecogerObjetoBasura(string nombreObjeto)
    {
        for (int i = 0; i < slotsObjetos.Count; i++)
        {
            if (slotsObjetos[i] == "") { slotsObjetos[i] = nombreObjeto; break; }
        }
    }

    void FixedUpdate()
    {
        if (estaEscalando)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = new Vector2(0, inputVertical * (velocidadMovimiento * 0.6f));
        }
        else
        {
            rb.gravityScale = 2.5f;
            rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, rb.linearVelocity.y);
        }
        estaEnSuelo = Physics2D.OverlapCircle(piesPosicion.position, radioDeteccion, queEsSuelo);
        estaEnPared = Physics2D.OverlapCircle(detectorPared.position, 0.3f, queEsPared);
    }

    void Saltar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
        contadorBufferSalto = 0f; contadorCoyote = 0f;
    }

    void SaltarDesdePared()
    {
        resistenciaActual -= gastoSaltoPared;
        rb.linearVelocity = Vector2.zero;
        contadorCooldownAgarre = 0.2f;
        estaEscalando = false;
        float dir = mirandoDerecha ? -1 : 1;
        rb.AddForce(new Vector2(dir * 5f, fuerzaSalto * 1.1f), ForceMode2D.Impulse);
        contadorBufferSalto = 0f;
    }

    void ManejarAnimaciones()
    {
        if (anim == null) return;
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        if (estaEnSuelo) contadorAireEstable = 0.05f; else contadorAireEstable -= Time.deltaTime;
        anim.SetBool("isGrounded", contadorAireEstable > 0f);
        anim.SetBool("isClimbing", estaEscalando);
        if (estaEscalando) anim.SetFloat("VerticalSpeed", Mathf.Abs(rb.linearVelocity.y));
        else anim.SetFloat("VerticalSpeed", 1f);
    }

    void ManejarGiro()
    {
        if (!estaEscalando)
        {
            if (inputHorizontal > 0 && !mirandoDerecha) Voltear();
            else if (inputHorizontal < 0 && mirandoDerecha) Voltear();
        }
    }

    void Voltear()
    {
        mirandoDerecha = !mirandoDerecha;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmos()
    {
        if (piesPosicion) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(piesPosicion.position, radioDeteccion); }
        if (detectorPared) { Gizmos.color = Color.blue; Gizmos.DrawWireSphere(detectorPared.position, 0.3f); }
    }
}