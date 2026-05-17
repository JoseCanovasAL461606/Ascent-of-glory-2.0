using UnityEngine;

public class MenuPausa : MonoBehaviour
{
    public GameObject panelPausa; 
    private bool juegoPausado = false;

    void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        panelPausa.SetActive(true);
        Time.timeScale = 0f; 
        juegoPausado = true;
    }

    public void Reanudar()
    {
        panelPausa.SetActive(false);
        Time.timeScale = 1f; 
        juegoPausado = false;
    }

    public void SalirDelJuego()
    {
        Debug.Log("Cerrando el juego..."); 
        Application.Quit(); 
    }
}
