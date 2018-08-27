using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ColorDetector : MonoBehaviour {

    /// <summary>
    /// location of the scanned object on the screen;
    /// </summary>
    [HideInInspector]
    public DataRectifier dataRectifier;
    public Vector3 objectScreenPosition;
    [HideInInspector]
    public int xStartCord, yStartCord;
    private Camera cam;
    private Texture2D screenCap;
    ColorDetectableStorage cds;
    private ColorNode[] nodes;

    //Product Special Dimmensions Correlating to Distance Placed Below
    public float paleonolaWidth = 300f, paleonolaHeight = 500f;

    //length and width of the region thats detected from the screen.
    public int checkedWidth = Screen.width / 3, checkedHeight = Screen.height / 3;

    [HideInInspector]
    public string actualProduct;

    //These are used for associating RGB values with a generalized color.
    public enum GeneralizedColor
    {
        Red,
        Orange,
        Yellow,
        GreenYellow,
        Green,
        GreenCyan,
        Cyan,
        BlueCyan,
        Blue,
        BlueMagenta,
        Magenta,
        RedMagenta
    }

    private void Start()
    {
        cam = Camera.main;
        screenCap = new Texture2D(checkedWidth, checkedHeight);
        dataRectifier = GetComponent<DataRectifier>();
        cds = GetComponent<ColorDetectableStorage>();
    }

    private void Awake()
    {
        InstantiateColorNodes();
    }

    public void UpdateObjectScreenPosition(Vector3 pos)
    {
        objectScreenPosition = pos;
    }

    /// <summary>
    /// Captures the screen and stores it in a Texture2D named screenCap.
    /// </summary>
    private void CaptureScreen()
    {
        //Process for capturing the screen's view and putting it into a Texture2D

        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        cam.targetTexture = rt;
        screenCap = new Texture2D(checkedWidth, checkedHeight, TextureFormat.RGB24, false);
        cam.Render();
        RenderTexture.active = rt;

        screenCap.ReadPixels(new Rect(xStartCord, yStartCord, checkedWidth, checkedHeight), 0, 0);

        screenCap.Apply();
        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
    }

    /// <summary>
    /// Gets the average color of all the pixels in the specified area.
    /// </summary>
    /// <param name="cap">The Texture2D screenCap</param>
    /// <returns>The average color.</returns>
    private Color GetAverageColor(Texture2D cap)
    {
        float red = 0, blue = 0, green = 0;
        int total = 0;

        Color32[] capColors = cap.GetPixels32();

        foreach (Color32 c in capColors)
        {
            red += c.r;
            green += c.g;
            blue += c.b;

            total++;
        }

        return new Color((red / total), (green / total), (blue / total));
    }

    /// <summary>
    /// Determines the product based on the currently viewed product and the average color.
    /// </summary>
    /// <returns>The vuforia file name of the correct file, otherwise returns null if no brand is detected.</returns>
    public string DetermineProduct(string fileName, Transform imgTarget, double distance)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(imgTarget.position);
        UpdateObjectScreenPosition(screenPos);
        string determinedProductName = null;
        string brand = DetermineBrand(fileName);


        if (brand == null || !cds.isColorDetectedBrand(brand))
        {
            //No Brand Found
            //for the shopping cart
            //TODO: CHECK THAT THIS SHIT WORKS AND MAKE ALL OF IT EQUAL
            Debug.Log("MEME");
            //string productName = imgTarget.GetComponent<Vuforia.ImageTargetBehaviour>().TrackableName;
            dataRectifier.SetImageValidity(false);
            //dataRectifier.correctedFoodName = productName;
            //
            dataRectifier.correctedFoodName = fileName;

            actualProduct = null;
            return null;
        }

        bool regionScanable = SetCheckedDimmensions(distance, brand); //Sets the dimmensions of the color capture. Determines if the region can be scanned.
        if (!regionScanable)
        {
            return null;
        }
        Transform child = imgTarget.GetChild(0);
        
        if((int)Time.time % 2 == 0)
        {
            Debug.Log("SCREEN CAP");
            child.gameObject.GetComponent<MeshRenderer>().enabled = false;

            foreach (Transform uiChild in child)
            {
                uiChild.GetComponent<MeshRenderer>().enabled = false;
            }
            
            CaptureScreen();
            child.gameObject.GetComponent<MeshRenderer>().enabled = true;

            foreach (Transform uiChild in child)
            {
                uiChild.GetComponent<MeshRenderer>().enabled = true;
            }
        }

        Color avgColor = GetAverageColor(screenCap);
        ColorNode[] cnodes = GetThreeClosestNodes(avgColor);
        GeneralizedColor[] genColors = ConvertColorNodeArrayToGeneralized(cnodes);

        determinedProductName = cds.getCorrectProduct(brand, genColors);

        if (debugging == true)
        {
            DebugMode(imgTarget, screenPos, brand, fileName, determinedProductName, screenCap, avgColor, genColors);
        }

        RectifyInformation(imgTarget, determinedProductName);

        return determinedProductName;
    }

    //TODO, naming convention changed. What is returned is only the upc now.
    /// <summary>
    /// Determines the brand of the product based off of the currently recognized image.
    /// </summary>
    /// <param name="product">The Vuforia file name of the product</param>
    /// <returns>The brand of the product</returns>
    public string DetermineBrand(string fileName)
    {
        //TODO I NEED TO FIX THIS
        string upcCode = "";

        foreach (char c in fileName)
        {
            if (System.Char.IsDigit(c))
            {
                upcCode += c;
            }
            else
            {
                break;
            }
        }

        return GettingSpreadsheetInfo.Brand(upcCode);
    }

    //TODO: THE CHECKED AREA IS POSSIBLY TO LARGE, SCALING ISSUES WHEN ROTATING, and AVERAGE COLOR IS BROWNISH FOR THE RED PRODUCT

    /// <summary>
    /// Checks the dimmensions of the area to be scanned.
    /// </summary>
    /// <param name="distance">Distance of the target away from the camera</param>
    /// <param name="brand">The product brand</param>
    /// <returns>returns true if successful and false if need to abort.</returns>
    public bool SetCheckedDimmensions(double distance, string brand)
    {
        if (brand == null)
        {
            return false;
        }

        if (brand.Equals("Paleonola"))
        {
            //TODO: Distance fucking sucks
            checkedHeight = (int)((1 - distance) * paleonolaHeight);
            checkedWidth = (int)((1 - distance) * paleonolaWidth);
        }

        xStartCord = (int)(objectScreenPosition.x - (checkedWidth / 2));
        yStartCord = (int)(objectScreenPosition.y - (checkedHeight / 2));

        if (xStartCord < 0)
        {
            //Starting cord out of bounds on left side
            //Debug.Log(xStartCord + "XCord Less Than Zero");
            checkedWidth += xStartCord;
            xStartCord = 0;

            if (checkedWidth <= 0)
            {
                return false;
            }
        }
        else if (xStartCord > Screen.width)
        {
            //Debug.Log(xStartCord + "XCord Greater Than Width");
            //Starting cord is out of bounds on the right side
            return false;
        }

        if (xStartCord + checkedWidth >= Screen.width)
        {
            //Checked area will be out of bounds
            //Debug.Log(xStartCord + "XCord Partially Greater Than Width");
            checkedWidth = Screen.width - xStartCord;
        }

        if (yStartCord < 0)
        {
            //Out of bounds, above camera
            //Debug.Log(yStartCord + "YCord is less than zero");
            checkedHeight -= (Mathf.Abs(yStartCord) + 1);
            yStartCord = 0;

            if (checkedHeight <= 0)
            {
                //Out of frame
                //Debug.LogError("CHECKED HEIGHT < 0");
                return false;
            }
        }
        else if (yStartCord > Screen.height)
        {
            //Out of bounds, below camera
            //Debug.Log(yStartCord + "YCord is greater than height");
            return false;
        }

        if (yStartCord + checkedHeight >= Screen.height)
        {
            //Checked area is too large and will check out of bounds, must shrink the area.
            //Debug.Log(yStartCord + "YCord is partially greater than height");
            //Debug.Log(yStartCord + "YStartCord + checkedHeight > Screen.height : " + yStartCord + " " + checkedHeight + " > " + Screen.height);
            checkedHeight = Screen.height - yStartCord;

            if (checkedHeight <= 0)
            {
                return false;
            }
        }
        //Debug.Log("Official X and Y values " + xStartCord + "   " + yStartCord);
        return true;
    }

    //Todo: Get better color nodes. Refer to the excel document and the gimp
    void InstantiateColorNodes()
    {
        nodes = new ColorNode[12];
        nodes[0] = new ColorNode(new Color(255, 0, 0), GeneralizedColor.Red);
        nodes[1] = new ColorNode(new Color(255, 127, 0), GeneralizedColor.Orange);
        nodes[2] = new ColorNode(new Color(255, 255, 0), GeneralizedColor.Yellow);
        nodes[3] = new ColorNode(new Color(127, 255, 0), GeneralizedColor.GreenYellow);
        nodes[4] = new ColorNode(new Color(0, 255, 0), GeneralizedColor.Green);
        nodes[5] = new ColorNode(new Color(0, 255, 127), GeneralizedColor.GreenCyan);
        nodes[6] = new ColorNode(new Color(0, 255, 255), GeneralizedColor.Cyan);
        nodes[7] = new ColorNode(new Color(0, 255, 255), GeneralizedColor.BlueCyan);
        nodes[8] = new ColorNode(new Color(0, 0, 255), GeneralizedColor.Blue);
        nodes[9] = new ColorNode(new Color(127, 0, 255), GeneralizedColor.BlueMagenta);
        nodes[10] = new ColorNode(new Color(255, 0, 255), GeneralizedColor.Magenta);
        nodes[11] = new ColorNode(new Color(255, 0, 127), GeneralizedColor.RedMagenta);
    }

    public ColorNode[] GetThreeClosestNodes(Color c)
    {
        ColorNode[] closestNodeArray = new ColorNode[3];
        float[] distanceValues = new float[3];

        for (int i = 0; i < nodes.Length; i++)
        {
            float distance = CalculateColorDistance(c, nodes[i].c);

            //index 0 is the farthest, 3 is closest
            for (int j = closestNodeArray.Length - 1; j >= 0; j--)
            {
                if (closestNodeArray[j] == null || distance < distanceValues[j])
                {
                    for (int k = 0; k < j; k++)
                    {
                        closestNodeArray[k] = closestNodeArray[k + 1];
                        distanceValues[k] = distanceValues[k + 1];
                    }

                    closestNodeArray[j] = nodes[i];
                    distanceValues[j] = distance;

                    break;
                }
            }
        }

        return closestNodeArray;
    }

    public GeneralizedColor[] ConvertColorNodeArrayToGeneralized(ColorNode[] nodes)
    {
        ColorDetector.GeneralizedColor[] generalizedColors;
        generalizedColors = new ColorDetector.GeneralizedColor[nodes.Length];

        for (int i = 0; i < nodes.Length; i++)
        {
            generalizedColors[i] = nodes[i].gc;
        }

        return generalizedColors;
    }

    
    /*

    /// <summary>
    /// Determines the generalized color based off of the rgb value.
    /// </summary>
    /// <returns></returns>
    public GeneralizedColor DetermineGeneralizedColor(Color c)
    {
        int closestIndex = -1;
        float closestDistance = -1;
        for (int i = 0; i < nodes.Length; i++)
        {
            float d = CalculateColorDistance(c, nodes[i].c);
            
            if(closestIndex == -1 || d < closestDistance)
            {
                closestIndex = i;
                closestDistance = d;
                continue;
            }
        }

        Debug.Log("Color : " + c + "   |  Distance " + closestDistance + " to " + nodes[closestIndex].gc);
        return nodes[closestIndex].gc; //TODO: Add more colors
    }

    */

    public float CalculateColorDistance(Color a, Color b)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(a.r - b.r, 2) + Mathf.Pow(a.g - b.g, 2) + Mathf.Pow(a.b - b.b, 2));

        return distance;
    }

    public Color AverageGeneralizedColors(GeneralizedColor[] colors)
    {
        float r = 0, g = 0, b = 0;

        foreach(GeneralizedColor gc in colors)
        {
            foreach(ColorNode n in nodes)
            {
                if(n.gc == gc)
                {
                    //found matching ColorNode
                    r += n.c.r;
                    g += n.c.g;
                    b += n.c.b;
                    break;
                }
            }
        }

        Color avg = new Color(r / colors.Length, g / colors.Length, b / colors.Length);
        return avg;
    }

    // TODO: FIX COLOR DETERMINATION

    public Color GetProductsColor(Texture productTexture)
    {
        if(productTexture == null)
        {
            Debug.LogError("Texture Supplied Is Null");
            return Color.white;
        }
        Texture2D texture = (Texture2D) productTexture;
        Color avgColor = GetAverageColor(texture);
        return avgColor;
    }

    //I should one day make a rectifier class that corrects what image is being viewed based on all the given information.
    public void RectifyInformation(Transform imageTarget, string actualProduct)
    {
        if (actualProduct != null)
        {
            this.actualProduct = actualProduct;

            if (!dataRectifier.GetImageValidity())
            {
                dataRectifier.SetImageValidity(true);
            }
            dataRectifier.CorrectInformation(imageTarget, actualProduct);
        }
        else
        {
            this.actualProduct = null;
            dataRectifier.SetImageValidity(false);
        }
    }

    [Header("Debugging Options Below")]
    //Public Debugging Variables Below:
    [Tooltip("Make sure to enable debugging before selecting any other debugging options")]
    public bool debugging;
    public bool displayAverageColor, displayGeneralizedColor, displayObjectScreenPosition, displayCheckedAreas, displayCapturedTexture, displayStartingCaptureCords;
    
    //Debugging Classes Below
    public void DebugMode(Transform trans,Vector3 screenPos, string determinedBrand, string assumedProductName, string determinedProductName, Texture2D texture, Color avgColor, GeneralizedColor[] genColors)
    {
        //Displays a wired cube within the editor to show the area that's being checked
        if (debugging)
        {
            if (displayAverageColor)
            {
                Debug.Log("Average Color: (Red: " + avgColor.r + "), (Green: " + avgColor.g + "), (Blue: " + avgColor.b + "). ");
            }
            if (displayGeneralizedColor)
            {
                Debug.Log("Generalized Color: " + dataRectifier.correctedFoodName);
            }
            if (displayObjectScreenPosition)
            {
                Debug.Log("Objects Position To Camera: " + objectScreenPosition);
            }
            if (displayCheckedAreas)
            {
                //Note: this will mess with the rgb value
                GameObject checkedAreaCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                checkedAreaCube.transform.position = screenPos;

                Vector3 checkedAreaScale = new Vector3(checkedWidth, checkedHeight, 0);
                checkedAreaCube.transform.localScale = checkedAreaScale;
                Destroy(checkedAreaCube,.1f);
            }
            if (displayStartingCaptureCords)
            {
                Debug.Log("X Start Cord : " + xStartCord + " | Y Start Cord : " + yStartCord);
            }
        }

    }

    private void OnGUI()
    {
        if (debugging)
        {
            if (displayCapturedTexture)
            {
                Rect screenShotRect = new Rect(Vector2.zero, new Vector2(screenCap.width, screenCap.height));
                GUI.DrawTexture(screenShotRect, screenCap);
            }
        }
    }
}

public class ColorNode
{
    public ColorDetector.GeneralizedColor gc;
    public Color c;

    public ColorNode(Color c, ColorDetector.GeneralizedColor gc)
    {
        this.c = c;
        this.gc = gc;
    }
}

