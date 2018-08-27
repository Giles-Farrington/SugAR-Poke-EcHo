using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO: Automate detecting products and color given a brand.
[System.Serializable]
public class ColorDetectable
{
    public bool updated = false;
    public string brand;
    [SerializeField]
    public Product[] autoDetectedProducts;
    ColorDetector colorDetector;

    public bool debugging = true;
    public bool debugProductCreation = true, debugProductRecognition = true;


    //TODO, naming convention changed, check within method for comment
    /// <summary>
    /// Give the brand isn't null, this will auto detect products and put them in the array.
    /// </summary>
    public void detectProducts()
    {
        Debug.Log("MEME");
        if(brand == null)
        {
            Debug.LogError("No Brand Name Entered");
            return;
        }

        LinkedList<Product> tempProductList = new LinkedList<Product>();
        foreach (Transform imgTarget in Camera.main.transform)
        {
            Material imgTargetMaterial = imgTarget.GetComponent<MeshRenderer>().material;
            string vuforiaFileName = imgTargetMaterial.name;
            //TODO, brand string is just UPC, fix this
            string brand = colorDetector.DetermineBrand(vuforiaFileName);

            if (this.brand.Equals(brand))
            {

                //Found a product of a similar brand.
                int lastIndex = vuforiaFileName.LastIndexOf("Material");
                vuforiaFileName = vuforiaFileName.Substring(0, lastIndex);

                if (ProductAlreadyAdded(vuforiaFileName))
                {
                    //product already in list
                    continue;
                }
                updated = true;
                Color color = colorDetector.GetProductsColor(imgTargetMaterial.mainTexture);

                Product newProduct = new Product(brand, vuforiaFileName, color);
                tempProductList.AddLast(newProduct);
            }
        }

        //turn linkedlist into array
        autoDetectedProducts = new Product[tempProductList.Count];
        int i = 0;
        foreach (Product p in tempProductList)
        {
            autoDetectedProducts[i] = p;
            i++;
        }
    }

    bool ProductAlreadyAdded(string productName)
    {
        if(autoDetectedProducts == null)
        {
            return false;
        }
        Debug.Log(autoDetectedProducts.Length);
        foreach(Product p in autoDetectedProducts)
        {
            if (p.name.Equals(productName))
            {
                return true;
            }
        }
        return false;
    }


    //TODO, naming convention changed so the string "brand" is now upc code.
    public ColorDetectable(string brand)
    {
        this.brand = brand;

        StartUp();
    }

    public void StartUp()
    {

        if (brand == null || brand.Equals(""))
        {
            return;
        }
        colorDetector = Camera.main.GetComponent<ColorDetector>();
        detectProducts();

        if (debugging && debugProductCreation)
        {
            Debug.Log("===================================");
            foreach (Product product in autoDetectedProducts)
            {
                Debug.Log("===================================");

                Debug.Log(product.brand);
                Debug.Log(product.name);
                Debug.Log(product.color);

                Debug.Log("===================================");

            }
        }
        
    }

    public string GetProductFromColor(ColorDetector.GeneralizedColor[] colors)
    {
        LinkedList<Product>[] assortedMatches = new LinkedList<Product>[colors.Length + 1];

        //This is fucking stupid but it's required
        for (int i = 0; i < assortedMatches.Length; i++)
        {
            assortedMatches[i] = new LinkedList<Product>();
        }

        foreach (Product p in autoDetectedProducts)
        {
            int matches = p.GetNumOfSameColors(colors);
            assortedMatches[matches].AddLast(p);
        }

        for (int i = assortedMatches.Length - 1; i >= 0; i--)
        {
            if(assortedMatches[i].Count != 0)
            {
                //closest match found
                if(assortedMatches[i].Count > 1)
                {
                    //Solution:
                    Color avg = colorDetector.AverageGeneralizedColors(colors);

                    float distance = -1;
                    Product pickedProduct = null;
                    
                    foreach(Product matchedProduct in assortedMatches[i])
                    {
                        float curDistance = colorDetector.CalculateColorDistance(matchedProduct.color, avg);

                        if(distance == -1 || curDistance < distance)
                        {
                            distance = curDistance;
                            pickedProduct = matchedProduct;
                        }
                    }

                    if (debugging && debugProductRecognition)
                    {
                        Debug.LogError("Too many matches");
                        Debug.Log("----------------------");

                        foreach (ColorDetector.GeneralizedColor seekedOutColors in colors)
                        {
                            Debug.Log("Seeked out Color : " + seekedOutColors);
                        }

                        Debug.Log("Number of Matches : " + i);
                        Debug.Log("Number of Products With That Many Matches : " + assortedMatches[i].Count);

                        foreach (Product matchedProduct in assortedMatches[i])
                        {
                            Debug.Log("Product : " + matchedProduct.name);
                        }

                        Debug.Log("Chosen Product: " + pickedProduct.name);
                        Debug.Log("----------------------");
                    }

                    return pickedProduct.name;
                }

                if(debugging && debugProductRecognition)
                {
                    Debug.Log("Chosen Product: " + assortedMatches[i].First.Value.name);
                }
                return assortedMatches[i].First.Value.name;
            }
        }
        return null;
    }
}

[System.Serializable]
public class Product
{
    public string brand;
    public string name;
    public Color color;
    public ColorDetector.GeneralizedColor[] generalizedColors;

    public Product(string brand, string name, Color color)
    {
        this.brand = brand;
        this.name = name;
        this.color = color;

        ColorNode[] nodes = Camera.main.GetComponent<ColorDetector>().GetThreeClosestNodes(color);
        generalizedColors = Camera.main.GetComponent<ColorDetector>().ConvertColorNodeArrayToGeneralized(nodes);
    }

    public int GetNumOfSameColors(ColorDetector.GeneralizedColor[] gcs)
    {
        int matches = 0;

        foreach(ColorDetector.GeneralizedColor gc in gcs)
        {
            foreach(ColorDetector.GeneralizedColor generalizedColor in generalizedColors)
            {
                if(gc == generalizedColor)
                {
                    matches++;
                }
            }
        }

        return matches;
    }
}