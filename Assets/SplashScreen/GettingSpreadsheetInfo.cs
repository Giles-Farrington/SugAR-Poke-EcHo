using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GettingSpreadsheetInfo : MonoBehaviour {

    protected static string GetInfo(string upc, string title)
    {
        TextAsset txt = (TextAsset)Resources.Load("1", typeof(TextAsset));
        string content = txt.text;
        string[] product = content.Split(new char[] { '\n' });  //Split products and save in product array
        List<string> productItem = new List<string>(product[0].Split(new char[] { '\t' }));
        int upcIndex = productItem.IndexOf("UPC");  //Get UPC column index
        int titleIndex = productItem.IndexOf(title);    //Get the column index of what is needed

        for (int i = 1; i < product.Length - 1; i++)
        {
            productItem = new List<string>(product[i].Split(new char[] { '\t' }));
            if (productItem[upcIndex] == upc) return productItem[titleIndex];
        }

        return "Does not exist";
    }
    public static string Brand(string upc)
    {
        return (GetInfo(upc, "Brand"));
    }

    public static string ProductName(string upc)
    {
        return (GetInfo(upc, "Product Name"));
    }
}
