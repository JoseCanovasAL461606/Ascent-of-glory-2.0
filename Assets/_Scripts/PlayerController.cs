using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float velocidadMovimiento = 8f;
    public float fuerzaSalto = 10f;

    [Header("Detección de Suelo y Pared")]
    public Transform piesPosicion;
    public float radioDeteccionSuelo = 0.3f;
    public LayerMask queEsSuelo;
    public Transform detectorPared;
    public float radioDeteccionPared = 0.1f;
    public LayerMask queEsPared;

    [Header("Interacción e Inventario")]
    public float distanciaInteraccion = 1.5f;
    public LayerMask queEsInteractuable;

    public List<string> slotsComida = new List<string> { "", "" };
    public List<string> slotsObjetos = new List<string> { "", "" };

    [Header("Imágenes para la Interfaz")]
    public Sprite iconoManzana;
    public Sprite iconoLata;
    public Sprite iconoMuelle;
    public Sprite iconoBurbuja;

    [Header("Supervivencia")]
    public float vidaMaxima = 100f;
    public float vidaActual = 100f;
    public float resistenciaMaxima = 100f;
    public float resistenciaActual = 100f;
    public float gastoEscalada = 20f;
    public float gastoSaltoPared = 15f;

    [Header("Estados Alterados (Power-Ups)")]
    private float tiempoResistenciaIlimitada = 0f;
    private float tiempoInvulnerabilidad = 0f;
    private bool superSaltoActivo = false;
    public bool esInvulnerable = false;
    public GameObject escudoBurbujaVisual;

    [Header("Ayudas de Salto")]
    public float tiempoBufferSalto = 0.2f;
    private float contadorBufferSalto;
    public float tiempoCoyote = 0.15f;
    private float contadorCoyote;

    private float contadorPerdidaControl;

    [Header("Muerte")]
    public Sprite spriteMuerto;

    private ProgressBar uiHealthBar;
    private ProgressBar uiStaminaBar;
    private Label uiScoreLabel;
    private VisualElement[] iconosInventarioComida = new VisualElement[2];
    private VisualElement[] iconosInventarioObjetos = new VisualElement[2];

    private Rigidbody2D rb;
    private Animator anim;
    private float inputHorizontal;
    private float inputVertical;
    private bool inputEscalar; 
    private bool estaEnSuelo;
    private bool estaEnPared;
    private bool estaEscalando;
    private bool mirandoDerecha = true;
    private float contadorCooldownAgarre;
    private float contadorAireEstable;
    private bool estaMuerto = false;

    private ControlesJugador controles;

    void Awake()
    {
        controles = new ControlesJugador();
    }

    void OnEnable()
    {
        controles.Enable();

    
        controles.Jugador.Mover.performed += LeerMovimiento;
        controles.Jugador.Mover.canceled += LeerMovimiento;

        controles.Jugador.Saltar.performed += AlSaltar;

        controles.Jugador.Escalar.performed += EmpezarEscalar;
        controles.Jugador.Escalar.canceled += TerminarEscalar;

        controles.Jugador.Interactuar.performed += AlInteractuar;

        controles.Jugador.Obj1.performed += AlUsarObj1;
        controles.Jugador.Obj2.performed += AlUsarObj2;
        controles.Jugador.Com1.performed += AlUsarCom1;
        controles.Jugador.Com2.performed += AlUsarCom2;

        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) uiDocument = FindFirstObjectByType<UIDocument>();
        if (uiDocument != null)
        {
            VisualElement root = uiDocument.rootVisualElement;
            uiHealthBar = root.Q<ProgressBar>("HealthBar");
            uiStaminaBar = root.Q<ProgressBar>("StaminaBar");
            uiScoreLabel = root.Q<Label>("TextoPuntosHUD");

            for (int i = 0; i < 2; i++)
            {
                VisualElement slotComida = root.Q<VisualElement>($"Slot_Comida_{i + 1}");
                if (slotComida != null) iconosInventarioComida[i] = slotComida.Q<VisualElement>("Icon");

                VisualElement slotObjeto = root.Q<VisualElement>($"Slot_Objeto_{i + 1}");
                if (slotObjeto != null) iconosInventarioObjetos[i] = slotObjeto.Q<VisualElement>("Icon");
            }
        }
    }

    void OnDisable()
    {
     
        controles.Jugador.Mover.performed -= LeerMovimiento;
        controles.Jugador.Mover.canceled -= LeerMovimiento;

        controles.Jugador.Saltar.performed -= AlSaltar;

        controles.Jugador.Escalar.performed -= EmpezarEscalar;
        controles.Jugador.Escalar.canceled -= TerminarEscalar;

        controles.Jugador.Interactuar.performed -= AlInteractuar;

        controles.Jugador.Obj1.performed -= AlUsarObj1;
        controles.Jugador.Obj2.performed -= AlUsarObj2;
        controles.Jugador.Com1.performed -= AlUsarCom1;
        controles.Jugador.Com2.performed -= AlUsarCom2;

        controles.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        vidaActual = vidaMaxima;
        resistenciaActual = resistenciaMaxima;

        if (escudoBurbujaVisual != null) escudoBurbujaVisual.SetActive(false);
    }

   
    private void LeerMovimiento(InputAction.CallbackContext contexto)
    {
        Vector2 movimiento = contexto.ReadValue<Vector2>();
        inputHorizontal = movimiento.x;
        inputVertical = movimiento.y;
    }

    private void AlSaltar(InputAction.CallbackContext contexto)
    {
        if (!estaMuerto) contadorBufferSalto = tiempoBufferSalto;
    }

    private void EmpezarEscalar(InputAction.CallbackContext contexto) => inputEscalar = true;
    private void TerminarEscalar(InputAction.CallbackContext contexto) => inputEscalar = false;

    private void AlInteractuar(InputAction.CallbackContext contexto)
    {
        if (estaMuerto) return;

        float direccion = mirandoDerecha ? 1f : -1f;
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(0.5f, 1f), 0f, new Vector2(direccion, 0f), distanciaInteraccion, queEsInteractuable);

        if (hit.collider != null)
        {
            IInteractable objeto = hit.collider.GetComponent<IInteractable>();
            if (objeto != null) objeto.Interactuar(this);
        }
    }

    private void AlUsarObj1(InputAction.CallbackContext contexto) { if (!estaMuerto) UsarSlotObjeto(0); }
    private void AlUsarObj2(InputAction.CallbackContext contexto) { if (!estaMuerto) UsarSlotObjeto(1); }
    private void AlUsarCom1(InputAction.CallbackContext contexto) { if (!estaMuerto) UsarSlotComida(0); }
    private void AlUsarCom2(InputAction.CallbackContext contexto) { if (!estaMuerto) UsarSlotComida(1); }
    

    void Update()
    {
        if (estaMuerto) return;

       
        if (contadorCooldownAgarre > 0f) contadorCooldownAgarre -= Time.deltaTime;
        if (contadorPerdidaControl > 0f) contadorPerdidaControl -= Time.deltaTime;

        ManejarPowerUps();
        ManejarResistencia();
        ManejarSaltoYEscala();
        ActualizarUI();
        ManejarAnimaciones();
        ManejarGiro();
    }

    void ManejarPowerUps()
    {
        if (tiempoResistenciaIlimitada > 0) tiempoResistenciaIlimitada -= Time.deltaTime;

        if (tiempoInvulnerabilidad > 0)
        {
            tiempoInvulnerabilidad -= Time.deltaTime;
            esInvulnerable = true;
            if (tiempoInvulnerabilidad <= 0)
            {
                esInvulnerable = false;
                if (escudoBurbujaVisual != null) escudoBurbujaVisual.SetActive(false);
            }
        }
    }

    void UsarSlotComida(int index)
    {
        if (slotsComida[index] == "Fruta")
        {
            float curacion = vidaMaxima * 0.25f;
            vidaActual = Mathf.Clamp(vidaActual + curacion, 0, vidaMaxima);
            slotsComida[index] = "";
        }
    }

    void UsarSlotObjeto(int index)
    {
        string objeto = slotsObjetos[index];
        if (objeto == "") return;

        if (objeto == "Lata") tiempoResistenciaIlimitada = 3f;
        else if (objeto == "Muelle") superSaltoActivo = true;
        else if (objeto == "Burbuja")
        {
            tiempoInvulnerabilidad = 3f;
            esInvulnerable = true;
            if (escudoBurbujaVisual != null) escudoBurbujaVisual.SetActive(true);
        }
        slotsObjetos[index] = "";
    }

    public void RecibirDano(float cantidad)
    {
        if (estaMuerto || esInvulnerable) return;

        vidaActual = Mathf.Clamp(vidaActual - cantidad, 0, vidaMaxima);
        ActualizarUI();

        if (vidaActual <= 0)
        {
            StartCoroutine(RutinaMuerte());
        }
    }

    IEnumerator RutinaMuerte()
    {
        estaMuerto = true;

        if (anim != null) anim.enabled = false;

        yield return new WaitUntil(() => estaEnSuelo == true);

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        CapsuleCollider2D col = GetComponent<CapsuleCollider2D>();
        if (col != null) col.enabled = false;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && spriteMuerto != null) sr.sprite = spriteMuerto;

        transform.position = new Vector3(transform.position.x, transform.position.y - 1.2f, transform.position.z);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ManejarResistencia()
    {
        float topeResistencia = (vidaActual / vidaMaxima) * resistenciaMaxima;
        if (estaEnSuelo && !estaEscalando)
        {
            resistenciaActual = Mathf.MoveTowards(resistenciaActual, topeResistencia, 50f * Time.deltaTime);
        }
        if (tiempoResistenciaIlimitada > 0) resistenciaActual = topeResistencia;
    }

    void ManejarSaltoYEscala()
    {
        if (estaEnSuelo) contadorCoyote = tiempoCoyote; else contadorCoyote -= Time.deltaTime;

      
        contadorBufferSalto -= Time.deltaTime;

        bool tieneStaminaParaEscalar = resistenciaActual > 0 || tiempoResistenciaIlimitada > 0;

       
        if (estaEnPared && !estaEnSuelo && inputEscalar && tieneStaminaParaEscalar && contadorCooldownAgarre <= 0f)
        {
            estaEscalando = true;
            if (tiempoResistenciaIlimitada <= 0) resistenciaActual -= gastoEscalada * Time.deltaTime;
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
        if (uiScoreLabel != null && GameManager.instance != null)
        {
            uiScoreLabel.text = "Puntuacion: " + GameManager.instance.puntosTotales;
        }

        for (int i = 0; i < 2; i++)
        {
            if (iconosInventarioComida[i] != null)
                iconosInventarioComida[i].style.backgroundImage = (slotsComida[i] == "Fruta") ? new StyleBackground(iconoManzana) : null;

            if (iconosInventarioObjetos[i] != null)
            {
                if (slotsObjetos[i] == "Lata") iconosInventarioObjetos[i].style.backgroundImage = new StyleBackground(iconoLata);
                else if (slotsObjetos[i] == "Muelle") iconosInventarioObjetos[i].style.backgroundImage = new StyleBackground(iconoMuelle);
                else if (slotsObjetos[i] == "Burbuja") iconosInventarioObjetos[i].style.backgroundImage = new StyleBackground(iconoBurbuja);
                else iconosInventarioObjetos[i].style.backgroundImage = null;
            }
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
        if (estaMuerto) return;

        if (estaEscalando)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = new Vector2(0, inputVertical * (velocidadMovimiento * 0.6f));
        }
        else
        {
            rb.gravityScale = 2.5f;

            if (contadorPerdidaControl <= 0f)
            {
                rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, rb.linearVelocity.y);
            }
        }

        estaEnSuelo = Physics2D.OverlapCircle(piesPosicion.position, radioDeteccionSuelo, queEsSuelo);
        estaEnPared = Physics2D.OverlapCircle(detectorPared.position, radioDeteccionPared, queEsPared);
    }

    void Saltar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        float fuerzaAplicada = superSaltoActivo ? fuerzaSalto * 1.6f : fuerzaSalto;
        superSaltoActivo = false;
        rb.AddForce(Vector2.up * fuerzaAplicada, ForceMode2D.Impulse);
        contadorBufferSalto = 0f; contadorCoyote = 0f;
    }

    void SaltarDesdePared()
    {
        if (tiempoResistenciaIlimitada <= 0) resistenciaActual -= gastoSaltoPared;
        rb.linearVelocity = Vector2.zero;
        contadorCooldownAgarre = 0.15f;

        estaEscalando = false;

        float dir = mirandoDerecha ? -1f : 1f;

        if (inputHorizontal == 0 || (inputHorizontal > 0 && mirandoDerecha) || (inputHorizontal < 0 && !mirandoDerecha))
        {
            rb.AddForce(new Vector2(dir * 2f, fuerzaSalto * 1.1f), ForceMode2D.Impulse);
            contadorPerdidaControl = 0.05f;
        }
        else
        {
            rb.AddForce(new Vector2(dir * 6f, fuerzaSalto * 1.1f), ForceMode2D.Impulse);
            contadorPerdidaControl = 0.25f;
            Voltear();
        }

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
        if (!estaEscalando && contadorPerdidaControl <= 0f)
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
        if (piesPosicion) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(piesPosicion.position, radioDeteccionSuelo); }
        if (detectorPared) { Gizmos.color = Color.blue; Gizmos.DrawWireSphere(detectorPared.position, radioDeteccionPared); }
    }
}