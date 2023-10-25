using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class Authenticator
{
    public Authenticator()
    {
        Initializer();
    }
    public async void Initializer()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void SignIn(Action callback = null)
    {
        AuthenticationService.Instance.SignedIn += callback;
        try
        {
            #if UNITY_EDITOR
            if (ParrelSync.ClonesManager.IsClone())
            {
                string customArgument = ParrelSync.ClonesManager.GetArgument();

                AuthenticationService.Instance.SwitchProfile($"Clone_{customArgument}_Profile");
            }
            #endif
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
