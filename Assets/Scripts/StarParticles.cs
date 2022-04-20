/// This class is used at the end of the game handles the particle effects. (It is currently composed of 2 particle systems for a better effect).
/// When these effects are over, it fires the GameOver event. This causes the "Replay" button to appear.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarParticles : MonoBehaviour {

    #region Fields
    ///Serialized
    ParticleSystem[] _particleSystems = null;
    #endregion Fields

    #region Events
    // In this prototype, the GameOver event is only used to make the "replay" button appear.
    public delegate void GameOverHandler();
    private GameOverHandler _gameOverEvent;
    #endregion Events

    #region Properties
    public event GameOverHandler GameOverEvent
    {
        add { _gameOverEvent += value; }
        remove { _gameOverEvent -= value; }
    }
    #endregion Properties

    #region Methods
    private void Awake()
    {
        // This list will be used to check if particles are still "Alive".
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    private void Update()
    {
        bool endGame = IsGameOver();

        if (endGame == true)
        {
            OnGameOver();
        }
    }

    private bool IsGameOver()
    {
        // We assume that game is over when all particle effects are gone.

        for (int i = 0; i < _particleSystems.Length; i++)
        {
            if (_particleSystems[i].IsAlive() == true)
            {
                return false;
            }
        }

        return true;
    }
	
	private void OnGameOver ()
    {
        if(_gameOverEvent != null)
        {
            _gameOverEvent();
        }

        gameObject.SetActive(false);
	}
    #endregion Methods
}
