using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuManager : MonoBehaviour
{
    public void EmpezarJuego()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Nivel_1"); 
    }
}