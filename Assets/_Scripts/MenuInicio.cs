using UnityEngine;
using UnityEngine.UIElements; 
using UnityEngine.SceneManagement;

public class MenuInicio : MonoBehaviour
{
    private void OnEnable()
    {
        
        UIDocument uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        
        Button btnJugar = root.Q<Button>("BotonJugar");
        

        
        if (btnJugar != null) btnJugar.clicked += Jugar;
        
    }

    private void Jugar()
    {
        
        SceneManager.LoadScene("Nivel_1");
    }

   
}
