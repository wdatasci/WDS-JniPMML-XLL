Attribute VB_Name = "WDSCore"
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
Const WDSCoreContextID = 40001
Const WDSVBAModuleName = "WDSCore"

'References that workbook needs to compile VBAProject with this module
'Visual Basic For Applications
'Microsoft Excel 16.0 Object Library
'OLE Automation
'Microsoft Office 16.0 Object Library
'Microsoft Scripting Runtime



Public Sub ActivateOrAddSheet(ByVal arg1 As String, Optional indx = 0, Optional BeforeOrAfter = 1)

TryIt:

    On Error GoTo CatchIt

    Sheets(arg1).Activate

    GoTo ElseIt
CatchIt:

    If indx = 0 Then
        indx = ActiveSheet.Index
    End If

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

Private Function fSheetName_MacroOptions_Array() As Variant
    fSheetName_MacroOptions_Array = Array("fSheetName" _
    , "Returns the sheet name for a given range" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("ACellOrRange", "Point to a cell or range"), _
    Array("force", "an optional argument (for dependency chaining if necessary") _
    ) _
    )
End Function

Function fSheetName(ByRef arg1 As Range, Optional force = 0)
    fSheetName = arg1.Parent.Name
End Function

Private Function fWBName_MacroOptions_Array() As Variant
    fWBName_MacroOptions_Array = Array("fWBName" _
    , "Returns the workbook name for a given range" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("ACellOrRange", "Point to a cell or range"), _
    Array("force", "an optional argument (for dependency chaining if necessary") _
    ) _
    )
End Function

Function fWBName(ByRef arg1 As Range, Optional force = 0)
    fWBName = arg1.Worksheet.Parent.Name
End Function

Private Function fWBPath_MacroOptions_Array() As Variant
    fWBPath_MacroOptions_Array = Array("fWBPath" _
    , "Returns the workbook path for a given range" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("ACellOrRange", "Point to a cell or range"), _
    Array("force", "an optional argument (for dependency chaining if necessary") _
    ) _
    )
End Function

Function fWBPath(ByRef arg1 As Range, Optional force = 0)
    fWBPath = arg1.Worksheet.Parent.Path
End Function

Private Function fIsASheetName_MacroOptions_Array() As Variant
    fIsASheetName_MacroOptions_Array = Array("fIsASheetName" _
    , "Returns True if input string is a sheetname in the workbook referenced by the second argument" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("InputString", "String to check if it is a worksheet name"), _
    Array("ACellOrRange", "A reference cell for the target workbook, if not used, checks against the active workbook") _
    ) _
    )
End Function

Function fIsASheetName(ByVal s As String, ByRef arg As Range) As Boolean
    Dim x As Worksheet
    Dim twb As Workbook

    If arg Is Nothing Then
        Set twb = ActiveWorkbook
    Else
        Set twb = arg.Parent.Parent
    End If

    fIsASheetName = False
    For Each x In twb.Sheets
        If x.Name = s Then
            fIsASheetName = True
            Exit For
        End If
    Next x

End Function

Private Function fArray_MacroOptions_Array() As Variant
    fArray_MacroOptions_Array = Array("fArray" _
    , "Returns a single VBA array from a var-arg of inputs. Region values are taken row-wise." _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("Input1", "A value or range") _
    , Array("Input2", "A value or range") _
    ) _
    )
End Function
'simple test for up to 3 dims
Private Function zfNDims(ByRef arg As Variant) As Variant
    On Error GoTo CatchIt
TryIt:
    Dim n As Integer
    Dim rv(1 To 4) As Variant
    If TypeOf arg Is Range Then
        If arg.Areas.Count = 1 Then
            rv(1) = 2
            rv(2) = arg.Rows.Count
            rv(3) = arg.Columns.Count
        Else
            rv(1) = 3
            rv(2) = 1
            rv(3) = arg.Areas.Count
        End If
    Else
        For n = 0 To 2
            rv(1) = n
            rv(2 + n) = UBound(arg, n + 1)
        Next n
    End If
CatchIt:
    zfNDims = rv
    On Error GoTo 0
End Function
Private Function zfArrayCount(ByRef arg As Variant) As Integer
    Dim d As Variant
    d = zfNDims(arg)
    Dim n, i, j, k As Integer
    n = 0
    Select Case d(1)
        Case 0
            If IsArray(arg) Then
                n = zfArrayCount(arg)
            Else
                n = n + 1
            End If
        Case 1
            For i = LBound(arg, 1) To UBound(arg, 1)
                If IsArray(arg(i)) Then
                    n = n + zfArrayCount(arg(i))
                Else
                    n = n + 1
                End If
            Next i
        Case 2
            For i = LBound(arg, 1) To UBound(arg, 1)
                For j = LBound(arg, 2) To UBound(arg, 2)
                    If IsArray(arg(i, j)) Then
                        n = n + zfArrayCount(arg(i, j))
                    Else
                        n = n + 1
                    End If
                Next j
            Next i
        Case Else
            For i = LBound(arg, 1) To UBound(arg, 1)
                For j = LBound(arg, 2) To UBound(arg, 2)
                    For k = LBound(arg, 3) To UBound(arg, 3)
                        If IsArray(arg(i, j, k)) Then
                            n = n + zfArrayCount(arg(i, j, k))
                        Else
                            n = n + 1
                        End If
                    Next k
                Next j
            Next i
    End Select
    zfArrayCount = n
End Function

Function fArray(ParamArray arg() As Variant) As Variant
    Dim rv As Variant
    Dim n, i, j, k As Integer
    n = 0
    Dim rng As Range
    Dim cllctn As Collection
    For j = LBound(arg) To UBound(arg)
        If TypeOf arg(j) Is Range Then
            Set rng = arg(j)
            If rng.Areas.Count > 1 Then
                For k = 1 To rng.Areas.Count
                    n = n + rng.Areas.Item(k).Cells.Count
                Next k
            Else
                n = n + rng.Cells.Count
            End If
        ElseIf TypeOf arg(j) Is Collection Then
            Set cllctn = arg(j)
            n = n + cllctn.Count
        Else
            n = n + zfArrayCount(arg(j))
        End If
    Next j
    ReDim rv(1 To n) As Variant
    Dim c As Range
    Dim V As Variant
    n = 0
    For j = LBound(arg) To UBound(arg)
        If TypeOf arg(j) Is Range Then
            Set rng = arg(j)
            If rng.Areas.Count > 1 Then
                For k = 1 To rng.Areas.Count
                    For Each c In rng.Cells
                        n = n + 1
                        rv(n) = c.Value2
                    Next c
                Next k
            Else
                For Each c In rng.Cells
                    n = n + 1
                    rv(n) = c.Value2
                Next c
            End If
        ElseIf TypeOf arg(j) Is Collection Then
            Set cllctn = arg(j)
            For Each V In cllctn
                n = n + 1
                rv(n) = V
            Next V
        Else
            Dim d As Variant
            Dim jj As Integer
            d = zfNDims(arg(j))
            Select Case d(1)
                Case 0
                    n = n + 1
                    rv(n) = arg(j)
                Case 1
                    For i = LBound(arg(j), 1) To UBound(arg(j), 1)
                        n = n + 1
                        rv(n) = arg(j)(i)
                    Next i
                Case 2
                    For i = LBound(arg(j), 1) To UBound(arg(j), 1)
                        For jj = LBound(arg(j), 2) To UBound(arg(j), 2)
                            n = n + 1
                            rv(n) = arg(j)(i, jj)
                        Next jj
                    Next i
                Case Else
                    For i = LBound(arg(j), 1) To UBound(arg(j), 1)
                        For jj = LBound(arg(j), 2) To UBound(arg(j), 2)
                            For k = LBound(arg(j), 3) To UBound(arg(j), 3)
                                n = n + 1
                                rv(n) = arg(j)(i, jj, k)
                            Next k
                        Next jj
                    Next i
            End Select
        End If
    Next j
    fArray = rv
End Function
Function fArrayAsColumn(ParamArray arg() As Variant) As Variant
    Dim rv As Variant
    Dim n, i, j, k As Integer
    n = 0
    Dim rng As Range
    Dim cllctn As Collection
    For j = LBound(arg) To UBound(arg)
        If TypeOf arg(j) Is Range Then
            Set rng = arg(j)
            If rng.Areas.Count > 1 Then
                For k = 1 To rng.Areas.Count
                    n = n + rng.Areas.Item(k).Cells.Count
                Next k
            Else
                n = n + rng.Cells.Count
            End If
        ElseIf TypeOf arg(j) Is Collection Then
            Set cllctn = arg(j)
            n = n + cllctn.Count
        Else
            n = n + zfArrayCount(arg(j))
        End If
    Next j
    ReDim rv(1 To n, 1) As Variant
    Dim c As Range
    Dim V As Variant
    n = 0
    For j = LBound(arg) To UBound(arg)
        If TypeOf arg(j) Is Range Then
            Set rng = arg(j)
            If rng.Areas.Count > 1 Then
                For k = 1 To rng.Areas.Count
                    For Each c In rng.Cells
                        n = n + 1
                        rv(n, 1) = c.Value2
                    Next c
                Next k
            Else
                For Each c In rng.Cells
                    n = n + 1
                    rv(n, 1) = c.Value2
                Next c
            End If
        ElseIf TypeOf arg(j) Is Collection Then
            Set cllctn = arg(j)
            For Each V In cllctn
                n = n + 1
                rv(n, 1) = V
            Next V
        Else
            Dim d As Variant
            Dim jj As Integer
            d = zfNDims(arg(j))
            Select Case d(1)
                Case 0
                    n = n + 1
                    rv(n, 1) = arg(j)
                Case 1
                    For i = LBound(arg(j), 1) To UBound(arg(j), 1)
                        n = n + 1
                        rv(n, 1) = arg(j)(i)
                    Next i
                Case 2
                    For i = LBound(arg(j), 1) To UBound(arg(j), 1)
                        For jj = LBound(arg(j), 2) To UBound(arg(j), 2)
                            n = n + 1
                            rv(n, 1) = arg(j)(i, jj)
                        Next jj
                    Next i
                Case Else
                    For i = LBound(arg(j), 1) To UBound(arg(j), 1)
                        For jj = LBound(arg(j), 2) To UBound(arg(j), 2)
                            For k = LBound(arg(j), 3) To UBound(arg(j), 3)
                                n = n + 1
                                rv(n, 1) = arg(j)(i, jj, k)
                            Next k
                        Next jj
                    Next i
            End Select
        End If
    Next j
    fArrayAsColumn = rv
End Function

Private Function fKeyValuePairs_MacroOptions_Array() As Variant
    fKeyValuePairs_MacroOptions_Array = Array("fKeyValuePairs" _
    , "Returns a VBA array of Scripting.Dictionary object from vararg inputs of the form {Key1 (string),Value1} or Key1,Value1,Key2,Value2, etc." _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("Input1", "Either a pair or a key followed by a value.") _
    , Array("Input2", "A value associated with an Input1 key or a {Key (string), Value} pair") _
    ) _
    )
End Function
Function fKeyValuePairs(ParamArray arg() As Variant) As Variant
    Dim args
    args = arg
    Dim d As Dictionary
    Set d = zfKeyValuePairs(args)
    Dim rv
    Dim nrows, ncols As Integer
    Dim ky
    Dim ndims
    If d.Exists("SquareUp") Then
        nrows = 0
        ncols = 1
        Dim rvndims As Dictionary
        Set rvndims = CreateObject("Scripting.Dictionary")
        For Each ky In d.Keys()
            rvndims.Add Key:=ky, Item:=zfNDims(d.Item(ky))
            ndims = rvndims.Item(ky)
            If ndims(1) = 0 Then
                nrows = nrows + 1
            ElseIf ndims(1) = 1 Then
                If ndims(2) > ncols Then: ncols = ndims(2)
                nrows = nrows + 1
            ElseIf ndims(1) = 2 Then
                If ndims(3) > ncols Then: ncols = ndims(3)
                nrows = nrows + ndims(2)
            Else
                If ndims(2) * ndims(4) > ncols Then: ncols = ndims(2) * ndims(4)
                nrows = nrows + ndims(3)
            End If
        Next ky
        ReDim rv(1 To nrows, 1 To 1 + ncols) As Variant
        nrows = 0
        Dim V
        Dim i, j, k As Integer
        For Each ky In d.Keys()
            ndims = rvndims.Item(ky)
            V = d.Item(ky)
            If ndims(1) = 0 Then
                nrows = nrows + 1
                rv(nrows, 1) = ky
                rv(nrows, 2) = V
            ElseIf ndims(1) = 1 Then
                nrows = nrows + 1
                rv(nrows, 1) = ky
                For j = 1 To ndims(2)
                    rv(nrows, 1 + j) = V(j)
                Next j
            ElseIf ndims(1) = 2 Then
                For i = 1 To ndims(2)
                    nrows = nrows + 1
                    rv(nrows, 1) = ky
                    For j = 1 To ndims(3)
                        rv(nrows, 1 + j) = V(i, j)
                    Next j
                Next i
            Else
                For i = 1 To ndims(3)
                    nrows = nrows + 1
                    rv(nrows, 1) = ky
                    ncols = 1
                    For k = 1 To ndims(2)
                        For j = 1 To ndims(4)
                            ncols = ncols + 1
                            rv(nrows, ncols) = V(i, k, j)
                        Next j
                    Next k
                Next i
            End If
        Next ky
    Else
        ReDim rv(1 To d.Count, 2) As Variant
        i = 0
        For Each ky In d.Keys()
           i = i + 1
           rv(i, 1) = ky
           rv(i, 2) = d.Item(ky)
        Next ky
    End If
    fKeyValuePairs = rv
End Function

Function zfKeyValuePairs(arg) As Dictionary
    
    Dim twb As Workbook
    Set twb = ActiveWorkbook
    
    'Dim rv As Dictionary
    Set zfKeyValuePairs = CreateObject("Scripting.Dictionary")
    
    Dim n, ncols, i, j, k, jj As Integer
    n = 0
    ncols = 2
    Dim key_value
    Dim ky
    key_value = -1
    ky = "Unk"
    
    'returning to Excel requires a squaring up
    Dim square_up As Boolean
    square_up = False
    
    Dim rng As Range
    Dim cllctn As Collection
    For j = LBound(arg) To UBound(arg)
        If TypeOf arg(j) Is Range Then
            Set rng = arg(j)
            If key_value = -1 Then
                If rng.Areas.Count > 1 Then
                    For k = 1 To rng.Areas.Count
                        If rng.Areas.Item(k).Columns.Count < 2 Then GoTo CatchIt
                        ky = rng.Areas.Item(k).Cells(1, 1).value
                        zfKeyValuePairs.Add Key:=ky, Item:=Range(rng.Areas.Item(k).Cells(1, 2), rng.Areas.Item(k).SpecialCells(xlCellTypeLastCell)).value
                    Next k
                    key_value = -1
                Else
                    If rng.Columns.Count = 1 And rng.Rows.Count = 1 Then
                        ky = rng.Cells(1, 1).value
                        key_value = 0
                    Else
                        If rng.Columns.Count < 2 Then GoTo CatchIt
                        For k = 1 To rng.Rows.Count
                            For jj = 1 To rng.Columns.Count - 1
                                If IsEmpty(rng.Cells(k, jj + 1)) Then: Exit For
                            Next jj
                            zfKeyValuePairs.Add Key:=rng.Cells(k, 1).value, Item:=Range(rng.Cells(k, 2), rng.Cells(k, jj)).value
                        Next k
                        key_value = -1
                    End If
                End If
            Else
                For jj = 1 To rng.Columns.Count - 1
                    If IsEmpty(rng.Cells(1, jj + 1)) Then: Exit For
                Next jj
                zfKeyValuePairs.Add Key:=ky, Item:=Range(rng.Cells(1, 1), rng.Cells(rng.Rows.Count, jj)).value
                key_value = -1
            End If
        ElseIf TypeOf arg(j) Is Collection Then
            If key_value = -1 Then
                For k = LBound(arg(j)) To UBound(arg(j))
                    If key_value = -1 Then
                        ky = arg(j)(k)
                        key_value = 0
                    Else
                        zfKeyValuePairs.Add Key:=ky, Item:=arg(j)(k)
                        key_value = -1
                    End If
                Next k
                If key_value <> -1 Then GoTo CatchIt
            Else
                zfKeyValuePairs.Add Key:=ky, Item:=arg(j)
            End If
            key_value = -1
        Else
            Dim d As Variant
            d = zfNDims(arg(j))
            If key_value = 0 Then
                zfKeyValuePairs.Add Key:=ky, Item:=arg(j)
                key_value = -1
            Else
                Select Case d(1)
                    Case 0
                        ky = arg(j)
                        key_value = 0
                    Case 1
                        For i = LBound(arg(j), 1) To UBound(arg(j), 1)
                            If key_value = -1 Then
                                ky = arg(j)(i)
                                key_value = 0
                            Else
                                zfKeyValuePairs.Add Key:=ky, Item:=arg(j)(i)
                                key_value = -1
                            End If
                        Next i
                    Case 2
                        For i = LBound(arg(j), 1) To UBound(arg(j), 1)
                            For jj = LBound(arg(j), 2) To UBound(arg(j), 2)
                                If key_value = -1 Then
                                    ky = arg(j)(i)
                                    key_value = 0
                                Else
                                    zfKeyValuePairs.Add Key:=ky, Item:=arg(j)(i)
                                    key_value = -1
                                End If
                            Next jj
                        Next i
                    Case Else
                        For i = LBound(arg(j), 1) To UBound(arg(j), 1)
                            For jj = LBound(arg(j), 2) To UBound(arg(j), 2)
                                For k = LBound(arg(j), 3) To UBound(arg(j), 3)
                                    If key_value = -1 Then
                                        ky = arg(j)(i)
                                        key_value = 0
                                    Else
                                        zfKeyValuePairs.Add Key:=ky, Item:=arg(j)(i)
                                        key_value = -1
                                    End If
                                Next k
                            Next jj
                        Next i
                End Select
            End If
        End If
    Next j
    If key_value = 0 Then 'ends with unfilled key-value pair
        GoTo CatchIt
    End If
    GoTo ElseIt
CatchIt:
    Err.Raise Number:=WDSCoreContextID + 1, Source:=twb.Name & WDSVBAModuleName, Description:="Error in input format, should be (key1,value1) {key1, value1} or KeyValues (a Nx2 range)"
    Exit Function
ElseIt:
End Function


Private Function fQuote_MacroOptions_Array() As Variant
    fQuote_MacroOptions_Array = Array("fQuote" _
    , "Quotes the input string" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("InputString", "String to quote") _
    ) _
    )
End Function

Function fQuote(ByVal arg As String, Optional force = 0) As String
    If Len(arg) = 0 Then
        fQuote = ""
    Else
        If InStr(arg, """") > 0 And InStr(arg, """""") = 0 Then
            arg = Replace(arg, """", """""")
        End If
        fQuote = """" & arg & """"
    End If
End Function

Private Function fColumnFromCode_MacroOptions_Array() As Variant
    fColumnFromCode_MacroOptions_Array = Array("fColumnFromCode" _
    , "Returns the A1 column from R1C1 column" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("Code", "Convert R1C1 column to A1 column") _
    ) _
    )
End Function

Function fColumnFromCode(arg As Integer) As String
    Dim x, y, z As Integer
    z = Int((arg - 1) / 676)
    y = Int((arg - 1) / 26)
    x = arg - y * 26 - z * 676
    If z > 0 Then
        fColumnFromCode = Application.WorksheetFunction.Unichar(z + 64) + Application.WorksheetFunction.Unichar(y + 64) & Application.WorksheetFunction.Unichar(x + 64)
    ElseIf y > 0 Then
        fColumnFromCode = Application.WorksheetFunction.Unichar(y + 64) & Application.WorksheetFunction.Unichar(x + 64)
    Else
        fColumnFromCode = Application.WorksheetFunction.Unichar(x + 64)
    End If
End Function

Private Function mnmx_MacroOptions_Array() As Variant
    mnmx_MacroOptions_Array = Array("mnmx" _
    , "min(arg1,max(arg2,arg3)) (use for probability capping with mnmx(1,0,X) )" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("arg1", "minimum arg"), _
        Array("arg2", "max of arg2,arg3"), _
        Array("arg3", "max of arg2,arg3") _
    ) _
    )
End Function

Function mnmx(arg1, arg2, arg3)
    mnmx = lMin(arg1, lMax(arg2, arg3))
End Function

Private Function mxmn_MacroOptions_Array() As Variant
    mxmn_MacroOptions_Array = Array("mxmn" _
    , "max(arg1,min(arg2,arg3)) (use for probability capping with mxmn(0,1,X) )" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("arg1", "maximum arg"), _
        Array("arg2", "min of arg2,arg3"), _
        Array("arg3", "min of arg2,arg3") _
    ) _
    )
End Function

Function mxmn(arg1, arg2, arg3)
    mxmn = lMax(arg1, lMin(arg2, arg3))
End Function
Function fArrayMaxWith(ByRef arg1 As Variant, arg2 As Double) As Variant
Dim rv As Variant
ReDim rv(LBound(arg1, 1) To UBound(arg1, 1), LBound(arg1, 2) To UBound(arg1, 2)) As Variant
For j = LBound(arg1, 2) To UBound(arg1, 2)
For i = LBound(arg1, 1) To UBound(arg1, 1)
    If arg1(i, j) < arg2 Then
        rv(i, j) = arg2
    Else
        rv(i, j) = arg1(i, j)
    End If
Next i
Next j
fArrayMaxWith = rv
End Function
Function fArrayMaxWithArray(ByRef arg1 As Variant, ByRef arg2 As Variant) As Variant
Dim rv As Variant
ReDim rv(LBound(arg1, 1) To UBound(arg1, 1), LBound(arg1, 2) To UBound(arg1, 2)) As Variant
For j = LBound(arg1, 2) To UBound(arg1, 2)
For i = LBound(arg1, 1) To UBound(arg1, 1)
    If arg1(i, j) < arg2(i, j) Then
        rv(i, j) = arg2(i, j)
    Else
        rv(i, j) = arg1(i, j)
    End If
Next i
Next j
fArrayMaxWithArray = rv
End Function
Function fArrayMinWith(ByRef arg1 As Variant, arg2 As Double) As Variant
Dim rv As Variant
ReDim rv(LBound(arg1, 1) To UBound(arg1, 1), LBound(arg1, 2) To UBound(arg1, 2)) As Variant
For j = LBound(arg1, 2) To UBound(arg1, 2)
For i = LBound(arg1, 1) To UBound(arg1, 1)
    If arg1(i, j) > arg2 Then
        rv(i, j) = arg2
    Else
        rv(i, j) = arg1(i, j)
    End If
Next i
Next j
fArrayMinWith = rv
End Function
Function fArrayMinWithArray(ByRef arg1 As Variant, ByRef arg2 As Variant) As Variant
Dim rv As Variant
ReDim rv(LBound(arg1, 1) To UBound(arg1, 1), LBound(arg1, 2) To UBound(arg1, 2)) As Variant
For j = LBound(arg1, 2) To UBound(arg1, 2)
For i = LBound(arg1, 1) To UBound(arg1, 1)
    If arg1(i, j) > arg2(i, j) Then
        rv(i, j) = arg2(i, j)
    Else
        rv(i, j) = arg1(i, j)
    End If
Next i
Next j
fArrayMinWithArray = rv
End Function
Private Function fCleanLimits_MacroOptions_Array() As Variant
    fCleanLimits_MacroOptions_Array = Array("fCleanLimits" _
    , "Checks argument against left and right clean limits and returns default if out of limits" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("argument", "checked value"), _
        Array("CLLeft", "Returns default if arg lt CLLeft"), _
        Array("CLRight", "Returns default if arg gt CLRight"), _
        Array("Default", "Returned value if out of limits or bad value") _
    ) _
    )
End Function

Function fCleanLimits(arg1, arg2, arg3, Optional arg4 = 0)
    If arg1 < arg2 Then
        fCleanLimits = arg4
    ElseIf arg1 > arg3 Then
        fCleanLimits = arg4
    Else
        fCleanLimits = arg1
    End If
End Function

Private Function fDate2MonthID_MacroOptions_Array() As Variant
    fDate2MonthID_MacroOptions_Array = Array("fDate2MonthID" _
    , "Returns a MonthID value from a date value" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("argument", "value as a usual Excel date") _
    ) _
    )
End Function

Function fDate2MonthID(arg As Date) As Integer
    Dim y, m As Integer
    y = Year(arg)
    m = Month(arg)
    fDate2MonthID = (y - 2000) * 12 + m

End Function

Function fDateText2Date(ByVal arg As String, Optional arg2 = "MM/DD/YYYY", Optional dlm = "/", Optional Error_Value = 0) As Date
    On Error GoTo CatchIt
    Dim y, m, d, i, common As Integer

    common = 1
    
TryIt:
    
    If arg2 <> "MM/DD/YYYY" Or dlm <> "/" Then
        If arg2 = "MM-DD-YYYY" Then
            dlm = "-"
        ElseIf arg2 = "YYYY/MM/DD" And dlm = "/" Then
            common = 2
        ElseIf arg2 = "YYYY-MM-DD" And dlm = "/" Then
            common = 2
            dlm = "-"
        ElseIf arg2 = "YYYYMMDD" Then
            common = 3
        Else
            GoTo CatchIt
        End If
    End If
    
            
    If common = 1 Then
        
        i = Val(InStr(arg, dlm))
        m = Val(Left(arg, i - 1))
        arg = Mid(arg, i + 1)
        i = Val(InStr(arg, dlm))
        d = Val(Left(arg, i - 1))
        arg = Mid(arg, i + 1)
        y = Val(arg)
        
    ElseIf common = 2 Then
    
        i = Val(InStr(arg, dlm))
        y = Val(Left(arg, i - 1))
        arg = Mid(arg, i + 1)
        m = Val(Left(arg, i - 1))
        arg = Mid(arg, i + 1)
        d = Val(arg)
        
    Else
        
        y = Val(Left(arg, 4))
        m = Val(Mid(arg, 5, 2))
        d = Val(Mid(arg, 7, 2))
        
    End If
        
    fDateText2Date = DateSerial(y, m, d)
    
    GoTo ElseIt
    
CatchIt:

    fDateText2Date = Error_Value
    
ElseIt:

End Function

Sub wds_RangeConvert_DateText2Date()

Dim r, c As Range

Set r = Selection

Dim s As String
Dim x As Date

For Each c In r

    If Excel.WorksheetFunction.IsText(c.value) Then
        s = c.value
        On Error GoTo CatchIt
        If Len(s) > 0 Then
            x = fDateText2Date(s, "MM/DD/YYYY", "/", -1)
            If x > 0 Then
                c.value = x
                c.NumberFormat = "YYYY-MM-DD"
            End If
        End If
    
CatchIt:

    End If
Next c
        
End Sub

Private Function fMonthID2Date_MacroOptions_Array() As Variant
    fMonthID2Date_MacroOptions_Array = Array("fMonthID2Date" _
    , "Returns an Excel date (first of the month) from a MonthID value" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("argument", "A MonthID value, an integer number of months since 1999-12-01") _
    ) _
    )
End Function

Function fMonthID2Date(arg As Integer) As Date
    Dim y, m As Integer
    y = Int((arg - 1) / 12)
    m = arg - 12 * y
    y = y + 2000
    fMonthID2Date = DateSerial(y, m, 1)

End Function

Function fMonthID2DateArray(ByRef arg As Range) As Variant
    Dim i, y, m As Integer
    Dim rv
    ReDim rv(1 To arg.Rows.Count, 1 To 1) As Date
    
    For i = 1 To arg.Rows.Count
        m = Int(arg(i, 1))
        y = Int((m - 1) / 12)
        m = m - 12 * y
        y = y + 2000
        rv(i, 1) = DateSerial(y, m, 1)
    Next i
    fMonthID2DateArray = rv

End Function

Private Function fMonthN2MonthID_MacroOptions_Array() As Variant
    fMonthN2MonthID_MacroOptions_Array = Array("fMonthN2MonthID" _
    , "Returns a MonthID value from a YYYYMM integer" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("argument", "A MonthN (YYYYMM) integer") _
    ) _
    )
End Function

Function fMonthN2MonthID(arg As Integer) As Long
    Dim y, m As Integer
    y = Int((arg - 1) / 100)
    m = arg - 100 * y
    fMonthN2MonthID = (y - 2000) * 12 + m

End Function


Private Function fMonthID2MonthN_MacroOptions_Array() As Variant
    fMonthID2MonthN_MacroOptions_Array = Array("fMonthID2MonthN" _
    , "Returns a YYYYMM integer from a MonthID value" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("argument", "A MonthID value, an integer number of months since 1999-12-01") _
    ) _
    )
End Function

Function fMonthID2MonthN(arg As Integer) As Long
    Dim y, m As Integer
    y = Int((arg - 1) / 12)
    m = arg - 12 * y
    y = y + 2000
    fMonthID2MonthN = y * 100 + m

End Function

Function fMonthID2MonthNArray(ByRef arg As Range) As Variant
    Dim i, y, m As Integer
    Dim rv
    ReDim rv(1 To arg.Rows.Count, 1 To 1) As Date
    
    For i = 1 To arg.Rows.Count
        m = Int(arg(i, 1))
        y = Int((m - 1) / 12)
        m = m - 12 * y
        y = y + 2000
        rv(i, 1) = y * 100 + m
    Next i
    fMonthID2MonthNArray = rv

End Function

Private Function ifNull_MacroOptions_Array() As Variant
    ifNull_MacroOptions_Array = Array("ifNull" _
    , "Returns a default value if argument is null or invalid" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("argument", "checked value"), _
        Array("Default", "Returned value if out of limits or bad value") _
    ) _
    )
End Function

Function if10(arg)
    If arg = True Then
        if10 = 1
    ElseIf arg = False Then
        if10 = 0
    ElseIf arg Then
        if10 = 1
    Else
        if10 = 0
    End If
End Function

Function ifnull(ByVal arg As Variant, ByVal arg2 As Double)
    On Error Resume Next
    If Application.WorksheetFunction.IsError(arg) Then
        ifnull = arg2
    ElseIf Not IsNumeric(arg) Then
        ifnull = arg2
    Else
        ifnull = arg
    End If
End Function

Private Function ifNullOrZero_MacroOptions_Array() As Variant
    ifNullOrZero_MacroOptions_Array = Array("ifNullOrZero" _
    , "Returns a default value if argument is null or invalid (usefull for denominator checks)" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("argument", "checked value"), _
        Array("Default", "Returned value if out of limits or bad value") _
    ) _
    )
End Function

Function ifnullorzero(ByVal arg As Variant, ByVal arg2 As Double)

    ifnullorzero = ifnull(arg, arg2)
    If ifnullorzero = 0 Then
        ifnullorzero = arg2
    End If

End Function

Private Function lMax_MacroOptions_Array() As Variant
    lMax_MacroOptions_Array = Array("lMax" _
    , "A simple bivariate max for VBA purposes" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("argument1", "checked value"), _
        Array("argument2", "checked value") _
    ) _
    )
End Function

Function lMax(ByVal a As Double, ByVal b As Double)
    If a < b Then
        lMax = b
    Else
        lMax = a
    End If
End Function

Private Function lMin_MacroOptions_Array() As Variant
    lMin_MacroOptions_Array = Array("lMin" _
    , "A simple bivariate min for VBA purposes" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("argument1", "checked value"), _
        Array("argument2", "checked value") _
    ) _
    )
End Function

Function lMin(ByVal a As Double, ByVal b As Double)
    If a < b Then
        lMin = a
    Else
        lMin = b
    End If
End Function

Private Function XmlMapSchema_MacroOptions_Array() As Variant
    XmlMapSchema_MacroOptions_Array = Array("XmlMapSchema" _
    , "Returns the first XmlSchema associated with the ListObject the argument belongs to arg.ListObject.XmlMap.Schemas.Item(1).XML" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("aRange", "A cell or range that is part of an XmlMap'd ListObject")) _
    )
End Function
Public Function XmlMapSchema(ByRef aRange As Range)
    XmlMapSchema = aRange.ListObject.XmlMap.Schemas(1).XML
End Function

Private Function FlipSumProduct_MacroOptions_Array() As Variant
    FlipSumProduct_MacroOptions_Array = Array("FlipSumProduct" _
    , "Returns a single row/element of a convolution of a distribution column vector another column vector/matrix" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("Distribution", "A column vector, first element weights the last element of second argument, second-second last, etc."), _
    Array("Argument", "A column vector Or column-oriented array to be weighted by the first argument") _
    ) _
    )
End Function

Function FlipSumProduct(ByRef r As Range, ByRef s As Range, Optional outrows = 1) As Variant

    Dim rv
    ReDim rv(outrows, 1 To s.Columns.Count)
    
    Dim i, j, k, l, n As Integer
    
    n = r.Rows.Count
    If outrows < n Then
        n = outrows
    End If
    
    For l = 1 To outrows
    If l <= s.Rows.Count Then
        For i = 1 To l
            j = l - i + 1
            For k = 1 To s.Columns.Count
                rv(l, k) = rv(l, k) + s(j, k) * r(i, 1)
            Next k
        Next i
    Else
        For i = n - s.Rows.Count + l To n
            j = l - i + 1
            For k = 1 To s.Columns.Count
                rv(l, k) = rv(l, k) + s(j, k) * r(i, 1)
            Next k
        Next i
    End If
    Next l
    
    FlipSumProduct = rv

End Function

Sub wds_CalcCells()

    Dim c As Range
    For Each c In Selection
        c.Calculate
    Next c

End Sub

Function fNVBlock(ByRef corner As Range, ByRef dep As Range, Optional nrows = 1, Optional ncols = 1) As Variant

    fNVBlock = Range(corner.Cells(1, 1), corner.Cells(1, 1).Offset(nrows - 1, ncols - 1)).value

End Function

Function fNVBlockColumn(ByRef corner As Range, Optional ncol = 1, Optional nrows = 0, Optional shift = 0, Optional strict = 0, Optional dep = 1) As Variant

    If nrows <= 0 Or nrows > corner.Rows.Count Then
        nrows = corner.Rows.Count
    End If
    If ncol <= 0 Then
        ncol = 1
    End If
    If strict And (ncol > corner.Columns.Count) Then
        ncol = corner.Columns.Count
    End If
    
    If shift = 0 Then
        fNVBlockColumn = Range(corner.Cells(1, ncol), corner.Cells(nrows, 1).Offset(0, ncol - 1)).value
    Else
        fNVBlockColumn = Range(corner.Cells(1, ncol).Offset(shift, 0), corner.Cells(nrows, 1).Offset(0, ncol - 1).Offset(shift, 0)).value
    End If

End Function

Function fNVBlockSub(ByRef corner As Range, Optional r1 = 0, Optional c1 = 0, Optional r2 = 0, Optional c2 = 0, Optional shift = 0, Optional strict = 0, Optional dep = 1) As Variant

    If r1 <= 0 Then
        r1 = 1
    End If
    If strict And (r1 >= corner.Rows.Count) Then
        r1 = corner.Rows.Count
    End If
    If c1 <= 0 Then
        c1 = 1
    End If
    If strict And (c1 >= corner.Columns.Count) Then
        c1 = corner.Columns.Count
    End If
    If r2 <= 0 Then
        r2 = corner.Rows.Count
    End If
    If strict And (r2 >= corner.Rows.Count) Then
        r2 = corner.Rows.Count
    End If
    If c2 <= 0 Then
        c2 = corner.Columns.Count
    End If
    If strict And (c2 >= corner.Columns.Count) Then
        c2 = corner.Columns.Count
    End If
    
    If r2 < r1 Then
        r3 = r1
        r1 = r2
        r2 = r3
    End If
    If c2 < c1 Then
        c3 = c1
        c1 = c2
        c2 = c3
    End If
        
    If shift = 0 Then
        fNVBlockSub = Range(corner.Cells(r1, 1).Offset(0, c1 - 1), corner.Cells(r2, 1).Offset(0, c2 - 1)).value
    Else
        fNVBlockSub = Range(corner.Cells(r1, 1).Offset(0, c1 - 1).Offset(shift, 0), corner.Cells(r1, 1).Offset(0, c2 - 1).Offset(shift, 0)).value
    End If

End Function

Function fNVAddress(ByRef r As Range, Optional nrows = -1, Optional ncols = -1)

    If nrows > 0 Then
        If ncols > 0 Then
            fNVAddress = Range(r.Cells(1, 1), r.Cells(1, 1).Offset(nrows - 1, ncols - 1)).Address(1, 1, xlA1, 1)
        Else
            fNVAddress = Range(r.Cells(1, 1), r.Cells(1, r.Columns.Count).Offset(nrows - 1, 0)).Address(1, 1, xlA1, 1)
        End If
    Else
        If ncols > 0 Then
            fNVAddress = Range(r.Cells(1, 1), r.Cells(r.Rows.Count, 1).Offset(0, ncols - 1)).Address(1, 1, xlA1, 1)
        Else
            fNVAddress = r.Address(1, 1, xlA1, 1)
        End If
    End If
    
End Function

Function fNVFormula(ByRef r As Range) As String
    
    Dim rv As String
        rv = Mid(r.Formula, 2)
    fNVFormula = rv

End Function

Function fNVIndirect(ByVal s As String, Optional offsetr = 0, Optional offsetc = 0) As Variant
    
    fNVIndirect = Range(s).Offset(offsetr, offsetc).value

End Function

Function fNVNamedRangeFormula(ByVal s As String) As String
    
    Dim rv As String
    rv = Application.Names(s).RefersTo
    If Left(rv, 1) = "=" Then
        rv = Mid(rv, 2)
    End If
    fNVNamedRangeFormula = rv

End Function

Function fNV3DLookup(col, ByRef rws As Range, ByRef shts As Range) As Variant

    Dim rv As Variant
    ReDim rv(1 To rws.Rows.Count, 1 To shts.Columns.Count) As Variant
    
    Dim ws As Worksheet
    
    For j = 1 To shts.Columns.Count
        If IsEmpty(shts.Cells(1, j)) Or (shts.Cells(1, j).value = "") Then GoTo Next_j
        Set ws = Sheets(shts.Cells(1, j).value)
        For i = 1 To rws.Rows.Count
            rv(i, j) = ws.Cells(rws.Cells(i, 1).value, col).value
        Next i
Next_j:
    Next j
    
    fNV3DLookup = rv


End Function

Function Concat_Dlm(ByRef r As Range, ByVal dlm As String) As String

    Dim c As Range
    Dim rv As String
    Dim i As Integer
    rv = ""
    i = 0
    For Each c In r
       If Not IsEmpty(c) Then
           i = i + 1
           If i > 1 Then: rv = rv & dlm
           rv = rv & c.Text
        End If
    Next c
    Concat_Dlm = rv

End Function

Function Concat_Dlm_MultiRange(dlm, ParamArray Rngs() As Variant) As String

Dim r As Range
rv = ""
i = 0
For Each c In Rngs
    i = i + 1
    Set r = c
    If i > 1 Then: rv = rv & dlm
    rv = rv & Concat_Dlm(r, dlm)
Next
Concat_Dlm_MultiRange = rv
End Function

'See WDSUtilMatrix
Private Function WDSCore_deRange(arg) As Variant
Dim rv(1 To 3) As Variant
If TypeOf arg Is Range Then
    rv(1) = arg.value
    rv(2) = arg.Rows.Count
    rv(3) = arg.Columns.Count
    If rv(2) = 1 And rv(3) = 1 Then
        rv(2) = 0
        rv(3) = 0
    End If
Else
    rv(1) = arg
    Dim d(1 To 4) As Variant
    n = zfNDims(arg)
    If n(1) = 0 Then
        rv(2) = 0
        rv(3) = 0
    ElseIf n(1) = 1 Then
        rv(2) = n(2)
        rv(3) = 0
    ElseIf n(1) = 2 Then
        rv(2) = n(2)
        rv(3) = n(3)
    End If
End If
WDSCore_deRange = rv
End Function

Function sum_across_matrix_rows(arg, Optional c1 = 0, Optional c2 = 0) As Variant

    Dim varg
    varg = WDSCore_deRange(arg)
    
    If c1 <= 0 Then
        c1 = 1
    ElseIf c1 > varg(3) Then
        c1 = varg(2)
    End If
    
    If c2 < 0 Then
        c2 = 1
    ElseIf c2 = 0 Or c2 > varg(3) Then
        c2 = varg(2)
    End If
    
    If c2 < c1 Then
        c3 = c1
        c1 = c2
        c2 = c3
    End If
    
    
    Dim rv As Variant
    ReDim rv(1 To varg(1), 1 To 1) As Variant
    For i = 1 To varg(2)
        s = 0
        For j = c1 To c2
           s = s + varg(1)(i, j)
        Next j
        rv(i, 1) = s
    Next i
    
    sum_across_matrix_rows = rv
    
End Function


Function sum_acrossrows(ByRef arg As Range, Optional c1 = 0, Optional c2 = 0, Optional strict = 0, Optional dep = 1) As Variant

    If c1 <= 0 Then
        c1 = 1
    End If
    If strict And (c1 > arg.Columns.Count) Then
        c1 = arg.Columns.Count
    End If
    
    If c2 <= 0 Then
        c2 = arg.Columns.Count
    End If
    If strict And (c2 > arg.Columns.Count) Then
        c2 = arg.Columns.Count
    End If
    
    If c2 < c1 Then
        c3 = c1
        c1 = c2
        c2 = c3
    End If
    
    Dim rv As Variant
    ReDim rv(1 To arg.Rows.Count, 1 To 1) As Variant
    For i = 1 To arg.Rows.Count
        s = 0
        For j = c1 To c2
           s = s + arg(i, j).value
        Next j
        rv(i, 1) = s
    Next i
    
    sum_acrossrows = rv

End Function

Function sum_acrosscolumns(ByRef arg As Range, Optional r1 = 0, Optional r2 = 0, Optional strict = 0, Optional dep = 1) As Variant

    If r1 <= 0 Then
        r1 = 1
    End If
    If strict And (r1 > arg.Rows.Count) Then
        r1 = arg.Rows.Count
    End If
    
    If r2 <= 0 Then
        r2 = arg.Rows.Count
    End If
    If strict And (r2 > arg.Rows.Count) Then
        r2 = arg.Rows.Count
    End If
    
    If r2 < r1 Then
        r3 = r1
        r1 = r2
        r2 = r3
    End If
    
    Dim rv As Variant
    ReDim rv(1 To 1, 1 To arg.Columns.Count) As Variant
    For j = 1 To arg.Columns.Count
        s = 0
        For i = r1 To r2
           s = s + arg(i, j).value
        Next i
        rv(1, j) = s
    Next j
    
    sum_acrosscolumns = rv

End Function

Function bIn(arg, ParamArray args() As Variant) As Boolean

    bIn = False
    For Each V In args
        If arg = V Then
            bIn = True
            Exit Function
        End If
    Next V
    
End Function

Sub utl_CopyFormulas()

    Dim x, y, z As Range
    
    Set x = Selection
    
    If x.Areas.Count = 2 Then
        Set y = x.Areas(1)
        Set z = x.Areas(2)
        If z.Cells.Count > 1 Then
        Set z = z.SpecialCells(xlTopLeftCell)
        End If

        m = y.Rows.Count
        n = y.Columns.Count
        For i = 1 To m
        For j = 1 To n
            s = y(i, j).Formula
            z.Offset(i - 1, j - 1) = s
        Next j
        Next i
    End If
            

End Sub

Sub wds_Workbook_Overview()
    
    Dim twb As Workbook
    Dim tws As Worksheet
    Dim nws As Worksheet
    Dim ows As Worksheet
    
    Set twb = ActiveWorkbook
    Set tws = ActiveSheet
    
    If fIsASheetName("WorkbookOverview", tws.Cells(1, 1)) Then
        MsgBox ("WorkbookOverview sheet exists in activeworkbook " & twb.Name & " and will be cleared")
    End If
    Call ActivateOrAddSheet("WorkbookOverview")
    Set nws = ActiveSheet
    nws.Cells.Clear
    Dim i As Integer
    
    i = 1
    nws.Cells(i, 1) = "Workbook Name"
    nws.Cells(i, 2) = twb.Name
    i = i + 1
    nws.Cells(i, 1) = "Workbook Path"
    nws.Cells(i, 2) = twb.Path
    i = i + 1
    nws.Cells(i, 1) = "Application Path"
    nws.Cells(i, 2) = twb.Parent.Path
    i = i + 1
    i = i + 1
    nws.Cells(i, 1) = "Sheets"
    For Each ows In twb.Sheets
        i = i + 1
        nws.Cells(i, 1) = ows.Name
    Next ows
    
    i = i + 1
    i = i + 1
    nws.Cells(i, 1) = "Named Ranges"
    Dim nmrng As Name
    For Each nmrng In twb.Names
        i = i + 1
        nws.Cells(i, 1) = nmrng.Name
        nws.Cells(i, 2) = "Workbook Level"
        nws.Cells(i, 3) = "'" & nmrng.value
    Next nmrng
    For Each ows In twb.Sheets
        For Each nmrng In ows.Names
            i = i + 1
            nws.Cells(i, 1) = nmrng.Name
            nws.Cells(i, 2) = ows.Name
            nws.Cells(i, 3) = nmrng.value
        Next nmrng
    Next ows
    
    i = i + 1
    i = i + 1
    nws.Cells(i, 1) = "Data Connections"
    Dim wdc As WorkbookConnection
    For Each wdc In twb.Connections
        i = i + 1
        nws.Cells(i, 1) = wdc.Name
        nws.Cells(i, 2) = wdc.Description
    Next
    
    i = i + 1
    i = i + 1
    nws.Cells(i, 1) = "Theme"
    Dim wtheme As ThemeColor
    Dim ind
    Dim j, j1, j2, j3 As Long
    j = 0
    For Each ind In Array(msoThemeDark1, msoThemeLight1, msoThemeDark2, msoThemeLight2, msoThemeAccent1, msoThemeAccent2, msoThemeAccent3, msoThemeAccent4, msoThemeAccent5, msoThemeAccent6)
        Set wtheme = twb.Theme.ThemeColorScheme.Colors(ind)
        i = i + 1
        nws.Cells(i, 1) = ind
        nws.Cells(i, 2) = Array("msoThemeDark1", "msoThemeLight1", "msoThemeDark1", "msoThemeDark2", "msoThemeLight2", "msoThemeAccent1", "msoThemeAccent2", "msoThemeAccent3", "msoThemeAccent4", "msoThemeAccent5", "msoThemeAccent6")(ind)
        nws.Cells(i, 3) = ind
        Dim r As Range
        Set r = nws.Cells(i, 7)
        r.Borders.LineStyle = xlSolid
        r.Borders.Value = 0
        r.Borders.Weight = 3
        r.Interior.ThemeColor = wtheme.ThemeColorSchemeIndex
        r.Borders.ThemeColor = wtheme.ThemeColorSchemeIndex
        j = wtheme.RGB
        j1 = j Mod (RGB(255, 0, 0) + 1)
        j2 = (j - j1) Mod (RGB(0, 255, 0) + 256)
        j3 = (j - j1 - j2)
        j2 = j2 / 256
        j3 = j3 / 256 / 256
        nws.Cells(i, 3) = 255 - j3
        nws.Cells(i, 4) = 255 - j2
        nws.Cells(i, 5) = 255 - j1
        nws.Cells(i, 6) = Application.WorksheetFunction.Dec2Hex(j3 + j2 * 256 + j1 * 256 * 256)
    Next

    i = i + 1
    i = i + 1
    nws.Cells(i, 1) = "Styles"
    Dim wstyle As Style
    For Each wstyle In twb.Styles
        i = i + 1
        nws.Cells(i, 1) = wstyle.Name
        nws.Cells(i, 2) = wstyle.NameLocal
    Next

    i = i + 1
    i = i + 1
    nws.Cells(i, 1) = "Modules"
    Dim wmod
    For Each wmod In twb.VBProject.VBComponents
    If wmod.Type = 1 Or wmod.Type = 2 Then
        i = i + 1
        nws.Cells(i, 1) = wmod.Name
        On Error Resume Next
        Select Case wmod.Type
        Case 1
            nws.Cells(i, 2) = "VBAModule"
        Case 2
            nws.Cells(i, 2) = "VBAClass"
        Case Else
            nws.Cells(i, 2) = "Other"
        End Select
    End If
    Next
    
    i = i + 1
    i = i + 1
    nws.Cells(i, 1) = "References"
    For Each wvbref In twb.VBProject.References
        i = i + 1
        nws.Cells(i, 1) = wvbref.Name
        nws.Cells(i, 2) = wvbref.Description
    Next

End Sub

Sub wds_Workbook_CleanNamedRangesWithRefError()
    
    Dim twb As Workbook
    Dim tws, nws, ows As Worksheet
    
    Set twb = ActiveWorkbook
    Set tws = ActiveSheet
    
    Dim i As Integer
    
    Dim nmrng As Name
    For i = twb.Names.Count To 1 Step -1
        Set nmrng = twb.Names(i)
        If InStr(nmrng.value, "#REF") Then
            nmrng.Delete
        End If
    Next i
    For Each ows In twb.Sheets
        For i = ows.Names.Count To 1 Step -1
            Set nmrng = ows.Names(i)
            If InStr(nmrng.value, "#REF") Then
                nmrng.Delete
            End If
        Next i
    Next ows

End Sub

Sub wds_Workbook_CleanNamedRangesSelected()
    
    Dim twb As Workbook
    Dim tws, nws, ows As Worksheet
    
    calcprior = Application.Calculation
    On Error GoTo CatchIt
    Application.Calculation = xlCalculationManual
    
    Set twb = ActiveWorkbook
    Set tws = ActiveSheet
    If tws.Name <> "WorkbookOverview" Then
        MsgBox ("Select cells with named ranges on a sheet ""WorkbookOverview"" as created by Workbook_Overview")
        Exit Sub
    End If
    
    Dim r, c As Range
    Set r = Selection
    For Each c In r.Cells
        If c.Offset(0, 1).value = "Workbook Level" Then
            twb.Names(c.value).Delete
        Else
            If fIsASheetName(c.Offset(0, 1).value, c) Then
                twb.Sheets(c.Offset(0, 1).value).Names(c.value).Delete
            End If
        End If
    Next c
    
CatchIt:
    Application.Calculation = calcprior

ElseIt:
    
End Sub


Public Function WDSCore_CallMacroOptions_Arrays() As Variant

    Dim rv As Variant
    
    ReDim rv(1 To 100) As Variant
    Dim i As Integer
    i = 0
    i = i + 1
    rv(i) = fSheetName_MacroOptions_Array()
    i = i + 1
    rv(i) = fWBName_MacroOptions_Array()
    i = i + 1
    rv(i) = fWBPath_MacroOptions_Array()
    i = i + 1
    rv(i) = fIsASheetName_MacroOptions_Array()
    i = i + 1
    rv(i) = fArray_MacroOptions_Array()
    i = i + 1
    rv(i) = fKeyValuePairs_MacroOptions_Array()
    i = i + 1
    rv(i) = fQuote_MacroOptions_Array()
    i = i + 1
    rv(i) = fColumnFromCode_MacroOptions_Array()
    i = i + 1
    rv(i) = mnmx_MacroOptions_Array()
    i = i + 1
    rv(i) = fCleanLimits_MacroOptions_Array()
    i = i + 1
    rv(i) = fDate2MonthID_MacroOptions_Array()
    i = i + 1
    rv(i) = fMonthID2Date_MacroOptions_Array()
    i = i + 1
    rv(i) = fMonthN2MonthID_MacroOptions_Array()
    i = i + 1
    rv(i) = fMonthID2MonthN_MacroOptions_Array()
    i = i + 1
    rv(i) = ifNull_MacroOptions_Array()
    i = i + 1
    rv(i) = ifNullOrZero_MacroOptions_Array()
    i = i + 1
    rv(i) = lMax_MacroOptions_Array()
    i = i + 1
    rv(i) = lMin_MacroOptions_Array()
    i = i + 1
    rv(i) = XmlMapSchema_MacroOptions_Array()
    i = i + 1
    rv(i) = FlipSumProduct_MacroOptions_Array()
    i = i + 1
    rv(i) = rv(i - 1)
    rv(i)(1) = "Stop"

    WDSCore_CallMacroOptions_Arrays = rv

End Function
Public Sub WDSCore_CallMacroOptions()

    Dim localarrays As Variant
    localarrays = WDSCore_CallMacroOptions_Arrays()
    
    Call WDSCore_SetMacroOptions(localarrays)
    
End Sub

Public Function fWBCustomXMLParts(ByRef arg As Range) As Variant
    
    Dim rv As Variant
    Dim i As Integer
    ReDim rv(1 To arg.Worksheet.Parent.CustomXMLParts.Count) As Variant
    For i = 1 To arg.Worksheet.Parent.CustomXMLParts.Count
        rv(i) = arg.Worksheet.Parent.CustomXMLParts(i).XML
    Next
    fWBCustomXMLParts = rv

End Function

Public Sub WDSCore_RemoveIntelliSense()
    
    Dim xmlp As CustomXMLPart
    Dim node As CustomXMLNode
    For Each xmlp In ActiveWorkbook.CustomXMLParts
        If xmlp.DocumentElement.BaseName = "IntelliSense" Then
            If xmlp.DocumentElement.Attributes.Count > 0 Then
                For Each node In xmlp.DocumentElement.Attributes
                    If node.BaseName = "WDSVBAModule" Then
                        xmlp.Delete
                        Exit For
                    End If
                Next node
            End If
        End If
    Next xmlp

End Sub

Public Sub WDSCore_SetMacroOptions(functioninfoarrays)

    Dim doc As String
    doc = ""

    Dim xmlp As CustomXMLPart
    Dim wdsxmlp As CustomXMLPart
    Dim node As CustomXMLNode
    Dim node1 As CustomXMLNode
    Dim node2 As CustomXMLNode
    Dim found As Boolean
    found = False
    Dim found_wds As Boolean
    found_wds = False
    Dim found_other As Boolean
    found_other = False
    For Each xmlp In ActiveWorkbook.CustomXMLParts
        If xmlp.DocumentElement.BaseName = "IntelliSense" Then
            If found_wds Then
                found_other = True
            Else
                If xmlp.DocumentElement.Attributes.Count > 0 Then
                    For Each node In xmlp.DocumentElement.Attributes
                        If node.BaseName = "WDSVBAModule" Then
                            found_wds = True
                            Set wdsxmlp = xmlp
                            Exit For
                        End If
                    Next node
                Else
                    found_other = True
                End If
            End If
        End If
    Next xmlp

    If found_wds Then
        'delete any existing XML nodes and build a string XML of the others

        'Note: The approach below was tried just to add new functions, but
        'ExcelDna.IntelliSense may not be able to handle the namespace this approach creates on each nodes
        '        If False Then
        '            Set node = wdsxmlp.DocumentElement.SelectSingleNode("node()[name()=""FunctionInfo""]")
        '            Dim nnode As CustomXMLNode
        '            Dim nnodea As CustomXMLNode
        '
        '            'Call wdsxmlp.AddNode(node, "Function", node.NamespaceURI, , msoCustomXMLNodeElement)
        '            'leaving the argument off creates an empty namespace
        '            Call wdsxmlp.AddNode(node, "Function", , , msoCustomXMLNodeElement)
        '            Set nnode = node.LastChild
        '
        '            Call wdsxmlp.AddNode(nnode, "Name", , , msoCustomXMLNodeAttribute, x(1))
        '            Call wdsxmlp.AddNode(nnode, "Description", , , msoCustomXMLNodeAttribute, x(2))
        '            Call wdsxmlp.AddNode(nnode, "HelpTopic", , , msoCustomXMLNodeAttribute, x(3))
        '            Call wdsxmlp.AddNode(nnode, "Category", , , msoCustomXMLNodeAttribute, x(4))
        '            For i = 1 To nx
        '
        '                Call wdsxmlp.AddNode(nnode, "Argument", , , msoCustomXMLNodeElement)
        '                Set nnodea = nnode.LastChild
        '                Call wdsxmlp.AddNode(nnodea, "Name", , , msoCustomXMLNodeAttribute, x(5)(i)(1))
        '                Call wdsxmlp.AddNode(nnodea, "Description", , , msoCustomXMLNodeAttribute, x(5)(i)(2))
        '            Next i
        '        End If


        Dim s, s2, s3, XML As String
        Dim i As Integer
        
        's2 is for the XML of all other functions
        s2 = ""

        Dim nodes As CustomXMLNodes
        'Note: For some reason, the simpler "//FunctionInfo/Function"  XPath function was not working as it should...
        Set nodes = wdsxmlp.DocumentElement.SelectNodes("node()[name()=""FunctionInfo""]//node()[name()=""Function""]")
        If nodes.Count > 0 Then
            For Each node In nodes
                found = False
                's3 is for the XML of one function
                s3 = "<Function "
                For Each node1 In node.Attributes
                    If node1.BaseName = "Name" Then
                        For i = LBound(functioninfoarrays) To UBound(functioninfoarrays)
                            If IsEmpty(functioninfoarrays(i)) Then
                                Exit For
                            End If
                            x = functioninfoarrays(i)
                            If LCase(x(1)) = "stop" Then
                                Exit For
                            End If
                            If node1.NodeValue = x(1) Then
                                found = True
                                Exit For
                            End If
                        Next i
                    End If
                    If found Then
                        Exit For
                    End If
                    s3 = s3 + node1.BaseName + "=""" + node1.NodeValue + """ "
                Next node1
                If found Then
                    node.Delete
                Else
                    If node.ChildNodes.Count > 0 Then
                        s3 = s3 + ">"
                        For Each node1 In node.ChildNodes
                            s3 = s3 + "<Argument "
                            For Each node2 In node1.Attributes
                                s3 = s3 + node2.BaseName + "=""" + node2.NodeValue + """ "
                            Next node2
                            s3 = s3 + "/>"
                        Next node1
                        s3 = s3 + "</Function>"
                    Else
                        s3 = s3 + "/>"
                    End If
                    s2 = s2 + s3
                End If
            Next
        End If

    End If

    For i = LBound(functioninfoarrays) To UBound(functioninfoarrays)
        
        If IsEmpty(functioninfoarrays(i)) Then
            Exit For
        End If
        x = functioninfoarrays(i)
        If LCase(x(1)) = "stop" Then
            Exit For
        End If
        
        Dim nx As Integer
        nx = UBound(x(5)) - LBound(x(5)) + 1
        Dim mx
        ReDim mx(1 To nx) As String

            s = s + "<Function Name=""" + x(1) + """ Description=""" + x(2) + """ HelpTopic=""" + x(3) + """ Category=""" + x(4) + """ >" & Application.WorksheetFunction.Unichar(10)
        For j = 1 To nx
            s = s + "<Argument Name=""" + x(5)(j)(1) + """ Description=""" + x(5)(j)(2) + """ />" & Application.WorksheetFunction.Unichar(10)
            mx(j) = x(5)(j)(2)
        Next j
        s = s + "</Function>" & Application.WorksheetFunction.Unichar(10)

        Application.MacroOptions Macro:=x(1), Description:=x(2), HelpFile:=x(3), Category:=x(4), ArgumentDescriptions:=mx

        Next i

    If found_wds Then

        wdsxmlp.Delete
        XML = "<IntelliSense xmlns=""http://schemas.excel-dna.net/intellisense/1.0"" WDSVBAModule=""Yes"" ><FunctionInfo>" _
            + s2 + s + "</FunctionInfo></IntelliSense>"
        ActiveWorkbook.CustomXMLParts.Add (XML)

    Else

        If found_other Then
            MsgBox ("Note: there is another CustomXMLPart with ExcelDna.Intellisense, those functions may not show. " _
                + "Run <WDSCore_RemoveIntelliSense> to delete the WDSVBAModule attributed CustomXMLPart if required.")
        End If
            XML = "<IntelliSense xmlns=""http://schemas.excel-dna.net/intellisense/1.0"" WDSVBAModule=""Yes"" ><FunctionInfo>" _
            + s + "</FunctionInfo></IntelliSense>"
            ActiveWorkbook.CustomXMLParts.Add (XML)

        End If

        MsgBox ("Note: Updates to ExecelDna.IntelliSense for module " + WDSVBAModuleName _
        + " may not be available until workbook is saved and reopened, but fx (insert function) help should be available.")

End Sub














