using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Valores de Control")]
    [Range(-1f, 1f)] public float entradaX = 0f;
    [Range(-1f, 1f)] public float entradaY = 0f;
    public bool botonBajar = false;

    [Header("Simulación")]
    public bool usarTeclado = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (usarTeclado)
        {
            entradaX = Input.GetAxisRaw("Horizontal"); // Flechas Derecha/Izquierda o A/D
            entradaY = Input.GetAxisRaw("Vertical");   // Flechas Arriba/Abajo o W/S
            botonBajar = Input.GetKey(KeyCode.Space);  // Barra espaciadora para activar la garra
        }
    }

    public void InyectarDatosHardware(float x, float y, bool boton)
    {
        if (!usarTeclado)
        {
            entradaX = x;
            entradaY = y;
            botonBajar = boton;
        }
    }
}