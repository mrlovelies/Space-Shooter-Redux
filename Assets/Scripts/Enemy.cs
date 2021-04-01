using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _fireRate = 3f;
    private bool _isEnemyDead = false;
    private float _canFire = -1f;
    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _laserContainer;

    void Start()
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

    void Update()
    {
        CalculateMovement();
        FireLasers();
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.left * (_speed * Time.deltaTime));
        if (transform.position.x < -11f)
        {
            float randomY = Random.Range(-4f, 6f);
            transform.position = new Vector3(11f, randomY, 0);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isEnemyDead = true;
            if (_player != null)
                _player.Damage();

            _anim.SetTrigger("OnEnemyDeath");
            _speed = .5f;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, 2.8f);
        }

        if (other.CompareTag("Laser"))
        {
            _isEnemyDead = true;
            Destroy(other.gameObject);
            _anim.SetTrigger("OnEnemyDeath");
            _speed = .5f;
            _audioSource.Play();
            if (_player != null) _player.EnemyKill();
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, 2.8f);
        }
    }
}
