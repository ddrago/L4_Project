using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject MenuItemPrefab;
    public int Quantity;

    // Start is called before the first frame update
    void Start()
    {
        int radius = 20;
        float ratio = 1f / Quantity;
        // Range is:
        //          x: -20 -> 20
        //          y: 0
        for (int i = 0; i < Quantity; i++)
        {
            Vector3 pos = new Vector3(radius*ratio*(i-1), 0, 15);
            print(ratio);
            GameObject menuItemTest = Instantiate(MenuItemPrefab, pos, transform.rotation);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
