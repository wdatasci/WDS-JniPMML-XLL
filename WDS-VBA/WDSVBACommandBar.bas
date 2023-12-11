Attribute VB_Name = "WDSVBACommandBar"
'''Wypasek Data Science, Inc., Copyright 2019, 2020, 2021, 2022, 2023
'''Author: Christian Wypasek
Option Base 1
Const WDSVBACommandBarContextID = 40001

Public Sub wgu_WDSVBACommandBar()

Dim cmdbar As CommandBar
Dim cmbbox As CommandBarComboBox

TryIt:
On Error GoTo CatchIt
    Set cmdbar = CommandBars.Add("Custom")
GoTo ElseIt
CatchIt:
    Set cmdbar = CommandBars("Custom")
ElseIt:

Set cmbbox = cmdbar.Controls.Add(Type:=msoControlDropdown)
With cmbbox
    .Caption = "WDS VBA CommandBar"
    .Style = msoComboLabel
    .AddItem "Macros"
    .AddItem "-WorkBook Construction"
    .AddItem "--List VBA Modules"
    .AddItem "--VBA Import/Export"
    .AddItem "----VBA Import/Export Refresh"
    .AddItem "----VBA Import Selected"
    .AddItem "----VBA Export Selected"
    .AddItem "----VBA Delete Local Selected"
    .AddItem "Remove WDS VBA CommandBar"
    
    .OnAction = "WDSVBACommandBar_"
End With
cmdbar.Visible = True

End Sub
Private Sub WDSVBACommandBar_()
Dim cmdbar As CommandBarComboBox
Set cmdbar = CommandBars("Custom").Controls("WDS VBA CommandBar")

TryIt:
On Error GoTo CatchIt
    
    i = cmdbar.ListIndex
    c = cmdbar.List(i)
    Select Case c
        Case "Remove WDS VBA CommandBar"
            cmdbar.Delete
            Exit Sub
        Case "--List VBA Modules"
            ListVBAModules
        Case "--VBA Import/Export"
            VBAImportExport
        Case "----VBA Import/Export Refresh"
            VBAImportExport_Guts twb:=ActiveWorkbook, tws:=ActiveSheet
        Case "----VBA Import Selected"
            VBAImportSelected twb:=ActiveWorkbook, tws:=ActiveSheet
            VBAImportExport_Guts twb:=ActiveWorkbook, tws:=ActiveSheet
        Case "----VBA Export Selected"
            VBAExportSelected twb:=ActiveWorkbook, tws:=ActiveSheet
            VBAImportExport_Guts twb:=ActiveWorkbook, tws:=ActiveSheet
        Case "----VBA Delete Local Selected"
            VBADeleteSelected twb:=ActiveWorkbook, tws:=ActiveSheet
            VBAImportExport_Guts twb:=ActiveWorkbook, tws:=ActiveSheet
    
    End Select

GoTo ElseIt
CatchIt:
    

ElseIt:

    cmdbar.ListIndex = 1

End Sub


Private Sub ListVBAModules()
    Dim twb As Workbook
    Set twb = ActiveWorkbook
    WDSVBACommandBar.ActivateOrAddSheet ("VBAModules")
    Dim tws As Worksheet
    Set tws = ActiveSheet
    tws.Cells.Clear
    i = 1
    tws.Cells(i, 1).Value = "VBProject.VBComponents"
    tws.Cells(i, 2).Value = "Type"
    
    For Each o In twb.VBProject.VBComponents
    If o.Type = 1 Then
        i = i + 1
        tws.Cells(i, 1).Value = o.Name
        tws.Cells(i, 2).Value = o.Type
    End If
    Next
    
    tws.Cells.Columns.AutoFit
'    tws.Cells(1, 1).Activate
End Sub


Private Sub VBAImportExport()
    Dim twb As Workbook
    Set twb = ActiveWorkbook
    WDSVBACommandBar.ActivateOrAddSheet ("VBAModules")
    Dim tws As Worksheet
    Set tws = ActiveSheet
    tws.Cells.Clear
    i = 1
    tws.Cells(i, 1).Value = "VBA Import/Export"
    i = 2
    tws.Cells(i, 1).Value = "VBA External Location"
    tws.Cells(i, 2).Value = "D:\WDS\dev\lib\VBA\WDSCommon"
    tws.Cells(i, 3).Value = "V:\SystemsModelV1.1\SMProtoType\bas"
    
    i = 3
    tws.Cells(i, 1).Value = "Modules In WorkBook"
    tws.Cells(i, 2).Value = "Modules Available"
    VBAImportExport_Guts twb:=twb, tws:=tws
    
End Sub

Private Sub VBAImportExport_Guts(ByRef twb As Workbook, tws As Worksheet)
    If tws.Name <> "VBAModules" Then
        MsgBox ("Do not run except on a macro generated sheet called ""VBAModules""!  Cells may be erased....")
        Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & ".WDSVBACommandBar", Description:="Do not run except on a macro generated sheet called ""VBAModules""!"
    End If
    If tws.Cells.SpecialCells(xlCellTypeLastCell).Row > 3 Then: _
        tws.Range(tws.Cells(4, 1), tws.Cells.SpecialCells(xlCellTypeLastCell)).Clear
    
    i = 3
    j = i
    For Each o In twb.VBProject.VBComponents
        If o.Type = 1 Then
            i = i + 1
            tws.Cells(i, 1).Value = o.Name
        End If
    Next
    Dim fso As FileSystemObject
    Set fso = New FileSystemObject
    If fso.FolderExists(tws.Cells(2, 2).Value) Then
        For Each f In fso.GetFolder(tws.Cells(2, 2).Value).Files
            j = j + 1
            tws.Cells(j, 2).Value = f.Name
        Next
    End If
    
    
    tws.Cells.Columns.AutoFit
    tws.Cells(1, 1).Activate
    
End Sub
Private Sub VBAExportSelected(ByRef twb As Workbook, tws As Worksheet)
    If tws.Name <> "VBAModules" Then
        MsgBox ("Do not run except on a macro generated sheet called ""VBAModules""!  Cells may be erased....")
        Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & ".WDSVBACommandBar", Description:="Do not run except on a macro generated sheet called ""VBAModules""!"
    End If
    Dim x As Range
    Set x = Selection
    Dim c As Range
    
    Dim fso As FileSystemObject
    Set fso = New FileSystemObject
    If fso.FolderExists(tws.Cells(2, 2).Value) Then
    For Each c In x
        If c.Row <= 3 Or c.Column <> 1 Then: Exit For
            For Each o In twb.VBProject.VBComponents
                If o.Name = c.Value Then
                    fn = tws.Cells(2, 2).Value & "\" & o.Name & ".bas"
                    While fso.FileExists(fn)
                        fn = fn & ".tmp"
                    Wend
                    o.Export fn
                End If
            Next
        Next
    End If
    
    
End Sub
Private Sub VBADeleteSelected(ByRef twb As Workbook, tws As Worksheet)
    If tws.Name <> "VBAModules" Then
        MsgBox ("Do not run except on a macro generated sheet called ""VBAModules""!  Cells may be erased....")
        Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & ".WDSVBACommandBar", Description:="Do not run except on a macro generated sheet called ""VBAModules""!"
    End If
    Dim x As Range
    Set x = Selection
    Dim c As Range
    
    For Each c In x
        If c.Row <= 3 Or c.Column <> 1 Then: Exit For
        For Each o In twb.VBProject.VBComponents
            If o.Name = c.Value Then
                twb.VBProject.VBComponents.Remove o
            End If
        Next
    Next
    
    
End Sub
Private Sub VBAImportSelected(ByRef twb As Workbook, tws As Worksheet)
    If tws.Name <> "VBAModules" Then
        MsgBox ("Do not run except on a macro generated sheet called ""VBAModules""!  Cells may be erased....")
        Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & ".WDSVBACommandBar", Description:="Do not run except on a macro generated sheet called ""VBAModules""!"
    End If
    Dim x As Range
    Set x = Selection
    Dim c As Range
    
    Dim fso As FileSystemObject
    Set fso = New FileSystemObject
    If fso.FolderExists(tws.Cells(2, 2).Value) Then
        For Each c In x
            If c.Row <= 3 Or c.Column <> 2 Then: Exit For
            fn = tws.Cells(2, 2).Value & "\" & c.Value
            If fso.FileExists(fn) Then
                twb.VBProject.VBComponents.Import (fn)
            End If
        Next
    End If
    
    
End Sub

Public Sub ActivateOrAddSheet(ByVal arg1 As String, Optional indx = 1, Optional BeforeOrAfter = 1)

TryIt:

On Error GoTo CatchIt

    Sheets(arg1).Activate

GoTo ElseIt
CatchIt:

    Dim NewSheet As Worksheet
    Set NewSheet = Sheets.Add
    NewSheet.Name = arg1
    If BeforeOrAfter = 1 Then
        NewSheet.Move Before:=Sheets(indx)
    Else
        NewSheet.Move After:=Sheets(indx)
    End If
    
ElseIt:

End Sub

