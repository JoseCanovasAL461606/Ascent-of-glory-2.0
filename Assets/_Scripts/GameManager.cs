using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int puntosTotales = 0;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public void SumarPuntos(int cantidad)
    {
        puntosTotales += cantidad;
        
    }
}