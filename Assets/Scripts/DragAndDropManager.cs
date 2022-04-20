// This class manages the drag and drop system, but what happens once the item is dropped is managed by the item itself. (Through HandleDrop())

using UnityEngine;
using System.Collections;

public class DragAndDropManager : MonoBehaviour
{
    #region Fields
    // This should be enough, and better than using Mathf.infinity
    private const int MAX_RAYCAST_DISTANCE = 20;

    static private DragAndDropManager _instance = null;
    private GameManager _gameManager = null;
    private AudioSource _audio = null;
    private Pickable _draggedObject = null;
    private bool _dragEnabled = false;
    private bool _doDrag = false;

    //This is used to make some test before freeing the drag manager
    private Pickable _lastDraggedObject = null;

    // The layer masks we use when raycasting
    private int _uiLayerMask = 0;
    private int _receiverLayerMask = 0;
    #endregion Fields

    #region Properties
    static public DragAndDropManager Instance
    {
        get { return _instance; }
    }
    #endregion Properties

    #region Private Methods
    private void Awake()
    {
        _instance = this;
        _dragEnabled = true;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _audio = GetComponent<AudioSource>();
        _uiLayerMask = 1 << LayerMask.NameToLayer("UI");
    }

    private void StartDrag()
    {
        // Request is accepted. Item picked -> fire corresponding event
        _draggedObject.FirePickedEvent();

        // We can't pick another element
        _dragEnabled = false;

        // Play pickup sound. (Maybe it would be better to let the AudioManager react to the "PickedUpEvent", but it would get too messy on a bigger project)
        _audio.Play();

        _doDrag = true;
        _receiverLayerMask = _draggedObject.GetReceiverLayerMask();

        StartCoroutine(DragCoroutine());
    }

    /// <summary>
    /// This coroutine Handle the dragging process.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DragCoroutine()
    {
        //Raycast to the Background and determine the new position of the object.
        while (_doDrag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, MAX_RAYCAST_DISTANCE, _uiLayerMask))
            {
                // We always use the same distance to pick a point on the ray.
                _draggedObject.transform.position = ray.GetPoint(_draggedObject.DistanceFromCamera);
            }

            yield return null;
        }
    }
    
    /// <summary>
    /// This methods tries to find a receiver with a Raycast on a specific layer.
    /// </summary>
    /// <param name="receiver">This parameter will store the result.</param>
    /// <returns></returns>
    private bool LookForReceiver (out GameObject receiver)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        receiver = null;

        if (Physics.Raycast(ray, out hit, MAX_RAYCAST_DISTANCE, _receiverLayerMask))
        {
            if(hit.transform.gameObject != null)
            {
                receiver = hit.transform.gameObject;
            }
        }

        return receiver != null;
    }
    #endregion Private Methods

    #region Public Methods
    /// <summary>
    /// Checks if the item was dropped on a valid receiver and if yes, returns it.
    /// </summary>
    /// <typeparam name="T">Receiver type</typeparam>
    /// <param name="receiver">Result receiver</param>
    public T FindReceiver<T>()
    {
        GameObject objectReceiver;
        T receiver;

        // Check if it was dropped on a valid object and try to get the related component
        if (LookForReceiver(out objectReceiver))
        {
            receiver = objectReceiver.GetComponent<T>();
        }
        else
        {
            receiver = default(T);
        }

        return receiver;
    }

    /// <summary>
    /// Allows the player to start dragging a new item.
    /// </summary>
    /// <param name="pickable"></param>
    public void AllowNewDrag(Pickable pickable)
    {
        // We want to be sure that the caller is the one being dragged
        if (pickable == _lastDraggedObject)
        {
            _dragEnabled = true;
        }
    }

    public bool IsDragging()
    {
        return _doDrag;
    }

    /// <summary>
    /// This is called when we want the item to stop following the mouse's cursor.
    /// </summary>
    /// <param name="pickable"></param>
    public void StopDragging(Pickable pickable)
    {
        // We want to be sure that the caller is the one being dragged
        if (pickable == _draggedObject)
        {
            _lastDraggedObject = _draggedObject;
            _draggedObject = null;
            _doDrag = false;
        }
    }

    /// <summary>
    /// When a Pickable is clicked, it requests to be dragged.
    /// </summary>
    /// <param name="pickable">Item to drag.</param>
    public void RequestDrag(Pickable pickable)
    {
        /// The double check here is usefull when we drop the trash on the wrong bin.
        /// While this bin opens and the good one closes, _dragEnabled is the only variable preventing from picking another object.
        if (_gameManager.State == GameManager.GameState.Playing && _dragEnabled == true)
        {
            _draggedObject = pickable;
            StartDrag();
        }
    }
    #endregion Public Methods
}