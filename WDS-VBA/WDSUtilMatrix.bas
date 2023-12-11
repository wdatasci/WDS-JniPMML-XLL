Attribute VB_Name = "WDSUtilMatrix"
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
Function RowNormX(arg, ByRef Topology As Range)

Dim rv As Variant
Dim startrow, stoprow, startcol, stopcol, nrows, ncols, i, j As Integer
Dim s As Double

If TypeOf arg Is Range Then

startrow = 1
stoprow = arg.Rows.Count
startcol = 1
stopcol = arg.Columns.Count

ReDim rv(startrow To stoprow, startcol To stopcol) As Double

For i = startrow To stoprow
    s = 0
    For j = startcol To stopcol
    If Topology(i, j).Value = 1 Then
       s = s + arg(i, j).Value
    End If
    Next
    If s <> 0 Then
        For j = startcol To stopcol
        If Topology(i, j).Value = 1 Then
            rv(i, j) = Val(arg(i, j).Value) / s
        End If
        Next j
    Else
        For j = startcol To stopcol
            rv(i, j) = 0
        Next j
    End If
Next i
 
Else

startrow = LBound(arg, 1)
stoprow = UBound(arg, 1)
startcol = LBound(arg, 2)
stopcol = UBound(arg, 2)

ReDim rv(startrow To stoprow, startcol To stopcol) As Double



For i = startrow To stoprow
    s = 0
    For j = startcol To stopcol
        If Topology(i, j).Value = 1 Then
            s = s + arg(i, j)
        End If
    Next
    If s <> 0 Then
        For j = startcol To stopcol
            If Topology(i, j).Value = 1 Then
                rv(i, j) = Val(arg(i, j)) / s
            End If
        Next j
    Else
        For j = startcol To stopcol
            rv(i, j) = 0
        Next j
    End If
Next i

End If
RowNormX = rv


End Function


Function NormedBaseOdds(ofset, ByRef BaseOdds As Range, ByRef Topology As Range)

Dim base As Range
Set base = BaseOdds.Offset(ofset * BaseOdds.Rows.Count, 0)
NormedBaseOdds = RowNormX(base, Topology)




End Function

Function ScoredAndNormedBaseOdds(i, mtm, ByRef ofset As Range, ByRef BaseOdds As Range, ByRef Topology As Range, ByRef ijs As Range, ByRef vs As Range)

Dim j, ii, jj As Integer

basev = BaseOdds.Offset(ofset(i, 1).Value * BaseOdds.Rows.Count, 0).Value

If mtm <= 6 Then
    For ii = 2 To vs.Columns.Count
        For jj = 1 To ii - 1
            basev(ii, jj) = 0
        Next jj
    Next ii
If mtm <= 3 Then
    For ii = 2 To vs.Columns.Count - 6
        If Topology(ii, ii).Value = 1 Then
                basev(ii, ii) = basev(ii, ii) * 0.1
        End If
        For jj = ii + 1 To BaseOdds.Columns.Count
            If Topology(ii, jj).Value = 1 Then
                    basev(ii, jj) = basev(ii, jj) * 10
            End If
        Next jj
    Next ii
End If
End If

For j = 1 To vs.Columns.Count
    If IsEmpty(vs(i, j)) Then GoTo BreakNextj
    ii = ijs(1, j).Value
    jj = ijs(2, j).Value
    If Topology(ii, jj).Value = 1 Then
            basev(ii, jj) = basev(ii, jj) * Exp(vs(i, j).Value)
    End If
Next j
BreakNextj:

ScoredAndNormedBaseOdds = RowNormX(basev, Topology)


End Function


Function MatrixMult(m1, m2) As Variant


lm1 = deRange(m1)
lm2 = deRange(m2)



Dim rv

If lm1(3) <> lm2(2) Then
    rv = Error()
Else
    ReDim rv(1 To lm1(2), 1 To lm2(3))
    For i = 1 To lm1(2)
        For j = 1 To lm2(3)
            s = 0
            For k = 1 To lm1(3)
               s = s + lm1(1)(i, k) * lm2(1)(k, j)
            Next k
            rv(i, j) = s
        Next j
    Next i
End If

MatrixMult = rv

End Function

Function mPanelData(ByRef arg As Range, ofset As Long) As Variant

nrows = arg.Rows.Count

Dim rv As Variant
ReDim rv(1 To nrows, 1 To 6) As Long




For i = 1 To nrows
    iM1 = i - 1
    If i = 1 Or arg(i, 1) <> 0 Then
        rv(i, 1) = 1
        rv(i, 3) = 1
        rv(i, 5) = i + ofset
    Else
        rv(i, 1) = 0
        rv(i, 3) = rv(iM1, 3) + 1
        rv(i, 5) = rv(iM1, 5)
    End If
Next i
For i = nrows To 1 Step -1
    iP1 = i + 1
    If i = nrows Then
        rv(i, 2) = 1
        rv(i, 4) = 1
        rv(i, 6) = i + ofset
    ElseIf rv(iP1, 1) <> 0 Then
        rv(i, 2) = 1
        rv(i, 4) = 1
        rv(i, 6) = i + ofset
    Else
        rv(i, 2) = 0
        rv(i, 4) = rv(iP1, 4) + 1
        rv(i, 6) = rv(iP1, 6)
    End If
Next i


mPanelData = rv



End Function

Sub subPanelize()

calc_at_start = Application.Calculation
On Error GoTo ElseIt

Application.Calculation = xlCalculationManual
Application.ScreenUpdating = False

Dim r As Range
Dim r2 As Range
Set r = Selection

Range(r.Offset(1, 0), r.Offset(r.SpecialCells(xlCellTypeLastCell).Row - r.Row, 0)).Clear

r.Copy
For Each r2 In Range(r.Offset(1, -6), r.Offset(1, -6).End(xlDown))

    If r2.Value <> 0 And r2.Offset(0, 3) <> 1 Then
        
        r2.Offset(0, 6).PasteSpecial (xlPasteFormulas)
    End If
    
Next
        
ElseIt:
Application.Calculation = calc_at_start
Application.ScreenUpdating = True


End Sub

Sub test1()

Dim x(1 To 3, 1 To 3) As Double

For i = 1 To 3
For j = 1 To 3
    x(i, j) = ActiveSheet.Cells(24 + i, 8 + j).Value
Next j
Next i
ActiveSheet.Range("E1:G3") = x
ActiveSheet.Range("E5:G7") = x

y = RowNormX(x, ActiveSheet.Range("E1:G3"))

ActiveSheet.Range("I1:K3") = y


End Sub


'simple test for up to 3 dims
Private Function zfNDims(ByRef arg As Variant) As Variant
    On Error GoTo CatchIt
TryIt:
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

Function deRange(arg) As Variant
Dim rv(1 To 3) As Variant
If TypeOf arg Is Range Then
    rv(1) = arg.Value
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
deRange = rv
End Function

Private Function mget(arg, i, j) As Variant

Dim rv
If arg(2) = 0 Then
    If arg(3) = 0 Then
        rv = arg(1)
    Else
        rv = arg(1)(i)
    End If
Else
    If arg(3) = 0 Then
        rv = arg(1)(i, 1)
    Else
        rv = arg(1)(i, j)
    End If
End If

mget = rv
End Function


Function RFScheduled(PanelInd, LoanAgeMos, PrinBal, IntRatePct, TermMos, Optional PmtAmt = Nothing, Optional stoprow = 0)


lPanelInd = deRange(PanelInd)
lLoanAgeMos = deRange(LoanAgeMos)
lPrinBal = deRange(PrinBal)
lIntRatePct = deRange(IntRatePct)
lTermMos = deRange(TermMos)

Dim rv As Variant
Dim startrow, startcol, stopcol, nrows, ncols, i, j As Integer
Dim s, p, pp, pi As Double

ir = mget(lIntRatePct, 1, 1)
If ir > 1 Then
    ir = ir / 1200
Else
    ir = ir / 12
End If

age = mget(lLoanAgeMos, 1, 1)
bal = mget(lPrinBal, 1, 1)
term = mget(lTermMos, 1, 1)

If PmtAmt Is Nothing Then
    If age = 0 Then
        p = Application.WorksheetFunction.Pmt(ir, term, bal)
    Else
        p = Application.WorksheetFunction.Pmt(ir, term - age, bal)
    End If
Else
    lPmtAmt = deRange(PmtAmt)
    p = mget(lPmtAmt, 1, 1)
End If


startrow = 1
If stoprow = 0 Then
    stoprow = lLoanAgeMos(2)
End If
startcol = 1
stopcol = 4

ReDim rv(startrow To stoprow, startcol To stopcol) As Double

rv(startrow, 1) = bal



For i = startrow + 1 To stoprow
    pind = mget(lPanelInd, i, 1)
    On Error GoTo CatchIt
    If pind <> 0 Then
        If i < stoprow Then
            ir = mget(lIntRatePct, i, 1)
            If ir > 1 Then
                ir = ir / 1200
            Else
                ir = ir / 12
            End If
            age = mget(lLoanAgeMos, i, 1)
            bal = mget(lPrinBal, i, 1)
            term = mget(lTermMos, i, 1)
            If PmtAmt Is Nothing Then
                If age = 0 Then
                    p = Application.WorksheetFunction.Pmt(ir, term, bal)
                Else
                    p = Application.WorksheetFunction.Pmt(ir, term - age, bal)
                End If
            Else
                p = mget(lPmtAmt, i, 1)
            End If
        End If
        rv(i, 1) = bal
    Else
        If rv(i - 1, 1) <= 0.0001 Then
            For j = startcol To stopcol
                rv(i, j) = 0
            Next j
        Else
            pi = -ir * rv(i - 1, 1)
            pp = p - pi
            If -pp > rv(i - 1, 1) Then
                pp = -rv(i - 1, 1)
                p = pi + pp
            End If
            rv(i, 4) = p
            rv(i, 3) = pi
            rv(i, 2) = pp
            rv(i, 1) = rv(i - 1, 1) + pp
        End If
    End If


    GoTo ElseIt
    
CatchIt:
    For j = startcol To stopcol
        rv(i, j) = -1
    Next j


ElseIt:
    
Next i
On Error GoTo 0

RFScheduled = rv


End Function

Sub RollRow(i, age, mtm, panelindex, rv, m, lNDist, lPrinBalDist, lRFSched, lpmtprin, lpmtint)

    Dim lv, lvp, lvi
    ReDim lv(1 To 1, 1 To lPrinBalDist(3)) As Double
    ReDim lvp(1 To 1, 1 To lPrinBalDist(3)) As Double
    ReDim lvi(1 To 1, 1 To lPrinBalDist(3)) As Double
    ReDim lvpinc(1 To 1, 1 To lPrinBalDist(3)) As Double
    ReDim lviinc(1 To 1, 1 To lPrinBalDist(3)) As Double
    
        For j = 1 To lNDist(3)
        On Error GoTo CatchIt
            s = 0
            sp = 0
            sa = 0
            sainc = 0
            sb = 0
            sbinc = 0
            For k = 1 To lNDist(3)
                p = rv(i - 1, k) * m(k, j)
                If p > 0 Then
                    s = s + p
                    sp = sp + rv(i - 1, lNDist(3) + k) * m(k, j)
                    sa = sa + lpmtprin(1, k) * m(k, j)
                    sb = sb + lpmtint(1, k) * m(k, j)
                    If j <= 2 And k <= j Then
                        sainc = sainc - p * mget(lRFSched, i, 2)
                        sbinc = sbinc - p * mget(lRFSched, i, 3)
                    ElseIf j <= 5 And (k - 2) < age And (k - 2) < panelindex Then
                        For kk = j To k
                            If i - (kk - 2) < 1 Then
                                panelindex = panelindex
                            End If
                            sainc = sainc - p * mget(lRFSched, i - (kk - 2), 2)
                            sbinc = sbinc - p * mget(lRFSched, i - (kk - 2), 3)
                        Next kk
                    End If
                End If
            Next k
            rv(i, j) = s
            lv(1, j) = sp
            If sainc > sp Then
                sbinc = sbinc * sp / sainc
                sainc = sp
            End If
            If j <= 2 And mtm <= 1 And sp - sainc > 0 Then
                sainc = sp
            End If
            lpmtprin(1, j) = sa + sainc
            lvpinc(1, j) = sainc
            lpmtint(1, j) = sb + sbinc
            lviinc(1, j) = sbinc
            GoTo ElseIt
CatchIt:
            panelindex = panelindex
ElseIt:
        Next j
        On Error GoTo 0
        
        For j = 1 To lNDist(3)
            rv(i, j + lNDist(3)) = lv(1, j) - lvpinc(1, j)
            rv(i, j + lNDist(3) + lPrinBalDist(3)) = lvpinc(1, j)
            rv(i, j + lNDist(3) + 2 * lPrinBalDist(3)) = lviinc(1, j)
        Next j
            
        

End Sub

Function RollIt(PanelInd, LoanAgeMos, WAM, RFSched, NDist, PrinBalDist, ByRef ofset As Range, ByRef BaseOdds As Range, ByRef Topology As Range, Optional ByRef ijs As Range, Optional ByRef vs As Range)


lPanelInd = deRange(PanelInd)
lLoanAgeMos = deRange(LoanAgeMos)
lWAM = deRange(WAM)
lRFSched = deRange(RFSched)
lNDist = deRange(NDist)
lPrinBalDist = deRange(PrinBalDist)
loffset = deRange(ofset)


Dim rv As Variant
Dim startrow, stoprow, startcol, stopcol, nrows, ncols, i, j, panelindex As Integer
Dim s, s1, s2 As Double



startrow = 1
stoprow = lLoanAgeMos(2)
startcol = 1
stopcol = lNDist(3) + lPrinBalDist(3) * 3 + 6



ReDim rv(startrow To stoprow, startcol To stopcol) As Double

For j = 1 To lNDist(3)
    rv(startrow, j) = mget(lNDist, 1, j)
Next j
For j = 1 To lPrinBalDist(3)
    rv(startrow, j + lNDist(3)) = mget(lPrinBalDist, 1, j)
Next j
panelindex = 1

s1 = 0
s2 = 0
For j = 1 To lNDist(3) - 6
    s1 = s1 + rv(startrow, j)
    s2 = s2 + rv(startrow, j + lNDist(3))
Next j
rv(startrow, stopcol - 5) = s1
rv(startrow, stopcol - 2) = s2


Dim lpmtprin
Dim lpmtint
ReDim lpmtprin(1 To 1, 1 To lPrinBalDist(3))
ReDim lpmtint(1 To 1, 1 To lPrinBalDist(3))



For i = startrow + 1 To stoprow
On Error GoTo CatchIt
    pind = mget(lPanelInd, i, 1)
    If pind > 0 Then
          
        For j = 1 To lNDist(3)
            rv(i, j) = mget(lNDist, i, j)
        Next j
        For j = 1 To lPrinBalDist(3)
            rv(i, j + lNDist(3)) = mget(lPrinBalDist, i, j)
        Next j
        panelindex = 1
    Else
        age = mget(lLoanAgeMos, i, 1)
        mtm = mget(lWAM, i, 1)
        panelindex = panelindex + 1
        rv(i, stopcol - 3) = rv(i - 1, stopcol - 3) + rv(i - 1, stopcol - 4)
        rv(i, stopcol) = rv(i - 1, stopcol) + rv(i - 1, stopcol - 1)
        If panelindex > 2 And (rv(i - 1, stopcol - 5) <= 0.000001 Or rv(i - 1, stopcol - 2) <= 0.005) Then
            rv(i, stopcol - 4) = rv(i - 1, stopcol - 5)
            rv(i, stopcol - 1) = rv(i - 1, stopcol - 2)
        Else
            If ijs Is Nothing Then
                m = NormedBaseOdds(mget(loffset, i, 1), BaseOdds, Topology)
            Else
                m = ScoredAndNormedBaseOdds(i, mtm, ofset, BaseOdds, Topology, ijs, vs)
            End If
            Call RollRow(i, age, mtm, panelindex, rv, m, lNDist, lPrinBalDist, lRFSched, lpmtprin, lpmtint)
            s1 = 0
            s2 = 0
            For j = 1 To lNDist(3) - 6
                s1 = s1 + rv(i, j)
                s2 = s2 + rv(i, j + lNDist(3))
            Next j
            rv(i, stopcol - 5) = s1
            rv(i, stopcol - 2) = s2
        End If
    End If
    GoTo ElseIt
CatchIt:
    panelindex = panelindex
ElseIt:
Next i
 

RollIt = rv


End Function




