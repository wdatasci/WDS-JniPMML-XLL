Attribute VB_Name = "WDSUtilPivotTable"
'''Wypasek Data Science, Inc., Copyright 2019
'''Author: Christian Wypasek
'''
'''MIT License
'''
'''Copyright (c) 2019 Wypasek Data Science, Inc. (WDataSci, WDS)
'''
'''Permission is hereby granted, free of charge, to any person obtaining a copy
'''of this software and associated documentation files (the "Software"), to deal
'''in the Software without restriction, including without limitation the rights
'''to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'''copies of the Software, and to permit persons to whom the Software is
'''furnished to do so, subject to the following conditions:
'''
'''The above copyright notice and this permission notice shall be included in all
'''copies or substantial portions of the Software.
'''
'''THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
'''IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
'''FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
'''AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
'''LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
'''OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
'''SOFTWARE.

Dim vLPT_SpecPage As String
Sub nnn_LoadPivotTableNotes()
    notes = "WDS VBACode to load data into a PivotTable " & Chr(10) & Chr(10) & _
        "The basic premise is a ""Spec"" page to script out the creation/recreation of a pivottable through an ODBC connection." & _
        Chr(10) & Chr(10) & _
        "Use aabLoadPivotTableODBCSpecProtoType as a startup example."
    MsgBox (notes)
End Sub

Sub pvt_LoadPivotTableODBCSpecProtoType()
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
    Range("A11").Value = "Where Clause"
    Range("B11", "E11").Merge
    Range("A12").Value = "CalculatedField"
    Range("B12").Value = "Formula"
    Range("A1:A12").Cells.Font.Bold = True
    Range("A12:B12").Cells.Font.Bold = True
    Range("A12:B12").Cells.Font.Underline = True

    ActiveSheet.Cells.EntireColumn.AutoFit
   
End Sub
Sub xpvt_LoadPivotTableODBCSpec()
    
    vLPT_SpecPage = "PivotTableODBCSpec"
    
    xxx_LoadPivotTableODBCSpecSub
    
End Sub
Sub pvt_LoadPivotTableODBCSpecFromThisPage()
    
    vLPT_SpecPage = ActiveSheet.Name
    
    xxx_LoadPivotTableODBCSpecSub
    
End Sub
Sub pvt_LoadPivotTableODBCSpecResetAllToSameSource()

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

Private Function bStartsWith(ByVal arg As String, arg2 As String)

    bStartsWith = False
    Dim l1, l2 As Integer
    l1 = Len(arg)
    l2 = Len(arg2)
    
    If l1 < l2 Then
        Exit Function
    ElseIf LCase(Left(arg, l2)) = LCase(arg2) Then
        bStartsWith = True
    End If

End Function

Private Function CheckFor(ByVal arg As String, arg2 As Variant)

Dim larg As String

larg = arg

For Each x In arg2
    If InStr(arg, x) > 0 Then
        CheckFor = True
        Exit Function
    End If
Next x

CheckFor = False


End Function

Private Sub xxx_LoadPivotTableODBCSpecSub()
    
    Dim s, xs As String
    
    Dim r As Range
    Set r = Selection
    Dim c, d As Range
    
    Dim tws, nws As Worksheet
    WDSCore.ActivateOrAddSheet (vLPT_SpecPage)
    
    Set tws = ActiveSheet
    
    Dim con As ADODB.Connection
    Dim rs As ADODB.Recordset
    
    If Not bIn(Range(vLPT_SpecPage & "!B1").Text, "Existing", "Range") Then
    
    Set con = New ADODB.Connection
    Set rs = New ADODB.Recordset
    
    Call xsql_ProcessODBCConnectionString
    con.Open ConnectionString:=[ODBCConnectionString]

    End If

    calcprior = Application.Calculation
    On Error GoTo CatchIt
    Application.Calculation = xlCalculationManual
    
    Dim rPages, rColumnFields, rDataFields, rRowFields, rWhereClause, rCalculatedFields As Range
    Dim rowPages, rowColumnFields, rowDataFields, rowRowFields, rowWhereClause, rowCalculatedFields As Integer
    
    Dim i As Integer
    
    i = 7
    While Not IsEmpty(tws.Cells(i, 1))
        s = tws.Cells(i, 1).Text
        If bStartsWith(s, "Page") Then
            Set rPages = tws.Cells(i, 1)
            rowPages = i
        ElseIf bStartsWith(s, "Column") Then
            Set rColumnFields = tws.Cells(i, 1)
            rowColumnFields = i
        ElseIf bStartsWith(s, "RowF") Then
            Set rRowFields = tws.Cells(i, 1)
            rowRowFields = i
        ElseIf bStartsWith(s, "DataF") Then
            Set rDataFields = tws.Cells(i, 1)
            rowDataFields = i
        ElseIf bStartsWith(s, "where") Then
            Set rWhereClause = tws.Cells(i, 1)
            rowWhereClause = i
        ElseIf bStartsWith(s, "Calculated") Then
            Set rCalculatedFields = tws.Cells(i, 1)
            rowCalculatedFields = i
        End If
        i = i + 1
    Wend

    
    WDSCore.ActivateOrAddSheet (tws.Cells(5, 2).Text)
    Set nws = ActiveSheet
    ActiveSheet.Cells.Clear
    
    cs = "ODBC;" & _
        "DSN=" & Range(vLPT_SpecPage & "!B1").Text & ";" & _
        "DATABASE=" & Range(vLPT_SpecPage & "!B2").Text & ";" & _
        "SERVER=" & Range(vLPT_SpecPage & "!F1").Text & ";" & _
        "PORT=" & Range(vLPT_SpecPage & "!F2").Text & ";" & _
        "UID=" & thisUID & ";" & _
        "PWD=" & thisUID & ";" & _
        "OPTION=0"

    Dim pvt As PivotTable


    If Not bIn(Range(vLPT_SpecPage & "!B1").Text, "Existing", "Range") Then
        
        Dim q1, q2, q3, q4 As String
        Dim lrng As Range
        q1 = ""
        q2 = ""
        q3 = ""
        q4 = ""
        i = 0
        For Each g In Array(rowPages, rowColumnFields, rowRowFields)
            Set lrng = tws.Rows(g)
            For Each x In lrng.Cells
                If IsEmpty(x) Then GoTo BreakNxt0
                If Not bIn(x.Text, "Pages", "RowFields", "ColumnFields", "Data") Then
                    i = i + 1
                    If i > 1 Then
                        q1 = q1 & ","
                        q2 = q2 & ","
                        q3 = q3 & ","
                    End If
                    If InStr(x.Text, " as ") > 0 Then
                        xs1 = Split(x.Text, " as ")
                        xs = xs1(UBound(xs1))
                    Else
                        xs = x.Text
                    End If
                    q1 = q1 & x.Text
                    q2 = q2 & i
                    q3 = q3 & i
                End If
            Next x
BreakNxt0:
        Next g
        
        i = 0
        For Each g In Array(rowDataFields)
            Set lrng = tws.Rows(g)
            For Each x In lrng.Cells
                If IsEmpty(x) Then GoTo BreakNxt00
                If Not bIn(x.Text, "Pages", "RowFields", "ColumnFields", "Data", "DataFields") Then
                    If InStr(x.Text, " as ") > 0 Then
                        q4 = q4 & "," & x.Text
                    End If
                End If
            Next x
BreakNxt00:
        Next g
        
        
        
        Dim q As String
        q = "SELECT column_name from v_catalog.columns where table_schema='" & Range(vLPT_SpecPage & "!B2").Text & "' and table_name='" & Range(vLPT_SpecPage & "!B3").Text & "' order by ordinal_position"
        rs.Open q, ActiveConnection:=con
        n = rs.RecordCount
        If n < 1 Then
            rs.Close
            q = "SELECT column_name from v_catalog.view_columns where table_schema='" & Range(vLPT_SpecPage & "!B2").Text & "' and table_name='" & Range(vLPT_SpecPage & "!B3").Text & "' order by ordinal_position"
            rs.Open q, ActiveConnection:=con
            n = rs.RecordCount
        End If
            
        Do Until rs.EOF
            For Each fld In rs.Fields
                If Left(fld.Value, 1) = "_" Then
                    q4 = q4 & ", sum(" & fld.Value & ") as " & fld.Value
                End If
            Next fld
            rs.MoveNext
        Loop
        rs.Close

        Dim wc As String
        If Not rWhereClause Is Nothing Then
            wc = " " & rWhereClause.Offset(0, 1).Text & " "
        Else
            wc = ""
        End If
        
        With ActiveWorkbook.PivotCaches.Add(SourceType:=xlExternal)
            .Connection = "ODBC;" & [ODBCConnectionString]
            .CommandType = xlCmdSql
            .CommandText = "select " & q1 & q4 & " from " & Range(vLPT_SpecPage & "!B2").Text & "." & Range(vLPT_SpecPage & "!B3").Text & wc & " group by " & q2
            .CreatePivotTable TableDestination:=Range(vLPT_SpecPage & "!B5").Text & "!" & Range(vLPT_SpecPage & "!B6").Text, TableName:=Range(vLPT_SpecPage & "!B4").Text, DefaultVersion:=xlPivotTableVersion10
        End With
    
    ElseIf Range(vLPT_SpecPage & "!B1").Text = "Range" Then

        ActiveWorkbook.PivotCaches.Create(SourceType:=xlDatabase, SourceData:=Range(vLPT_SpecPage & "!B3").Text, _
            Version:=6).CreatePivotTable Range(vLPT_SpecPage & "!B5").Text & "!" & Range(vLPT_SpecPage & "!B6").Text, _
            TableName:=Range(vLPT_SpecPage & "!B4").Text, DefaultVersion:=6
        With ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text)
  '          .ColumnGrand = True
            .HasAutoFormat = True
            .DisplayErrorString = False
            .DisplayNullString = True
            .EnableDrilldown = True
            .ErrorString = ""
            .MergeLabels = False
            .NullString = ""
            .PageFieldOrder = 2
            .PageFieldWrapCount = 0
            .PreserveFormatting = True
 '           .RowGrand = True
            .SaveData = True
            .PrintTitles = False
            .RepeatItemsOnEachPrintedPage = True
            .TotalsAnnotation = False
            .CompactRowIndent = 1
            .InGridDropZones = False
            .DisplayFieldCaptions = True
            .DisplayMemberPropertyTooltips = False
            .DisplayContextTooltips = True
            .ShowDrillIndicators = True
            .PrintDrillIndicators = False
            .AllowMultipleFilters = False
            .SortUsingCustomLists = True
            .FieldListSortAscending = False
            .ShowValuesRow = False
            .CalculatedMembersInFilters = False
'            .RowAxisLayout xlCompactRow
        
        .ColumnGrand = False
        .RowGrand = False
        .InGridDropZones = True
        .RowAxisLayout xlTabularRow
        
        End With

    Else

        ActiveWorkbook.PivotCaches.Create(SourceType:=xlDatabase, SourceData:=Range(vLPT_SpecPage & "!B3").Text, _
            Version:=6).CreatePivotTable Range(vLPT_SpecPage & "!B5").Text & "!" & Range(vLPT_SpecPage & "!B6").Text, _
            TableName:=Range(vLPT_SpecPage & "!B4").Text, DefaultVersion:=6
        With ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text)
  '          .ColumnGrand = True
            .HasAutoFormat = True
            .DisplayErrorString = False
            .DisplayNullString = True
            .EnableDrilldown = True
            .ErrorString = ""
            .MergeLabels = False
            .NullString = ""
            .PageFieldOrder = 2
            .PageFieldWrapCount = 0
            .PreserveFormatting = True
 '           .RowGrand = True
            .SaveData = True
            .PrintTitles = False
            .RepeatItemsOnEachPrintedPage = True
            .TotalsAnnotation = False
            .CompactRowIndent = 1
            .InGridDropZones = False
            .DisplayFieldCaptions = True
            .DisplayMemberPropertyTooltips = False
            .DisplayContextTooltips = True
            .ShowDrillIndicators = True
            .PrintDrillIndicators = False
            .AllowMultipleFilters = False
            .SortUsingCustomLists = True
            .FieldListSortAscending = False
            .ShowValuesRow = False
            .CalculatedMembersInFilters = False
'            .RowAxisLayout xlCompactRow
        
        .ColumnGrand = False
        .RowGrand = False
        .InGridDropZones = True
        .RowAxisLayout xlTabularRow
        
        End With


    End If
    
    Sheets(Range(vLPT_SpecPage & "!B5").Text).Activate
    
    For Each x In Range(rCalculatedFields.Offset(1, 0), rCalculatedFields.Offset(1000, 0)).Cells

        If IsEmpty(x) Then GoTo BreakNxt_x
        On Error Resume Next
        
        If x.Text <> "" Then
            y = x.Offset(0, 1).Text
            Sheets(Range(vLPT_SpecPage & "!B5").Text).PivotTables(Range(vLPT_SpecPage & "!B4").Text).CalculatedFields.Add _
            x.Text, x.Offset(0, 1).Text, True
        End If
        
    Next
BreakNxt_x:
   
   
   

   
    Dim dataonrow, dataoncolumns As Integer
    dataonrow = 0
    dataoncolumn = 0
   
   
   
    Dim V As Variant
    For Each x In Range(rDataFields.Offset(0, 1), rDataFields.Offset(0, 100))
    If IsEmpty(x) Then GoTo BreakNxt1
    If x.Text <> "DataFields" Then
    
        If InStr(x.Text, " as ") > 0 Then
            xs1 = Split(x.Text, " as ")
            xs = xs1(UBound(xs1))
        Else
            xs = x.Text
        End If
                    
        If Left(xs, 1) = "_" Then
            Set V = ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).AddDataField(ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).PivotFields(xs), Mid(xs, 2), xlSum)
        Else
            Set V = ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).AddDataField(ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).PivotFields(xs), "Sum of " & xs, xlSum)
        End If
        
        If _
            (Len(xs) > 3 And Left(xs, 3) = "Avg") Or _
            (Len(xs) > 3 And Left(xs, 3) = "Ave") Or _
            xs = "_N" Or _
            xs = "Count" Or xs = "count" Then
            V.NumberFormat = "#,###"
        End If
        If InStr(xs, "Amt") > 0 Or InStr(xs, "Bal") > 0 Then
            V.NumberFormat = "#,###,"
        End If
        If CheckFor(xs, Array("WAM", "WACS", "WALA")) Then
            V.NumberFormat = "0.0"
        ElseIf CheckFor(xs, Array("CDR", "CPR", "WAC", "Rate")) Then
            V.NumberFormat = "0.0%"
        End If
    End If
    Next
BreakNxt1:
   
    For Each x In Range(rPages.Offset(0, 1), rPages.Offset(0, 100))
    If IsEmpty(x) Then GoTo BreakNxt2
    If x.Text <> "Pages" Then
        If InStr(x.Text, " as ") > 0 Then
            xs1 = Split(x.Text, " as ")
            xs = xs1(UBound(xs1))
        Else
            xs = x.Text
        End If
        With ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).PivotFields(xs)
            .Orientation = xlPageField
            '.ShowAllItems = True
            .Subtotals = Array(False, False, False, False, False, False, False, False, False, False, False, False)
        End With
    End If
    Next
BreakNxt2:
        
    k = 0
    For Each x In Range(rColumnFields.Offset(0, 1), rColumnFields.Offset(0, 100))
    If IsEmpty(x) Then GoTo BreakNxt4
    If x.Text <> "ColumnFields" Then
    k = k + 1
    If x.Text = "Data" Then
        dataoncolumn = k
    Else
        If InStr(x.Text, " as ") > 0 Then
            xs1 = Split(x.Text, " as ")
            xs = xs1(UBound(xs1))
        Else
            xs = x.Text
        End If
        With ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).PivotFields(xs)
            .Orientation = xlColumnField
            .Subtotals = Array(False, False, False, False, False, False, False, False, False, False, False, False)
            '.ShowAllItems = True
        End With
    End If
    End If
    Next
BreakNxt4:
        
    If dataoncolumn > 0 Then
        With ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).DataPivotField
            .Orientation = xlColumnField
            .Position = dataoncolumn
        End With
    End If
    
    k = 0
    For Each x In Range(rRowFields.Offset(0, 1), rRowFields.Offset(0, 100))
    If IsEmpty(x) Then GoTo BreakNxt3
    If x.Text <> "RowFields" Then
    k = k + 1
    If x.Text = "Data" Then
        dataonrow = k
    Else
        
        If InStr(x.Text, " as ") > 0 Then
            xs1 = Split(x.Text, " as ")
            xs = xs1(UBound(xs1))
        Else
            xs = x.Text
        End If

        With ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).PivotFields(xs)
            .Orientation = xlRowField
            .ShowAllItems = False
            .Subtotals = Array(False, False, False, False, False, False, False, False, False, False, False, False)
            '.ShowAllItems = True
        End With
    End If
    End If
    Next
BreakNxt3:
        
        
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
    ActiveSheet.PivotTables(Range(vLPT_SpecPage & "!B4").Text).InGridDropZones = True
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
    
CatchIt:

ElseIt:
    Application.Calculation = calcprior
    

        
End Sub

