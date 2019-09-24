using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pipe_s : Product_s
{
    public float length;
    public Pipe_s (Pipe pipe) : base (Product.type_.glassPipe, pipe.uniqueID, pipe.objectID, pipe.subID, pipe.GetName(), pipe.productGO.transform.position, pipe.productGO.transform.eulerAngles)
    {
        length = pipe.length;
    }
}
