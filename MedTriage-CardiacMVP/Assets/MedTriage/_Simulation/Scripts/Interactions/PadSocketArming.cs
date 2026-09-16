using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using MedTriage.Simulation.Interactions;

// Arms the defibrillator when BOTH chest pad sockets are filled.
public class PadSocketArming : MonoBehaviour
{
    public XRSocketInteractor AnteriorSocket;
    public XRSocketInteractor LateralSocket;
    public MedTriage.Simulation.Interactions.DefibrillatorController Defibrillator;
    public AudioSource SeatedBeep;
    public float SeatedBeepVolume = 0.35f;

    bool m_BothSeated;
    public bool BothSeated { get { return m_BothSeated; } }

    void OnEnable()
    {
        if (AnteriorSocket != null)
        {
            AnteriorSocket.selectEntered.AddListener(HandleEnter);
            AnteriorSocket.selectExited.AddListener(HandleExit);
        }
        if (LateralSocket != null)
        {
            LateralSocket.selectEntered.AddListener(HandleEnter);
            LateralSocket.selectExited.AddListener(HandleExit);
        }
    }

    void OnDisable()
    {
        if (AnteriorSocket != null)
        {
            AnteriorSocket.selectEntered.RemoveListener(HandleEnter);
            AnteriorSocket.selectExited.RemoveListener(HandleExit);
        }
        if (LateralSocket != null)
        {
            LateralSocket.selectEntered.RemoveListener(HandleEnter);
            LateralSocket.selectExited.RemoveListener(HandleExit);
        }
    }

    void HandleEnter(SelectEnterEventArgs args) { Evaluate(); }
    void HandleExit(SelectExitEventArgs args) { Evaluate(); }

    void Evaluate()
    {
        bool a = AnteriorSocket != null && AnteriorSocket.hasSelection;
        bool l = LateralSocket != null && LateralSocket.hasSelection;
        bool now = a && l;
        if (now == m_BothSeated) return;
        m_BothSeated = now;
        if (!now) return;
        if (Defibrillator != null) Defibrillator.AttachPads();
        if (SeatedBeep != null && SeatedBeep.clip != null)
            SeatedBeep.PlayOneShot(SeatedBeep.clip, SeatedBeepVolume);
        Debug.Log("[PadSocketArming] Both pads seated - defibrillator armed.");
    }
}
