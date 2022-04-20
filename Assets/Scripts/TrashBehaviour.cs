// This class manage each trash individually. It extends the pickable class.

using UnityEngine;
using System.Collections;
using System;

public class TrashBehaviour : Pickable {

    #region Fields
        /// serialized
    [SerializeField] private TrashManager.TrashType _trashType = TrashManager.TrashType.Undefined;

    /// Non Serialized
    private const float OBJECTS_MIN_DISTANCE_ABOVE_GROUND = 0.1f;
    private TrashManager _trashManager = null;

    // This variable is used when the object is dropped on the floor. It make so its has the same Z value than at the game start, so we're sure the object won't fall behind the bins.
    private float _baseZ = 0;

    // If the trash is dropped on a bin, we store a reference of this bin for later purpose
    private BinBehaviour _linkedBin = null;
    #endregion Fields

    #region Private Methods
    protected override void Start () {

        base.Start();

        _trashManager = TrashManager.Instance;
        _baseZ = transform.position.z;
    }

    override protected void HandleDrop()
    {
        // If no item is currently dragged, exit the method
        if(_dragAndDropManager.IsDragging() == false)
        {
            return;
        }

        base.HandleDrop();

        BinBehaviour bin = _dragAndDropManager.FindReceiver<BinBehaviour>();

        // Did we find a bin ?
        if (bin != null)
        {
            // We link this trash to the bin it was dropped on
            _linkedBin = bin;

            // Check if going into the wrong bin is allowed.
            if (bin.Type == _trashType || GameManager.Instance.CannotJumpIntoWrongBin == false)
            {
                // The rigidbody may cause problems when animating, so we remove it.
                Destroy(GetComponent<Rigidbody>());

                // We also remove the collider so the object is not clickable anymore.
                Destroy(GetComponent<Collider>());

                // Prepare the position for the Jump animation.
                transform.position = bin.TrashTarget.position;
                transform.rotation = bin.TrashTarget.rotation;

                if (bin.Type == _trashType)
                {
                    // It was dropped on the right bin. The bin is already open, so the trash can jump into it.
                    JumpIntoBin();
                }
                else
                {
                    // It wasn't dropped on the right bin. We need to open it and close the "right" one.

                    // Open the bin the trash was dropped on and ask for a callback when the animation is over. When the "BinClose" animation is over, the trash will jump into it.
                    bin.Open(this);

                    // Close the bin it should have been dropped on. (Using this event, the right bin will close.)
                    _trashManager.OnTrashDropped(_trashType);
                }
            }
            else
            {
                DropTrash();
            }

        }
        else
        {
            DropTrash();
        }
    }

    /// <summary>
    /// Lets the trash fall on the ground.
    /// </summary>
    private void DropTrash()
    {
        _trashManager.OnTrashDropped(_trashType);
        FreeDragManager();

        // Reset the Z value of the object.
        transform.position = new Vector3(transform.position.x, transform.position.y, _baseZ);
    }

    private void Update()
    {
        ClampTrashPosition();
    }

    // Make sure the trash don't go beyond the screen limits. On the Z axis limits are difficult to determine, we want it to be able to move on the axis but not too much, so I used a collider.
    private void ClampTrashPosition()
    {
        float x = Mathf.Clamp(transform.position.x, Camera.main.transform.position.x - _trashManager.ScreenHorizontalLimit, Camera.main.transform.position.x + _trashManager.ScreenHorizontalLimit);
        float y = transform.position.y <= _trashManager.Ground.position.y ? OBJECTS_MIN_DISTANCE_ABOVE_GROUND : transform.position.y;

        transform.position = new Vector3(x, y, transform.position.z);
    }
    #endregion Private Methods

    #region Public Methods
    public void JumpIntoBin()
    {
        // For some reason, the animator was disabling the gravity even when in an "empty" state.
        // When enabling the component, it will start the "jump" animation right away, I deleted this empty state from the animator since it became useless.
        GetComponent<Animator>().enabled = true;
    }

    public override void FirePickedEvent()
    {
        _trashManager.OnTrashPicked(_trashType);
    }

    public override void FireDroppedEvent()
    {
        _trashManager.OnTrashDropped(_trashType);
    }

    /// <summary>
    /// Called when the trash is inside the bin.
    /// </summary>
    public void OnJumpAnimationComplete()
    {
        // Let the player pick another object.
        FreeDragManager();

        // Inform the Trash Manager
        _trashManager.OnTrashInsideBin();

        // Inform the linked bin
        _linkedBin.OnTrashIsInside(_trashType);

        // Disappear
        gameObject.SetActive(false);
    }

    public override int GetReceiverLayerMask()
    {
        return _trashManager.GetBinsLayerMask();
    }

    public override PickableType GetPickableType()
    {
        return PickableType.Trash;
    }
    #endregion Public Methods
}
