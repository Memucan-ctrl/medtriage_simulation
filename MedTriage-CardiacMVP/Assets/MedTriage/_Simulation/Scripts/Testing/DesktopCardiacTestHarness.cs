#if ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine;
using MedTriage.Shared.Managers;
using MedTriage.Shared.Data;
using MedTriage.Simulation.Interactions;

namespace MedTriage.Simulation.Testing
{
    public class DesktopCardiacTestHarness : MonoBehaviour
    {
        public CompressionDetector CompressionDetector;
        public DefibrillatorController DefibrillatorController;
        public Transform ChestTarget;
        public Transform TestHand;
        public float CompressionDepthMeters = 0.055f;
        public bool AutoCompressions;
        float timer;
        bool pressing;
        Vector3 releasedPosition;

        void Awake()
        {
            if (CompressionDetector == null) CompressionDetector = FindAnyObjectByType<CompressionDetector>();
            if (DefibrillatorController == null) DefibrillatorController = FindAnyObjectByType<DefibrillatorController>();
            if (CompressionDetector != null && ChestTarget == null) ChestTarget = CompressionDetector.ChestTarget;
            if (TestHand == null)
            {
                GameObject go = new GameObject("Desktop_Test_CompressorHand");
                TestHand = go.transform;
            }
            if (ChestTarget != null)
            {
                releasedPosition = ChestTarget.position + ChestTarget.up * 0.08f;
                TestHand.position = releasedPosition;
                TestHand.rotation = ChestTarget.rotation;
            }
            if (CompressionDetector != null && CompressionDetector.CompressorHand == null) CompressionDetector.CompressorHand = TestHand;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B) && TaskManager.Instance != null) TaskManager.Instance.BeginSession();
            if (Input.GetKeyDown(KeyCode.C)) AutoCompressions = !AutoCompressions;
            if (Input.GetKeyDown(KeyCode.Space)) StartPress();
            if (Input.GetKeyDown(KeyCode.Alpha1) && DefibrillatorController != null) DefibrillatorController.AttachPads();
            // Alpha2 rhythm classification is temporarily disabled here to avoid coupling this desktop harness to the scenario enum namespace.
            if (Input.GetKeyDown(KeyCode.Alpha3) && DefibrillatorController != null) DefibrillatorController.BeginCharge();
            if (Input.GetKeyDown(KeyCode.Alpha4) && DefibrillatorController != null) DefibrillatorController.CallClear();
            if (Input.GetKeyDown(KeyCode.Alpha5) && DefibrillatorController != null) DefibrillatorController.TryDeliverShock();
            if (Input.GetKeyDown(KeyCode.F) && TaskManager.Instance != null) TaskManager.Instance.Finish();

            if (AutoCompressions) RunAuto();
            else if (pressing)
            {
                timer += Time.deltaTime;
                if (timer >= 0.12f) ReleasePress();
            }
        }

        void RunAuto()
        {
            timer += Time.deltaTime;
            if (!pressing && timer >= 0.18f) StartPress();
            else if (pressing && timer >= 0.12f) ReleasePress();
        }

        void StartPress()
        {
            if (ChestTarget == null || TestHand == null) return;
            pressing = true;
            timer = 0f;
            TestHand.position = ChestTarget.position - ChestTarget.up * CompressionDepthMeters;
        }

        void ReleasePress()
        {
            if (TestHand == null) return;
            pressing = false;
            timer = 0f;
            TestHand.position = releasedPosition;
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(20,20,560,260), GUI.skin.box);
            GUILayout.Label("Desktop Cardiac Test Harness");
            GUILayout.Label("B Begin | C Auto Compressions | Space Single | 1 Pads | 3 Charge | 4 Clear | 5 Shock | F Finish");
            GUILayout.Label("AutoCompressions: " + AutoCompressions);
            if (TaskManager.Instance != null)
            {
                GUILayout.Label("SessionActive: " + TaskManager.Instance.SessionActive + " Phase: " + TaskManager.Instance.Phase);
                GUILayout.Label("Events: " + TaskManager.Instance.GetEventsCopy().Count + " Cycles: " + TaskManager.Instance.CompletedCycles);
                GUILayout.Label("SecondsSinceLastCompression: " + TaskManager.Instance.SecondsSinceLastCompression().ToString("0.0"));
            }
            if (DefibrillatorController != null)
            {
                GUILayout.Label("DefibState: " + DefibrillatorController.State + " Charge: " + DefibrillatorController.ChargeProgress01.ToString("0.00") + " Shocks: " + DefibrillatorController.ShocksDelivered);
                GUILayout.Label("LastDenyReason: " + DefibrillatorController.LastDenyReason);
            }
            GUILayout.EndArea();
        }
    }
}
#endif
