using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;

using ExcelDna.Integration;

using MOIE=Microsoft.Office.Interop.Excel;

using com.WDataSci.JniPMML;

namespace WDataSci.HDF5
{

    public class Cmds
    {

        [ExcelCommand(Description = "Export XmlMapped List Object to HDF5", ExplicitRegistration = true)]
        public static void ExportXmlMappedListToHDF5()
        {

            String sFileName = "test.h5";
            ExcelReference selection=null;
            MOIE.Application ma=null;
            MOIE.Range mr=null;
            MOIE.XmlMap aXmlMap=null;
            MOIE.ListObject aListObject=null;
            RecordSetMD aRecordSetMD=null;

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
                                            .cAs(RecordSetMDEnums.eType.HDF5,RecordSetMDEnums.eSchemaType.XSD)
                                            ;

                aRecordSetMD.SchemaMatter.InputSchema = new XmlDocument();
                aRecordSetMD.SchemaMatter.InputSchema.LoadXml(aXmlMap.Schemas.Item[1].XML);

                MessageBoxButtons msgboxbuttons = MessageBoxButtons.YesNoCancel;
                DialogResult msgboxresponse = MessageBox.Show("Write HDF5 file from XmlMap'd ListObject of selection?", "Confirm", msgboxbuttons);

                isContinuing = (isContinuing && msgboxresponse == System.Windows.Forms.DialogResult.Yes);

                if ( isContinuing )
                    using ( SaveFileDialog aSaveFileDialog = new SaveFileDialog() ) {
                        aSaveFileDialog.InitialDirectory = ma.ActiveWorkbook.Path;
                        aSaveFileDialog.Filter = "HDF5 Files (*.h5)|*.h5|All Files (*.*)|*.*";
                        aSaveFileDialog.FilterIndex = 1;
                        aSaveFileDialog.RestoreDirectory = true;
                        aSaveFileDialog.FileName = sFileName;
                        aSaveFileDialog.AddExtension = true;
                        aSaveFileDialog.CheckFileExists = true;
                        aSaveFileDialog.CheckPathExists = true;
                        aSaveFileDialog.Title = "Export XmlMap'd ListObject to HDF5 (*.h5) File....";

                        if ( aSaveFileDialog.ShowDialog() == DialogResult.OK ) {
                            sFileName = aSaveFileDialog.FileName;
                            if ( !sFileName.ToLower().EndsWith(".h5") )
                                sFileName += ".h5";
                        }
                        else
                            isContinuing = false;

                    }

                if ( isContinuing ) {
                    aRecordSetMD
                        .cToFile(sFileName)
                        .cWithDataSetName("RecordSet")
                        .mReadMapFor(null, null, true);

                    int nColumns = aRecordSetMD.nColumns();
                    if ( aListObject.ListColumns.Count != nColumns ) {
                        throw new com.WDataSci.WDS.WDSException("ListObject Column Count Does Not Match Schema Node List Count!");
                    }

                    aRecordSetMD.HDF5Matter.mWriteRecordSet(aRecordSetMD, aListObject);
                }

            }
            catch ( com.WDataSci.WDS.WDSException e ) {
                MessageBox.Show(e.getMessage());
            }
            catch ( Exception e ) {
                com.WDataSci.WDS.WDSException we=new com.WDataSci.WDS.WDSException("Error in ExportXmlMappedListToHDF5 to "+sFileName,e);
                MessageBox.Show(we.getMessage());
            }
            finally {
                
                selection = null;
                aListObject = null;
                aXmlMap = null;
                mr = null;
                ma = null;
                if (aRecordSetMD!=null) aRecordSetMD.Dispose();
                aRecordSetMD = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return;
        }


        [ExcelCommand(Description = "Import Compound DataSet from HDf5", ExplicitRegistration = true)]
        public static void ImportHDF5CompoundDS()
        {

            String sFileName = "test.h5";
            ExcelReference selection=null;
            MOIE.Application tapp=null;
            MOIE.Workbook twb=null;
            MOIE.Sheets twbSheets=null;
            MOIE.Worksheet tws=null;
            MOIE.Range tblr=null;
            MOIE.Range trng=null;
            MOIE.Range trng2=null;
            MOIE.ListObject tbl=null;
            tapp = (ExcelDnaUtil.Application as MOIE.Application);
            Boolean screenupdating_prior = tapp.ScreenUpdating;
            MOIE.XlCalculation calculation_prior = tapp.Calculation;
            RecordSetMD aRecordSetMD=null;
            RecordSet aRecordSet=null;

            //using isContinuing instead of throwing on last steps
            Boolean isContinuing = true;

            try {

                //tapp.ScreenUpdating = false;
                tapp.Calculation = MOIE.XlCalculation.xlCalculationManual;

                int i, iP1, ii, iiP1, j, jP1, jj, jjP1;


                using ( OpenFileDialog aOpenFileDialog = new OpenFileDialog() ) {
                    aOpenFileDialog.InitialDirectory = tapp.ActiveWorkbook.Path;
                    aOpenFileDialog.Filter = "HDF5 Files (*.h5)|*.h5|All Files (*.*)|*.*";
                    aOpenFileDialog.FilterIndex = 1;
                    aOpenFileDialog.RestoreDirectory = true;
                    aOpenFileDialog.CheckPathExists = true;
                    aOpenFileDialog.CheckFileExists = true;
                    aOpenFileDialog.FileName = sFileName;
                    aOpenFileDialog.AddExtension = true;
                    aOpenFileDialog.Title = "Import compound data from HDF5 (*.h5) File....";

                    if ( aOpenFileDialog.ShowDialog() == DialogResult.OK )
                        sFileName = aOpenFileDialog.FileName;
                    else
                        isContinuing = false;
                }

                if ( isContinuing ) {
                    String hDSPath = tapp.InputBox("Input path (with or without initial root /), if left as \"/\" and there is only 1 data set, that is used", "HDF5 path to dataset in file", "/");

                    aRecordSetMD = new RecordSetMD(RecordSetMDEnums.eMode.Input)
                    .cAs(RecordSetMDEnums.eType.HDF5)
                    .cFromFile(sFileName)
                    .cWithDataSetName(hDSPath)
                    .mReadMapFor(null, null, true)
                    ;

                    aRecordSet = new RecordSet()
                    .cAsInput()
                    .mReadRecordSet(aRecordSetMD)
                    ;

                    MessageBoxButtons msgboxbuttons = MessageBoxButtons.YesNoCancel;
                    DialogResult msgboxresponse;

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
                            tws.Name = aRecordSetMD.HDF5Matter.DSName;
                        }
                        catch ( Exception e ) {
                            String s = tapp.InputBox("Cannot name sheet to " + aRecordSetMD.HDF5Matter.DSName, "New Sheet Name", "Leave-As-Is", 100, 100, "");
                            if ( !s.Equals("Leave-As-Is") ) {
                                try {
                                    tws.Name = s;
                                }
                                catch ( Exception ) {

                                }
                            }
                        }
                    }

                    int nRows = aRecordSet.Records.Count;
                    int nColumns = aRecordSetMD.nColumns();

                    tapp.ScreenUpdating = false;
                    tapp.Calculation = MOIE.XlCalculation.xlCalculationManual;
                    for ( jj = 0, jjP1 = 1 ; jj < nColumns ; jj++, jjP1++ ) {
                        trng2.Offset[0, jj].Value2 = aRecordSetMD.Column[jj].Name;
                        for ( i = 0, iP1 = 1 ; i < nRows ; i++, iP1++ )
                            trng2.Offset[iP1, jj].Value2 = aRecordSet.Records_Orig[i][jj];
                    }

                    tblr = tws.Range[trng2, trng2.Offset[nRows, nColumns - 1]];
                    tbl = (MOIE.ListObject) tws.ListObjects.AddEx(MOIE.XlListObjectSourceType.xlSrcRange, tblr, null, MOIE.XlYesNoGuess.xlYes);

                }

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
                selection = null;
                if ( aRecordSetMD != null ) aRecordSetMD.Dispose();
                aRecordSetMD = null;
                if ( aRecordSet != null ) aRecordSet.Dispose();
                aRecordSet = null;
                tapp = null;
                twb = null;
                twbSheets = null;
                tws = null;
                trng = null;
                trng2 = null;
                tblr = null;
                tbl = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return;
        }
    }

}
