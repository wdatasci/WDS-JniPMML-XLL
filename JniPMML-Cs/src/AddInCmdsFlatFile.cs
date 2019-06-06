using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;

using ExcelDna.Integration;

using MOIE=Microsoft.Office.Interop.Excel;

using com.WDataSci.JniPMML;

namespace WDataSci.FlatFile
{

    public class Cmds
    {

        [ExcelCommand(Description = "Export XMLMapped List Object to CSV", ExplicitRegistration = true)]
        public static void ExportXmlMappedListToCSV()
        {

            String sFileName = "test.h5";
            ExcelReference selection;
            MOIE.Application ma;
            MOIE.Range mr;
            MOIE.XmlMap aXmlMap;
            MOIE.ListObject aListObject;
            RecordSetMD aRecordSetMD;

            //using isContinuing instead of throwing on last steps
            Boolean isContinuing=true;

            try {
                selection = (ExcelReference) XlCall.Excel(XlCall.xlfSelection);
                ma = (ExcelDnaUtil.Application as MOIE.Application);
                mr = ma.Evaluate(XlCall.Excel(XlCall.xlfReftext, selection, true)) as MOIE.Range;

                try {
                    aListObject = mr.ListObject;
                    aXmlMap = aListObject.XmlMap;
                }
                catch ( Exception ) {
                    throw new com.WDataSci.WDS.WDSException("Error: could not pull XmlMap from selection");
                }

                aRecordSetMD=new RecordSetMD(RecordSetMDEnums.eMode.Internal)
                .cAs(RecordSetMDEnums.eType.CSV,RecordSetMDEnums.eSchemaType.XSD, false, aXmlMap.Schemas.Item[1].XML)
                .mReadMapFor(null, null, true)
                ;

                int nColumns=aRecordSetMD.nColumns();
                if ( aListObject.ListColumns.Count != nColumns ) {
                    throw new com.WDataSci.WDS.WDSException("ListObject Column Count Does Not Match Schema Node List Count!");
                }

                MessageBoxButtons msgboxbuttons = MessageBoxButtons.YesNoCancel;
                DialogResult msgboxresponse = MessageBox.Show("Write CSV file from XmlMap'd ListObject of selection?", "Confirm", msgboxbuttons);

                isContinuing = (isContinuing && msgboxresponse == System.Windows.Forms.DialogResult.Yes);

                if ( isContinuing )
                    using ( SaveFileDialog aSaveFileDialog = new SaveFileDialog() ) {
                        aSaveFileDialog.InitialDirectory = ma.ActiveWorkbook.Path;
                        aSaveFileDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
                        aSaveFileDialog.FilterIndex = 1;
                        aSaveFileDialog.RestoreDirectory = true;
                        aSaveFileDialog.FileName = sFileName;
                        aSaveFileDialog.AddExtension = true;
                        aSaveFileDialog.DefaultExt = ".csv";
                        //aSaveFileDialog.CheckFileExists = true;
                        aSaveFileDialog.CheckPathExists = true;
                        aSaveFileDialog.Title = "Export XmlMap'd ListObject to CSV (*.csv) File....";

                        if ( aSaveFileDialog.ShowDialog() == DialogResult.OK ) {
                            sFileName = aSaveFileDialog.FileName;
                            if ( !sFileName.ToLower().EndsWith(".csv") )
                                sFileName += ".csv";
                        }
                        else
                            isContinuing = false;

                    }

                if ( isContinuing ) {
                    aRecordSetMD
                        .cToFile(sFileName)
                        .cWithHeaderRow()
                        .FileMatter.mWriteRecordSet(aRecordSetMD, aListObject);
                }

            }
            catch ( com.WDataSci.WDS.WDSException e ) {
                MessageBox.Show(e.getMessage());
            }
            catch ( Exception e ) {
                com.WDataSci.WDS.WDSException we=new com.WDataSci.WDS.WDSException("Error in ExportXMLMappedListToCSV to "+sFileName,e);
                MessageBox.Show(we.getMessage());
            }
            finally {
                selection = null;
                aListObject = null;
                aXmlMap = null;
                mr = null;
                ma = null;
                aRecordSetMD = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return;
        }

        public static String XSDUserInput()
        {

            //Typing for possible GC purposes
            MOIE.Application tapp = null;
            MOIE.Range trng = null;
            MOIE.Range trng2 = null;
            MOIE.XmlMap aXmlMap = null;
            MOIE.ListObject aListObject = null;
            MOIE.Workbook twb = null;
            MOIE.Worksheet tws = null;
            JniPMMLItem aJniPMMLItem=null;
            XmlDocument aXmlDocument=null;
            XmlNodeList aXmlNodeList=null;

            String rv = "";


            int h=-1;
            Boolean bIsModelCached=true;
            tapp = (ExcelDnaUtil.Application as MOIE.Application);
            Boolean screenupdating_prior=tapp.ScreenUpdating;
            MOIE.XlCalculation calculation_prior=tapp.Calculation;

            try {


                int i, j, iP1, jP1, ii, iiP1;

                twb = tapp.ActiveWorkbook;
                tws = twb.ActiveSheet;

                String sFile="?";
                MessageBoxButtons msgboxbuttons = MessageBoxButtons.YesNoCancel;
                DialogResult msgboxresponse;
                bIsModelCached = false;
                msgboxresponse = MessageBox.Show("Would you like to point to an XSD file (Yes/no)?", "Confirm", msgboxbuttons);
                if ( msgboxresponse == System.Windows.Forms.DialogResult.Cancel )
                    throw new com.WDataSci.WDS.WDSException("Cancel");
                if ( msgboxresponse == System.Windows.Forms.DialogResult.Yes )
                    using ( OpenFileDialog aOpenFileDialog = new OpenFileDialog() ) {
                        aOpenFileDialog.InitialDirectory = tapp.ActiveWorkbook.Path;
                        aOpenFileDialog.Filter = "XSD File (*.xsd)|*.xsd|All Files (*.*)|*.*";
                        aOpenFileDialog.FilterIndex = 1;
                        aOpenFileDialog.RestoreDirectory = true;
                        aOpenFileDialog.AddExtension = true;
                        aOpenFileDialog.DefaultExt = ".xsd";
                        aOpenFileDialog.CheckFileExists = true;
                        aOpenFileDialog.CheckPathExists = true;
                        aOpenFileDialog.Title = "XML Schema (XSD) File....";
                        if ( aOpenFileDialog.ShowDialog() == DialogResult.OK )
                            sFile = aOpenFileDialog.FileName;
                        else
                            throw new com.WDataSci.WDS.WDSException("Cancel");
                        rv=com.WDataSci.WDS.Util.FetchFileAsString(sFile);
                    }
                else {
                    msgboxresponse = MessageBox.Show("Point to an XSD string in a cells (Yes) or leave unspecified (No)?", "Confirm", msgboxbuttons);
                    if ( msgboxresponse == System.Windows.Forms.DialogResult.Cancel )
                        throw new com.WDataSci.WDS.WDSException("Cancel");
                    if ( msgboxresponse == System.Windows.Forms.DialogResult.Yes ) {
                        try {
                            MOIE.Range trng3 = tapp.InputBox("Use an XSD as one string contained in a cell, enter cell address (navigable)", "XSD Input", "Entire XSD File as a String", 100, 100, "", 0, 8) as MOIE.Range;
                            sFile = trng3.Text;
                            trng3 = null;
                            if ( !sFile.StartsWith("<?xml") ) {
                                if ( sFile.IndexOf("!") < 0 )
                                    sFile = "'[" + tapp.ActiveWorkbook.Name + "]" + aListObject.DataBodyRange.Worksheet.Name + "'!" + sFile;
                                ExcelReference rf = XlCall.Excel(XlCall.xlfEvaluate, sFile) as ExcelReference;
                                trng3 = tapp.Evaluate(XlCall.Excel(XlCall.xlfReftext, rf, true)) as MOIE.Range;
                                sFile = trng3.Text;
                                rf = null;
                                trng3 = null;
                                rv = sFile;
                            } else {
                                throw new com.WDataSci.WDS.WDSException("Error, value not a valid XSD string");
                            }
                        }
                        catch {
                            throw new com.WDataSci.WDS.WDSException("Cancel");
                        }
                    }
                }

            }
            catch ( com.WDataSci.WDS.WDSException e ) {
                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                if ( !e.getMessage().Equals("Cancel") ) {
                    MessageBox.Show(e.getMessage() + "\n" + e.StackTrace.ToString());
                }
            }
            catch ( Exception e ) {
                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                MessageBox.Show("Error!\n" + e.Message + "\n" + e.StackTrace.ToString());
            }
            finally {

                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                if ( tapp.Calculation != calculation_prior ) tapp.Calculation = calculation_prior;

                aListObject = null;
                aXmlMap = null;
                tapp = null;
                trng = null;
                trng2 = null;
                twb = null;
                tws = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();

            }
            return rv;
        }


        [ExcelCommand(Description = "Import CSV to XMLMapped List", ExplicitRegistration =true)]
        public static void ImportCSVToXMLMappedList()
        {
            String sFileName = "test.csv";
            ExcelReference selection;
            MOIE.Application tapp;
            MOIE.Range trng;
            MOIE.Range trng2;
            MOIE.Workbook twb;
            MOIE.Sheets twbSheets;
            MOIE.Worksheet tws;
            MOIE.Range tblr;
            MOIE.ListObject tbl;
            MOIE.XmlMap aXmlMap;
            tapp = (ExcelDnaUtil.Application as MOIE.Application);
            Boolean screenupdating_prior=tapp.ScreenUpdating;
            MOIE.XlCalculation calculation_prior=tapp.Calculation;
            RecordSetMD aRecordSetMD;
            RecordSet aRecordSet;

            //using isContinuing instead of throwing on last steps
            Boolean isContinuing=true;

            try {

                //tapp.ScreenUpdating = false;
                tapp.Calculation = MOIE.XlCalculation.xlCalculationManual;

                int i, iP1;

                using ( OpenFileDialog aOpenFileDialog = new OpenFileDialog() ) {

                    aOpenFileDialog.InitialDirectory = tapp.ActiveWorkbook.Path;
                    aOpenFileDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
                    aOpenFileDialog.FilterIndex = 1;
                    aOpenFileDialog.RestoreDirectory = true;
                    aOpenFileDialog.CheckPathExists = true;
                    aOpenFileDialog.CheckFileExists = true;
                    aOpenFileDialog.FileName = sFileName;
                    aOpenFileDialog.AddExtension = true;
                    aOpenFileDialog.DefaultExt = ".csv";
                    aOpenFileDialog.Title = "Import compound data from CSV (*.csv) File....";

                    if ( aOpenFileDialog.ShowDialog() == DialogResult.OK )
                        sFileName = aOpenFileDialog.FileName;
                    else
                        isContinuing = false;
                }

                if ( !isContinuing )
                    throw new com.WDataSci.WDS.WDSException("Cancel");

                String aXSDString = XSDUserInput();
                if ( aXSDString.Equals("Cancel") || aXSDString.StartsWith("Err") )
                    throw new com.WDataSci.WDS.WDSException(aXSDString);
                Boolean isXSDProvided = aXSDString.StartsWith("<");


                RecordSetMDEnums.eSchemaType aSchemaType = RecordSetMDEnums.eSchemaType.XSD;
                if ( !isXSDProvided ) aSchemaType = RecordSetMDEnums.eSchemaType.NamingConvention;

                aRecordSetMD = new RecordSetMD(RecordSetMDEnums.eMode.Input)
                .cAs(RecordSetMDEnums.eType.CSV, aSchemaType, false, aXSDString)
                .cAsDlmFile(sFileName)
                ;

                MessageBoxButtons msgboxbuttons = MessageBoxButtons.YesNoCancel;
                DialogResult msgboxresponse;

                msgboxresponse = MessageBox.Show("Does file have a header row (Yes) or (No)?", "Confirm", msgboxbuttons);
                if ( msgboxresponse == System.Windows.Forms.DialogResult.Cancel )
                    throw new com.WDataSci.WDS.WDSException("Cancel");
                if ( msgboxresponse == System.Windows.Forms.DialogResult.Yes )
                    aRecordSetMD.cWithHeaderRow();

                aRecordSetMD.mReadMapFor(null, null, true);

                aRecordSet = new RecordSet()
                .cAsInput()
                .mReadRecordSet(aRecordSetMD)
                ;

                msgboxresponse = MessageBox.Show("Write to a new sheet (Yes) or point to cell for the upper left corder (No)?", "Confirm", msgboxbuttons);
                if ( msgboxresponse == System.Windows.Forms.DialogResult.Cancel )
                    throw new com.WDataSci.WDS.WDSException("Cancel");
                if ( msgboxresponse == System.Windows.Forms.DialogResult.No ) {
                    try {
                        selection = (ExcelReference) XlCall.Excel(XlCall.xlfSelection);
                        trng = tapp.Evaluate(XlCall.Excel(XlCall.xlfReftext, selection, true)) as MOIE.Range;
                        trng2 = tapp.InputBox("Enter cell address (navigable)", "Output Location", trng.Address.ToString(), 100, 100, "", 0, 8) as MOIE.Range;
                        trng = null;
                        tws = trng2.Parent;
                        twb = tws.Parent;
                    }
                    catch {
                        throw new com.WDataSci.WDS.WDSException("Cancel");
                    }
                }
                else {
                    twb = tapp.ActiveWorkbook;
                    twbSheets = twb.Sheets;
                    tws = twbSheets.Add();
                    twbSheets = null;
                    trng2 = tws.Cells[1, 1];
                    try {
                        tws.Name = sFileName;
                    }
                    catch ( Exception e ) {
                        String s = tapp.InputBox("Cannot name sheet to " + sFileName, "New Sheet Name", "Leave-As-Is", 100, 100, "");
                        if ( !s.Equals("Leave-As-Is") ) {
                            try {
                                tws.Name = s;
                            }
                            catch ( Exception ) {

                            }
                        }
                    }
                }

                //tapp.ScreenUpdating = false;

                int nRows = aRecordSet.Records.Count;
                int nColumns = aRecordSetMD.nColumns();

                for ( uint jj = 0 ; jj < nColumns ; jj++ ) {
                    trng2.Offset[0,jj].Value2 = aRecordSetMD.Column[jj].Name;
                    for ( i = 0, iP1=1 ; i < nRows ; i++, iP1++ )
                        trng2.Offset[iP1, jj].Value2 = aRecordSet.Records_Orig[i][jj];
                }

                tblr = tws.Range[trng2, trng2.Offset[nRows, nColumns - 1]];
                tbl = (MOIE.ListObject) tws.ListObjects.AddEx(MOIE.XlListObjectSourceType.xlSrcRange, tblr, null, MOIE.XlYesNoGuess.xlYes);

                if ( aRecordSetMD.SchemaType.bIn(RecordSetMDEnums.eSchemaType.XSD) ) {

                    aXmlMap = twb.XmlMaps.Add(aRecordSetMD.SchemaMatter.InputSchemaString);

                    for ( int j = 0, jP1 = 1 ; j < nColumns ; j++, jP1++ ) {
                        tbl.ListColumns[jP1].XPath.SetValue(aXmlMap
                            , "/" + aRecordSetMD.SchemaMatter.RecordSetElementName
                            + "/" + aRecordSetMD.SchemaMatter.RecordElementName
                            + "/" + aRecordSetMD.Column[j].Name);
                    }

                }

                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;

            }
            catch ( com.WDataSci.WDS.WDSException e ) {
                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                MessageBox.Show(e.getMessage());
            }
            catch ( Exception e ) {
                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                MessageBox.Show(e.Message);
            }
            finally {
                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                if ( tapp.Calculation != calculation_prior ) tapp.Calculation = calculation_prior;
                //Queuing up for GC
                aXmlMap = null;
                aRecordSet = null;
                aRecordSetMD = null;
                tapp = null;
                twb = null;
                twbSheets = null;
                tws = null;
                tblr = null;
                tbl = null;
                trng = null;
                trng2 = null;
                selection = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return;
        }

    }

}
