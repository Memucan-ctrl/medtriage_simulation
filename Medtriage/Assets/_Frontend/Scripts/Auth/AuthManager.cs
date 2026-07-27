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
        
        public static string CurrentPlayerId => AuthenticationService.Instance.PlayerId;
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

            if (!ServicesInitialized)
                return AuthResult.Fail("Unity Services could not be initialized. Check your connection and try again.");

            try
            {
                if (AuthenticationService.Instance.IsSignedIn)
                {
                    // A trainee may already own an anonymous account. Add credentials to
                    // that same player so Cloud Save progress and the Player ID are preserved.
                    await AuthenticationService.Instance.AddUsernamePasswordAsync(username, password);
                }
                else
                {
                    await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                }

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

            if (!ServicesInitialized)
                return AuthResult.Fail("Unity Services could not be initialized. Check your connection and try again.");

            try
            {
                // Unity Authentication does not allow a second sign-in while an
                // anonymous/cached player is active. Explicitly leave that session
                // before signing in to the requested linked account.
                if (AuthenticationService.Instance.IsSignedIn)
                    AuthenticationService.Instance.SignOut(true);

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

            if (!ServicesInitialized)
                return false;

            if (AuthenticationService.Instance.IsSignedIn)
                return true;

            if (!AuthenticationService.Instance.SessionTokenExists)
                return false;

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
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
            if (ServicesInitialized && AuthenticationService.Instance.IsSignedIn)
                AuthenticationService.Instance.SignOut(true);

            SessionManager.Instance?.SignOut();
        }
 
private static string FriendlyMessage(Exception e)
        {
            string message = e.Message ?? "Something went wrong. Please try again.";
            Debug.LogWarning($"[AuthManager] {message}");
            string lower = message.ToLowerInvariant();

            if (lower.Contains("username") && (lower.Contains("taken") || lower.Contains("already")))
                return "That username is already linked to an account. Try signing in instead.";
            if (lower.Contains("invalid") || lower.Contains("password") && lower.Contains("incorrect"))
                return "That username or password isn't correct. Please try again.";
            if (lower.Contains("provider") || lower.Contains("configuration") || lower.Contains("disabled"))
                return "Username and password authentication is not enabled for this Unity project.";
            if (lower.Contains("already signed in"))
                return "A trainee session is already active. Please try again.";
            if (lower.Contains("network") || lower.Contains("timeout") || lower.Contains("reach"))
                return "We couldn't reach the authentication service. Check your connection and try again.";

            return message;
        }
    

public static async Task<AuthResult> SignInAnonymouslyAsync()
        {
            await InitializeServicesAsync();

            if (!ServicesInitialized)
                return AuthResult.Fail("Unity Services could not be initialized. Check your connection and try again.");

            if (AuthenticationService.Instance.IsSignedIn)
            {
                SessionManager.Instance?.OnLoginSuccess(AuthenticationService.Instance.PlayerId);
                return AuthResult.Ok();
            }

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
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