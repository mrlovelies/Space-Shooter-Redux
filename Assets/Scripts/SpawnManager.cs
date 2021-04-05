using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private float _enemySpawnRate = 2.5f;
    private bool _isSpawningActive = false;

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private GameObject _powerUpContainer;
    [SerializeField] private GameObject _ammoPowerup;
    [SerializeField] private GameObject _healthPowerup;
    public void StartSpawning()
    {
        _isSpawningActive = true;
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnAmmoRoutine());
        StartCoroutine(SpawnHealthRoutine());
    }

    private void OnEnable()
    {
        Player.onPlayerDeath += this.OnPlayerDeath;
    }

    private void OnDisable()
    {
        Player.onPlayerDeath -= this.OnPlayerDeath;
    }

    public void OnPlayerDeath()
    {
        _isSpawningActive = false;
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3f);

        while (_isSpawningActive)
        {
            Vector3 posToSpawn = new Vector3(11f, Random.Range(-4f, 6f), 0);
            Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity, _enemyContainer.transform);
            yield return new WaitForSeconds(_enemySpawnRate);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(5f);

        while (_isSpawningActive)
        {
            float _nextSpawn = Random.Range(3f, 8f);
            int _nextPowerupID = Random.Range(0, _powerups.Length);
            Vector3 posToSpawn = new Vector3(11f, Random.Range(-4f, 6f), 0);
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
            Vector3 posToSpawn = new Vector3(11f, Random.Range(-4f, 6f), 0);
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
            Vector3 posToSpawn = new Vector3(11f, Random.Range(-4f, 6f), 0);
            Instantiate(_healthPowerup, posToSpawn, Quaternion.identity, _powerUpContainer.transform);
            yield return new WaitForSeconds(_nextSpawn);
        }
    }
}
