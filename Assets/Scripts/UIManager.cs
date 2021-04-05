using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private Image _livesImage;
    [SerializeField] private Sprite[] _livesSprite;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _restartText;
    [SerializeField] private Text _ammoText;

    void Start()
    {
        _scoreText.text = $"Score: {0}";
        _gameOverText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Player.onPlayerDeath += this.OnPlayerDeath;
    }

    private void OnDisable()
    {
        Player.onPlayerDeath -= this.OnPlayerDeath;
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = $"Score: {playerScore}";
    }

    public void UpdateLives(int livesRemaining)
    {
        _livesImage.sprite = _livesSprite[livesRemaining];
    }

    public void UpdateAmmo(int ammoCurrent, int ammoMax)
    {
        _ammoText.text = $"{ammoCurrent}/{ammoMax}";

        if (ammoCurrent == 0)
        {
            _ammoText.color = Color.red;
        }
        else
        {
            _ammoText.color = Color.white;
        }
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void OnPlayerDeath()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    public void OnRestart(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        SceneManager.LoadScene("Game");
    }

    public void OnQuit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Application.Quit();
    }
}
