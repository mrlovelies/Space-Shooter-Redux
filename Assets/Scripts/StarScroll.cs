using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarScroll : MonoBehaviour
{
    [SerializeField] private float _speed = 0.5f;
    

    void Start()
    {
        
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x - _speed * Time.deltaTime, transform.position.y, transform.position.z);
    }
}