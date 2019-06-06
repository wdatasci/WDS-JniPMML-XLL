using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.WDataSci.JniPMML
{

    public class FieldMDEnums
    {

        public enum eDTyp : int { Unk = 0, Dbl, Lng, Int, Dte, DTm, Str, VLS, Byt, Bln }

        public enum eRTyp : int { Unknown = 0, Output, Target, Feature, Cluster }

    }

    public static class FieldMDExt
    {

        public static FieldMDEnums.eDTyp FromInt(this FieldMDEnums.eDTyp self, int arg)
        {
            switch ( arg ) {
                case 1: return FieldMDEnums.eDTyp.Dbl;
                case 2: return FieldMDEnums.eDTyp.Lng;
                case 3: return FieldMDEnums.eDTyp.Int;
                case 4: return FieldMDEnums.eDTyp.Dte;
                case 5: return FieldMDEnums.eDTyp.DTm;
                case 6: return FieldMDEnums.eDTyp.Str;
                case 7: return FieldMDEnums.eDTyp.VLS;
                case 8: return FieldMDEnums.eDTyp.Byt;
                case 9: return FieldMDEnums.eDTyp.Bln;
                default: return FieldMDEnums.eDTyp.Unk;
            }
        }

        public static FieldMDEnums.eDTyp eDTyp_FromInt(int arg)
        {
            switch ( arg ) {
                case 1: return FieldMDEnums.eDTyp.Dbl;
                case 2: return FieldMDEnums.eDTyp.Lng;
                case 3: return FieldMDEnums.eDTyp.Int;
                case 4: return FieldMDEnums.eDTyp.Dte;
                case 5: return FieldMDEnums.eDTyp.DTm;
                case 6: return FieldMDEnums.eDTyp.Str;
                case 7: return FieldMDEnums.eDTyp.VLS;
                case 8: return FieldMDEnums.eDTyp.Byt;
                case 9: return FieldMDEnums.eDTyp.Bln;
                default: return FieldMDEnums.eDTyp.Unk;
            }
        }


        public static int AsInt(this FieldMDEnums.eDTyp self)
        {
            switch ( self ) {
                case FieldMDEnums.eDTyp.Dbl: return 1;
                case FieldMDEnums.eDTyp.Lng: return 2;
                case FieldMDEnums.eDTyp.Int: return 3;
                case FieldMDEnums.eDTyp.Dte: return 4;
                case FieldMDEnums.eDTyp.DTm: return 5;
                case FieldMDEnums.eDTyp.Str: return 6;
                case FieldMDEnums.eDTyp.VLS: return 7;
                case FieldMDEnums.eDTyp.Byt: return 8;
                case FieldMDEnums.eDTyp.Bln: return 9;
                default: return 0;
            }
        }


        public static int eDTyp_AsInt(this FieldMDEnums.eDTyp self)
        {
            switch ( self ) {
                case FieldMDEnums.eDTyp.Dbl: return 1;
                case FieldMDEnums.eDTyp.Lng: return 2;
                case FieldMDEnums.eDTyp.Int: return 3;
                case FieldMDEnums.eDTyp.Dte: return 4;
                case FieldMDEnums.eDTyp.DTm: return 5;
                case FieldMDEnums.eDTyp.Str: return 6;
                case FieldMDEnums.eDTyp.VLS: return 7;
                case FieldMDEnums.eDTyp.Byt: return 8;
                case FieldMDEnums.eDTyp.Bln: return 9;
                default: return 0;
            }
        }


        //typl is used with a string length needs to be passed out
        public static FieldMDEnums.eDTyp eDTyp_FromAlias(String arg, ref int[] typl)
        {
            String larg = arg.ToLower();
            //strip out any XSD namespaces
            if ( larg.Contains(":") ) {
                larg = larg.Substring(larg.LastIndexOf(":") + 1);
            }
            typl[0] = 0;
            switch ( larg ) {
                case "dbl":
                    return FieldMDEnums.eDTyp.Dbl;
                case "lng":
                    return FieldMDEnums.eDTyp.Lng;
                case "int":
                    return FieldMDEnums.eDTyp.Int;
                case "dte":
                    return FieldMDEnums.eDTyp.Dte;
                case "dtm":
                    return FieldMDEnums.eDTyp.DTm;
                case "vls":
                    return FieldMDEnums.eDTyp.VLS;
                case "str":
                    return FieldMDEnums.eDTyp.Str;
                case "byt":
                    return FieldMDEnums.eDTyp.Byt;
                case "bln":
                    return FieldMDEnums.eDTyp.Bln;

                case "nbr":
                case "number":
                case "numeric":
                case "double":
                case "float":
                case "decimal":
                case "dec":
                    return FieldMDEnums.eDTyp.Dbl;

                case "ulong":
                case "unsignedlong":
                case "int64":
                case "uint64":
                    return FieldMDEnums.eDTyp.Lng;

                case "short":
                case "unsignedshort":
                case "uint":
                case "int32":
                case "uint32":
                case "integer":
                case "unsignedint":
                case "unsignedinteger":
                case "negativeinteger":
                case "nonnegativeinteger":
                case "positiveinteger":
                case "nonpositiveinteger":
                    return FieldMDEnums.eDTyp.Int;

                case "string":
                case "varchar":
                case "normalizedstring":
                case "token":
                case "nmtoken":
                    return FieldMDEnums.eDTyp.VLS;

                case "char":
                    return FieldMDEnums.eDTyp.Str;

                case "byte":
                case "unsignedbyte":
                case "bytes":
                    return FieldMDEnums.eDTyp.Byt;
                case "Boolean":
                    return FieldMDEnums.eDTyp.Bln;

                default:
                    Boolean checklen = false;
                    FieldMDEnums.eDTyp rc = FieldMDEnums.eDTyp.Unk;
                    if ( larg.StartsWith("vls") ) {
                        checklen = true;
                        rc = FieldMDEnums.eDTyp.VLS;
                    }
                    else if ( larg.StartsWith("str") ) {
                        checklen = true;
                        rc = FieldMDEnums.eDTyp.Str;
                    }
                    if ( !checklen ) return rc;
                    if ( larg.EndsWith("restricted") ) {
                        typl[0] = -1;
                        return rc;
                    }
                    try {
                        int l = Int32.Parse(larg.Substring(3));
                        typl[0] = l;
                        return rc;
                    }
                    catch ( Exception e ) {
                        typl[0] = -1;
                        return rc;
                    }
            }
        }

        public static String toString(this FieldMDEnums.eDTyp self)
        {
            switch ( self ) {
                //case Dbl:
                case FieldMDEnums.eDTyp.Dbl:
                    return "Dbl-Double";
                //case Lng:
                case FieldMDEnums.eDTyp.Lng:
                    return "Lng-Long";
                //case Int:
                case FieldMDEnums.eDTyp.Int:
                    return "Int-Integer";
                //case Dte:
                case FieldMDEnums.eDTyp.Dte:
                    return "Dte-Date";
                //case DTm:
                case FieldMDEnums.eDTyp.DTm:
                    return "DTm-DateTime";
                //case Str:
                case FieldMDEnums.eDTyp.Str:
                    return "Str-Fixed Length String";
                //case VLS:
                case FieldMDEnums.eDTyp.VLS:
                    return "VLS-Variable Length String";
                //case Byt:
                case FieldMDEnums.eDTyp.Byt:
                    return "Byt-Byte Blob";
                //case Bln:
                case FieldMDEnums.eDTyp.Bln:
                    return "Bln-Boolean";
                default:
                    return "Unknown";
            }
        }

        public static Boolean equals(this FieldMDEnums.eDTyp self, FieldMDEnums.eDTyp arg)
        {
            return self.Equals(arg);
        }

        public static Boolean eDTyp_bIn(this FieldMDEnums.eDTyp self, params FieldMDEnums.eDTyp[] args)
        {
            foreach ( FieldMDEnums.eDTyp arg in args ) if ( self.Equals(arg) ) return true;
            return false;
        }

        public static Boolean bIn(this FieldMDEnums.eDTyp self, params FieldMDEnums.eDTyp[] args)
        {
            foreach ( FieldMDEnums.eDTyp arg in args ) if ( self.Equals(arg) ) return true;
            return false;
        }

        public static Boolean isString(this FieldMDEnums.eDTyp self)
        {
            return self.bIn(FieldMDEnums.eDTyp.VLS, FieldMDEnums.eDTyp.Str);
        }


        public static Boolean isNumeric(this FieldMDEnums.eDTyp self)
        {
            return self.bIn(FieldMDEnums.eDTyp.Dbl, FieldMDEnums.eDTyp.Lng, FieldMDEnums.eDTyp.Int, FieldMDEnums.eDTyp.Bln);
        }


        public static FieldMDEnums.eRTyp FromInt(this FieldMDEnums.eRTyp self, int arg)
        {
            switch ( arg ) {
                case 1: return FieldMDEnums.eRTyp.Output;
                case 2: return FieldMDEnums.eRTyp.Target;
                case 3: return FieldMDEnums.eRTyp.Feature;
                case 4: return FieldMDEnums.eRTyp.Cluster;
                default: return FieldMDEnums.eRTyp.Unknown;
            }
        }

        public static FieldMDEnums.eRTyp eRTyp_FromInt(int arg)
        {
            switch ( arg ) {
                case 1: return FieldMDEnums.eRTyp.Output;
                case 2: return FieldMDEnums.eRTyp.Target;
                case 3: return FieldMDEnums.eRTyp.Feature;
                case 4: return FieldMDEnums.eRTyp.Cluster;
                default: return FieldMDEnums.eRTyp.Unknown;
            }
        }


        public static int AsInt(this FieldMDEnums.eRTyp self)
        {
            switch ( self ) {
                case FieldMDEnums.eRTyp.Output: return 1;
                case FieldMDEnums.eRTyp.Target: return 2;
                case FieldMDEnums.eRTyp.Feature: return 3;
                case FieldMDEnums.eRTyp.Cluster: return 4;
                default: return 0;
            }
        }


        public static int eRTyp_AsInt(this FieldMDEnums.eRTyp self)
        {
            switch ( self ) {
                case FieldMDEnums.eRTyp.Output: return 1;
                case FieldMDEnums.eRTyp.Target: return 2;
                case FieldMDEnums.eRTyp.Feature: return 3;
                case FieldMDEnums.eRTyp.Cluster: return 4;
                default: return 0;
            }
        }


        //typl is used with a string length needs to be passed out
        public static FieldMDEnums.eRTyp eRTyp_FromAlias(String arg)
        {
            String larg = arg.ToLower();
            //strip out any XSD namespaces
            if ( larg.Contains(":") ) {
                larg = larg.Substring(larg.LastIndexOf(":") + 1);
            }
            switch ( larg ) {
                case "output":
                    return FieldMDEnums.eRTyp.Output;
                case "target":
                    return FieldMDEnums.eRTyp.Target;
                case "feature":
                    return FieldMDEnums.eRTyp.Feature;
                case "cluster":
                    return FieldMDEnums.eRTyp.Cluster;
                default:
                    return FieldMDEnums.eRTyp.Unknown;
            }
        }

        public static String toString(this FieldMDEnums.eRTyp self)
        {
            switch ( self ) {
                case FieldMDEnums.eRTyp.Output: return "Output";
                case FieldMDEnums.eRTyp.Target: return "Target";
                case FieldMDEnums.eRTyp.Feature: return "Feature";
                case FieldMDEnums.eRTyp.Cluster: return "Cluster";
                default:
                    return "Unknown";
            }
        }

        public static Boolean equals(this FieldMDEnums.eRTyp self, FieldMDEnums.eRTyp arg)
        {
            return self.Equals(arg);
        }

        public static Boolean eRTyp_bIn(this FieldMDEnums.eRTyp self, params FieldMDEnums.eRTyp[] args)
        {
            foreach ( FieldMDEnums.eRTyp arg in args ) if ( self.Equals(arg) ) return true;
            return false;
        }

        public static Boolean bIn(this FieldMDEnums.eRTyp self, params FieldMDEnums.eRTyp[] args)
        {
            foreach ( FieldMDEnums.eRTyp arg in args ) if ( self.Equals(arg) ) return true;
            return false;
        }
    }

}

