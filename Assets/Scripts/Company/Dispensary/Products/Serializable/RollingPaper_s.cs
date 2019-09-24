using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RollingPaper_s : Product_s
{
    public RollingPaper.PaperType paperType;
    public RollingPaper_s(RollingPaper paper) : base (Product.type_.rollingPaper, paper.uniqueID, paper.objectID, paper.subID, paper.GetName(), paper.productGO.transform.position, paper.productGO.transform.eulerAngles)
    {
        paperType = paper.paperType;
    }
}
