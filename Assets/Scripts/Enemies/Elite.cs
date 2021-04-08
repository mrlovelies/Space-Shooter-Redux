using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elite : Enemy
{
    public float frequency = 5.0f;  // Speed of sine movement
    public float magnitude = 0.5f;   // Size of sine movement
    private Vector3 axis;

    private Vector3 pos;

    protected override void Start()
    {
        base.Start();
        pos = transform.position;
        axis = transform.up;
    }

    protected override void CalculateMovement()
    {
        
        if (transform.position.x < -9.3f)
        {
            float randomY = Random.Range(-4.5f, 4.5f);
            pos = new Vector3(9.3f, randomY, 0);
            transform.position = pos;
        }
        else
        {
            pos -= transform.right * (Time.deltaTime * _speed);
            transform.position = pos + axis * (Mathf.Sin (Time.time * frequency) * magnitude);
        }
    }
}
