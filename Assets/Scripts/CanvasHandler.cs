using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasHandler : MonoBehaviour
{
    [SerializeField] GameObject _menuDefeat;

    PlayerHealth _playerHealth;

    private void OnEnable()
    {
        _playerHealth = GetComponent<PlayerHealth>();
        _playerHealth.OnDead += ActiveLoseMenu;
    }

    public void ActiveLoseMenu()
    {
        _menuDefeat.SetActive(true);
    }

    private void OnDisable()
    {
        _playerHealth.OnDead -= ActiveLoseMenu;
    }
}
