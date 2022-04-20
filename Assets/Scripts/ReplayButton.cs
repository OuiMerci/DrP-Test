/// This Class manages the "Replay" button.
/// This button will appear after the game through the GameOverEvent is fired.

using UnityEngine;
using System.Collections;

public class ReplayButton : MonoBehaviour {

    #region Fields
        /// Serialized
    [SerializeField] private StarParticles _starParticles = null;
    #endregion Fields

    #region Methods
    private void Awake () {
        // In this case, we cannot use OnEnable/OnDisable to register to events. So we have to use Awake/OnDestroy.
        _starParticles.GameOverEvent += SetActiveTrue;
        gameObject.SetActive(false);
	}

    private void OnDestroy()
    {
        _starParticles.GameOverEvent -= SetActiveTrue;
    }

    private void SetActiveTrue ()
    {
        gameObject.SetActive(true);
	}
    #endregion Methods
}
