using System.IO.Ports;
using UnityEngine;

public class ReceptorArduino : MonoBehaviour
{
    // Tu puerto identificado en Windows
    private SerialPort stream = new SerialPort("COM4", 115200); 

    [Header("Configuración de Calibración")]
    [Tooltip("Valor central real del potenciómetro físico cuando no lo tocas")]
    public float centroX = 512f;
    public float centroY = 512f;
    [Tooltip("Margen para ignorar pequeñas variaciones eléctricas en el centro")]
    public float zonaMuerta = 15f; 

    void Start()
    {
        // Si el InputManager está configurado para usar teclado, no abrimos el puerto para ahorrar recursos
        if (InputManager.Instance != null && InputManager.Instance.usarTeclado)
        {
            Debug.LogWarning("⚠️ ReceptorArduino: La simulación está en modo TECLADO. Puerto serial desactivado.");
            return;
        }

        ObtenerAccesoPuerto();
    }

    void ObtenerAccesoPuerto()
    {
        try 
        {
            stream.ReadTimeout = 50; 
            stream.Open();
            Debug.Log("🔌 Conexión exitosa con el periférico en el puerto COM4.");
        }
        catch (System.Exception e) 
        {
            Debug.LogError("❌ Error de comunicación serial: " + e.Message);
        }
    }

    void Update()
{
    // DEBÚG 1: ¿El modo teclado nos está bloqueando el puerto?
    if (InputManager.Instance != null && InputManager.Instance.usarTeclado)
    {
        if (stream.IsOpen) stream.Close();
        return;
    }

    // DEBÚG 2: ¿El puerto está cerrado? Intentamos abrir en vivo
    if (!stream.IsOpen)
    {
        Debug.LogWarning("🔌 [DEBUG] El puerto estaba cerrado. Intentando reconectar...");
        ObtenerAccesoPuerto();
        return;
    }

    // DEBÚG 3: ¿El puerto está abierto pero el buffer está vacío? (Sospechoso de falso contacto)
    if (stream.IsOpen)
    {
        // Esto te dirá si el puerto está vivo pero con 0 bytes llegando
        if (stream.BytesToRead == 0)
        {
            // Deja este log activo solo para la prueba, puede saturar la consola si hay falso contacto
            Debug.Log("⏳ [DEBUG] Puerto abierto en COM4, pero llegan 0 bytes. Revisa la conexión física del Arduino.");
        }
        else
        {
            Debug.Log($"📦 [DEBUG] ¡Están llegando bytes! Bytes en cola: {stream.BytesToRead}");
        }
    }

    // Lectura e inyección
    if (stream.IsOpen && stream.BytesToRead > 0)
    {
        try 
        {
            string datos = stream.ReadLine();
            
            // DEBÚG 4: ¿Qué string exacto está llegando por el cable?
            Debug.Log("📝 [DEBUG] String Crudo Recibido: " + datos);

            string[] valores = datos.Split(',');

            if (valores.Length == 4)
            {
                int rawX = int.Parse(valores[0]);
                int rawY = int.Parse(valores[1]);
                bool boton1 = valores[2] == "1";

                float xProcesado = MapearEjeConZonaMuerta(rawX, centroX);
                float yProcesado = MapearEjeConZonaMuerta(rawY, centroY);

                // DEBÚG 5: ¿Qué datos procesados le estamos mandando al Singleton?
                Debug.Log($"🚀 [DEBUG] Inyectando -> X: {xProcesado} | Y: {yProcesado} | Botón: {boton1}");

                InputManager.Instance.InyectarDatosHardware(xProcesado, yProcesado, boton1);
            }
            else
            {
                Debug.LogError($"⚠️ [DEBUG] Formato incorrecto. Se esperaban 4 datos y llegaron {valores.Length}. Texto: {datos}");
            }
        }
        catch (System.TimeoutException) 
        {
            Debug.LogWarning("⏰ [DEBUG] Timeout en la lectura del puerto.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("💥 [DEBUG] Error crítico en lectura: " + ex.Message);
        }
    }

}

    // Método matemático para transformar y estabilizar las lecturas físicas del potenciómetro
    float MapearEjeConZonaMuerta(float valorCrudo, float centro)
    {
        // Si está dentro del margen de ruido del centro, asumimos que está quieto (0)
        if (Mathf.Abs(valorCrudo - centro) <= zonaMuerta) return 0f;

        if (valorCrudo < centro)
        {
            // Mapea el tramo izquierdo: de [0 a centro-zonaMuerta] hacia [-1f a 0f]
            return Mathf.InverseLerp(0f, centro - zonaMuerta, valorCrudo) - 1f;
        }
        else
        {
            // Mapea el tramo derecho: de [centro+zonaMuerta a 1023] hacia [0f a 1f]
            return Mathf.InverseLerp(centro + zonaMuerta, 1023f, valorCrudo);
        }
    }

    void OnApplicationQuit()
    {
        if (stream != null && stream.IsOpen) 
        {
            stream.Close();
            Debug.Log("🔒 Puerto serial liberado correctamente.");
        }
    }
} 