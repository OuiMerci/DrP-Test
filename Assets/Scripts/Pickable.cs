// A pickable item is an element that can be dragged and dropped.
// Of course, we only have one pickable type on this prototype. However, this class allows adding more easily.

using UnityEngine;
using System.Collections;

public abstract class Pickable : MonoBehaviour {

    #region Enum
    public enum PickableType
    {
        Undefined,
        Trash
    }
    #endregion Enum

    #region Fields
    protected DragAndDropManager _dragAndDropManager = null;
    protected PickableType _pickableType = PickableType.Trash;

    // During the drag, this variable is used to keep the object at the same distance from the camera.
    private float _distanceFromCamera = 0;
    #endregion Fields

    #region Properties
    public float DistanceFromCamera
    {
        get { return _distanceFromCamera; }
    }
    #endregion Properties

    #region Protected Methods
    protected virtual void Start ()
    {
        _dragAndDropManager = DragAndDropManager.Instance;
        _distanceFromCamera = Vector3.Distance(Camera.main.transform.position, transform.position);
    }

    protected void OnMouseDown()
    {
        _dragAndDropManager.RequestDrag(this);
    }

    protected void OnMouseUp()
    {
        HandleDrop();
    }

    /// <summary>
    /// Tells the DragAndDrop Manager that the player can pick another object.
    /// </summary>
    protected void FreeDragManager()
    {
        _dragAndDropManager.AllowNewDrag(this);
    }

    virtual protected void HandleDrop()
    {
        // If no object is being dragged, we leave this method
        if (_dragAndDropManager.IsDragging() == false)
        {
            return;
        }

        _dragAndDropManager.StopDragging(this);
    }
    #endregion Protected Methods

    #region Public Methods
    virtual public void FirePickedEvent(){ }

    virtual public void FireDroppedEvent() { }

    virtual public int GetReceiverLayerMask()
    {
        return 1 << LayerMask.NameToLayer("Default");
    }

    abstract public PickableType GetPickableType();

    #endregion Public Methods
}
