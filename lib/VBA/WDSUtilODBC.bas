Attribute VB_Name = "WDSUtilODBC"
'''Wypasek Data Science, Inc., Copyright 2019, 2020, 2021, 2022, 2023
'''Author: Christian Wypasek
'''
'''MIT License
'''
'''Copyright (c) 2019, 2020, 2021, 2022, 2023 Wypasek Data Science, Inc. (WDataSci, WDS)
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

Option Base 1

Sub xsql_SetUpODBCNamedRanges()

    Dim tws, nws As Worksheet
    Set tws = ActiveSheet
    If False Then 'fIsASheetName("ODBCParameters", tws.Cells(1, 1)) Then
        MsgBox ("ODBCParameters Worksheet already exists, running baa_SetUpODBCNamedRanges my overwrite other data!")
    Else
        Call ActivateOrAddSheet("ODBCParameters")
        Set nws = ActiveSheet
        i = 5
        nws.Cells(i, 1) = "ODBC Parameters (fill only as needed)"
        Range(nws.Cells(i, 1), nws.Cells(i, 2)).Merge (True)
        i = i + 1
        nws.Cells(i, 1) = "ConnectionString"
        nws.Parent.Names.Add Name:="ODBC" & nws.Cells(i, 1).Value, RefersTo:=nws.Cells(i, 2)
        i = i + 1
        nws.Cells(i, 1) = "Source"
        nws.Cells(i, 2) = "ODBC"
        nws.Parent.Names.Add Name:="ODBC" & nws.Cells(i, 1).Value, RefersTo:=nws.Cells(i, 2)
        i = i + 1
        nws.Cells(i, 1) = "DSN"
        nws.Parent.Names.Add Name:="ODBC" & nws.Cells(i, 1).Value, RefersTo:=nws.Cells(i, 2)
        i = i + 1
        nws.Cells(i, 1) = "Driver"
        nws.Parent.Names.Add Name:="ODBC" & nws.Cells(i, 1).Value, RefersTo:=nws.Cells(i, 2)
        i = i + 1
        nws.Cells(i, 1) = "UserID"
        nws.Parent.Names.Add Name:="ODBC" & nws.Cells(i, 1).Value, RefersTo:=nws.Cells(i, 2)
        i = i + 1
        nws.Cells(i, 1) = "UserName"
        nws.Parent.Names.Add Name:="ODBC" & nws.Cells(i, 1).Value, RefersTo:=nws.Cells(i, 2)
        i = i + 1
        nws.Cells(i, 1) = "Password"
        nws.Parent.Names.Add Name:="ODBC" & nws.Cells(i, 1).Value, RefersTo:=nws.Cells(i, 2)
        i = i + 1
        nws.Cells(i, 1) = "Additional"
        nws.Parent.Names.Add Name:="ODBC" & nws.Cells(i, 1).Value, RefersTo:=nws.Cells(i, 2)
    End If
    
End Sub

Sub xsql_ProcessODBCConnectionString()

TryIt:
    On Error GoTo CatchIt
    Dim s, ls As String
    If IsEmpty([ODBCConnectionString]) Then
            s = [ODBCSource]
            If s <> "" Then
                ss = s & ";"
            Else
                ss = ""
            End If
            s = [ODBCDSN]
            If s <> "" Then
                ss = ss & "DSN=" & s & ";"
            End If
            s = [ODBCDriver]
            If s <> "" Then
                ss = ss & "Driver={" & s & "};"
            End If
            s = [ODBCUserName]
            If s <> "" Then
                ss = ss & "USER=" & s & ";"
            End If
            s = [ODBCUserID]
            If s <> "" Then
                ss = ss & "USER ID=" & s & ";"
            End If
            s = [ODBCPassword]
            If s <> "" Then
                ss = ss & "PASSWORD=" & s & ";"
            End If
            s = [ODBCAdditional]
            If s <> "" Then
                ss = ss & s & ";"
            End If
            [ODBCConnectionString] = ss
    End If
    GoTo ElseIt
CatchIt:
    MsgBox ("Error in saa_ProcessODBCConnectionString")

ElseIt:

End Sub

Sub sql_QueryResultsBelow()
    
    Dim r As Range
    Set r = Selection
    Set r = r.Cells(1, 1)
    Dim rValue As String
    rValue = r.Value
    Dim out As Range
    Set out = r.Cells(1, 1).Offset(1, 0)
    On Error Resume Next
        Dim nm As String
        nm = out.ListObject.Name
        If Len(nm) > 0 Then
            Dim r2 As Range
            Set r2 = out.ListObject.Range
            out.ListObject.Delete
            r2.Clear
        End If
    On Error GoTo 0
    
    Dim con As ADODB.Connection
    Dim rs As ADODB.Recordset
    
    Set con = New ADODB.Connection
    Set rs = New ADODB.Recordset
    
    Call xsql_ProcessODBCConnectionString
    Dim s As String
    s = [ODBCConnectionString]
    If InStr(s, "ASK") > 0 Then
        answer = InputBox("Requesting ASK for " & s)
        s = Replace(s, "ASK", answer)
    End If
    con.Open ConnectionString:=s
    
    
    Dim fld As ADODB.Field
    
    calcprior = Application.Calculation
    screenprior = Application.ScreenUpdating
    On Error GoTo CatchIt
    Application.Calculation = xlCalculationManual
    Application.ScreenUpdating = False
    rs.Open rValue, ActiveConnection:=con
    j = -1
    For Each fld In rs.Fields
        j = j + 1
        out.Offset(0, j) = fld.Name
    Next
    j = rs.Fields.Count
    jj = out.Offset(1, 0).CopyFromRecordset(rs)
    
    If (Not IsEmpty(out.Offset(0, 1))) And (Not IsEmpty(out.Offset(1, 0))) Then
        Set out = Range(out, out.Offset(jj, j - 1))
        out.Parent.ListObjects.Add(xlSrcRange, out, , xlYes).Name = "Table" & (ActiveSheet.ListObjects.Count + 1)
    End If
    
    rs.Close
    con.Close
    
CatchIt:
    Application.Calculation = calcprior
    Application.ScreenUpdating = screenprior

End Sub



Sub sql_TableFieldSummaries()

    Dim twb As Workbook
    Dim tws As Worksheet
    Dim r, c As Range
    Set r = Selection
    Set tws = r.Parent
    Set twb = tws.Parent
    
    
    Dim con As ADODB.Connection
    Dim rs As ADODB.Recordset
    
    Set con = New ADODB.Connection
    Set rs = New ADODB.Recordset
    
    Call xsql_ProcessODBCConnectionString
    Dim s As String
    s = [ODBCConnectionString]
    con.Open ConnectionString:=s
    
    Dim fld As ADODB.Field
    
    calcprior = Application.Calculation
    screenprior = Application.ScreenUpdating
    On Error GoTo CatchIt
    Application.Calculation = xlCalculationManual
    Application.ScreenUpdating = False
    
    i = 9
    tws.Cells(10, i + 1) = "Count"
    tws.Cells(10, i + 2) = "Count Distinct"
    tws.Cells(10, i + 3) = "Count NonMissing"
    tws.Cells(10, i + 4) = "Avg"
    tws.Cells(10, i + 5) = "Min"
    tws.Cells(10, i + 6) = "10%"
    tws.Cells(10, i + 7) = "25%"
    tws.Cells(10, i + 8) = "50%"
    tws.Cells(10, i + 9) = "75%"
    tws.Cells(10, i + 10) = "90%"
    tws.Cells(10, i + 11) = "Max"
    
    Dim whr As String
    If Not IsEmpty(tws.Cells(8, 1)) Then
        whr = tws.Cells(8, 1).Value
    Else
        whr = ""
    End If
    
    
    For Each c In r
        
        s = "select count(" & c.Value & ") as cnt"
        s = s & ",count(distinct " & c.Value & ") as cntdistinct"
        s = s & ",sum(if10(" & c.Value & " is not null)) as cntnonmissing"
        
        If InStr(c.Offset(0, 1).Value, "char") > 0 Then
        ElseIf InStr(c.Offset(0, 1).Value, "date") > 0 Then
            s = s & ",NULL as avg"
            s = s & ",min(" & c.Value & ") as mn"
            s = s & ",NULL as d1"
            s = s & ",NULL as q1"
            s = s & ",NULL as q2"
            s = s & ",NULL as q3"
            s = s & ",NULL as d9"
            s = s & ",max(" & c.Value & ") as mx"
        ElseIf InStr(c.Offset(0, 1).Value, "time") > 0 Then
            s = s & ",NULL as avg"
            s = s & ",min(" & c.Value & ") as mn"
            s = s & ",NULL as d1"
            s = s & ",NULL as q1"
            s = s & ",NULL as q2"
            s = s & ",NULL as q3"
            s = s & ",NULL as d9"
            s = s & ",max(" & c.Value & ") as mx"
        Else
            s = s & ",avg(" & c.Value & ") as avg"
            s = s & ",min(" & c.Value & ") as mn"
            s = s & ",NULL as d1"
            s = s & ",NULL as q1"
            s = s & ",NULL as q2"
            s = s & ",NULL as q3"
            s = s & ",NULL as d9"
            s = s & ",max(" & c.Value & ")  as mx"
        End If
            
        s = s & " from " & c.Parent.Cells(2, 2).Value & "." & c.Parent.Cells(3, 2).Value
        
        s = s & " " & whr
        
        
        rs.Open s, ActiveConnection:=con
        
        c.Offset(0, i - 1).CopyFromRecordset rs
        
        rs.Close
    
        If InStr(c.Offset(0, 1).Value, "char") > 0 Then
        ElseIf InStr(c.Offset(0, 1).Value, "date") > 0 Or InStr(c.Offset(0, 1).Value, "time") > 0 Then
        ElseIf InStr(c.Offset(0, 1).Value, "int") > 0 Or InStr(c.Offset(0, 1).Value, "long") > 0 Then
            s = "select distinct min(" & c.Value & ") over () as mn"
            s = s & ",percentile_disc(0.1) within group (order by " & c.Value & ") over() as decile1"
            s = s & ",percentile_disc(0.25) within group (order by " & c.Value & ") over() as quartile1"
            s = s & ",percentile_disc(0.5) within group (order by " & c.Value & ") over() as quartile2"
            s = s & ",percentile_disc(0.75) within group (order by " & c.Value & ") over() as quartile3"
            s = s & ",percentile_disc(0.9) within group (order by " & c.Value & ") over() as decile9"
            s = s & ",max(" & c.Value & ") over() as mx"
            s = s & " from " & c.Parent.Cells(2, 2).Value & "." & c.Parent.Cells(3, 2).Value
            s = s & " " & whr
            rs.Open s, ActiveConnection:=con
            c.Offset(0, i - 1 + 4).CopyFromRecordset rs
            rs.Close
        Else
            s = "select distinct min(" & c.Value & ") over () as mx"
            s = s & ",percentile_cont(0.1) within group (order by " & c.Value & ") over () as decile1"
            s = s & ",percentile_cont(0.25) within group (order by " & c.Value & ") over () as quartile1"
            s = s & ",percentile_cont(0.5) within group (order by " & c.Value & ") over() as quartile2"
            s = s & ",percentile_cont(0.75) within group (order by " & c.Value & ") over()  as quartile3"
            s = s & ",percentile_cont(0.9) within group (order by " & c.Value & ") over() as decile9"
            s = s & ",max(" & c.Value & ") over() as mx"
            s = s & " from " & c.Parent.Cells(2, 2).Value & "." & c.Parent.Cells(3, 2).Value
            s = s & " " & whr
            rs.Open s, ActiveConnection:=con
            c.Offset(0, i - 1 + 4).CopyFromRecordset rs
            rs.Close
        End If
    
    
    
    
    
        Range(c.Offset(0, i - 1), c.Offset(0, i + 11)).NumberFormat = "#,###,###,###,##0"
        
        If InStr(c.Offset(0, 1).Value, "char") > 0 Then
            
            s = "select x,n from (select " _
                    & c.Value & " as x, count(*) as n " _
                    & " from " & c.Parent.Cells(2, 2).Value & "." & c.Parent.Cells(3, 2).Value _
                    & " " & whr _
                    & " group by " & c.Value _
                    & ") a order by n desc limit 10"
            rs.Open s, ActiveConnection:=con
            
            ii = 1
            Do Until rs.EOF
                If ii < 24 Then
                    For Each fld In rs.Fields
                        ii = ii + 1
                        c.Offset(0, i + ii) = Trim(fld.Value)
                        If IsEmpty(c.Offset(0, i + ii)) Then
                            c.Offset(0, i) = "<empty>"
                        End If
                    Next fld
                End If
                rs.MoveNext
            Loop
            
            rs.Close
        
        End If
        If InStr(c.Offset(0, 1).Value, "date") > 0 Then
            Range(c.Offset(0, i + 3), c.Offset(0, i + 12)).NumberFormat = "yyyy-mm-dd"
        End If
        If InStr(c.Offset(0, 1).Value, "datetime") > 0 Then
            Range(c.Offset(0, i + 3), c.Offset(0, i + 12)).NumberFormat = "yyyy-mm-dd HH:MM:SS"
        End If
        If InStr(c.Offset(0, 1).Value, "int") > 0 Then
            Range(c.Offset(0, i + 3), c.Offset(0, i + 12)).NumberFormat = "#,###,###,###,##0"
        End If
        If InStr(c.Offset(0, 1).Value, "float") > 0 Then
            Range(c.Offset(0, i + 3), c.Offset(0, i + 12)).NumberFormat = "#,###,###,###,##0.00"
        End If
    
    Next c
    
    
    
    rs.Close
    
CatchIt:
    con.Close
    Application.Calculation = calcprior
    Application.ScreenUpdating = screenprior


End Sub

Sub sql_VerticaSchemaTableSummary()
    Call xsql_ProcessODBCConnectionString
    Dim s As String
    s = [ODBCConnectionString]
    
    Dim lSchema As String
    lSchema = InputBox("Schema to examine:")
    
    Dim nws As Worksheet
    Call ActivateOrAddSheet(lSchema)
    Set nws = ActiveSheet
        
    nws.Range("A9").Activate
    
    Application.CutCopyMode = False
    With ActiveSheet.ListObjects.Add(SourceType:=0, Source:="ODBC;" & s, Destination:=Range("$A$9")).QueryTable
        .CommandText = Array( _
        "SELECT tables.table_schema, tables.table_name" & Chr(13) & "" & Chr(10) & "FROM v_catalog.tables tables" & Chr(13) & "" & Chr(10) & "WHERE (tables.table_schema='" & lSchema & "')" _
        )
        .RowNumbers = False
        .FillAdjacentFormulas = False
        .PreserveFormatting = True
        .RefreshOnFileOpen = False
        .BackgroundQuery = True
        .RefreshStyle = xlInsertDeleteCells
        .SavePassword = False
        .SaveData = True
        .AdjustColumnWidth = True
        .RefreshPeriod = 0
        .PreserveColumnInfo = True
        .ListObject.DisplayName = "Table_Summary_Of_" & lSchema
        .Refresh BackgroundQuery:=False
    End With

End Sub

Sub sql_MSSQLSchemaTableSummary()
    Call xsql_ProcessODBCConnectionString
    Dim s As String
    s = [ODBCConnectionString]
    If InStr(s, "ASK") > 0 Then
        answer = InputBox("Requesting ASK for " & s)
        s = Replace(s, "ASK", answer)
    End If
 
    Dim lSchema As String
    lSchema = InputBox("Schema to examine:")
    
    Dim nws As Worksheet
    Call ActivateOrAddSheet(lSchema)
    Set nws = ActiveSheet
        
    nws.Range("A9").Activate
    
    Application.CutCopyMode = False
    With ActiveSheet.ListObjects.Add(SourceType:=0, Source:=s, Destination:=Range("$A$9")).QueryTable
        .CommandText = Array( _
        "SELECT tables.table_schema, tables.table_name" & Chr(13) & "" & Chr(10) & "FROM v_catalog.tables tables" & Chr(13) & "" & Chr(10) & "WHERE (tables.table_schema='" & lSchema & "')" _
        )
        .RowNumbers = False
        .FillAdjacentFormulas = False
        .PreserveFormatting = True
        .RefreshOnFileOpen = False
        .BackgroundQuery = True
        .RefreshStyle = xlInsertDeleteCells
        .SavePassword = False
        .SaveData = True
        .AdjustColumnWidth = True
        .RefreshPeriod = 0
        .PreserveColumnInfo = True
        .ListObject.DisplayName = "Table_Summary_Of_" & lSchema
        .Refresh BackgroundQuery:=False
    End With

End Sub

Sub sql_ColumnsFromSelectedTables()

    Dim r As Range
    Set r = Selection
    Dim c, d As Range
    
    Dim con As ADODB.Connection
    Dim rs As ADODB.Recordset
    
    Set con = New ADODB.Connection
    Set rs = New ADODB.Recordset
    
    Call xsql_ProcessODBCConnectionString
    Dim s As String
    s = [ODBCConnectionString]
    con.Open ConnectionString:=s
    
    Dim fld As ADODB.Field
    
    For Each c In r
        rs.Open "select count(*) as n from " & c.Offset(0, -1).Value & "." & c.Value, ActiveConnection:=con
        Set fld = rs.Fields(0)
        c.Offset(0, 2) = fld.Value
        rs.Close
        rs.Open "select column_name from v_catalog.columns where table_name='" & c.Value & "' order by ordinal_position", ActiveConnection:=con
        n = rs.RecordCount
        i = 2
        Do Until rs.EOF
            i = i + 1
            For Each fld In rs.Fields
                c.Offset(0, i) = fld.Value
            Next fld
            rs.MoveNext
        Loop
        rs.Close
    Next c


End Sub

Sub sql_TableSummariesForSelected()

    Dim r As Range
    Set r = Selection
    Dim c, d As Range
    
    Dim con As ADODB.Connection
    Dim rs As ADODB.Recordset
    
    Set con = New ADODB.Connection
    Set rs = New ADODB.Recordset
    
    Call xsql_ProcessODBCConnectionString
    Dim s As String
    s = [ODBCConnectionString]
    con.Open ConnectionString:=s
    
    Dim fld As ADODB.Field
    
    Dim twsn As String
    twsn = ActiveSheet.Name
    
    Dim nws As Worksheet
    
    For Each c In r
    
        
        Call ActivateOrAddSheet(twsn & "." & c.Value, ActiveWorkbook.Sheets.Count, 0)
        Set nws = ActiveSheet
        
        nws.Cells(1, 1) = "Table Summary"
        nws.Cells(2, 1) = "Schema"
        nws.Cells(2, 2) = twsn
        nws.Cells(3, 1) = "Table"
        nws.Cells(3, 2) = c.Value
        nws.Cells(4, 1) = "NRows"
        
        rs.Open "select count(*) as n from " & c.Offset(0, -1).Value & "." & c.Value, ActiveConnection:=con
        Set fld = rs.Fields(0)
        nws.Cells(4, 2) = fld.Value
        rs.Close
        

        
        Dim q As String
        q = "SELECT ordinal_position, column_name, data_type, data_type_length, character_maximum_length, is_nullable, column_default from v_catalog.columns where table_schema='" & twsn & "' and table_name='" & c.Value & "' order by ordinal_position"
            
        nws.Cells(5, 1) = "Column Query"
        Range(nws.Cells(6, 1), nws.Cells(6, 4)).Merge
        nws.Cells(6, 1) = q
        
        nws.Cells(7, 1) = "Optional where clause for field summaries"
        Range(nws.Cells(8, 1), nws.Cells(8, 4)).Merge
        
        nws.Cells(9, 1) = "Columns"
        
        Application.CutCopyMode = False
        With nws.ListObjects.Add(SourceType:=0, Source:="ODBC;" & s, Destination:=Range("$A$10")).QueryTable
            .CommandText = Array(q)
            .RowNumbers = False
            .FillAdjacentFormulas = False
            .PreserveFormatting = True
            .RefreshOnFileOpen = False
            .BackgroundQuery = True
            .RefreshStyle = xlInsertDeleteCells
            .SavePassword = False
            .SaveData = True
            .AdjustColumnWidth = True
            .RefreshPeriod = 0
            .PreserveColumnInfo = True
            .ListObject.DisplayName = "Table_Summary_Of_" & nws.Name
            .Refresh BackgroundQuery:=False
        End With

    Next c


End Sub




