using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Controls.IPlayerActions
{
    [SerializeField] private float _speed = 5.0f;
    private Vector3 _direction;
    
    private Controls.IPlayerActions _playerActionsImplementation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * (_direction.x * _speed * Time.deltaTime));
        transform.Translate(Vector3.up * (_direction.y * _speed * Time.deltaTime));
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>();
    }
}
