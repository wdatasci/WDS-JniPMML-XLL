/* Java >>> */
package com.WDataSci.JniPMML;

import com.WDataSci.WDS.Util;

/* <<< Java */
/* C# >>> *
using System;

using static com.WDataSci.WDS.JavaLikeExtensions;

namespace com.WDataSci.JniPMML
{
/* <<< C# */

/**
 * <p>FieldBaseMD contains the cross-package data field information.</p>
 * <p>It holds cached information for input and output processing and
 * provides any simplifications, for example, numeric types that are
 * not int, long, byte, or boolean, are taken as double.</p>
 * <p>On purpose, the enumerated type names are only 3 characters to
 * make the use explicit and not cause any confusion with XSD, PMML, SQL, or HDF5
 * types.</p>
 * <p>On the Java side, because of the <i>finalized</i> status of org.dmg.pmml.FieldName,
 * which is used as the column key for input and output columns of the jpmml evaluator,
 * handling of the mapping and cache of org.dmg.pmml.FieldNames is handled by the
 * FieldMD implementation of XDataFieldKeyInterface&lt;FieldName&gt;.</p>
 */
public class FieldBaseMD
{

    public String Name = null;
    public com.WDataSci.JniPMML.FieldMDEnums.eDTyp DTyp = com.WDataSci.JniPMML.FieldMDEnums.eDTyp.Unk;
    public com.WDataSci.JniPMML.FieldMDEnums.eRTyp RTyp = com.WDataSci.JniPMML.FieldMDEnums.eRTyp.Unknown;
    public int StringMaxLength = 0;  //chars fixed length for Str, max length for VLS
    public long ByteMaxLength = 0;
    public long ByteMemLength = 0;  //bytes used on a buffer (8 for VLen fields pointing to VLen space)

    //fields not check in .Equals
    public FieldMDEnums.eDTyp ExternalDTyp = FieldMDEnums.eDTyp.Unk; //used when incoming or outgoing DTyp differs
    public long ByteMemOffset = 0;  //might only used be used for less object-wrapped HDF5 interfaces
    public String Format = "";  //not yet fully implemented and passing in and out through Schema and Header info

    //Java
    public WranglerHDF5.HDF5DataType HDF5DataType = null;
    //C# public HDF5DataType HDF5DataType = null;

    public FieldBaseMD() { }

    public FieldBaseMD(String _Name, FieldMDEnums.eDTyp _DTyp)
    {
        this.Name = _Name;
        this.DTyp = _DTyp;
        if ( this.DTyp.bIn(FieldMDEnums.eDTyp.VLS, FieldMDEnums.eDTyp.Str) ) {
            this.StringMaxLength = Default.StringMaxLength;
            this.ByteMaxLength = 2 * Default.StringMaxLength;
        }
    }

    public FieldBaseMD(String Name, FieldMDEnums.eDTyp DTyp, int StringMaxLength)
    {
        this.Name = Name;
        this.DTyp = DTyp;
        if ( StringMaxLength > 0 ) {
            this.StringMaxLength = StringMaxLength;
            this.ByteMaxLength = 2L * StringMaxLength;
        }
        else if ( this.DTyp.bIn(FieldMDEnums.eDTyp.VLS, FieldMDEnums.eDTyp.Str) ) {
            this.StringMaxLength = Default.StringMaxLength;
            this.ByteMaxLength = Default.StringMaxLength;
        }
    }

    public FieldBaseMD(FieldBaseMD arg)
    throws com.WDataSci.WDS.WDSException, Exception
    {
        this.Name = arg.Name;
        this.DTyp = arg.DTyp;
        this.RTyp = arg.RTyp;
        this.StringMaxLength = arg.StringMaxLength;
        this.ByteMaxLength = arg.ByteMaxLength;
        if ( arg.HDF5DataType != null )
            this.HDF5DataType = new WranglerHDF5().new_HDF5DataType(arg);
        this.ExternalDTyp = arg.ExternalDTyp;
        this.ByteMemOffset = arg.ByteMemOffset;
        this.ByteMemLength = arg.ByteMemLength;
        this.Format = arg.Format;
    }

    public FieldBaseMD(String Name, int hclass, int hlength, int horder, int hsign)
    throws com.WDataSci.WDS.WDSException, Exception
    {
        this.Name = Name;
        WranglerHDF5 tmp = new WranglerHDF5();
        this.HDF5DataType = tmp.new_HDF5DataType(hclass, hlength, horder, hsign);
        this.DTyp = this.HDF5DataType.eDTyp();
        if ( this.DTyp.equals(FieldMDEnums.eDTyp.Str) ) {
            this.StringMaxLength = hlength;
            this.ByteMaxLength = 2L * hlength;
        }
        else if ( this.DTyp.equals(FieldMDEnums.eDTyp.VLS) ) {
            this.StringMaxLength = Default.StringMaxLength;
            this.ByteMaxLength = 2L * hlength;
        }
        else if ( this.DTyp.equals(FieldMDEnums.eDTyp.Byt) ) {
            this.ByteMaxLength = hlength;
        }
    }

    public boolean Equals(FieldBaseMD arg)
    {
        if ( !com.WDataSci.WDS.Util.MatchingNullityAndValueEquals(this.Name, arg.Name) ) return false;
        if ( !this.DTyp.equals(arg.DTyp) ) return false;
        if ( this.StringMaxLength != arg.StringMaxLength ) return false;
        if ( this.ByteMaxLength != arg.ByteMaxLength ) return false;
        if ( !Util.MatchingNullityAndValueEquals(this.Format, arg.Format) ) return false;

        //if ( !Util.MatchingNullity(this.MapKey, arg.MapKey) ) return false;
        //if ( this.MapKey != null && !this.MapKey.getValue().equals(arg.MapKey.getValue()) )
        //return false;

        if ( !Util.MatchingNullity(this.HDF5DataType, arg.HDF5DataType) ) return false;
        return this.HDF5DataType == null || this.HDF5DataType.Equals(arg.HDF5DataType);
    }

    public void Copy(FieldBaseMD arg)
    throws com.WDataSci.WDS.WDSException, Exception
    {
        this.Name = arg.Name;
        this.DTyp = arg.DTyp;
        this.StringMaxLength = arg.StringMaxLength;
        this.ByteMaxLength = arg.ByteMaxLength;
        this.ByteMemLength = arg.ByteMemLength;
        this.ByteMemOffset = arg.ByteMemOffset;
        this.Format = arg.Format;
        if ( arg.HDF5DataType == null ) this.HDF5DataType = null;
        else
            this.HDF5DataType = new WranglerHDF5().new_HDF5DataType(arg);
    }

    public long FLenByteLength()
    throws com.WDataSci.WDS.WDSException
    {
        if ( this.ByteMemLength > 0 ) return this.ByteMaxLength;
        return switch (this.DTyp) {
            //case FieldMDEnums.eDTyp.Dbl:
            //case FieldMDEnums.eDTyp.Dte:
            //case FieldMDEnums.eDTyp.DTm:
            //case FieldMDEnums.eDTyp.Lng:
            case Dbl, Dte, DTm, Lng, VLS ->
                //case FieldMDEnums.eDTyp.VLS:
                    8;
            case Int ->
                //case FieldMDEnums.eDTyp.Int:
                    4;
            case Str ->
                //case FieldMDEnums.eDTyp.Str:
                    this.ByteMaxLength;
            case Byt ->
                //case FieldMDEnums.eDTyp.Byt:
                    this.ByteMaxLength;
            case Bln ->
                //case FieldMDEnums.eDTyp.Bln:
                    1;
            default -> throw new com.WDataSci.WDS.WDSException("Error, un-implemented OutputColumn DataType !");
        };
    }

    public boolean isVLen()
    {
        return this.DTyp.bIn(FieldMDEnums.eDTyp.VLS, FieldMDEnums.eDTyp.Byt);
    }

    //Java
    @SuppressWarnings( "DuplicateBranchesInSwitch" )
    public void Consistency()
    throws com.WDataSci.WDS.WDSException
    {
        switch ( this.DTyp ) {
            case Dbl:
                //case FieldMDEnums.eDTyp.Dbl:
                this.StringMaxLength = 0;
                this.ByteMaxLength = 0;
                this.Format = "";
                this.ByteMemLength = 8;
                break;
            case Dte:
                //case FieldMDEnums.eDTyp.Dte:
                this.StringMaxLength = 0;
                this.ByteMaxLength = 0;
                this.Format = "YYYY-MM-DD";
                this.ByteMemLength = 8;
                break;
            case DTm:
                //case FieldMDEnums.eDTyp.DTm:
                this.StringMaxLength = 0;
                this.ByteMaxLength = 0;
                this.Format = "YYYY-MM-DD hh:mm:ss";
                this.ByteMemLength = 8;
                break;
            case Lng:
                //case FieldMDEnums.eDTyp.Lng:
                this.StringMaxLength = 0;
                this.ByteMaxLength = 0;
                this.Format = "";
                this.ByteMemLength = 8;
                break;
            case Int:
                //case FieldMDEnums.eDTyp.Int:
                this.StringMaxLength = 0;
                this.ByteMaxLength = 0;
                this.Format = "";
                this.ByteMemLength = 4;
                break;
            case Str:
                //case FieldMDEnums.eDTyp.Str:
            case VLS:
                //case FieldMDEnums.eDTyp.VLS:
                if ( this.StringMaxLength <= 0 ) {
                    if ( this.ByteMaxLength <= 0 ) {
                        this.StringMaxLength = Default.StringMaxLength;
                        this.ByteMaxLength = 2 * Default.StringMaxLength;
                    }
                    else
                        this.StringMaxLength = (int) (this.ByteMaxLength / 2);
                }
                else if ( this.ByteMaxLength <= 0 )
                    this.ByteMaxLength = 2L * this.StringMaxLength;
                //if ( this.ByteMaxLength == 0 && this.StringMaxLength > 0 )
                //    this.ByteMaxLength = 2L * this.StringMaxLength;
                this.Format = "";
                if ( this.DTyp.equals(FieldMDEnums.eDTyp.Str) )
                    this.ByteMemLength = this.ByteMaxLength;
                else
                    this.ByteMemLength = 8;
                break;
            case Byt:
                //case FieldMDEnums.eDTyp.Byt:
                this.StringMaxLength = 0;
                this.Format = "";
                this.ByteMemLength = 8;  //treated as variable Length
                break;
            case Bln:
                //case FieldMDEnums.eDTyp.Bln:
                this.StringMaxLength = 0;
                this.ByteMaxLength = 0;
                this.Format = "";
                this.ByteMemLength = 4;
                break;
            default:
                throw new com.WDataSci.WDS.WDSException("Error, un-implemented OutputColumn DataType !");
        }
    }

    public boolean isMappedToHDF5DataType()
    {
        return (this.HDF5DataType != null);
    }

    public FieldBaseMD MapToHDF5DataType(FieldMDEnums.eDTyp DTyp)
    throws com.WDataSci.WDS.WDSException, Exception
    {
        this.HDF5DataType = new WranglerHDF5().new_HDF5DataType(DTyp);
        return this;
    }

    public FieldBaseMD MapToHDF5DataType(FieldMDEnums.eDTyp DTyp, int nStringMaxLength, boolean anyVLen)
    throws com.WDataSci.WDS.WDSException, Exception
    {
        this.HDF5DataType = new WranglerHDF5().new_HDF5DataType(DTyp, nStringMaxLength, anyVLen);
        return this;
    }


    public FieldBaseMD MapToHDF5DataType(int hclass, int hlength, int horder, int hsign)
    throws com.WDataSci.WDS.WDSException, Exception
    {
        this.HDF5DataType = new WranglerHDF5().new_HDF5DataType(hclass, hlength, horder, hsign);
        return this;
    }


    /* C# >>> *
    public FieldBaseMD MapToHDF5DataType(System.Int64 arg)
    /* <<< C# */
    /* Java >>> */
    public FieldBaseMD MapToHDF5DataType(long arg)
    throws com.WDataSci.WDS.WDSException, Exception
    /* <<< Java */
    {
        this.HDF5DataType = new WranglerHDF5().new_HDF5DataType(arg);
        //this.HDF5DataType = new WranglerHDF5().HDF5DataType(arg);
        return this;
    }

    public static class Default
    {

        /* Java >>> */
        public final static int HeaderStringMaxLength = 64;  //default value for column/field name lengths
        public final static int StringMaxLength = 64;        //default value for strings, fixed length or variable

        /* <<< Java */

        /* C# >>> *
        public static int HeaderStringMaxLength = 64;  //default value for column/field name lengths
        public static int StringMaxLength = 64;        //default value for strings, fixed length or variable

        /* <<< C# */
    }

}
/* C# >>> *
}
/* <<< C# */
