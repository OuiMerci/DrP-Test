// This class manages all the bins as a group. Since this is just a prototype, this manager is only used for the introduction.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BinsManager : MonoBehaviour
{
    #region Events
    // Called when the "Open/Close Bin" animations are complete. The GameManager will react to it and change GameState to "Playing".
    public delegate void IntroductionCompleteHandler();
    private IntroductionCompleteHandler _introductionCompleteEvent;
    #endregion Events

    #region Fields
    /// Serialized Fields
    [SerializeField] private List<BinBehaviour> _binsList = null;
    [SerializeField] private AudioClip _binOpenSound = null;
    [SerializeField] private AudioClip _binCloseSound = null;

    /// Non serialized Fields
    private static BinsManager _instance = null;
    private GameManager _gameManager = null;
    private Animation _animation = null;
    private AudioSource _audioSource = null;
    private int _binsCount = 0;
    #endregion Fields

    #region Properties
    public event IntroductionCompleteHandler IntroductionCompleteEvent
    {
        add { _introductionCompleteEvent += value; }
        remove { _introductionCompleteEvent -= value; }
    }

    public static BinsManager Instance
    {
        get { return _instance; }
    }
    #endregion Properties

    #region Private Methods
    private void Awake ()
    {
        _instance = this;
        _animation = GetComponent<Animation>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }
    #endregion Private Methods

    #region Public Methods
    // This method is for the instruction : "After that the container will open and close 1 time."
    public void StartOpenCloseAnimations()
    {
        _binsList.ForEach(b => b.Open());
        _binsList.ForEach(b => b.Close(true));
    }

    public void ToggleRollInSound()
    {
        if(_audioSource.isPlaying == true)
        {
            _audioSource.Stop();
        }
        else
        {
            _audioSource.Play();
        }
    }

    // When a bin finishes its "BinClose" animation, it calls this method. Depending on the Game State, it may fire an event.
    public void OnCloseAnimationComplete()
    {
        // If this is the intro, it will wait for all bins to have finish the animation before firing the "Introduction Complete" event.
        // In this prototype, all animation should end at the same time, so this step is not mandatory. However, I think it's something nice to have.
        if(_gameManager.State == GameManager.GameState.Intro)
        {
            _binsCount++;

            if (_binsCount >= _binsList.Count)
            {
                _binsCount = 0;
                
                if(_introductionCompleteEvent != null)
                {
                    _introductionCompleteEvent();
                }
            }
        }

        // After a trash is dropped in a bin, I used to wait for the bin to close before firing an event. This happened in this method.
        // The game manager now directly reacts to the "TrashJumpCompleteEvent" and changes the game state. It allows playing faster.
    }

    // Called by the game manager when the game starts.
    public void StartRollIn()
    {
        _animation.Play("BinRollIn");
    }

    public void PlayCloseSound()
    {
        _audioSource.PlayOneShot(_binCloseSound);

    }

    public void PlayOpenSound()
    {
        _audioSource.PlayOneShot(_binOpenSound);
    }

    public void PlayRecyclingSound(AudioClip sound)
    {
        _audioSource.PlayOneShot(sound);
    }
    #endregion Public Methods
}
