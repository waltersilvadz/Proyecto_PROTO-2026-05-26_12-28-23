using UnityEngine;

public class ZonaEntrega : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Premio"))
        {
            ClawController controlador = FindObjectOfType<ClawController>();
            if (controlador != null)
            {
                controlador.RegistrarEntregaExitosa();
            }
        }
    }
}