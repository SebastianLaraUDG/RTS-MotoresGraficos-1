using UnityEngine;
using UnityEngine.InputSystem;

public class ControlCamara : MonoBehaviour
{
    private InputAction movimientoAction;
    private InputAction rotacionAction;

    private Transform yaw;

    [SerializeField] private float velocidadRotacion = 1f;
    [SerializeField] private float velocidadMovimiento = 1f;

    // Limites del mapa
    [SerializeField] private Vector2 limiteMin = new Vector2(-50, -50);
    [SerializeField] private Vector2 limiteMax = new Vector2(50, 50);

    private void Awake()
    {
        // Asigna acciones de gameplay
        movimientoAction = InputSystem.actions.FindAction("Movimiento");
        rotacionAction = InputSystem.actions.FindAction("Rotacion");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Busca el yaw
        yaw = transform.Find("Yaw");
    }

    // Update is called once per frame
    void Update()
    {
        // Solo hacer cambio si se detectan inputs

        // Movimiento
        if (Input.anyKey)
        {
            Vector2 movimiento = movimientoAction.ReadValue<Vector2>();
//            print("Valores de Movimiento del "+ this + ": " + movimiento);

            // Movimiento adelante/atras lado/lado dependiendo de la rotacion
            Vector3 movimientoRotado = yaw.rotation * new Vector3(movimiento.x, 0f, movimiento.y);
            transform.Translate(Time.deltaTime * velocidadMovimiento * movimientoRotado);

            // Aplicar limites a la posicion de la cámara
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, limiteMin.x, limiteMax.x);
            pos.z = Mathf.Clamp(pos.z, limiteMin.y, limiteMax.y);
            transform.position = pos;
        }

        // Rotacion
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            float cambioRotacion = rotacionAction.ReadValue<float>();
//            print("Rotacion: " + cambioRotacion);
            yaw.Rotate(0f, cambioRotacion * velocidadRotacion * Time.deltaTime, 0f);
        }

    }
}
