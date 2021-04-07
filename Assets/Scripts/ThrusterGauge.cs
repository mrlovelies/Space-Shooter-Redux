using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrusterGauge : MonoBehaviour
{
    
    private Slider _slider;
    private Player _player;
    [SerializeField] private float _thrusterConsumption = 30f;
    [SerializeField] private float _maxThrusters = 100;
    [SerializeField] private float _currentThrusters;
    [SerializeField] private bool _isThrusterActive = false;
    [SerializeField] private float _thrusterRechargeCooldown = 4f;
    private bool _rechargeActive = false;

    private void Start()
    {
        _slider = GetComponent<Slider>();
        if (_slider == null) Debug.LogError("Slider::ThrusterGauge is NULL");

        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (_player == null) Debug.LogError("Player::ThrusterGauge is NULL");
        
        _currentThrusters = _maxThrusters;
    }

    private void Update()
    {
        if (_isThrusterActive)
        {
            _slider.value += -_thrusterConsumption * Time.deltaTime;
            _currentThrusters += -_thrusterConsumption * Time.deltaTime;
        }
        if (_rechargeActive)
        {
            _slider.value += _thrusterConsumption * Time.deltaTime;
            _currentThrusters += _thrusterConsumption * Time.deltaTime;
        }

        CheckThrusters();
    }
    
    private void CheckThrusters()
    {
        if (_currentThrusters <= 0)
        {
            _currentThrusters = 0;
            DeactivateThrusters();
            _player.DeactivateThrusters();
        } else if (_currentThrusters >= _maxThrusters)
        {
            _rechargeActive = false;
            _currentThrusters = _maxThrusters;
        }
    }

    public void ActivateThrusters()
    {
        StopCoroutine(ThrusterRechargeRoutine());
        _rechargeActive = false;
        _isThrusterActive = true;
    }

    public void DeactivateThrusters()
    {
        _isThrusterActive = false;
        StartCoroutine(ThrusterRechargeRoutine());
    }

    IEnumerator ThrusterRechargeRoutine()
    {
        yield return new WaitForSeconds(_thrusterRechargeCooldown);

        _rechargeActive = true;
    }
}
