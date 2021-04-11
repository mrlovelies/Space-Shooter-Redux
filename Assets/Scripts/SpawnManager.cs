using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    private bool _isSpawningActive = false;

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private GameObject _powerUpContainer;
    [SerializeField] private GameObject _ammoPowerup;
    [SerializeField] private GameObject _healthPowerup;
    [SerializeField] private GameObject[] _enemyTypes;
    private WaveCreator _waveCreator;

    private void Start()
    {
        _waveCreator = GetComponent<WaveCreator>();
        if (_waveCreator == null) Debug.LogError("WaveCreator::SpawnManager is NULL");
    }

    public void StartSpawning()
    {
        _isSpawningActive = true;
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnAmmoRoutine());
        StartCoroutine(SpawnHealthRoutine());
        _waveCreator.StartWave(1);
    }

    private void OnEnable()
    {
        Player.onPlayerDeath += this.OnPlayerDeath;
    }

    private void OnDisable()
    {
        Player.onPlayerDeath -= this.OnPlayerDeath;
    }

    public void StopSpawning()
    {
        _isSpawningActive = false;
    }

    public void OnPlayerDeath()
    {
        StopSpawning();
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(5f);

        while (_isSpawningActive)
        {
            float _nextSpawn = Random.Range(3f, 8f);
            int _nextPowerupID = Random.Range(0, _powerups.Length);
            Vector3 posToSpawn = new Vector3(11f, Random.Range(-4.5f, 4.5f), 0);
            Instantiate(_powerups[_nextPowerupID], posToSpawn, Quaternion.identity, _powerUpContainer.transform);
            yield return new WaitForSeconds(_nextSpawn);
        }
    }

    IEnumerator SpawnAmmoRoutine()
    {
        yield return new WaitForSeconds(4f);

        while (_isSpawningActive)
        {
            float _nextSpawn = Random.Range(3f, 6f);
            Vector3 posToSpawn = new Vector3(11f, Random.Range(-4.5f, 4.5f), 0);
            Instantiate(_ammoPowerup, posToSpawn, Quaternion.identity, _powerUpContainer.transform);
            yield return new WaitForSeconds(_nextSpawn);
        }
    }

    IEnumerator SpawnHealthRoutine()
    {
        yield return new WaitForSeconds(20f);

        while (_isSpawningActive)
        {
            float _nextSpawn = Random.Range(14f, 20f);
            Vector3 posToSpawn = new Vector3(11f, Random.Range(-4.5f, 4.5f), 0);
            Instantiate(_healthPowerup, posToSpawn, Quaternion.identity, _powerUpContainer.transform);
            yield return new WaitForSeconds(_nextSpawn);
        }
    }
}
