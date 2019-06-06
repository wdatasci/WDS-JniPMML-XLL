Imports ExcelDna.Integration
Imports MOIE = Microsoft.Office.Interop.Excel
Imports Microsoft.VisualBasic.FileIO

'Namespace com.WDataSci.JniPMML

Public Module JniPMMLVBAConstructor
        'Option Base 1
        Const WDSVBACommandBarContextID = 40001
        Public Const WDSJniPMMLModuleName = "WDSJniPMML"

        Public Sub JniPMMLVBACheck()
            'switching from ExcelDna to MOIE
            Dim tapp As MOIE.Application
            tapp = ExcelDnaUtil.Application

            Dim twb As MOIE.Workbook = tapp.ActiveWorkbook
            Dim tws As MOIE.Worksheet = twb.ActiveSheet
            If tws.Name <> VBAUtil.WDSVBAModuleCheckSheetName Then
                MsgBox("Do not run except on a macro generated sheet called """ + VBAUtil.WDSVBAModuleCheckSheetName + """!  Cells may be erased....")
                Err.Raise(Number:=WDSVBACommandBarContextID + 1, Source:=twb.Name & ".JniPMMLVBACheck", Description:="Do not run except on a macro generated sheet called """ + VBAUtil.WDSVBAModuleCheckSheetName + """!")
            End If

            Dim o As Microsoft.Vbe.Interop.VBComponent

            Dim resp As String = "Continue"

            Dim found As Boolean = False

            For Each o In twb.VBProject.VBComponents
                If o.Type = 1 Then
                    If o.Name = WDSJniPMMLModuleName Then
                        found = True
                        Exit For
                    End If
                End If
            Next

            resp = "No"
            If Not found Then
                resp = tapp.InputBox(WDSJniPMMLModuleName + " Is Not a VBA module, would you Like to add it? Yes/No", "Missing " + WDSJniPMMLModuleName + ".bas", "Yes")
                If resp = "Yes" Then
                    _Add_WDSJniPMML(twb)
                    VBAUtil.WDSVBAImportExport_Guts() 'twb:=twb, tws:=tws
                End If
            Else
                MsgBox(WDSJniPMMLModuleName + " is already a VBA module, Remove and Re-Add if necessary")
            End If


            tws = Nothing
            twb = Nothing
            tapp = Nothing
            o = Nothing

            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()

        End Sub

        Public Sub WDSVBAComponentAdd_WDSJniPMML()
            'switching from ExcelDna to MOIE
            Dim tapp As MOIE.Application
            tapp = ExcelDnaUtil.Application
            Dim twb As MOIE.Workbook = tapp.ActiveWorkbook
            _Add_WDSJniPMML(twb)
            twb = Nothing
            tapp = Nothing
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Sub

        Public Sub WDSVBAComponentRemove_WDSJniPMML()
            'switching from ExcelDna to MOIE
            Dim tapp As MOIE.Application
            tapp = ExcelDnaUtil.Application
            Dim twb As MOIE.Workbook = tapp.ActiveWorkbook
            For Each o In twb.VBProject.VBComponents
                If o.Type = 1 Then
                    If o.Name = WDSJniPMMLModuleName Then
                        twb.VBProject.VBComponents.Remove(o)
                        Exit For
                    End If
                End If
            Next
            twb = Nothing
            tapp = Nothing
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Sub

        Private Sub _Add_WDSJniPMML(twb As MOIE.Workbook)

            Dim newModule As Microsoft.Vbe.Interop.VBComponent = twb.VBProject.VBComponents.Add(Microsoft.Vbe.Interop.vbext_ComponentType.vbext_ct_StdModule)
            newModule.Name = WDSJniPMMLModuleName

            Dim ms As String
            'just for formatting purposes below, mstab will be stripped out of string before writing to module
            Dim mstab As String = vbNewLine + "                "
            Dim ms_wo_leaders As String
            Dim msmo As String = ""
            Dim n As Integer
            n = 0

            Try

                ms = "'WDSJniPMML - A few small VBA functions and wraps of JniPMML which Excel can handle in a non-volatile manner
                '
                'CodeDoc - CJW - The jPMML evaluator JNI wrapper AddIn becomes a COM-AddIn and uses ExcelDNA as generic 
                'AddIn facilitator.  Normally, in VBA, one might denote an argument ""ByRef arg As Range"" in order to extract 
                'information such as the ListObject and XMLMap. This can be done in ExcelDNA C# or VB, however, the function becomes 
                '""volatile"", which means large blocks of evaluations might be needlessly recalculated every time there is
                'a minor change anywhere in the workbook.  
                '
                Option base 1
                Const WDSCoreContextID = 40002
                Const WDSVBAModuleName = """ + WDSJniPMMLModuleName + """
                "



                ms_wo_leaders = Replace(ms, mstab, vbNewLine)
                newModule.CodeModule.InsertLines(1, ms_wo_leaders)

                ms = "
                Public Function JniPMML_Eval_WithoutCache( _
                    ByVal bToCalcSwitch as Integer _
                    , ByVal PMMLInput as String _
                    , ByVal bInputDataHasHeaderRow as Integer _
                    , ByRef InputTableReference as Range _
                    , Optional nOutputStringMaxLength = 64 _
                    ) 
                    JniPMML_Eval_WithoutCache=Application.Run(""JniPMML_Eval_Volatile"", bToCalcSwitch, PMMLInput, bInputDataHasHeaderRow, InputTableReference, nOutputStringMaxLength)
                    Application.Volatile(False) ' setting at the top may not kill volatility
                End Function
                Private Function JniPMML_Eval_WithoutCache_MacroOptions_Array() as Variant
                    JniPMML_Eval_WithoutCache_MacroOptions_Array = Array(""JniPMML_Eval_WithoutCache"" _
                        , ""A non-volatile self contained call to the JniPMML evaluator (VBA wrap of JniPMML_Eval_Volatile).  The first argument is just to turn it off/on to kill the drag on calculation time."" _
                        , ""http://WDataSci.com"" _
                        , ""WDS.JniPMML"" _
                        , Array(Array(""ToCalcSwitch"",""0/1, just to kill calculation""), _
                                    Array(""PMMLInput"",""Path to external PMML filename or entire file as a string (an entire string is determined by the usual XML starting characters)""), _
                                    Array(""InputDataHasHeaderRow"",""0/1, If input includes header row, output will include header row.""), _
                                    Array(""InputTableReference"",""An XMLMap'd and exportable ListObject Table, column names are taken from the XMLMap""), _
                                    Array(""OutputStringMaxLength"",""An optional output maximum string length, defaults to 64"") _
                        ) _
                    )
                End Function
                "
                n += 1
                msmo += "   i=i+1" + Environment.NewLine
                msmo += "   x(i) = JniPMML_Eval_WithoutCache_MacroOptions_Array()" + Environment.NewLine
                ms_wo_leaders = Replace(ms, mstab, vbNewLine)
                newModule.CodeModule.InsertLines(newModule.CodeModule.CountOfLines + 1, ms_wo_leaders)

                ms = "
                Public Function JniPMML_Eval_CacheHeaders( _
                    HandleOrTag _
                    , ByRef XmlMappedListRef as Range _
                    , Optional nOutputStringMaxLength = 64 _
                    ) 
                    JniPMML_Eval_CacheHeaders=Application.Run(""JniPMML_Eval_CacheHeaders_Volatile"", HandleOrTag, XmlMappedListRef, nOutputStringMaxLength)
                    Application.Volatile(False) ' setting at the top may not kill volatility
                End Function
                Private Function JniPMML_Eval_CacheHeaders_MacroOptions_Array() as Variant
                    JniPMML_Eval_CacheHeaders_MacroOptions_Array = Array(""JniPMML_Eval_CacheHeaders"" _
                        , ""Caches just the input and output headers for Eval, on both the C# and Java sides, subsequent calls to WDS.JniPMML__Headerless can follow.  Set the headerless calls to depend on this Major.Minor output."" _
                        , ""http://WDataSci.com"" _
                        , ""WDS.JniPMML"" _
                        , Array(Array(""HandleOrTag"", ""Use a Major.Minor Handle output from CreateHandle to chain calcuation dependency""), _
                                    Array(""XmlMappedListRef"", ""Point to a reference cell or range of the XmlMap'd list [one that does not change with data, such as the header]""), _
                                    Array(""nOutputStringMaxLength"", ""An optional output maximum string length, defaults to 64"") _
                        ) _
                    )
                End Function
                "
                n += 1
                msmo += "   i=i+1" + Environment.NewLine
                msmo += "   x(i) = JniPMML_Eval_CacheHeaders_MacroOptions_Array()" + Environment.NewLine
                ms_wo_leaders = Replace(ms, mstab, vbNewLine)
                newModule.CodeModule.InsertLines(newModule.CodeModule.CountOfLines + 1, ms_wo_leaders)

                'wrap up with one macro to call all the MacroOptions
                ms = "
                Public Sub WDSJniPMML_CallMacroOptions()
                    Dim x(1 to " + n.ToString() + ") as Variant
                    Dim i as Integer
                    i=0
                " + msmo + "
                    Call WDSCore_SetMacroOptions(x)
                End Sub
                "
                ms_wo_leaders = Replace(ms, mstab, vbNewLine)
                newModule.CodeModule.InsertLines(newModule.CodeModule.CountOfLines + 1, ms_wo_leaders)

            Catch ex As Exception

                MsgBox(ex.Message)

            End Try
        End Sub

    End Module

'End Namespace
