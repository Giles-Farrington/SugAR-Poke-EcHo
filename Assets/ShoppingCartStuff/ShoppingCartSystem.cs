using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShoppingCartSystem : MonoBehaviour {

    public LinkedList<CartItem> cart;
    DataRectifier dataRectifier;
    public GameObject cartPanel,cartList,shoppingCartDisplay;
    public GameObject navigationBar;
    public Text totalTSP;
    Animator anim, navigationBarAnim;

    private Canvas c;
    //public Text debugText;

    //Raycaster things
    GraphicRaycaster gr;
    EventSystem es;

    //Sprite Stuff
    public Image cartUIImage;
    public Sprite cartNotUpdated, cartUpdated;

    bool cartDisplayed;
    bool quanityIconsDisplayed;

    public void CreateNewCartItem(CartItem c)
    {
        GameObject newItem = Instantiate(cartPanel, cartList.transform);

        newItem.transform.Find("BrandText").GetComponent<Text>().text = c.brand;
        newItem.transform.Find("ProductText").GetComponent<Text>().text = c.name;
        //newItem.transform.Find("UPCText").GetComponent<Text>().text = c.upcCode;
        newItem.transform.Find("TSPText").GetComponent<Text>().text = c.numTsp.ToString();
        //newItem.transform.Find("AmountText").GetComponent<Text>().text = c.amountInCart.ToString();
        newItem.name = c.upcCode;
        c.displayer = newItem;
        UpdateCartItem(c);

        newItem.SetActive(true);
        SetUpIncrementButtons(c);
        cart.AddLast(c);

        UpdateTotalTSP();
        //return newItem;
    }

    public void SetUpIncrementButtons(CartItem c)
    {
        Transform parent = c.displayer.transform;
        Transform quantityParent = parent.Find("AmountToggleParent");


        Transform plus = quantityParent.Find("IncreaseAmountButton");
        Transform minus = quantityParent.Find("DecreaseAmountButton");

        Button increase = plus.GetComponent<Button>();
        Button decrease = minus.GetComponent<Button>();

        plus.gameObject.SetActive(false);
        minus.gameObject.SetActive(false);

        Button toggleDisplay = quantityParent.GetComponent<Button>();

        increase.onClick.AddListener( () => { AddItemToCart(c.fileName); });
        decrease.onClick.AddListener(() => { RemoveItemFromCart(c.upcCode); });
        toggleDisplay.onClick.AddListener( () => { ToggleDisplayQuantityControl(parent.gameObject); });
    }

    public void UpdateCartItem(CartItem c)
    {
        Transform itemEntry = c.displayer.transform;

        if(c.amountInCart <= 0)
        {
            cart.Remove(c);
            c.displayer.GetComponent<Animator>().SetBool("beingDestroyed", true);
            Destroy(itemEntry.gameObject,.5f);
            UpdateTotalTSP();
            return;
        }
        Transform amountParent = itemEntry.transform.Find("AmountToggleParent");
        amountParent.Find("AmountText").GetComponent<Text>().text = c.amountInCart.ToString();
        UpdateTotalTSP();

        if(cartDisplayed == false)
        {
            cartUIImage.sprite = cartUpdated;
        }
    }

    public void ToggleDisplayCart(bool setBool)
    {
        cartDisplayed = setBool;
        anim.SetBool("cartDisplayed", cartDisplayed);
        navigationBarAnim.SetBool("hidden",cartDisplayed);

        if (cartDisplayed == true)
        {
            cartUIImage.sprite = cartNotUpdated;
        }
    }

    public void ToggleDisplayQuantityControl(GameObject item)
    {
        Animator a = item.GetComponent<Animator>();
        Transform parent = item.transform.Find("AmountToggleParent");
        Transform plus = parent.Find("IncreaseAmountButton");
        Transform minus = parent.Find("DecreaseAmountButton");

        Debug.Log("Quantity Toggled");
        quanityIconsDisplayed = !quanityIconsDisplayed;

        a.SetBool("quanityIconsDisplayed", quanityIconsDisplayed);

        if (!quanityIconsDisplayed)
        {
            StartCoroutine(ToggleObjects(.3f, plus.gameObject, minus.gameObject, false));
        }
        else
        {
            plus.gameObject.SetActive(true);
            minus.gameObject.SetActive(true);
        }
    }

    IEnumerator ToggleObjects(float time, GameObject a, GameObject b, bool state)
    {
        Debug.Log("MEME");
        yield return new WaitForSeconds(time);

        Debug.Log("MEMEMEMEMEE");
        a.SetActive(state);
        b.SetActive(state);
    }

    void ResetCart()
    {
        cart = new LinkedList<CartItem>();
    }

    public void AddCurrentViewedItem()
    {
        string fileName = dataRectifier.correctedFoodName;
        //debugText.text = "Adding " + fileName;
        //Debug.Log("File Name: " + fileName + " was added to your shopping cart");

        string upcCode = GetUpcCode(fileName);

        //makes sure only one tap adds item to cart
        foreach (CartItem item in cart)
        {
            if (item.IsEqual(upcCode))
            {
                return;
            }
        }

        AddItemToCart(fileName);
        //DebugCart();
    }

    void AddItemToCart(string fileName)
    {
        string upcCode = GetUpcCode(fileName);
        //debugText.text += " | UPC: " + upcCode;

        foreach (CartItem item in cart)
        {
            if (item.IsEqual(upcCode))
            {
                item.IncreaseAmount();
                UpdateCartItem(item);
                return;
            }
        }
        //Problem
        string brand = GettingSpreadsheetInfo.Brand(upcCode);
        //debugText.text += " | Brand: " + brand;
        string productName = GettingSpreadsheetInfo.ProductName(upcCode);
        //debugText.text += " | Product Name: " + productName;
        int numTsp = GetNumTsp(fileName);
        //debugText.text += " | Tsp: " + numTsp;
        CartItem newItem = new CartItem(fileName, brand, productName, upcCode, numTsp);
        CreateNewCartItem(newItem);
    }

    public void RemoveItemFromCart(string upcCode)
    {

        foreach(CartItem item in cart)
        {
            if (item.IsEqual(upcCode))
            {
                item.DecreaseAmount();

                UpdateCartItem(item);

                break;
            }
        }
    }

    public string GetUpcCode(string fileName)
    {
        string productName = "";

        foreach(char c in fileName)
        {
            if (System.Char.IsDigit(c))
            {
                productName += c;
            }
            else
            {
                break;
            }
        }

        return productName;
    }

    public int GetNumTsp(string fileName)
    {
        int startingIndex = fileName.LastIndexOf("-") + 1;
        int num = int.Parse(fileName.Substring(startingIndex, fileName.Length - startingIndex));

        return num;
    }

    public void CaptureScreenTap()
    {
        if (Input.touchCount != 0 || Input.GetMouseButtonDown(0))
        {
            //debugText.text = "";

            Vector2 pos;

            float slidingDown;

#if UNITY_ANDROID && !UNITY_EDITOR
            //Android
            Debug.Log("On Android");
            pos = Input.GetTouch(0).position;
            slidingDown = Input.GetTouch(0).deltaPosition.y;
            //debugText.text += "Android Build";
            //debugText.text += " | Num Fingers " + Input.touchCount;
#elif UNITY_EDITOR
            //Unity Editor
            pos = Input.mousePosition;
            slidingDown = Input.GetAxis("Mouse Y");
            //debugText.text += "Unity Editor";
#endif

            //debugText.text += " | position: " + pos;

            Ray r = Camera.main.ScreenPointToRay(pos); //TODO check it works for mobile
            RaycastHit hit;
            if(Physics.Raycast(r,out hit))
            {
                Debug.Log(hit.transform.name);
                //debugText.text += " | hit " + hit.transform.name;
                if (hit.transform.name.Equals("TeaspoonCounter 2 1")){
                    AddCurrentViewedItem();
                    //TODO Add some animation or something for on click
                }
            }
            else
            {
                //debugText.text += " | No Physical Object Detected";
                PointerEventData pointerEventData = new PointerEventData(es);
                pointerEventData.position = pos;
                List<RaycastResult> results = new List<RaycastResult>();

                gr.Raycast(pointerEventData, results);

                foreach(RaycastResult rr in results)
                {
                    if (rr.gameObject.name.Equals("DraggableCartPanel"))
                    {
                        Debug.Log("HEH" + slidingDown);
                        //debugText.text += " | GUI Element Deteceted";
                        if (slidingDown < 0)
                        {
                            //swiping down TODO check and make sure delaposition is right
                            Debug.Log("LOL");
                            ToggleDisplayCart(false);
                        }
                    }
                }
            }
        }
    }

    public void UpdateTotalTSP()
    {
        int numTotalTSP = 0;

        foreach(CartItem c in cart)
        {
            numTotalTSP += (c.numTsp * c.amountInCart);
        }

        totalTSP.text = numTotalTSP.ToString();
    }

    private void Start()
    {
        c = GameObject.Find("Canvas").GetComponent<Canvas>();
        gr = c.GetComponent<GraphicRaycaster>();
        es = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        quanityIconsDisplayed = false;
        dataRectifier = Camera.main.GetComponent<DataRectifier>();
        cart = new LinkedList<CartItem>();
        anim = shoppingCartDisplay.GetComponent<Animator>();
        navigationBarAnim = navigationBar.GetComponent<Animator>();
        cartDisplayed = false;
        UpdateTotalTSP();
    }

    private void Update()
    {
        CaptureScreenTap();
    }

    void DebugCart()
    {
        foreach(CartItem c in cart)
        {
            Debug.Log("==============");
            Debug.Log("UPC: " + c.upcCode);
            Debug.Log("Brand: " + c.brand);
            Debug.Log("Name: " + c.name);
            Debug.Log("Tsp: " + c.numTsp);
            Debug.Log("Amount: " + c.amountInCart);
            Debug.Log("==============");
        }
    }
}

public class CartItem
{
    public string fileName;
    public string brand, name, upcCode;
    public int numTsp;
    public int amountInCart;
    public GameObject displayer;

    public CartItem(string fileName, string brand, string name, string upcCode, int numTsp)
    {
        this.fileName = fileName;
        this.brand = brand;
        this.name = name;
        this.upcCode = upcCode;
        this.numTsp = numTsp;
        this.amountInCart = 1;
    }

    public bool IsEqual(string upc)
    {
        if (this.upcCode.Equals(upc))
        {
            return true;
        }
        return false;
    }

    public void IncreaseAmount()
    {
        amountInCart++;
    }

    //Returns true if amount hits zero
    public bool DecreaseAmount()
    {
        amountInCart--;

        if(amountInCart == 0)
        {
            return true;
        }

        return false;
    }
}

