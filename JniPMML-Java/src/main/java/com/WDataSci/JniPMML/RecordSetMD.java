/* Java >>> */
package com.WDataSci.JniPMML;

import com.WDataSci.WDS.WDSException;
import org.dmg.pmml.FieldName;
import org.dmg.pmml.Model;
import org.w3c.dom.Document;

import javax.xml.parsers.DocumentBuilderFactory;
import java.io.PrintWriter;
import java.nio.ByteBuffer;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.List;
import java.util.Map;
import java.util.Set;

import static com.WDataSci.WDS.Util.MatchingNullity;
import static com.WDataSci.WDS.Util.MatchingNullityAndValueEquals;

/* <<< Java */
/* C# >>> *

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using com.WDataSci.WDS;
using static com.WDataSci.WDS.JavaLikeExtensions;
using static com.WDataSci.WDS.Util;

namespace com.WDataSci.JniPMML
{
/* <<< C# */

    public class RecordSetMD
    {

        public RecordSetMDEnums.eMode Mode = RecordSetMDEnums.eMode.Unknown;
        public RecordSetMDEnums.eType Type = RecordSetMDEnums.eType.Unknown;
        public RecordSetMDEnums.eSchemaType SchemaType = RecordSetMDEnums.eSchemaType.Unknown;

        public FieldMD[] Column = null;

        public WranglerFlatFile FileMatter = null;
        public __SchemaMatter SchemaMatter = null;
        public __ModeMatter ModeMatter = null;
        public WranglerHDF5 HDF5Matter = null;
        public WranglerDBB DBBMatter = null;

        public static long DefaultHeaderMaxStringLength = 64;
        public static long DefaultHeaderMaxStringByteLength = 128;
        public static long DefaultMaxStringLength = 32;
        public static long DefaultMaxStringByteLength = 64;

        //the map must be initiated with a mode
        public RecordSetMD(RecordSetMDEnums.eMode arg)
        {
            this.Mode = arg;
        }

        public void Dispose()
        throws com.WDataSci.WDS.WDSException, Exception
        {
            if ( this.FileMatter != null ) this.FileMatter.Dispose();
            this.FileMatter = null;
            if ( this.SchemaMatter != null ) this.SchemaMatter.Dispose();
            this.SchemaMatter = null;
            //this.ModeMatter.Dispose();
            this.ModeMatter = null;
            if ( this.HDF5Matter != null ) this.HDF5Matter.Dispose();
            this.HDF5Matter = null;
            if ( this.DBBMatter != null ) this.DBBMatter.Dispose();
            this.DBBMatter = null;
            /* C# >>> *
               GC.Collect();
               GC.WaitForPendingFinalizers();
               GC.Collect();
               GC.WaitForPendingFinalizers();
            /* <<< C# */
        }

        /* C# >>> *
           ~RecordSetMD() { this.Dispose(); }
        /* <<< C# */


        //Java
        public class __SchemaMatter extends WranglerXSD
                //C# public class __SchemaMatter : WranglerXSD
        {
            public String InputSchemaFileName = null;
            public String InputSchemaString = null;
            //Java
            public Document InputSchema = null;
            //C# public XmlDocument InputSchema=null;
            public String RecordSetElementName = null;
            public String RecordElementName = null;

            public boolean Equals(__SchemaMatter arg)
            {
                if ( !MatchingNullityAndValueEquals(this.InputSchemaFileName, arg.InputSchemaFileName) ) return false;
                if ( !MatchingNullityAndValueEquals(this.InputSchemaString, arg.InputSchemaString) ) return false;
                if ( !MatchingNullityAndValueEquals(this.RecordSetElementName, arg.RecordSetElementName) ) return false;
                if ( !MatchingNullityAndValueEquals(this.RecordElementName, arg.RecordElementName) ) return false;
                //Java
                if ( !this.InputSchema.equals(arg.InputSchema) ) return false;
                return true;
            }

            public void Dispose()
            {
                this.InputSchema = null;
            }
            /* C# >>> *
               ~__SchemaMatter()
               {
               this.Dispose();
               }
            /* <<< C# */

        }

        public class __ModeMatter
        {
            public int nInputFields = 0;
            public boolean bRepeatInputFields = false;
            public String CompositeNameDlm = "-";
            public String CompositeInputNameSuffix = "Input";
            public int OutputMaxStringLength = Default.StringMaxLength;

            public boolean Equals(__ModeMatter arg)
            {
                if ( this.nInputFields != arg.nInputFields ) return false;
                if ( this.bRepeatInputFields != arg.bRepeatInputFields ) return false;
                if ( !MatchingNullityAndValueEquals(this.CompositeNameDlm, arg.CompositeNameDlm) ) return false;
                if ( !MatchingNullityAndValueEquals(this.CompositeInputNameSuffix, arg.CompositeInputNameSuffix) )
                    return false;
                if ( this.OutputMaxStringLength != arg.OutputMaxStringLength ) return false;
                return true;
            }
        }

        /* Java >>> */
        public RecordSetMD cUsingCmdArguments(CmdArgs args)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            if ( this.Mode.equals(RecordSetMDEnums.eMode.Input) ) {

                this.cAs(RecordSetMDEnums.eType.FromAlias(args.aInputFileType));

                if ( this.Type.isFlatFile() ) {
                    this.FileMatter.hasHeaderRow = args.bInputHasHeaderRow;
                    this.FileMatter.Dlm = args.aInputFileDlm;
                    Path p = Paths.get(args.aInputFileName);
                    this.FileMatter.Path = p.getParent().toString();
                    this.FileMatter.FileName = p.getFileName().toString();
                    //set any non-standard delimiters
                    switch ( this.Type ) {
                        case Dlm:
                            //case FieldMDEnums.eDTyp.Dlm:
                            String ext = this.FileMatter.FileName.substring(this.FileMatter.FileName.lastIndexOf(".")).toLowerCase();
                            switch ( ext ) {
                                case ".csv":
                                    this.FileMatter.Dlm = ",";
                                    break;
                                case ".pipe":
                                case ".pipedlm":
                                    this.FileMatter.Dlm = "|";
                                    break;
                                case ".txt":
                                case ".tsv":
                                    this.FileMatter.Dlm = "\t";
                                    break;
                                default:
                                    this.FileMatter.Dlm = args.aInputFileDlm;
                                    break;
                            }
                            break;
                        case CSV:
                            //case FieldMDEnums.eDTyp.CSV:
                            this.FileMatter.Dlm = ",";
                            break;
                        case TXT:
                            //case FieldMDEnums.eDTyp.TXT:
                            this.FileMatter.Dlm = "\t";
                            break;
                        default:
                            this.FileMatter.Dlm = ",";
                            break;
                    }
                }

                if ( this.Type.bIn(RecordSetMDEnums.eType.HDF5) ) {
                    Path p = Paths.get(args.aInputFileName);
                    this.FileMatter.Path = p.getParent().toString();
                    this.FileMatter.FileName = p.getFileName().toString();

                    this.SchemaType = RecordSetMDEnums.eSchemaType.HDF5;
                    this.HDF5Matter = new WranglerHDF5();
                    this.HDF5Matter.DSName = args.aInputSchemaRecordSetName;
                    this.DBBMatter = new WranglerDBB();
                }
                else if ( args.aInputSchemaFileName != null &&
                        (args.aInputSchemaFileName.toLowerCase().endsWith(".xsd") || args.aInputFileType.toLowerCase().endsWith("xsd")) ) {
                    this.SchemaType = RecordSetMDEnums.eSchemaType.XSD;
                    this.SchemaMatter = new __SchemaMatter();
                    this.SchemaMatter.InputSchemaFileName = args.aInputSchemaFileName;
                    this.SchemaMatter.RecordSetElementName = args.aInputSchemaRecordSetName;
                }
                else if ( args.aInputSchemaFileName != null &&
                        (args.aInputSchemaFileName.toLowerCase().endsWith(".xml") || args.aInputFileType.toLowerCase().endsWith("xml")) ) {
                    this.SchemaType = RecordSetMDEnums.eSchemaType.XML;
                    throw new WDSException("Error InputSchemaType of xml not implemented yet!");
                }
                else if ( this.FileMatter.FileName.toLowerCase().endsWith(".json") ) {
                    this.SchemaType = RecordSetMDEnums.eSchemaType.JSON;
                    throw new WDSException("Error InputSchemaType of json not implemented yet!");
                }
                else {
                    this.SchemaType = RecordSetMDEnums.eSchemaType.FromAlias(args.aInputSchemaType);
                }

            }
            else if ( this.Mode.equals(RecordSetMDEnums.eMode.Output) ) {

                this.cAs(RecordSetMDEnums.eType.FromAlias(args.aOutputFileType));

                if ( this.Type.isFile() ) {

                    Path p = Paths.get(args.aOutputFileName);
                    this.FileMatter.Path = p.getParent().toString();
                    this.FileMatter.FileName = p.getFileName().toString();

                    if ( this.Type.isFlatFile() )
                        this.FileMatter.hasHeaderRow = args.bOutputHeaderRow;

                    //handle any delimiter confusion
                    if ( this.Type.bIn(RecordSetMDEnums.eType.CSV) ) this.FileMatter.Dlm = ",";
                    else if ( this.Type.bIn(RecordSetMDEnums.eType.TXT) ) this.FileMatter.Dlm = "\t";
                    else if ( this.Type.bIn(RecordSetMDEnums.eType.Dlm) ) {
                        if ( !args.aOutputFileDlm.equals("InputDlm") )
                            this.FileMatter.Dlm = args.aOutputFileDlm;
                        else if ( args.aOutputFileDlm.equals("InputDlm") ) {
                            RecordSetMDEnums.eType intype = RecordSetMDEnums.eType.FromAlias(args.aInputFileType);
                            if ( intype.bIn(RecordSetMDEnums.eType.CSV) ) this.FileMatter.Dlm = ",'";
                            else if ( intype.bIn(RecordSetMDEnums.eType.TXT) ) this.FileMatter.Dlm = "\t";
                            else this.FileMatter.Dlm = args.aInputFileDlm;
                        }
                        else
                            this.FileMatter.Dlm = ",";
                    }
                }

                if ( this.Type.equals(RecordSetMDEnums.eType.HDF5) ) {
                    this.HDF5Matter.DSName = args.aOutputHDF5DataSetName;
                }

                this.ModeMatter = new __ModeMatter();
                this.ModeMatter.bRepeatInputFields = args.bOutputInputFields;
                if ( this.ModeMatter.bRepeatInputFields ) {
                    this.ModeMatter.CompositeNameDlm = args.aOutputCompositeFieldDlm;
                    this.ModeMatter.CompositeInputNameSuffix = args.aOutputInputFieldNameSuffix;
                    this.ModeMatter.OutputMaxStringLength = args.aOutputHDF5FixedStringLength;
                }

                //for an output map, SchemaType is always the RecordSetMD (which can then write other schema out)
                this.SchemaType = RecordSetMDEnums.eSchemaType.RecordSetMD;

            }
            return this;
        }

        /* <<< Java */

        public int nColumns()
        {
            if ( this.Column == null )
                return 0;
            return this.Column.length;
        }

        public int nColumns(FieldMDEnums.eRTyp arg)
        {
            int rv = 0;
            int i = -1;
            for ( i = 0; i < this.Column.length; i++ ) {
                if ( this.Column[i].RTyp.bIn(arg) ) rv += 1;
            }
            return rv;
        }

        private RecordSetMD cFile(String aFileName)
        {
            if ( this.FileMatter == null ) this.FileMatter = new WranglerFlatFile();
            /* Java >>> */
            java.nio.file.Path p = Paths.get(aFileName);
            this.FileMatter.Path = p.getParent().toString();
            this.FileMatter.FileName = p.getFileName().toString();
            /* <<< Java */
            /* C# >>> *
               this.FileMatter.Path = System.IO.Path.GetDirectoryName(aFileName);
               this.FileMatter.FileName = System.IO.Path.GetFileName(aFileName);
            /* <<< C# */
            return this;
        }

        public RecordSetMD cFromFile(String aFileName) { return this.cFile(aFileName); }
        public RecordSetMD cToFile(String aFileName) { return this.cFile(aFileName); }

        public RecordSetMD cAs(RecordSetMDEnums.eType arg, RecordSetMDEnums.eSchemaType schema)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            this.Type = arg;
            this.SchemaType = schema;
            if ( this.SchemaType.bIn(RecordSetMDEnums.eSchemaType.XSD, RecordSetMDEnums.eSchemaType.XML) ) {
                this.SchemaMatter = new __SchemaMatter();
            }
            if ( this.ModeMatter == null && this.Mode.Equals(RecordSetMDEnums.eMode.Internal) ) {
                this.ModeMatter = new __ModeMatter();
                this.ModeMatter.bRepeatInputFields = false;
            }
            switch ( this.Type ) {
                case DBB:
                    //case RecordSetMDEnums.eType.XDataDBB:
                    this.DBBMatter = new WranglerDBB();
                    break;
                case HDF5:
                    //case RecordSetMDEnums.eType.HDF5:
                    this.SchemaType = RecordSetMDEnums.eSchemaType.HDF5;
                    this.FileMatter = new WranglerFlatFile();
                    this.DBBMatter = new WranglerDBB();
                    this.HDF5Matter = new WranglerHDF5();
                    break;
                case TXT:
                    //case RecordSetMDEnums.eType.TXT:
                    this.FileMatter = new WranglerFlatFile();
                    this.FileMatter.Dlm = "\t";
                    break;
                case Dlm:
                    //case RecordSetMDEnums.eType.Dlm:
                case CSV:
                    //case RecordSetMDEnums.eType.CSV:
                    this.FileMatter = new WranglerFlatFile();
                    this.FileMatter.Dlm = ",";
                    break;
                case XML:
                    //case RecordSetMDEnums.eType.XML:
                case JSON:
                    //case RecordSetMDEnums.eType.JSON:
                case SQL:
                    //case RecordSetMDEnums.eType.SQL:
                default:
                    throw new WDSException("Error RecordSetMD Type, %s, not implemented yet!");
            }
            return this;
        }

        public RecordSetMD cAs(RecordSetMDEnums.eType arg, RecordSetMDEnums.eSchemaType schema, Boolean isFileName, String schemadetails)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            this.cAs(arg, schema);
            if ( isFileName || (schemadetails != null && schemadetails.length() > 0) ) {
                switch ( this.SchemaType ) {
                    case XSD:
                    //case RecordSetMDEnums.eSchemaType.XSD:
                        if ( isFileName ) {
                            String s = com.WDataSci.WDS.Util.FetchFileAsString(schemadetails);
                            //Java
                            this.SchemaMatter.InputSchema = DocumentBuilderFactory.newInstance().newDocumentBuilder().parse(s);
                            //C# this.SchemaMatter.InputSchema = new XmlDocument();
                            //C# this.SchemaMatter.InputSchema.LoadXml(s);
                            this.SchemaMatter.InputSchemaFileName = schemadetails;
                            this.SchemaMatter.InputSchemaString = s;
                        }
                        else {
                            //Java
                            this.SchemaMatter.InputSchema = DocumentBuilderFactory.newInstance().newDocumentBuilder().parse(schemadetails);
                            //C# this.SchemaMatter.InputSchema = new XmlDocument();
                            //C# this.SchemaMatter.InputSchema.LoadXml(schemadetails);
                            this.SchemaMatter.InputSchemaString = schemadetails;
                        }
                        break;
                    default:
                        break;
                }
            }
            return this;
        }

        public RecordSetMD cAs(RecordSetMDEnums.eType arg)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            this.Type = arg;
            if ( this.ModeMatter == null && this.Mode.Equals(RecordSetMDEnums.eMode.Internal) ) {
                this.ModeMatter = new __ModeMatter();
                this.ModeMatter.bRepeatInputFields = false;
            }
            switch ( arg ) {
                case DBB:
                    //case RecordSetMDEnums.eType.XDataDBB:
                    this.SchemaType = RecordSetMDEnums.eSchemaType.DBB;
                    this.DBBMatter = new WranglerDBB();
                    break;
                case HDF5:
                    //case RecordSetMDEnums.eType.HDF5:
                    this.SchemaType = RecordSetMDEnums.eSchemaType.HDF5;
                    this.FileMatter = new WranglerFlatFile();
                    this.DBBMatter = new WranglerDBB();
                    this.HDF5Matter = new WranglerHDF5();
                    break;
                case TXT:
                    //case RecordSetMDEnums.eType.TXT:
                    this.SchemaType = RecordSetMDEnums.eSchemaType.XSD;
                    this.FileMatter = new WranglerFlatFile();
                    this.FileMatter.Dlm = "\t";
                    break;
                case Dlm:
                    //case RecordSetMDEnums.eType.Dlm:
                case CSV:
                    //case RecordSetMDEnums.eType.CSV:
                    this.SchemaType = RecordSetMDEnums.eSchemaType.XSD;
                    this.FileMatter = new WranglerFlatFile();
                    this.FileMatter.Dlm = ",";
                    break;
                case XML:
                    //case RecordSetMDEnums.eType.XML:
                case JSON:
                    //case RecordSetMDEnums.eType.JSON:
                default:
                    throw new WDSException("Error RecordSetMD Type, %s, not implemented yet!");
            }
            return this;
        }


        public RecordSetMD cWithHeaderRow()
        {
            if ( this.FileMatter == null ) this.FileMatter = new WranglerFlatFile();
            this.FileMatter.hasHeaderRow = true;
            return this;
        }

        public RecordSetMD cWithCompositeFieldNameDlm(String aCompositeFieldNameDlm)
        {
            if ( this.ModeMatter == null ) this.ModeMatter = new __ModeMatter();
            this.ModeMatter.CompositeNameDlm = aCompositeFieldNameDlm;
            return this;
        }

        public RecordSetMD cAsDlmFile(String aFileName)
        {
            this.cFile(aFileName);
            String lName = aFileName.toLowerCase();
            if ( lName.endsWith(".csv") ) {
                this.Type = RecordSetMDEnums.eType.CSV;
                this.FileMatter.Dlm = ",";
            }
            else if ( lName.endsWith(".txt") ) {
                this.Type = RecordSetMDEnums.eType.TXT;
                this.FileMatter.Dlm = "\t";
            }
            else if ( lName.endsWith(".pipe") ) {
                this.Type = RecordSetMDEnums.eType.Dlm;
                this.FileMatter.Dlm = "|";
            }
            else {
                this.Type = RecordSetMDEnums.eType.Dlm;
                this.FileMatter.Dlm = ",";
            }
            return this;
        }

        public RecordSetMD cAsDlmFile(String aFileName, String dlm)
        {
            this.cFile(aFileName);
            this.FileMatter.Dlm = dlm;
            switch ( dlm ) {
                case ",":
                    this.Type = RecordSetMDEnums.eType.CSV;
                    break;
                case "\t":
                    this.Type = RecordSetMDEnums.eType.TXT;
                    break;
                default:
                    this.Type = RecordSetMDEnums.eType.Dlm;
                    break;
            }
            return this;
        }

        public RecordSetMD cWithOutRepeatInputSet()
        {
            if ( this.ModeMatter == null ) this.ModeMatter = new __ModeMatter();
            this.ModeMatter.bRepeatInputFields = false;
            return this;
        }

        public RecordSetMD cRepeatInputSet()
        {
            if ( this.ModeMatter == null ) this.ModeMatter = new __ModeMatter();
            this.ModeMatter.bRepeatInputFields = true;
            return this;
        }

        public RecordSetMD cRepeatInputSetWithSuffix(String aInputFieldSuffix)
        {
            if ( this.ModeMatter == null ) this.ModeMatter = new __ModeMatter();
            this.ModeMatter.bRepeatInputFields = true;
            this.ModeMatter.CompositeInputNameSuffix = aInputFieldSuffix;
            return this;
        }

        public RecordSetMD cRepeatInputSetWithSuffix(String aInputFieldSuffix, String aCompositeFieldNameDlm)
        {
            if ( this.ModeMatter == null ) this.ModeMatter = new __ModeMatter();
            this.ModeMatter.bRepeatInputFields = true;
            this.ModeMatter.CompositeInputNameSuffix = aInputFieldSuffix;
            this.ModeMatter.CompositeNameDlm = aCompositeFieldNameDlm;
            return this;
        }

        public RecordSetMD cWithDlm(String arg)
        throws com.WDataSci.WDS.WDSException
        {
            if ( this.Type.bIn(RecordSetMDEnums.eType.HDF5, RecordSetMDEnums.eType.DBB, RecordSetMDEnums.eType.JSON, RecordSetMDEnums.eType.XML) )
                throw new WDSException("Error RecordSetMD must be a flat file type in order to set a delimiter!");
            if ( this.FileMatter == null ) this.FileMatter = new WranglerFlatFile();
            this.FileMatter.Dlm = arg;
            return this;
        }

        public RecordSetMD cWithDelimiter(String arg)
        throws com.WDataSci.WDS.WDSException
        { return this.cWithDlm(arg); }


        public long nHeaderStringMaxLength()
        {
            if ( this.DBBMatter != null )
                return this.DBBMatter.Header.MaxStringLength;
            return RecordSetMD.DefaultHeaderMaxStringLength;
        }

        public long nHeaderByteMaxLength()
        {
            if ( this.DBBMatter != null )
                return this.DBBMatter.Header.MaxStringByteLength;
            return RecordSetMD.DefaultHeaderMaxStringByteLength;
        }

        public boolean isModeValid()
        throws com.WDataSci.WDS.WDSException, Exception
        {
            if ( this.Type.equals(RecordSetMDEnums.eType.HDF5) )
                throw new WDSException("Error in RecordSetMD.cSetHeaderBuffer, not available for HDF5 (see unfinished TODO-s in HDF java code)");
            if ( this.DBBMatter == null ) this.DBBMatter = new WranglerDBB();
            return true;
        }

        public RecordSetMD cSetHeaderBufferAs(DBB arg, int nRecords, int nRecordCoreLength, int nRecordVariableLength)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            this.isModeValid();
            this.DBBMatter.cSetHeaderBufferAs(arg, nRecords, nRecordCoreLength, nRecordVariableLength);
            return this;
        }

        public RecordSetMD cSetHeaderBufferFrom(DBB arg)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            this.isModeValid();
            this.DBBMatter.cSetHeaderBufferFrom(arg);
            return this;
        }


        /* Java >>> */
        public RecordSetMD cSetHeaderBufferAs(ByteBuffer arg, int nRecords, int nRecordCoreLength, int nRecordVariableLength)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            this.isModeValid();
            this.DBBMatter.cSetHeaderBufferAs(new DBB(arg), nRecords, nRecordCoreLength, nRecordVariableLength);
            return this;
        }

        public RecordSetMD cSetHeaderBufferFrom(ByteBuffer arg)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            this.isModeValid();
            this.DBBMatter.cSetHeaderBufferFrom(new DBB(arg));
            return this;
        }

        public RecordSetMD cSetRecordSetBufferAs(ByteBuffer arg, long nRecords, long nRecordCoreLength,
                                                 long nRecordVariableLength, long nCoreLength, long nTotalLength)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            this.isModeValid();
            this.DBBMatter.cSetRecordSetBufferAs(new DBB(arg), nRecords, nRecordCoreLength, nRecordVariableLength, nCoreLength, nTotalLength);
            return this;
        }

        public RecordSetMD cSetRecordSetBufferAs(ByteBuffer arg)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            this.isModeValid();
            this.DBBMatter.cSetRecordSetBufferAs(new DBB(arg));
            return this;
        }

        public RecordSetMD cSetRecordSetBufferFrom(ByteBuffer arg)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            this.isModeValid();
            long nTotalLength = arg.capacity();
            if ( nTotalLength < 40 )
                throw new WDSException("Error in RecordSetMD.cSetBuffer, buffer size insufficient!");
            this.DBBMatter.cSetRecordSetBufferFrom(new DBB(arg));

            long[] csize = new long[1];
            long[] hsize = new long[1];
            long[] rsize = new long[1];
            long[] cleadsize = new long[1];
            long[] hleadsize = new long[1];
            long[] hflensize = new long[1];
            long[] hvlensize = new long[1];
            long[] rleadsize = new long[1];
            long[] rflensize = new long[1];
            long[] rvlensize = new long[1];

            this.DBBMatter.mBytesRequired(this, this.DBBMatter.RecordSet.Buffer.nRecords
                    , csize, hsize, rsize
                    , cleadsize
                    , hleadsize, hflensize, hvlensize
                    , rleadsize, rflensize, rvlensize
            );
            if ( this.DBBMatter.RecordSet.Buffer.nRecordFLenBytes != rflensize[0]
                    || this.DBBMatter.RecordSet.Buffer.nRecordVLenBytes != rvlensize[0]
                    || this.DBBMatter.RecordSet.Buffer.nDBBFLenBytes != rflensize[0] * this.DBBMatter.RecordSet.Buffer.nRecords
                    || this.DBBMatter.RecordSet.Buffer.nDBBVLenBytes != rvlensize[0] * this.DBBMatter.RecordSet.Buffer.nRecords
            )
                throw new com.WDataSci.WDS.WDSException("Error, RecordSet sizes do not match previously provided Header Information");

            return this;
        }
        /* <<< Java */

        public RecordSetMD cSetRecordSetBufferAs(DBB arg, long nRecords, long nRecordCoreLength,
                                                 long nRecordVariableLength, long nCoreLength, long nTotalLength)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            this.isModeValid();
            this.DBBMatter.cSetRecordSetBufferAs(arg, nRecords, nRecordCoreLength, nRecordVariableLength, nCoreLength, nTotalLength);
            return this;
        }

        public RecordSetMD cSetRecordSetBufferAs(DBB arg)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            this.isModeValid();
            this.DBBMatter.cSetRecordSetBufferAs(arg);
            return this;
        }

        public RecordSetMD cSetRecordSetBufferFrom(DBB arg)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            this.isModeValid();
            long nTotalLength = arg.Length;
            if ( nTotalLength < 40 )
                throw new WDSException("Error in RecordSetMD.cSetBuffer, buffer size insufficient!");
            this.DBBMatter.cSetRecordSetBufferFrom(arg);
            return this;
        }

        public RecordSetMD cWithRecordSetElementName(String arg)
        {
            if ( this.SchemaType.equals(RecordSetMDEnums.eSchemaType.XSD) ) {
                if ( this.SchemaMatter == null )
                    this.SchemaMatter = new __SchemaMatter();
                this.SchemaMatter.RecordSetElementName = new String(arg);
            }
            return this;
        }

        public RecordSetMD cWithRecordSetAndRecordElementNames(String arg, String arg1)
        {
            if ( this.SchemaType.equals(RecordSetMDEnums.eSchemaType.XSD) ) {
                if ( this.SchemaMatter == null )
                    this.SchemaMatter = new __SchemaMatter();
                this.SchemaMatter.RecordSetElementName = new String(arg);
                this.SchemaMatter.RecordElementName = new String(arg1);
            }
            return this;
        }

        public RecordSetMD cWithRecordElementName(String arg)
        {
            if ( this.SchemaType.equals(RecordSetMDEnums.eSchemaType.XSD) ) {
                if ( this.SchemaMatter == null )
                    this.SchemaMatter = new __SchemaMatter();
                this.SchemaMatter.RecordElementName = new String(arg);
            }
            return this;
        }


        public RecordSetMD cWithDataSetName(String arg)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            if ( this.Type.equals(RecordSetMDEnums.eType.HDF5) ) {
                if ( this.HDF5Matter == null ) this.HDF5Matter = new WranglerHDF5();
                this.HDF5Matter.DSName = arg;
            }
            return this;
        }

        public RecordSetMD mCopyColumnsFrom(RecordSetMD arg)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            this.Column = null;
            this.Column = new FieldMD[arg.nColumns()];
            for ( int jj = 0; jj < arg.nColumns(); jj++ ) {
                this.Column[jj] = new FieldMD(arg.Column[jj]);
                this.Column[jj].Consistency();
            }
            return this;
        }

        public boolean Equals(RecordSetMD arg, boolean bIgnoreMode)
        {
            if ( !bIgnoreMode && !this.Mode.equals(arg.Mode) ) return false;
            if ( !this.Type.equals(arg.Type) ) return false;
            if ( !this.SchemaType.equals(arg.SchemaType) ) return false;

            if ( this.Column == null ) {
                if ( arg.Column != null ) return false;
            }
            else {
                if ( arg.Column == null ) return false;
                if ( this.nColumns() != arg.nColumns() ) return false;
            }
            for ( int jj = 0; jj < this.nColumns(); jj++ ) {
                if ( !this.Column[jj].Equals(arg.Column[jj]) ) return false;
            }

            if ( !MatchingNullity(this.FileMatter, arg.FileMatter) ) return false;
            if ( this.FileMatter != null && !this.FileMatter.Equals(arg.FileMatter) ) return false;

            if ( !MatchingNullity(this.SchemaMatter, arg.SchemaMatter) ) return false;
            if ( this.SchemaMatter != null && !this.SchemaMatter.Equals(arg.SchemaMatter) ) return false;

            if ( !bIgnoreMode ) {
                if ( !MatchingNullity(this.ModeMatter, arg.ModeMatter) ) return false;
                if ( this.ModeMatter != null && !this.ModeMatter.Equals(arg.ModeMatter) ) return false;
            }

            return true;

        }

        public RecordSetMD mReadMapFor(JniPMMLItem aJniPMMLItem, PrintWriter pw, boolean bFillDictionaryNames)
        throws com.WDataSci.WDS.WDSException
        {
            try {

                if ( !this.Mode.bIn(RecordSetMDEnums.eMode.Input, RecordSetMDEnums.eMode.Internal) )
                    throw new WDSException("Error, wrong mode for RecordSetMD.mReadMapFor(JniPMML)!");

                if ( this.SchemaType.equals(RecordSetMDEnums.eSchemaType.XSD) ) {
                    this.SchemaMatter.mReadMapFor(this, aJniPMMLItem, pw, bFillDictionaryNames);
                }
                else if ( this.SchemaType.equals(RecordSetMDEnums.eSchemaType.NamingConvention) && this.Type.isFlatFile() ) {
                    this.FileMatter.mReadMapFor(this, aJniPMMLItem, pw, bFillDictionaryNames);
                }
                else if ( this.Type.equals(RecordSetMDEnums.eType.HDF5) ) {
                    this.HDF5Matter.mReadMapFor(this, aJniPMMLItem, pw, bFillDictionaryNames);
                }
                else if ( this.Type.equals(RecordSetMDEnums.eType.DBB) ) {
                    this.DBBMatter.mReadMap(this, aJniPMMLItem, pw, bFillDictionaryNames);
                }

                if ( pw != null ) pw.printf("leaving RecordSetMD.mReadMapFor constructor\n");
                if ( pw != null ) pw.flush();
                return this;
            }
            catch ( Exception e ) {
                throw new WDSException("Error in RecordSetMD.mReadMapFor", e);
            }
        }

        public RecordSetMD mColumnConsistency()
        throws com.WDataSci.WDS.WDSException
        {
            for ( int jj = 0; jj < this.Column.length; jj++ )
                this.Column[jj].Consistency();
            return this;
        }

        //Java
        public RecordSetMD mPrepForOutput(RecordSetMD aInputRecordSetMD, JniPMMLItem aJniPMML, List<Map<FieldName, Object>> Results)
        //C# public RecordSetMD mPrepForOutput<T>(RecordSetMD aInputRecordSetMD, JniPMMLItem aJniPMML, List<Map<T, Object>> Results)
        throws Exception
        {

            int i = -1;
            int j = -1;
            int k = -1;
            int jj = -1;

            int nInputMap = aInputRecordSetMD.nColumns();

            //C# throw new com.WDataSci.WDS.WDSException("Error, not implemented on the C# side");

            /* Java >>> */
            //Output
            List<Model> m = aJniPMML.PMMLMatter.Doc.getModels();
            org.dmg.pmml.Output mo = m.get(0).getOutput();
            List<org.dmg.pmml.OutputField> mol = mo.getOutputFields();

            //Java
            Set<FieldName> ks = Results.get(0).keySet();
            //Java
            FieldName[] ksa = new FieldName[ks.size()];
            //Java
            ks.toArray(ksa);
            //C# FieldName[] ksa = Results.get(0).keyArray();

            int nResultColumns = ks.size();
            int nColumns = nResultColumns;
            int nRows = Results.size();

            if ( this.ModeMatter == null ) this.ModeMatter = new __ModeMatter();

            if ( this.ModeMatter.bRepeatInputFields ) {
                nColumns += nInputMap;
                this.ModeMatter.nInputFields = nInputMap;
                this.Column = new FieldMD[nColumns];
                for ( j = 0; j < nInputMap; j++ ) {
                    this.Column[j] = new FieldMD(aInputRecordSetMD.Column[j]);
                    this.Column[j].Name = aInputRecordSetMD.Column[j].Name + this.ModeMatter.CompositeNameDlm + this.ModeMatter.CompositeInputNameSuffix;
                }
                jj = nInputMap;
            }
            else {
                this.Column = new FieldMD[nColumns];
                jj = 0;
            }

            for ( k = 0, j = jj; k < nResultColumns; k++, j++ ) {
                this.Column[j] = new FieldMD();
                this.Column[j].Name = ksa[k].toString();
                this.Column[j].MapToMapKey(ksa[k]);

                org.dmg.pmml.OutputField of = mol.get(k);
                org.dmg.pmml.DataType ofdtyp = of.getDataType();

                if ( nResultColumns != mol.size() )
                    throw new WDSException("Error, difference between result field count and model output field count!");

                //if the output field does not have a type, does it match an input field? This can happen with a result feature.
                boolean found = false;
                if ( ofdtyp == null ) {
                    for ( i = 0; !found && i < nInputMap; i++ ) {
                        if ( aInputRecordSetMD.Column[i].hasMapKey() && aInputRecordSetMD.Column[i].MapKey.getValue().equals(this.Column[j].Name) ) {
                            found = true;
                            this.Column[j].Copy(aInputRecordSetMD.Column[i]);
                            break;
                        }
                    }
                    if ( found && of.getResultFeature() != null ) {
                        this.Column[j].Name = this.Column[j].Name + this.ModeMatter.CompositeNameDlm + of.getResultFeature().toString();
                    }
                }
                if ( !found ) {
                    //If not found as an input field or a feature of one, extract the rest of the X mapping info from the PMML
                    if ( of.getDataType().equals(org.dmg.pmml.DataType.DOUBLE) || of.getDataType().equals(org.dmg.pmml.DataType.FLOAT) ) {
                        this.Column[j].DTyp = FieldMDEnums.eDTyp.Dbl;
                    }
                    else if ( of.getDataType().equals(org.dmg.pmml.DataType.INTEGER) ) {
                        this.Column[j].DTyp = FieldMDEnums.eDTyp.Int;
                        //there may not be a long PMML output type, double check if field is named like an input long
                        for ( found = false, i = 0; !found && i < nInputMap; i++ ) {
                            if ( this.Column[i].hasMapKey() && this.Column[i].MapKey.getValue().equals(this.Column[j].Name) ) {
                                found = true;
                                if ( this.Column[i].DTyp.equals(FieldMDEnums.eDTyp.Lng) ) {
                                    this.Column[j].DTyp = FieldMDEnums.eDTyp.Lng;
                                }
                            }
                        }
                    }
                    else if ( of.getDataType().equals(org.dmg.pmml.DataType.DATE) ) {
                        this.Column[j].DTyp = FieldMDEnums.eDTyp.Dte;
                    }
                    else if ( of.getDataType().equals(org.dmg.pmml.DataType.DATE_TIME) ) {
                        this.Column[j].DTyp = FieldMDEnums.eDTyp.DTm;
                    }
                    else if ( of.getDataType().equals(org.dmg.pmml.DataType.STRING) ) {
                        this.Column[j].DTyp = FieldMDEnums.eDTyp.VLS;
                    }
                    else if ( of.getDataType().equals(org.dmg.pmml.DataType.BOOLEAN) ) {
                        throw new WDSException("Error, OutputColumn DataType for boolean not implemented!");
                    }
                    else {
                        throw new WDSException("Error, un-implemented OutputColumn DataType !");
                    }
                }
            }

            return this;
            /* <<< Java */
        }

        /* C# >>> *
           public RecordSetMD mWritePrepFor(int nRows)
               throws com.WDataSci.WDS.WDSException
           {
           this.DBBMatter.mWritePrepFor(this, nRows);
           return this;
           }

           public void mBytesRequired(long nRecords, out long csize, out long hsize, out long rsize
           , out long cleadsize
           , out long hleadsize, out long hflensize, out long hvlensize
           , out long rleadsize, out long rflensize, out long rvlensize
           )
           {
           if ( this.DBBMatter == null ) this.DBBMatter = new WranglerDBB();
           this.DBBMatter.mBytesRequired(this, nRecords, out csize, out hsize, out rsize
           , out cleadsize
           , out hleadsize, out hflensize, out hvlensize
           , out rleadsize, out rflensize, out rvlensize
           );
           }

           public void mBytesRequired(long nRecords, out long rsize, out long rflensize, out long rvlensize)
           {
           if ( this.DBBMatter == null ) this.DBBMatter = new WranglerDBB();
           this.DBBMatter.mBytesRequired(this, nRecords, out rsize, out rflensize, out rvlensize);
           }

        /* <<< C# */

        public void mUpdateWithPMMLSchema(JniPMMLItem aJniPMML)
        throws com.WDataSci.WDS.WDSException
        {
            try {

                /* Java >>> */
                FieldName[] lFieldNames = aJniPMML.PMMLDataFieldNames();
                String[] lFieldStringNames = aJniPMML.PMMLDataFieldStringNames();
                int nDataFieldNames = lFieldNames.length;

                for ( int i = 0; i < this.Column.length; i++ ) {
                    FieldMD cm = this.Column[i];
                    //Search for PMML DataFieldName map
                    for ( int j = 0; j < nDataFieldNames; j++ ) {
                        if ( cm.Name.equals(lFieldStringNames[j]) ) {
                            cm.MapToMapKey(lFieldNames[j]);
                            break;
                        }
                    }
                }
                /* <<< Java */

            }
            catch ( Exception e ) {
                throw new WDSException("Error, RecordSetMD.mUpdateWithPMMLSchema:", e);
            }

        }

        public int mWriteMapToBuffer()
        throws com.WDataSci.WDS.WDSException
        {
            this.DBBMatter.mWriteMap(this); //, nColumns, nColumnNameMaxByteLength;
            return 0;
        }


        /* Java >>> */
        public int mWriteMapToBuffer(ByteBuffer arg
        )
        throws com.WDataSci.WDS.WDSException
        {
            this.DBBMatter.mWriteMap(this); //, nColumns, nColumnNameMaxByteLength;
            return 0;
        }
        /* <<< Java */

    }
    /* C# >>> *
}
/* <<< C# */
