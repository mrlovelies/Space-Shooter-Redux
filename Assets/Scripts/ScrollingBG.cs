using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBG : MonoBehaviour
{
    [SerializeField] private float _speed = .1f;
    private Renderer _renderer;
    
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null) Debug.LogError("Renderer::ScrollingBG is NULL");
    }
    
    void Update()
    {
        _renderer.material.mainTextureOffset = new Vector2(Time.time * _speed, 0);
    }
}
