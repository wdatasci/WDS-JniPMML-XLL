'''Wypasek Data Science, Inc., Copyright 2019
'''Author: Christian Wypasek
Option Base 1
Const WDSCoreContextID = 40001
Const WDSVBAModuleName = "WDSCore"

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

Private Function IsASheetName_MacroOptions_Array() As Variant
    IsASheetName_MacroOptions_Array = Array("IsASheetName" _
    , "Returns True if input string is a sheetname in the workbook referenced by the second argument" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("InputString", "String to check if it is a worksheet name"), _
    Array("ACellOrRange", "A reference cell for the target workbook, if not used, checks against the active workbook") _
    ) _
    )
End Function

Function IsASheetName(ByVal s As String, ByRef arg As Range) As Boolean
    Dim x As Worksheet
    Dim twb As Workbook

    If arg Is Nothing Then
        Set twb = ActiveWorkbook
    Else
        Set twb = arg.Parent.Parent
    End If

    IsASheetName = False
    For Each x In twb.Sheets
        If x.Name = s Then
            IsASheetName = True
            Exit For
        End If
    Next x

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
        fColumnFromCode = Chr(z + 64) + Chr(y + 64) & Chr(x + 64)
    ElseIf y > 0 Then
        fColumnFromCode = Chr(y + 64) & Chr(x + 64)
    Else
        fColumnFromCode = Chr(y + 64)
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

Function FlipSumProduct(ByRef r As Range, ByRef s As Range) As Double

    Dim sm, lcls As Double

    sm = 0

    Dim i, j, k, l As Integer

    l = r.Rows.Count
    If l > s.Rows.Count Then
        l = s.Rows.Count
    End If

    k = l + 1
    For i = 1 To l
        k = k - 1
        lcls = ifnull(r.Cells(k, 1), 0) * ifnull(s.Cells(i, 1), 0)
        sm = sm + lcls
    Next

    FlipSumProduct = sm

End Function

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
    rv(i) = IsASheetName_MacroOptions_Array()
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

            s = s + "<Function Name=""" + x(1) + """ Description=""" + x(2) + """ HelpTopic=""" + x(3) + """ Category=""" + x(4) + """ >"
        For j = 1 To nx
            s = s + "<Argument Name=""" + x(5)(j)(1) + """ Description=""" + x(5)(j)(2) + """ />"
            mx(j) = x(5)(j)(2)
        Next j
        s = s + "</Function>"

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












