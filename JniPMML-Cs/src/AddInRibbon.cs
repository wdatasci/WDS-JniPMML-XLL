using System.Runtime.InteropServices;
using ExcelDna.Integration;
using ExcelDna.Integration.CustomUI;

using MOIE=Microsoft.Office.Interop.Excel;

using WDataSci_JniPMML_XLL;

namespace WDataSci.JniPMML
{
    /// <summary>
    /// WDataSci Excel ribbon creation.
    /// </summary>
    [ComVisible(true)]
    public class RibbonController : ExcelRibbon
    {

        public override string GetCustomUI(string uiName)
        {
            return
            @"<customUI xmlns='http://schemas.microsoft.com/office/2006/01/customui' loadImage='LoadImage'>
                <ribbon>
                    <tabs>
                    <tab id='WDSTab' label='WDS'>
                        <group id='WDSGroup0' label='About'>
                        <button id='Button0' 
                                image='WDataSciMark1300x240' 
                                size='large' 
                                onAction='RunTagMacro' 
                                tag='ShowAboutForm' 
                                />
                        </group>
                        <group id='WDSGroup1' label='WDS HDF5/FlatFile'>
                        <menu id='WDSGroup1M1' label='HDF5' image='WDataSciMark1300x240'>
                        <button id='Button1a_1' 
                                label='Export XMLMapped List To HDF5' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='ExportXMLMappedListToHDF5' 
                                />
                        <button id='Button1a_2' 
                                label='Import HDF5 CompoundDS Type' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='ImportHDF5CompoundDS'
                                />
                        </menu>
                        <menu id='WDSGroup1M2' label='FlatFile' image='WDataSciMark1300x240'>
                        <button id='Button1b_1' 
                                label='Export XMLMapped ListObject To CSV' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='ExportXMLMappedListToCSV' 
                                />
                        <button id='Button1b_2' 
                                label='Import CSV to ListObject' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='ImportCSVToXMLMappedList'
                                />
                        </menu>
                        </group >
                        <group id='WDSGroup2' label='WDS JniPMML'>
                        <menu id='WDSGroup2M1' label='JniPMML' image='WDataSciMark1300x240'>
                        <button id='Button3_3' 
                                label='Add XmlMap To Selected ListObject' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='JniPMML_XmlMap_Helper' 
                                />
                        <button id='Button2_1' 
                                label='Evaluate XMLMapped List Via JniPMML' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='JniPMML_Eval_XmlMappedList'
                                />
                        <button id='Button2_2' 
                                label='JniPMML Java Cmd Line Call Prep' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='JniPMML_Cmd_Prep'
                                />
                        <button id='Button2_3' 
                                label='JniPMML Java Cmd Line Call' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='JniPMML_Cmd'
                                />
                        </menu>
                        <menu id='WDSGroup2M2' label='JniPMML VBA' image='WDataSciMark1300x240'>
                        <button id='Button2M2_1' 
                                label='Add VBA Module: WDSJniPMML' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='VBAComponentAdd_WDSJniPMML' 
                                />
                        <button id='Button2M2_2' 
                                label='Remove VBA Module: WDSJniPMML' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='VBAComponentRemove_WDSJniPMML' 
                                />
                        </menu>
                        </group >
                        <group id='WDSGroup3' label='WDS Workbook Helpers'>
                        <menu id='WDSGroup3M1' label='WDSCore VBA' image='WDataSciMark1300x240'>
                        <button id='Button3M1_1' 
                                label='Add VBA Module: WDSCore' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='VBAComponentAdd_WDSCore' 
                                />
                        <button id='Button3M1_2' 
                                label='Remove VBA Module: WDSCore' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='VBAComponentRemove_WDSCore' 
                                />
                        </menu>
                        <menu id='WDSGroup3M2' label='VBA Project' image='WDataSciMark1300x240'>
                        <button id='Button3M2_3' 
                                label='VBA Module Check Sheet' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='VBACheck' 
                                />
                        <button id='Button3M2_4' 
                                label=' -- Refresh VBA Module Check Sheet' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='VBACheckRefresh' 
                                />
                        <button id='Button3M2_5' 
                                label=' -- Import External Selections From Check Sheet' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='WDSVBAImportSelected' 
                                />
                        <button id='Button3M2_6' 
                                label=' -- Export Local Selections From Check Sheet' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='WDSVBAExportSelected' 
                                />
                        <button id='Button3M2_7' 
                                label=' -- Delete Local Selections From Check Sheet' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='WDSVBADeleteSelected' 
                                />
                        <button id='Button3M2_8' 
                                label='Remove VBA Module Check Sheet' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='VBACheckRemove' 
                                />
                        </menu>
                        <menu id='WDSGroup3M3' label='ListObjects' image='WDataSciMark1300x240'>
                        <button id='Button3M3_1' 
                                label='Add XmlMap To Selected ListObject' 
                                image='WDataSciMark1300x240' 
                                onAction='RunTagMacro' 
                                tag='JniPMML_XmlMap_Helper' 
                                />
                        </menu>
                        </group>
                    </tab>
                    </tabs>
                </ribbon>
                </customUI>";
        }


        [ExcelCommand(Description ="About"
            ,ExplicitRegistration =true
            )]
        public static void ShowAboutForm()
        {
            WDataSci_JniPMML_XLL_About wf = new WDataSci_JniPMML_XLL_About();
            wf.ShowDialog();
            return;
        }

        [ExcelCommand(Description ="VBA Module Check"
            ,ExplicitRegistration =true
            )]
        public static void VBACheck()
        {
            MOIE.Application tapp = (ExcelDnaUtil.Application as MOIE.Application);
            tapp.Run("WDSVBAModuleReview");
            tapp.Run("JniPMMLVBACheck");
            tapp.Run("WDSCoreVBACheck");
            return;
        }

        [ExcelCommand(Description ="VBA Module Check Refresh"
            ,ExplicitRegistration =true
            )]
        public static void VBACheckRefresh()
        {
            MOIE.Application tapp = (ExcelDnaUtil.Application as MOIE.Application);
            tapp.Run("WDSVBAModuleReviewRefresh");
            return;
        }

        [ExcelCommand(Description ="Remove VBA Module Check"
            ,ExplicitRegistration =true
            )]
        public static void VBACheckRemove()
        {
            MOIE.Application tapp = (ExcelDnaUtil.Application as MOIE.Application);
            tapp.Run("WDSRemoveVBACheckSheet");
            return;
        }

        [ExcelCommand(Description ="Add VBA Module: WDSJniPMML"
            ,ExplicitRegistration =true
            )]
        public static void VBAComponentAdd_WDSJniPMML()
        {
            MOIE.Application tapp = (ExcelDnaUtil.Application as MOIE.Application);
            tapp.Run("WDSVBAComponentAdd_WDSJniPMML");
            tapp.Run("WDSJniPMML_CallMacroOptions");
            return;
        }

        [ExcelCommand(Description ="Remove VBA Module: WDSJniPMML"
            ,ExplicitRegistration =true
            )]
        public static void VBAComponentRemove_WDSJniPMML()
        {
            MOIE.Application tapp = (ExcelDnaUtil.Application as MOIE.Application);
            tapp.Run("WDSVBAComponentRemove_WDSJniPMML");
            return;
        }


        [ExcelCommand(Description ="Add VBA Module: WDSCore"
            ,ExplicitRegistration =true
            )]
        public static void VBAComponentAdd_WDSCore()
        {
            MOIE.Application tapp = (ExcelDnaUtil.Application as MOIE.Application);
            tapp.Run("WDSVBAComponentAdd_WDSCore");
            tapp.Run("WDSCore_CallMacroOptions");
            return;
        }

        [ExcelCommand(Description ="Remove VBA Module: WDSCore"
            ,ExplicitRegistration =true
            )]
        public static void VBAComponentRemove_WDSCore()
        {
            MOIE.Application tapp = (ExcelDnaUtil.Application as MOIE.Application);
            tapp.Run("WDSVBAComponentRemove_WDSCore");
            return;
        }


    }

}



