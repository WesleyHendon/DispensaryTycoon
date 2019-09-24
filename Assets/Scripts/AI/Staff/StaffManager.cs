using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StaffManager : MonoBehaviour
{
    public StaffSelectionDisplay staffCircle;

    public Database db;
    public DispensaryManager dm;
    public GameObject staffParent; // staff are children of this while going to and from the store
    public List<GameObject> staffSpawnLocations = new List<GameObject>();

    public bool staffSelected;
    public Staff selectedStaff;

    public GameObject staffModel;
    public StaffSelectionDisplay currentStaffCircle;
    public Image staffDisplayPanel;


    public List<Staff> activeStaff = new List<Staff>(); // All active staff
    public List<Staff_s> allStaff = new List<Staff_s>();

    void Start()
    {
        try
        {
            staffParent = new GameObject("Staff");
            db = GameObject.Find("Database").GetComponent<Database>();
            dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        }
        catch (NullReferenceException)
        {

        }
    }

    public void HireStaff()
    {
        dm.dispensary.HireNewStaff();
    }

    public Queue<StaffAIAction> queuedActions = new Queue<StaffAIAction>();

    public void AddActionToQueue(StaffAIAction newAction)
    {
        queuedActions.Enqueue(newAction);
        TryToAssignStaff();
    }

    public void TryToAssignStaff()
    {
        if (queuedActions.Count > 0)
        {
            StaffAIAction toAssign = queuedActions.Dequeue();
            switch (toAssign.preferredJob)
            {
                case Dispensary.JobType.StoreBudtender:
                    Staff staff = GetStoreBudtender();
                    //print(staff.parentStaff.staffName);
                    if (staff != null)
                    {
                        staff.AssignAction(toAssign);
                    }
                    else
                    {
                        StartCoroutine(TryAgain(toAssign));
                    }
                    break;
            }
        }
    }

    float currentTry = 0;
    float maxNumberOfTries = 4;

    IEnumerator TryAgain(StaffAIAction action)
    {
        yield return new WaitForSecondsRealtime(.5f);
        if (!(currentTry + 1 > maxNumberOfTries))
        {
            currentTry++;
            TryToAssignStaff();
        }
        else
        {
            currentTry = 0;
            CancelAction(action);
        }
    }

    public void CancelAction(StaffAIAction action)
    {
        action.CancelAction();
    }

    public Staff GetCashier()
    {
        List<Staff> cashiers = new List<Staff>();
        foreach (Staff staff in dm.dispensary.activeStaff)
        {
            if (staff.HasJob())
            {
                if (staff.job.jobName == "Cashier")
                {
                    cashiers.Add(staff);
                }
            }
        }
        int randomVal = UnityEngine.Random.Range(0, cashiers.Count - 1);
        if (cashiers.Count > 0)
        {
            return cashiers[randomVal];
        }
        return null;
    }

    public Staff GetStoreBudtender()
    {
        List<Staff> storeBudtenders = new List<Staff>();
        foreach (Staff staff in dm.dispensary.activeStaff)
        {
            if (staff.HasJob())
            {
                if (staff.job.jobName == "Store Budtender")
                {
                    storeBudtenders.Add(staff);
                }
            }
        }
        int randomVal = UnityEngine.Random.Range(0, storeBudtenders.Count - 1);
        if (storeBudtenders.Count > 0)
        {
            return storeBudtenders[randomVal];
        }
        return null;
    }

    public Staff GetSmokeBudtender()
    {
        List<Staff> smokeBudtenders = new List<Staff>();
        foreach (Staff staff in dm.dispensary.activeStaff)
        {
            if (staff.HasJob())
            {
                if (staff.job.jobName == "Smoke Budtender")
                {
                    smokeBudtenders.Add(staff);
                }
            }
        }
        int randomVal = UnityEngine.Random.Range(0, smokeBudtenders.Count - 1);
        if (smokeBudtenders.Count > 0)
        {
            return smokeBudtenders[randomVal];
        }
        return null;
    }

    /*public Staff SpawnStaff(Staff_s staffToSpawn)
    {
        GameObject newStaffGO = Instantiate(staffModel);
        newStaffGO.transform.parent = staffParent.transform;
        Staff newStaff = newStaffGO.GetComponent<Staff>();
        newStaff.staffName = staffToSpawn.staffName;
        newStaff.jobType = staffToSpawn.jobType;
        newStaff.CreateJob();
        //newStaff.job = GetJob(staffToSpawn.jobName);
        //newStaff.job.AssignStaff(newStaff);
        newStaff.payRate = staffToSpawn.payRate;
        newStaff.satisfaction = staffToSpawn.satisfaction;
        newStaff.timeStartingWorking = staffToSpawn.timeStartingWorking;
        activeStaff.Add(newStaff);
        newStaff.currentStatus = StaffPathfinding.StaffAction.enteringStore;
        return newStaff;
    }*/

    /*public Job GetJob(string jobName)
    {
        /*switch (jobName)
        {
            case "None":
                return new NoJob();
            case "Budtender":
                return dm.dispensary.GetJob(jobName);
            case "Cashier":
                return dm.dispensary.GetJob(jobName);
        }
        return new NoJob();* /
        return dm.dispensary.GetJob(jobName);
    }*/

    /*public void SetStaffInactive(Staff staffIn)
    {
        List<Staff> newActiveStaff = new List<Staff>();
        Staff toRemove = null;
        foreach (Staff staff in activeStaff)
        {
            if (!staff.staffName.Equals(staffIn.staffName))
            {
                newActiveStaff.Add(staff);
            }
            else
            {
                toRemove = staff;
            }
        }
        if (toRemove != null)
        {
            Destroy(toRemove);
        }
        activeStaff = newActiveStaff;
    }*/

    /* public void HireStaffMember()
     {
         float randomTwoGender_value = UnityEngine.Random.value;
         bool male = true;
         if (randomTwoGender_value > .5)
         {
             male = true;
         }
         else
         {
             male = false;
         }
         string name = (male) ? db.GetRandomFullName(true) : db.GetRandomFullName(false);
         Staff_s newStaff_s = new Staff_s(name, male, "None", Dispensary.JobType.None, 0.0f, 0.0f, Time.time);
         Staff newStaff = SpawnStaff(newStaff_s);
         float distance = 10000;
         Vector3 spawnLocation = Vector3.zero;
         foreach (GameObject obj in staffSpawnLocations)
         {
             Dispensary dispensary = GameObject.Find("Dispensary").GetComponent<Dispensary>();
             float newDistance = Vector3.Distance(obj.transform.position, dispensary.Main_c.GetRandomEntryDoor().transform.position);
             if (newDistance < distance)
             {
                 distance = newDistance;
                 spawnLocation = obj.transform.position;
             }
         }
         newStaff.transform.position = spawnLocation;
         Staff newStaffComponent = newStaff.GetComponent<Staff>();
         newStaffComponent.staffName = name;
         newStaffComponent.OnSpawn();
         dm.uiManagerObject.GetComponent<UIManager_v5>().dispensaryWindow.CreateList_StaffWindow(string.Empty);
         //dm.uiManagerObject.GetComponent<UIManager_v3>().staffPanel.GetComponent<StaffUIPanel>().CreateList();
     }*/

    /*public void SelectStaffMember(Staff staffToSelect)
    {
        if (!staffToSelect.selected)
        {
            foreach (Staff staff_ in activeStaff)
            {
                if (staff_.selected)
                {
                    staff_.HighlightOff();
                }
                staff_.selected = false;
            }
            staffToSelect.HighlightOn(Color.magenta);
            staffToSelect.selected = true;
            selectedStaff = staffToSelect;
            staffSelected = true;
            currentStaffCircle = Instantiate(staffCircle);
            currentStaffCircle.staff = staffToSelect;
        }
    }

    public void DeselectStaffMember(Staff toDeselect)
    {
        if (toDeselect.selected)
        {
            toDeselect.selected = false;
            toDeselect.HighlightOff();
            selectedStaff = null;
            staffDisplayPanel.gameObject.SetActive(false);
            Destroy(currentStaffCircle.gameObject);

        }
    }*/

    /*public void AssignActionToStaff(StaffAIAction action, bool ignorePreferred)
    {
        List<Staff> applicableStaff = new List<Staff>();
        foreach (Staff staff in activeStaff)
        {
            if (staff.job != null)
            {
                if (!ignorePreferred)
                {
                    if (staff.job.jobName == action.preferredJob)
                    {
                        applicableStaff.Add(staff);
                    }
                }
                else
                {
                    if (staff.job.jobName == "Cashier")
                    {
                        applicableStaff.Add(staff);
                    }
                }
            }
        }
        if (applicableStaff.Count > 0)
        {
            int rand = UnityEngine.Random.Range(0, applicableStaff.Count - 1);
            applicableStaff[rand].AssignAction(action);
        }
        else
        {
            if (!ignorePreferred) // If this run was already ignoring, cancel and notify user of error
            {
                AssignActionToStaff(action, true);
            }
            else
            {
                dm.notificationManager.AddToQueue("No staff available", NotificationManager.NotificationType.problem);
            }
        }
    }

    public void FireStaffMember(Staff staff_)
    {
        List<Staff> newList = new List<Staff>();
        foreach (Staff emp in activeStaff)
        {
            if (staff_ != emp)
            {
                newList.Add(emp);
            }
        }
        activeStaff = newList;
        staff_.SetAction(StaffPathfinding.StaffAction.leavingStore);
    }*/
}
