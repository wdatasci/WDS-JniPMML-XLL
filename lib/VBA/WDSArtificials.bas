Attribute VB_Name = "WDSArtificials"
'''Wypasek Data Science, Inc., Copyright 2019
'''Author: Christian Wypasek
Option Base 1
Const WDSContextID = 40003
Const WDSModuleName = "WDSVBAArtificialFunctions"

Private Enum eTreatment
    Unknown = -1
    None = 0
    Constant = 1
    CodedMissings = 2
    Discrete = 3
    Hats = 4
    iHats = 5
    BSplineOrder2 = 6
    BSplineOrder3 = 7
    Categorical = 8
    CategoricalNumeric = 9
End Enum


Private Type tVarInfoBundle
    
    ncrits As Integer
    ncritrows As Integer
    ncols As Integer
    firstcol As Integer
    lastcol As Integer
    
    bUseCLLeft As Boolean
    bUseCLRight As Boolean
    
    Treatment As eTreatment
    
End Type

Private Function fVarInfoBundle(ByVal Treatment As String, ByRef arg2 As Range _
    , CleanLimitLeftVal As Variant, CleanLimitRightVal As Variant) As tVarInfoBundle

    Dim rv As tVarInfoBundle
    
    rv.ncrits = arg2.Columns.Count
    For i = 1 To rv.ncrits
        If IsEmpty(arg2.Cells(1, i)) Then
            rv.ncrits = i - 1
            Exit For
        End If
    Next i
    
    rv.bUseCLLeft = Not IsEmpty(CleanLimitLeftVal)
    rv.bUseCLRight = Not IsEmpty(CleanLimitRightVal)
    
    rv.ncols = 1
    rv.firstcol = 0
    rv.lastcol = 1
    
    Select Case LCase(Treatment)
        Case "hats", "bz1"
            rv.Treatment = Hats
            rv.ncols = rv.ncrits + 1
        Case "discrete", "discretize", "disc", "buckets", "levels", "intervals", "bz0"
            rv.Treatment = Discrete
            rv.ncols = rv.ncrits + 2
        Case "ihats", "integratedhats"
            rv.Treatment = iHats
            rv.ncols = rv.ncrits + 1
        Case "bsplineorder2", "bsplineo2", "bso2", "bz2"
            rv.Treatment = BSplineOrder2
            rv.ncols = rv.ncrits
        Case "bsplineorder3", "bsplineo3", "bso3", "bz3"
            rv.Treatment = BSplineOrder3
            rv.ncols = rv.ncrits - 1
        Case "cat", "categorical", "string"
            rv.Treatment = Categorical
            rv.ncritrows = arg2.Rows.Count
            rv.ncols = rv.ncrits + 1
        Case "categoricalnumeric", "ncategorical", "catnum", "ncat"
            rv.Treatment = CategoricalNumeric
            rv.ncritrows = arg2.Rows.Count
            rv.ncols = rv.ncrits + 1
        Case "none", "straight", "straightup", "numeric"
            rv.Treatment = None
            rv.ncols = 1
            rv.firstcol = 1
        Case "codedmissings", "missings"
            rv.Treatment = CodedMissings
            rv.ncols = 2
        Case "constant"
            rv.Treatment = Constant
            rv.ncols = 1
            rv.firstcol = 1
        Case Else
        Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Unrecognized Treatment!"
    End Select
    
    If (rv.Treatment = Hats Or rv.Treatment = iHats) And rv.ncrits = 1 Then
        Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Knots"
    ElseIf (rv.Treatment = BSplineOrder2) And rv.ncrits <= 3 Then
        Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Knots"
    ElseIf (rv.Treatment = BSplineOrder3) And rv.ncrits <= 4 Then
        Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Knots"
    ElseIf (rv.Treatment = Discrete Or rv.Treatment = Categorical Or rv.Treatment = CategoricalNumeric) And rv.ncrits = 0 Then
        Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Knots"
    End If
    
    fVarInfoBundle = rv
    
End Function


Private Function fArtificialsCount_MacroOptions_Array() As Variant
    fArtificialsCount_MacroOptions_Array = Array("fArtificialsCount" _
    , "Returns the number of artificial variables for a given treatment and set of critical values" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("Treatment", "One of [None|Constant|CodedMissings|Discrete|Hats|iHats|BSplineOrder2|BSO2|BSO3|Categorical|CategoricalNumeric] or an alias"), _
    Array("CriticalValues", "A range of critical values") _
    ) _
    )
End Function

Function fArtificialsCount(ByVal Treatment As String, ByRef arg2 As Range) As Integer
    Dim vib As tVarInfoBundle
    vib = fVarInfoBundle(Treatment, arg2, None, None)
    fArtificialsCount = vib.ncols
End Function

Private Function fArtificialsLabels_MacroOptions_Array() As Variant
    fArtificialsLabels_MacroOptions_Array = Array("fArtificialsLabels" _
    , "Returns the labels for artificial variables for a given treatment and set of critical values" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("Treatment", "One of [None|Constant|CodedMissings|Discrete|Hats|iHats|BSplineOrder2|BSO2|BSO3|Categorical|CategoricalNumeric] or an alias"), _
    Array("CriticalValues", "A range of critical values"), _
    Array("VariableNameBase", "Optional, defaults to X") _
    ) _
    )
End Function

Function fArtificialsLabels(ByVal Treatment As String, ByRef arg2 As Range, Optional VariableBase = "X") As String()
    Dim vib As tVarInfoBundle
    vib = fVarInfoBundle(Treatment, arg2, None, None)
    Dim rv
    ReDim rv(1 To vib.ncols) As String
    For i = 1 To vib.ncols
        rv(i) = VariableBase & (i - 1 + vib.firstcol)
    Next i
    fArtificialsLabels = rv
End Function


Private Function fArtificials_MacroOptions_Array() As Variant
    fArtificials_MacroOptions_Array = Array("fArtificials" _
    , "Returns an array-value of artificial variables for a range input given treatment and set of critical values" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("Treatment", "One of [None|Constant|CodedMissings|Discrete|Hats|iHats|BSplineOrder2|BSO2|BSO3|Categorical|CategoricalNumeric] or an alias"), _
    Array("Input", "An input value or column"), _
    Array("CriticalValues", "A range of critical values"), _
    Array("CleanLimitLeft", "Optional, left hand clean limit"), _
    Array("CleanLimitRight", "Optional, right hand clean limit") _
    ) _
    )
End Function

Function fArtificials(ByVal Treatment As String, ByRef arg As Range, ByRef arg2 As Range, _
    Optional CleanLimitLeftVal = Empty, Optional CleanLimitRightVal = Empty) As Variant()
    
    Dim vib As tVarInfoBundle
    vib = fVarInfoBundle(Treatment, arg2, CleanLimitLeftVal, CleanLimitRightVal)
    
    Dim rc As Variant
    
    Dim CVs 'CriticalValues
    Dim dCVs 'for sequential differences
    Dim d2CVs 'for two-step sequential differences
    Dim d3CVs 'for three-step sequential differences
    If vib.Treatment = Categorical Or vib.Treatment = CategoricalNumeric Then
        ReDim CVs(1 To arg2.Rows.Count, 1 To arg2.Columns.Count) As Variant
        CVs = arg2.Value
    ElseIf Not (vib.Treatment = None Or vib.Treatment = Constant) Then
        n = vib.ncrits
        
        ReDim CVs(1 To n) As Double
        For i = 1 To vib.ncrits
            CVs(i) = arg2.Cells(1, i).Value
        Next i
        'check the critical values for order
        For i = 2 To vib.ncrits
            If CVs(i) <= CVs(i - 1) Then
                Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Knots"
            End If
        Next i
        If vib.Treatment = Hats Or vib.Treatment = iHats Or vib.Treatment = BSplineOrder2 Or vib.Treatment = BSplineOrder3 Then
            ReDim dCVs(1 To vib.ncrits - 1) As Double
            For i = 1 To vib.ncrits - 1
                dCVs(i) = CVs(i + 1) - CVs(i)
            Next i
        End If
        If vib.Treatment = BSplineOrder2 Or vib.Treatment = BSplineOrder3 Or vib.Treatment = iHats Then
            ReDim d2CVs(1 To vib.ncrits - 1) As Double
            For i = 1 To vib.ncrits - 2
                d2CVs(i) = CVs(i + 2) - CVs(i)
            Next i
        End If
        If vib.Treatment = BSplineOrder3 Then
            ReDim d3CVs(1 To vib.ncrits - 1) As Double
            For i = 1 To vib.ncrits - 3
                d3CVs(i) = CVs(i + 3) - CVs(i)
            Next i
        End If
    End If
    
        
    nrows = Application.Caller.Rows.Count
    If nrows < arg.Rows.Count Then: nrows = arg.Rows.Count
    
    
    ReDim rc(nrows, vib.ncols) As Variant
    
    
    For i = 1 To nrows
        For j = 1 To vib.ncols
            rc(i, j) = 0
        Next
    Next
    
    'On Error Resume Next
    Dim tempval As Variant
    Dim tempdouble As Double
    Dim x As Double

    'CodeDoc - CJW :
    '   For consistency, using:
    '       r for row index
    '       i for critical value index or the artificial index in the usual sense, X_0, X_1, ..., X_{n}
    '       ia for the VBA 'option base 1' artificial index
    '       k for score index

    For r = 1 To nrows
        tempval = arg.Cells(r, 1).Value

        If vib.Treatment = None Or vib.Treatment = Constant Then
            ia = 1

            rc(r, ia) = tempval

        ElseIf vib.Treatment = Categorical Or vib.Treatment = CategoricalNumeric Then
            found = False
            If IsError(tempval) Then
                found = True
                i = 0
            Else
                For i = 1 To vib.ncrits
                    For j = 1 To vib.ncritrows
                        If IsEmpty(CVs(j, i)) Then
                            Exit For
                        End If
                        If tempval = CVs(j, i) Then
                            found = True
                            Exit For
                        End If
                    Next j
                    If found Then
                        Exit For
                    End If
                Next i
            End If
            If found Then
                ia = i + 1
            Else
                ia = 1
            End If

                rc(r, ia) = 1

        Else
            bIsMissing = Not IsNumeric(tempval)
            If Not bIsMissing And vib.bUseCLLeft Then
                bIsMissing = tempval < CleanLimitLeftVal
            End If
            If Not bIsMissing And vib.bUseCLRight Then
                bIsMissing = tempval < CleanLimitRightVal
            End If

            If bIsMissing Then
                ia = 1

                    rc(r, ia) = 1

            Else


            'just to keep things communicable and relatable to usual mathematical discussion

            x = CDbl(tempval)

            If vib.Treatment = CodedMissings Then
                'simple case, missings have already been addressed
                i = 1
                ia = 2

                rc(r, ia) = x

            ElseIf x <= CVs(1) Then
                'all non-missing first artificials are 1 left of the first critical value, except iHats
                i = 1
                ia = 2
                If vib.Treatment = iHats Then
                    tempdouble = x - CVs(1)

                        rc(r, ia) = tempdouble

                Else

                        rc(r, ia) = 1

                End If
            ElseIf x >= CVs(vib.ncrits) Then
                'all non-missing last artificials are 1 right of the last critical value, except iHats
                i = vib.ncrits
                ia = vib.ncols
                If vib.Treatment = iHats Then
                    tempdouble = (x - CVs(i) + dCVs(i - 1) / 2)

                        rc(r, ia) = tempdouble
                        For j = 2 To vib.ncrits - 1
                            ia = j + 1
                            rc(r, ia) = rc(r, ia) + d2CVs(j - 1) / 2
                        Next
                        j = 1
                        ia = 2
                        rc(r, ia) = rc(r, ia) + dCVs(j) / 2

                Else

                        rc(r, ia) = 1

                End If
            Else
                
                
                'main guts of the function.....
                
                'find the critical value interval.....
                For i = vib.ncrits - 1 To 1 Step -1
                    If x >= CVs(i) Then
                        Exit For
                    End If
                Next i
                
                'usual VBA index
                ia = i + 1
                If vib.Treatment = Discrete Then

                    rc(r, ia) = 1

                ElseIf vib.Treatment = Hats Then
                    tempdouble = (x - CVs(i)) / dCVs(i)

                        rc(r, ia + 1) = tempdouble
                        rc(r, ia) = (1 - tempdouble)

                ElseIf vib.Treatment = iHats Then
                tempdouble = ((x - CVs(i)) ^ 2 / dCVs(i) / 2)

                        iaP1 = ia + 1
                        
                        rc(r, iaP1) = rc(r, iaP1) + tempdouble
                        rc(r, ia) = rc(r, ia) + (x - CVs(i) - tempdouble)
                        
                        For ii = 1 To (i - 1)
                            iia = ii + 1
                            iiaP1 = iia + 1
                            
                            rc(r, iiaP1) = rc(r, iiaP1) + dCVs(ii) / 2
                            rc(r, iia) = rc(r, iia) + dCVs(ii) / 2
                        
                        Next ii

                ElseIf vib.Treatment = BSplineOrder2 Then
                    'the first artificial is a left catch all, necessary through knot3
                    'k+2 is more akin to the "usual" index, mapped back to "option base 1" with 0 being the missing code variable
                    
                    iM1 = i - 1
                    ia = i + 2
                    iaM1 = ia - 1
                    iaM2 = ia - 2
                    
                    xMci = (x - CVs(i))
                    xMciM1 = 0
                
                    If i > 1 Then
                        xMciM1 = (x - CVs(iM1))
                    End If
                    
                    'the last artificial is a right catch all
                    'therefore, fo0, [f]unction [o]ffset [0], stops with CVs(vib.ncrits-2)
                    'and fo1 is a catch all at CVs(vib.ncrits-1)
                    
                    fo0 = 0
                    fo1 = 0
                    fo2 = 0
                    
                    If i < vib.ncrits - 1 Then
                        ' a+b=1,p+q=1
                        ' a*p
                        fo0 = xMci / d2CVs(i) * xMci / dCVs(i)
                    End If
                    If i = 1 Then
                        'fo1 is a catch all where not defined
                        fo1 = 1 - fo0
                    ElseIf i < vib.ncrits - 1 Then
                        'the starting interpolation of the preceding basis * corresponding right side of lower order Hat
                        '+the starting right interpolation of preceding basis * corresponding left side of lower order Hat
                        ' a*q
                        ' +b*p
                        fo1 = xMciM1 / d2CVs(iM1) * (1 - xMci / dCVs(i)) _
                                    + (1 - xMci / d2CVs(i)) * xMci / dCVs(i)
                    End If
                    If i = 2 Then
                        'fo2 is a catch all where not defined
                        fo2 = 1 - fo0 - fo1
                    ElseIf i > 2 Then
                        'the remaining right interpolation of second preceding basis * corresponding right side of lower order Hat
                        ' b*q
                        fo2 = (1 - xMciM1 / d2CVs(iM1)) * (1 - xMci / dCVs(i))
                    End If
                    If i = vib.ncrits - 1 Then
                        fo1 = 1 - fo2
                    End If
                    

                    If ia < vib.ncols Then
                        rc(r, ia) = fo0
                    Else
                        rc(r, vib.ncols) = fo0
                    End If
                    
                    If iaM1 > 1 Then
                        If iaM1 < vib.ncols Then
                            rc(r, iaM1) = fo1
                        Else
                            rc(r, vib.ncols) = rc(r, vib.ncols) + fo1
                        End If
                    End If
                    
                    If iaM2 > 1 Then
                        rc(r, iaM2) = rc(r, iaM2) + fo2
                    End If
                    
                ElseIf vib.Treatment = BSplineOrder3 Then
                    
                    
                    iM1 = i - 1
                    iM2 = i - 2
                    ia = i + 2
                    iaM1 = ia - 1
                    iaM2 = ia - 2
                    iaM3 = ia - 3
                    
                    xMci = (x - CVs(i))
                    xMciM1 = 0
                    xMciM2 = 0
                
                    If i = 3 Then
                    x = x
                    End If
                    If i > 1 Then
                        xMciM1 = (x - CVs(iM1))
                    End If
                    If i > 2 Then
                        xMciM2 = (x - CVs(iM2))
                    End If
                    
                    fo0 = 0
                    fo1 = 0
                    fo2 = 0
                    fo3 = 0
                    
                    If i < vib.ncrits - 2 Then
                        ' u+v=1,a+b=1,p+q=1
                        ' u[0]*a[0]*p[0]
                        fo0 = xMci / d3CVs(i) * xMci / d2CVs(i) * xMci / dCVs(i)
                    End If
                    If i = 1 Then
                        fo1 = 1 - fo0
                    ElseIf i < vib.ncrits - 2 Then
                        ' u[-1]*(a[-1]*q[0] + b*p[0])
                        '+v[0]*(a[0]*p[0])
                    
                        fo1 = (xMciM1 / d3CVs(iM1)) * (xMciM1 / d2CVs(iM1) * (1 - xMci / dCVs(i)) _
                                                        + (1 - xMci / d2CVs(i)) * (xMci / dCVs(i))) + _
                                (1 - xMci / d3CVs(i)) * (xMci / d2CVs(i) * xMci / dCVs(i))
                                                                                                                  
                    End If
                    If i = 2 Then
                        fo2 = 1 - fo0 - fo1
                    ElseIf i > 2 And i < vib.ncrits - 1 Then
                        ' u[-2]*(b[-1]*q[0])
                        '+v[-1]*(a[-1]*q[0]+b[0]*p[0])
                        fo2 = (xMciM2 / d3CVs(iM2)) * ((1 - xMciM1 / d2CVs(iM1)) * (1 - xMci / dCVs(i))) _
                            + (1 - xMciM1 / d3CVs(iM1)) * (xMciM1 / d2CVs(iM1) * (1 - xMci / dCVs(i)) _
                                                         + (1 - xMci / d2CVs(i)) * (xMci / dCVs(i)))
                    End If
                    If i = 3 Then
                        fo3 = 1 - fo0 - fo1 - fo2
                    ElseIf i > 3 Then
                        ' v[-2]*b[-1]*p[0]
                        fo3 = (1 - xMciM2 / d3CVs(iM2)) * (1 - xMciM1 / d2CVs(iM1)) * (1 - xMci / dCVs(i))
                    End If
                    
                    
                    
                    If i = vib.ncrits - 2 Then
                        fo1 = 1 - fo2 - fo3
                    End If
                    If i = vib.ncrits - 1 Then
                        fo2 = 1 - fo3
                    End If
                    
                    y = 1

                    If ia < vib.ncols Then
                        rc(r, ia) = fo0
                    Else
                        rc(r, vib.ncols) = fo0
                    End If
                    If iaM1 > 1 Then
                        If iaM1 < vib.ncols Then
                            rc(r, iaM1) = fo1
                        Else
                            rc(r, vib.ncols) = rc(r, vib.ncols) + fo1
                        End If
                    End If
                    If iaM2 > 1 Then
                        If iaM2 < vib.ncols Then
                            rc(r, iaM2) = fo2
                        Else
                            rc(r, vib.ncols) = rc(r, vib.ncols) + fo2
                        End If
                    End If
                    If iaM3 > 1 Then
                    If iaM3 < vib.ncols Then
                        rc(r, iaM3) = fo3
                    Else
                        rc(r, vib.ncols) = rc(r, vib.ncols) + fo3
                    End If
                    End If
                    
                    
                    
                End If
            End If
            End If
            End If
        
    Next
    
    fArtificials = rc

End Function



Private Function fArtificialsScored_MacroOptions_Array() As Variant
    fArtificialsScored_MacroOptions_Array = Array("fArtificialsScored" _
    , "Returns an array-value of scored artificial variables for a range input given treatment and set of critical values" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("Treatment", "One of [None|Constant|CodedMissings|Discrete|Hats|iHats|BSplineOrder2|BSO2|BSO3|Categorical|CategoricalNumeric] or an alias"), _
    Array("Input", "An input value or column"), _
    Array("CriticalValues", "A range of critical values"), _
    Array("CoefficientValues", "A range of coefficients for which columns match the number of artificials and rows represent separate score sets"), _
    Array("CleanLimitLeft", "Optional, left hand clean limit"), _
    Array("CleanLimitRight", "Optional, right hand clean limit") _
    ) _
    )
End Function


Function fArtificialsScored(ByVal Treatment As String, ByRef arg As Range, ByRef arg2 As Range, _
    ByRef arg3 As Range, _
    Optional CleanLimitLeftVal = Empty, Optional CleanLimitRightVal = Empty) As Variant()
    
    Dim vib As tVarInfoBundle
    vib = fVarInfoBundle(Treatment, arg2, CleanLimitLeftVal, CleanLimitRightVal)
    
    Dim rc As Variant
    
    If arg3.Columns.Count <> vib.ncols Then
                Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Coefficients"
    End If
    Dim Coef
    nscores = arg3.Rows.Count
    ReDim Coef(nscores, arg3.Columns.Count) As Double
    Coef = arg3.Value
    
    
    Dim CVs 'CriticalValues
    Dim dCVs 'for sequential differences
    Dim d2CVs 'for two-step sequential differences
    Dim d3CVs 'for three-step sequential differences
    If vib.Treatment = Categorical Or vib.Treatment = CategoricalNumeric Then
        ReDim CVs(1 To arg2.Rows.Count, 1 To arg2.Columns.Count) As Variant
        CVs = arg2.Value
    ElseIf Not (vib.Treatment = None Or vib.Treatment = Constant) Then
        n = vib.ncrits
        
        ReDim CVs(1 To n) As Double
        For i = 1 To vib.ncrits
            CVs(i) = arg2.Cells(1, i).Value
        Next i
        'check the critical values for order
        For i = 2 To vib.ncrits
            If CVs(i) <= CVs(i - 1) Then
                Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Knots"
            End If
        Next i
        If vib.Treatment = Hats Or vib.Treatment = iHats Or vib.Treatment = BSplineOrder2 Or vib.Treatment = BSplineOrder3 Then
            ReDim dCVs(1 To vib.ncrits - 1) As Double
            For i = 1 To vib.ncrits - 1
                dCVs(i) = CVs(i + 1) - CVs(i)
            Next i
        End If
        If vib.Treatment = BSplineOrder2 Or vib.Treatment = BSplineOrder3 Or vib.Treatment = iHats Then
            ReDim d2CVs(1 To vib.ncrits - 1) As Double
            For i = 1 To vib.ncrits - 2
                d2CVs(i) = CVs(i + 2) - CVs(i)
            Next i
        End If
        If vib.Treatment = BSplineOrder3 Then
            ReDim d3CVs(1 To vib.ncrits - 1) As Double
            For i = 1 To vib.ncrits - 3
                d3CVs(i) = CVs(i + 3) - CVs(i)
            Next i
        End If
        
    End If
    
        
    nrows = Application.Caller.Rows.Count
    If nrows < arg.Rows.Count Then: nrows = arg.Rows.Count
    
    
    ReDim rc(nrows, nscores) As Variant
    
    
'    For i = 1 To nrows
'        For j = 1 To nscores
'            rc(i, j) = 0
'        Next
'    Next
    
    'On Error Resume Next
    Dim tempval As Variant
    Dim tempdouble As Double
    Dim x As Double

    'CodeDoc - CJW :
    '   For consistency, using:
    '       r for row index
    '       i for critical value index or the artificial index in the usual sense, X_0, X_1, ..., X_{n}
    '       ia for the VBA 'option base 1' artificial index
    '       k for score index

    For r = 1 To nrows
        tempval = arg.Cells(r, 1).Value
        If vib.Treatment = None Or vib.Treatment = Constant Then
            ia = 1
            For k = 1 To nscores
                rc(r, k) = Coef(k, ia) * tempval
            Next k
        ElseIf vib.Treatment = Categorical Or vib.Treatment = CategoricalNumeric Then
            found = False
            If IsError(tempval) Then
                found = True
                i = 0
            Else
                For i = 1 To vib.ncrits
                    For j = 1 To vib.ncritrows
                        If IsEmpty(CVs(j, i)) Then
                            Exit For
                        End If
                        If tempval = CVs(j, i) Then
                            found = True
                            Exit For
                        End If
                    Next j
                    If found Then
                        Exit For
                    End If
                Next i
            End If
            If found Then
                ia = i + 1
            Else
                ia = 1
            End If
            For k = 1 To nscores
                rc(r, k) = Coef(k, ia)
            Next k
        Else
            bIsMissing = Not IsNumeric(tempval)
            If Not bIsMissing And vib.bUseCLLeft Then
                bIsMissing = tempval < CleanLimitLeftVal
            End If
            If Not bIsMissing And vib.bUseCLRight Then
                bIsMissing = tempval < CleanLimitRightVal
            End If

            If bIsMissing Then
                ia = 1
                For k = 1 To nscores
                    rc(r, k) = Coef(k, ia)
                Next k
            Else

            'just to keep things communicable and relatable to usual mathematical discussion

            x = CDbl(tempval)

            If vib.Treatment = CodedMissings Then
                'simple case, missings have already been addressed
                i = 1
                ia = 2
                For k = 1 To nscores
                    rc(r, k) = Coef(k, 2) * x
                Next k
            ElseIf x <= CVs(1) Then
                'all non-missing first artificials are 1 left of the first critical value, except iHats
                i = 1
                ia = 2
                If vib.Treatment = iHats Then
                    tempdouble = x - CVs(1)
                    For k = 1 To nscores
                        rc(r, k) = Coef(k, 2) * tempdouble
                    Next
                Else
                    For k = 1 To nscores
                        rc(r, k) = Coef(k, 2)
                    Next
                End If
            ElseIf x >= CVs(vib.ncrits) Then
                'all non-missing last artificials are 1 right of the last critical value, except iHats
                i = vib.ncrits
                ia = vib.ncols
                If vib.Treatment = iHats Then
                    tempdouble = (x - CVs(i) + dCVs(i - 1) / 2)
                    For k = 1 To nscores
                        rc(r, k) = Coef(k, ia) * tempdouble
                        For j = 2 To vib.ncrits - 1
                            ia = j + 1
                            rc(r, k) = rc(r, k) + Coef(k, ia) * d2CVs(j - 1) / 2
                        Next
                        j = 1
                        ia = 2
                        rc(r, k) = rc(r, k) + Coef(k, ia) * dCVs(j) / 2
                    Next k
                Else
                    For k = 1 To nscores
                        rc(r, k) = Coef(k, ia)
                    Next k
                End If
            Else
                
                'main guts of the function.....
                
                'find the critical value interval.....
                For i = vib.ncrits - 1 To 1 Step -1
                    If x >= CVs(i) Then
                        Exit For
                    End If
                Next i
                
                'usual VBA index
                ia = i + 1
                If vib.Treatment = Discrete Then
                    For k = 1 To nscores
                        rc(r, k) = Coef(k, ia)
                    Next k
                ElseIf vib.Treatment = Hats Then
                    tempdouble = (x - CVs(i)) / dCVs(i)
                    For k = 1 To nscores
                        rc(r, k) = rc(r, k) + Coef(k, ia + 1) * tempdouble
                        rc(r, k) = rc(r, k) + Coef(k, ia) * (1 - tempdouble)
                    Next k
                ElseIf vib.Treatment = iHats Then
                    tempdouble = ((x - CVs(i)) ^ 2 / dCVs(i) / 2)

                        iaP1 = ia + 1
                        For k = 1 To nscores
                            rc(r, k) = rc(r, k) + Coef(k, iaP1) * tempdouble
                            rc(r, k) = rc(r, k) + Coef(k, ia) * (x - CVs(i) - tempdouble)
                        Next k
                        For ii = 1 To (i - 1)
                            iia = ii + 1
                            iiaP1 = iia + 1
                            For k = 1 To nscores
                                rc(r, k) = rc(r, k) + Coef(k, iiaP1) * dCVs(ii) / 2
                                rc(r, k) = rc(r, k) + Coef(k, iia) * dCVs(ii) / 2
                            Next k
                        Next ii
                
                ElseIf vib.Treatment = BSplineOrder2 Then
                    'first artificial is a left catch all, necessary through knot3
                    'i+2 is more akin to the "usual" index, mapped back to "option base 1" with 0 being the missing code variable
                    iM1 = i - 1
                    ia = i + 2
                    iaM1 = ia - 1
                    iaM2 = ia - 2
                    
                    xMci = (x - CVs(i))
                    xMciM1 = 0
                
                    If i > 1 Then
                        xMciM1 = (x - CVs(iM1))
                    End If
                                        
                    'the last artificial is a right catch all
                    'therefore, fo0, [f]unction [o]ffset [0], stops with CVs(vib.ncrits-2)
                    'and fo1 is a catch all at CVs(vib.ncrits-1)
                    
                    fo0 = 0
                    fo1 = 0
                    fo2 = 0
                    
                    If i < vib.ncrits - 1 Then
                        fo0 = xMci / d2CVs(i) * xMci / dCVs(i)
                    End If
                    If i = 1 Then
                        'fo1 is a catch all where not defined
                        fo1 = 1 - fo0
                    ElseIf i < vib.ncrits - 1 Then
                        fo1 = xMciM1 / d2CVs(iM1) * (1 - xMci / dCVs(i)) _
                                    + (1 - xMci / d2CVs(i)) * xMci / dCVs(i)
                    End If
                    If i = 2 Then
                        'fo2 is a catch all where not defined
                        fo2 = 1 - fo0 - fo1
                    ElseIf i > 2 Then
                        fo2 = (1 - xMciM1 / d2CVs(iM1)) * (1 - xMci / dCVs(i))
                    End If
                    If i = vib.ncrits - 1 Then
                        fo1 = 1 - fo2
                    End If

                    For k = 1 To nscores
                        If ia < vib.ncols Then
                            rc(r, k) = Coef(k, ia) * fo0
                        Else
                            rc(r, k) = Coef(k, vib.ncols) * fo0
                        End If
                        If iaM1 < vib.ncols Then
                            rc(r, k) = rc(r, k) + Coef(k, iaM1) * fo1
                        Else
                            rc(r, k) = rc(r, k) + Coef(k, vib.ncols) * fo1
                        End If
                        If i > 1 Then
                            rc(r, k) = rc(r, k) + Coef(k, iaM2) * fo2
                        End If
                    Next k
                    
                ElseIf vib.Treatment = BSplineOrder3 Then
                    
                    
                    iM1 = i - 1
                    iM2 = i - 2
                    ia = i + 2
                    iaM1 = ia - 1
                    iaM2 = ia - 2
                    iaM3 = ia - 3
                    
                    xMci = (x - CVs(i))
                    xMciM1 = 0
                    xMciM2 = 0
                
                    If i = 3 Then
                    x = x
                    End If
                    If i > 1 Then
                        xMciM1 = (x - CVs(iM1))
                    End If
                    If i > 2 Then
                        xMciM2 = (x - CVs(iM2))
                    End If
                    
                    fo0 = 0
                    fo1 = 0
                    fo2 = 0
                    fo3 = 0
                    
                    If i < vib.ncrits - 2 Then
                        ' u+v=1,a+b=1,p+q=1
                        ' u[0]*a[0]*p[0]
                        fo0 = xMci / d3CVs(i) * xMci / d2CVs(i) * xMci / dCVs(i)
                    End If
                    If i = 1 Then
                        fo1 = 1 - fo0
                    ElseIf i < vib.ncrits - 2 Then
                        ' u[-1]*(a[-1]*q[0] + b*p[0])
                        '+v[0]*(a[0]*p[0])
                    
                        fo1 = (xMciM1 / d3CVs(iM1)) * (xMciM1 / d2CVs(iM1) * (1 - xMci / dCVs(i)) _
                                                        + (1 - xMci / d2CVs(i)) * (xMci / dCVs(i))) + _
                                (1 - xMci / d3CVs(i)) * (xMci / d2CVs(i) * xMci / dCVs(i))
                                                                                                                  
                    End If
                    If i = 2 Then
                        fo2 = 1 - fo0 - fo1
                    ElseIf i > 2 And i < vib.ncrits - 1 Then
                        ' u[-2]*(b[-1]*q[0])
                        '+v[-1]*(a[-1]*q[0]+b[0]*p[0])
                        fo2 = (xMciM2 / d3CVs(iM2)) * ((1 - xMciM1 / d2CVs(iM1)) * (1 - xMci / dCVs(i))) _
                            + (1 - xMciM1 / d3CVs(iM1)) * (xMciM1 / d2CVs(iM1) * (1 - xMci / dCVs(i)) _
                                                         + (1 - xMci / d2CVs(i)) * (xMci / dCVs(i)))
                    End If
                    If i = 3 Then
                        fo3 = 1 - fo0 - fo1 - fo2
                    ElseIf i > 3 Then
                        ' v[-2]*b[-1]*p[0]
                        fo3 = (1 - xMciM2 / d3CVs(iM2)) * (1 - xMciM1 / d2CVs(iM1)) * (1 - xMci / dCVs(i))
                    End If
                    
                    
                    
                    If i = vib.ncrits - 2 Then
                        fo1 = 1 - fo2 - fo3
                    End If
                    If i = vib.ncrits - 1 Then
                        fo2 = 1 - fo3
                    End If
                    
                    y = 1

For k = 1 To nscores
                    If ia < vib.ncols Then
                        rc(r, k) = Coef(k, ia) * fo0
                    Else
                        rc(r, k) = Coef(k, vib.ncols) * fo0
                    End If
                    If iaM1 > 1 Then
                        If iaM1 < vib.ncols Then
                            rc(r, k) = rc(r, k) + Coef(k, iaM1) * fo1
                        Else
                            rc(r, k) = rc(r, k) + Coef(k, vib.ncols) * fo1
                        End If
                    End If
                    If iaM2 > 1 Then
                        If iaM2 < vib.ncols Then
                            rc(r, k) = rc(r, k) + Coef(k, iaM2) * fo2
                        Else
                            rc(r, k) = rc(r, k) + Coef(k, vib.ncols) * fo2
                        End If
                    End If
                    If iaM3 > 1 Then
                    If iaM3 < vib.ncols Then
                        rc(r, k) = rc(r, k) + Coef(k, iaM3) * fo3
                    Else
                        rc(r, k) = rc(r, k) + Coef(k, vib.ncols) * fo3
                    End If
                    End If
Next k
                    
                    
                    
                End If
            End If
        End If
        End If
    Next
    
    fArtificialsScored = rc

End Function


Public Function WDSArtificials_CallMacroOptions_Arrays() As Variant

    Dim rv As Variant
    
    ReDim rv(1 To 100) As Variant
    Dim i As Integer
    i = 0
    i = i + 1
    rv(i) = fArtificialsCount_MacroOptions_Array()
    rv(i) = fArtificialsLabels_MacroOptions_Array()
    rv(i) = fArtificials_MacroOptions_Array()
    rv(i) = fArtificialsScored_MacroOptions_Array()
    i = i + 1
    rv(i) = rv(i - 1)
    rv(i)(1) = "Stop"

    WDSArtificials_CallMacroOptions_Arrays = rv

End Function

