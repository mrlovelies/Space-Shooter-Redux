using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField] private int _lives = 3;
    [SerializeField] private float _speed = 6.5f;
    [SerializeField] private float _fireRate = 0.15f;
    [SerializeField] private bool _isTripleShotActive = false;
    [SerializeField] private float _tripleShotDuration = 3f;
    [SerializeField] private bool _isSpeedBoostActive = false;
    [SerializeField] private float _speedBoostMultiplier = 1.25f;
    [SerializeField] private float _thrusterMultiplier = 1.5f;
    [SerializeField] private float _speedBoostDuration = 5f;
    [SerializeField] private int _ammoMax = 15;
    [SerializeField] private int _ammoCurrent = 15;
    [SerializeField] private int _missilesMax = 3;
    [SerializeField] private int _missilesCurrent = 3;
    [SerializeField] private bool _isShieldActive = false;
    [SerializeField] private int _score = 0;
    [SerializeField] private int _shieldPower = 0;
    private float _canFire = -1f;
    private Vector3 _direction;
    [SerializeField] private ThrusterGauge _thrusterGauge;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _laserContainer;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _missilePrefab;
    [SerializeField] private GameObject _shieldVisualizer;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private Color[] _shieldPowerColor;
    private SpriteRenderer _shieldVisualizerSprite;
    [SerializeField] private GameObject[] _engineDamage;
    [SerializeField] private GameObject _thruster;
    [SerializeField] private AudioClip _laserSoundClip;
    [SerializeField] private AudioClip _ammoEmptySoundClip;
    private AudioSource _audioSource;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private InputManager _inputManager;

    public delegate void PlayerDeath();
    public static event PlayerDeath onPlayerDeath;
    private Controls.IPlayerActions _playerActionsImplementation;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.FindWithTag("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) Debug.LogError("SpawnManager::Player is NULL");

        _uiManager = GameObject.FindWithTag("UI_Manager").GetComponent<UIManager>();
        if (_uiManager == null) Debug.LogError("UIManager::Player is NULL");

        _inputManager = GameObject.FindWithTag("Input_Manager").GetComponent<InputManager>();
        if (_inputManager == null) Debug.LogError("InputManager::Player is NULL");

        _shieldVisualizerSprite = _shieldVisualizer.GetComponent<SpriteRenderer>();
        if (_shieldVisualizerSprite == null) Debug.LogError("Shield_Visualizer->SpriteRenderer::Player is NULL"); 

        _audioSource = GetComponent<AudioSource>();
        //Assert.IsNull(_audioSource, "_audioSource == null");
        if (_audioSource == null)
        {
            Debug.LogError("Null");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }

        _uiManager.UpdateAmmo(_ammoCurrent, _ammoMax);
        _uiManager.UpdateMissiles(_missilesCurrent, _missilesMax);
        
        
        _thrusterGauge = GameObject.FindWithTag("Thruster_Gauge").GetComponent<ThrusterGauge>();
        if (_thrusterGauge == null) Debug.LogError("ThrusterGauge::Player is NULL");
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        transform.Translate(_direction * (_speed * Time.deltaTime));

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -7.87f, 7.5f), transform.position.y, 0);
        
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
            _shieldPower--;

            if (_shieldPower < 1)
            {
                _isShieldActive = false;
                _shieldVisualizer.SetActive(false);
            }
            else
            {
                _shieldVisualizerSprite.color = _shieldPowerColor[_shieldPower - 1];
            }
            return;
        }


        CameraShake.Shake(.25f, .1f);
        _lives--;
        _uiManager.UpdateLives(_lives);

        int engineNum = Random.Range(0, 2);

        // TODO: Look into refactoring this
        if (!_engineDamage[engineNum].activeSelf)
        {
            _engineDamage[engineNum].SetActive(true);
        }
        else
        {
            if (engineNum == 0)
            {
                _engineDamage[1].SetActive(true);
            }
            else if (engineNum == 1)
            {
                _engineDamage[0].SetActive(true);
            }
        }

        if (_lives < 1)
        {
            _speed = 0;
            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, .5f);
            Destroy(explosion, 2.5f);
            onPlayerDeath?.Invoke();
            Destroy(gameObject);   
        }
    }

    public void AddHealth()
    {
        if (_lives > 2) return;

        _lives++;
        _uiManager.UpdateLives(_lives);

        foreach (GameObject engine in _engineDamage)
        {
            if (engine.activeSelf)
            {
                engine.SetActive(false);
                return;
            }
        }
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
        _shieldPower = 3;
        _shieldVisualizerSprite.color = _shieldPowerColor[_shieldPower - 1];
        _shieldVisualizer.SetActive(true);
        _isShieldActive = true;
    }

    public void AddAmmo()
    {
        _ammoCurrent += 10;
        if (_ammoCurrent > _ammoMax) _ammoCurrent = _ammoMax;
        _uiManager.UpdateAmmo(_ammoCurrent, _ammoMax);
    }

    public void AddMissile()
    {
        _missilesCurrent++;
        if (_missilesCurrent > _missilesMax) _missilesCurrent = _missilesMax;
        _uiManager.UpdateMissiles(_missilesCurrent, _missilesMax);
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

    public Transform GetClosestEnemy(GameObject[] enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach(GameObject potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }
     
        return bestTarget;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (Time.time > _canFire && _ammoCurrent > 0)
        {
            _audioSource.clip = _laserSoundClip;
            _audioSource.Play();

            if (_isTripleShotActive)
            {
                InstantiateLaser(_tripleShotPrefab);
            }
            else
            {
                InstantiateLaser(_laserPrefab);
            }

            _ammoCurrent--;
            _uiManager.UpdateAmmo(_ammoCurrent, _ammoMax);
            _canFire = Time.time + _fireRate;
        }
        else
        {
            _audioSource.volume = .5f;
            _audioSource.clip = _ammoEmptySoundClip;
            _audioSource.Play();
        }
    }
    
    public void ActivateThrusters()
    {
        _thruster.SetActive(true);
        _speed *= _thrusterMultiplier;
        _thrusterGauge.ActivateThrusters();
    }

    public void DeactivateThrusters()
    {
        _thruster.SetActive(false);
        _speed /= _thrusterMultiplier;
        _thrusterGauge.DeactivateThrusters();
    }

    public void OnThrusters(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ActivateThrusters();
        } else if (context.canceled)
        {
            DeactivateThrusters();
        }
    }

    public void OnFireMissile(InputAction.CallbackContext context)
    {
        if (!context.performed || _missilesCurrent < 1) return;

        GameObject[] _enemiesActive = GameObject.FindGameObjectsWithTag("Enemy");
        Transform target = GetClosestEnemy(_enemiesActive);

        GameObject missile = Instantiate(_missilePrefab, transform.position + new Vector3(0.2f, -.27f, 0), Quaternion.identity, _laserContainer.transform);
        _missilesCurrent--;
        _uiManager.UpdateMissiles(_missilesCurrent, _missilesMax);
        missile.GetComponent<HomingMissile>().MissileTarget(target);
    }
}
