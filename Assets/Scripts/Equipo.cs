using UnityEngine;
using System.Collections.Generic;
using System;

public class Equipo
{
    // Todas las unidades de este equipo
    public List<Unidad> unidades = new List<Unidad>();

    // Solo las unidades seleccionadas.
    // Usado solo por el jugador.
    public List<Unidad> unidadesSeleccionadas = new List<Unidad>();

    public Edificio edificioSeleccionado = null;

    public Edificio basePrincipal = null;

    public int maximoUnidades = 10;

    private int _oro;
    public int oro
    {
        get => _oro;
        set
        {
            if (_oro != value)
            {
                _oro = value;
                onOroChanged?.Invoke(_oro); // Dispara el evento cuando el oro cambia
            }
        }
    }
    public event Action<int> onOroChanged;


    /// <summary>
    /// Selecciona una unidad. Se agregara a la lista, se seteara como Seleccionada
    /// y activara su contorno
    /// </summary>
    /// <param name="unidadNueva">La unidad a agregarse</param>
    public void SeleccionaUnidad(Unidad unidadNueva)
    {
        /*
        if (unidadNueva == null)
        {
            Debug.LogWarning(unidadNueva.name + " intento seleccionarse pero es null");
            return;
        }
        // Si no teniamos la unidad, la agregamos
        if (!unidades.Contains(unidadNueva) && !unidadesSeleccionadas.Contains(unidadNueva))
        {
            unidadNueva.bEstaSeleccionada = true;
            // Activa el outline
            unidadNueva.outline.enabled = true;
            // Agrega la unidad a la lista
            unidades.Add(unidadNueva);
            unidadesSeleccionadas.Add(unidadNueva);
            return;
        }
        Debug.Log("La unidad ya estaba en la lista, asi que no se han realizado cambios.");
        */
        if (unidadNueva == null || unidadesSeleccionadas.Contains(unidadNueva)) return;

        unidadNueva.bEstaSeleccionada = true;
        unidadNueva.outline.enabled = true;
        unidadesSeleccionadas.Add(unidadNueva);
    }

    /// <summary>
    /// Deselecciona una unidad. La remueve de la lista unidadesSeleccionadas si se especifica.
    /// </summary>
    /// <param name="unidadDeseleccionar">La unidad a deseleccionar</param>
    /// <param name="bBorrarDeLista">Deberia borrarse de la lista unidadesSeleccionadas? True si solo borraras este elemento, False si borraras toda la lista</param>
    public void DeseleccionaUnidad(Unidad unidadDeseleccionar, bool bBorrarDeLista = true)
    {
        if (unidadDeseleccionar == null)
        {
            Debug.LogWarning("Se intentó deseleccionar una unidad pero es null");
            return;
        }

        if (!unidades.Contains(unidadDeseleccionar))
        {
            Debug.Log("La unidad NO está en la lista de unidades, así que no se han realizado cambios.");
            return;
        }

        // La deseleccionamos antes de borrarla de la lista seleccionada
        unidadDeseleccionar.bEstaSeleccionada = false;
        // Desactiva el outline
        unidadDeseleccionar.outline.enabled = false;

        if (bBorrarDeLista)
        {
            // En caso de que haya solo un elemento, es lo mismo que limpiar la lista
            if (unidadesSeleccionadas.Count <= 1)
            {
                unidadesSeleccionadas.Clear();
                return;
            }

            for (int i = 0; i < unidadesSeleccionadas.Count; i++)
            {
                // En caso de encontrar la unidad a remover
                if (unidadesSeleccionadas[i] != null && unidadesSeleccionadas[i] == unidadDeseleccionar)
                {
                    // OPTIMIZACION
                    // Intercambia el dato final (relevante) con el que queremos
                    // borrar. Esto para que al momento de borrar no se reorganice la lista
                    unidadesSeleccionadas[i] = unidadesSeleccionadas[unidadesSeleccionadas.Count - 1];
                    // Despues del cambio, borra el ultimo elemento
                    unidadesSeleccionadas.RemoveAt(unidadesSeleccionadas.Count - 1);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Deselecciona y vacia TODA la lista de unidades seleccionadas
    /// </summary>
    public void DeseleccionarUnidades()
    {
        /*
        if (unidadesSeleccionadas.Count < 1) return;
        // Desactiva el outline de los objetos
        foreach (Unidad unidadAct in unidadesSeleccionadas)
        {
            // Es false ya que borrar una por una en este caso
            // seria muy costoso y tardado. Es mejor el Clear().
            DeseleccionaUnidad(unidadAct, false);
        }
        // Limpia la lista de unidades seleccionadas
        unidadesSeleccionadas.Clear();
        */
        foreach (Unidad unidad in unidadesSeleccionadas)
        {
            unidad.bEstaSeleccionada = false;
            unidad.outline.enabled = false;
        }
        unidadesSeleccionadas.Clear();
    }
}
