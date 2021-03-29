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
        Player.onPlayerDeath += this.OnPlayerDeath;
    }

    private void OnDisable()
    {
        Player.onPlayerDeath -= this.OnPlayerDeath;
    }

    public void OnPlayerDeath()
    {
        _playerInput.currentActionMap = _playerInput.actions.FindActionMap("Death Menu");
    }
}
