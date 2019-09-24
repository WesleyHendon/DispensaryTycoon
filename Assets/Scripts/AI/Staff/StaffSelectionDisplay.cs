using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffSelectionDisplay : MonoBehaviour
{
    public Staff staff;

    void Start()
    {
        transform.position = new Vector3(staff.transform.position.x, .05f, staff.transform.position.z);
    }

    void Update()
    {
        Vector3 pos = new Vector3(staff.transform.position.x, .05f, staff.transform.position.z);
        transform.position = pos;
        /*if (!staff.selected)
        {
            Destroy(this.gameObject);
        }*/
    }
}
