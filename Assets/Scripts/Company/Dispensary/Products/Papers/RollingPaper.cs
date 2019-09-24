using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingPaper : Product
{
    public enum PaperType
    {
        smallJoint,
        mediumJoint,
        largeJoint,
        kingJoint,
        smallCone,
        mediumCone,
        largeCone,
        kingCone,
        bluntWrap // packs of cigars
    }

    public static PaperType GetRandomPaperType()
    {
        int rand = UnityEngine.Random.Range(0, 8);
        switch (rand)
        {
            case 0:
                return PaperType.smallJoint;
            case 1:
                return PaperType.mediumJoint;
            case 2:
                return PaperType.largeJoint;
            case 3:
                return PaperType.kingJoint;
            case 4:
                return PaperType.smallCone;
            case 5:
                return PaperType.mediumCone;
            case 6:
                return PaperType.largeCone;
            case 7:
                return PaperType.kingCone;
            case 8:
                return PaperType.bluntWrap;
        }
        return GetRandomPaperType();
    }

    public GameObject paperGO;
    public PaperType paperType;

    public RollingPaper(StoreObjectReference objectReference, PaperType paperType_, GameObject createdObject) : base(objectReference, createdObject, type_.rollingPaper)
    {
        paperGO = createdObject;
        paperType = paperType_;
    }

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos)
    {

    }

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos, StoreObjectFunction_DisplayShelf displayShelf)
    {

    }

    public override string GetName()
    {
        return "";
    }

    public override float GetQuantity()
    {
        return 1;
    }

    public override string GetQuantityString()
    {
        return "1";
    }

    public override Product_s MakeSerializable()
    {
        return new RollingPaper_s(this);
    }
}
