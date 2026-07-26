using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
 
namespace Medtriage.Frontend.Diagnostics
{
    /// <summary>
    /// TEMPORARY diagnostic script. Attach to any empty GameObject in a scratch
    /// scene (not part of Build Settings), press Play, and read the Console.
    /// Confirms Unity Gaming Services Authentication and Cloud Save are wired up
    /// correctly, independent of SessionManager or any UI. Remove this GameObject
    /// once every line below prints [PASS].
    /// </summary>
    public class UGSSmokeTest : MonoBehaviour
    {
        [SerializeField] private string testUsername = "smoketest_user";
        [SerializeField] private string testPassword = "SmokeTest123!";
 
        private async void Start()
        {
            Debug.Log("[UGSSmokeTest] Starting...");
 
            try
            {
                await UnityServices.InitializeAsync();
                Debug.Log("[UGSSmokeTest] [PASS] UnityServices.InitializeAsync succeeded.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[UGSSmokeTest] [FAIL] InitializeAsync: {e.Message}");
                return;
            }
 
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(testUsername, testPassword);
                Debug.Log("[UGSSmokeTest] [PASS] SignUpWithUsernamePasswordAsync created a new test account.");
            }
            catch (AuthenticationException)
            {
                Debug.Log("[UGSSmokeTest] Test account already exists from a previous run — trying sign-in instead.");
                try
                {
                    await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(testUsername, testPassword);
                    Debug.Log("[UGSSmokeTest] [PASS] SignInWithUsernamePasswordAsync succeeded.");
                }
                catch (Exception e2)
                {
                    Debug.LogError($"[UGSSmokeTest] [FAIL] Sign-in fallback: {e2.Message}");
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[UGSSmokeTest] [FAIL] SignUpWithUsernamePasswordAsync: {e.Message}");
                return;
            }
 
            Debug.Log($"[UGSSmokeTest] Signed in. PlayerId = {AuthenticationService.Instance.PlayerId}");
 
            try
            {
                var toSave = new Dictionary<string, object> { { "smoke_test_key", "hello_medtriage" } };
                await CloudSaveService.Instance.Data.Player.SaveAsync(toSave);
                Debug.Log("[UGSSmokeTest] [PASS] Cloud Save SaveAsync succeeded.");
 
                var loaded = await CloudSaveService.Instance.Data.Player.LoadAsync(
                    new HashSet<string> { "smoke_test_key" });
 
                if (loaded.TryGetValue("smoke_test_key", out var item) &&
                    item.Value.GetAs<string>() == "hello_medtriage")
                {
                    Debug.Log("[UGSSmokeTest] [PASS] Cloud Save LoadAsync returned the value we just saved.");
                }
                else
                {
                    Debug.LogError("[UGSSmokeTest] [FAIL] Cloud Save LoadAsync did not return the expected value.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[UGSSmokeTest] [FAIL] Cloud Save round-trip: {e.Message}");
                return;
            }
 
            Debug.Log("[UGSSmokeTest] Done. If every line above says [PASS], Authentication and Cloud Save are correctly wired up.");
        }
    }
}

