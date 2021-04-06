using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : MonoBehaviour
{
    private Player _player;
    private Transform _target;
    [SerializeField] private float _timeToExplode = 10f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _rotateSpeed = 200f;
    [SerializeField] private GameObject _missileExplosion;
    private IEnumerator _timedExplosion;
    private Rigidbody2D _rb;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (_player == null) Debug.LogError("Player::HomingMissile is NULL");

        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null) Debug.LogError("Rigidbody2D::HomingMissile is NULL");

        _timedExplosion = MissileExplodeRoutine();
        StartCoroutine(_timedExplosion);
    }

    private void FixedUpdate()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        if (_target)
        {
            Vector2 direction = (Vector2)_target.position - _rb.position;

            direction.Normalize();
            float rotateAmt = Vector3.Cross(direction, transform.right).z;
            _rb.angularVelocity = -rotateAmt * _rotateSpeed;
        }
        _rb.velocity = transform.right * _speed;
    }

    public void MissileTarget(Transform target)
    {
        _target = target;
    }

    IEnumerator MissileExplodeRoutine()
    {
        yield return new WaitForSeconds(_timeToExplode);
        _speed = 0f;
        GameObject explosion = Instantiate(_missileExplosion, transform.position, Quaternion.identity);
        Destroy(explosion, 2.5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StopCoroutine(_timedExplosion);
            _speed = 0f;
            GameObject explosion = Instantiate(_missileExplosion, transform.position, Quaternion.identity);
            Destroy(explosion, 2.5f);
            Destroy(gameObject);
        }
    }
}
