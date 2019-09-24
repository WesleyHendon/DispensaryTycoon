using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CashRegister : MonoBehaviour
{
    public GameObject staffPosition; // position for staff attending to the register
    int money; // cash in register
    public Staff assigned; // staff thats assigned here
}
