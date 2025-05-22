using UnityEngine;

public class UnidadMelee : Unidad
{
    protected float tiempoTanscurridoUltimoAtaque = 0f;

    private Collider objetivoActualEnRango;


    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    /*
    protected override void Update()
    {
        base.Update();
        tiempoTranscurridoUltimoAtaque += Time.deltaTime;

        if (objetivoActualEnRango != null && tiempoTranscurridoUltimoAtaque >= tiempoEntreAtaques)
        {
            // Verificar si es una Unidad
            Unidad unidad = objetivoActualEnRango.GetComponent<Unidad>();
            if (unidad != null)
            {
                unidad.RecibeDanio(danio);

                if (unidad.salud <= 0f)
                {
                    if (unidad == ultimoEnemigoAtacado)
                        ultimoEnemigoAtacado = null;

                    if (unidad == entidadObjetivoActual)
                        entidadObjetivoActual = null;

                    objetivoActualEnRango = null;
                }

                tiempoTranscurridoUltimoAtaque = 0f;
                return;
            }

            // Verificar si es un Edificio
            Edificio edificio = objetivoActualEnRango.GetComponent<Edificio>();
            if (edificio != null)
            {
                edificio.RecibirDanio(danio);

                if (edificio.salud <= 0f)
                {
                    if (edificio == ultimoEdificioAtacado)
                        ultimoEdificioAtacado = null;

                    if (edificio == entidadObjetivoActual)
                        entidadObjetivoActual = null;

                    objetivoActualEnRango = null;
                }

                tiempoTranscurridoUltimoAtaque = 0f;
            }
        }
    }
    */
    protected override void Update()
    {
        base.Update();
        tiempoTranscurridoUltimoAtaque += Time.deltaTime;

        if (objetivoActualEnRango != null && tiempoTranscurridoUltimoAtaque >= tiempoEntreAtaques)
        {
            // Validar que el objetivo sigue siendo válido
            GameObject objetivoGO = objetivoActualEnRango.gameObject;

            // Si esta unidad es de la IA
            if (controladoPorIA)
            {
                // Ataca unidades del jugador (tag: "Unidad")
                if (objetivoGO.CompareTag("Unidad"))
                {
                    Unidad unidad = objetivoGO.GetComponent<Unidad>();
                    if (unidad != null)
                    {
                        unidad.RecibeDanio(danio);
                        RevisarMuerte(unidad);
                    }
                }
                // Ataca edificios del jugador (tag: "Edificio")
                else if (objetivoGO.CompareTag("Edificio"))
                {
                    Edificio edificio = objetivoGO.GetComponent<Edificio>();
                    if (edificio != null)
                    {
                        edificio.RecibirDanio(danio);
                        RevisarMuerte(edificio);
                    }
                }
            }
            else // Esta unidad es del jugador
            {
                // Ataca cualquier entidad enemiga (tag: "EntidadEnemiga")
                if (objetivoGO.CompareTag("Entidad Enemiga"))
                {
                    Unidad unidad = objetivoGO.GetComponent<Unidad>();
                    if (unidad != null)
                    {
                        unidad.RecibeDanio(danio);
                        RevisarMuerte(unidad);
                    }
                    else
                    {
                        Edificio edificio = objetivoGO.GetComponent<Edificio>();
                        if (edificio != null)
                        {
                            edificio.RecibirDanio(danio);
                            RevisarMuerte(edificio);
                        }
                    }
                }
            }

            tiempoTranscurridoUltimoAtaque = 0f;
        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (objetivoActualEnRango == null && other.CompareTag("Entidad Enemiga"))
        {
            // Solo tomamos un objetivo a la vez
            if (other.GetComponent<Unidad>() != null || other.GetComponent<Edificio>() != null)
            {
                objetivoActualEnRango = other;
            }
        }

        // Nuevo: Daño al colisionar con una unidad enemiga
        Unidad otraUnidad = other.GetComponent<Unidad>();
        if (otraUnidad != null && otraUnidad.equipoDuenio != this.equipoDuenio)
        {
            otraUnidad.RecibeDanio(danio);

            if (otraUnidad.salud <= 0f)
            {
                if (otraUnidad == ultimoEnemigoAtacado)
                    ultimoEnemigoAtacado = null;

                if (otraUnidad == entidadObjetivoActual)
                    entidadObjetivoActual = null;

                if (otraUnidad.gameObject == objetivoActualEnRango?.gameObject)
                    objetivoActualEnRango = null;
            }
        }
        
    }
    */
    private void OnTriggerEnter(Collider other)
    {
        if (objetivoActualEnRango != null) return;

        GameObject obj = other.gameObject;

        if (controladoPorIA)
        {
            // IA busca objetivos del jugador
            if (obj.CompareTag("Unidad") || obj.CompareTag("Edificio"))
            {
                objetivoActualEnRango = other;
            }
        }
        else
        {
            // Jugador busca objetivos enemigos
            if (obj.CompareTag("Entidad Enemiga"))
            {
                objetivoActualEnRango = other;
            }
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other == objetivoActualEnRango)
        {
            objetivoActualEnRango = null;
        }

        if (other.GetComponent<Unidad>() == entidadObjetivoActual || other.GetComponent<Edificio>() == entidadObjetivoActual)
        {
            entidadObjetivoActual = null;
        }
    }


    private void RevisarMuerte(Unidad unidad)
    {
        if (unidad.salud <= 0f)
        {
            if (unidad == ultimoEnemigoAtacado)
                ultimoEnemigoAtacado = null;

            if (unidad == entidadObjetivoActual)
                entidadObjetivoActual = null;

            if (unidad.gameObject == objetivoActualEnRango?.gameObject)
                objetivoActualEnRango = null;
        }
    }

    private void RevisarMuerte(Edificio edificio)
    {
        if (edificio.salud <= 0f)
        {
            if (edificio == ultimoEdificioAtacado)
                ultimoEdificioAtacado = null;

            if (edificio == entidadObjetivoActual)
                entidadObjetivoActual = null;

            if (edificio.gameObject == objetivoActualEnRango?.gameObject)
                objetivoActualEnRango = null;
        }
    }
}
