using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SignController : MonoBehaviour
{
    public bool isOutputToFile = false;
    public bool isOutputToPython = false;
    public bool isClearFolderBeforeRun = false;

    public string outputPath = "";
    public string TrafficSignType;
    public float minimumRenderedArea = 10f;             //threshold of output vision
    public float minimumWidthHeightRatio = 1 / 3f;     //threshold of width/height ratio to output vision
    public float maximumWidthHeightRatio = 3 / 1f;     //threshold of width/height ratio to output vision
    public heyCar pythonConnector;

    public Camera visionCamera;
    public GameObject car; // Reference to the car

    List<GameObject> trafficSigns = new List<GameObject>();
    List<Tuple<Rect, int>> annotations = new List<Tuple<Rect, int>>();

    public string versionCode = "v1";

    void Start()
    {
        FindTrafficSignsRecursively(this.transform, ref trafficSigns);

        foreach (var sign in trafficSigns)
        {
            sign.AddComponent<VisionDetectorObject>().signController = this;
            //Debug.Log("Found traffic sign: " + sign.name);
        }

        if(isOutputToFile && isClearFolderBeforeRun)
        {
            ClearDirectoryContents(this.outputPath);
        }
    }
    void FindTrafficSignsRecursively(Transform parent, ref List<GameObject> foundSigns)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(TrafficSignType))
            {
                foundSigns.Add(child.gameObject);
            }

            // Recursively search in the current child
            FindTrafficSignsRecursively(child, ref foundSigns);
        }
    }

    private void LateUpdate()
    {
        if (annotations.Count <= 0) return;

        GetVision();

        if (isOutputToFile)
        {
            OutputVision();
        }
        if (isOutputToPython)
        {
            pythonConnector.SendVision(this.imgData);
        }
        annotations.Clear();
    }

    internal void OnSignIsRendered(Rect boundingBox, int classIndex)
    {
        float scaleRatio = 640f / 512f;

        // Scale the bounding box size and position
        Rect scaledBoundingBox = new Rect(
            boundingBox.x * scaleRatio,
            boundingBox.y * scaleRatio,
            boundingBox.width * scaleRatio,
            boundingBox.height * scaleRatio);

        // Flip the Y coordinate around the texture's central line
        // Remember that in Unity's texture coordinate system, Y=0 is at the bottom.
        scaledBoundingBox.y = 640 - (scaledBoundingBox.y + scaledBoundingBox.height);

        annotations.Add(new Tuple<Rect, int>(scaledBoundingBox, classIndex));
    }



    private int imgId = 0;
    byte[] imgData = null;
    private void GetVision()
    {
        // Capture the screen or RenderTexture content
        RenderTexture activeRenderTexture = RenderTexture.active;
        RenderTexture.active = this.visionCamera.targetTexture;

        Texture2D texture = new Texture2D(this.visionCamera.targetTexture.width, this.visionCamera.targetTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, this.visionCamera.targetTexture.width, this.visionCamera.targetTexture.height), 0, 0);
        texture.Apply();
        //foreach (var annotation in annotations)
        //{
        //    DrawBoundingBox(ref texture, annotation.Item1, Color.red); // Color can be changed as needed
        //}
        RenderTexture.active = activeRenderTexture; // Reset active RenderTexture

        // Save texture as PNG
        imgData = texture.EncodeToPNG();
        RenderTexture.active = null;
    }

    string isTrainOrVal = "Train";
    public int trainValProportion=20;
    private void OutputVision()
    {
        System.Random random = new System.Random();

        isTrainOrVal = random.Next(0,100) >= trainValProportion ? "train" : "val";

        // Generate filename based on the current time (or any unique naming scheme)
        string fileName = $"YBDriving_{versionCode}_{System.DateTime.Now:yyyyMMddHHmmss}_{imgId}";
        string imagePath = $"{outputPath}/{isTrainOrVal}/images/{fileName}.png";
        string labelPath = $"{outputPath}/{isTrainOrVal}/labels/{fileName}.txt";
        imgId++;

        System.IO.File.WriteAllBytes(imagePath, imgData);
        Debug.Log($"Saved vision texture to {imagePath}");


        SaveAnnotations(annotations, labelPath);
    }
    private void SaveAnnotations(List<Tuple<Rect, int>> annotations, string path)
    {
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
        {
            foreach (Tuple<Rect, int> annotation in annotations)
            {
                // Normalize the bounding box coordinates
                float x_center = (annotation.Item1.xMin + annotation.Item1.width / 2) / this.visionCamera.targetTexture.width;
                float y_center = 1-(annotation.Item1.yMin + annotation.Item1.height / 2) / this.visionCamera.targetTexture.height;
                float width = annotation.Item1.width / this.visionCamera.targetTexture.width;
                float height = annotation.Item1.height / this.visionCamera.targetTexture.height;

                string line = $"{annotation.Item2} {x_center} {y_center} {width} {height}";
                file.WriteLine(line);
            }
        }
        Debug.Log($"Saved annotations to {path}");
    }

    public void ClearDirectoryContents(string path)
    {
        deleteAllFiles( $"{path}/train/images/");
        deleteAllFiles( $"{path}/train/labels/");
        deleteAllFiles( $"{path}/val/images/");
        deleteAllFiles( $"{path}/val/labels/");

        void deleteAllFiles(string filepath)
        {
            foreach (string file in System.IO.Directory.GetFiles(filepath))
            {
                System.IO.File.Delete(file);
            }
        }
    }

    private void DrawBoundingBox(ref Texture2D texture, Rect boundingBox, Color color)
    {
        // Draw the border of the rectangle
        DrawLine(ref texture, (int)boundingBox.xMin, (int)boundingBox.yMin, (int)boundingBox.xMax, (int)boundingBox.yMin, color); // Top
        DrawLine(ref texture, (int)boundingBox.xMin, (int)boundingBox.yMin, (int)boundingBox.xMin, (int)boundingBox.yMax, color); // Left
        DrawLine(ref texture, (int)boundingBox.xMax, (int)boundingBox.yMin, (int)boundingBox.xMax, (int)boundingBox.yMax, color); // Right
        DrawLine(ref texture, (int)boundingBox.xMin, (int)boundingBox.yMax, (int)boundingBox.xMax, (int)boundingBox.yMax, color); // Bottom
    }

    private void DrawLine(ref Texture2D tex, int x0, int y0, int x1, int y1, Color col)
    {
        int dy = y1 - y0;
        int dx = x1 - x0;
        int stepx, stepy;

        if (dy < 0) { dy = -dy; stepy = -1; } else { stepy = 1; }
        if (dx < 0) { dx = -dx; stepx = -1; } else { stepx = 1; }
        dy <<= 1;        // dy is now 2*dy
        dx <<= 1;        // dx is now 2*dx

        tex.SetPixel(x0, y0, col);
        if (dx > dy)
        {
            int fraction = dy - (dx >> 1);  // same as 2*dy - dx
            while (x0 != x1)
            {
                if (fraction >= 0)
                {
                    y0 += stepy;
                    fraction -= dx;    // same as fraction -= 2*dx
                }
                x0 += stepx;
                fraction += dy;    // same as fraction += 2*dy
                tex.SetPixel(x0, y0, col);
            }
        }
        else
        {
            int fraction = dx - (dy >> 1);
            while (y0 != y1)
            {
                if (fraction >= 0)
                {
                    x0 += stepx;
                    fraction -= dy;
                }
                y0 += stepy;
                fraction += dx;
                tex.SetPixel(x0, y0, col);
            }
        }
        tex.Apply();
    }
}
