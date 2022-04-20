/// The GameManager manages the game's workflow. Since this is just prototype, its main purpose is to handle the game's states and the transitions between them.

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

    #region Enums
    public enum GameState
    {
        Intro,
        Playing,
        WaitForAnimationToEnd,
        Victory
    }
    #endregion Enums

    #region Fields
    // We could get these by instance, but this way, we will be sure that we have it for the Enable();
    [SerializeField] private TrashManager _trashManager = null;
    [SerializeField] private StarParticles _starParticles = null;
    [SerializeField] private BinsManager _binsManager = null;
    [SerializeField] private bool _cannotJumpIntoWrongBin = false;

    static private GameManager _instance;
    private GameState _gameState = GameState.Intro;

    // Amount of trash at the beginning
    private int _baseTrashCount = 0;

    // Amount of trash already dropped in bin
    private int _destoyedTrash = 0;
    #endregion Fields

    #region Properties
    static public GameManager Instance
    {
        get { return _instance; }
    }

    public GameState State
    {
        get { return _gameState; }
    }

    public bool CannotJumpIntoWrongBin
    {
        get { return _cannotJumpIntoWrongBin; }
    }
    #endregion Properties

    #region Private Methods
    private void Awake () {
        _instance = this;
	}

    private void Start()
    {
        // Store the amount of trash we have to recycle
        _baseTrashCount = _trashManager.GetChildrenCount();

        // Start  the game
        GameStart();
    }

    private void OnEnable()
    {
        _binsManager.IntroductionCompleteEvent += OnIntroductionComplete;
        _trashManager.TrashJumpStartEvent += OnTrashJumpStarted;
        _trashManager.TrashInsideBinEvent += OnTrashInsidebin;
    }

    private void OnDisable()
    {
        _binsManager.IntroductionCompleteEvent -= OnIntroductionComplete;
        _trashManager.TrashJumpStartEvent -= OnTrashJumpStarted;
        _trashManager.TrashInsideBinEvent -= OnTrashInsidebin;
    }

    // Introduction -> Playing
    private void OnIntroductionComplete()
    {
        _gameState = GameState.Playing;
    }

    // Playing -> WaitForAnimationToEnd
    private void OnTrashJumpStarted()
    {
        _gameState = GameState.WaitForAnimationToEnd;
    }

    // This is called when a trash has been dropped in a bin. If this was the last trash, we start the "Victory" state, else, we go back to the Playing state.
    private void OnTrashInsidebin()
    {
        _destoyedTrash++;
        bool victory = CheckTrashCount();

        if(victory)
        {
            _gameState = GameState.Victory;
            _starParticles.gameObject.SetActive(true);
        }
        else
        {
            _gameState = GameState.Playing;
        }
    }

    private void GameStart()
    {
        // In a bigger project, an event would be nice here. But since we only have to start an animation, I kept it simple.
        _binsManager.StartRollIn();
    }

    // Check if all trashed have been dropped into bins.
    private bool CheckTrashCount()
    {
        if(_destoyedTrash >= _baseTrashCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion Private Methods

    #region Public Methods
    // Called by the "Replay" button, after all particle effects are done. Simply reloads the game.
    public void OnReplay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion Public Methods
}
