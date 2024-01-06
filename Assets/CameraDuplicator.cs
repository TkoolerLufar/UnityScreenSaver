using System.Linq;
using UnityEngine;

public class CameraDuplicator : MonoBehaviour
{
    /// <summary>
    /// A canvas for this camera.
    /// </summary>
    public Canvas Canvas;

    /// <summary>
    /// False if this camera is a replica from the Main Camera.
    /// </summary>
    bool IsMainCamera = true;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // Set the canvas' target display as same as the camera
        var camera = GetComponent<Camera>();
        this.Canvas.targetDisplay = camera.targetDisplay;
        // Duplicate this camera for other displays
        if (this.IsMainCamera)
        {
            foreach (var i in Enumerable.Range(0, Display.displays.Length))
            {
                if (i == camera.targetDisplay)
                {
                    continue;
                }
                var display = Display.displays[i];
                display.Activate();
                var anotherCameraObject = Instantiate(gameObject);
                anotherCameraObject.name = $"Camera {i}";
                anotherCameraObject.GetComponent<Camera>().targetDisplay = i;
                anotherCameraObject.GetComponent<CameraDuplicator>().IsMainCamera = false;
                anotherCameraObject.transform.parent = this.transform.parent.transform;
            }
        }
    }
}
