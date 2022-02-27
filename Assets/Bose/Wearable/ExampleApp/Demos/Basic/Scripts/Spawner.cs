using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject MenuItemPrefab;
    public int Quantity;
    public int Radius = 40;

    // Start is called before the first frame update
    void Start()
    {
        float ratio = 1f / Quantity;

        for (int i = 1-Quantity; i <= Quantity-1; i+=2)
        {
            Vector3 pos = new Vector3(i*ratio*Radius, 0, 15);
            GameObject menuItemTest = Instantiate(MenuItemPrefab, pos, transform.rotation);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
