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
    [SerializeField] private Text _gameStatusText;
    [SerializeField] private Text _restartText;
    [SerializeField] private Text _ammoText;
    [SerializeField] private Text _missilesText;
    [SerializeField] private GameObject _nameInputContainer;
    [SerializeField] private InputField _nameInput;

    void Start()
    {
        _scoreText.text = $"Score: {0}";
        _gameStatusText.gameObject.SetActive(false);
    }

    private void Update()
    {
        DisableControls();
    }

    private void OnEnable()
    {
        Player.onPlayerDeath += this.OnPlayerDeath;
    }

    private void OnDisable()
    {
        Player.onPlayerDeath -= this.OnPlayerDeath;
    }

    private void DisableControls()
    {
        if (_nameInput.isFocused)
        {
            InputSystem.DisableDevice(Keyboard.current);
        }
        else
        {
            InputSystem.EnableDevice(Keyboard.current);
        }
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

    public void UpdateMissiles(int missilesCurrent, int missilesMax)
    {
        _missilesText.text = $"{missilesCurrent}/{missilesMax}";
    }

    IEnumerator GameStatusFlickerRoutine(string gameStatusMessage)
    {
        while (true)
        {
            _gameStatusText.text = $"{gameStatusMessage}";
            yield return new WaitForSeconds(0.5f);
            _gameStatusText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void OnPlayerDeath()
    {
        StartGameStatusScreen("Game Over");
    }

    public void StartGameStatusScreen(string msg)
    {
        _gameStatusText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        _nameInputContainer.SetActive(true);
        StartCoroutine(GameStatusFlickerRoutine(msg));
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
