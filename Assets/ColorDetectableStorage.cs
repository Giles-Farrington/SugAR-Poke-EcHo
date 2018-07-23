using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorDetectableStorage : MonoBehaviour {

    [Header("Insert Brands Below")]
    [Tooltip("When typing in brands, make sure the spelling is correct and that each brand gets it own entry box")]
    public string[] inputBrand;
    [Header("Debugging Tools Below")]
    [SerializeField]
    ColorDetectable[] colorDetectedBrands;

	public void AddBrands()
    {
        colorDetectedBrands = new ColorDetectable[inputBrand.Length];
        for (int i = 0; i < inputBrand.Length; i++)
        {
            ColorDetectable cd = new ColorDetectable(inputBrand[i]);
            colorDetectedBrands[i] = cd;
        }
    }

    /// <summary>
    /// Returns the index of the brand supplied or -1 if it's not there.
    /// </summary>
    /// <param name="brand"></param>
    /// <returns>The index</returns>
    public int getIndexOfBrand(string brand)
    {
        for (int i = 0; i < colorDetectedBrands.Length; i++)
        {
            if (colorDetectedBrands[i].brand.Equals(brand))
            {
                return i;
            }
        }
        return -1;
    }

    public string getCorrectProduct(string brand, ColorDetector.GeneralizedColor[] colors)
    {
        int index = getIndexOfBrand(brand);

        if(index == -1)
        {
            Debug.LogError("Brand Not Registered");
            return null;
        }

        string productName = colorDetectedBrands[index].GetProductFromColor(colors);
        return productName;
    }

    private void Start()
    {
        AddBrands();
    }
}
