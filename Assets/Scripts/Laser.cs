using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    private bool _isEnemyLaser = false;
    private float _reverseDirection = 1;
    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        if (_isEnemyLaser) _reverseDirection = -1;
        
        transform.Translate((Vector3.right * (_speed * Time.deltaTime)) * _reverseDirection, Space.World);

        if (transform.position.x >= 15f || transform.position.x <= -15f)
        {
            Destroy(gameObject);
            
            if (transform.parent.CompareTag("Triple_Shot") || transform.parent.CompareTag("Enemy_Laser")) Destroy(transform.parent.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _isEnemyLaser)
        {
            Destroy(gameObject);

            Player player = other.GetComponent<Player>();
            if (player != null) player.Damage();
        }
    }
}
