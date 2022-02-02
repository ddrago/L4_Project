using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{

    [SerializeField]
    private static Ray _ray;

    private static RaycastHit _hitInfo;

    public static void Test_answer()
    {
        if (Physics.Raycast(_ray, out _hitInfo, 100))
        {
            Debug.Log("I'm seeing something!");
        }
        else
        {
            Debug.Log("Oh no! I'm nodding at nothing!");
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
        }
        else
        {
            Debug.DrawLine(_ray.origin, _ray.origin + _ray.direction * 100, Color.red);
        }
    }
}
