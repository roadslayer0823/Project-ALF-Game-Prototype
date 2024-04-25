using UnityEngine;
using UnityEngine.Rendering;

public class ScreenshotHandler : MonoBehaviour {

    [SerializeField] bool saveScreenshotImage = false;

    private bool takeScreenshotOnNextFrame = false;
    private bool isReadyToCut = false;
    private Sprite imageToCut = null;

    private void OnPostRender()
    {
        if (takeScreenshotOnNextFrame)
        {
            this.takeScreenshotOnNextFrame = false;

            // get the screen width and height
            int _width = Display.main.systemWidth;
            int _height = Display.main.systemHeight;

            // create 2d texture
            Texture2D _screenshotTexture = new Texture2D(_width, _height, TextureFormat.ARGB32, false);

            // read the screen pixels and apply it to the created 2d texture
            Rect _rect = new Rect(0, 0, _width, _height);
            _screenshotTexture.ReadPixels(_rect, 0, 0);
            _screenshotTexture.Apply();

            if (this.saveScreenshotImage) // true if want save the image into local directory
            {
                byte[] _byteArray = _screenshotTexture.EncodeToPNG();
                System.IO.File.WriteAllBytes(Application.dataPath + "/CameraScreenshot.png", _byteArray);
                Debug.Log("Saved CameraScreenshot.png");
            }

            // create sprite from the screenshot texture
            Sprite _capturedImage = Sprite.Create(_screenshotTexture, _rect, new Vector2(0.5f, 0.5f));
            this.imageToCut = _capturedImage;

            this.isReadyToCut = true;
        }
    }

    // to take screenshot
    public void TakeScreenshot() {
        this.takeScreenshotOnNextFrame = true;
    }

    // get the captured image
    public Sprite GetImageToCut()
    {
        return this.imageToCut;
    }

    // to check is the image capture done
    public bool GetIsReadyToCut()
    {
        return this.isReadyToCut;
    }
}
