using UnityEngine;
using UnityEngine.Assertions;


public class Starfield : MonoBehaviour
{
    [SerializeField] private int _maxStars = 100;
    [SerializeField] private float _starSize = 0.1f;
    [SerializeField] private float _starSizeRange = 0.5f;
    [SerializeField] private float _fieldWidth = 20f;
    [SerializeField] private float _fieldHeight = 11f;
    [SerializeField] private bool _colorize = false;
    [SerializeField] private float _speed = 0.5f;
     
    private float _xOffset;
    private float _yOffset;

    private Transform _bgCamera;

    private ParticleSystem _particles;
    private ParticleSystem.Particle[] _stars;


    void Awake()
    {
        _bgCamera = GameObject.FindWithTag("BG_Camera").transform;
        _stars = new ParticleSystem.Particle[_maxStars];
        _particles = GetComponent<ParticleSystem>();

        Assert.IsNotNull(_particles, "Particle system missing from object!");

        _xOffset = _fieldWidth * .5f;
        _yOffset = _fieldHeight * .5f;

        for (int i = 0; i < _maxStars; i++)
        {
            float randSize = Random.Range(_starSizeRange, _starSizeRange + 1f);
            float scaledColor = (true == _colorize) ? randSize - _starSizeRange : 1f;

            _stars[i].position = GetRandomInRectangle(_fieldWidth, _fieldHeight) + transform.position;
            _stars[i].startSize = _starSize * randSize;
            _stars[i].startColor = new Color(1f, scaledColor, scaledColor, 1f);
        }
        _particles.SetParticles(_stars, _stars.Length);
    }

    Vector3 GetRandomInRectangle(float width, float height)
    {
        float x = Random.Range(0, width);
        float y = Random.Range(0, height);
        return new Vector3(x - _xOffset, y - _yOffset, 0);
    }

    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        transform.position = new Vector3(transform.position.x - _speed * Time.deltaTime, transform.position.y, transform.position.z);

        for (int i = 0; i < _maxStars; i++)
        {
            Vector3 pos = _stars[i].position + transform.position;

            if (pos.x < (_bgCamera.position.x - _xOffset))
            {
                pos.x += _fieldWidth;
            }

            _stars[i].position = pos - transform.position;
        }
        _particles.SetParticles(_stars, _stars.Length);
    }
}