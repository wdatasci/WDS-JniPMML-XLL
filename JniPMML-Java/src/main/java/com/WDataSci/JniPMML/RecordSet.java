/* Java >>> */
package com.WDataSci.JniPMML;

import org.dmg.pmml.Field;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

/* <<< Java */
/* C# >>> *


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using com.WDataSci.WDS;
using FieldName = com.WDataSci.JniPMML.FieldName;
using static com.WDataSci.WDS.JavaLikeExtensions;
using static com.WDataSci.WDS.Util;

namespace com.WDataSci.JniPMML
{ 
/* <<< C# */

public class RecordSet
{

    public List<Map<String, Object>> Records = null;
    public List<Object[]> Records_Orig = null;
    public Boolean isInput = false;
    public RecordSet() { }

    public RecordSet(List<Map<String, Object>> _Records)
    {
        this.Records = _Records;
    }

        public void Dispose()
        {
            //Java 
            if ( this.Records != null ) this.Records.clear();
            //C# if ( this.Records != null ) this.Records.Clear();
            this.Records = null;
            //Java 
            if ( this.Records_Orig != null ) this.Records_Orig.clear();
            //C# if ( this.Records_Orig != null ) this.Records_Orig.Clear();
            this.Records_Orig = null;
        }

        /* C# >>> *
        ~RecordSet()
        {
            this.Dispose();
        }
        /* <<< C# */

    public RecordSet cAsInput()
    {
        this.isInput = true;
        if ( this.Records == null )
            //Java 
            this.Records = new ArrayList<>(0);
        //C# this.Records = new List<Map<T,Object>>(0);
        if ( this.Records_Orig == null )
            //Java 
            this.Records_Orig = new ArrayList<>(0);
        //C# this.Records_Orig = new List<Object[]>(0);
        return this;
    }

    public RecordSet cAsOutput()
    {
        this.isInput = false;
        if ( this.Records == null )
            //Java 
            this.Records = new ArrayList<>(0);
        //C# this.Records = new List<Map<T,Object>>(0);
        return this;
    }

    public RecordSet cAsOutput(List<Map<String, Object>> _Records)
    {
        this.isInput = false;
        this.Records = _Records;
        return this;
    }

    public Boolean isEmpty() { return this.Records == null; }

    public RecordSet mReadRecordSet(RecordSetMD aRecordSetMD)
    throws com.WDataSci.WDS.WDSException, Exception
    {

        if ( aRecordSetMD.Mode.equals(RecordSetMDEnums.eMode.Output) )
            throw new com.WDataSci.WDS.WDSException("Error, cannot Load an RecordSet from an RecordSetMD with Output Mode");

        switch ( aRecordSetMD.Type ) {
            case Dlm:
                //case RecordSetMDEnums.eType.Dlm:
            case CSV:
                //case RecordSetMDEnums.eType.CSV:
            case TXT:
                //case RecordSetMDEnums.eType.TXT:
                aRecordSetMD.FileMatter.mReadRecordSet(aRecordSetMD, this, null);
                break;
            case HDF5:
                //case RecordSetMDEnums.eType.HDF5:
                aRecordSetMD.HDF5Matter.mReadRecordSet(aRecordSetMD, this, null);
                break;
            case DBB:
                //case RecordSetMDEnums.eType.DBB:
                aRecordSetMD.DBBMatter.mReadRecordSet(aRecordSetMD, this, null);
                break;
            default:
                break;
        }

        return this;

    }


    public RecordSet mWriteRecordSet(RecordSetMD aOutputRecordSetMD, RecordSetMD aInputRecordSetMD, RecordSet aInputRecordSet)
    throws com.WDataSci.WDS.WDSException, Exception
    {

        if ( aOutputRecordSetMD.Type.bIn(RecordSetMDEnums.eType.CSV, RecordSetMDEnums.eType.TXT, RecordSetMDEnums.eType.Dlm) ) {
            aOutputRecordSetMD.FileMatter.mWriteRecordSet(aOutputRecordSetMD, this, aInputRecordSetMD, aInputRecordSet);
        }
        else if ( aOutputRecordSetMD.Type.bIn(RecordSetMDEnums.eType.HDF5) ) {
            aOutputRecordSetMD.HDF5Matter.mWriteRecordSet(aOutputRecordSetMD, this, aInputRecordSetMD, aInputRecordSet);
        }
        else if ( aOutputRecordSetMD.Type.bIn(RecordSetMDEnums.eType.DBB) ) {
            aOutputRecordSetMD.DBBMatter.mWriteRecordSet(aOutputRecordSetMD, this, aInputRecordSetMD, aInputRecordSet);
        }

        return this;

    }

}

/* C# >>> *
}
/* <<< C# */
