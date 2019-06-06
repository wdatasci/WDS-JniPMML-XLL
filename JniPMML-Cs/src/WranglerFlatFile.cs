/* Java >>> * 
package com.WDataSci.JniPMML;

import org.apache.commons.csv.CSVFormat;
import org.apache.commons.csv.CSVParser;
import org.apache.commons.csv.CSVPrinter;
import org.apache.commons.csv.CSVRecord;
import org.dmg.pmml.FieldName;

import java.io.BufferedWriter;
import java.io.FileReader;
import java.io.PrintWriter;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.*;

import static com.WDataSci.WDS.Util.PathAndName;
import static com.WDataSci.WDS.Util.MatchingNullityAndValueEquals;
import static com.WDataSci.WDS.Util.bIn;

/* <<< Java */

/* C# >>> */

using System;
using System.IO;
using System.Collections.Generic;

using CsvHelper;

using com.WDataSci.WDS;
using static com.WDataSci.WDS.JavaLikeExtensions;
using static com.WDataSci.WDS.Util;

using MOIE = Microsoft.Office.Interop.Excel;

namespace com.WDataSci.JniPMML
{

    /* <<< C# */

    public class WranglerFlatFile
    {
        public String Path = null;
        public String FileName = null;
        public String Dlm = "";
        public Boolean hasHeaderRow = false;

        //only used to hold reader if schema is implied by the header
        //Java public CSVParser __CSVParser = null;
        //Java public Iterator<CSVRecord> __CSVParserIterator = null;
        //C#
        public StreamReader __StreamReader = null;
        //C#
        public CsvParser __CSV = null;

        /* C# >>> */
        ~WranglerFlatFile()
        {
            this.Dispose();
        }
        /* <<< C# */

        public void Dispose()
        {
            /* Java >>> *
            this.__CSVParser = null;
            this.__CSVParserIterator = null;
            /* <<< Java */
            /* C# >>> */
            this.__CSV = null;
            this.__StreamReader = null;
            /* <<< C# */
        }

        public Boolean Equals(WranglerFlatFile arg)
        {
            if ( !MatchingNullityAndValueEquals(this.Path, arg.Path) ) return false;
            if ( !MatchingNullityAndValueEquals(this.FileName, arg.FileName) ) return false;
            if ( !MatchingNullityAndValueEquals(this.Dlm, arg.Dlm) ) return false;
            if ( this.hasHeaderRow != arg.hasHeaderRow ) return false;
            return true;
        }

        public void cPointToFile(RecordSetMD aInputRecordSetMD, String aPath, String aFileName, Boolean hasHeaderRow, String dlm)
        //throws com.WDataSci.WDS.WDSException
        {
            if ( aInputRecordSetMD.FileMatter == null )
                aInputRecordSetMD.FileMatter = new WranglerFlatFile();
            aInputRecordSetMD.FileMatter.hasHeaderRow = hasHeaderRow;
            aInputRecordSetMD.FileMatter.Dlm = dlm;
            this.Path = new_String(aPath);
            this.FileName = new_String(aFileName);
            aInputRecordSetMD.FileMatter.Path = new_String(this.Path);
            aInputRecordSetMD.FileMatter.FileName = new_String(this.FileName);
        }

        public void cPointToFile(RecordSetMD aInputRecordSetMD, String aFileName, Boolean hasHeaderRow, String dlm)
        //throws com.WDataSci.WDS.WDSException
        {
            if ( aInputRecordSetMD.FileMatter == null )
                aInputRecordSetMD.FileMatter = new WranglerFlatFile();
            aInputRecordSetMD.FileMatter.hasHeaderRow = hasHeaderRow;
            aInputRecordSetMD.FileMatter.Dlm = dlm;
            /* Java >>> *
               java.nio.file.Path p = Paths.get(aFileName);
               this.Path = p.getParent().toString();
               this.FileName = p.getFileName().toString();
            /* <<< Java */
            /* C# >>> */
            this.Path = System.IO.Path.GetDirectoryName(aFileName);
            this.FileName = System.IO.Path.GetFileName(aFileName);
            /* <<< C# */
            aInputRecordSetMD.FileMatter.Path = new_String(this.Path);
            aInputRecordSetMD.FileMatter.FileName = new_String(this.FileName);
        }

        public void mReadMapFor(RecordSetMD aRecordSetMD, JniPMMLItem aJniPMML, PrintWriter pw, Boolean bFillDictionaryNames)
        //throws com.WDataSci.WDS.WDSException
        {

            try {

                int ii = -1;
                int j = -1;
                int k = -1;


                //are we using a JniPMML object (as when called from C# and does it have PMMLMatter
                Boolean bUsingJniPMML = (aJniPMML != null);
                //Java Boolean bCheckingAgainstPMML = (aJniPMML != null && aJniPMML.PMMLMatter.Doc != null);
                //C#
                Boolean bCheckingAgainstPMML = false;

                //Java org.dmg.pmml.DataField[] lDataFields = null;
                String[] lFieldStringNames = null;
                int nDataFieldNames = 0;

                if ( bCheckingAgainstPMML ) {
                    /* Java >>> *
                       lDataFields = aJniPMML.PMMLDataFields();
                       nDataFieldNames = lDataFields.length;
                       lFieldStringNames = new String[nDataFieldNames];
                       for (i = 0; i < nDataFieldNames; i++)
                       lFieldStringNames[i] = lDataFields[i].getName().getValue();
                    /* <<< Java */
                }


                String aPathAndName = null;
                aPathAndName = com.WDataSci.WDS.Util.PathAndName(this.Path, this.FileName);
                /* Java >>> *
                this.__CSVParser = new CSVParser(new FileReader(aPathAndName), CSVFormat.EXCEL);
                this.__CSVParserIterator = this.__CSVParser.iterator();
                CSVRecord inputLine = this.__CSVParserIterator.next();
                /* <<< Java */
                /* C# >>> */
                this.__StreamReader = new StreamReader(aPathAndName);
                this.__CSV = new CsvParser(this.__StreamReader);
                String[] inputLine = this.__CSV.Read();
                /* <<< C# */

                //Java int xnlLength = inputLine.size();
                //C#
                int xnlLength = inputLine.Length;

                aRecordSetMD.Column = new FieldMD[xnlLength];

                //for when the length is packed into the XSD type for limiting strings
                int[] typl = new int[1];
                for ( ii = 0 ; ii < xnlLength ; ii++ ) {

                    aRecordSetMD.Column[ii] = new FieldMD();
                    FieldMD cm = aRecordSetMD.Column[ii];

                    //Java cm.Name = inputLine.get(ii);
                    //C#
                    cm.Name = inputLine[ii];

                    Boolean found = false;

                    if ( bCheckingAgainstPMML ) {
                        //Search for PMML DataFieldName map
                        for ( j = 0 ; !found && j < nDataFieldNames ; j++ ) {
                            if ( cm.Name.equals(lFieldStringNames[j]) ) {
                                found = true;
                                cm.MapToMapKey(lFieldStringNames[j]);
                                break;
                            }
                        }

                        /* Java >>> *
                        // if found in PMML, check for PMML DataType
                            if ( found ) {
                                org.dmg.pmml.DataType ofdtyp = lDataFields[j].getDataType();
                                switch ( ofdtyp ) {
                                    case FLOAT:
                                    case DOUBLE:
                                        cm.DTyp = FieldMDEnums.eDTyp.Dbl;
                                        break;
                                    case INTEGER:
                                        if ( cm.Name.endsWith("ID") )
                                            cm.DTyp = FieldMDEnums.eDTyp.Lng;
                                        else
                                            cm.DTyp = FieldMDEnums.eDTyp.Int;
                                        break;
                                    case STRING:
                                        cm.DTyp = FieldMDEnums.eDTyp.VLS;
                                        break;
                                    case DATE:
                                        cm.DTyp = FieldMDEnums.eDTyp.Dte;
                                        break;
                                    case DATE_TIME:
                                    case TIME:
                                        cm.DTyp = FieldMDEnums.eDTyp.DTm;
                                        break;
                                    case BOOLEAN:
                                        cm.DTyp = FieldMDEnums.eDTyp.Bln;
                                        break;
                                    case DATE_DAYS_SINCE_0:
                                    case DATE_DAYS_SINCE_1960:
                                    case DATE_DAYS_SINCE_1970:
                                    case DATE_DAYS_SINCE_1980:
                                    case TIME_SECONDS:
                                    case DATE_TIME_SECONDS_SINCE_0:
                                    case DATE_TIME_SECONDS_SINCE_1960:
                                    case DATE_TIME_SECONDS_SINCE_1970:
                                    case DATE_TIME_SECONDS_SINCE_1980:
                                    default:
                                        cm.DTyp = FieldMDEnums.eDTyp.Int;
                                        break;
                                }
                            }
                        /* <<< Java */
                    }

                    if ( !found ) {

                        if ( bFillDictionaryNames )
                            cm.MapToMapKey(cm.Name);

                        String s = cm.Name;

                        String cls = null;
                        String mod = null;
                        String rep = null;
                        String postmod = null;

                        //We are assuming a modified three part naming convention
                        //The core three part naming convention is assumed to be CamelCase and of the form
                        //   Class[Modifier]Representation
                        //The Representation defines the data type (we will assume all strings are VLen).
                        //
                        //We allow a _PostModifier (preceded by an underscore).  This implies that all
                        //fields with a root name are related and we are not embedding obfuscation.
                        //
                        //Examples based on consumer loan data are:
                        //   Long form                     Short form
                        //   PrincipalBalance              PrinBal
                        //   PaymentAmount                 PmtAmt
                        //   PaymentPrincipalAmount        PmtPrinAmt
                        //   PaymentPrincipalAmount_Lag1   PmtPrinAmt_Lag1
                        //
                        //See documentation on WDataSci naming conventions.

                        k = s.lastIndexOf("_");
                        if ( k > 0 ) {
                            postmod = s.substring(k + 1);
                            s = s.substring(0, k - 1);
                        }
                        String sl = s.toLowerCase();

                        //recognize Date and DateTime first
                        if ( s.endsWith("Date") )
                            cm.DTyp = FieldMDEnums.eDTyp.Dte;
                        else if ( s.endsWith("Time") )
                            cm.DTyp = FieldMDEnums.eDTyp.DTm;
                        //longs
                        else if ( cm.Name.endsWith("ID") )
                            cm.DTyp = FieldMDEnums.eDTyp.Lng;
                        else if ( cm.Name.endsWith("Nbr") && (sl.startsWith("acc") || sl.startsWith("loan")) )
                            cm.DTyp = FieldMDEnums.eDTyp.Lng;
                        else if ( cm.Name.endsWith("Nbr") )
                            cm.DTyp = FieldMDEnums.eDTyp.Int;
                        else if ( cm.Name.endsWith("Count") )
                            cm.DTyp = FieldMDEnums.eDTyp.Int;
                        else if ( cm.Name.endsWith("Status") )
                            cm.DTyp = FieldMDEnums.eDTyp.VLS;
                        else if ( cm.Name.length() > 3 &&
                                bIn(cm.Name.substring(cm.Name.length() - 3)
                                    , "Bal", "Amt", "Pct") )
                            cm.DTyp = FieldMDEnums.eDTyp.Dbl;
                        else if ( cm.Name.length() > 3 &&
                                bIn(cm.Name.substring(cm.Name.length() - 3)
                                    , "Mos") )
                            cm.DTyp = FieldMDEnums.eDTyp.Int;
                        else if ( cm.Name.length() > 4 &&
                                bIn(cm.Name.substring(cm.Name.length() - 4)
                                    , "Rate") )
                            cm.DTyp = FieldMDEnums.eDTyp.Dbl;
                        else if ( cm.Name.length() > 4 &&
                                bIn(cm.Name.substring(cm.Name.length() - 4)
                                    , "Name", "Code", "Stat", "Flag") )
                            cm.DTyp = FieldMDEnums.eDTyp.VLS;
                        else
                            cm.DTyp = FieldMDEnums.eDTyp.VLS;
                    }

                    if ( cm.DTyp.bIn(FieldMDEnums.eDTyp.VLS, FieldMDEnums.eDTyp.Str) )
                        cm.StringMaxLength = FieldMD.Default.StringMaxLength;
                }

            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error mapping input columns:", e);
            }
        }


        public void mReadRecordSet(RecordSetMD aInputRecordSetMD, RecordSet aInputRecordSet, PrintWriter pw)
        //throws com.WDataSci.WDS.WDSException
        {
            this.mReadRecordSet(aInputRecordSetMD
                    , aInputRecordSet
                    , aInputRecordSetMD.FileMatter.Path
                    , aInputRecordSetMD.FileMatter.FileName
                    , aInputRecordSetMD.FileMatter.hasHeaderRow
                    , aInputRecordSetMD.FileMatter.Dlm
                    , pw
                    );

        }


        public void mReadRecordSet(RecordSetMD aInputRecordSetMD
                , RecordSet aInputRecordSet
                , String aPath
                , String aFileName
                , Boolean hasHeaderRow
                , String dlm
                , PrintWriter pw
                )
        //throws com.WDataSci.WDS.WDSException
        {
            try {

                if ( aInputRecordSetMD.FileMatter == null )
                    aInputRecordSetMD.FileMatter.cPointToFile(aInputRecordSetMD, aPath, aFileName, hasHeaderRow, dlm);

                int nInputMap = aInputRecordSetMD.nColumns();
                int i = -1;
                int j = -1;

                String aPathAndName = null;

                //Java if ( this.__CSV == null || this.__CSV.isClosed() || !this.Path.equals(aPath) || !this.FileName.equals(aFileName) ) 
                //C#
                if ( this.__CSV == null || !this.Path.equals(aPath) || !this.FileName.equals(aFileName) ) 
                {
                    aPathAndName = com.WDataSci.WDS.Util.PathAndName(aPath, aFileName);
                    //Java this.__CSV = new CSVParser(new FileReader(aPathAndName), CSVFormat.EXCEL);
                    //Java this.__CSVParserIterator = this.__CSVParser.iterator();
                    //C#
                    this.__CSV = new CsvParser(new StreamReader(aPathAndName));
                }

                if ( aInputRecordSet.isEmpty() ) {
                    //Java aInputRecordSet.Records = new ArrayList<>(0);
                    //C#
                    aInputRecordSet.Records = new List<Map<FieldName, Object>>(0);
                    //Java aInputRecordSet.Records_Orig = new ArrayList<>(0);
                    //C#
                    aInputRecordSet.Records_Orig = new List<Object[]>(0);
                }

                //Java CSVRecord inputLine;
                //C#
                String[] inputLine;

                //Java if ( hasHeaderRow && this.__CSVParser.getCurrentLineNumber() == 0 ) 
                //C#
                if ( hasHeaderRow && this.__CSV.Context.Row < 1 ) {
                    //Java inputLine = this.__CSVParserIterator.next();
                    //C#
                    inputLine = this.__CSV.Read();
                }
                int row = -1;


                //Java while (this.__CSVParserIterator.hasNext()) 
                //C#
                while ( true ) {
                    //Java inputLine = this.__CSVParserIterator.next();
                    //C#
                    inputLine = this.__CSV.Read();

                    if ( inputLine == null ) break;

                    int nIncomingFields = inputLine.size();
                    if ( nIncomingFields == 0 || (nIncomingFields == 1 && nInputMap > 1) ) break;

                    //if nIncomingFields<nInputMap, pad with nulls, otherwise throw
                    if ( nIncomingFields > nInputMap )
                        //Java throw new com.WDataSci.WDS.WDSException(String.format("Error reading from delimited file, row %d has insufficient columns, stopping input!\n", row + 1));
                        //C#
                        throw new com.WDataSci.WDS.WDSException("Error reading from delimited file, row " + (row + 1) + " has insufficient columns, stopping input!\n");

                    row++;
                    //Java Map<FieldName, Object> inputRow = new LinkedHashMap<>();
                    //C#
                    Map<FieldName, Object> inputRow = new Map<FieldName, Object>();
                    Object[] inputRow_orig = new Object[nInputMap];
                    for ( j = 0 ; j < nInputMap ; j++ ) {
                        String s = null;
                        if (j<nIncomingFields) s= inputLine.get(j);
                        if ( s == null || s.isEmpty() ) {
                            inputRow_orig[j] = null;
                            if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                inputRow.put(aInputRecordSetMD.Column[j].MapKey, null);
                        }
                        else if ( aInputRecordSetMD.Column[j].DTyp.equals(FieldMDEnums.eDTyp.Dbl) ) {
                            //Java Double lv = Double.parseDouble(inputLine.get(j));
                            /* C# >>> */
                            double tmplv = double.NaN;
                            double? lv = null;
                            if ( double.TryParse(inputLine.get(j), out tmplv) ) lv = tmplv;
                            /* <<< C# */
                            inputRow_orig[j] = lv;
                            if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                        }
                        else if ( aInputRecordSetMD.Column[j].DTyp.equals(FieldMDEnums.eDTyp.Lng) ) {
                            //Java Long lv = Long.parseLong(inputLine.get(j));
                            /* C# >>> */
                            long tmplv = long.MinValue;
                            long? lv = null;
                            if ( long.TryParse(inputLine.get(j), out tmplv) ) lv = tmplv;
                            /* <<< C# */
                            inputRow_orig[j] = lv;
                            if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                        }
                        else if ( aInputRecordSetMD.Column[j].DTyp.equals(FieldMDEnums.eDTyp.Int) ) {
                            //Java Integer lv = Integer.parseInt(inputLine.get(j));
                            /* C# >>> */
                            int tmplv = int.MinValue;
                            int? lv = null;
                            if ( int.TryParse(inputLine.get(j), out tmplv) ) lv = tmplv;
                            /* <<< C# */
                            inputRow_orig[j] = lv;
                            if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                        }
                        else {
                            //Java String lv = com.WDataSci.JniPMML.Util.CleanAsToken(inputLine.get(j));
                            //C#
                            String lv = com.WDataSci.WDS.Util.CleanAsToken(inputLine.get(j));
                            inputRow_orig[j] = lv;
                            if ( aInputRecordSetMD.Column[j].hasMapKey() )
                                inputRow.put(aInputRecordSetMD.Column[j].MapKey, lv);
                        }
                    }
                    aInputRecordSet.Records.add(inputRow);
                    aInputRecordSet.Records_Orig.add(inputRow_orig);
                }
                //Java this.__CSV.close();
                //C#
                this.__CSV.Dispose();
                //C#? this.__StreamReader.Dispose();
            }
            catch ( com.WDataSci.WDS.WDSException e ) {
                throw new com.WDataSci.WDS.WDSException("Error reading from delimited file:", e);
            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error reading from delimited file:", e);
            }

        }

        public void mWriteRecordSet(RecordSetMD aOutputRecordSetMD, RecordSet aOutputRecordSet, RecordSetMD aInputRecordSetMD, RecordSet aInputRecordSet)
        //throws com.WDataSci.WDS.WDSException
        {

            int i = -1;
            int j = -1;
            int k = -1;
            int jj = -1;

            int nInputMap = aOutputRecordSetMD.ModeMatter.nInputFields;

            int nColumns = aOutputRecordSetMD.Column.Length;
            int nResultColumns = nColumns;
            if ( aOutputRecordSetMD.ModeMatter.bRepeatInputFields )
                nResultColumns -= nInputMap;


            int nRows = aOutputRecordSet.Records.size();


            if ( aOutputRecordSetMD.Type.bIn(RecordSetMDEnums.eType.CSV, RecordSetMDEnums.eType.TXT, RecordSetMDEnums.eType.Dlm) ) {
                try {

                    String aPathAndName = null;
                    aPathAndName = com.WDataSci.WDS.Util.PathAndName(aOutputRecordSetMD.FileMatter.Path, aOutputRecordSetMD.FileMatter.FileName);
                    //Java BufferedWriter outBW = Files.newBufferedWriter(Paths.get(aPathAndName));
                    //C# 
                    StreamWriter outBW = new StreamWriter(aPathAndName);

                    //Java CSVPrinter outCSV = null;
                    //C# 
                    CsvWriter outCSV = null;

                    if ( aOutputRecordSetMD.FileMatter.hasHeaderRow ) {
                        int nTotalColumns=nColumns;
                        if ( aOutputRecordSetMD.ModeMatter.bRepeatInputFields )
                            nTotalColumns+=aInputRecordSetMD.nColumns();
                        String[] hr = new String[nTotalColumns];
                        jj = 0;
                        if ( aOutputRecordSetMD.ModeMatter.bRepeatInputFields )
                            for ( k = 0; k < nColumns; k++, jj++ ) hr[jj] = aInputRecordSetMD.Column[k].Name;
                        for ( k = 0; k < nColumns; k++, jj++ ) hr[jj] = aOutputRecordSetMD.Column[k].Name;
                        /* Java >>> * 
                           outCSV = new CSVPrinter(outBW, CSVFormat.DEFAULT.withHeader(hr).withDelimiter(aOutputRecordSetMD.FileMatter.Dlm.charAt(0)));
                        /* <<< Java */
                        /* C# >>> */
                        outCSV = new CsvWriter(outBW);
                        outCSV.printRecord(hr);
                        /* <<< C# */
                    }
                    else {
                        //Java outCSV = new CSVPrinter(outBW, CSVFormat.DEFAULT.withDelimiter(aOutputRecordSetMD.FileMatter.Dlm.charAt(0)));
                        //C#
                        outCSV = new CsvWriter(outBW);
                    }
                    outCSV.flush();

                    for ( i = 0 ; i < aInputRecordSet.Records.size() ; i++ ) {
                        //Java List<String> outputrow = new ArrayList<>(0);
                        //C#
                        List<String> outputrow = new List<String>(0);
                        jj = 0;
                        if ( aOutputRecordSetMD.ModeMatter.bRepeatInputFields ) {
                            //Java for (Object obj : aInputRecordSet.Records_Orig.get(i)) 
                            //C#
                            foreach ( Object obj in aInputRecordSet.Records_Orig.get(i) ) 
                            {
                                if ( obj == null )
                                    outputrow.add(null);
                                else
                                    outputrow.add(obj.toString());
                            }
                            jj = nInputMap;
                        }
                        for ( k = 0, j = jj ; k < nResultColumns ; k++, j++ ) {
                            Object obj = aOutputRecordSet.Records.get(i).get(aOutputRecordSetMD.Column[j].MapKey);
                            if ( obj == null )
                                outputrow.add(null);
                            else
                                outputrow.add(aOutputRecordSet.Records.get(i).get(aOutputRecordSetMD.Column[j].MapKey).toString());
                        }
                        outCSV.printRecord(outputrow);
                    }
                    outCSV.flush();

                    outCSV.close();
                    outBW.close();

                }
                catch ( Exception e ) {
                    throw new com.WDataSci.WDS.WDSException("Error in WranglerFlatFile.mWriteRecordSet", e);
                }

            }
        }

        public void mWriteRecordSet(RecordSetMD aOutputRecordSetMD, RecordSet aOutputRecordSet)
        //throws com.WDataSci.WDS.WDSException
        {

            int i = -1;
            int j = -1;
            int k = -1;
            int jj = -1;

            int nInputMap = aOutputRecordSetMD.ModeMatter.nInputFields;

            int nColumns = aOutputRecordSetMD.Column.Length;
            int nResultColumns = nColumns;

            int nRows = aOutputRecordSet.Records.size();

            if ( aOutputRecordSetMD.Type.bIn(RecordSetMDEnums.eType.CSV, RecordSetMDEnums.eType.TXT, RecordSetMDEnums.eType.Dlm) ) {
                try {

                    String aPathAndName = null;
                    aPathAndName = com.WDataSci.WDS.Util.PathAndName(aOutputRecordSetMD.FileMatter.Path, aOutputRecordSetMD.FileMatter.FileName);
                    //Java BufferedWriter outBW = Files.newBufferedWriter(Paths.get(aPathAndName));
                    //C#
                    StreamWriter outBW = new StreamWriter(aPathAndName);

                    //Java CSVPrinter outCSV = null;
                    //C# 
                    CsvWriter outCSV = null;

                    if ( aOutputRecordSetMD.FileMatter.hasHeaderRow ) {
                        String[] hr = new String[nColumns];
                        for ( k = 0 ; k < nColumns ; k++ ) hr[k] = aOutputRecordSetMD.Column[k].Name;
                        /* Java >>> * 
                           outCSV = new CSVPrinter(outBW, CSVFormat.DEFAULT.withHeader(hr).withDelimiter(aOutputRecordSetMD.FileMatter.Dlm.charAt(0)));
                        /* <<< Java */
                        /* C# >>> */
                        outCSV = new CsvWriter(outBW);
                        outCSV.printRecord(hr);
                        /* <<< C# */
                    }
                    else {
                        //Java outCSV = new CSVPrinter(outBW, CSVFormat.DEFAULT.withDelimiter(aOutputRecordSetMD.FileMatter.Dlm.charAt(0)));
                        //C#
                        outCSV = new CsvWriter(outBW);
                    }
                    outCSV.flush();

                    for ( i = 0 ; i < aOutputRecordSet.Records.size() ; i++ ) {
                        //Java List<String> outputrow = new ArrayList<>(0);
                        //C#
                        List<String> outputrow = new List<String>(0);
                        jj = 0;
                        for ( k = 0, j = jj ; k < nResultColumns ; k++, j++ ) {
                            Object obj = aOutputRecordSet.Records.get(i).get(aOutputRecordSetMD.Column[j].MapKey);
                            if ( obj == null )
                                outputrow.add(null);
                            else
                                outputrow.add(aOutputRecordSet.Records.get(i).get(aOutputRecordSetMD.Column[j].MapKey).toString());
                        }
                        outCSV.printRecord(outputrow);
                    }
                    outCSV.flush();

                    outCSV.close();
                    outBW.close();

                }
                catch ( Exception e ) {
                    throw new com.WDataSci.WDS.WDSException("Error in WranglerFlatFile.mWriteRecordSet", e);
                }

            }
        }

        /* C# >>> */
        public void mWriteRecordSet(RecordSetMD aOutputRecordSetMD, MOIE.ListObject aListObject)
        {

            int i = -1;
            int iP1 = 0;
            int j = -1;
            int jP1 = 0;
            int k = -1;
            int jj = -1;

            int nInputMap = aOutputRecordSetMD.ModeMatter.nInputFields;

            int nColumns = aOutputRecordSetMD.Column.Length;
            int nResultColumns = nColumns;

            int nRows = aListObject.ListRows.Count;

            if ( aOutputRecordSetMD.Type.bIn(RecordSetMDEnums.eType.CSV, RecordSetMDEnums.eType.TXT, RecordSetMDEnums.eType.Dlm) ) {
                try {

                    String aPathAndName = null;
                    aPathAndName = com.WDataSci.WDS.Util.PathAndName(aOutputRecordSetMD.FileMatter.Path, aOutputRecordSetMD.FileMatter.FileName);
                    StreamWriter outBW = new StreamWriter(aPathAndName);

                    CsvWriter outCSV = null;

                    if ( aOutputRecordSetMD.FileMatter.hasHeaderRow ) {
                        String[] hr = new String[nColumns];
                        for ( k = 0 ; k < nColumns ; k++ ) hr[k] = aOutputRecordSetMD.Column[k].Name;
                        outCSV = new CsvWriter(outBW);
                        outCSV.printRecord(hr);
                    }
                    else {
                        outCSV = new CsvWriter(outBW);
                    }
                    outCSV.flush();

                    Object[,] value2 = aListObject.DataBodyRange.Value2;

                    for ( i = 0, iP1=1 ; i < nRows ; i++, iP1++ ) {
                        List<String> outputrow = new List<String>(0);
                        for ( j = 0, jP1=1 ; j < nResultColumns ; j++, jP1++ ) {
                            Object obj = value2[iP1,jP1];
                            if ( obj == null )
                                outputrow.add(null);
                            else
                                outputrow.add(obj.toString());
                        }
                        outCSV.printRecord(outputrow);
                    }
                    outCSV.flush();

                    outCSV.close();
                    outBW.close();

                }
                catch ( Exception e ) {
                    throw new com.WDataSci.WDS.WDSException("Error in WranglerFlatFile.mWriteRecordSet", e);
                }
            }

        }
        /* <<< C# */

    }
    /* C# >>> */
}
/* <<< C# */
