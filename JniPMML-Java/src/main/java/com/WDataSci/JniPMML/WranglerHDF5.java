/* Java >>> */
package com.WDataSci.JniPMML;

import com.WDataSci.WDS.WDSException;

import com.sun.istack.Nullable;
import hdf.object.Attribute;
import hdf.object.Datatype;
import hdf.object.FileFormat;
import hdf.object.h5.H5CompoundDS;
import hdf.object.h5.H5File;
import org.dmg.pmml.FieldName;

import java.io.PrintWriter;
import java.util.*;

import static com.WDataSci.WDS.Util.PathAndName;


/* <<< Java */
/* C# >>> *
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.CompilerServices;


using ExcelDna.Integration.CustomUI;
using ExcelDna.Integration;
using ExcelDna.Utilities;

using MOIE = Microsoft.Office.Interop.Excel;

using HDF.PInvoke;
using static HDF.PInvoke.H5A;

using com.WDataSci.WDS;
using static com.WDataSci.WDS.JavaLikeExtensions;
using static com.WDataSci.WDS.Util;
using static com.WDataSci.JniPMML.Util;

using FieldName = com.WDataSci.JniPMML.FieldName;

namespace com.WDataSci.JniPMML
{
/* <<< C# */


    public class WranglerHDF5
    {

        /* Java >>> */
        public H5File File = null;
        public H5CompoundDS CompoundDS = null;
        /* <<< Java */
        /* C# >>> *
           public Int64 File = 0;
        //public H5File File = null;
        public Int64 CompoundDS = 0;
        /* <<< C# */

        public String DSName = null;
        public boolean bIsInMemory = false;

        public WranglerHDF5()
        throws com.WDataSci.WDS.WDSException, Exception
        {}

        /* Java >>> */
        public void Dispose()
        throws com.WDataSci.WDS.WDSException
        {
            try {
                this.File.close();
            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error finalizing Wrangler for HDF5", e);
            }
        }

        protected void finalize()
        throws com.WDataSci.WDS.WDSException
        {
            this.Dispose();
        }
        /* <<< Java */
        /* C# >>> *
           public void Dispose()
           {
           try {
           H5D.close(this.CompoundDS);
           H5F.close(this.File);
           this.File = 0;
           this.CompoundDS = 0;
           this.DSName = null;
           this.bIsInMemory = false;
           }
           catch ( Exception e ) {
           throw new com.WDataSci.WDS.WDSException("Error finalizing Wrangler for HDF5", e);
           }
           }
           ~WranglerHDF5()
           {
           this.Dispose();
           }
        /* <<< C# */

        //Java
        @SuppressWarnings( "SynchronizationOnLocalVariableOrMethodParameter" )
        public void mReadMapFor(RecordSetMD aRecordSetMD, JniPMMLItem aJniPMML, PrintWriter pw, boolean bFillDictionaryNames)
        throws com.WDataSci.WDS.WDSException
        {
            //Java
            synchronized ( aRecordSetMD ) {
                try {

                    int i = -1, j = -1, k = -1;
                    //Java
                    int ii = -1, jj = -1, kk = -1;
                    //C# uint ii=-1,jj=-1,kk=-1;

                    //are we using a JniPMML object (as when called from C# and does it have PMMLMatter
                    boolean bUsingJniPMML = (aJniPMML != null);
                    boolean bCheckingAgainstPMML = (aJniPMML != null && aJniPMML.PMMLMatter.Doc != null);

                    String[] lFieldStringNames = null;
                    int nDataFieldNames = 0;

                    if ( bCheckingAgainstPMML ) {
                        lFieldStringNames = aJniPMML.PMMLDataFieldStringNames();
                        nDataFieldNames = lFieldStringNames.length;
                    }

                    if ( aRecordSetMD.FileMatter.FileName == null )
                        throw new WDSException("Error, RecordSetMD needs populated PMML and HDF5FileName");

                    if ( aRecordSetMD.DBBMatter == null )
                        aRecordSetMD.DBBMatter = new WranglerDBB();

                    //point to file or memory
                    try {
                        if ( aRecordSetMD.HDF5Matter == null ) aRecordSetMD.HDF5Matter = new WranglerHDF5();
                        String aPathAndName = PathAndName(aRecordSetMD.FileMatter.Path, aRecordSetMD.FileMatter.FileName);
                        //Java
                        aRecordSetMD.HDF5Matter.File = new hdf.object.h5.H5File(aPathAndName, hdf.object.h5.H5File.READ);
                        //Java
                        aRecordSetMD.HDF5Matter.File.open();
                        //C# aRecordSetMD.HDF5Matter.File = H5F.open(aPathAndName, H5F.ACC_RDONLY);
                    }
                    /* Java >>> */
                    catch ( Throwable e )
                        /* <<< Java */
                        /* C# >>> *
                           catch ( Exception e )
                        /* <<< C# */ {
                        throw new WDSException("Error, failed to open " + aRecordSetMD.FileMatter.FileName + ":", e);
                    }


                    //get compound dataset information
                    String hDSPath = aRecordSetMD.HDF5Matter.DSName;
                    if ( hDSPath == null || hDSPath.isEmpty() || hDSPath.equals("/") ) {
                        /* Java >>> */
                        hDSPath = aRecordSetMD.HDF5Matter.File.getRootObject().getName();
                        aRecordSetMD.HDF5Matter.DSName = hDSPath;
                        /* <<< Java */
                        /* C# >>> *
                        //Ugh, the HDF5 Java vs C# is what happens when two interfaces to same underlying objects are written by different people

                        //walk the root, make sure there is only one dataset or group and take that name
                        System.Int64 hRG = 0;
                        hRG = H5G.open(aRecordSetMD.HDF5Matter.File, "/");

                        List<String> datasetNames = new List<String>();
                        List<String> groupNames = new List<String>();

                        //CodeRef CJW: Hint https://github.com/HDFGroup/HDF.PInvoke/wiki/Cookbook-:-Iteration
                        H5L.iterate_t ldaf = delegate (long objid, IntPtr namePtr, ref H5L.info_t info, IntPtr op_data) {
                        String objectName = Marshal.PtrToStringAnsi(namePtr);
                        H5O.info_t gInfo = new H5O.info_t();
                        H5O.get_info_by_name(objid, objectName, ref gInfo);
                        if ( gInfo.type == H5O.type_t.DATASET ) {
                        datasetNames.Add(objectName);
                        }
                        else if ( gInfo.type == H5O.type_t.GROUP ) {
                        groupNames.Add(objectName);
                        }
                        return 0;
                        };

                        System.UInt64 xxx = 0;
                        H5L.iterate(hRG, H5.index_t.NAME, H5.iter_order_t.INC, ref xxx, ldaf, (IntPtr) null);
                        if ( datasetNames.Count != 1 || groupNames.Count > 0 ) {
                        MessageBox.Show("Without a data set path, there is more than one data set and or has sub groups, please rerun and input path\n"
                        + datasetNames.Count + " data sets\n" 
                        + groupNames.Count + " sub groups of / ");
                        }
                        hDSPath = datasetNames[0];
                        aRecordSetMD.HDF5Matter.DSName = hDSPath;
                        /* <<< C# */
                    }

                    try {
                        //Java
                        aRecordSetMD.HDF5Matter.CompoundDS = new H5CompoundDS(aRecordSetMD.HDF5Matter.File, aRecordSetMD.HDF5Matter.DSName, "/");
                        //Java
                        aRecordSetMD.HDF5Matter.CompoundDS.read();
                        //C# aRecordSetMD.HDF5Matter.CompoundDS = H5D.open(aRecordSetMD.HDF5Matter.File, hDSPath);
                    }
                    catch ( Exception e ) {
                        throw new WDSException("Error opening data set" + hDSPath + ":", e);
                    }

                    /* Java >>> */
                    int nColumns = aRecordSetMD.HDF5Matter.CompoundDS.getMemberCount();
                    long hTypSize = aRecordSetMD.HDF5Matter.CompoundDS.getWidth();
                    String[] lNames = aRecordSetMD.HDF5Matter.CompoundDS.getMemberNames();

                    hdf.object.Datatype[] hTypes = aRecordSetMD.HDF5Matter.CompoundDS.getMemberTypes();
                    aRecordSetMD.Column = new FieldMD[nColumns];
                    for ( jj = 0; jj < nColumns; jj++ ) {
                        aRecordSetMD.Column[jj] = new FieldMD();
                        FieldMD cm = aRecordSetMD.Column[jj];
                        cm.Name = lNames[jj];
                        if ( bCheckingAgainstPMML ) {
                            //Search for PMML DataFieldName map
                            for ( j = 0; j < nDataFieldNames; j++ ) {
                                if ( cm.Name.equals(lFieldStringNames[j]) ) {
                                    cm.MapToMapKey(lFieldStringNames[j]);
                                    break;
                                }
                            }
                        }
                        else if ( bFillDictionaryNames ) {
                            cm.MapToMapKey(cm.Name);
                        }
                    }
                    List<Attribute> metadata = null;
                    boolean bUsesAttribues = false;
                    if ( aRecordSetMD.HDF5Matter.CompoundDS.hasAttribute() ) {
                        metadata = aRecordSetMD.HDF5Matter.CompoundDS.getMetadata();
                        bUsesAttribues = (metadata != null && metadata.size() > 0);
                        int natt = 0;
                        List<String> attNames = new ArrayList<>();
                        if ( bUsesAttribues ) {
                            natt = metadata.size();
                            for ( i = 0; i < natt; i++ )
                                attNames.add(metadata.get(i).getName());
                        }
                    }
                    /* <<< Java */


                    /* C# >>> *
                       Boolean bUsesAttribues = false;

                       H5O.info_t tmpinfo = new H5O.info_t();
                       H5O.get_info(aRecordSetMD.HDF5Matter.CompoundDS, ref tmpinfo);
                       List<String> attNames = new List<String>(0);
                       List<Object> attValues = new List<Object>(0);
                       if ( tmpinfo.num_attrs > 0 ) {

                       H5A.operator_t op = delegate(long location_id, IntPtr attr_name, ref info_t ainfo, IntPtr op_data){
                       String objectName = Marshal.PtrToStringAnsi(attr_name);
                       System.Int64 attid = H5A.open(location_id, objectName);
                       System.Int64 atttype = H5A.get_type(attid);
                       H5T.class_t atttypemem = (H5T.class_t) H5T.get_native_type(atttype, H5T.direction_t.ASCEND);
                       System.Int64 attspace = H5A.get_space(attid);
                       System.UInt64[] sdim = new System.UInt64[64];
                       int rank = H5S.get_simple_extent_ndims(attspace);
                       int ret = H5S.get_simple_extent_dims(attspace, sdim, null);
                       if ( rank == 1 && sdim[0] == 1 ) {
                       IntPtr attlen = H5T.get_size(attid);
                       byte[] buf = new byte[(int) attlen];
                       GCHandle bufpin = GCHandle.Alloc(buf, GCHandleType.Pinned);
                       H5A.read(attid, (long) atttypemem, bufpin.AddrOfPinnedObject());
                       switch ( atttypemem ) {
                       case H5T.class_t.STRING:
                       attValues.Add(System.Text.ASCIIEncoding.ASCII.GetString(buf));
                       attNames.Add(objectName);
                       break;
                       case H5T.class_t.FLOAT:
                       attValues.Add(BitConverter.ToDouble(buf, 0));
                       attNames.Add(objectName);
                       break;
                       case H5T.class_t.INTEGER:
                       if ( (int) attlen == 2 )
                       attValues.Add(BitConverter.ToInt16(buf, 0));
                       else if ( (int) attlen == 4 )
                       attValues.Add(BitConverter.ToInt32(buf, 0));
                       else if ( (int) attlen == 8 )
                       attValues.Add(BitConverter.ToInt64(buf, 0));
                       attNames.Add(objectName);
                       break;
                       default:
                       break;

                       }
                       }
                       return 0;
                       }
                       ;

                       ulong opl = 0;
                       H5A.iterate(aRecordSetMD.HDF5Matter.CompoundDS, H5.index_t.NAME, H5.iter_order_t.INC, ref opl, op, IntPtr.Zero);
                       bUsesAttribues = true;

                       }

                       System.Int64 hSpc = H5D.get_space(aRecordSetMD.HDF5Matter.CompoundDS);
                       System.UInt64[] dimsf = new System.UInt64[5];
                       System.UInt64[] dimsfmax = new System.UInt64[5];
                       int ndims = H5S.get_simple_extent_dims(hSpc, dimsf, dimsfmax);
                       System.Int64 hTyp = H5D.get_type(aRecordSetMD.HDF5Matter.CompoundDS);
                       H5T.class_t hClass = H5T.get_class(hTyp);
                       if ( hClass != H5T.class_t.COMPOUND )
                       throw new com.WDataSci.WDS.WDSException("Data set " + hDSPath + " in HDF5 file " + aRecordSetMD.FileMatter.FileName + " is not a compound type!");
                       System.Int64 hNTyp = H5T.get_native_type(hTyp, H5T.direction_t.DEFAULT);

                       int hnColumns = (int) H5T.get_nmembers(hNTyp);
                       long hTypSize = (long) H5T.get_size(hTyp);

                       IntPtr[] hSizes = new IntPtr[hnColumns];

                    aRecordSetMD.Column = new FieldMD[hnColumns];
                    H5T.class_t[] hClasses = new H5T.class_t[hnColumns];
                    System.Int64[] hTypes = new System.Int64[hnColumns];

                    for ( jj = 0; jj < hnColumns; jj++ ) {
                        aRecordSetMD.Column[jj] = new FieldMD();
                        System.Int64 hNTypMem = H5T.get_member_type(hNTyp, jj);
                        IntPtr hNTypMemNamePtr = H5T.get_member_name(hNTyp, jj);
                        aRecordSetMD.Column[jj].Name = Marshal.PtrToStringAnsi(hNTypMemNamePtr);
                        hTypes[jj] = H5T.get_member_type(hNTyp, jj);
                        hClasses[jj] = H5T.get_member_class(hNTyp, jj);
                        aRecordSetMD.Column[jj].ByteMemOffset = (long) H5T.get_member_offset(hNTyp, jj);
                        if ( jj > 0 ) {
                            aRecordSetMD.Column[jj - 1].ByteMemLength = aRecordSetMD.Column[jj].ByteMemOffset - aRecordSetMD.Column[jj - 1].ByteMemOffset;
                            if ( jj == hnColumns - 1 ) {
                                aRecordSetMD.Column[jj].ByteMemLength = hTypSize - aRecordSetMD.Column[jj].ByteMemOffset;
                            }
                        }

                    }
                    /* <<< C# */


                    //iterate through columns (dataset members)
                    long offset = 0; // in case get_member_offset above is not working....

                    for ( jj = 0; jj < nColumns; jj++ ) {

                        /* Java >>> */
                        hdf.object.Datatype hType = hTypes[jj];

                        //VLen strings are supposed to have a size of -1
                        int vlen = (int) hType.getDatatypeSize();
                        // the case for variable Length byte fields may not work
                        if ( hType.isVarStr() || hType.isVLEN() ) vlen = -1;

                        aRecordSetMD.Column[jj] = new FieldMD(lNames[jj]
                                , hType.getDatatypeClass()
                                , vlen
                                , hType.getDatatypeOrder()
                                , hType.getDatatypeSign());
                        FieldMD cm = aRecordSetMD.Column[jj];
                        /* <<< Java */
                        /* C# >>> *
                           FieldMD cm = aRecordSetMD.Column[jj];
                           H5T.class_t hClss = hClasses[jj];
                           System.Int64 hType = hTypes[jj];
                           cm.HDF5DataType = new HDF5DataType(hType);
                           switch ( hClss ) {
                           case H5T.class_t.FLOAT:
                           cm.MapToHDF5DataType(FieldMDEnums.eDTyp.Dbl);
                           break;
                           case H5T.class_t.INTEGER:
                           if ( cm.ByteMemLength == 8 )
                           cm.MapToHDF5DataType(FieldMDEnums.eDTyp.Int);
                           else
                           cm.MapToHDF5DataType(FieldMDEnums.eDTyp.Lng);
                           break;
                           case H5T.class_t.STRING:
                        //note: HDF5 is ASCII
                        if ( H5T.is_variable_str(hType) != 0 )
                        cm.MapToHDF5DataType(FieldMDEnums.eDTyp.VLS, Default.StringMaxLength, true);
                        else
                        cm.MapToHDF5DataType(FieldMDEnums.eDTyp.Str, Default.StringMaxLength, true);
                        break;
                        default:
                        break;
                           }
                           cm.DTyp = cm.HDF5DataType.eDTyp();
                        /* <<< C# */

                        if ( cm.ByteMemLength == 0 ) {

                            switch ( cm.DTyp ) {
                                case Dbl:
                                case Dte:
                                case DTm:
                                    //case FieldMDEnums.eDTyp.Dbl:
                                    //case FieldMDEnums.eDTyp.Dte:
                                    //case FieldMDEnums.eDTyp.DTm:
                                    cm.ByteMemLength = 8; // (long)sizeof(double);
                                    break;
                                case Lng:
                                case Byt:
                                    //case FieldMDEnums.eDTyp.Lng:
                                    //case FieldMDEnums.eDTyp.Byt:
                                    cm.ByteMemLength = 8; // (long)sizeof(long);
                                    break;
                                case Int:
                                    //case FieldMDEnums.eDTyp.Int:
                                    cm.ByteMemLength = 4;
                                    break;
                                case VLS:
                                    //case FieldMDEnums.eDTyp.VLS:
                                    //?? if ( cm.ByteMaxLength == 0 ) cm.ByteMaxLength = cm.StringMaxLength; //since HDF5 is ascii
                                    cm.ByteMemLength = 8; // (long)sizeof(long);
                                    break;
                                case Str:
                                    //case FieldMDEnums.eDTyp.Str:
                                    //?? if ( cm.ByteMaxLength == 0 ) cm.ByteMaxLength = cm.StringMaxLength; //since HDF5 is ascii
                                    cm.ByteMemLength = cm.StringMaxLength; //since HDF5 is ascii
                                    break;
                                case Bln:
                                    //case FieldMDEnums.eDTyp.Bln:
                                    cm.ByteMemLength = 1;
                                    break;
                                default:
                                    break;
                            }

                            cm.ByteMemOffset = offset;
                            offset += (int) cm.ByteMemLength;
                        }

                        boolean found = false;
                        if ( bCheckingAgainstPMML ) {
                            //Search for PMML DataFieldName map
                            for ( j = 0; !found && j < nDataFieldNames; j++ ) {
                                found = (cm.Name.equals(lFieldStringNames[j]));
                                if ( found )
                                    cm.MapToMapKey(lFieldStringNames[j]);
                            }
                        }

                        if ( bFillDictionaryNames && !cm.hasMapKey() ) {
                            cm.MapToMapKey(cm.Name);
                        }

                        if ( bUsesAttribues ) {
                            /* Java >>> */
                            i = metadata.indexOf(cm.Name + "/@DTyp");
                            if ( i >= 0 ) {
                                String rep = metadata.get(i).getData().toString();
                                int[] l = {-1};
                                com.WDataSci.JniPMML.FieldMDEnums.eDTyp dtyp = com.WDataSci.JniPMML.FieldMDEnums.eDTyp.FromAlias(rep, l);
                                if ( l[0] > 0 ) cm.StringMaxLength = l[0];
                                if ( !dtyp.equals(cm.DTyp) ) {
                                    //there may be a remapping as with dates
                                    cm.ExternalDTyp = cm.DTyp;
                                    cm.DTyp = dtyp;
                                }
                            }
                            i = metadata.indexOf(cm.Name + "/@StringMaxLength");
                            if ( i >= 0 ) cm.StringMaxLength = (int) (metadata.get(i).getData());
                            /* <<< Java */
                            /* C# >>> *
                               int i = attNames.IndexOf(cm.Name + "/@DTyp");
                               if ( i >= 0 ) {
                               String rep = attValues.get(i).toString();
                               int[] l = {-1};
                               FieldMDEnums.eDTyp dtyp = FieldMDExt.eDTyp_FromAlias(rep, ref l);
                               if ( l[0] > 0 ) cm.StringMaxLength = l[0];
                               if ( !dtyp.equals(cm.DTyp) ) {
                            //there may be a remapping as with dates
                            cm.ExternalDTyp = cm.DTyp;
                            cm.DTyp = dtyp;
                               }
                               }
                               i = attNames.IndexOf(cm.Name + "/@StringMaxLength");
                               if ( i >= 0 ) cm.StringMaxLength = (int) (attValues.get(i));
                            /* <<< C# */
                        }

                        //If Date and DateTime cannot be determined from attributes, check naming convention.
                        //Here a modified three-part name convention is assumed:
                        //    DataClass [PrimaryModifier] Representation [[_Tail Modifier]]
                        //
                        //CJW: WDataSci data field name conventions are CamelCase where underscores
                        //are reserved for later modifications.   See WDataSci ModelSpecification for details.
                        //For other uses, we will assume the last _XXX might be a modifier and will
                        //look for the Representation for Date or DateTime in the at the end or before the last
                        //underscore.

                        if ( !cm.DTyp.bIn(com.WDataSci.JniPMML.FieldMDEnums.eDTyp.Dte, com.WDataSci.JniPMML.FieldMDEnums.eDTyp.DTm) ) {

                            String tmpname2 = cm.Name;
                            if ( tmpname2.indexOf("_") >= 0 ) {
                                tmpname2 = tmpname2.substring(tmpname2.lastIndexOf("_") + 1);
                            }
                            boolean CheckDateTimeName = (tmpname2.endsWith("DateTime") || cm.Name.endsWith("DateTime")
                                    || tmpname2.endsWith("Time") || cm.Name.endsWith("Time")
                                    || tmpname2.endsWith("DTm") || cm.Name.endsWith("DTm")
                            );
                            boolean CheckDateName = (tmpname2.endsWith("Date") || cm.Name.endsWith("Date")
                                    || tmpname2.endsWith("Dte") || cm.Name.endsWith("Dte")
                            );

                            if ( CheckDateTimeName ) {
                                cm.ExternalDTyp = cm.DTyp;
                                cm.DTyp = com.WDataSci.JniPMML.FieldMDEnums.eDTyp.DTm;
                            }
                            else if ( CheckDateName ) {
                                cm.ExternalDTyp = cm.DTyp;
                                cm.DTyp = com.WDataSci.JniPMML.FieldMDEnums.eDTyp.Dte;
                            }
                        }

                    }
                }
                catch ( Exception e ) {
                    throw new WDSException("Error mapping input columns:", e);
                }
                //Java
            } //end of Java Synchronize
        }

        public long mReadPrepFor(RecordSetMD aRecordSetMD, PrintWriter pw)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            int nColumns = aRecordSetMD.nColumns();
            long rv = 0;
            for ( int jj = 0; jj < nColumns; jj++ ) {

                if ( aRecordSetMD.Column[jj].DTyp.bIn(FieldMDEnums.eDTyp.Str, FieldMDEnums.eDTyp.VLS) ) {
                    if ( aRecordSetMD.Column[jj].StringMaxLength == 0 )
                        aRecordSetMD.Column[jj].StringMaxLength = Default.StringMaxLength;
                }

                if ( aRecordSetMD.Column[jj].HDF5DataType == null ) {
                    if ( aRecordSetMD.Mode.equals(RecordSetMDEnums.eMode.Input) ) {
                        if ( aRecordSetMD.Column[jj].StringMaxLength > 0 )
                            aRecordSetMD.Column[jj].HDF5DataType = new HDF5DataType(aRecordSetMD.Column[jj].DTyp, aRecordSetMD.Column[jj].StringMaxLength, Default.anyVLenRead);
                        else
                            aRecordSetMD.Column[jj].HDF5DataType = new HDF5DataType(aRecordSetMD.Column[jj].DTyp, Default.StringMaxLength, Default.anyVLenRead);

                    }
                    else {
                        if ( aRecordSetMD.Column[jj].StringMaxLength > 0 )
                            aRecordSetMD.Column[jj].HDF5DataType = new HDF5DataType(aRecordSetMD.Column[jj].DTyp, aRecordSetMD.Column[jj].StringMaxLength, Default.anyVLenWrite);
                        else {
                            if ( aRecordSetMD.ModeMatter != null )
                                aRecordSetMD.Column[jj].HDF5DataType = new HDF5DataType(aRecordSetMD.Column[jj].DTyp, aRecordSetMD.ModeMatter.OutputMaxStringLength, Default.anyVLenWrite);
                            else
                                aRecordSetMD.Column[jj].HDF5DataType = new HDF5DataType(aRecordSetMD.Column[jj].DTyp, Default.StringMaxLength, Default.anyVLenWrite);
                        }
                    }
                }

                if ( aRecordSetMD.Column[jj].ByteMemLength == 0 ) {

                    switch ( aRecordSetMD.Column[jj].DTyp ) {
                        case Dbl:
                        case Dte:
                        case DTm:
                            //case FieldMDEnums.eDTyp.Dbl:
                            //case FieldMDEnums.eDTyp.Dte:
                            //case FieldMDEnums.eDTyp.DTm:
                            aRecordSetMD.Column[jj].ByteMemLength = 8; // (long)sizeof(double);
                            break;
                        case Lng:
                        case Byt:
                            //case FieldMDEnums.eDTyp.Lng:
                            //case FieldMDEnums.eDTyp.Byt:
                            aRecordSetMD.Column[jj].ByteMemLength = 8; // (long)sizeof(long);
                            break;
                        case Int:
                            //case FieldMDEnums.eDTyp.Int:
                            aRecordSetMD.Column[jj].ByteMemLength = 4;
                            break;
                        case VLS:
                            //case FieldMDEnums.eDTyp.VLS:
                            if ( aRecordSetMD.Column[jj].ByteMaxLength == 0 )
                                aRecordSetMD.Column[jj].ByteMaxLength = aRecordSetMD.Column[jj].StringMaxLength; //since HDF5 is ascii
                            aRecordSetMD.Column[jj].ByteMemLength = 8; // (long)sizeof(long);
                            break;
                        case Str:
                            //case FieldMDEnums.eDTyp.Str:
                            if ( aRecordSetMD.Column[jj].ByteMaxLength == 0 )
                                aRecordSetMD.Column[jj].ByteMaxLength = aRecordSetMD.Column[jj].StringMaxLength; //since HDF5 is ascii
                            aRecordSetMD.Column[jj].ByteMemLength = aRecordSetMD.Column[jj].StringMaxLength; //since HDF5 is ascii
                            break;
                        case Bln:
                            //case FieldMDEnums.eDTyp.Bln:
                            aRecordSetMD.Column[jj].ByteMemLength = 4;
                            break;
                        default:
                            break;
                    }
                }
                aRecordSetMD.Column[jj].ByteMemOffset = rv;
                rv += aRecordSetMD.Column[jj].ByteMemLength;
            }
            return rv;
        }

        public long mWritePrepFor(RecordSetMD aRecordSetMD, PrintWriter pw)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            int nColumns = aRecordSetMD.nColumns();
            long rv = 0;
            for ( int jj = 0; jj < nColumns; jj++ ) {
                if ( aRecordSetMD.Column[jj].DTyp.bIn(FieldMDEnums.eDTyp.Str, FieldMDEnums.eDTyp.VLS) && !Default.anyVLenWrite )
                    aRecordSetMD.Column[jj].DTyp = FieldMDEnums.eDTyp.Str;
            }
            return this.mReadPrepFor(aRecordSetMD, pw);
        }

        public void UpdateOutputMapForHDF5(RecordSetMD aRecordSetMD, PrintWriter pw)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            int nColumns = aRecordSetMD.nColumns();
            for ( int jj = 0; jj < nColumns; jj++ ) {
                if ( aRecordSetMD.Column[jj].DTyp.bIn(com.WDataSci.JniPMML.FieldMDEnums.eDTyp.Str) )
                    aRecordSetMD.Column[jj].DTyp = FieldMDEnums.eDTyp.Str;
                aRecordSetMD.Column[jj].HDF5DataType = new HDF5DataType(aRecordSetMD.Column[jj].DTyp, aRecordSetMD.ModeMatter.OutputMaxStringLength, Default.anyVLenWrite);
            }
        }

        //Java 
        public void mReadRecordSet(RecordSetMD aInputRecordSetMD, RecordSet aInputRecordSet, PrintWriter pw)
        //C# public void mReadRecordSet(RecordSetMD aInputRecordSetMD, RecordSet aInputRecordSet, PrintWriter pw)
        throws com.WDataSci.WDS.WDSException, Exception
        {
                /* C# >>> *
                   byte[] bytebuff = null;
                   GCHandle gc = GCHandle.Alloc(bytebuff, GCHandleType.Pinned);
                   Stack<GCHandle> gch_stack = null;
                   Span<byte> buffspan = null;
                   DBB aDBB = null;
                /* <<< C# */

            try {
                if ( aInputRecordSet.isEmpty() ) {
                    //Java
                    aInputRecordSet.Records = new ArrayList<>(0);
                    //C# aInputRecordSet.Records = new List<Map<T, Object>>(0);
                    //Java
                    aInputRecordSet.Records_Orig = new ArrayList<>(0);
                    //C# aInputRecordSet.Records_Orig = new List<Object[]>(0);
                }
                int nInputColumns = aInputRecordSetMD.nColumns();
                int i = -1;
                int j = -1;

                //Java
                int rank = aInputRecordSetMD.HDF5Matter.CompoundDS.getRank();
                    /* C# >>> *
                       System.Int64 hDSSpace = H5D.get_space(aInputRecordSetMD.HDF5Matter.CompoundDS);
                       System.UInt64[] sdim=new System.UInt64[64];
                       int rank = H5S.get_simple_extent_ndims(hDSSpace);
                       int ret = H5S.get_simple_extent_dims(hDSSpace, sdim, null);
                    /* <<< C# */
                if ( rank != 1 )
                    throw new WDSException("Error DataSet in HDF5 (" + aInputRecordSetMD.FileMatter.FileName + ") is not single rank compound (rank=" + rank + ")!");
                //Java
                int nRows = (int) aInputRecordSetMD.HDF5Matter.CompoundDS.getDims()[0];
                //C# int nRows = (int) sdim[0];

                    /* C# >>> *

                       gch_stack = new Stack<GCHandle>();

                       long rflensize = this.mReadPrepFor(aInputRecordSetMD,pw);

                       unsafe {
                       int nAllocationSize = (int) (nRows * rflensize);
                       nAllocationSize = 65536 * ((int) (nAllocationSize / 65536 + 1));
                       gc.Free();
                       bytebuff = new byte[nAllocationSize];
                       gc = GCHandle.Alloc(bytebuff, GCHandleType.Pinned);
                       buffspan = new Span<byte>(bytebuff, 0, nAllocationSize);

                       aDBB = new DBB()
                       .Wrap(ref bytebuff)
                       .cAsBigEndian()
                       .cAsHDF5BulkCompoundDSWriteLayout(nRows, rflensize);

                       System.Int64 hTyp = H5D.get_type(aInputRecordSetMD.HDF5Matter.CompoundDS);
                       System.Int64 status = H5D.read(aInputRecordSetMD.HDF5Matter.CompoundDS, hTyp, H5S.ALL, H5S.ALL, H5P.DEFAULT, gc.AddrOfPinnedObject());

                       for ( i = 0; i < nRows; i++ ) {
                       Object[] inputRow_orig = new Object[nInputColumns];
                       Map<FieldName, Object> inputRow = new Map<FieldName, Object>();
                       for ( j = 0; j < nInputColumns; j++ ) {
                       switch ( aInputRecordSetMD.Column[j].DTyp ) {
                       case FieldMDEnums.eDTyp.Dbl: {
                       double? lv = aDBB.GetLayerDouble(1);
                       inputRow_orig[j] = lv;
                       if ( aInputRecordSetMD.Column[j].hasMapKey() )
                       inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                       }
                       break;
                       case FieldMDEnums.eDTyp.Lng: {
                       long? lv = aDBB.GetLayerLong(1);
                       inputRow_orig[j] = lv;
                       if ( aInputRecordSetMD.Column[j].hasMapKey() )
                       inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                       }
                       break;
                       case FieldMDEnums.eDTyp.Int: {
                       int? lv = aDBB.GetLayerInt(1);
                       inputRow_orig[j] = lv;
                       if ( aInputRecordSetMD.Column[j].hasMapKey() )
                       inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                       }
                       break;
                       case FieldMDEnums.eDTyp.Dte: {
                       if ( aInputRecordSetMD.Column[i].ExternalDTyp.isNumeric() ) {
                       double? lv = aDBB.GetLayerDouble(1);
                       lv = null;
                       inputRow_orig[j] = lv;
                       if ( aInputRecordSetMD.Column[j].hasMapKey() )
                       inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                       }
                       else if ( aInputRecordSetMD.Column[i].ExternalDTyp.isString() ) {
                       double? lv = null;
                       inputRow_orig[j] = lv;
                       if ( aInputRecordSetMD.Column[j].hasMapKey() )
                       inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                       throw new com.WDataSci.WDS.WDSException("Error Date and DateTime conversions not implemented yet");
                       }
                       }
                       break;
                       case FieldMDEnums.eDTyp.DTm: {
                       if ( aInputRecordSetMD.Column[i].ExternalDTyp.isNumeric() ) {
                       double? lv = aDBB.GetLayerDouble(1);
                       inputRow_orig[j] = lv;
                       if ( aInputRecordSetMD.Column[j].hasMapKey() )
                       inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                       }
                       else if ( aInputRecordSetMD.Column[i].ExternalDTyp.isString() ) {
                           double? lv = null;
                           inputRow_orig[j] = lv;
                           if ( aInputRecordSetMD.Column[j].hasMapKey() )
                               inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                           throw new com.WDataSci.WDS.WDSException("Error Date and DateTime conversions not implemented yet");
                       }
                       }
                break;
                //case VLS:
                    case FieldMDEnums.eDTyp.VLS: {
                                                     IntPtr lIntPtr = (IntPtr) aDBB.GetLayerLong(1);
                                                     String lv = Marshal.PtrToStringAnsi(lIntPtr);
                                                     inputRow_orig[j] = lv;
                                                     if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                                         inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                    }
                    break;
                    //case Str:
                    case FieldMDEnums.eDTyp.Str: {
                                                     String lv = aDBB.GetLayerFLenString(1, aInputRecordSetMD.Column[j].ByteMemLength);
                                                     inputRow_orig[j] = lv;
                                                     if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                                         inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                    }
                    break;
                    //case Bln:
                    case FieldMDEnums.eDTyp.Bln: {
                                                     int? lvi = aDBB.GetLayerInt(1);
                                                     Boolean? lv;
                                                     if ( lvi == null ) lv = null;
                                                     else lv = (lvi != 0);
                                                     inputRow_orig[j] = lv;
                                                     if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                                         inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                    }
                    break;
                    //case Byt:
                    case FieldMDEnums.eDTyp.Byt:
                    default:
                    throw new com.WDataSci.WDS.WDSException("Error, unhandled DTyp in conversion from HDF5");
                       }
                       }
                if ( pw != null ) pw.println(inputRow);
                aInputRecordSet.Records.add(inputRow);
                aInputRecordSet.Records_Orig.add(inputRow_orig);
                       }

                       } //unsafe

                /* <<< C# */

                /* Java >>> */
                Object x = aInputRecordSetMD.HDF5Matter.CompoundDS.getData();
                ArrayList<Object> memberdata = (ArrayList<Object>) x; //aInputRecordSetMD.HDF5Matter.CompoundDS.getData();

                for ( i = 0; i < nRows; i++ ) {
                    Object[] inputRow_orig = new Object[nInputColumns];
                    Map<org.dmg.pmml.FieldName, Object> inputRow = new LinkedHashMap<>();
                    for ( j = 0; j < nInputColumns; j++ ) {
                        switch ( aInputRecordSetMD.Column[j].DTyp ) {
                            case Dbl: {
                                Double lv = ((double[]) memberdata.get(j))[i];
                                if ( lv.isInfinite() || lv.isNaN() || lv.equals(Double.MAX_VALUE) || lv.equals(Double.MIN_VALUE) )
                                    lv = null;
                                inputRow_orig[j] = lv;
                                if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                    inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                            }
                            break;
                            case Lng: {
                                Long lv = ((long[]) memberdata.get(j))[i];
                                if ( lv.equals(Long.MAX_VALUE) || lv.equals(Long.MIN_VALUE) ) lv = null;
                                inputRow_orig[j] = lv;
                                if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                    inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                            }
                            break;
                            case Int: {
                                Integer lv = ((int[]) memberdata.get(j))[i];
                                if ( lv.equals(Integer.MAX_VALUE) || lv.equals(Integer.MIN_VALUE) ) lv = null;
                                inputRow_orig[j] = lv;
                                if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                    inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                            }
                            break;
                            case Dte: {
                                if ( aInputRecordSetMD.Column[i].ExternalDTyp.isNumeric() ) {
                                    Double lv = ((double[]) memberdata.get(j))[i];
                                    if ( lv.isInfinite() || lv.isNaN() || lv.equals(Double.MAX_VALUE) || lv.equals(Double.MIN_VALUE) )
                                        lv = null;
                                    inputRow_orig[j] = lv;
                                    if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                        inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                                }
                                else if ( aInputRecordSetMD.Column[i].ExternalDTyp.isString() ) {
                                    Double lv = null;
                                    inputRow_orig[j] = lv;
                                    if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                        inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                                    throw new com.WDataSci.WDS.WDSException("Error Date and DateTime conversions not implemented yet");
                                }
                            }
                            break;
                            case DTm: {
                                if ( aInputRecordSetMD.Column[i].ExternalDTyp.isNumeric() ) {
                                    Double lv = ((double[]) memberdata.get(j))[i];
                                    if ( lv.isInfinite() || lv.isNaN() || lv.equals(Double.MAX_VALUE) || lv.equals(Double.MIN_VALUE) )
                                        lv = null;
                                    inputRow_orig[j] = lv;
                                    if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                        inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                                }
                                else if ( aInputRecordSetMD.Column[i].ExternalDTyp.isString() ) {
                                    Double lv = null;
                                    inputRow_orig[j] = lv;
                                    if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                        inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                                    throw new com.WDataSci.WDS.WDSException("Error Date and DateTime conversions not implemented yet");
                                }
                            }
                            break;
                            case VLS:
                            case Str: {
                                String lv = ((String[]) memberdata.get(j))[i];
                                inputRow_orig[j] = lv;
                                if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                    inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                            }
                            break;
                            case Bln: {
                                Integer lvi = ((int[]) memberdata.get(j))[i];
                                Boolean lv;
                                if ( lvi == null ) lv = null;
                                else lv = (lvi != 0);
                                inputRow_orig[j] = lv;
                                if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                    inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                            }
                            break;
                            case Byt:
                            default:
                                throw new com.WDataSci.WDS.WDSException("Error, unhandled DTyp in conversion from HDF5");
                        }
                    }
                    if ( pw != null ) pw.println(inputRow);
                    aInputRecordSet.Records.add(inputRow);
                    aInputRecordSet.Records_Orig.add(inputRow_orig);
                }
                /* <<< Java */

            }
            catch ( Exception e ) {
                throw new WDSException("Error reading from HDF5 file:", e);
            }
            finally {
                    /* C# >>> *
                       while ( gch_stack.Count > 0 ) {
                       GCHandle gch = gch_stack.Pop();
                       gch.Free();
                       }
                       aDBB = null;
                       buffspan = null;
                       bytebuff = null;
                       gc.Free();
                    /* <<< C# */

            }

        }


        public int mWriteMap(RecordSetMD aRecordSetMD)
        throws com.WDataSci.WDS.WDSException
        {   //implemented at the time of mWriteRowSet
            return 0;
        }

        //Java 
        public void mWriteRecordSet(RecordSetMD aOutputRecordSetMD, RecordSet aOutputRecordSet, RecordSetMD aInputRecordSetMD, RecordSet aInputRecordSet)
        //C# public void mWriteRecordSet(RecordSetMD aOutputRecordSetMD, RecordSet aOutputRecordSet, RecordSetMD aInputRecordSetMD, RecordSet aInputRecordSet)
        throws com.WDataSci.WDS.WDSException, Exception
        {

            if ( aOutputRecordSetMD.HDF5Matter == null ) {
                aOutputRecordSetMD.HDF5Matter = new WranglerHDF5();
                aOutputRecordSetMD.HDF5Matter.DSName = "OutputRowSet";
            }

            int i = -1;
            int j = -1;
            int k = -1;
            int jj = -1;

            int nInputColumns = aOutputRecordSetMD.ModeMatter.nInputFields;

            int nColumns = aOutputRecordSetMD.Column.length;
            int nResultColumns = nColumns;
            if ( aOutputRecordSetMD.ModeMatter.bRepeatInputFields )
                nResultColumns -= nInputColumns;


            int nRows = aOutputRecordSet.Records.size();


            aOutputRecordSetMD.HDF5Matter.mWritePrepFor(aOutputRecordSetMD, null);

            try {

                String aPathAndName = com.WDataSci.WDS.Util.PathAndName(aOutputRecordSetMD.FileMatter.Path, aOutputRecordSetMD.FileMatter.FileName);

                /* Java >>> */
                H5File outHDF5 = new H5File(aPathAndName, FileFormat.CREATE);
                outHDF5.open();
                /* <<< Java */
                    /* C# >>> *
                       System.Int64 outHDF5 = H5F.create(aPathAndName, H5F.ACC_TRUNC);
                    //System.Int64 outHDF5 = H5F.open(sFileName, H5F.ACC_RDONLY);
                    /* <<< C# */

                //single is just a place holder for attribute writing calls below
                //Java
                @Nullable
                long[] single = {1};

                //Java
                @Nullable
                long[] dims = {nRows};
                //Java
                @Nullable
                String[] membernames = new String[nColumns];
                //Java
                @Nullable
                hdf.object.Datatype[] memberDatatypes = new hdf.object.Datatype[nColumns];
                //C# System.Int64[] memberDatatypes = new System.Int64[nColumns];
                int[] memberSizes = new int[nColumns];
                //Java
                @Nullable
                ArrayList<Object> data = new ArrayList<>();
                //C# System.Collections.Generic.List<Object> data = new System.Collections.Generic.List<Object>(nColumns);

                //Java
                @Nullable
                hdf.object.Group pgroup = (hdf.object.Group) outHDF5.get("/");

                    /* C# >>> *
                       String sStrLen = "VARIABLE";
                    //ma.InputBox("Input path fixed string length (HDF-Java 1.11.4 has a problem reading compound datasets with variable length strings, use VARIABLE to write vlen strings)","String Max Lengths", "64");
                    int nFixedStrLen = 0;
                    if ( !sStrLen.Equals("VARIABLE") ) {
                    nFixedStrLen = (sStrLen).ConvertTo<int>();
                    }
                    System.Int64 hStrType = H5T.copy(H5T.C_S1);
                    if ( nFixedStrLen == 0 ) {
                    H5T.set_size(hStrType, H5T.VARIABLE);
                    }
                    else {
                    H5T.set_size(hStrType, (IntPtr) nFixedStrLen);
                    H5T.set_strpad(hStrType, H5T.str_t.NULLTERM);
                    }
                    /* <<< C# */

                Object obj = null;

                jj = 0;
                if ( aOutputRecordSetMD.ModeMatter.bRepeatInputFields ) {
                    //Java
                    ArrayList<Object> data_orig = null;
                    //C# System.Collections.Generic.List<Object> data_orig = null;
                    if ( aInputRecordSetMD.Type.bIn(RecordSetMDEnums.eType.HDF5) ) {
                        //Java
                        data_orig = (ArrayList<Object>) aInputRecordSetMD.HDF5Matter.CompoundDS.getData();
                        //C#
                        //??????? data_orig = (System.Collections.Generic.List<Object>) aInputRecordSetMD.HDF5Matter.CompoundDS.getData();
                    }
                    else {
                        //Java
                        data_orig = new ArrayList<>(nInputColumns);
                        //C# data_orig = new System.Collections.Generic.List<Object>(nInputColumns);
                    }
                    for ( j = 0; j < nInputColumns; j++ ) {
                        membernames[j] = aOutputRecordSetMD.Column[j].Name;
                        memberSizes[j] = 1;
                        memberDatatypes[j] = aOutputRecordSetMD.Column[j].HDF5DataType.data;
                        if ( aInputRecordSetMD.Type.bIn(RecordSetMDEnums.eType.HDF5) ) {
                            //Java
                            data.add(j, data_orig.get(j));
                            //C# data[j] = data_orig[j];
                        }
                        else {
                            switch ( aOutputRecordSetMD.Column[j].DTyp ) {
                                case Dbl:
                                    //case FieldMDEnums.eDTyp.Dbl:
                                case Dte:
                                    //case FieldMDEnums.eDTyp.Dte:
                                case DTm:
                                    //case FieldMDEnums.eDTyp.DTm:
                                    double[] lvd = new double[nRows];
                                    for ( i = 0; i < nRows; i++ ) {
                                        obj = aInputRecordSet.Records_Orig.get(i)[j];
                                        if ( obj == null )
                                            lvd[i] = Double.NaN;
                                        else
                                            lvd[i] = (double) obj;
                                    }
                                    //Java
                                    data.add(lvd);
                                    //C# data[j] = lvd;
                                    break;
                                case Lng:
                                    //case FieldMDEnums.eDTyp.Lng:
                                    long[] lvl = new long[nRows];
                                    for ( i = 0; i < nRows; i++ ) {
                                        obj = aInputRecordSet.Records_Orig.get(i)[j];
                                        if ( obj == null )
                                            //Java
                                            lvl[i] = Long.MIN_VALUE;
                                            //C# lvl[i] = long.MinValue;
                                        else
                                            lvl[i] = (long) obj;
                                    }
                                    //Java
                                    data.add(lvl);
                                    //C# data[j] = lvl;
                                    break;
                                case Bln:
                                    //case FieldMDEnums.eDTyp.Bln:
                                case Int:
                                    //case FieldMDEnums.eDTyp.Int:
                                    int[] lvi = new int[nRows];
                                    for ( i = 0; i < nRows; i++ ) {
                                        obj = aInputRecordSet.Records_Orig.get(i)[j];
                                        if ( obj == null )
                                            //Java
                                            lvi[i] = Integer.MIN_VALUE;
                                            //C# lvi[i] = int.MinValue;
                                        else
                                            lvi[i] = (int) obj;
                                    }
                                    //Java
                                    data.add(lvi);
                                    //C# data[j] = lvi;
                                    break;
                                case VLS:
                                    //case FieldMDEnums.eDTyp.VLS:
                                case Str:
                                    //case FieldMDEnums.eDTyp.Str:
                                    String[] lvs = new String[nRows];
                                    for ( i = 0; i < nRows; i++ )
                                        lvs[i] = aInputRecordSet.Records_Orig.get(i)[j].toString();
                                    //Java
                                    data.add(lvs);
                                    //C# data[j] = lvs;
                                    break;
                                case Byt:
                                    //case FieldMDEnums.eDTyp.Byt:
                                default:
                                    throw new WDSException("Error, un-implemented OutputColumn DataType !");
                            }
                        }
                    }
                    jj = nInputColumns;
                }


                for ( k = 0, j = jj; k < nResultColumns; k++, j++ ) {

                    membernames[j] = aOutputRecordSetMD.Column[j].Name;
                    memberSizes[j] = 1;
                    memberDatatypes[j] = aOutputRecordSetMD.Column[j].HDF5DataType.data;

                    switch ( aOutputRecordSetMD.Column[j].DTyp ) {
                        case Dbl:
                            //case FieldMDEnums.eDTyp.Dbl:
                        case Dte:
                            //case FieldMDEnums.eDTyp.Dte:
                        case DTm:
                            //case FieldMDEnums.eDTyp.DTm:
                            //Java
                            data.add(new double[nRows]);
                            //C# data[j] = new double[nRows];
                            break;
                        case Lng:
                            //case FieldMDEnums.eDTyp.Lng:
                            data.add(new long[nRows]);
                            //C# data[j] = new long[nRows];
                            break;
                        case Bln:
                            //case FieldMDEnums.eDTyp.Bln:
                        case Int:
                            //case FieldMDEnums.eDTyp.Int:
                            data.add(new int[nRows]);
                            //C# data[j] = new int[nRows];
                            break;
                        case VLS:
                            //case FieldMDEnums.eDTyp.VLS:
                        case Str:
                            //case FieldMDEnums.eDTyp.Str:
                            data.add(new String[nRows]);
                            //C# data[j] = new String[nRows];
                            break;
                        case Byt:
                            //case FieldMDEnums.eDTyp.Byt:
                        default:
                            throw new WDSException("Error, un-implemented OutputColumn DataType !");
                    }
                }

                for ( i = 0; i < nRows; i++ ) {
                    for ( k = 0, j = nInputColumns; k < nResultColumns; k++, j++ ) {
                        obj = aOutputRecordSet.Records.get(i).get(aOutputRecordSetMD.Column[j].MapKey);
                        switch ( aOutputRecordSetMD.Column[j].DTyp ) {
                            case Dte:
                                //case FieldMDEnums.eDTyp.Dte:
                            case DTm:
                                //case FieldMDEnums.eDTyp.DTm:
                            case Dbl:
                                //case FieldMDEnums.eDTyp.Dbl:
                                //((double[]) data.get(j))[i] = (double) obj;
                                if ( obj == null )
                                    ((double[]) data.get(j))[i] = Double.NaN;
                                else
                                    ((double[]) data.get(j))[i] = (double) obj;
                                break;
                            case Lng:
                                //case FieldMDEnums.eDTyp.Lng:
                                if ( obj == null )
                                    //Java
                                    ((long[]) data.get(j))[i] = Long.MIN_VALUE;
                                    //C# ((long[]) data.get(j))[i] = long.MinValue;
                                else
                                    ((long[]) data.get(j))[i] = (long) obj;
                                break;
                            case Bln:
                                //case FieldMDEnums.eDTyp.Bln:
                            case Int:
                                //case FieldMDEnums.eDTyp.Int:
                                if ( obj == null )
                                    //Java
                                    ((int[]) data.get(j))[i] = Integer.MIN_VALUE;
                                    //C# ((int[]) data.get(j))[i] = int.MinValue;
                                else
                                    ((int[]) data.get(j))[i] = (int) obj;
                                break;
                            case VLS:
                                //case FieldMDEnums.eDTyp.VLS:
                            case Str:
                                //case FieldMDEnums.eDTyp.Str:
                                ((String[]) data.get(j))[i] = (String) aOutputRecordSet.Records.get(i).get(aOutputRecordSetMD.Column[j].MapKey);
                                break;
                            case Byt:
                                //case FieldMDEnums.eDTyp.Byt:
                            default:
                                throw new WDSException("Error, un-implemented OutputColumn DataType !");
                        }
                    }
                }


                /* Java >>> */
                hdf.object.h5.H5CompoundDS outputCDS2 = (hdf.object.h5.H5CompoundDS) outHDF5.createCompoundDS("/" + aOutputRecordSetMD.HDF5Matter.DSName
                        , pgroup, dims, null, null, 0
                        , membernames, memberDatatypes, memberSizes, (Object) data);

                hdf.object.h5.H5Datatype lVLenStringDataType = null;
                hdf.object.h5.H5Datatype lFLenStringDataType = null;
                hdf.object.h5.H5Datatype lLongDataType = null;
                lFLenStringDataType = new hdf.object.h5.H5Datatype(Datatype.CLASS_STRING, FieldMD.Default.HeaderStringMaxLength, -1, -1);
                lVLenStringDataType = new hdf.object.h5.H5Datatype(Datatype.CLASS_STRING, -1, -1, -1);
                lLongDataType = new hdf.object.h5.H5Datatype(hdf.object.h5.H5Datatype.CLASS_INTEGER, 8, -1, -1);

                long[] attvl = {0L};
                String[] attvs = {""};
                for ( j = 0; j < nColumns; j++ ) {
                    attvs[0] = aOutputRecordSetMD.Column[j].DTyp.ToString();
                    Attribute att = new Attribute(outputCDS2
                            , aOutputRecordSetMD.Column[j].Name + "/@DTyp"
                            , lVLenStringDataType
                            , single
                            , attvs);
                    outputCDS2.writeMetadata(att);
                    switch ( aOutputRecordSetMD.Column[j].DTyp ) {
                        case VLS:
                        case Str:
                            attvl[0] = aOutputRecordSetMD.Column[j].StringMaxLength;
                            att = new Attribute(outputCDS2
                                    , aOutputRecordSetMD.Column[j].Name + "/@StringMaxLength"
                                    , lLongDataType
                                    , single
                                    , attvl);
                            outputCDS2.writeMetadata(att);
                            break;
                        case Byt:
                            attvl[0] = aOutputRecordSetMD.Column[j].ByteMaxLength;
                            att = new Attribute(outputCDS2
                                    , aOutputRecordSetMD.Column[j].Name + "/@ByteMaxLength"
                                    , lLongDataType
                                    , single
                                    , attvl);
                            outputCDS2.writeMetadata(att);
                            break;
                        default:
                            break;
                    }
                }

                //outputCDS.close();
                outHDF5.close();
                /* <<< Java */
                    /* C# >>> *

                    /* <<< C# */

            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error writing HDF5", e);
            }

        }



        /* C# >>> *
           public int mWriteRecordSet(RecordSetMD aRecordSetMD
           , MOIE.ListObject aListObject
           )
           {
           try {

           int nRows = aListObject.ListRows.Count;
           int nColumns = aListObject.ListColumns.Count;

           if ( nColumns != aRecordSetMD.nColumns() )
           throw new com.WDataSci.WDS.WDSException("Error, Excel ListObject #columns does not match RecordSetMD #columns");


           if ( this.DSName == null || this.DSName.isEmpty() )
           this.DSName = "RecordSet";

           String aPathAndName = com.WDataSci.WDS.Util.PathAndName(aRecordSetMD.FileMatter.Path, aRecordSetMD.FileMatter.FileName);

           long rflensize = this.mWritePrepFor(aRecordSetMD, null);

           Stack<GCHandle> gch_stack = new Stack<GCHandle>();

           unsafe {

           MOIE.Range r = aListObject.Range;
           object[,] ro = aListObject.Range.Value2;

           byte[] bytebuf = new byte[(int) (nRows * rflensize)];
           Span<byte> Spanbytebuff = new Span<byte>(bytebuf, 0, (int)( nRows * rflensize));

           DBB aDBB = new DBB()
           .Wrap(ref bytebuf)
           .cAsHDF5BulkCompoundDSWriteLayout(nRows,rflensize);

           fixed ( byte* bp = Spanbytebuff ) {

           for ( int i = 0; i < nRows; i++ ) {

           for ( int j = 0; j < nColumns; j++ ) {

           object obj = ro[i + 2, j + 1];
           switch ( aRecordSetMD.Column[j].DTyp ) {
           case FieldMDEnums.eDTyp.Dbl:
           aDBB.PutLayerDouble(1, obj);
           break;
           case FieldMDEnums.eDTyp.Lng:
           aDBB.PutLayerLong(1, obj);
           break;
           case FieldMDEnums.eDTyp.Dte:
           aDBB.PutLayerDouble(1, obj);
           break;
           case FieldMDEnums.eDTyp.DTm:
           aDBB.PutLayerDouble(1, obj);
           break;
           case FieldMDEnums.eDTyp.Int:
           aDBB.PutLayerInt(1, obj);
           break;
           case FieldMDEnums.eDTyp.Str:
           case FieldMDEnums.eDTyp.VLS:

                                        String s = obj.ToString();
                                        //Note: since HDF5 is ascii, remove spurious characters 
                                        s=Regex.Replace(s, @"[^\u0001-\u00F7]", "");
                                        //Note: in opening the standard dataset Audit.csv and saving as Audit.h5
                                        //spurious \u0001 characters showed up in later field values
                                        s=Regex.Replace(s, @"[\u0001-\u0007]", "");

           GCHandle gch = GCHandle.Alloc(Encoding.Convert(Encoding.Default
           , Encoding.UTF8
           , Encoding.Default.GetBytes(s.ConvertTo<String>())), GCHandleType.Pinned);
           gch_stack.Push(gch);
           aDBB.PutLayerLong(1, (long) gch.AddrOfPinnedObject());
           break;

           default:
           throw new Exception("Hey");
           }

           }
           }

                        try {

                            aRecordSetMD.HDF5Matter.File = H5F.create(aPathAndName, H5F.ACC_TRUNC);
                            System.Int64 hDType = H5T.create(H5T.class_t.COMPOUND, (IntPtr) rflensize);
                            for ( int jj = 0 ; jj < aRecordSetMD.nColumns() ; jj++ )
                                H5T.insert(hDType, aRecordSetMD.Column[jj].Name, (IntPtr) aRecordSetMD.Column[jj].ByteMemOffset, aRecordSetMD.Column[jj].HDF5DataType.data);

                            System.UInt64[] dimsf = { (System.UInt64) nRows };
                            System.Int64 hDSSpc = H5S.create_simple(1, dimsf, null);
                            aRecordSetMD.HDF5Matter.CompoundDS = H5D.create(aRecordSetMD.HDF5Matter.File, this.DSName, hDType, hDSSpc);

                            System.Int64 stat = H5D.write(aRecordSetMD.HDF5Matter.CompoundDS
                                    , hDType
                                    , H5S.ALL
                                    , H5S.ALL
                                    , H5P.DEFAULT
                                    , (IntPtr) bp
                                    );

                            System.Int64 lVLenStringDataType = H5T.copy(H5T.C_S1);
                            H5T.set_size(lVLenStringDataType, H5T.VARIABLE);
                            System.Int64 lDTyp = H5T.copy(H5T.C_S1);
                            H5T.set_size(lDTyp, new IntPtr(3));
                            //H5T.set_strpad(lDTyp, H5T.str_t.NULLTERM);
                            System.Int64 lFLenStringDataType = H5T.copy(H5T.C_S1);
                            H5T.set_size(lFLenStringDataType, (IntPtr) (Default.HeaderStringMaxLength));
                            H5T.set_strpad(lFLenStringDataType, H5T.str_t.NULLTERM);
                            System.Int64 lLongDataType = H5T.copy(H5T.NATIVE_INT64);
                            System.UInt64[] attdims = { 1 };

                            System.Int64 attid = 0;

                            for ( int j = 0 ; j < nColumns ; j++ ) {

                                long lvl = 0;
                                try {

                                    {

                                        String aDTyp = aRecordSetMD.Column[j].DTyp.ToString().substring(0, 3);
                                        byte[] aDTypBytes = Encoding.Convert(Encoding.Default, Encoding.ASCII, Encoding.Default.GetBytes(aDTyp));
                                        System.Int64 attscalar = H5S.create(H5S.class_t.SCALAR);
                                        attid = H5A.create(aRecordSetMD.HDF5Matter.CompoundDS
                                                , aRecordSetMD.Column[j].Name + "/@DTyp"
                                                , lDTyp
                                                , attscalar);
                                        //IntPtr aDTypArray = Marshal.StringToHGlobalAnsi(aDTyp);
                                        GCHandle gch = GCHandle.Alloc(aDTypBytes, GCHandleType.Pinned);
                                        gch_stack.Push(gch);
                                        H5A.write(attid, lDTyp, gch.AddrOfPinnedObject());
                                        H5S.close(attscalar);
                                        H5A.close(attid);
                                        //H5A.write(attid, lDTyp, aDTypArray);
                                        //Marshal.FreeHGlobal(aDTypArray);

                                    }

                                    switch ( aRecordSetMD.Column[j].DTyp ) {
                                        case FieldMDEnums.eDTyp.VLS:
                                        case FieldMDEnums.eDTyp.Str: {
                                            lvl = aRecordSetMD.Column[j].StringMaxLength;
                                            GCHandle gch = GCHandle.Alloc(lvl, GCHandleType.Pinned);
                                            gch_stack.Push(gch);
                                            System.Int64 attscalar = H5S.create(H5S.class_t.SCALAR);
                                            attid = H5A.create(aRecordSetMD.HDF5Matter.CompoundDS
                                                    , aRecordSetMD.Column[j].Name + "/@StringMaxLength"
                                                    , lLongDataType
                                                    , attscalar);
                                            H5A.write(attid, lLongDataType, gch.AddrOfPinnedObject());
                                            H5S.close(attscalar);
                                            H5A.close(attid);
                                        }
                                        break;
                                        case FieldMDEnums.eDTyp.Byt: {
                                            lvl = aRecordSetMD.Column[j].ByteMaxLength;
                                            GCHandle gch = GCHandle.Alloc(lvl, GCHandleType.Pinned);
                                            gch_stack.Push(gch);
                                            System.Int64 attscalar = H5S.create(H5S.class_t.SCALAR);
                                            attid = H5A.create(aRecordSetMD.HDF5Matter.CompoundDS
                                                    , aRecordSetMD.Column[j].Name + "/@ByteMaxLength"
                                                    , lLongDataType
                                                    , attscalar);
                                            H5A.write(attid, lLongDataType, gch.AddrOfPinnedObject());
                                            H5S.close(attscalar);
                                            H5A.close(attid);
                                        }
                                        break;
                                        default:
                                            break;
                                    }

                                }
                                catch ( Exception e ) {
                                    throw new com.WDataSci.WDS.WDSException("Error writing attributes to " + aRecordSetMD.HDF5Matter.DSName, e);
                                }

                            }

                            if ( attid > 0 ) H5A.close(attid);

                            if ( hDSSpc > 0 ) H5S.close(hDSSpc);
                            if ( hDType > 0 ) H5T.close(hDType);

                            H5D.close(aRecordSetMD.HDF5Matter.CompoundDS);
                            aRecordSetMD.HDF5Matter.CompoundDS = 0;
                            H5F.close(aRecordSetMD.HDF5Matter.File);
                            aRecordSetMD.HDF5Matter.File = 0;

                        }
                        catch ( Exception e ) {
                            MessageBox.Show("Error in H5D.write!\n" + e.StackTrace.ToString());
                        }

                        while ( gch_stack.Count > 0 ) {
                            GCHandle gch = gch_stack.Pop();
                            if ( gch.IsAllocated ) gch.Free();
                        }

                    } //fixed
                } //unsafe

                return 0;

            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error in writing output map to DBB", e);
            }
        }
/* <<< C# */

        public HDF5DataType new_HDF5DataType(int hclass, int hlength, int horder, int hsign)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            return new HDF5DataType(hclass, hlength, horder, hsign);
        }

        public HDF5DataType new_HDF5DataType(HDF5DataType arg)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            return new HDF5DataType(arg.data);
        }

        //Java
        public
        //C# internal
        HDF5DataType new_HDF5DataType(FieldBaseMD arg)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            return new HDF5DataType(arg.HDF5DataType.data);
        }

        public HDF5DataType new_HDF5DataType(FieldMDEnums.eDTyp DTyp)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            return new HDF5DataType(DTyp, Default.StringMaxLength, false);
        }

        public HDF5DataType new_HDF5DataType(com.WDataSci.JniPMML.FieldMDEnums.eDTyp DTyp, long nStringMaxLength, boolean anyVLen)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            return new HDF5DataType(DTyp, nStringMaxLength, anyVLen);
        }


        public HDF5DataType new_HDF5DataType(long arg)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            //just for compatibility with C#
            /*from Datatype.class
              public static final int CLASS_INTEGER = 0;
              public static final int CLASS_FLOAT = 1;
              public static final int CLASS_CHAR = 2;
              public static final int CLASS_STRING = 3;
              public static final int CLASS_BITFIELD = 4;
              public static final int CLASS_OPAQUE = 5;
              public static final int CLASS_COMPOUND = 6;
              public static final int CLASS_REFERENCE = 7;
              public static final int CLASS_ENUM = 8;
              public static final int CLASS_VLEN = 9;
              public static final int CLASS_ARRAY = 10;
              public static final int CLASS_TIME = 11;
              */
            if ( arg == 0 )
                return new HDF5DataType((int) arg, 4, -1, -1);
            else if ( arg == 3 )
                return new HDF5DataType((int) arg, Default.StringMaxLength, -1, -1);
            else
                return new HDF5DataType((int) arg, 8, -1, -1);
        }


        //this class is just to wrap the HDF5 D
        public class HDF5DataType
        {
            //Java
            public hdf.object.Datatype data = null;
            //C# public System.Int64 data=-1;

            //C# public HDF5DataType(System.Int64 hclass, int hlength, int horder, int hsign)
            //Java
            public HDF5DataType(int hclass, int hlength, int horder, int hsign)
            throws Exception
            {
                //Java
                this.data = new hdf.object.h5.H5Datatype(hclass, hlength, horder, hsign);
            /* C# >>> *
            //C# will not let switches using H5T.NATIVE_AAA as cases
            if ( bIn((long) hclass, H5T.C_S1, H5T.NATIVE_CHAR) ) {
            System.Int64 hStrType = H5T.copy(H5T.C_S1);
            //H5T.set_size(hStrType, H5T.VARIABLE);
            H5T.set_size(hStrType, (IntPtr) hlength);
            H5T.set_strpad(hStrType, H5T.str_t.NULLTERM);
            this.data = hStrType;
            return;
            }
            else if ( bIn((long) hclass, H5T.NATIVE_DOUBLE, H5T.NATIVE_DOUBLE, H5T.NATIVE_INT, H5T.NATIVE_INT32, H5T.NATIVE_INT64, H5T.NATIVE_LONG) ) {
            this.data = hclass;
            }
            else
            throw new com.WDataSci.WDS.WDSException("Error HDF5 output datatype not handled");
            /* <<< C# */
            }

            /* Java >>> */
            public HDF5DataType(hdf.object.Datatype arg)
            throws Exception
            {
                this.data = new hdf.object.h5.H5Datatype(arg.getDatatypeClass(), (int) arg.getDatatypeSize(), arg.getDatatypeOrder(), arg.getDatatypeSign());
            }
            /* <<< Java */
    /* C# >>> *
       public HDF5DataType(System.Int64 arg)
       {
       this.data = arg;
       }
    /* <<< C# */

            public HDF5DataType(com.WDataSci.JniPMML.FieldMDEnums.eDTyp DTyp, long nStringMaxLength, boolean anyVLen)
            throws com.WDataSci.WDS.WDSException, Exception
            {
                switch ( DTyp ) {
                    case Dbl:
                        this.data = new hdf.object.h5.H5Datatype(hdf.object.Datatype.CLASS_FLOAT, 8, -1, -1);
                        //case FieldMDEnums.eDTyp.Dbl:
                    case Dte:
                        //case FieldMDEnums.eDTyp.Dte:
                    case DTm:
                        //case FieldMDEnums.eDTyp.DTm:
                        //Java
                        this.data = new hdf.object.h5.H5Datatype(hdf.object.Datatype.CLASS_FLOAT, 8, -1, -1);
                        //C# this.data = H5T.NATIVE_DOUBLE;
                        return;
                    case Lng:
                        //case FieldMDEnums.eDTyp.Lng:
                        //Java
                        this.data = new hdf.object.h5.H5Datatype(hdf.object.Datatype.CLASS_INTEGER, 8, -1, -1);
                        //C# this.data = H5T.NATIVE_LONG;
                        return;
                    case Int:
                        //case FieldMDEnums.eDTyp.Int:
                        //Java
                        this.data = new hdf.object.h5.H5Datatype(hdf.object.Datatype.CLASS_INTEGER, 4, -1, -1);
                        //C# this.data = H5T.NATIVE_INT32;
                        return;
                    case VLS:
                        //case FieldMDEnums.eDTyp.VLS:
                        /* Java >>> */
                        if ( anyVLen ) {
                            // according to the docs, to get a variable Length string it should be....
                            //this.data = new hdf.object.h5.H5Datatype(hdf.object.Datatype.CLASS_STRING, -1, -1, -1);
                            //but if un-implemented in Java, use a fixed Length string
                            this.data = new hdf.object.h5.H5Datatype(hdf.object.Datatype.CLASS_STRING, (int) nStringMaxLength, -1, -1);
                            return;
                        }
                        // else falls through
                        /* <<< Java */
                    /* C# >>> *
                       System.Int64 hVLSType = H5T.copy(H5T.C_S1);
                       if ( anyVLen ) {
                       H5T.set_size(hVLSType, H5T.VARIABLE);
                       }
                       else {
                       H5T.set_size(hVLSType, (IntPtr) nStringMaxLength);
                       H5T.set_strpad(hVLSType, H5T.str_t.NULLTERM);
                       }
                       this.data = hVLSType;
                       return;
                    /* <<< C# */
                    case Str:
                        //case FieldMDEnums.eDTyp.Str:
                        //Java
                        this.data = new hdf.object.h5.H5Datatype(hdf.object.Datatype.CLASS_STRING, (int) nStringMaxLength, -1, -1);
                    /* C# >>> *
                       System.Int64 hStrType = H5T.copy(H5T.C_S1);
                       H5T.set_size(hStrType, (IntPtr) nStringMaxLength);
                       H5T.set_strpad(hStrType, H5T.str_t.NULLTERM);
                       this.data = hStrType;
                    /* <<< C# */
                        return;
                    case Byt:
                        //case FieldMDEnums.eDTyp.Byt:
                    case Bln:
                        //case FieldMDEnums.eDTyp.Bln:
                        //Java
                        this.data = new hdf.object.h5.H5Datatype(hdf.object.Datatype.CLASS_INTEGER, 1, -1, -1);
                        //C# this.data = H5T.NATIVE_INT32;
                        return;
                    default:
                        throw new com.WDataSci.WDS.WDSException("Error, un-implemented OutputColumn DataType !" + DTyp.ToString());
                }
            }

            public com.WDataSci.JniPMML.FieldMDEnums.eDTyp eDTyp()
            {
                /* Java >>> */
                com.WDataSci.JniPMML.FieldMDEnums.eDTyp rv;
                if ( this.data.isInteger() ) {
                    if ( this.data.getDatatypeSize() > 4 )
                        return FieldMDEnums.eDTyp.Lng;
                    else
                        return FieldMDEnums.eDTyp.Int;
                }
                else if ( this.data.isFloat() )
                    return FieldMDEnums.eDTyp.Dbl;
                else if ( this.data.isVarStr() )
                    return FieldMDEnums.eDTyp.VLS;
                else if ( this.data.isString() )
                    return FieldMDEnums.eDTyp.Str;
                return FieldMDEnums.eDTyp.Unk;
                /* <<< Java */
        /* C# >>> *
           H5T.class_t c = H5T.get_class(this.data);
           if ( c.Equals(H5T.class_t.INTEGER) ) {
           int s = (int) H5T.get_size(this.data);
           if ( s > 4 )
           return FieldMDEnums.eDTyp.Lng;
           else
           return FieldMDEnums.eDTyp.Int;
           }
           else if ( c.Equals(H5T.class_t.FLOAT) )
           return FieldMDEnums.eDTyp.Dbl;
           else if ( H5T.is_variable_str(this.data) != 0 )
           return FieldMDEnums.eDTyp.VLS;
           else if ( c.Equals(H5T.class_t.STRING) )
           return FieldMDEnums.eDTyp.Str;
           return FieldMDEnums.eDTyp.Unk;
        /* <<< C# */
            }

            public boolean Equals(HDF5DataType arg)
            {
                /* Java >>> */
                if ( this.data.getDatatypeClass() != arg.data.getDatatypeClass() ) return false;
                if ( this.data.getDatatypeSize() != arg.data.getDatatypeSize() ) return false;
                if ( this.data.getDatatypeOrder() != arg.data.getDatatypeOrder() ) return false;
                if ( this.data.getDatatypeSign() != arg.data.getDatatypeSign() ) return false;
                /* <<< Java */
                return true;
            }

        }

    }

/* C# >>> *
}
/* <<< C# */
