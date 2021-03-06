using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    [SerializeField]
    private static Ray _ray;
    private RaycastHit _hitInfo;

    public void menuItem1_answer()
    {
        if (Physics.Raycast(_ray, out _hitInfo, 100))
        {
            Debug.Log("1: Yoooo watchya lookin at");
        }
    }
    
    public void menuItem2_answer()
    {
        if (Physics.Raycast(_ray, out _hitInfo, 100))
        {
            Debug.Log("2: Oy oy oy oy >:(");
        }
    }

    public void test_answer()
    {
        if (Physics.Raycast(_ray, out _hitInfo, 100))
        {
            Debug.Log("test 1");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        _ray = ray;

        if (Physics.Raycast(_ray, out _hitInfo, 100))
        {
            Debug.DrawLine(_ray.origin, _hitInfo.point, Color.green);
        } else
        {
            Debug.DrawLine(_ray.origin, _ray.origin + _ray.direction * 100, Color.red);
        }
    }
}
