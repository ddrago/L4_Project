using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject MenuItemPrefab;
    public int Quantity;
    public int Radius = 40;

    public enum Layout
    {
        horizontal,
        vertical,
        pie
    };

    //Create the variable field
    [SerializeField] public Layout layout;

    public void CreateMenuLayout()
    {
        float ratio = 1f / Quantity;
        float itemSize = (Radius * 2) / Quantity;

        int nameNum = 1;

        switch (layout)
        {
            case Layout.horizontal:

                for (int i = 1 - Quantity; i <= Quantity - 1; i += 2)
                {
                    Vector3 pos = new Vector3(i * ratio * Radius, 0, 15);
                    GameObject menuItemTest = Instantiate(MenuItemPrefab, pos, transform.rotation);

                    menuItemTest.transform.localScale = new Vector3(itemSize, Radius, 1);
                    menuItemTest.name = "Item" + nameNum;
                    nameNum++;
                }
                break;

            case Layout.vertical:

                for (int i = Quantity - 1; i >= 1 - Quantity; i -= 2)
                {
                    Vector3 pos = new Vector3(0, i * ratio * Radius, 15);
                    GameObject menuItemTest = Instantiate(MenuItemPrefab, pos, transform.rotation);

                    menuItemTest.transform.localScale = new Vector3(Radius, itemSize, 1);
                    menuItemTest.name = "Item" + nameNum;
                    nameNum++;
                }
                break;

            case Layout.pie:

                for (int i = 0; i < Quantity; ++i)
                {
                    float theta = (-2 * Mathf.PI / Quantity) * i; //the - helps make the order of the items be clock-wise
                    theta += Mathf.PI/4;  // shift the angle by 45 degrees counter-clockwise
                    print("theta: " + theta);
                    
                    float x = Mathf.Cos(theta);
                    float y = Mathf.Sin(theta);
                    float r = Radius / (Mathf.Sqrt(2));
                    print("x: " + x);
                    print("y: " + y);


                    Vector3 pos = new Vector3(x * r, y * r, 15);
                    GameObject menuItemTest = Instantiate(MenuItemPrefab, pos, transform.rotation);

                    menuItemTest.transform.localScale = new Vector3(Radius, Radius, 1);
                    menuItemTest.name = "Item" + nameNum;
                    nameNum++;
                }
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateMenuLayout();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
