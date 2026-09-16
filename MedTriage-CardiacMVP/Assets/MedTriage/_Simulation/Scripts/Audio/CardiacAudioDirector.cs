using UnityEngine;
using MedTriage.Simulation.Interactions;

namespace MedTriage.Simulation.Audio
{
    public class CardiacAudioDirector : MonoBehaviour
    {
        public DefibrillatorController Defibrillator;
        public Animator PatientAnimator;
        public AudioSource ShockSource;
        public AudioSource FlatlineSource;
        public AudioSource MonitorLoop;
        public float ShockVolume = 0.35f;
        public float FlatlineVolume = 0.30f;

        private int _lastShocks = -1;
        private bool _lastTerminated;

        private void Start()
        {
            if (Defibrillator != null) _lastShocks = Defibrillator.ShocksDelivered;
            if (PatientAnimator != null) _lastTerminated = PatientAnimator.GetBool("Terminated");
        }

        private void Update()
        {
            if (Defibrillator != null)
            {
                int s = Defibrillator.ShocksDelivered;
                if (_lastShocks >= 0 && s > _lastShocks) PlayShock();
                _lastShocks = s;
            }

            if (PatientAnimator != null)
            {
                bool term = PatientAnimator.GetBool("Terminated");
                if (term && !_lastTerminated) PlayFlatline();
                _lastTerminated = term;
            }
        }

        private void PlayShock()
        {
            if (ShockSource == null || ShockSource.clip == null) return;
            ShockSource.volume = ShockVolume;
            ShockSource.PlayOneShot(ShockSource.clip, ShockVolume);
            Debug.Log("[CardiacAudioDirector] Shock delivered.");
        }

        private void PlayFlatline()
        {
            if (MonitorLoop != null && MonitorLoop.isPlaying) MonitorLoop.Stop();
            if (FlatlineSource == null || FlatlineSource.clip == null) return;
            FlatlineSource.volume = FlatlineVolume;
            FlatlineSource.Play();
            Debug.Log("[CardiacAudioDirector] Flatline.");
        }
    }
}