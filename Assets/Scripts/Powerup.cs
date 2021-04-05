using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Powerup : MonoBehaviour
{
    
    [SerializeField] private int _powerupID; // 0: Triple Shot | 1: Speed Boost | 2: Shields
    [SerializeField] private float _speed = 5f;
    [SerializeField] private AudioClip _audioClip;
    void Start()
    {
    }

    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.left * (_speed * Time.deltaTime));
        
        if (transform.position.x < -11f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player == null) Debug.LogError("Powerup::Player is NULL");
         
            switch (_powerupID)
            {
                case 0:
                    player.ActivateTripleShot();
                    break;
                case 1:
                    player.ActivateSpeedBoost();
                    break;
                case 2:
                    player.ActivateShield();
                    break;
                case 3:
                    player.AddAmmo();
                    break;
                default:
                    break;
            }
            
            AudioSource.PlayClipAtPoint(_audioClip, 0.9f * Camera.main.transform.position + 0.1f * transform.position, 1f);
            Destroy(gameObject);
        }
    }
}
