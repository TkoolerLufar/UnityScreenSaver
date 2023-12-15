using TMPro;
using UnityEngine;

public class ServiceGetterScript : MonoBehaviour
{
    public TextMeshProUGUI TextMesh;

    // Start is called before the first frame update
    void Start()
    {
        if (TextMesh == null) {
            return;
        }
        #if !UNITY_EDITOR && UNITY_ANDROID
        var currentService =
            new AndroidJavaClass("jp.tkoolerlufar.madewithunity.screensaver.UnityPlayerDream")
            .GetStatic<AndroidJavaObject>("Companion")
            .Call<AndroidJavaObject>("getCurrentService");
        if (null != (object)currentService) {
            TextMesh.text = "Android Screen Saver Running";
        }
        else {
            TextMesh.text = "Android Screen Saver Failed";
        }
        #elif UNITY_STANDALONE_WIN
        if (PlayerPrefs.GetInt("isPreview") != 0)
        {
            TextMesh.text = "Preview";
        }
        else
        {
            TextMesh.text = "Windows Screen Saver Running";
        }
        #endif
    }
}
