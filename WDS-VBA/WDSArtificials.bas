Attribute VB_Name = "WDSArtificials"
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

Option Base 1
Const WDSContextID = 40003
Const WDSModuleName = "WDSArtificials"

'''See WDSModelSpec documentation for a more detailed description

'eTreatment is an enum for the WDSModelSpec standard treatments
'for a single variable.
'
'As per the documentation, regardless of the model type, individual
'modeling variables are constructed from raw Sources. Note: The
'{\em contruction} has already had any required transformations
'employed, therefore, the input to a {\em Variable} (a mini-score,
'generally used in a larger system) is a single value per row or
'observation and all values for the same Variable within a larger
'model recieve the same treatment.

Private Enum eTreatment
    Unknown = -1
    None = 0                'Numeric, Note: missing or invalid values result in in-valid scores
    Constant = 1            'Numeric, all values are the same, such as for an intercept
    CodedMissings = 2       'Numeric, see docs, X0 is always a missing indicator
    DiscreteLC = 3          'Numeric
    DiscreteRC = 4          'Numeric
    Hats = 5                'Numeric, piecewise linear continuous
    iHats = 6               'Numeric, {\em Integrated Hats}
    BSplineOrder2 = 7       'Numeric
    BSplineOrder3 = 8       'Numeric
    Categorical = 9         'Character/String, X1, ..., XN indicators, X0 indicates all others
    CategoricalNumeric = 10 'Numeric, simple indicators for a finite set of values, X0 all others
End Enum

'Arguments are parsed into an object to encapsulate information as needed.

Private Type tVariableMatter
    
    Name As String
    Handle As String
    ArtBaseName As String
    
    Treatment As eTreatment
    
    CleanLimits As Variant
    bUseCLLeft As Boolean
    bUseCLRight As Boolean
    
    
    CritVals As Variant
    nCritVals As Integer
    nCritValRows As Integer
    
    nArtVars As Integer
    iArtVar_First As Integer
    iArtVar_Last As Integer
    
    ArtVarLabels As Variant
    
    CoefVals As Variant
    nScores As Integer
    
End Type

'simple test for up to 3 dims
Private Function zfNDims(ByRef arg As Variant) As Variant
    On Error GoTo CatchIt
    Dim rv(1 To 4)
TryIt:
    For n = 0 To 2
        u = UBound(arg, n + 1)
        rv(n + 2) = u - LBound(arg, n + 1) + 1
    Next n
CatchIt:
    rv(1) = n
    zfNDims = rv
    On Error GoTo 0
End Function

'a private function to parse and validate arguments

Private Function fVariableMatter(ByVal Treatment As String _
    , ByRef CriticalValues As Variant _
    , ByRef CleanLimits As Variant _
    , Optional CoefficientValues = Null) As tVariableMatter

    
    Dim rv As tVariableMatter
    
    
    If TypeOf CriticalValues Is Range Then
        rv.CritVals = CriticalValues.Value
        rv.nCritVals = CriticalValues.Columns.Count
        For i = 1 To rv.nCritVals
            If IsEmpty(CriticalValues.Cells(1, i)) Then
                rv.nCritVals = i - 1
                Exit For
            End If
        Next i
        rv.nCritValRows = CriticalValues.Rows.Count
    Else
        ndims = zfNDims(CriticalValues)
        If ndims(1) = 0 Then
            rv.nCritVals = 1
            rv.nCritValRows = 1
            rv.CritVals = Array(Array(CriticalValues))
        ElseIf ndims(1) = 1 Then
            rv.nCritVals = ndims(1 + 1)
            rv.nCritValRows = 1
            rv.CritVals = Array(CriticalValues)
        ElseIf ndims(1) = 2 Then
            rv.nCritValRows = ndims(1 + 1)
            rv.nCritVals = ndims(1 + 2)
            rv.CritVals = CriticalValues
        End If
    End If
        
    ndims = zfNDims(CleanLimits)
    If Not ((ndims(1) = 1 And ndims(1 + 1) = 2) Or (ndims(1) = 2 And ndims(1 + 1) = 1 And ndims(1 + 2) = 2)) Then
        Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid CleanLimits"
    End If
    If (ndims(1) = 1 And ndims(1 + 1) = 2) Then
        rv.CleanLimits = Array(CleanLimits(1), CleanLimits(2))
    ElseIf (ndims(1) = 2 And ndims(1 + 1) = 1 And ndims(1 + 2) = 2) Then
        rv.CleanLimits = Array(CleanLimits(1, 1), CleanLimits(1, 2))
    End If
    rv.bUseCLLeft = Not IsEmpty(CleanLimits(1))
    rv.bUseCLRight = Not IsEmpty(CleanLimits(2))
    
    rv.nArtVars = 1
    rv.iArtVar_First = 0
    rv.iArtVar_Last = 1
    
    Select Case LCase(Treatment)
        Case "hats", "bz1"
            rv.Treatment = Hats
            rv.nArtVars = rv.nCritVals + 1
        Case "discretelc", "discretizelc", "disclc", "bucketslc", "levelslc", "intervalslc", "bz0lc", "bso0LC", "caglad", "collor", "lcrl"
            rv.Treatment = DiscreteLC
            rv.nArtVars = rv.nCritVals + 2
        Case "discreterc", "discretizerc", "discrc", "bucketsrc", "levelsrc", "intervalsrc", "bz0rc", "bso0rc", "cadlag", "corlol", "rcll"
            rv.Treatment = DiscreteRC
            rv.nArtVars = rv.nCritVals + 2
        Case "discrete", "discretize", "disc", "buckets", "levels", "intervals", "bz0", "bso0"
            rv.Treatment = DiscreteRC
            rv.nArtVars = rv.nCritVals + 2
        Case "ihats", "integratedhats"
            rv.Treatment = iHats
            rv.nArtVars = rv.nCritVals + 1
        Case "bsplineorder2", "bsplineo2", "bso2", "bz2"
            rv.Treatment = BSplineOrder2
            rv.nArtVars = rv.nCritVals
        Case "bsplineorder3", "bsplineo3", "bso3", "bz3"
            rv.Treatment = BSplineOrder3
            rv.nArtVars = rv.nCritVals - 1
        Case "cat", "categorical", "string"
            rv.Treatment = Categorical
            rv.nArtVars = rv.nCritVals + 1
        Case "categoricalnumeric", "ncategorical", "catnum", "ncat"
            rv.Treatment = CategoricalNumeric
            rv.nArtVars = rv.nCritVals + 1
        Case "none", "straight", "straightup", "numeric"
            rv.Treatment = None
            rv.nArtVars = 1
            rv.iArtVar_First = 1
        Case "codedmissings", "missings"
            rv.Treatment = CodedMissings
            rv.nArtVars = 2
        Case "constant"
            rv.Treatment = Constant
            rv.nArtVars = 1
            rv.iArtVar_First = 1
        Case Else
        Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Unrecognized Treatment!"
    End Select
    
    If (rv.Treatment = Hats Or rv.Treatment = iHats) And rv.nCritVals = 1 Then
        Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Knots"
    ElseIf (rv.Treatment = BSplineOrder2) And rv.nCritVals <= 3 Then
        Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Knots"
    ElseIf (rv.Treatment = BSplineOrder3) And rv.nCritVals <= 5 Then
        Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Knots"
    ElseIf (rv.Treatment = DiscreteLC Or rv.Treatment = DiscreteRC Or rv.Treatment = Categorical Or rv.Treatment = CategoricalNumeric) And rv.nCritVals = 0 Then
        Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Knots"
    End If
    
    If Not (rv.Treatment = None Or rv.Treatment = Constant Or rv.Treatment = Categorical Or rv.Treatment = CategoricalNumeric) Then
        For i = 2 To rv.nCritVals
            If rv.CritVals(1, i) <= rv.CritVals(1, i - 1) Then
                Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Knots"
            End If
        Next i
    End If
    
    If Not IsNull(CoefficientValues) Then
        If TypeOf CoefficientValues Is Range Then
            If rv.nArtVars <> CoefficientValues.Columns.Count Then
                Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Coefficients"
            End If
            rv.nScores = CoefficientValues.Rows.Count
            rv.CoefVals = CoefficientValues.Value
            For i = 1 To rv.nScores
                If IsEmpty(CoefficientValues.Cells(i, 1)) Then
                    rv.nScores = i - 1
                    Exit For
                End If
            Next i
        Else
            ndims = zfNDims(CoefficientValues)
            If ndims(1) = 0 Then
                If rv.nArtVars <> 1 Then
                    Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Coefficients"
                End If
                rv.nScores = 1
                rv.CoefVals = Array(Array(CoefficientValues))
            ElseIf ndims(1) = 1 Then
                If rv.nArtVars <> ndims(1 + 1) Then
                    Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Coefficients"
                End If
                rv.nScores = 1
                rv.CoefVals = Array(CoefficientValues)
            ElseIf ndims(1) = 2 Then
                If rv.nArtVars <> ndims(1 + 2) Then
                    Err.Raise Number:=WDSContextID + 1, Source:=twb.Name & WDSModuleName, Description:="Invalid Coefficients"
                End If
                rv.nScores = ndims(1 + 1)
                rv.CoefVals = CoefficientValues
            End If
        End If
    Else
        rv.nScores = -1
    End If
    
    fVariableMatter = rv
    
End Function


Private Function fArtificialsCount_MacroOptions_Array() As Variant
    fArtificialsCount_MacroOptions_Array = Array("fArtificialsCount" _
    , "Returns the number of artificial variables for a given treatment and set of critical values" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("Treatment", "One of [None|Constant|CodedMissings|DiscreteLC|DiscreteRC|Hats|iHats|BSplineOrder2|BSO2|BSO3|Categorical|CategoricalNumeric] or an alias"), _
    Array("CriticalValues", "A range of critical values") _
    ) _
    )
End Function

Function fArtificialsCount(ByVal Treatment As String, ByRef CriticalValues As Range) As Integer
    Dim varm As tVariableMatter
    varm = fVariableMatter(Treatment, CriticalValues, Array(Nothing, Nothing))
    fArtificialsCount = varm.nArtVars
End Function

Private Function fArtificialsLabels_MacroOptions_Array() As Variant
    fArtificialsLabels_MacroOptions_Array = Array("fArtificialsLabels" _
    , "Returns the labels for artificial variables for a given treatment and set of critical values" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("Treatment", "One of [None|Constant|CodedMissings|DiscreteLC|DiscreteRC|Hats|iHats|BSplineOrder2|BSO2|BSO3|Categorical|CategoricalNumeric] or an alias"), _
    Array("CriticalValues", "A range of critical values"), _
    Array("VariableNameBase", "Optional, defaults to X") _
    ) _
    )
End Function

Function fArtificialsLabels(ByVal Treatment As String, ByRef CriticalValues As Range, Optional VariableBase = "X") As String()
    Dim varm As tVariableMatter
    varm = fVariableMatter(Treatment, CriticalValues, Array(Nothing, Nothing), Null)
    Dim rv
    ReDim rv(1 To varm.nArtVars) As String
    For i = 1 To varm.nArtVars
        rv(i) = VariableBase & (i - 1 + varm.iArtVar_First)
    Next i
    fArtificialsLabels = rv
End Function

Private Function fArtificialsKV_MacroOptions_Array() As Variant
    fArtificialsKV_MacroOptions_Array = Array("fArtificials" _
    , "Returns an array-value of artificial variables for a range input. After the inputs to be evaluated, all other arguments are Key-Value pairs." _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("Input", "An input value or column"), _
    Array("Key", "A key followed by a value or a range where the first column contains keys and subsequent columns contain the values"), _
    Array("Value", "Value associated with prior key or new key-value set") _
    ) _
    )
End Function

Function fArtificialsKV(ByRef InputValues As Range _
    , ParamArray arg() As Variant _
    ) As Variant()
    
    Dim args
    args = arg
    Dim d As Dictionary
    Set d = zfKeyValuePairs(args)
    Dim CleanLimits
    If d.Exists("CleanLimits") Then
        CleanLimits = d.Item("CleanLimits")
    End If
    Dim CleanLimitLeftValue
    If d.Exists("CleanLimitLeftValue") Then
        CleanLimitLeftValue = d.Item("CleanLimitLeftValue")
    End If
    Dim CleanLimitRightValue
    If d.Exists("CleanLimitRightValue") Then
        CleanLimitRightValue = d.Item("CleanLimitRightValue")
    End If
    
    If IsEmpty(CleanLimitLeftValue) And Not IsEmpty(CleanLimits) Then
        CleanLimitLeftValue = CleanLimits(1, 1)
    End If
    If IsEmpty(CleanLimitRightValue) And Not IsEmpty(CleanLimits) Then
        CleanLimitRightValue = CleanLimits(1, 2)
    End If
    
    If d.Exists("CoefficientValues") Then
        fArtificialsKV = fArtificialsScored(d.Item("Treatment"), InputValues, d.Item("CriticalValues"), d.Item("CoefficientValues"), CleanLimitLeftValue, CleanLimitRightValue)
    Else
        fArtificialsKV = fArtificials(d.Item("Treatment"), InputValues, d.Item("CriticalValues"), CleanLimitLeftValue, CleanLimitRightValue)
    End If
        
        
    
End Function
    

Private Function fArtificials_MacroOptions_Array() As Variant
    fArtificials_MacroOptions_Array = Array("fArtificials" _
    , "Returns an array-value of artificial variables for a range input given treatment and set of critical values." _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("Treatment", "One of [None|Constant|CodedMissings|DiscreteLC|DiscreteRC|Hats|iHats|BSplineOrder2|BSO2|BSO3|Categorical|CategoricalNumeric] or an alias"), _
    Array("Input", "An input value or column"), _
    Array("CriticalValues", "A range of critical values"), _
    Array("CleanLimitLeft", "Optional, left hand clean limit"), _
    Array("CleanLimitRight", "Optional, right hand clean limit") _
    ) _
    )
End Function


Function fArtificials(ByVal Treatment As String _
    , ByRef InputValues As Range _
    , ByRef CriticalValues _
    , Optional CleanLimitLeftValue = Empty _
    , Optional CleanLimitRightValue = Empty _
    , Optional eps = 0.00000001 _
    ) As Variant()
    
    Dim varm As tVariableMatter
    varm = fVariableMatter(Treatment, CriticalValues, Array(CleanLimitLeftValue, CleanLimitRightValue), Null)
    
    Dim rc As Variant
    
    Dim CVs 'CriticalValues
    Dim dCVs 'for sequential differences
    Dim d2CVs 'for two-step sequential differences
    Dim d3CVs 'for three-step sequential differences
    Dim Cnstnt As Double
    
    If varm.Treatment = Categorical Or varm.Treatment = CategoricalNumeric Then
        ReDim CVs(1 To varm.nCritValRows, 1 To varm.nCritVals) As Variant
        CVs = varm.CritVals
    ElseIf varm.Treatment = Constant Then
        Cnstnt = varm.CritVals(1, 1)
    ElseIf Not (varm.Treatment = None Or varm.Treatment = Constant) Then
        n = varm.nCritVals
        
        ReDim CVs(1, 1 To n) As Double
        CVs = varm.CritVals
        
        If varm.Treatment = Hats Or varm.Treatment = iHats Or varm.Treatment = BSplineOrder2 Or varm.Treatment = BSplineOrder3 Then
            ReDim dCVs(1, 1 To varm.nCritVals - 1) As Double
            For i = 1 To varm.nCritVals - 1
                dCVs(1, i) = CVs(1, i + 1) - CVs(1, i)
            Next i
        End If
        If varm.Treatment = BSplineOrder2 Or varm.Treatment = BSplineOrder3 Or varm.Treatment = iHats Then
            ReDim d2CVs(1, 1 To varm.nCritVals - 1) As Double
            For i = 1 To varm.nCritVals - 2
                d2CVs(1, i) = CVs(1, i + 2) - CVs(1, i)
            Next i
        End If
        If varm.Treatment = BSplineOrder3 Then
            ReDim d3CVs(1, 1 To varm.nCritVals - 1) As Double
            For i = 1 To varm.nCritVals - 3
                d3CVs(1, i) = CVs(1, i + 3) - CVs(1, i)
            Next i
        End If
    
    End If
    
        
    nrows = Application.Caller.Rows.Count
    If nrows < InputValues.Rows.Count Then: nrows = InputValues.Rows.Count
    
    
    ReDim rc(nrows, varm.nArtVars) As Variant
    
    
    For i = 1 To nrows
        For j = 1 To varm.nArtVars
            rc(i, j) = 0
        Next
    Next
    
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
        tempval = InputValues.Cells(r, 1).Value

        If varm.Treatment = None Then
            
            ia = 1
            rc(r, ia) = tempval

        ElseIf varm.Treatment = Constant Then
            
            ia = 1
            rc(r, ia) = Cnstnt

        ElseIf varm.Treatment = Categorical Or varm.Treatment = CategoricalNumeric Then
            
            isTreatmentNumeric = varm.Treatment = CategoricalNumeric
            found = False
            If IsError(tempval) Then
                found = True
                i = 0
            Else
                For i = 1 To varm.nCritVals
                    For j = 1 To varm.nCritValRows
                        If IsEmpty(CVs(j, i)) Then
                            Exit For
                        End If
                        If isTreatmentNumeric Then
                            If Abs(tempval - CVs(j, i)) < eps Then
                                found = True
                                Exit For
                            End If
                        Else
                            If tempval = CVs(j, i) Then
                                found = True
                                Exit For
                            End If
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
            If Not bIsMissing And varm.bUseCLLeft Then
                bIsMissing = tempval < varm.CleanLimits(1)
            End If
            If Not bIsMissing And varm.bUseCLRight Then
                bIsMissing = tempval > varm.CleanLimits(2)
            End If

            If bIsMissing Then
                ia = 1

                    rc(r, ia) = 1

            Else


            'just to keep things communicable and relatable to usual mathematical discussion

            x = CDbl(tempval)

            If varm.Treatment = CodedMissings Then
                'simple case, missings have already been addressed
                i = 1
                ia = 2

                rc(r, ia) = x

            ElseIf x <= CVs(1, 1) + eps Then

                'all non-missing first artificials are 1 left of the first critical value, except iHats and DiscreteRC
                i = 1
                ia = 2
                If varm.Treatment = iHats Then

                        tempdouble = x - CVs(1, 1)

                        rc(r, ia) = tempdouble

                Else

                        If (varm.Treatment = DiscreteRC) And (x >= CVs(1, 1) - eps) Then

                            i = i + 1
                            ia = ia + 1

                        End If

                        rc(r, ia) = 1

                End If

            ElseIf x >= CVs(1, varm.nCritVals) - eps Then

                'all non-missing last artificials are 1 right of the last critical value, except iHats and DiscreteLC
                i = varm.nCritVals
                ia = varm.nArtVars
                If varm.Treatment = iHats Then
                    tempdouble = (x - CVs(1, i) + dCVs(1, i - 1) / 2)

                        rc(r, ia) = tempdouble
                        For j = 2 To varm.nCritVals - 1
                            ia = j + 1
                            rc(r, ia) = rc(r, ia) + d2CVs(1, j - 1) / 2
                        Next
                        j = 1
                        ia = 2
                        rc(r, ia) = rc(r, ia) + dCVs(1, j) / 2

                Else
                
                        If (varm.Treatment = DiscreteLC) And (x <= CVs(1, varm.nCritVals) + eps) Then

                            i = i - 1
                            ia = ia - 1

                        End If

                        rc(r, ia) = 1

                End If
            Else
                
                
                'main guts of the function.....
                
                'find the critical value interval.....
                If varm.Treatment = DiscreteLC Then
                    For i = varm.nCritVals - 1 To 1 Step -1
                        If x > CVs(1, i) + eps Then
                            Exit For
                        End If
                    Next i
                    'Discrete cases are shifted one
                    i = i + 1
                ElseIf varm.Treatment = DiscreteRC Then
                    For i = varm.nCritVals - 1 To 1 Step -1
                        If x > CVs(1, i) - eps Then
                            Exit For
                        End If
                    Next i
                    i = i + 1
                Else
                    For i = varm.nCritVals - 1 To 1 Step -1
                        If x >= CVs(1, i) Then
                            Exit For
                        End If
                    Next i
                End If
                
                'usual VBA index
                ia = i + 1
                If (varm.Treatment = DiscreteRC) Or (varm.Treatment = DiscreteLC) Then

                    rc(r, ia) = 1

                ElseIf varm.Treatment = Hats Then
                    tempdouble = (x - CVs(1, i)) / dCVs(1, i)

                        rc(r, ia + 1) = tempdouble
                        rc(r, ia) = (1 - tempdouble)

                ElseIf varm.Treatment = iHats Then
                tempdouble = ((x - CVs(1, i)) ^ 2 / dCVs(1, i) / 2)

                        iaP1 = ia + 1
                        
                        rc(r, iaP1) = rc(r, iaP1) + tempdouble
                        rc(r, ia) = rc(r, ia) + (x - CVs(1, i) - tempdouble)
                        
                        For ii = 1 To (i - 1)
                            iia = ii + 1
                            iiaP1 = iia + 1
                            
                            rc(r, iiaP1) = rc(r, iiaP1) + dCVs(1, ii) / 2
                            rc(r, iia) = rc(r, iia) + dCVs(1, ii) / 2
                        
                        Next ii

                ElseIf varm.Treatment = BSplineOrder2 Then
                    'the first artificial is a left catch all, necessary through knot3
                    'k+2 is more akin to the "usual" index, mapped back to "option base 1" with 0 being the missing code variable
                    
                    iM1 = i - 1
                    ia = i + 2
                    iaM1 = ia - 1
                    iaM2 = ia - 2
                    
                    xMci = (x - CVs(1, i))
                    xMciM1 = 0
                
                    If i > 1 Then
                        xMciM1 = (x - CVs(1, iM1))
                    End If
                    
                    'the last artificial is a right catch all
                    'therefore, fo0, [f]unction [o]ffset [0], stops with CVs(1, varm.nCritVals-2)
                    'and fo1 is a catch all at CVs(1, varm.nCritVals-1)
                    
                    fo0 = 0
                    fo1 = 0
                    fo2 = 0
                    
                    If i < varm.nCritVals - 1 Then
                        ' a+b=1,p+q=1
                        ' a*p
                        fo0 = xMci / d2CVs(1, i) * xMci / dCVs(1, i)
                    End If
                    If i = 1 Then
                        'fo1 is a catch all where not defined
                        fo1 = 1 - fo0
                    ElseIf i < varm.nCritVals - 1 Then
                        'the starting interpolation of the preceding basis * corresponding right side of lower order Hat
                        '+the starting right interpolation of preceding basis * corresponding left side of lower order Hat
                        ' a*q
                        ' +b*p
                        fo1 = xMciM1 / d2CVs(1, iM1) * (1 - xMci / dCVs(1, i)) _
                                    + (1 - xMci / d2CVs(1, i)) * xMci / dCVs(1, i)
                    End If
                    If i = 2 Then
                        'fo2 is a catch all where not defined
                        fo2 = 1 - fo0 - fo1
                    ElseIf i > 2 Then
                        'the remaining right interpolation of second preceding basis * corresponding right side of lower order Hat
                        ' b*q
                        fo2 = (1 - xMciM1 / d2CVs(1, iM1)) * (1 - xMci / dCVs(1, i))
                    End If
                    If i = varm.nCritVals - 1 Then
                        fo1 = 1 - fo2
                    End If
                    

                    If ia < varm.nArtVars Then
                        rc(r, ia) = fo0
                    Else
                        rc(r, varm.nArtVars) = fo0
                    End If
                    
                    If iaM1 > 1 Then
                        If iaM1 < varm.nArtVars Then
                            rc(r, iaM1) = fo1
                        Else
                            rc(r, varm.nArtVars) = rc(r, varm.nArtVars) + fo1
                        End If
                    End If
                    
                    If iaM2 > 1 Then
                        rc(r, iaM2) = rc(r, iaM2) + fo2
                    End If
                    
                ElseIf varm.Treatment = BSplineOrder3 Then
                    
                    
                    iM1 = i - 1
                    iM2 = i - 2
                    ia = i + 2
                    iaM1 = ia - 1
                    iaM2 = ia - 2
                    iaM3 = ia - 3
                    
                    xMci = (x - CVs(1, i))
                    xMciM1 = 0
                    xMciM2 = 0
                
                    If i = 3 Then
                    x = x
                    End If
                    If i > 1 Then
                        xMciM1 = (x - CVs(1, iM1))
                    End If
                    If i > 2 Then
                        xMciM2 = (x - CVs(1, iM2))
                    End If
                    
                    fo0 = 0
                    fo1 = 0
                    fo2 = 0
                    fo3 = 0
                    
                    If i < varm.nCritVals - 2 Then
                        ' u+v=1,a+b=1,p+q=1
                        ' u[0]*a[0]*p[0]
                        fo0 = xMci / d3CVs(1, i) * xMci / d2CVs(1, i) * xMci / dCVs(1, i)
                    End If
                    If i = 1 Then
                        fo1 = 1 - fo0
                    ElseIf i < varm.nCritVals - 2 Then
                        ' u[-1]*(a[-1]*q[0] + b*p[0])
                        '+v[0]*(a[0]*p[0])
                    
                        fo1 = (xMciM1 / d3CVs(1, iM1)) * (xMciM1 / d2CVs(1, iM1) * (1 - xMci / dCVs(1, i)) _
                                                        + (1 - xMci / d2CVs(1, i)) * (xMci / dCVs(1, i))) + _
                                (1 - xMci / d3CVs(1, i)) * (xMci / d2CVs(1, i) * xMci / dCVs(1, i))
                                                                                                                  
                    End If
                    If i = 2 Then
                        fo2 = 1 - fo0 - fo1
                    ElseIf i > 2 And i < varm.nCritVals - 1 Then
                        ' u[-2]*(b[-1]*q[0])
                        '+v[-1]*(a[-1]*q[0]+b[0]*p[0])
                        fo2 = (xMciM2 / d3CVs(1, iM2)) * ((1 - xMciM1 / d2CVs(1, iM1)) * (1 - xMci / dCVs(1, i))) _
                            + (1 - xMciM1 / d3CVs(1, iM1)) * (xMciM1 / d2CVs(1, iM1) * (1 - xMci / dCVs(1, i)) _
                                                         + (1 - xMci / d2CVs(1, i)) * (xMci / dCVs(1, i)))
                    End If
                    If i = 3 Then
                        fo3 = 1 - fo0 - fo1 - fo2
                    ElseIf i > 3 Then
                        ' v[-2]*b[-1]*p[0]
                        fo3 = (1 - xMciM2 / d3CVs(1, iM2)) * (1 - xMciM1 / d2CVs(1, iM1)) * (1 - xMci / dCVs(1, i))
                    End If
                    
                    
                    
                    If i = varm.nCritVals - 2 Then
                        fo1 = 1 - fo2 - fo3
                    End If
                    If i = varm.nCritVals - 1 Then
                        fo2 = 1 - fo3
                    End If
                    
                    If ia < varm.nArtVars Then
                        rc(r, ia) = fo0
                    Else
                        rc(r, varm.nArtVars) = fo0
                    End If
                    If iaM1 > 1 Then
                        If iaM1 < varm.nArtVars Then
                            rc(r, iaM1) = fo1
                        Else
                            rc(r, varm.nArtVars) = rc(r, varm.nArtVars) + fo1
                        End If
                    End If
                    If iaM2 > 1 Then
                        If iaM2 < varm.nArtVars Then
                            rc(r, iaM2) = fo2
                        Else
                            rc(r, varm.nArtVars) = rc(r, varm.nArtVars) + fo2
                        End If
                    End If
                    If iaM3 > 1 Then
                    If iaM3 < varm.nArtVars Then
                        rc(r, iaM3) = fo3
                    Else
                        rc(r, varm.nArtVars) = rc(r, varm.nArtVars) + fo3
                    End If
                    End If
                    
                    
                    
                End If
            End If
            End If
            End If
        
    Next
    
    fArtificials = rc

End Function

Private Function fArtificialsScoredLabels_MacroOptions_Array() As Variant
    fArtificialsScoredLabels_MacroOptions_Array = Array("fArtificialsScored" _
    , "Returns an array-value of score labels for an input range of coefficients" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("CoefficientValues", "A range of coefficients for which columns match the number of artificials and rows represent separate score sets"), _
    Array("VariableNameBase", "Optional, defaults to X") _
    ) _
    )
End Function

Function fArtificialsScoredLabels(ByRef CoefficientValues As Range, Optional VariableBase = "Score") As String()
    nArtVars = CoefficientValues.Rows.Count
    Dim rv
    ReDim rv(1 To nArtVars) As String
    For i = 1 To nArtVars
        rv(i) = VariableBase & i
    Next i
    fArtificialsScoredLabels = rv

End Function

Private Function fArtificialsScored_MacroOptions_Array() As Variant
    fArtificialsScored_MacroOptions_Array = Array("fArtificialsScored" _
    , "Returns an array-value of scored artificial variables for a range input given treatment and set of critical values" _
    , "http://WDataSci.com" _
    , "WDS" _
    , Array(Array("Treatment", "One of [None|Constant|CodedMissings|DiscreteLC|DiscreteRC|Hats|iHats|BSplineOrder2|BSO2|BSO3|Categorical|CategoricalNumeric] or an alias"), _
    Array("Input", "An input value or column"), _
    Array("CriticalValues", "A range of critical values"), _
    Array("CoefficientValues", "A range of coefficients for which columns match the number of artificials and rows represent separate score sets"), _
    Array("CleanLimitLeft", "Optional, left hand clean limit"), _
    Array("CleanLimitRight", "Optional, right hand clean limit") _
    ) _
    )
End Function


Function fArtificialsScored(ByVal Treatment As String _
    , ByRef InputValues As Range _
    , ByRef CriticalValues _
    , ByRef CoefficientValues _
    , Optional CleanLimitLeftVal = Empty _
    , Optional CleanLimitRightVal = Empty _
    , Optional eps = 0.00000001 _
    ) As Variant()
    
    On Error GoTo Hey
    Dim varm As tVariableMatter
    varm = fVariableMatter(Treatment, CriticalValues, Array(CleanLimitLeftVal, CleanLimitRightVal), CoefficientValues)
    
    Dim rc As Variant
    Dim lrc As Variant

    Dim Coef
    nScores = varm.nScores
    ReDim Coef(nScores, varm.nArtVars) As Double
    Coef = varm.CoefVals
    
    
    Dim CVs 'CriticalValues
    Dim dCVs 'for sequential differences
    Dim d2CVs 'for two-step sequential differences
    Dim d3CVs 'for three-step sequential differences
    Dim Cnstnt As Double
    
    If varm.Treatment = Categorical Or varm.Treatment = CategoricalNumeric Then
        ReDim CVs(1 To varm.nCritValRows, 1 To varm.nCritVals) As Variant
        CVs = varm.CritVals
    ElseIf varm.Treatment = Constant Then
        Cnstnt = varm.CritVals(1, 1)
    ElseIf Not (varm.Treatment = None Or varm.Treatment = Constant) Then
        n = varm.nCritVals
        
        ReDim CVs(1, 1 To n) As Double
        CVs = varm.CritVals
        
        If varm.Treatment = Hats Or varm.Treatment = iHats Or varm.Treatment = BSplineOrder2 Or varm.Treatment = BSplineOrder3 Then
            ReDim dCVs(1, 1 To varm.nCritVals - 1) As Double
            For i = 1 To varm.nCritVals - 1
                dCVs(1, i) = CVs(1, i + 1) - CVs(1, i)
            Next i
        End If
        If varm.Treatment = BSplineOrder2 Or varm.Treatment = BSplineOrder3 Or varm.Treatment = iHats Then
            ReDim d2CVs(1, 1 To varm.nCritVals - 1) As Double
            For i = 1 To varm.nCritVals - 2
                d2CVs(1, i) = CVs(1, i + 2) - CVs(1, i)
            Next i
        End If
        If varm.Treatment = BSplineOrder3 Then
            ReDim d3CVs(1, 1 To varm.nCritVals - 1) As Double
            For i = 1 To varm.nCritVals - 3
                d3CVs(1, i) = CVs(1, i + 3) - CVs(1, i)
            Next i
        End If
        
    End If
    
        
    nrows = Application.Caller.Rows.Count
    If nrows < InputValues.Rows.Count Then: nrows = InputValues.Rows.Count
    
    
    ReDim rc(nrows, nScores) As Variant
    
    
'    For i = 1 To nrows
'        For j = 1 To nscores
'            rc(i, j) = 0
'        Next
'    Next
    
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
        tempval = InputValues.Cells(r, 1).Value
        If varm.Treatment = None Then
            ia = 1
            For k = 1 To nScores
                rc(r, k) = Coef(k, ia) * tempval
            Next k
        ElseIf varm.Treatment = Constant Then
            ia = 1
            For k = 1 To nScores
                rc(r, k) = Coef(k, ia) * Cnstnt
            Next k
        ElseIf varm.Treatment = Categorical Or varm.Treatment = CategoricalNumeric Then
            found = False
            If IsError(tempval) Then
                found = True
                i = 0
            Else
                For i = 1 To varm.nCritVals
                    For j = 1 To varm.nCritValRows
                        If IsEmpty(CVs(j, i)) Then
                            Exit For
                        End If
                        If isTreatmentNumeric Then
                            If Abs(tempval - CVs(j, i)) < eps Then
                                found = True
                                Exit For
                            End If
                        Else
                            If tempval = CVs(j, i) Then
                                found = True
                                Exit For
                            End If
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
            For k = 1 To nScores
                rc(r, k) = Coef(k, ia)
            Next k
        Else
            bIsMissing = Not IsNumeric(tempval)
            If Not bIsMissing And varm.bUseCLLeft Then
                bIsMissing = tempval < varm.CleanLimits(1)
            End If
            If Not bIsMissing And varm.bUseCLRight Then
                bIsMissing = tempval > varm.CleanLimits(2)
            End If

            If bIsMissing Then
                ia = 1
                For k = 1 To nScores
                    rc(r, k) = Coef(k, ia)
                Next k
            Else

            'just to keep things communicable and relatable to usual mathematical discussion

            x = CDbl(tempval)

            If varm.Treatment = CodedMissings Then
                'simple case, missings have already been addressed
                i = 1
                ia = 2
                For k = 1 To nScores
                    rc(r, k) = Coef(k, 2) * x
                Next k
            ElseIf x <= CVs(1, 1) + eps Then
                'all non-missing first artificials are 1 left of the first critical value, except iHats and DiscreteRC
                i = 1
                ia = 2
                If varm.Treatment = iHats Then
                    tempdouble = x - CVs(1, 1)
                    For k = 1 To nScores
                        rc(r, k) = Coef(k, ia) * tempdouble
                    Next
                Else

                        If (varm.Treatment = DiscreteRC) And (x >= CVs(1, 1) - eps) Then

                            i = i + 1
                            ia = ia + 1

                        End If

                    For k = 1 To nScores
                        rc(r, k) = Coef(k, ia)
                    Next
                End If
            ElseIf x >= CVs(1, varm.nCritVals) - eps Then
                'all non-missing last artificials are 1 right of the last critical value, except iHats and DiscreteLC
                i = varm.nCritVals
                ia = varm.nArtVars
                If varm.Treatment = iHats Then
                    tempdouble = (x - CVs(1, i) + (CVs(1, i) - CVs(1, i - 1)) / 2)
                    For k = 1 To nScores
                        rc(r, k) = Coef(k, ia) * tempdouble
                    Next k
                    For k = 1 To nScores
                        For j = 2 To varm.nCritVals - 1
                            ia = j + 1
                            rc(r, k) = rc(r, k) + Coef(k, ia) * (CVs(1, j + 1) - CVs(1, j - 1)) / 2
                        Next
                        j = 1
                        ia = 2
                        rc(r, k) = rc(r, k) + Coef(k, ia) * (CVs(1, 2) - CVs(1, 1)) / 2
                    Next k
                Else
                
                        If (varm.Treatment = DiscreteLC) And (x <= CVs(1, varm.nCritVals) + eps) Then

                            i = i - 1
                            ia = ia - 1

                        End If

                    For k = 1 To nScores
                        rc(r, k) = Coef(k, ia)
                    Next k
                End If
            Else
                
                'main guts of the function.....
                
                'find the critical value interval.....
                If varm.Treatment = DiscreteLC Then
                    For i = varm.nCritVals - 1 To 1 Step -1
                        If x > CVs(1, i) + eps Then
                            Exit For
                        End If
                    Next i
                    'Discrete cases are shifted one
                    i = i + 1
                ElseIf varm.Treatment = DiscreteRC Then
                    For i = varm.nCritVals - 1 To 1 Step -1
                        If x > CVs(1, i) - eps Then
                            Exit For
                        End If
                    Next i
                    i = i + 1
                Else
                    For i = varm.nCritVals - 1 To 1 Step -1
                        If x >= CVs(1, i) Then
                            Exit For
                        End If
                    Next i
                End If
                
                'usual VBA index
                ia = i + 1
                If (varm.Treatment = DiscreteRC) Or (varm.Treatment = DiscreteLC) Then
                    For k = 1 To nScores
                        rc(r, k) = Coef(k, ia)
                    Next k
                ElseIf varm.Treatment = Hats Then
                    tempdouble = (x - CVs(1, i)) / dCVs(1, i)
                    For k = 1 To nScores
                        rc(r, k) = rc(r, k) + Coef(k, ia + 1) * tempdouble
                        rc(r, k) = rc(r, k) + Coef(k, ia) * (1 - tempdouble)
                    Next k
                ElseIf varm.Treatment = iHats Then
                    tempdouble = ((x - CVs(1, i)) ^ 2 / dCVs(1, i) / 2)

                        iaP1 = ia + 1
                        For k = 1 To nScores
                            rc(r, k) = rc(r, k) + Coef(k, iaP1) * tempdouble
                            rc(r, k) = rc(r, k) + Coef(k, ia) * (x - CVs(1, i) - tempdouble)
                        Next k
                        For ii = 1 To (i - 1)
                            iia = ii + 1
                            iiaP1 = iia + 1
                            For k = 1 To nScores
                                rc(r, k) = rc(r, k) + Coef(k, iiaP1) * dCVs(1, ii) / 2
                                rc(r, k) = rc(r, k) + Coef(k, iia) * dCVs(1, ii) / 2
                            Next k
                        Next ii
                
                ElseIf varm.Treatment = BSplineOrder2 Then
                    'first artificial is a left catch all, necessary through knot3
                    'i+2 is more akin to the "usual" index, mapped back to "option base 1" with 0 being the missing code variable
                    iM1 = i - 1
                    ia = i + 2
                    iaM1 = ia - 1
                    iaM2 = ia - 2
                    
                    xMci = (x - CVs(1, i))
                    xMciM1 = 0
                
                    If i > 1 Then
                        xMciM1 = (x - CVs(1, iM1))
                    End If
                                        
                    'the last artificial is a right catch all
                    'therefore, fo0, [f]unction [o]ffset [0], stops with CVs(1, varm.nCritVals-2)
                    'and fo1 is a catch all at CVs(1, varm.nCritVals-1)
                    
                    fo0 = 0
                    fo1 = 0
                    fo2 = 0
                    
                    If i < varm.nCritVals - 1 Then
                        fo0 = xMci / d2CVs(1, i) * xMci / dCVs(1, i)
                    End If
                    If i = 1 Then
                        'fo1 is a catch all where not defined
                        fo1 = 1 - fo0
                    ElseIf i < varm.nCritVals - 1 Then
                        fo1 = xMciM1 / d2CVs(1, iM1) * (1 - xMci / dCVs(1, i)) _
                                    + (1 - xMci / d2CVs(1, i)) * xMci / dCVs(1, i)
                    End If
                    If i = 2 Then
                        'fo2 is a catch all where not defined
                        fo2 = 1 - fo0 - fo1
                    ElseIf i > 2 Then
                        fo2 = (1 - xMciM1 / d2CVs(1, iM1)) * (1 - xMci / dCVs(1, i))
                    End If
                    If i = varm.nCritVals - 1 Then
                        fo1 = 1 - fo2
                    End If

                    ReDim lrc(1, varm.nArtVars) As Double
                    For i = 1 To varm.nArtVars
                        lrc(1, i) = 0
                    Next i
                    
                    If ia < varm.nArtVars Then
                        lrc(1, ia) = fo0
                    Else
                        lrc(1, varm.nArtVars) = fo0
                    End If
                    
                    If iaM1 > 1 Then
                        If iaM1 < varm.nArtVars Then
                            lrc(1, iaM1) = fo1
                        Else
                            lrc(1, varm.nArtVars) = lrc(1, varm.nArtVars) + fo1
                        End If
                    End If
                                     
                    If iaM2 > 1 Then
                        lrc(1, iaM2) = lrc(1, iaM2) + fo2
                    End If
                    
                    
                    For k = 1 To nScores
                        
                        For i = 1 To varm.nArtVars
                            rc(r, k) = rc(r, k) + Coef(k, i) * lrc(1, i)
                        Next i
                        
                    Next k
                    
                ElseIf varm.Treatment = BSplineOrder3 Then
                    
                    
                    iM1 = i - 1
                    iM2 = i - 2
                    ia = i + 2
                    iaM1 = ia - 1
                    iaM2 = ia - 2
                    iaM3 = ia - 3
                    
                    xMci = (x - CVs(1, i))
                    xMciM1 = 0
                    xMciM2 = 0
                
                    If i = 3 Then
                    x = x
                    End If
                    If i > 1 Then
                        xMciM1 = (x - CVs(1, iM1))
                    End If
                    If i > 2 Then
                        xMciM2 = (x - CVs(1, iM2))
                    End If
                    
                    fo0 = 0
                    fo1 = 0
                    fo2 = 0
                    fo3 = 0
                    
                    If i < varm.nCritVals - 2 Then
                        ' u+v=1,a+b=1,p+q=1
                        ' u[0]*a[0]*p[0]
                        fo0 = xMci / d3CVs(1, i) * xMci / d2CVs(1, i) * xMci / dCVs(1, i)
                    End If
                    If i = 1 Then
                        fo1 = 1 - fo0
                    ElseIf i < varm.nCritVals - 2 Then
                        ' u[-1]*(a[-1]*q[0] + b*p[0])
                        '+v[0]*(a[0]*p[0])
                    
                        fo1 = (xMciM1 / d3CVs(1, iM1)) * (xMciM1 / d2CVs(1, iM1) * (1 - xMci / dCVs(1, i)) _
                                                        + (1 - xMci / d2CVs(1, i)) * (xMci / dCVs(1, i))) + _
                                (1 - xMci / d3CVs(1, i)) * (xMci / d2CVs(1, i) * xMci / dCVs(1, i))
                                                                                                                  
                    End If
                    If i = 2 Then
                        fo2 = 1 - fo0 - fo1
                    ElseIf i > 2 And i < varm.nCritVals - 1 Then
                        ' u[-2]*(b[-1]*q[0])
                        '+v[-1]*(a[-1]*q[0]+b[0]*p[0])
                        fo2 = (xMciM2 / d3CVs(1, iM2)) * ((1 - xMciM1 / d2CVs(1, iM1)) * (1 - xMci / dCVs(1, i))) _
                            + (1 - xMciM1 / d3CVs(1, iM1)) * (xMciM1 / d2CVs(1, iM1) * (1 - xMci / dCVs(1, i)) _
                                                         + (1 - xMci / d2CVs(1, i)) * (xMci / dCVs(1, i)))
                    End If
                    If i = 3 Then
                        fo3 = 1 - fo0 - fo1 - fo2
                    ElseIf i > 3 Then
                        ' v[-2]*b[-1]*p[0]
                        fo3 = (1 - xMciM2 / d3CVs(1, iM2)) * (1 - xMciM1 / d2CVs(1, iM1)) * (1 - xMci / dCVs(1, i))
                    End If
                    
                    
                    
                    If i = varm.nCritVals - 2 Then
                        fo1 = 1 - fo2 - fo3
                    End If
                    If i = varm.nCritVals - 1 Then
                        fo2 = 1 - fo3
                    End If
                    
                    ReDim lrc(1, varm.nArtVars) As Double
                    For i = 1 To varm.nArtVars
                        lrc(1, i) = 0
                    Next i

                    If ia < varm.nArtVars Then
                        lrc(1, ia) = fo0
                    Else
                        lrc(1, varm.nArtVars) = fo0
                    End If
                    If iaM1 > 1 Then
                        If iaM1 < varm.nArtVars Then
                            lrc(1, iaM1) = fo1
                        Else
                            lrc(1, varm.nArtVars) = lrc(1, varm.nArtVars) + fo1
                        End If
                    End If
                    If iaM2 > 1 Then
                        If iaM2 < varm.nArtVars Then
                            lrc(1, iaM2) = fo2
                        Else
                            lrc(1, varm.nArtVars) = lrc(1, varm.nArtVars) + fo2
                        End If
                    End If
                    If iaM3 > 1 Then
                    If iaM3 < varm.nArtVars Then
                        lrc(1, iaM3) = fo3
                    Else
                        lrc(1, varm.nArtVars) = lrc(1, varm.nArtVars) + fo3
                    End If
                    End If

                    For k = 1 To nScores
                        
                        For i = 1 To varm.nArtVars
                            rc(r, k) = rc(r, k) + Coef(k, i) * lrc(1, i)
                        Next i
                        
                    Next k
                    
                    
                End If
            End If
        End If
        End If
    Next
    
    fArtificialsScored = rc

Hey:
    x = 1
End Function


Public Function WDSArtificials_CallMacroOptions_Arrays() As Variant

    Dim rv As Variant
    
    ReDim rv(1 To 100) As Variant
    Dim i As Integer
    i = 0
    i = i + 1
    rv(i) = fArtificialsCount_MacroOptions_Array()
    i = i + 1
    rv(i) = fArtificialsLabels_MacroOptions_Array()
    i = i + 1
    rv(i) = fArtificials_MacroOptions_Array()
    i = i + 1
    rv(i) = fArtificialsKV_MacroOptions_Array()
    i = i + 1
    rv(i) = fArtificialsScored_MacroOptions_Array()
    i = i + 1
    rv(i) = fArtificialsScoredLabels_MacroOptions_Array()
    i = i + 1
    rv(i) = rv(i - 1)
    rv(i)(1) = "Stop"

    WDSArtificials_CallMacroOptions_Arrays = rv

End Function

