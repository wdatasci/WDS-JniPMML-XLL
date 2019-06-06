using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.WDataSci.JniPMML {

    public class RecordSetMDEnums {

        public enum eType : int { Unknown = 0, Dlm, CSV, TXT, HDF5, DBB, XML, JSON, SQL }

        public enum eMode : int { Unknown = 0, Input, Output, Internal }

        public enum eSchemaType : int { Unknown = 0, XSD, XML, JSON, HDF5, DBB, XDataMap, SQL, NamingConvention }

    }

    public static class RecordSetMDExt {

        ///////////////////////////////////////////////////////////////////////

        public static RecordSetMDEnums.eType eType_FromInt(int arg) {
            switch ( arg ) {
                case 1: return RecordSetMDEnums.eType.Dlm;
                case 2: return RecordSetMDEnums.eType.CSV;
                case 3: return RecordSetMDEnums.eType.TXT;
                case 4: return RecordSetMDEnums.eType.HDF5;
                case 5: return RecordSetMDEnums.eType.DBB;
                case 6: return RecordSetMDEnums.eType.XML;
                case 7: return RecordSetMDEnums.eType.JSON;
                case 8: return RecordSetMDEnums.eType.SQL;
                default: return RecordSetMDEnums.eType.Unknown;
            }
        }

        public static Boolean isFlatFile(this RecordSetMDEnums.eType self)
        {
            return self.bIn(RecordSetMDEnums.eType.Dlm, RecordSetMDEnums.eType.CSV, RecordSetMDEnums.eType.TXT, RecordSetMDEnums.eType.XML, RecordSetMDEnums.eType.JSON);
        }

        public static int AsInt(this RecordSetMDEnums.eType self) {
            switch ( self ) {
                case RecordSetMDEnums.eType.Dlm: return 1;
                case RecordSetMDEnums.eType.CSV: return 2;
                case RecordSetMDEnums.eType.TXT: return 3;
                case RecordSetMDEnums.eType.HDF5: return 4;
                case RecordSetMDEnums.eType.DBB: return 5;
                case RecordSetMDEnums.eType.XML: return 6;
                case RecordSetMDEnums.eType.JSON: return 7;
                case RecordSetMDEnums.eType.SQL: return 8;
                default: return 0;
            }
        }


        public static RecordSetMDEnums.eType eType_FromAlias(String arg) {
            switch ( arg.ToLower() ) {
                case "csv":
                case "comma-delimited":
                case "comma-separated":
                    return RecordSetMDEnums.eType.CSV;
                case "txt":
                case "tab-delimited":
                case "tab-separated":
                    return RecordSetMDEnums.eType.TXT;
                case "dlm":
                case "pipe":
                case "pipe-delimited":
                case "pipe-separated":
                case "delimited":
                    return RecordSetMDEnums.eType.Dlm;
                case "hdf5":
                case "h5":
                    return RecordSetMDEnums.eType.HDF5;
                case "xdatadbb":
                case "bytebuffer":
                case "bb":
                case "byte":
                case "bytes":
                    return RecordSetMDEnums.eType.DBB;
                case "json":
                    return RecordSetMDEnums.eType.JSON;
                case "xml":
                    return RecordSetMDEnums.eType.XML;
                case "sql":
                    return RecordSetMDEnums.eType.SQL;
                default:
                    return RecordSetMDEnums.eType.Unknown;
            }
        }

        public static Boolean equals(this RecordSetMDEnums.eType self, RecordSetMDEnums.eType arg) { return self.Equals(arg); }

        public static Boolean bIn(this RecordSetMDEnums.eType self, params RecordSetMDEnums.eType[] args) {
            foreach ( RecordSetMDEnums.eType arg in args ) if ( self.Equals(arg) ) return true;
            return false;
        }

        public static Boolean bIn(this RecordSetMDEnums.eType self, params String[] args) {
            foreach ( String s in args ) if ( self.Equals(eType_FromAlias(s)) ) return true;
            return false;
        }

        public static String toString(this RecordSetMDEnums.eType self)
        {
            switch (self) {
                case RecordSetMDEnums.eType.Dlm:
                    return "Dlm";
                case RecordSetMDEnums.eType.CSV:
                    return "CSV";
                case RecordSetMDEnums.eType.TXT:
                    return "TXT";
                case RecordSetMDEnums.eType.HDF5:
                    return "HDF5";
                case RecordSetMDEnums.eType.DBB:
                    return "DBB";
                case RecordSetMDEnums.eType.XML:
                    return "XML";
                case RecordSetMDEnums.eType.JSON:
                    return "JSON";
                case RecordSetMDEnums.eType.SQL:
                    return "SQL";
                default:
                    return "Unknown";
            }
        }



        ///////////////////////////////////////////////////////////////////////

        public static RecordSetMDEnums.eMode eMode_FromInt(int arg) {
            switch ( arg ) {
                case 1: return RecordSetMDEnums.eMode.Input;
                case 2: return RecordSetMDEnums.eMode.Output;
                case 3: return RecordSetMDEnums.eMode.Internal;
                default: return RecordSetMDEnums.eMode.Unknown;
            }
        }


        public static int AsInt(this RecordSetMDEnums.eMode self) {
            switch ( self ) {
                case RecordSetMDEnums.eMode.Input: return 1;
                case RecordSetMDEnums.eMode.Output: return 2;
                case RecordSetMDEnums.eMode.Internal: return 3;
                default: return 0;
            }
        }


        public static RecordSetMDEnums.eMode eMode_FromAlias(String arg) {
            switch ( arg.ToLower() ) {
                case "input":
                case "in":
                    return RecordSetMDEnums.eMode.Input;
                case "output":
                case "out":
                    return RecordSetMDEnums.eMode.Output;
                case "internal":
                case "int":
                    return RecordSetMDEnums.eMode.Internal;
                default:
                    return RecordSetMDEnums.eMode.Unknown;
            }
        }

        public static Boolean equals(this RecordSetMDEnums.eMode self, RecordSetMDEnums.eMode arg) { return self.Equals(arg); }

        public static Boolean bIn(this RecordSetMDEnums.eMode self, params RecordSetMDEnums.eMode[] args) {
            foreach ( RecordSetMDEnums.eMode arg in args ) if ( self.Equals(arg) ) return true;
            return false;
        }

        public static Boolean bIn(this RecordSetMDEnums.eMode self, params String[] args) {
            Boolean found=false;
            foreach ( String s in args ) if ( self.Equals(eMode_FromAlias(s)) ) return true;
            return false;
        }



        ///////////////////////////////////////////////////////////////////////


        public static RecordSetMDEnums.eSchemaType eSchemaType_FromInt(int arg) {
            switch ( arg ) {
                case 1: return RecordSetMDEnums.eSchemaType.XSD;
                case 2: return RecordSetMDEnums.eSchemaType.XML;
                case 3: return RecordSetMDEnums.eSchemaType.JSON;
                case 4: return RecordSetMDEnums.eSchemaType.HDF5;
                case 5: return RecordSetMDEnums.eSchemaType.DBB;
                case 6: return RecordSetMDEnums.eSchemaType.XDataMap;
                case 7: return RecordSetMDEnums.eSchemaType.SQL;
                case 8: return RecordSetMDEnums.eSchemaType.NamingConvention;
                default: return RecordSetMDEnums.eSchemaType.Unknown;
            }
        }


        public static int AsInt(this RecordSetMDEnums.eSchemaType self) {
            switch ( self ) {
                case RecordSetMDEnums.eSchemaType.XSD: return 1;
                case RecordSetMDEnums.eSchemaType.XML: return 2;
                case RecordSetMDEnums.eSchemaType.JSON: return 3;
                case RecordSetMDEnums.eSchemaType.HDF5: return 4;
                case RecordSetMDEnums.eSchemaType.DBB: return 5;
                case RecordSetMDEnums.eSchemaType.XDataMap: return 6;
                case RecordSetMDEnums.eSchemaType.SQL: return 7;
                case RecordSetMDEnums.eSchemaType.NamingConvention: return 8;
                default: return 0;
            }
        }

        public static Boolean equals(this RecordSetMDEnums.eSchemaType self, RecordSetMDEnums.eSchemaType arg) { return self.Equals(arg); }

        public static RecordSetMDEnums.eSchemaType eSchemaType_FromAlias(String arg) {
            switch ( arg.ToLower() ) {
                case "xsd":
                case "xmlschema":
                    return RecordSetMDEnums.eSchemaType.XSD;
                case "xml":
                    return RecordSetMDEnums.eSchemaType.XML;
                case "json":
                    return RecordSetMDEnums.eSchemaType.JSON;
                case "hdf5":
                    return RecordSetMDEnums.eSchemaType.HDF5;
                case "xdatadbb":
                case "bytebuffer":
                case "bb":
                case "byte":
                case "bytes":
                    return RecordSetMDEnums.eSchemaType.DBB;
                case "xdatamap":
                    return RecordSetMDEnums.eSchemaType.XDataMap;
                case "sql":
                    return RecordSetMDEnums.eSchemaType.SQL;
                case "namingconvention":
                case "name":
                case "names":
                    return RecordSetMDEnums.eSchemaType.NamingConvention;
                default:
                    return RecordSetMDEnums.eSchemaType.Unknown;
            }
        }

        public static String toString(this RecordSetMDEnums.eSchemaType self)
        {
            switch (self) {
                case RecordSetMDEnums.eSchemaType.XSD:
                    return "XSD";
                case RecordSetMDEnums.eSchemaType.XML:
                    return "XML Signature";
                case RecordSetMDEnums.eSchemaType.JSON:
                    return "JSON";
                case RecordSetMDEnums.eSchemaType.HDF5:
                    return "HDF5 Metadata";
                case RecordSetMDEnums.eSchemaType.DBB:
                    return "DBB Layout";
                case RecordSetMDEnums.eSchemaType.XDataMap:
                    return "Internally generated RecordSetMD";
                case RecordSetMDEnums.eSchemaType.SQL:
                    return "SQL";
                case RecordSetMDEnums.eSchemaType.NamingConvention:
                    return "NamingConvention";
                default:
                    return "Unknown";
            }
        }
    
        public static Boolean bIn(this RecordSetMDEnums.eSchemaType self, params RecordSetMDEnums.eSchemaType[] args) {
            foreach ( RecordSetMDEnums.eSchemaType arg in args ) if ( self.Equals(arg) ) return true;
            return false;
        }

        public static Boolean bIn(this RecordSetMDEnums.eSchemaType self, params String[] args) {
            foreach ( String s in args ) if ( self.Equals(eSchemaType_FromAlias(s)) ) return true;
            return false;
        }

    }
}


