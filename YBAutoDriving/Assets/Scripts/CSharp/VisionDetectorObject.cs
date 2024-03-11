using UnityEngine;
using System.Collections;
using static UnityEngine.UI.Image;

/// <summary>
/// This script attaches to traffic sign, lights, and other traffic signs
/// </summary>
public class VisionDetectorObject : MonoBehaviour
{
    Renderer rend;
    public SignController signController;
    private Rect boundingBox;
    private bool shouldDrawBoundingBox = false;
    public int classId = 0;
    //a detector placed in front of sign to detect the facing direction
    private GameObject visibilitydetector;

    void Start()
    {
        rend = this.GetComponent<Renderer>();
        visibilitydetector = this.transform.Find("visibilitydetector").gameObject;

        //get the class Id from the object name
        if (this.name.ToLower().Contains("15km")) classId = 0;
        if (this.name.ToLower().Contains("30km")) classId = 1;
        if (this.name.ToLower().Contains("50km")) classId = 2;
        if (this.name.ToLower().Contains("70km")) classId = 3;
        if (this.name.ToLower().Contains("giveway")) classId = 4;
        if (this.name.ToLower().Contains("stop")) classId = 5;
    }

    void OnWillRenderObject()
    {

        if (Camera.current == signController.visionCamera)
        {
            boundingBox = CalculateBoundingBox(gameObject, Camera.current);
            var widthHeightRatio = boundingBox.width/boundingBox.height;


            shouldDrawBoundingBox =
                Mathf.Abs(boundingBox.height) * Mathf.Abs(boundingBox.width) > signController.minimumRenderedArea &&            //Not draw sign with small rendered size
                Vector3.Distance(this.gameObject.transform.position, signController.visionCamera.transform.position) < 30f      //Not draw sign too far away; 20f = 40meters
                && widthHeightRatio> signController.minimumWidthHeightRatio
                && widthHeightRatio< signController.maximumWidthHeightRatio
                //&& I_Can_See(this.gameObject, signController.visionCamera)
                && I_Can_See(this.visibilitydetector, signController.visionCamera)
                ? true : false;
            //Debug.Log($"{this.name} Min:{boundingBox.height}, Max:{boundingBox.width}, Draw?{shouldDrawBoundingBox}");

            //Regitster target object, and output image
            if (shouldDrawBoundingBox)
            {
                signController.OnSignIsRendered(boundingBox, this.classId);
            }
        }
    }
    void OnBecameInvisible()
    {
        shouldDrawBoundingBox = false;
    }

    private bool I_Can_See(GameObject Object, Camera camera)
    {

        //Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);

        //if (GeometryUtility.TestPlanesAABB(planes, Object.GetComponent<Collider>().bounds))
        //    return true;
        //else
        //    return false;


        RaycastHit hit;
        Vector3 heading = Object.transform.position - camera.transform.position;
        Vector3 direction = heading.normalized;// / heading.magnitude;

        if (Physics.Linecast(camera.transform.position, Object.GetComponentInChildren<Renderer>().bounds.center, out hit))
        {
            if (hit.transform.name != Object.name)
            {
                /* -->
                Debug.DrawLine(cam.transform.position, toCheck.GetComponentInChildren<Renderer>().bounds.center, Color.red);
                Debug.LogError(toCheck.name + " occluded by " + hit.transform.name);
                */
                //Debug.Log(Object.name + " occluded by " + hit.transform.name);
                return false;
            }
        }
        return true;
    }
    //https://forum.unity.com/threads/how-do-i-use-renderer-isvisible.377388/
    //https://docs.unity3d.com/ScriptReference/Renderer-isVisible.html
    //https://discussions.unity.com/t/how-can-i-know-if-a-gameobject-is-seen-by-a-particular-camera/248/9

    Rect CalculateBoundingBox(GameObject sign, Camera cam)
    {
        // Calculate and return the 2D bounding box of the sign in screen space
        // This will depend on how you represent the size and shape of your signs

        // Example for a simple approach using the object's renderer bounds (you might need a more complex approach)
        var bounds = rend.bounds;
        var minPoint = cam.WorldToScreenPoint(bounds.min);
        var maxPoint = cam.WorldToScreenPoint(bounds.max);

        // Adjusting for a custom viewport size (640x640 pixels)
        // Assuming the RenderTexture attached to the custom camera has this resolution
        float screenScale = 640f / 512f;
        minPoint.x /= screenScale;
        maxPoint.x /= screenScale;
        minPoint.y = (minPoint.y) / screenScale;
        maxPoint.y = (maxPoint.y) / screenScale;

        // Unity's screen coordinates: bottom is 0; convert to top as 0 for consistency with UI systems
        //minPoint.y = minPoint.y;
        //maxPoint.y = maxPoint.y;
        if (maxPoint.y < minPoint.y)
        {
            maxPoint.y = maxPoint.y + minPoint.y;
            minPoint.y = maxPoint.y - minPoint.y;
            maxPoint.y = maxPoint.y - minPoint.y;
        }

        var rect = new Rect(minPoint.x, 512 - maxPoint.y, maxPoint.x - minPoint.x, maxPoint.y - minPoint.y);
        //Debug.Log($"{this.name} :{minPoint},{maxPoint}");
        return rect;
    }
    void OnGUI()
    {
        if (shouldDrawBoundingBox)
        {
            // Define a highly visible style for the box
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.normal.textColor = Color.white;
            //style.normal.background = MakeTex(64, 64, Color.red);


            style.fontSize = 12;
            style.alignment = TextAnchor.MiddleCenter;
            style.border = new RectOffset(2, 2, 2, 2);


            // Increase thickness of the box for visibility
            GUI.color = Color.red;
            GUI.Box(new Rect(boundingBox.xMin - 2, boundingBox.yMin - 2, boundingBox.width + 4, boundingBox.height + 4), GUIContent.none, style); // Outer thick border
            GUI.color = Color.white; // Reset color to draw the text inside
            GUI.Box(boundingBox, this.name, style); // Inner box with text
            //Debug.Log($"{this.name} Min:{boundingBox.min}, Max:{boundingBox.max}");
        }

        Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

    }


}
