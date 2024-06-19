using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interruptor : MonoBehaviour, IActivable
{

    public void Activate()
    {
        // Implementa la lógica de activación aquí
        Debug.Log("Object Activated!");
    }
}

