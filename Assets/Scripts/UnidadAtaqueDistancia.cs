using UnityEngine;

public class UnidadAtaqueDistancia : Unidad
{
    [Header("Ataque a distancia")]
    [SerializeField] private GameObject proyectilPrefab; // Prefab del proyectil
    [SerializeField] private Transform firePoint; // Punto de disparo
    [SerializeField] private float rangoAtaque = 10f; // Rango máximo de ataque a distancia

    protected override void Awake()
    {
        base.Awake();
        tiempoTranscurridoUltimoAtaque = tiempoEntreAtaques;
    }


    protected override void Update()
    {
        base.Update();

        // Si no hay objetivo, no hacemos nada
        if (entidadObjetivoActual == null)
        {
            return;
        }

        // Calcular la distancia al objetivo
        float dist = Vector3.Distance(transform.position, entidadObjetivoActual.transform.position);

        // Verificamos si estamos dentro del rango de ataque
        if (dist <= rangoAtaque)
        {
            // Si estamos dentro del rango, dejamos de movernos
            navMeshAgent.isStopped = true;

            // Aseguramos que la unidad apunte al objetivo
            transform.LookAt(new Vector3(entidadObjetivoActual.transform.position.x, transform.position.y, entidadObjetivoActual.transform.position.z));

            // Si podemos atacar (y el tiempo de cooldown ha pasado)
            if (tiempoTranscurridoUltimoAtaque >= tiempoEntreAtaques)
            {
                Atacar();
                tiempoTranscurridoUltimoAtaque = 0f; // Reiniciar el tiempo de cooldown
            }
        }
        else
        {
            // Si no estamos dentro del rango de ataque, seguimos moviéndonos hacia el objetivo
            navMeshAgent.isStopped = false;
            posObjetivo = entidadObjetivoActual.transform.position;
        }

        // Actualizamos el tiempo de cooldown entre ataques
        if (tiempoTranscurridoUltimoAtaque < tiempoEntreAtaques)
        {
            tiempoTranscurridoUltimoAtaque += Time.deltaTime;
        }
    }

    // Método para realizar el ataque
    private void Atacar()
    {
        if (proyectilPrefab != null && firePoint != null)
        {
            // Crear un proyectil en el punto de disparo
            GameObject proyectil = Instantiate(proyectilPrefab, firePoint.position, firePoint.rotation);

            // Configurar el proyectil con el objetivo (por ejemplo, usando un script en el proyectil para dirigirlo)
            var projectileScript = proyectil.GetComponent<Proyectil>();
            if (projectileScript != null)
            {
                projectileScript.SetTarget(entidadObjetivoActual.transform);
            }
        }
    }
}