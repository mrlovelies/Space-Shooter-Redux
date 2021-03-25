using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;

    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.right * (_speed * Time.deltaTime), Space.World);

        if (transform.position.x >= 15f)
        {
            Destroy(gameObject);
            
            if (transform.parent.CompareTag("Triple_Shot")) Destroy(transform.parent.gameObject);
        }
    }
}
