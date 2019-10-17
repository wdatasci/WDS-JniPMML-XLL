Attribute VB_Name = "WDSUtilTumblers"
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


Function StringSplit(ByVal arg As String, Optional dlm = ",", Optional transp = 0) As Variant
    Dim rv As Variant
    rv = Split(arg, dlm)
    Dim rv2 As Variant
    Dim n, n0 As Integer
    n0 = LBound(rv)
    n = UBound(rv) - n0 + 1
    Dim i As Integer
    If transp = 0 Then
        ReDim rv2(1 To n) As Variant
        For i = 1 To n
            rv2(i) = Trim(rv(i - 1 + n0))
        Next i
    Else
        ReDim rv2(1 To n, 1) As Variant
        For i = 1 To n
            rv2(i, 1) = Trim(rv(i - 1 + n0))
        Next i
    End If
    StringSplit = rv2
End Function
Private Sub TumblersInc(ByRef tops, ByRef V, ByRef s, ByVal n As Long)
Dim i As Long
If V(1) = 0 Then
    For i = 1 To n
        V(i) = 1
    Next i
    s = False
Else
    V(n) = V(n) + 1
    If V(n) < tops(n) Then Exit Sub
    i = n
    While i > 1 And V(i) > tops(i)
        V(i - 1) = V(i - 1) + 1
        V(i) = 1
        i = i - 1
    Wend
    s = V(1) >= tops(1)
End If
End Sub
Private Sub TumblersIncDim(ByRef tops, ByRef V, ByRef s, ByVal n As Long, ByVal k As Long)
Dim i As Long
If V(1) = 0 Then
    For i = 1 To n
        V(i) = 1
    Next i
    s = False
Else
    If k < 1 Or k > n Then
        s = True
        For i = 1 To n
            V(i) = tops(i)
        Next i
        Exit Sub
    End If
    V(k) = V(k) + 1
    For i = k + 1 To n
        V(i) = 1
    Next i
    i = k
    While i > 1 And V(i) > tops(i)
        V(i - 1) = V(i - 1) + 1
        V(i) = 1
        i = i - 1
    Wend
    s = V(1) >= tops(1)
End If
End Sub

Function TumblersN(ParamArray args()) As Long
    t = 1
    For Each n In args
        t = t * n
    Next n
    TumblersN = t
End Function

Function Tumblers(ParamArray args()) As Variant
    Dim tops
    Dim V
    Dim i, j, t, n As Long
    n = UBound(args) - LBound(args) + 1
    ReDim tops(1 To n) As Long
    ReDim V(1 To n) As Long
    t = 1
    For i = 1 To n
        tops(i) = args(i - 1 + LBound(args))
        t = t * tops(i)
    Next i
    Dim stp As Boolean
    Dim rv As Variant
    ReDim rv(1 To t, 1 To n) As Long
    For i = 1 To t
        Call TumblersInc(tops, V, stp, n)
        For j = 1 To n
            rv(i, j) = V(j)
        Next j
    Next i
    Tumblers = rv
End Function

Function CrossProdEnum(ParamArray args()) As Variant
    Dim tops
    Dim V
    Dim i, j, t, n As Long
    n = UBound(args) - LBound(args) + 1
    ReDim tops(1 To n) As Long
    ReDim V(1 To n) As Long
    t = 1
    For i = 1 To n
        If TypeOf args(i - 1 + LBound(args)) Is Range Then
            Set r = args(i - 1 + LBound(args))
            tops(i) = r.Cells.Count 'Len(args(i - LBound(args) + 1))
        Else
            tops(i) = UBound(args) - LBound(args) + 1
        End If
        t = t * tops(i)
    Next i
    Dim stp As Boolean
    Dim rv As Variant
    ReDim rv(1 To t, 1 To n) As Long
    For i = 1 To t
        Call TumblersInc(tops, V, stp, n)
        For j = 1 To n
        rv(i, j) = V(j)
        Next j
    Next i
    CrossProdEnum = rv
End Function

Function CrossProdEnumLong(ParamArray args()) As Variant
    Dim tops
    Dim V
    Dim i, j, t, n As Long
    n = UBound(args) - LBound(args) + 1
    ReDim tops(1 To n) As Long
    ReDim V(1 To n) As Long
    Dim b
    ReDim b(1 To n) As Boolean
    Dim vargs
    ReDim vargs(1 To n) As Variant
    t = 1
    For i = 1 To n
        If TypeOf args(i - 1 + LBound(args)) Is Range Then
            Set r = args(i - 1 + LBound(args))
            vargs(i) = r.Value
            tops(i) = r.Cells.Count 'Len(args(i - LBound(args) + 1))
            b(i) = True
        Else
            tops(i) = UBound(args(i - 1 + LBound(args))) - LBound(args(i - 1 + LBound(args))) + 1
            b(i) = False
        End If
        t = t * tops(i)
    Next i
    Dim stp As Boolean
    Dim rv As Variant
    ReDim rv(1 To t, 1 To 2 + 2 * n) As Variant
    For i = 1 To t
        rv(i, 1) = t
        rv(i, 2) = i - 1
        Call TumblersInc(tops, V, stp, n)
        For j = 1 To n
            If b(j) Then
                rv(i, 2 + j) = vargs(j)(V(j), 1)
            Else
                rv(i, 2 + j) = args(j - 1 + LBound(args))(V(j))
            End If
            rv(i, 2 + n + j) = V(j)
        Next j
    Next i
    CrossProdEnumLong = rv
End Function


