using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public abstract class Enemy : MonoBehaviour
{
    public float _speed = 5f;
    public int _health = 1;
    [SerializeField] private float _fireRate = 3f;
    private bool _isEnemyDead = false;
    private float _canFire = -1f;
    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    [SerializeField] private GameObject _laserPrefab;
    private GameObject _laserContainer;

    protected virtual void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (_player == null) Debug.LogError("Player::Enemy is NULL");

        _anim = GetComponentInChildren<Animator>();
        if (_anim == null) Debug.LogError("Animator::Enemy is NULL");

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) Debug.LogError("AudioSource::Enemy is NULL");

        _laserContainer = GameObject.FindWithTag("Laser_Container");
        if (_laserContainer == null) Debug.LogError("Laser_Container::Enemy is NULL");

    }

    protected virtual void FixedUpdate()
    {
        CalculateMovement();
        FireLasers();
    }

    protected virtual void CalculateMovement()
    {
        transform.Translate(Vector3.left * (_speed * Time.deltaTime));
        if (transform.position.x < -9.3f)
        {
            float randomY = Random.Range(-4.5f, 4.5f);
            transform.position = new Vector3(9.3f, randomY, 0);
        }
    }

    void FireLasers()
    {
        if (Time.time > _canFire && !_isEnemyDead)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity, _laserContainer.transform);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++) lasers[i].AssignEnemyLaser();
        }
    }

    //TODO: Give weapon types a damage var
    private void Damage(int damageDealt)
    {
        _health -= damageDealt;

        if (_health < 1)
        {
            _isEnemyDead = true;
            _anim.SetTrigger("OnEnemyDeath");
            _speed = .5f;
            _audioSource.Play();
            if (_player != null) _player.EnemyKill();
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, .50f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_player != null)
                _player.Damage();

            Damage(1);
        }

        if (other.CompareTag("Laser"))
        {
            bool isEnemyLaser = other.GetComponent<Laser>()._isEnemyLaser;
            if (!isEnemyLaser)
            {
                Destroy(other.gameObject);
                Damage(1);
            }
            
        }
        
        if (other.CompareTag("Missile_Explosion"))
        {
            Damage(2);
        }
    }
}
