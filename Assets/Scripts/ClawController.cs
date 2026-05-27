using UnityEngine;

public class ClawController : MonoBehaviour
{
    [Header("Componentes de la Estructura")]
    public Transform ejeX; 
    public Transform modeloGarra; 

    [Header("Límites de Movimiento (Ajustar en Editor)")]
    public Vector2 limitesX = new Vector2(-4f, 4f);
    public Vector2 limitesZ = new Vector2(-4f, 4f);
    public float alturaMaximaY = 5f;
    public float profundidadMinimaY = 0.5f;

    [Header("Velocidades")]
    public float velocidadTraslacion = 4f;
    public float velocidadDescenso = 5f;

    private Vector3 posicionOriginalLocalGarra;
    public Transform puntoAnclaje;
    private Transform objetoSujetado = null;
    
    private bool descendiendo = false;
    private bool ascendiendo = false;

    void Start()
    {
        if (modeloGarra != null)
            posicionOriginalLocalGarra = modeloGarra.localPosition;

        posicionOriginalLocalGarra = modeloGarra.localPosition;
        if (puntoAnclaje == null) Debug.LogError("Por favor, verifica la jerarquía. No se encontró 'PuntoAnclaje'.");
    }

    void Update()
    {
        // Si el juego está pausado, no ejecutar movimientos
        if (MenuManager.isPaused) return;

        if (!descendiendo && !ascendiendo)
        {
            ProcesarMovimientoHorizontal();
            if (InputManager.Instance.botonBajar) descendiendo = true;
        }
        else
        {
            ProcesarCicloVertical();
        }
    }

    void ProcesarMovimientoHorizontal()
    {
        // Mover en X (Izquierda/Derecha) modificando el contenedor padre Eje_X
        float desplazamientoX = InputManager.Instance.entradaX * velocidadTraslacion * Time.deltaTime;
        Vector3 nuevaPosX = ejeX.position;
        nuevaPosX.x = Mathf.Clamp(nuevaPosX.x + desplazamientoX, limitesX.x, limitesX.y);
        ejeX.position = nuevaPosX;

        // Mover en Z (Adelante/Atrás) modificando la posición de este objeto Eje_Y_Carro
        float desplazamientoZ = InputManager.Instance.entradaY * velocidadTraslacion * Time.deltaTime;
        Vector3 nuevaPosZ = transform.position;
        nuevaPosZ.z = Mathf.Clamp(nuevaPosZ.z + desplazamientoZ, limitesZ.x, limitesZ.y);
        transform.position = nuevaPosZ;
    }

    void ProcesarCicloVertical()
    {
        if (descendiendo)
        {
            modeloGarra.Translate(Vector3.down * velocidadDescenso * Time.deltaTime, Space.World);
            if (modeloGarra.position.y <= profundidadMinimaY)
            {
                descendiendo = false;
                ascendiendo = true;
                IntentarCaptura();
            }
        }
        else if (ascendiendo)
        {
            Vector3 destinoLocal = new Vector3(modeloGarra.localPosition.x, posicionOriginalLocalGarra.y, modeloGarra.localPosition.z);
            modeloGarra.localPosition = Vector3.MoveTowards(modeloGarra.localPosition, destinoLocal, velocidadDescenso * Time.deltaTime);

            if (Mathf.Abs(modeloGarra.localPosition.y - posicionOriginalLocalGarra.y) < 0.01f)
            {
                ascendiendo = false;
            }
        }
    }

    void IntentarCaptura()
    {
        Collider[] colisiones = Physics.OverlapSphere(puntoAnclaje.position, 1.2f);
        foreach (var col in colisiones)
        {
            if (col.CompareTag("Premio") && objetoSujetado == null)
            {
                objetoSujetado = col.transform;
                Rigidbody rb = objetoSujetado.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                objetoSujetado.SetParent(puntoAnclaje);
                objetoSujetado.localPosition = Vector3.zero;
                break;
            }
        }
    }

    public void RegistrarEntregaExitosa()
    {
        if (objetoSujetado != null)
        {
            objetoSujetado.SetParent(null);
            Destroy(objetoSujetado.gameObject, 0.2f);
            objetoSujetado = null;
            GameManager.Instance.SumarPuntos(100);
        }
    }
}