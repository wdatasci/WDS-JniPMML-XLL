Attribute VB_Name = "WDSNelderMead"
Option Base 1

Sub wds_NM_SetUpFromSelected()

    calcprior = Application.Calculation
    On Error GoTo CatchIt
    Application.Calculation = xlCalculationManual
    
    Dim x As Range
    Set x = Selection
    
    Dim tws As Worksheet
    Dim nws As Worksheet
    
    Set tws = ActiveSheet
    
    
    
    If fIsASheetName("WDSNelderMead", x.Areas(1).Cells(1, 1)) Then
    
        inp = MsgBox("Clear Sheet WDSNelderMead?", vbQuestion + vbYesNo + vbDefaultButton1, "Sheet: WDSNelderMead Exists")
        
        If inp = "No" Then
            Exit Sub
        End If
        
        Set nws = tws.Parent.Sheets("WDSNelderMead")
    
    Else
    
        Call ActivateOrAddSheet("WDSNelderMead")
        Set nws = ActiveSheet
    
    End If
    
    nws.Cells.Clear
    
    i = 0
    i = i + 1
    nws.Cells(i, 1) = "WDS Nelder Mead Simplex Data"
    
    i = i + 1
    nws.Cells(i, 1) = "Init +/- Delta Mult"
    nws.Cells(i, 2) = "Terminal Obj Eps"
    nws.Cells(i, 3) = "Terminal Var Eps"
    nws.Cells(i, 4) = "Eval Count Limit"
    nws.Cells(i, 5) = "Expansion Step Limit"
    i = i + 1
    nws.Cells(i, 1) = 0.1
    nws.Cells(i, 2) = 0.0001
    nws.Cells(i, 3) = 0.0001
    nws.Cells(i, 4) = 100
    nws.Cells(i, 5) = 10
    
    i = i + 1
    nws.Cells(i, 1) = "Target"
    nws.Cells(i, 5) = "Best"
    nws.Cells(i, 6) = "Evaluations"
    i = i + 1
    nws.Cells(i, 1) = "Cell At SetUp"
    nws.Cells(i, 2) = "Cell"
    nws.Cells(i, 3) = "Value"
    i = i + 1
    'nws.Cells(i, 1) = "''" & tws.Name & "'!" & x.Areas(1).Cells(1, 1).Address
    nws.Cells(i, 3).FormulaR1C1 = "='" & tws.Name & "'!" & x.Areas(1).Cells(1, 1).Address(1, 1, xlR1C1)
    nws.Cells(i, 2) = "=fNVFormula(RC[1])"
    nws.Cells(i, 1) = fNVFormula(nws.Cells(i, 3))
    
    i = i + 1
    nws.Cells(i, 1) = "Variables"
    i = i + 1
    nws.Cells(i, 1) = "Cell At SetUp"
    nws.Cells(i, 2) = "Cell"
    nws.Cells(i, 3) = "Value"
    
    j = 0
    For Each a In x.Areas
        For Each c In a.Cells
            j = j + 1
            If j = 1 Then
            
            Else
                nws.Cells(i - 1, 2) = j - 1
                'nws.Cells(i + j - 1, 1) = "''" & tws.Name & "'!" & c.Address
                nws.Cells(i + j - 1, 2).FormulaR1C1 = "=fNVFormula(RC[1])"
                nws.Cells(i + j - 1, 3).Formula = "='" & tws.Name & "'!" & c.Address
                nws.Cells(i + j - 1, 1) = fNVFormula(nws.Cells(i + j - 1, 3))
            End If
        Next c
    Next a
            
    nws.Columns.AutoFit

CatchIt:

ElseIt:
    Application.Calculation = calcprior


End Sub

Sub wds_NM_Run()

    calcprior = Application.Calculation
    On Error GoTo CatchIt
    Application.Calculation = xlCalculationManual
    
    Dim tws As Worksheet
    Dim nws As Worksheet
    
    If Not fIsASheetName("WDSNelderMead", ActiveSheet.Cells(1, 1)) Then
    
        MsgBox ("WDSNelderMead sheet does not exist, run wds_NM_SetUp")
        Exit Sub
            
    Else
    
        Set nws = ActiveWorkbook.Sheets("WDSNelderMead")
    
    End If
    
    Dim target As Range
    Set target = Range(Mid(nws.Cells(6, 3).Formula, 2))
    Set tws = target.Parent
    
    Range(nws.Cells(5, 5), nws.Range("E5").SpecialCells(xlCellTypeLastCell)).Clear
    
    
    nv = nws.Cells(7, 2).Value
    
    Dim v() As Range
    ReDim v(1 To nv)
    
    Dim nwsr As Range
    Dim nwsrb As Range
    Dim nwsrbi As Range
    Set nwsrbi = nws.Cells(5, 5)
    Set nwsr = Range(nws.Cells(6, 3), nws.Cells(8 + nv, 3))
    Set nwsrb = Range(nws.Cells(6, 5), nws.Cells(8 + nv, 5))
    
    
    
    Dim xi() As Integer
    ReDim xi(1 To nv + 1)
    
    Dim x() As Double
    ReDim x(1 To nv, 1 To nv + 1)
    
    Dim y() As Double
    ReDim y(1 To nv + 1)
    
    itn = 1
    itb = 1
    iib = 1
    iiw = 1
    itw = 1
    iiw2 = 1
    itw2 = 1
    
    
    Dim b As Double
    Dim w As Double
    Dim w2 As Double
    
    y(1) = target
    b = target
    w = target
    w2 = target
    xi(1) = itn
    For i = 1 To nv
        Set v(i) = Range(Mid(nws.Cells(8 + i, 3).Formula, 2))
        x(i, itn) = v(i)
    Next i
    
    
    nwsrbi = itn
    nwsrbi.Offset(0, itn) = itn
    nwsrb.Offset(0, itn) = nwsr.Value
    nwsrbi.Offset(3, itn) = "Init"
    
    
    Dim d As Double
    Dim s As Double
    
    For ii = 1 To nv
        
        For i = 1 To nv
            v(i) = x(i, 1)
        Next i
        
        d = Abs(x(ii, 1))
        If d < 0.0001 Then
            d = 1
        End If
        v(ii) = x(ii, 1) + d * nws.Cells(3, 1)
        
        itn = itn + 1
        nwsrbi.Offset(0, -1) = itn
        Application.Calculate
        nwsrbi.Offset(0, itn) = itn
        nwsrb.Offset(0, itn) = nwsr.Value
        nwsrbi.Offset(3, itn) = "Init+"
        
        y(ii + 1) = target
        xi(ii + 1) = itn
        For i = 1 To nv
            x(i, ii + 1) = v(i)
        Next i
         
        v(ii) = x(ii, 1) - d * nws.Cells(3, 1)
        
        itn = itn + 1
        nwsrbi.Offset(0, -1) = itn
        Application.Calculate
        nwsrbi.Offset(0, itn) = itn
        nwsrb.Offset(0, itn) = nwsr.Value
        nwsrbi.Offset(3, itn) = "Init-"
        
        If target > y(ii + 1) Then
            nwsrbi.Offset(3, itn) = "Init-X"
            y(ii + 1) = target
            xi(ii + 1) = itn
            For i = 1 To nv
                x(i, ii + 1) = v(i)
            Next i
        Else
            nwsrbi.Offset(3, itn - 1) = "Init+X"
        End If
    
    Next ii
            
    Dim dv() As Double
    ReDim dv(1 To nv)
    Dim cv() As Double
    ReDim cv(1 To nv)
    
    For ii = 2 To nv + 1
        If y(ii) > b Then
            b = y(ii)
            iib = ii
            itb = xi(ii)
            nwsrbi = itb
            nwsrb = nwsrb.Offset(0, itb).Value
        End If
        If y(ii) <= w Then
            w = y(ii)
            iiw = ii
        End If
    Next ii
    w2 = b
    For i = 1 To nv + 1
        If i <> iiw And y(i) < w2 Then
            w2 = y(i)
            iiw2 = i
        End If
    Next i
        
    
    d = b - w
    s = 0
    For i = 1 To nv
        s = s + Abs(x(i, iib) - x(i, iiw))
    Next i
        
    nwsrbi.Offset(2, 0) = s
        
    Dim itn_top As Integer
    itn_top = nws.Cells(3, 4)
    
    If itn >= itn_top Then
        itn_top = (Int(itn / nws.Cells(3, 4)) + 1) * nws.Cells(3, 4)
        nwsrbi.Offset(1, -1) = itn_top
    End If
        
    While d > nws.Cells(3, 2) And s > nws.Cells(3, 3) And itn < itn_top
        
        nwsrbi.Offset(0, -1) = itn
    
        Dim m As Double
        
        For i = 1 To nv
            s = 0
            For j = 1 To nv + 1
                If j <> iiw Then
                    s = s + x(i, j)
                End If
            Next j
            cv(i) = s / nv
            dv(i) = cv(i) - x(i, iiw)
        Next i
        
        keepgoing = True
        found = False
        itf = 0
        
        Dim steps As Double
        steps = 0
        
        m = y(iiw)
        ml1 = m
        itm = 0
        
        While keepgoing And steps < nws.Cells(3, 5)
            ml1 = m
            itn = itn + 1
            steps = steps + 1
            For i = 1 To nv
                v(i) = cv(i) + steps * dv(i)
            Next i
            nwsrbi.Offset(0, -1) = itn
            Application.Calculate
            nwsrbi.Offset(0, itn) = itn
            nwsrb.Offset(0, itn) = nwsr.Value
            nwsrbi.Offset(3, itn) = "Reflect"
'            keepgoing = (target > m)
            m = target
            keepgoing = (target > y(iiw2)) And (m > ml1)
            If keepgoing Then
                itm = itn
            End If
'            keepgoing = (target > b) And (m > ml1)
        Wend
            
        If itm > 0 Then
            
            nwsrbi.Offset(3, itm) = "Reflect-X"
            For ii = itm + 1 To itn
                nwsrbi.Offset(3, ii) = "Reflect-E"
            Next ii
            
            y(iiw) = nwsrbi.Offset(1, itm)
            xi(iiw) = itm
            
            For i = 1 To nv
                x(i, iiw) = nwsrbi.Offset(3 + i, itm)
            Next i
            
        Else
        
            itn = itn + 1
            For i = 1 To nv
                v(i) = cv(i) + 0.5 * dv(i)
            Next i
            nwsrbi.Offset(0, -1) = itn
            Application.Calculate
            nwsrbi.Offset(0, itn) = itn
            nwsrb.Offset(0, itn) = nwsr.Value
            nwsrbi.Offset(3, itn) = "ContractOutside"
            m = target
            
            itn = itn + 1
            For i = 1 To nv
                v(i) = cv(i) - 0.5 * dv(i)
            Next i
            nwsrbi.Offset(0, -1) = itn
            Application.Calculate
            nwsrbi.Offset(0, itn) = itn
            nwsrb.Offset(0, itn) = nwsr.Value
            nwsrbi.Offset(3, itn) = "ContractInside"
                    
'            If target <= y(iiw) And m <= y(iiw) Then ' contract
            If target <= y(iiw2) And m <= y(iiw2) Then ' contract
                
                For ii = 1 To nv + 1
                    If ii <> iib Then
                        itn = itn + 1
                        For i = 1 To nv
                            x(i, ii) = x(i, iib) + 0.5 * (x(i, ii) - x(i, iib))
                            v(i) = x(i, ii)
                        Next i
                        nwsrbi.Offset(0, -1) = itn
                        Application.Calculate
                        nwsrbi.Offset(0, itn) = itn
                        nwsrb.Offset(0, itn) = nwsr.Value
                        nwsrbi.Offset(3, itn) = "Contract-X"
                        y(ii) = target
                        xi(ii) = itn
                    End If
                Next ii
            
            ElseIf target > m Then
            
                nwsrbi.Offset(3, itn) = "ContractInside-X"
                y(iiw) = target
                xi(iiw) = itn
                For i = 1 To nv
                    x(i, iiw) = v(i)
                Next i
            
            Else
            
                nwsrbi.Offset(3, itn - 1) = "ContractOutside-X"
                y(iiw) = m
                xi(iiw) = itn - 1
                For i = 1 To nv
                    x(i, iiw) = nwsrbi.Offset(3 + i, xi(iiw))
                Next i
            
            End If
                
        End If
        
        b = w
        iib = iiw
        For ii = 1 To nv + 1
            If y(ii) >= b Then
                b = y(ii)
                iib = ii
                itb = xi(ii)
                nwsrb = nwsrb.Offset(0, itb).Value
                nwsrbi = itb
            End If
            If ii = 1 Or y(ii) < w Then
                w = y(ii)
                iiw = ii
            End If
        Next ii
        w2 = b
        iiw2 = iiw
        For i = 1 To nv + 1
            If i <> iiw And i <> iib And y(i) < w2 Then
                w2 = y(i)
                iiw2 = i
            End If
        Next i
        
        d = b - w
        s = 0
        For i = 1 To nv
            s = s + Abs(x(i, iib) - x(i, iiw))
        Next i
    
        
        If itn >= itn_top Then
        
            Application.ScreenUpdating = True
            If ActiveSheet.Name = nws.Name Then
                If nws.ChartObjects.Count > 0 Then
                
                    Dim co As ChartObject
                    For Each co In nws.ChartObjects
                        co.Chart.Refresh
                    Next co
                    
                End If
            End If
            
            inp = MsgBox("Another " & nws.Cells(3, 4) & " Evaluations?", vbQuestion + vbYesNo + vbDefaultButton1, "Reached Evaluation Limit: " & itn_top)
            
            If inp = vbYes Then
                itn_top = (Int(itn / nws.Cells(3, 4)) + 1) * nws.Cells(3, 4)
                nwsrbi.Offset(1, -1) = itn_top
            End If
            
    
        End If
    
    
    Wend
            
    For i = 1 To nv
        v(i) = x(i, iib)
    Next
            
GoTo ElseIt
                
CatchIt:
    huh = 1

ElseIt:
    Application.Calculation = calcprior



End Sub



