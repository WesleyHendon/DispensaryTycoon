using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Job
{
    public Staff staff;
    public string jobName;
    public string jobDescription;

    public Job(Staff activeStaff)
    {
        staff = activeStaff;
    }

    public void Print(string item)
    {
        GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>().Tempprint(item);
    }

    public abstract void OnEnterStore();
}
