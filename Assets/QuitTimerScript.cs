using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuitTimerScript : MonoBehaviour
{
    public float QuitSecondsAfterBoot;
    private Vector3 prevMousePosition = Vector3.left;
    private bool isQuitting = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(this.QuitTimerRoutine());
    }

    void Update()
    {
        // ぐるぐる回す
        this.transform.eulerAngles += new Vector3(0.0F, 0.0F, 6.0F);

        bool isPreview = PlayerPrefs.GetInt("isPreview") != 0;
        if (isPreview)
        {
            return;
        }

        // Quit if the screen is be touched
        if (Input.touchCount > 0)
        {
            Quit();
        }

        var currentMousePosition = Input.mousePosition;
        try
        {
            var mouseVelocity = currentMousePosition - prevMousePosition;
            if (prevMousePosition != Vector3.left &&
            mouseVelocity != Vector3.zero)
            {
                Quit();
            }
        }
        finally
        {
            prevMousePosition = currentMousePosition;
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
