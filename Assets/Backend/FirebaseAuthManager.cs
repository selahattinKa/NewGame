using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;
#if !UNITY_EDITOR
using Google;
#endif

namespace CanavarZindanlari.Backend
{
    /// <summary>
    /// Firebase başlatma + Google Sign-In (cihazda) / Anonim giriş (editörde).
    /// Sahneye tek örnek olarak eklenir, DontDestroyOnLoad.
    /// </summary>
    public class FirebaseAuthManager : MonoBehaviour
    {
        public static FirebaseAuthManager Instance { get; private set; }

        [Tooltip("Firebase Console → Project Settings → General → Web client ID")]
        public string WebClientId = "";

        public FirebaseUser CurrentUser => _auth?.CurrentUser;
        public bool IsSignedIn         => _auth?.CurrentUser != null;
        public bool FirebaseReady       => _ready;

        public event Action<FirebaseUser> OnSignInSuccess;
        public event Action<string>       OnSignInFailed;
        public event Action               OnSignedOut;

        private FirebaseAuth _auth;
        private bool         _ready;

        // ── Lifecycle ────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private async void Start() => await InitFirebase();

        // ── Firebase başlatma ────────────────────────────────────────────────

        private async Task InitFirebase()
        {
            var status = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (status != DependencyStatus.Available)
            {
                Debug.LogError($"[Auth] Firebase hazır değil: {status}");
                OnSignInFailed?.Invoke($"Firebase başlatılamadı: {status}");
                return;
            }

            _auth  = FirebaseAuth.DefaultInstance;
            _ready = true;

            if (_auth.CurrentUser != null)
                OnSignInSuccess?.Invoke(_auth.CurrentUser);
        }

        // ── Giriş ────────────────────────────────────────────────────────────

        public async void SignIn()
        {
            if (!_ready) { Debug.LogWarning("[Auth] Firebase henüz hazır değil."); return; }

#if UNITY_EDITOR
            await SignInAnonymous();
#else
            await SignInGoogle();
#endif
        }

#if UNITY_EDITOR
        private async Task SignInAnonymous()
        {
            try
            {
                var result = await _auth.SignInAnonymouslyAsync();
                Debug.Log($"[Auth] Editör anonim giriş: {result.User.UserId}");
                OnSignInSuccess?.Invoke(result.User);
            }
            catch (Exception e) { OnSignInFailed?.Invoke(e.Message); }
        }
#else
        private async Task SignInGoogle()
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId    = WebClientId,
                RequestIdToken = true,
            };

            try
            {
                var googleUser = await GoogleSignIn.DefaultInstance.SignIn();
                var credential  = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);
                var result      = await _auth.SignInWithCredentialAsync(credential);
                Debug.Log($"[Auth] Google giriş: {result.User.DisplayName}");
                OnSignInSuccess?.Invoke(result.User);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Auth] Google Sign-In hatası: {e.Message}");
                OnSignInFailed?.Invoke(e.Message);
            }
        }
#endif

        // ── Çıkış ────────────────────────────────────────────────────────────

        public void SignOut()
        {
            _auth?.SignOut();
#if !UNITY_EDITOR
            GoogleSignIn.DefaultInstance.SignOut();
#endif
            OnSignedOut?.Invoke();
        }
    }
}
