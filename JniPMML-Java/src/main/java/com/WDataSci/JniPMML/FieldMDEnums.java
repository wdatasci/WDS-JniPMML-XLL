package com.WDataSci.JniPMML;

public class FieldMDEnums
{
    public enum eDTyp
    {
        Unk(0), Dbl(1), Lng(2), Int(3), Dte(4), DTm(5), Str(6), VLS(7), Byt(8), Bln(9);
        private final int val;

        eDTyp(int arg)
        {
            this.val = arg;
        }

        public boolean isString() {
            return (this.equals(VLS) || this.equals(Str));
        }

        public boolean isVLenString() {
            return (this.equals(VLS));
        }

        public boolean isVLen() {
            return (this.equals(VLS) || this.equals(Byt));
        }

        public boolean isNumeric() {
            return (this.bIn(Dbl,Lng,Int,Bln));
        }


        public int AsInt()
        {
            switch (this) {
                case Dbl:
                    return 1;
                case Lng:
                    return 2;
                case Int:
                    return 3;
                case Dte:
                    return 4;
                case DTm:
                    return 5;
                case Str:
                    return 6;
                case VLS:
                    return 7;
                case Byt:
                    return 8;
                case Bln:
                    return 9;
                default:
                    return 0;
            }
        }


        public static eDTyp FromInt(int arg)
        {
            switch (arg) {
                case 1:
                    return Dbl;
                case 2:
                    return Lng;
                case 3:
                    return Int;
                case 4:
                    return Dte;
                case 5:
                    return DTm;
                case 6:
                    return Str;
                case 7:
                    return VLS;
                case 8:
                    return Byt;
                case 9:
                    return Bln;
                default:
                    return Unk;
            }
        }


        //typl is used with a string Length needs to be passed out
        public static eDTyp FromAlias(String arg, int[] typl)
        {
            String larg = arg.toLowerCase();
            //strip out any XSD namespaces
            if ( larg.contains(":") ) {
                larg = larg.substring(larg.lastIndexOf(":") + 1);
            }
            typl[0] = 0;
            switch (larg) {
                case "dbl":
                    return eDTyp.Dbl;
                case "lng":
                    return eDTyp.Lng;
                case "int":
                    return eDTyp.Int;
                case "dte":
                    return eDTyp.Dte;
                case "dtm":
                    return eDTyp.DTm;
                case "vls":
                    typl[0] = -1;
                    return eDTyp.VLS;
                case "str":
                    typl[0] = -1;
                    return eDTyp.Str;
                case "byt":
                    return eDTyp.Byt;
                case "bln":
                    return eDTyp.Bln;

                case "nbr":
                case "number":
                case "numeric":
                case "double":
                case "float":
                case "real":
                case "decimal":
                case "dec":
                case "money":
                case "smallmoney":
                    return eDTyp.Dbl;

                case "ulong":
                case "unsignedlong":
                case "int64":
                case "uint64":
                case "timestampnumeric":
                    return eDTyp.Lng;

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
                case "bigint":
                case "smallint":
                case "tinyint":
                    return eDTyp.Int;

                case "string":
                case "uniqueidentifier":
                case "varchar":
                case "nvarchar":
                case "normalizedstring":
                case "token":
                case "nmtoken":
                case "text":
                case "ntext":
                case "xml":
                case "xsd":
                case "xsl":
                    typl[0] = -1;
                    return eDTyp.VLS;

                case "char":
                case "nchar":
                    typl[0] = -1;
                    return eDTyp.Str;

                case "date":
                    return eDTyp.Dte;
                case "time":
                case "datetime":
                case "datetime2":
                case "datetimeoffset":
                case "smalldatetime":
                case "timestamp":
                    return eDTyp.DTm;

                case "byte":
                case "unsignedbyte":
                case "bytes":
                case "binary":
                case "varbinary":
                case "image":
                    typl[0] = -1;
                    return eDTyp.Byt;

                case "boolean":
                case "bool":
                case "bit":
                    return eDTyp.Bln;

                default:
                    boolean checklen = false;
                    eDTyp rc = eDTyp.Unk;
                    if ( larg.startsWith("vls") ) {
                        checklen = true;
                        rc = eDTyp.VLS;
                    }
                    else if ( larg.startsWith("str") ) {
                        checklen = true;
                        rc = eDTyp.Str;
                    }
                    if ( !checklen ) return rc;
                    if ( larg.endsWith("restricted") ) {
                        typl[0] = -1;
                        return rc;
                    }
                    try {
                        int l = Integer.parseInt(larg.substring(3));
                        typl[0] = l;
                        return rc;
                    } catch (Exception e) {
                        typl[0] = -1;
                        return rc;
                    }
            }
        }

        public String toVerboseString()
        {
            switch (this) {
                case Dbl:
                    return "Dbl-Double";
                case Lng:
                    return "Lng-Long";
                case Int:
                    return "Int-Integer";
                case Dte:
                    return "Dte-Date";
                case DTm:
                    return "DTm-DateTime";
                case Str:
                    return "Str-Fixed Length String";
                case VLS:
                    return "VLS-Variable Length String";
                case Byt:
                    return "Byt-Byte Blob";
                case Bln:
                    return "Bln-Boolean";
                default:
                    return "Unknown";
            }
        }
        @Override
        public String toString()
        {
            switch (this) {
                case Dbl:
                    return "Dbl";
                case Lng:
                    return "Lng";
                case Int:
                    return "Int";
                case Dte:
                    return "Dte";
                case DTm:
                    return "DTm";
                case Str:
                    return "Str";
                case VLS:
                    return "VLS";
                case Byt:
                    return "Byt";
                case Bln:
                    return "Bln";
                default:
                    return "Unk";
            }
        }

        public String ToString()
        {
            return this.toString();
        }

        public boolean bIn(eDTyp... args)
        {
            for (eDTyp arg : args) if ( this.equals(arg) ) return true;
            return false;
        }

    }

    public enum eRTyp
    {
        Unknown(0), Output(1), Target(2), Feature(3), Cluster(4);
        private final int val;

        eRTyp(int arg)
        {
            this.val = arg;
        }

        public int AsInt()
        {
            switch (this) {
                case Output:
                    return 1;
                case Target:
                    return 2;
                case Feature:
                    return 3;
                case Cluster:
                    return 4;
                default:
                    return 0;
            }
        }


        public static eRTyp FromInt(int arg)
        {
            switch (arg) {
                case 1:
                    return Output;
                case 2:
                    return Target;
                case 3:
                    return Feature;
                case 4:
                    return Cluster;
                default:
                    return Unknown;
            }
        }


        public static eRTyp FromAlias(String arg)
        {
            String larg = arg.toLowerCase();
            //strip out any XSD namespaces
            if ( larg.contains(":") ) {
                larg = larg.substring(larg.lastIndexOf(":") + 1);
            }
            switch ( larg ) {
                case "output":
                    return eRTyp.Output;
                case "target":
                    return eRTyp.Target;
                case "feature":
                    return eRTyp.Feature;
                case "cluster":
                    return eRTyp.Cluster;
                default:
                    return eRTyp.Unknown;
            }
        }

        public String toVerboseString()
        {
            switch (this) {
                case Output:
                    return "Output";
                case Target:
                    return "Target";
                case Feature:
                    return "Feature";
                case Cluster:
                    return "Cluster";
                default:
                    return "Unknown";
            }
        }

        @Override
        public String toString() { return this.toVerboseString(); }

        public String ToString() { return this.toVerboseString(); }

        public boolean bIn(eRTyp... args)
        {
            for (eRTyp arg : args) if ( this.equals(arg) ) return true;
            return false;
        }
    }

}

