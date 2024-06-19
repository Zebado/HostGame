using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interruptor : MonoBehaviour, IActivable
{
    [SerializeField] private Door door;
    [SerializeField] private Sprite interruptorActivated;

    public void Activate()
    {
        // Implementa la lógica de activación aquí
        door.ChangeToActive();
        GetComponent<SpriteRenderer>().sprite = interruptorActivated;
        Debug.Log("Object Activated!");
    }
}

