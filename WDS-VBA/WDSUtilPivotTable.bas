Attribute VB_Name = "WDSUtilPivotTable"
Dim vLPT_SpecPage As String
Sub nnn_LoadPivotTableNotes()
    notes = "WDS VBACode to load data into a PivotTable " & Chr(10) & Chr(10) & _
        "The basic premise is a ""Spec"" page to script out the creation/recreation of a pivottable through an ODBC connection." & _
        Chr(10) & Chr(10) & _
        "Use aabLoadPivotTableODBCSpecProtoType as a startup example."
    MsgBox (notes)
End Sub

Sub cab_LoadPivotTableODBCSpecProtoType()
    WDSCore.ActivateOrAddSheet ("PivotTableODBCSpec")
    Range("A1").Value = "DSN or 'Existing'"
    Range("A2").Value = "Database or SourceSheet.Name"
    Range("A3").Value = "Table or SourcePivotTableName"
    
    Range("E1").Value = "Server"
    Range("E1").AddComment
    Range("E1").Comment.Visible = True
    Range("E1").Comment.Text Text:="If using a pre-defined DSN (system or user)," & Chr(10) & "clear this block"
    Application.DisplayCommentIndicator = xlCommentIndicatorOnly
       
    Range("F1").Value = "localhost"
    Range("E2").Value = "Port"
    Range("F2").Value = 3306
    Range("G1").Value = "user"
    Range("H1").Value = "root"
    Range("G2").Value = "password"
    Range("H2").Value = "??"
    
    
    Range("A4").Value = "TargetTable"
    Range("A5").Value = "TargetSheet"
    Range("A6").Value = "TargetCell (in R#C# format)"
    Range("A7").Value = "Pages"
    Range("A8").Value = "RowFields"
    Range("A9").Value = "ColumnFields"
    Range("A10").Value = "DataFields"
    Range("A11").Value = "CalculatedField"
    Range("B11").Value = "Formula"
    Range("A1:A10").Cells.Font.Bold = True
    Range("A11:B11").Cells.Font.Bold = True
    Range("A11:B11").Cells.Font.Underline = True

    ActiveSheet.Cells.EntireColumn.AutoFit
   
End Sub
Sub caa_LoadPivotTableODBCSpec()
    
    vLPT_SpecPage = "PivotTableODBCSpec"
    
    xxx_LoadPivotTableODBCSpecSub
    
End Sub
Sub caa_LoadPivotTableODBCSpecFromThisPage()
    
    vLPT_SpecPage = ActiveSheet.Name
    
    xxx_LoadPivotTableODBCSpecSub
    
End Sub
Sub caa_LoadPivotTableODBCSpecResetAllToSameSource()

' not ready for prime time

    vLPT_SpecPage = ActiveSheet.Name
    
    Dim x As Worksheet
    Dim y As PivotTable
    
    For Each x In ActiveWorkbook.Sheets
        If x.Name <> Range(vLPT_SpecPage & "!B5").Text And x.Name <> vLPT_SpecPage Then
            If x.PivotTables.Count > 0 Then
                x.Activate
                For Each y In x.PivotTables
                    y.DataBodyRange.Cells(1, 1).Select
'                    y.PivotCache = ActiveWorkbook.PivotCaches(Range(vLPT_SpecPage & "!B4").Text)
'   ActiveSheet.PivotTableWizard SourceType:=xlPivotTable, SourceData:= _
'        "BasePVTDataTable1"
                Next
            End If
            
        End If
    Next
        
        
'    Range("E22").Select
'    ActiveSheet.PivotTableWizard SourceType:=xlPivotTable, SourceData:= _
'        "BasePVTDataTable1"
'    ActiveWindow.SmallScroll Down:=45
End Sub

Sub xxx_LoadPivotTableODBCSpecSub()
    
    WDSCore.ActivateOrAddSheet (Range(vLPT_SpecPage & "!B5").Text)
    ActiveSheet.Cells.Clear
    
    thisUID = Range(vLPT_SpecPage & "!H1").Text
    thisPWD = Range(vLPT_SpecPage & "!H2").Text
    If thisUID = "??" Then
        thisUID = InputBox("UserID for this target server?")
    End If
    If thisPWD = "??" Then
        thisPWD = InputBox("Password for this target server?")
    End If
    

    cs = "ODBC;" & _
        "DSN=" & Range(vLPT_SpecPage & "!B1").Text & ";" & _
        "DATABASE=" & Range(vLPT_SpecPage & "!B2").Text & ";" & _
        "SERVER=" & Range(vLPT_SpecPage & "!F1").Text & ";" & _
        "PORT=" & Range(vLPT_SpecPage & "!F2").Text & ";" & _
        "UID=" & thisUID & ";" & _
        "PWD=" & thisUID & ";" & _
        "OPTION=0"

    If Range(vLPT_SpecPage & "!B1").Text <> "Existing" Then
        With ActiveWorkbook.PivotCaches.Add(SourceType:=xlExternal)
            .Connection = cs
            .CommandType = xlCmdSql
            .CommandText = Array("select * from " & Range(vLPT_SpecPage & "!B2").Text & _
            "." & Range(vLPT_SpecPage & "!B3").Text)
            .CreatePivotTable TableDestination:=Range(vLPT_SpecPage & "!B5").Text & _
            "!" & Range(vLPT_SpecPage & "!B6").Text, _
            TableName:=Range(vLPT_SpecPage & "!B4").Text, _
            DefaultVersion:=xlPivotTableVersion10
        End With
    Else
        ActiveWorkbook.Worksheets(Range(vLPT_SpecPage & "!B2").Text). _
            PivotTables(Range(vLPT_SpecPage & "!B3").Text).PivotCache. _
            CreatePivotTable TableDestination:=Range(vLPT_SpecPage & "!B5").Text & _
            "!" & Range(vLPT_SpecPage & "!B6").Text, _
            TableName:=Range(vLPT_SpecPage & "!B4").Text, _
            DefaultVersion:=xlPivotTableVersion10
    End If
    
    Sheets(Range(vLPT_SpecPage & "!B5").Text).Activate
    
    For Each x In Range(vLPT_SpecPage & "!A12:A100").Cells

        On Error Resume Next
        
        If x.Text <> "" Then
            y = x.Offset(0, 1).Text
            Sheets(Range(vLPT_SpecPage & "!B5").Text).PivotTables(Range(vLPT_SpecPage & "!B4").Text).CalculatedFields.Add _
            x.Text, x.Offset(0, 1).Text, True
        End If
        
    Next
   
   
   

   
    Dim dataonrow, dataoncolumns As Integer
    dataonrow = 0
    dataoncolumn = 0
   
   
    Dim V As Variant
    For Each x In Range(vLPT_SpecPage & "!B10:AZ10")
    If x.Text <> "" Then
        Set V = ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).AddDataField(ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).PivotFields(x.Text), "Sum of " & x.Text, xlSum)
        If _
            (Len(x.Text) > 3 And Left(x.Text, 3) = "Avg") Or _
            (Len(x.Text) > 3 And Left(x.Text, 3) = "Ave") Or _
            x.Text = "n" Or x.Text = "N" Or _
            x.Text = "Count" Or x.Text = "count" Then
            V.NumberFormat = "#,###"
            
        End If
        If Len(x.Text) > 2 And (Left(x.Text, 2) = "st" Or Left(x.Text, 2) = "ft" Or Left(x.Text, 6) = "atCred") Then
            V.NumberFormat = "#,###,"
        End If
    End If
    Next
   
    For Each x In Range(vLPT_SpecPage & "!B7:Z7")
    If x.Text <> "" Then
        With ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).PivotFields(x.Text)
            .Orientation = xlPageField
            .ShowAllItems = True
            .Subtotals = Array(False, False, False, False, False, False, False, False, False, False, False, False)
        End With
    End If
    Next
        
    k = 0
    For Each x In Range(vLPT_SpecPage & "!B8:Z8")
    If x.Text <> "" Then
    k = k + 1
    If x.Text = "Data" Then
        dataonrow = k
    Else
        With ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).PivotFields(x.Text)
            .Orientation = xlRowField
            .Subtotals = Array(False, False, False, False, False, False, False, False, False, False, False, False)
            .ShowAllItems = True
        End With
    End If
    End If
    Next
        
    k = 0
    For Each x In Range(vLPT_SpecPage & "!B9:Z9")
    If x.Text <> "" Then
    k = k + 1
    If x.Text = "Data" Then
        dataoncolumn = k
    Else
        With ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).PivotFields(x.Text)
            .Orientation = xlColumnField
            .Subtotals = Array(False, False, False, False, False, False, False, False, False, False, False, False)
            .ShowAllItems = True
        End With
    End If
    End If
    Next
        
        
    If dataonrow > 0 Then
        With ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).DataPivotField
            .Orientation = xlRowField
            .Position = dataonrow
        End With
    ElseIf dataoncolumn > 0 Then
        With ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).DataPivotField
            .Orientation = xlColumnField
            .Position = dataoncolumn
        End With
    End If
    
    
    ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).HasAutoFormat = False
    ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).PivotSelect "", xlDataAndLabel, True
    With Selection.Font
        .Name = "Courier New"
        .FontStyle = "Regular"
        .Size = 10
        .Strikethrough = False
        .Superscript = False
        .Subscript = False
        .OutlineFont = False
        .Shadow = False
        .Underline = xlUnderlineStyleNone
        .ColorIndex = xlAutomatic
    End With

    Cells.Select
    Cells.EntireColumn.AutoFit
    
    Range("A1").Select
    

        
End Sub
