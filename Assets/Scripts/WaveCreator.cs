using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public class WaveAction
{
    public string name;
    public float spawnDelay;
    public Transform prefab;
    public int spawnCount;
}
 
[System.Serializable]
public class Wave
{
    public string name;
    public int waveDelay = 5;
    public string waveMessage;
    public List<WaveAction> actions;
}
public class WaveCreator : MonoBehaviour
{
    private Player _player;
    private UIManager _uiManager;
    public List<Wave> waves;
    private Wave currentWave;
    public Wave CurrentWave { get {return currentWave;} }
    private float delayFactor = 1.0f;
    private int _enemiesDestroyedInWave = 0;
    private Text _waveText;
    private Text _waveCountdownText;
    public int enemiesInWave;
    
    [SerializeField] private GameObject _enemyContainer;

    private void Start()
    {
        _waveText = GameObject.Find("Wave_text").GetComponent<Text>();
        _waveCountdownText = GameObject.Find("WaveCountdown_text").GetComponent<Text>();
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _uiManager = GameObject.FindWithTag("UI_Manager").GetComponent<UIManager>();
    }

    public int EnemiesInWave
    {
        get { return enemiesInWave; }
        set { enemiesInWave = value; }
    }

    IEnumerator SpawnWaveRoutine(int waveNumber)
    {
        Wave W = waves[waveNumber];
        EnemiesInWave = 0;
        foreach (WaveAction A in W.actions)
        {
            EnemiesInWave += A.spawnCount;
        }

        if (W.waveMessage != "")
        {
            _waveText.enabled = true;
            _waveCountdownText.enabled = true;
            _waveText.text = W.waveMessage;
            StartCoroutine(WaveCountdownRoutine(W.waveDelay));
            yield return new WaitForSeconds(W.waveDelay);
            _waveText.enabled = false;
            _waveCountdownText.enabled = false;
        }
        
        currentWave = W;
        foreach(WaveAction A in W.actions)
        {
            if(A.spawnDelay > 0)
                yield return new WaitForSeconds(A.spawnDelay * delayFactor);
            
            if (A.prefab != null && A.spawnCount > 0)
            {
                for(int i = 0; i < A.spawnCount; i++)
                {
                    yield return new WaitForSeconds(A.spawnDelay);
                    Vector3 posToSpawn = new Vector3(11f, Random.Range(-4.5f, 4.5f), 0);
                    Instantiate(A.prefab, posToSpawn, Quaternion.identity, _enemyContainer.transform);
                }
            }

        }
        yield return null;  // prevents crash if all delays are 0
    }

    IEnumerator WaveCountdownRoutine(int secondsToStart)
    {
        for (int i = secondsToStart; i > 0; i--)
        {
            _waveCountdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
    }

    public void StartWave(int waveNumber)
    {
        int waveIndexToLoad = waveNumber - 1;
        Debug.Log($"Wave Index to load: {waveIndexToLoad}");
        StartCoroutine(SpawnWaveRoutine(waveIndexToLoad));
    }

}