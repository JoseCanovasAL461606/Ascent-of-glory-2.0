using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 
public class MenuPausa : MonoBehaviour
{
    private VisualElement root;
    private bool estaPausado = false;

    private void OnEnable()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

       
        root.style.display = DisplayStyle.None;

        Button btnReanudar = root.Q<Button>("BotonReanudar");
        Button btnSalir = root.Q<Button>("BotonSalirMenu");

        if (btnReanudar != null) btnReanudar.clicked += Reanudar;
        if (btnSalir != null) btnSalir.clicked += SalirAlMenu;
    }

    private void Update()
    {
        
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (estaPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    private void Pausar()
    {
        estaPausado = true;
        Time.timeScale = 0f;

       
        root.style.display = DisplayStyle.Flex;
    }

    private void Reanudar()
    {
        estaPausado = false;
        Time.timeScale = 1f; 

       
        root.style.display = DisplayStyle.None;
    }

    private void SalirAlMenu()
    {
        
        Time.timeScale = 1f;

       
        SceneManager.LoadScene("MenuPrincipal");
    }
}
