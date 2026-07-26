using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Medtriage.Shared.Managers;
 
namespace Medtriage.Frontend.Auth
{
    /// <summary>
    /// Thin wrapper around Unity Gaming Services Authentication, using the
    /// Username &amp; Password provider (see Medtriage_Team_Development_Guide.docx,
    /// Section 6.2). Before this will work you must enable the Username &amp; Password
    /// ID provider for your project in the Unity Dashboard or the Editor's
    /// Services > Authentication > Configure window — see README.md.
    /// </summary>
    public static class AuthManager
    {
        public static bool ServicesInitialized { get; private set; }
 
        /// <summary>Call once, early (AppBootstrapper does this), before any sign-in call.</summary>
        public static async Task InitializeServicesAsync()
        {
            if (ServicesInitialized) return;
 
            try
            {
                await UnityServices.InitializeAsync();
                ServicesInitialized = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[AuthManager] Failed to initialize Unity Services: {e.Message}");
            }
        }
 
        public static async Task<AuthResult> SignUpAsync(string username, string password)
        {
            await InitializeServicesAsync();
 
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                SessionManager.Instance?.OnLoginSuccess(AuthenticationService.Instance.PlayerId);
                return AuthResult.Ok();
            }
            catch (AuthenticationException e)
            {
                return AuthResult.Fail(FriendlyMessage(e));
            }
            catch (RequestFailedException e)
            {
                return AuthResult.Fail(FriendlyMessage(e));
            }
        }
 
        public static async Task<AuthResult> SignInAsync(string username, string password)
        {
            await InitializeServicesAsync();
 
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
                SessionManager.Instance?.OnLoginSuccess(AuthenticationService.Instance.PlayerId);
                return AuthResult.Ok();
            }
            catch (AuthenticationException e)
            {
                return AuthResult.Fail(FriendlyMessage(e));
            }
            catch (RequestFailedException e)
            {
                return AuthResult.Fail(FriendlyMessage(e));
            }
        }
 
        /// <summary>
        /// Tries to restore a cached session (the SDK caches a session token locally
        /// after any previous successful sign-in, regardless of method) without
        /// showing the login screen again. Returns false if there is nothing to
        /// resume, in which case the caller should route to the Login scene.
        /// </summary>
        public static async Task<bool> TryResumeSessionAsync()
        {
            await InitializeServicesAsync();
 
            if (!AuthenticationService.Instance.SessionTokenExists)
                return false;
 
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                SessionManager.Instance?.OnLoginSuccess(AuthenticationService.Instance.PlayerId);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[AuthManager] Could not resume cached session: {e.Message}");
                return false;
            }
        }
 
        public static void SignOut()
        {
            if (AuthenticationService.Instance.IsSignedIn)
                AuthenticationService.Instance.SignOut();
 
            SessionManager.Instance?.SignOut();
        }
 
        private static string FriendlyMessage(Exception e)
        {
            var message = e.Message ?? "Something went wrong. Please try again.";
            Debug.LogWarning($"[AuthManager] {message}");
 
            if (message.Contains("Invalid"))
                return "That username or password isn't right. Please try again.";
            if (message.Contains("already"))
                return "That username is already taken. Try signing in instead.";
 
            return "We couldn't reach the login service. Check your connection and try again.";
        }
    }
 
    public readonly struct AuthResult
    {
        public bool Success { get; }
        public string ErrorMessage { get; }
 
        private AuthResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }
 
        public static AuthResult Ok() => new AuthResult(true, null);
        public static AuthResult Fail(string message) => new AuthResult(false, message);
    }
}