using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Objects
{
    public enum AdGroup
    {
        None,
        SuperAdmin,
        SystemUser,
        //Personal
        PersonalManager,
        NewEmployeeNote,
        // />
        //SpeCalc
        SpeCalcKontroler,
        SpeCalcManager,
        SpeCalcProduct,
        SpeCalcOperator,
        SpeCalcKonkurs,
        // />

        //Service
        ZipClaimClient,
        ZipClaimClientCounterView,
        ZipClaimClientZipView,
        ServiceAdmin,
        ServiceManager,
        ServiceEngeneer,
        ServiceOperator,
        ServiceControler,
        ServiceTech,
        //---доступы
        ServiceClaimClassifier,
        ServiceClaimClientAccess
        // />
    }
}