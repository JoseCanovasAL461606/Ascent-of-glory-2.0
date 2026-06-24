using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuVictoria : MonoBehaviour
{
    private VisualElement root;
    private Label textoPuntuacion;

    private void OnEnable()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

       
        root.style.display = DisplayStyle.None;

      
        textoPuntuacion = root.Q<Label>("TextoPuntuacion");
        Button btnMenu = root.Q<Button>("BotonMenu");
        Button btnSalir = root.Q<Button>("BotonSalir");

      
        if (btnMenu != null) btnMenu.clicked += IrAlMenu;
        if (btnSalir != null) btnSalir.clicked += SalirDelJuego;
    }

    
    public void MostrarVictoria(int puntuacionFinal)
    {
        Time.timeScale = 0f; 

     
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text = "Puntuacion: " + puntuacionFinal;
        }

        root.style.display = DisplayStyle.Flex; 
    }

    private void IrAlMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MenuPrincipal"); 
    }

    private void SalirDelJuego()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit(); 
    }
}
