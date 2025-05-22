using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;

    private Transform target;

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Mover el proyectil hacia el objetivo
        Vector3 direction = (target.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        transform.LookAt(target);
    }

    void OnTriggerEnter(Collider other)
    {
        // Detectar si el objeto con el tag "Entidad Enemiga" colisiona con el proyectil
        if (other.CompareTag("Entidad Enemiga"))
        {
            Debug.LogWarning(gameObject.name + " ha impactado a " + other.gameObject.name);
            // Aquí aplicas el daño o cualquier otra acción que quieras
            // Por ejemplo, si el enemigo tiene un script de salud:
            // other.GetComponent<Enemy>().TakeDamage(damage);
            Unidad unidadEnemiga = other.GetComponent<Unidad>();
            if (unidadEnemiga)
            {
                unidadEnemiga.RecibeDanio(damage);
                HitTarget();
                return;
            }

            Edificio edificioEnemigo = other.GetComponent<Edificio>();
            if(edificioEnemigo)
            {
                edificioEnemigo.RecibirDanio(damage);
                HitTarget();
                return;
            }
        }
    }

    void HitTarget()
    {
        // Puedes agregar efectos como partículas o sonido aquí
        Destroy(gameObject);  // Destruir el proyectil después del impacto
    }
}