using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    private string customId;

    public void UpdateCustomId(string newCustomId)
    {
        customId = newCustomId;
    }
    
    public void LoginWithCustomID()
    {
        var request = new LoginWithCustomIDRequest { CustomId = customId, CreateAccount = true};
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }
    
    public void LoginWithSteam()
    {
        var request = new LoginWithSteamRequest();
        PlayFabClientAPI.LoginWithSteam(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you logged in!");
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your login.");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    
    
}