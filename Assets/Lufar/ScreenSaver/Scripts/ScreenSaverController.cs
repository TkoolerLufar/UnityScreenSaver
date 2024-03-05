using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenSaverController : MonoBehaviour
{
    /// <summary>
    /// prevMousePosition is initialized as this invalid position.
    /// </summary>
    private static readonly Vector3 MouseNotCaptured = Vector3.left;

    /// <summary>
    /// True if this is preview.
    /// </summary>
    public bool IsPreview
    {
        get =>
#if UNITY_STANDALONE && !UNITY_EDITOR
        PlayerPrefs.GetInt("isPreview") != 0
#else
        false
#endif
        ;
    }
    private Vector3 prevMousePosition = MouseNotCaptured;
    private bool isQuitting = false;

    void Update()
    {
        if (IsPreview)
        {
            return;
        }

        // Quit when the screen is touched
        if (Input.touchCount > 0)
        {
            Quit();
        }

        // Quit when the mouse is moving
        var currentMousePosition = Input.mousePosition;
        try
        {
            var mouseVelocity = currentMousePosition - prevMousePosition;
            if (prevMousePosition != MouseNotCaptured &&
            mouseVelocity != Vector3.zero)
            {
                Quit();
            }
        }
        finally
        {
            prevMousePosition = currentMousePosition;
        }

        // Quit when any key or mouse button is pressed
        if (Input.anyKey)
        {
            Quit();
        }
    }

    /// <summary>
    /// Try to quit this screen saver.
    /// This function does nothing after once success.
    /// </summary>
    void Quit()
    {
        if (this.isQuitting)
        {
            return;
        }
        QuitApplication();
        this.isQuitting = true;
    }

    /// <summary>
    /// Quit this screen saver. Throw something if it fails.
    /// </summary>
    void QuitApplication()
    {
#if UNITY_EDITOR
        // Only log and do nothing else in editor
        Debug.Log("Quit!");
#elif UNITY_ANDROID
        using var UnityPlayerDream = new AndroidJavaClass("jp.tkoolerlufar.madewithunity.screensaver.UnityPlayerDream");
        using var UnityPlayerDreamCompanion = UnityPlayerDream.GetStatic<AndroidJavaObject>("Companion");
        using var currentService = UnityPlayerDreamCompanion.Call<AndroidJavaObject>("getCurrentService");
        if (currentService is not null)
        {
            currentService.Call("finish");
            return;
        }
        using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        using var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        if (currentActivity is not null)
        {
            currentActivity.Call<bool>("moveTaskToBack", true);
            return;
        }
        throw new SystemException("Failed to exit the screen saver.");
#else
        Application.Quit();
#endif
    }
}
