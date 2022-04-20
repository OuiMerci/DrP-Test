/// The TrashManager manages all the trash objects and trash related events.

using UnityEngine;
using System.Collections;

public class TrashManager : MonoBehaviour {

    #region enums
    public enum TrashType
    {
        Undefined,
        Food,
        Glass,
        Paper,
        Plastic
    }
    #endregion enums

    #region Events
    // Called when a trash is picked
    public delegate void TrashPickedHandler(TrashType type);
    private event TrashPickedHandler _trashPickedEvent;

    //This event is called when the trash was dropped, but not on a bin
    public delegate void TrashDroppedHandler(TrashType type);
    private event TrashDroppedHandler _trashDroppedEvent;

    // This event is called when a trash was dropped on a bin and jumps into it (animation started)
    public delegate void TrashJumpAnimationStarted();
    private event TrashJumpAnimationStarted _trashJumpStartEvent;

    // This event is called when a trash was dropped on a bin and jumped into it (animation complete)
    public delegate void TrashInsideBin();
    private event TrashInsideBin _trashInsideBinEvent;
    #endregion Events

    #region Fields
    /// Serialized
    // This is used to ensure that objects don't go under the ground, which could happen when dragged.
    // The ground is linked to the trashed manager, but if we had other physic related elements than trash (like bombs or bonuses), then this variable should be linked to another parent like "ElementsManager" for example
    [SerializeField] private Transform _ground;

        /// Non Serialized
    private const float OBJECTS_MIN_DISTANCE_TO_SCREEN_BORDER = 0.1f;
    static private TrashManager _instance = null;
    private int _binsLayerMask = 0;

    // This is used to keep objects inside the screen
    private float _screenHorizontalLimit = 0;
    #endregion Fields

    #region Properties
    static public TrashManager Instance
    {
        get { return _instance; }
    }

    public event TrashPickedHandler TrashPickedEvent
    {
        add { _trashPickedEvent += value; }
        remove { _trashPickedEvent -= value; }
    }

    public event TrashDroppedHandler TrashDroppedEvent
    {
        add { _trashDroppedEvent += value; }
        remove { _trashDroppedEvent -= value; }
    }

    public event TrashJumpAnimationStarted TrashJumpStartEvent
    {
        add { _trashJumpStartEvent += value; }
        remove { _trashJumpStartEvent -= value; }
    }

    public event TrashInsideBin TrashInsideBinEvent
    {
        add { _trashInsideBinEvent += value; }
        remove { _trashInsideBinEvent -= value; }
    }

    public float ScreenHorizontalLimit
    {
        get { return _screenHorizontalLimit; }
    }

    public Transform Ground
    {
        get { return _ground; }
    }
    #endregion Properties

    #region Private Methods
    private void Awake()
    {
        _instance = this;
    }

    private void Start ()
    {
        _screenHorizontalLimit = Camera.main.orthographicSize * Screen.width / Screen.height - OBJECTS_MIN_DISTANCE_TO_SCREEN_BORDER;

        // Used to check if the object is dropped on a bin
        _binsLayerMask = 1 << LayerMask.NameToLayer("BinsLayer");
    }
    #endregion Private Methods

    #region Public Methods
    public void OnTrashPicked(TrashType type)
    {
        if (_trashPickedEvent != null)
        {
            _trashPickedEvent(type);
        }
    }

    public void OnTrashDropped(TrashType type)
    {
        if (_trashDroppedEvent != null)
        {
            _trashDroppedEvent(type);
        }
    }

    public void OnTrashJumpAnimationStarted()
    {
        if(_trashJumpStartEvent != null)
        {
            _trashJumpStartEvent();
        }
    }

    public void OnTrashInsideBin()
    {
        if (_trashInsideBinEvent != null)
        {
            _trashInsideBinEvent();
        }
    }

    // Counts all children to determine how many trashes the plyer has to recycle.
    public int GetChildrenCount()
    {
        int trashCount = 0;

        foreach(Transform child in transform)
        {
            trashCount++;
        }

        return trashCount;
    }

    public int GetBinsLayerMask()
    {
        return _binsLayerMask;
    }
    #endregion Public Methods
}
