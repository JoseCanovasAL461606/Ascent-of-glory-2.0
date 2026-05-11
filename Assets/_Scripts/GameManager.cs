using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int puntosTotales = 0;
    public TextMeshProUGUI textoPuntuacion;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        // Al empezar, como los puntos son 0, ponemos un mensaje de ayuda
        ActualizarInterfaz();
    }

    public void SumarPuntos(int cantidad)
    {
        puntosTotales += cantidad;
        ActualizarInterfaz();
    }

    void ActualizarInterfaz()
    {
        if (textoPuntuacion != null)
        {
            if (puntosTotales == 0)
            {
                textoPuntuacion.text = "Puntuacion: 0";
            }
            else
            {
                textoPuntuacion.text = "Puntuacion: " + puntosTotales;
            }
        }
    }
}
