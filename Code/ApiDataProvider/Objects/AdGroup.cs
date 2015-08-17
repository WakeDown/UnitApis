using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Objects
{
    public enum AdGroup
    {
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
        ServiceAdmin,
        ServiceManager,
        ServiceEngeneer,
        ServiceOperator,
        ServiceControler
        // />
    }
}