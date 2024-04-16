using UnityEngine;

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

            int _width = Display.main.systemWidth;
            int _height = Display.main.systemHeight;

            Texture2D _screenshotTexture = new Texture2D(_width, _height, TextureFormat.ARGB32, false);

            Rect _rect = new Rect(0, 0, _width, _height);
            _screenshotTexture.ReadPixels(_rect, 0, 0);
            _screenshotTexture.Apply();

            if (this.saveScreenshotImage)
            {
                byte[] _byteArray = _screenshotTexture.EncodeToPNG();
                System.IO.File.WriteAllBytes(Application.dataPath + "/CameraScreenshot.png", _byteArray);
                Debug.Log("Saved CameraScreenshot.png");
            }
            
            Sprite _capturedImage = Sprite.Create(_screenshotTexture, _rect, new Vector2(0.5f, 0.5f));
            this.imageToCut = _capturedImage;

            this.isReadyToCut = true;
        }
    }

    public void TakeScreenshot() {
        Debug.Log("xxxxxxxxxxx");
        this.takeScreenshotOnNextFrame = true;
    }

    public Sprite GetImageToCut()
    {
        return this.imageToCut;
    }

    public bool GetIsReadyToCut()
    {
        return this.isReadyToCut;
    }
}
