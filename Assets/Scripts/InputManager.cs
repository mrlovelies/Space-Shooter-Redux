using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput _playerInput;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        Player.onPlayerDeath += this.UseRestartMenu;
    }

    private void OnDisable()
    {
        Player.onPlayerDeath -= this.UseRestartMenu;
    }

    public void UseRestartMenu()
    {
        _playerInput.currentActionMap = _playerInput.actions.FindActionMap("Restart Screen");
    }
}
