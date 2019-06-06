Imports ExcelDna.Integration
Imports MOIE = Microsoft.Office.Interop.Excel
Imports Microsoft.VisualBasic.FileIO

'Namespace com.WDataSci.WDS

Public Module WDSCoreVBAConstructor
        'Option Base 1
        Const WDSVBACommandBarContextID = 40001
        Public Const WDSCoreVBAModuleName = "WDSCore"

        Public Sub WDSCoreVBACheck()
            Dim tapp As MOIE.Application
            tapp = ExcelDnaUtil.Application

            Dim twb As MOIE.Workbook = tapp.ActiveWorkbook
            Dim tws As MOIE.Worksheet = twb.ActiveSheet
            If tws.Name <> WDSVBAModuleCheckSheetName Then
                MsgBox("Do not run except on a macro generated sheet called """ + WDSVBAModuleCheckSheetName + """!  Cells may be erased....")
                Err.Raise(Number:=WDSVBACommandBarContextID + 1, Source:=twb.Name & ".JniPMMLVBACheck", Description:="Do not run except on a macro generated sheet called """ + WDSVBAModuleCheckSheetName + """!")
            End If

            Dim o As Microsoft.Vbe.Interop.VBComponent

            Dim resp As String = "Continue"

            Dim found As Boolean = False

            For Each o In twb.VBProject.VBComponents
                If o.Type = 1 Then
                    If o.Name = WDSCoreVBAModuleName Then
                        found = True
                        Exit For
                    End If
                End If
            Next

            resp = "No"
            If Not found Then
                resp = tapp.InputBox(WDSCoreVBAModuleName + " Is Not a VBA module, would you Like to add it? Yes/No", "Missing " + WDSCoreVBAModuleName + ".bas", "Yes")
                If resp = "Yes" Then
                    _Add_WDSCoreVBA(twb)
                    WDSVBAImportExport_Guts() 'twb:=twb, tws:=tws
                End If
            Else
                MsgBox(WDSCoreVBAModuleName + " is already a VBA module, Remove and Re-Add if necessary")
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

        Public Sub WDSVBAComponentAdd_WDSCore()
            'switching from ExcelDna to MOIE
            Dim tapp As MOIE.Application
            tapp = ExcelDnaUtil.Application
            Dim twb As MOIE.Workbook = tapp.ActiveWorkbook
            _Add_WDSCoreVBA(twb)
            twb = Nothing
            tapp = Nothing
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Sub

        Public Sub WDSVBAComponentRemove_WDSCore()
            Dim tapp As MOIE.Application
            tapp = ExcelDnaUtil.Application
            Dim twb As MOIE.Workbook = tapp.ActiveWorkbook
            For Each o In twb.VBProject.VBComponents
                If o.Type = 1 Then
                    If o.Name = WDSCoreVBAModuleName Then
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

        Private Sub _Add_WDSCoreVBA(twb As MOIE.Workbook)


            Dim tapp As MOIE.Application
            tapp = twb.Application

            Dim a As MOIE.AddIn
            Dim p As String
            Dim ms As String

            p = "NA"
            For Each a In tapp.AddIns
                If a.Name.StartsWith("WDS") Or a.Name.StartsWith("JniPMML") Then
                    If FileSystem.DirectoryExists(a.Path + "\VBA") Then
                        If FileSystem.FileExists(a.Path + "\VBA\" + WDSCoreVBAModuleName + ".bas") Then
                            p = a.Path + "\VBA\WDSCore.bas"
                        End If
                        Exit For
                    End If
                End If
            Next

            If p = "NA" Then
                For Each a In tapp.AddIns2
                    If a.Name.StartsWith("WDS") Or a.Name.StartsWith("JniPMML") Then
                        If FileSystem.DirectoryExists(a.Path + "\VBA") Then
                            If FileSystem.FileExists(a.Path + "\VBA\" + WDSCoreVBAModuleName + ".bas") Then
                                p = a.Path + "\VBA\" + WDSCoreVBAModuleName + ".bas"
                            End If
                            Exit For
                        ElseIf FileSystem.DirectoryExists(a.Path + "\..\..\..\WDS-VBA") Then
                            If FileSystem.FileExists(a.Path + "\..\..\..\WDS-VBA\" + WDSCoreVBAModuleName + ".bas") Then
                                p = a.Path + "\..\..\..\WDS-VBA\" + WDSCoreVBAModuleName + ".bas"
                            End If
                            Exit For
                        End If
                    End If
                Next
            End If





            If p = "NA" Then
                MsgBox("Cannot find VBA\WDSCore.bas in the directory WDS-VB.xll or JniPMML-VB.xll, try VBA Module Check macro and point to location")
            Else

                ms = FileSystem.ReadAllText(p)

                Dim newModule As Microsoft.Vbe.Interop.VBComponent
                newModule = twb.VBProject.VBComponents.Add(Microsoft.Vbe.Interop.vbext_ComponentType.vbext_ct_StdModule)
                newModule.Name = WDSCoreVBAModuleName
                newModule.CodeModule.InsertLines(newModule.CodeModule.CountOfLines + 1, ms)
                newModule = Nothing

            End If

        End Sub

    End Module

'End Namespace
