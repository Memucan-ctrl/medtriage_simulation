using UnityEngine;

namespace MedTriage.Simulation.Patient
{
    [DisallowMultipleComponent]
    public class PatientRecoveryAnimator : MonoBehaviour
    {
        public bool Recovered;
        public Transform Chest;
        public Transform Head;
        public Transform LeftArm;
        public Transform RightArm;
        public float BreathingAmplitude = 0.018f;
        public float BreathingRate = 0.75f;
        public float HeadLookAmplitude = 9f;
        public float HeadLookRate = 0.28f;
        public float ArmMotionAmplitude = 3f;
        public float ArmMotionRate = 0.45f;
        Vector3 m_ChestBase;
        Quaternion m_HeadBase;
        Quaternion m_LeftArmBase;
        Quaternion m_RightArmBase;
        bool m_Cached;
        void Awake(){ AutoAssign(); CacheBase(); }
        void Start(){ AutoAssign(); CacheBase(); }
        void AutoAssign(){
            if(Chest==null) Chest=FindByNames(transform, "chest", "spine", "torso", "upperchest");
            if(Head==null) Head=FindByNames(transform, "head", "neck");
            if(LeftArm==null) LeftArm=FindByNames(transform, "leftarm", "leftupperarm", "larm", "arml");
            if(RightArm==null) RightArm=FindByNames(transform, "rightarm", "rightupperarm", "rarm", "armr");
        }
        Transform FindByNames(Transform root, params string[] keys){
            var all=root.GetComponentsInChildren<Transform>(true);
            foreach(var t in all){ var n=t.name.ToLowerInvariant().Replace(" ","").Replace("_",""); for(int i=0;i<keys.Length;i++) if(n.Contains(keys[i])) return t; }
            return null;
        }
        void CacheBase(){ if(m_Cached) return; if(Chest!=null) m_ChestBase=Chest.localPosition; if(Head!=null) m_HeadBase=Head.localRotation; if(LeftArm!=null) m_LeftArmBase=LeftArm.localRotation; if(RightArm!=null) m_RightArmBase=RightArm.localRotation; m_Cached=true; }
        public void TriggerRecovery(){ SetRecovered(true); }
        public void SetROSC(){ SetRecovered(true); }
        public void OnROSC(){ SetRecovered(true); }
        public void Recover(){ SetRecovered(true); }
        public void ResetPatientRecoveryVisuals(){ SetRecovered(false); }
        public void SetRecovered(bool value){ AutoAssign(); CacheBase(); Recovered=value; if(!Recovered){ if(Chest!=null) Chest.localPosition=m_ChestBase; if(Head!=null) Head.localRotation=m_HeadBase; if(LeftArm!=null) LeftArm.localRotation=m_LeftArmBase; if(RightArm!=null) RightArm.localRotation=m_RightArmBase; } }
        void Update(){
            if(!Recovered) return; AutoAssign(); CacheBase(); float t=Time.time;
            if(Chest!=null){ float b=Mathf.Sin(t*Mathf.PI*2f*BreathingRate)*BreathingAmplitude; Chest.localPosition=m_ChestBase+new Vector3(0f,b,b*0.35f); }
            if(Head!=null){ float yaw=Mathf.Sin(t*Mathf.PI*2f*HeadLookRate)*HeadLookAmplitude; float pitch=Mathf.Sin(t*Mathf.PI*2f*(HeadLookRate*0.55f))*3f; Head.localRotation=m_HeadBase*Quaternion.Euler(pitch,yaw,0f); }
            if(LeftArm!=null){ float a=Mathf.Sin(t*Mathf.PI*2f*ArmMotionRate)*ArmMotionAmplitude; LeftArm.localRotation=m_LeftArmBase*Quaternion.Euler(a,0f,0f); }
            if(RightArm!=null){ float a=Mathf.Sin(t*Mathf.PI*2f*(ArmMotionRate*0.9f))*ArmMotionAmplitude; RightArm.localRotation=m_RightArmBase*Quaternion.Euler(a,0f,0f); }
        }
    }
}
