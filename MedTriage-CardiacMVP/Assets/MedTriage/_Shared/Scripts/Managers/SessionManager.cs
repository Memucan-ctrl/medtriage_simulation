using UnityEngine;

namespace MedTriage.Shared.Managers
{
    public class SessionManager : MonoBehaviour
    {
        static SessionManager instance;
        public static SessionManager Instance { get { return instance; } }

        public string UserId { get; private set; }
        public bool IsSignedIn { get; private set; }

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        void OnDestroy()
        {
            if (instance == this) instance = null;
        }

        public void OnLoginSuccess(string userId)
        {
            UserId = userId;
            IsSignedIn = true;
        }

        public void OnLogout()
        {
            UserId = null;
            IsSignedIn = false;
        }
    }
}
