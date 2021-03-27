using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour, Controls.IPlayerActions
{
    [SerializeField] private int _lives = 3;
    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private float _fireRate = 0.15f;
    [SerializeField] private bool _isTripleShotActive = false;
    [SerializeField] private float _tripleShotDuration = 3f;
    [SerializeField] private bool _isSpeedBoostActive = false;
    [SerializeField] private float _speedBoostMultiplier = 1.25f;
    [SerializeField] private float _speedBoostDuration = 5f;
    [SerializeField] private bool _isShieldActive = false;
    [SerializeField] private int _score = 0;
    private float _canFire = -1f;
    private Vector3 _direction;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _laserContainer;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _shieldVisualizer;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    private PlayerInput _playerInput;
    private Controls.IPlayerActions _playerActionsImplementation;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.FindWithTag("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) Debug.LogError("SpawnManager::Player is NULL");
        _uiManager = GameObject.FindWithTag("UI_Manager").GetComponent<UIManager>();
        if (_uiManager == null) Debug.LogError("UIManager::Player is NULL");
        _playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        transform.Translate(_direction * (_speed * Time.deltaTime));

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -9f, 7.5f), transform.position.y, 0);
        
        if (transform.position.y >= 7.5)
            transform.position = new Vector3(transform.position.x, -5.5f, 0);
        else if (transform.position.y <= -5.5f)
            transform.position = new Vector3(transform.position.x, 7.5f, 0);
    }

    private void InstantiateLaser(GameObject _laserTypePrefab)
    {
        Instantiate(_laserTypePrefab, transform.position + new Vector3(.75f,0,0), Quaternion.identity, _laserContainer.transform);
    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
            return;
        }
        
        _lives--;
        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            _uiManager.OnPlayerDeath();
            OnPlayerDeath();
            gameObject.SetActive(false);
            //Destroy(gameObject);   
        }
    }

    public void OnPlayerDeath()
    {
        _playerInput.currentActionMap = _playerInput.actions.FindActionMap("Death Menu");
    }

    public void ActivateTripleShot()
    {
        _isTripleShotActive = true;
        StartCoroutine(DeactivateTripleShotRoutine());
    }

    public void ActivateSpeedBoost()
    {
        if (!_isSpeedBoostActive) _speed *= _speedBoostMultiplier;
        _isSpeedBoostActive = true;
        StartCoroutine(DeactivateSpeedBoostRoutine());
    }

    public void ActivateShield()
    {
        _shieldVisualizer.SetActive(true);
        _isShieldActive = true;
    }

    public void EnemyKill()
    {
        int pointsToAdd = Random.Range(10, 21);
        AddScore(pointsToAdd);
    }

    public void AddScore(int score)
    {
        _score += score;
        _uiManager.UpdateScore(_score);
    }

    IEnumerator DeactivateTripleShotRoutine()
    {
        yield return new WaitForSeconds(_tripleShotDuration);
        _isTripleShotActive = false;
    }

    IEnumerator DeactivateSpeedBoostRoutine()
    {
        yield return new WaitForSeconds(_speedBoostDuration);
        if (_isSpeedBoostActive) _speed /= _speedBoostMultiplier;
        _isSpeedBoostActive = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (Time.time > _canFire)
        {
            if (_isTripleShotActive)
            {
                InstantiateLaser(_tripleShotPrefab);
            }
            else
            {
                InstantiateLaser(_laserPrefab);
            }
            
            _canFire = Time.time + _fireRate;
        }
    }
}
