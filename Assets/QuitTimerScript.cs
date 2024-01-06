using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuitTimerScript : MonoBehaviour
{
    /// <summary>
    /// prevMousePosition is initialized as this invalid position.
    /// </summary>
    private static readonly Vector3 MouseNotCaptured = Vector3.left;

    /// <summary>
    /// If non-zero, the screen saver quits specified seconds after start.
    /// </summary>
    public float QuitSecondsAfterBoot;
    private Vector3 prevMousePosition = MouseNotCaptured;
    private bool isQuitting = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(this.QuitTimerRoutine());
    }

    void Update()
    {
        // Spin
        this.transform.eulerAngles += new Vector3(0.0F, 0.0F, Time.deltaTime * 360.0F);

        bool isPreview = PlayerPrefs.GetInt("isPreview") != 0;
        if (isPreview)
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

    IEnumerator<YieldInstruction> QuitTimerRoutine()
    {
        if (this.QuitSecondsAfterBoot <= 0.0F)
        {
            yield break;
        }
        yield return new WaitForSeconds(this.QuitSecondsAfterBoot);
        Quit();
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
        var currentService =
            new AndroidJavaClass("jp.tkoolerlufar.madewithunity.screensaver.UnityPlayerDream")
            .GetStatic<AndroidJavaObject>("Companion")
            .Call<AndroidJavaObject>("getCurrentService");
        if (currentService is not null) {
            currentService.Call("finish");
            return;
        }
        var currentActivity =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity");
        if (currentActivity is not null) {
            currentActivity.Call<bool>("moveTaskToBack", true);
            return;
        }
        throw new SystemException("Failed to exit the screen saver.");
#else
        Application.Quit();
#endif
    }
}
