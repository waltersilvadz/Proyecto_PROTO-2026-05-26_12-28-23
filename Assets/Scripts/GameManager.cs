using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int puntaje = 0;
    public TextMeshProUGUI txtPuntaje;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ActualizarInterfaz();
    }

    public void SumarPuntos(int cantidad)
    {
        puntaje += cantidad;
        ActualizarInterfaz();
    }

    void ActualizarInterfaz()
    {
        if (txtPuntaje != null)
        {
            txtPuntaje.text = $"CONEXION_ESTACION: ONLINE\nTELEMETRIA_SCORE: {puntaje:D4}";
        }
    }
}