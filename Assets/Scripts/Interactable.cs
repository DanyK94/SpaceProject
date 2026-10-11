using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual void Interact(){

        Debug.Log($"Interazione con: {gameObject.name}");

    }
    
}
