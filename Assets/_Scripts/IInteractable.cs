using UnityEngine;

// Esto no es un script que pones en un objeto, es una "regla" que otros scripts siguen.
public interface IInteractable
{
    // CUALQUIER script que siga esta regla DEBE tener esta función.
    void Interactuar(PlayerController player);
}
