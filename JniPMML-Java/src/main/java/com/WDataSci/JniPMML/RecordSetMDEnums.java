package com.WDataSci.JniPMML;

public class RecordSetMDEnums
{
    public enum eType
    {
        Unknown(0), Dlm(1), CSV(2), TXT(3), HDF5(4), DBB(5), XML(6), JSON(7), SQL(8);

        private final int val;

        eType(int arg)
        {
            this.val = arg;
        }

        public boolean isFlatFile()
        {
            return this.bIn(Dlm, CSV, TXT, XML, JSON);
        }

        public boolean isFile()
        {
            return this.bIn(Dlm, CSV, TXT, XML, JSON, HDF5);
        }

        public static eType FromInt(int arg)
        {
            switch (arg) {
                case 1:
                    return Dlm;
                case 2:
                    return CSV;
                case 3:
                    return TXT;
                case 4:
                    return HDF5;
                case 5:
                    return DBB;
                case 6:
                    return XML;
                case 7:
                    return JSON;
                case 8:
                    return SQL;
                default:
                    return Unknown;
            }
        }


        public static eType FromAlias(String arg)
        {
            switch (arg.toLowerCase()) {
                case "csv":
                case "comma-delimited":
                case "comma-separated":
                    return eType.CSV;
                case "txt":
                case "tab-delimited":
                case "tab-separated":
                    return eType.TXT;
                case "dlm":
                case "pipe":
                case "pipe-delimited":
                case "pipe-separated":
                case "delimited":
                    return eType.Dlm;
                case "hdf5":
                case "h5":
                    return eType.HDF5;
                case "xdatadbb":
                case "bytebuffer":
                case "bb":
                case "byte":
                case "bytes":
                    return eType.DBB;
                case "sql":
                    return eType.SQL;
                case "json":
                    return eType.JSON;
                case "xml":
                    return eType.XML;
                default:
                    return eType.Unknown;
            }
        }

        @Override
        public String toString()
        {
            switch (this) {
                case Dlm:
                    return "Dlm";
                case CSV:
                    return "CSV";
                case TXT:
                    return "TXT";
                case HDF5:
                    return "HDF5";
                case DBB:
                    return "DBB";
                case XML:
                    return "XML";
                case JSON:
                    return "JSON";
                case SQL:
                    return "SQL";
                default:
                    return "Unknown";
            }
        }

        public boolean bIn(eType... args)
        {
            for (eType arg : args) if ( this.equals(arg) ) return true;
            return false;
        }

        public boolean bIn(String... arg)
        {
            boolean found = false;
            for (int i = 0; !found && i < arg.length; i++) {
                switch (arg[i].toLowerCase()) {
                    case "dlm":
                    case "delimited":
                        if ( this.equals(eType.Dlm) ) found = true;
                        break;
                    case "csv":
                        if ( this.equals(eType.CSV) ) found = true;
                        break;
                    case "txt":
                        if ( this.equals(eType.TXT) ) found = true;
                        break;
                    case "hdf5":
                    case "h5":
                        if ( this.equals(eType.HDF5) ) found = true;
                        break;
                    case "xdatadbb":
                    case "bytebuffer":
                    case "bb":
                        if ( this.equals(eType.DBB) ) found = true;
                        break;
                    default:
                }
            }
            return false;
        }
    }

    public enum eMode
    {
        Unknown(0), Input(1), Output(2), Internal(3);
        private final int val;

        eMode(int arg)
        {
            this.val = arg;
        }

        public boolean Equals(eMode arg) {
            return this.equals(arg);
        }

        public boolean bIn(eMode... args)
        {
            for (eMode arg : args) if ( this.equals(arg) ) return true;
            return false;
        }

        @Override
        public String toString()
        {
            switch (this) {
                case Input:
                    return "InputMap";
                case Output:
                    return "OutputMap";
                case Internal:
                    return "InternalMap";
                default:
                    return "Unknown";
            }
        }
    }

    public enum eSchemaType
    {
        Unknown(0), XSD(1), XML(2), JSON(3), HDF5(4), DBB(5), RecordSetMD(6), SQL(7), NamingConvention(8);
        private final int val;

        eSchemaType(int arg)
        {
            this.val = arg;
        }

        public boolean bIn(eSchemaType... args)
        {
            for (eSchemaType arg : args) if ( this.equals(arg) ) return true;
            return false;
        }

        public static eSchemaType FromInt(int arg)
        {
            switch (arg) {
                case 1:
                    return XSD;
                case 2:
                    return XML;
                case 3:
                    return JSON;
                case 4:
                    return HDF5;
                case 5:
                    return DBB;
                case 6:
                    return RecordSetMD;
                case 7:
                    return SQL;
                case 8:
                    return NamingConvention;
                default:
                    return Unknown;
            }
        }

        public static eSchemaType FromAlias(String arg)
        {
            switch (arg.toLowerCase()) {
                case "xsd":
                case "xml-schema":
                    return eSchemaType.XSD;
                case "xml":
                    return eSchemaType.XML;
                case "json":
                    return eSchemaType.JSON;
                case "hdf5":
                case "h5":
                    return eSchemaType.HDF5;
                case "xdatadbb":
                case "bytebuffer":
                case "bb":
                case "byte":
                case "bytes":
                    return eSchemaType.DBB;
                case "xdatamap":
                    return eSchemaType.RecordSetMD;
                case "sql":
                    return eSchemaType.SQL;
                case "namingconvention":
                case "name":
                case "names":
                    return eSchemaType.NamingConvention;
                default:
                    return eSchemaType.Unknown;
            }
        }

        @Override
        public String toString()
        {
            switch (this) {
                case XSD:
                    return "XSD";
                case XML:
                    return "XML Signature";
                case JSON:
                    return "JSON";
                case HDF5:
                    return "HDF5 Metadata";
                case DBB:
                    return "DBB Layout";
                case RecordSetMD:
                    return "Internally generated RecordSetMD";
                case SQL:
                    return "SQL";
                case NamingConvention:
                    return "NamingConvention";
                default:
                    return "Unknown";
            }
        }
    }


}
