/// This class manages bins individually, mainly for animations (Open/close) and sounds.
/// It also calls back a waiting trash when this trash is waiting for the bin to open. (Happens when the trash and the bin aren't of the same type.)

using UnityEngine;
using System.Collections;

public class BinBehaviour : MonoBehaviour
{
    #region Fields
    /// Serialized
    [SerializeField] private TrashManager.TrashType _type = TrashManager.TrashType.Undefined;
    [SerializeField] private AudioClip _recycleSound = null;

        // trashTarget is used to set the trash at the right position for the jump animation.
    [SerializeField] private Transform _trashTarget = null;

    /// Non Serialized
    private BinsManager _binsManager = null;
    private Animation _animation = null;
    private TrashManager _trashManager = null;

        // When a trash hasn't been dropped on the right bin, we open this bin. When the bin is open, we callback this trash ans start the jump animation.
    private TrashBehaviour _trashToCallBack = null;
    #endregion Fields

    #region Properties
    public Transform TrashTarget
    {
        get { return _trashTarget; }
    }

    public TrashManager.TrashType Type
    {
        get { return _type; }
    }
    #endregion Properties

    #region Private Methods
    private void Awake()
    {
        // The component may be called by the BinManager's Start method, so it's safer to get the component in Awake.
        _animation = GetComponent<Animation>();
    }

    private void Start ()
    {
        _binsManager = BinsManager.Instance;
	}

    private void OnEnable()
    {
        // OnEnable will be called before Start(), so we need to initialize the _trashManager here. Also, I also changed the script execution order to make sure this is called after TrashManager's Awake().
        _trashManager = TrashManager.Instance;

        _trashManager.TrashPickedEvent += OnTrashPickedUp;
        _trashManager.TrashDroppedEvent += OnTrashDropped;
    }

    private void OnDisable()
    {
        // This may not be usefull in this prototype, but it's always better to have it.
        _trashManager.TrashPickedEvent -= OnTrashPickedUp;
        _trashManager.TrashDroppedEvent -= OnTrashDropped;
    }

    private void OnTrashPickedUp(TrashManager.TrashType pickedType)
    {
        // When you start dragging a piece of trash, the correct container opens.
        if(pickedType == _type)
        {
            Open();
        }
    }

    // When the dragged trash is dropped on the floor, the correct container closes
    private void OnTrashDropped(TrashManager.TrashType pickedType)
    {
        if (pickedType == _type)
        {
            Close();
        }
    }
    #endregion Private Methods

    #region Public Methods
    public void Open(TrashBehaviour trashToCallback = null)
    {
        _animation.Play("BinOpen");

        if(_trashToCallBack == null)
        {
            _trashToCallBack = trashToCallback;
        }
    }

    public void Close(bool queued = false)
    {
        if (queued == true)
        {
            // this is called during the introduction : we want the "BinOpen" animation to be completed before calling "BinClose".
            _animation.PlayQueued("BinClose");
        }
        else
        {
            _animation.Play("BinClose");
        }
    }

    // PlayOpenSound and PlayCloseSound are called through animation events.
    public void PlayOpenSound()
    {
        _binsManager.PlayOpenSound();
    }

    public void PlayCloseSound()
    {
        _binsManager.PlayCloseSound();
    }

    // In case a trash is dropped on this container, but isn't the same type. We call this trash back once the container is open. This is called though the OpenBin animation event.
    public void CheckForCallback()
    {
        if(_trashToCallBack != null)
        {
            _trashToCallBack.JumpIntoBin();
            _trashToCallBack = null;
        }
    }

    // Called when the trash's jump animation is complete.
    public void OnTrashIsInside(TrashManager.TrashType type)
    {
        Close();

        if(type == _type)
        {
            _binsManager.PlayRecyclingSound(_recycleSound);
        }
    }

    //When the close animation is complete, we call the BinsManager. It may fire an event, depending on the current gamestate.
    public void OnCloseAnimationComplete()
    {
        _binsManager.OnCloseAnimationComplete();
    }
    #endregion Public Methods
}
