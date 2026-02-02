Imports ExcelDna.Integration
Imports MOIE = Microsoft.Office.Interop.Excel

'Namespace com.WDataSci.WDS

Public Module VBAUtil

        'Option Base 1
        Const WDSVBACommandBarContextID = 40001

        Public Const WDSVBAModuleCheckSheetName = "VBA_Modules_Check"

    <ExcelCommand(ExplicitRegistration:=True)>
    Public Sub WDSVBAImportExport_Guts()
            Dim tapp As MOIE.Application
            tapp = ExcelDnaUtil.Application
            Dim twb As MOIE.Workbook = tapp.ActiveWorkbook
            Dim tws As MOIE.Worksheet = twb.ActiveSheet
            If tws.Name <> WDSVBAModuleCheckSheetName Then
                MsgBox("Do not run except on a macro generated sheet called """ + WDSVBAModuleCheckSheetName + """!  Cells may be erased....")
                Err.Raise(Number:=WDSVBACommandBarContextID + 1, Source:=twb.Name & ".WDSVBACommandBar", Description:="Do not run except on a macro generated sheet called """ + WDSVBAModuleCheckSheetName + """!")
            End If
            __WDSVBAImportExport_Guts(twb, tws)
            twb = Nothing
            tws = Nothing
            tapp = Nothing
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Sub

        Private Sub __WDSVBAImportExport_Guts(twb As MOIE.Workbook, tws As MOIE.Worksheet)
            If tws.Name <> WDSVBAModuleCheckSheetName Then
                MsgBox("Do not run except on a macro generated sheet called """ + WDSVBAModuleCheckSheetName + """!  Cells may be erased....")
                Err.Raise(Number:=WDSVBACommandBarContextID + 1, Source:=twb.Name & ".WDSVBACommandBar", Description:="Do not run except on a macro generated sheet called """ + WDSVBAModuleCheckSheetName + """!")
            End If
            If tws.Cells.SpecialCells(MOIE.XlCellType.xlCellTypeLastCell).Row > 3 Then
                tws.Range(tws.Cells(4, 1), tws.Cells.SpecialCells(MOIE.XlCellType.xlCellTypeLastCell)).Clear()
            End If

            Dim i, j As Integer
            i = 3
            j = i
        For Each o In twb.VBProject.VBComponents
            If o.Type = 1 Then
                i = i + 1
                tws.Cells(i, 1).Value = o.Name
            ElseIf o.Type = 2 Then
                i = i + 1
                tws.Cells(i, 1).Value = o.Name
            End If
        Next
        If Microsoft.VisualBasic.FileIO.FileSystem.DirectoryExists(tws.Cells(2, 2).Value) Then
            Dim f As String
            For Each f In Microsoft.VisualBasic.FileIO.FileSystem.GetFiles(tws.Cells(2, 2).Value)
                j = j + 1
                tws.Cells(j, 2).Value = f
            Next
        End If

        tws.Cells.Columns.AutoFit()
            tws.Cells(1, 1).Activate()
        End Sub

        Public Sub WDSVBAExportSelected()
            Dim tapp As MOIE.Application
            tapp = ExcelDnaUtil.Application
            Dim twb As MOIE.Workbook = tapp.ActiveWorkbook
            Dim tws As MOIE.Worksheet = twb.ActiveSheet
            If tws.Name <> WDSVBAModuleCheckSheetName Then
                MsgBox("Do not run except on a macro generated sheet called """ + WDSVBAModuleCheckSheetName + """!  Cells may be erased....")
                Err.Raise(Number:=WDSVBACommandBarContextID + 1, Source:=twb.Name & ".WDSVBACommandBar", Description:="Do not run except on a macro generated sheet called """ + WDSVBAModuleCheckSheetName + """!")
            End If
            Dim mr As MOIE.Range
            Dim selection As ExcelReference
            selection = XlCall.Excel(XlCall.xlfSelection)
            mr = tapp.Evaluate(XlCall.Excel(XlCall.xlfReftext, selection, True))

            Dim c As MOIE.Range
            Dim fn As String

            If Microsoft.VisualBasic.FileIO.FileSystem.DirectoryExists(tws.Cells(2, 2).Value) Then
                For Each c In mr
                    If c.Row <= 3 Or c.Column <> 1 Then
                        Exit For
                    End If
                    For Each o In twb.VBProject.VBComponents
                        If o.Name = c.Value2 Then
                            fn = tws.Cells(2, 2).Value & "\" & o.Name & ".bas"
                            While Microsoft.VisualBasic.FileIO.FileSystem.FileExists(fn)
                                fn += ".tmp"
                            End While
                            o.Export(fn)
                        End If
                    Next
                Next
            End If

            __WDSVBAImportExport_Guts(twb, tws)
            mr = Nothing
            selection = Nothing
            tapp = Nothing
            c = Nothing

        End Sub

        Public Sub WDSVBADeleteSelected()
            Dim tapp As MOIE.Application
            tapp = ExcelDnaUtil.Application
            Dim twb As MOIE.Workbook = tapp.ActiveWorkbook
            Dim tws As MOIE.Worksheet = twb.ActiveSheet
            If tws.Name <> WDSVBAModuleCheckSheetName Then
                MsgBox("Do not run except on a macro generated sheet called """ + WDSVBAModuleCheckSheetName + """!  Cells may be erased....")
                Err.Raise(Number:=WDSVBACommandBarContextID + 1, Source:=twb.Name & ".WDSVBACommandBar", Description:="Do not run except on a macro generated sheet called """ + WDSVBAModuleCheckSheetName + """!")
            End If
            Dim mr As MOIE.Range
            Dim selection As ExcelReference
            tapp = ExcelDnaUtil.Application
            selection = XlCall.Excel(XlCall.xlfSelection)
            mr = tapp.Evaluate(XlCall.Excel(XlCall.xlfReftext, selection, True))

            Dim c As MOIE.Range

            For Each c In mr
                If c.Row <= 3 Or c.Column <> 1 Then
                    Exit For
                End If
                For Each o In twb.VBProject.VBComponents
                    If o.Name = c.Value Then
                        twb.VBProject.VBComponents.Remove(o)
                    End If
                Next
            Next

            __WDSVBAImportExport_Guts(twb, tws)
            mr = Nothing
            selection = Nothing
            tapp = Nothing
            c = Nothing

        End Sub

        Public Sub WDSVBAImportSelected()
            Dim tapp As MOIE.Application
            tapp = ExcelDnaUtil.Application
            Dim twb As MOIE.Workbook = tapp.ActiveWorkbook
            Dim tws As MOIE.Worksheet = twb.ActiveSheet
            If tws.Name <> WDSVBAModuleCheckSheetName Then
                MsgBox("Do not run except on a macro generated sheet called """ + WDSVBAModuleCheckSheetName + """!  Cells may be erased....")
                Err.Raise(Number:=WDSVBACommandBarContextID + 1, Source:=twb.Name & ".WDSVBACommandBar", Description:="Do not run except on a macro generated sheet called """ + WDSVBAModuleCheckSheetName + """!")
            End If
            Dim mr As MOIE.Range
            Dim selection As ExcelReference
            tapp = ExcelDnaUtil.Application
            selection = XlCall.Excel(XlCall.xlfSelection)
            mr = tapp.Evaluate(XlCall.Excel(XlCall.xlfReftext, selection, True))

            Dim c As MOIE.Range

            Dim fn As String

            For Each c In mr
                If c.Row <= 3 Or c.Column <> 2 Then
                    Exit For
                End If
                'fn = tws.Cells(2, 2).Value & "\" & c.Value
                fn = c.Value
                If Microsoft.VisualBasic.FileIO.FileSystem.FileExists(fn) Then
                    twb.VBProject.VBComponents.Import(fn)
                End If
            Next

            __WDSVBAImportExport_Guts(twb, tws)
            mr = Nothing
            mr = Nothing
            selection = Nothing
            tapp = Nothing
            c = Nothing

        End Sub

        Public Sub WDSVBAModuleReview()
            'switching from ExcelDna to MOIE
            Dim tapp As MOIE.Application
            tapp = ExcelDnaUtil.Application

            Dim calcprior As MOIE.XlCalculation
            calcprior = tapp.Calculation

            Dim resp As String = "Continue"

            Dim twb As MOIE.Workbook = tapp.ActiveWorkbook
            Dim nws As MOIE.Worksheet
            Dim nws_nm As String = WDSVBAModuleCheckSheetName

            Try
                tapp.Calculation = Microsoft.Office.Interop.Excel.XlCalculation.xlCalculationManual

                Try
                    nws = twb.Worksheets(nws_nm)
                    resp = tapp.InputBox(WDSVBAModuleCheckSheetName + " Is already a worksheet, Continue And clear worksheet, Or Cancel?", "Another sheet already has this name", "Continue")
                    nws.Activate()
                Catch ex As Exception
                    Try
                        nws = tapp.ActiveWorkbook.Sheets.Add()
                        nws.Name = nws_nm
                    Catch ex2 As Exception
                        resp = tapp.InputBox("Problem adding " + WDSVBAModuleCheckSheetName, "Problem adding " + WDSVBAModuleCheckSheetName, "Continue")
                        Exit Sub
                    End Try
                End Try

                If resp <> "Continue" Then Exit Sub

                Dim n As Integer = twb.Sheets.Count

                nws.Cells.Clear()
                Dim i As Integer

                i = 1
                nws.Cells(i, 1).Value = "VBA Modules, Import/Export"
                nws.Range(nws.Cells(i, 1), nws.Cells(i, 5)).Merge(True)

                i = 2
                nws.Cells(i, 1).Value = "VBA External Location"

                Dim s As String
                s = AppDomain.CurrentDomain.BaseDirectory
                If s.EndsWith("\lib") Then
                    s += "\VBA"
                Else
                    s += "\..\..\..\WDS-VBA"
                End If

                nws.Cells(i, 2).Value = s

                nws.Cells(i, 3).Value = "<<<If manually changing External Location, use refresh VBA module check sheet"

                i = 3
                nws.Cells(i, 1).Value = "Modules In WorkBook"
                nws.Cells(i, 2).Value = "Modules Available"

                __WDSVBAImportExport_Guts(twb:=twb, tws:=nws)

            Catch ex As Exception

            End Try

            tapp.Calculation = calcprior

            nws = Nothing
            twb = Nothing
            tapp = Nothing

            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()

        End Sub

        Public Sub WDSVBAModuleReviewRefresh()
            Dim tapp As MOIE.Application
            tapp = ExcelDnaUtil.Application
            Dim twb As MOIE.Workbook = tapp.ActiveWorkbook
            Dim tws As MOIE.Worksheet = twb.ActiveSheet
            If tws.Name <> WDSVBAModuleCheckSheetName Then
                MsgBox("Do not run except on a macro generated sheet called """ + WDSVBAModuleCheckSheetName + """!  Cells may be erased....")
                Err.Raise(Number:=WDSVBACommandBarContextID + 1, Source:=twb.Name & ".WDSVBACommandBar", Description:="Do not run except on a macro generated sheet called """ + WDSVBAModuleCheckSheetName + """!")
            End If
            __WDSVBAImportExport_Guts(twb:=twb, tws:=tws)
            tws = Nothing
            twb = Nothing
            tapp = Nothing
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Sub

        Public Sub WDSRemoveVBACheckSheet()
            'switching from ExcelDna to MOIE
            Dim tapp As MOIE.Application
            tapp = ExcelDnaUtil.Application

            Dim twb As MOIE.Workbook = tapp.ActiveWorkbook
            Dim nws As MOIE.Worksheet
            Dim nws_nm As String = WDSVBAModuleCheckSheetName

            Try
                nws = twb.Worksheets(nws_nm)
                nws.Delete()
            Catch ex As Exception
                MsgBox(WDSVBAModuleCheckSheetName + " is not a worksheet")
            End Try

            nws = Nothing
            twb = Nothing
            tapp = Nothing

            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()

        End Sub

    End Module

'End Namespace