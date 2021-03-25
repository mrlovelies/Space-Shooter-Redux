using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Controls.IPlayerActions
{
    [SerializeField] private int _lives = 3;
    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private float _fireRate = 0.15f;
    [SerializeField] private bool _isTripleShotActive = false;
    [SerializeField] private float _tripleShotDuration = 3f;
    private float _canFire = -1f;
    private Vector3 _direction;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _laserContainer;
    [SerializeField] private GameObject _tripleShotPrefab;
    private SpawnManager _spawnManager;
    
    private Controls.IPlayerActions _playerActionsImplementation;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.FindWithTag("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) Debug.LogError("SpawnManager::Player is NULL");
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        transform.Translate(_direction * (_speed * Time.deltaTime));

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -9f, 0f), transform.position.y, 0);
        
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
        _lives--;

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(gameObject);   
        }
    }

    public void ActivateTripleShot()
    {
        _isTripleShotActive = true;
        StartCoroutine(DeactivateTripleShot());
    }

    IEnumerator DeactivateTripleShot()
    {
        yield return new WaitForSeconds(_tripleShotDuration);
        _isTripleShotActive = false;
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
