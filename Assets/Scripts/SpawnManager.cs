using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private float _enemySpawnRate = 2.5f;
    private bool _isSpawningActive = true;

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private GameObject _powerUpContainer;
    
    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    public void OnPlayerDeath()
    {
        _isSpawningActive = false;
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (_isSpawningActive)
        {
            Vector3 posToSpawn = new Vector3(11f, Random.Range(-4f, 6f), 0);
            Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity, _enemyContainer.transform);
            yield return new WaitForSeconds(_enemySpawnRate);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (_isSpawningActive)
        {
            float _nextSpawn = Random.Range(3f, 8f);
            int _nextPowerupID = Random.Range(0, 3);
            Vector3 posToSpawn = new Vector3(11f, Random.Range(-4f, 6f), 0);
            Instantiate(_powerups[_nextPowerupID], posToSpawn, Quaternion.identity, _powerUpContainer.transform);
            yield return new WaitForSeconds(_nextSpawn);
        }
    }
}
