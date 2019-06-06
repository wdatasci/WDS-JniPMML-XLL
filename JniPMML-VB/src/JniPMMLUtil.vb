Imports ExcelDna.Integration
Imports ExcelDna.Registration.VisualBasic

'Namespace com.WDataSci.JniPMML

Public Module JniPMMLUtil

        <ExcelFunction(Name:="JniPMML_ExpandComplexValue",
                Category:="WDS.JniPMML",
                Description:="Takes a complex PMML evaluator target object and expands.",
                ExplicitRegistration:=True,
                IsVolatile:=False
                )>
        Public Function JniPMML_ExpandComplexValue(
                                         <ExcelArgument(Name:="Value", Description:="A jpmml-like complex value {k1=v1, k2=[sk1=sv1, sk2=sv2],...}")>
                                         ByVal Value As Object
                                         ) As Object

            If VarType(Value) <> VariantType.String Then
                JniPMML_ExpandComplexValue = ""
            Else
                Dim sValue As String
                sValue = Value.ToString().Trim()
                If Not (sValue.StartsWith("{") Or sValue.StartsWith("[")) Then
                    JniPMML_ExpandComplexValue = Value
                Else
                    Dim d As Dictionary(Of String, Object)
                    d = JniPMML_DeserializeComplexValue(sValue)
                    Dim rvl As List(Of Object)
                    rvl = JniPMML_Flatten(d, "")
                    JniPMML_ExpandComplexValue = rvl.ToArray()
                End If
            End If

        End Function


        Private Function JniPMML_Flatten(ByRef arg As Dictionary(Of String, Object), ByVal prefix As String) As List(Of Object)
            Dim rv, rvs As List(Of Object)
            rv = New List(Of Object)
            Dim k, pk As String
            Dim v As Object
            For Each k In arg.Keys
                pk = k
                If prefix.Length > 0 Then
                    pk = prefix + k
                End If
                rv.Add(pk)
                v = arg(k)
                If TypeOf v Is Dictionary(Of String, Object) Then
                    If pk.Length > 0 Then
                        rvs = JniPMML_Flatten(v, pk + "_")
                    Else
                        rvs = JniPMML_Flatten(v, "")
                    End If
                    For Each v In rvs
                        rv.Add(v)
                    Next
                Else
                    rv.Add(v)
                End If
            Next
            JniPMML_Flatten = rv

        End Function

        Private Function JniPMML_DeserializeComplexValue(ByVal arg As String) As Dictionary(Of String, Object)


            arg = arg.Trim()

            'quick short circuit
            If arg.StartsWith("{") And arg.EndsWith("}") Then
                Return JniPMML_DeserializeComplexValue(arg.Substring(1, arg.Length - 2))
            End If


            Dim d As Dictionary(Of String, Object)
            d = New Dictionary(Of String, Object)

            Dim ld As Dictionary(Of String, Object)
            ld = New Dictionary(Of String, Object)

            Dim os As String
            Dim openstatus As Stack(Of String)
            Dim openindex As Stack(Of Integer)

            Dim i, j As Integer

            'initial
            openstatus = New Stack(Of String)
            openindex = New Stack(Of Integer)
            openstatus.Push("i")
            openindex.Push(0)

            Dim key As String
            key = ""
            Dim value As Object
            value = Nothing

            For i = 0 To arg.Length - 1
                Select Case arg(i)
                    Case "{" ' open a {} grouping
                        If openstatus.Peek() <> "[" Then ' skip if in other bracket type
                            If openstatus.Peek() = "k" Then 'where k{ instead of k={
                                os = openstatus.Pop()
                                j = openindex.Pop()
                                key = arg.Substring(j, i - j)
                                openstatus.Push("v")
                                openindex.Push(i)
                            ElseIf openstatus.Peek() = "=" Then 'where k={
                                os = openstatus.Pop()
                                j = openindex.Pop()
                                openstatus.Push("v")
                                openindex.Push(i)
                            ElseIf openstatus.Peek() = "i" Then 'where string starts with {
                                openstatus.Push("v")
                                openindex.Push(i)
                            End If
                            openstatus.Push(arg(i))
                            openindex.Push(i)
                        End If
                    Case "}"   'close {} grouping only if open {
                        If openstatus.Peek() <> "[" Then
                            If openstatus.Peek() = "{" Then
                                os = openstatus.Pop()
                                j = openindex.Pop()
                                If openstatus.Peek() = "v" Then ' enclosed {} pairs are skipped, only the first has status v before
                                    value = arg.Substring(j + 1, i - j - 1)
                                    os = openstatus.Pop()
                                    j = openindex.Pop()
                                    d.Add(key, JniPMML_DeserializeComplexValue(value))
                                    'to flag a completed block
                                    openstatus.Push(arg(i))
                                    openindex.Push(i)
                                End If
                            End If
                        End If
                    Case "[" ' open a [] grouping
                        If openstatus.Peek() <> "{" Then ' if not inside a {} grouping
                            If openstatus.Peek() = "k" Then 'where k[
                                os = openstatus.Pop()
                                j = openindex.Pop()
                                key = arg.Substring(j, i - j)
                                openstatus.Push("v")
                                openindex.Push(i)
                            ElseIf openstatus.Peek() = "=" Then 'where k=[
                                os = openstatus.Pop()
                                j = openindex.Pop()
                                openstatus.Push("v")
                                openindex.Push(i)
                            ElseIf openstatus.Peek() = "i" Then 'where string starts with [
                                openstatus.Push("v")
                                openindex.Push(i)
                            End If
                            openstatus.Push(arg(i))
                            openindex.Push(i)
                        End If
                    Case "]"   'close [] grouping only if open [
                        If openstatus.Peek() <> "{" Then
                            If openstatus.Peek() = "[" Then
                                os = openstatus.Pop()
                                j = openindex.Pop()
                                If openstatus.Peek() = "v" Then  ' k=v pair with something in front of [
                                    value = arg.Substring(j + 1, i - j - 1)
                                    os = openstatus.Pop()
                                    j = openindex.Pop()
                                    d.Add(key, JniPMML_DeserializeComplexValue(value))
                                    'to flag a completed block
                                    openstatus.Push(arg(i))
                                    openindex.Push(i)
                                End If
                            End If
                        End If
                    Case "="     'closes a keyword, open a value
                        If "{[".IndexOf(openstatus.Peek()) < 0 Then
                            If openstatus.Peek() = "k" Then
                                os = openstatus.Pop()
                                j = openindex.Pop()
                                key = arg.Substring(j, i - j)
                            Else
                                key = ""
                            End If
                            openstatus.Push("=")
                            openindex.Push(i)
                        End If
                    Case ","     'close a value
                        If "{[".IndexOf(openstatus.Peek()) < 0 Then
                            If "]}".IndexOf(openstatus.Peek()) >= 0 Then 'recently completed block, already processed, just pop "v" status
                                os = openstatus.Pop()
                                j = openindex.Pop()
                            ElseIf openstatus.Peek() = "v" Then  ' k=v pair
                                os = openstatus.Pop()
                                j = openindex.Pop()
                                value = arg.Substring(j, i - j)
                                d.Add(key, value)
                            ElseIf "=k".IndexOf(openstatus.Peek()) >= 0 Then ' k with no = or k=,
                                os = openstatus.Pop()
                                j = openindex.Pop()
                                key = arg.Substring(j, i - j)
                                value = Nothing
                                d.Add(key, value)
                            End If
                            key = ""
                            value = Nothing
                        End If
                    Case Else
                        If arg(i) <> " " And "{[".IndexOf(openstatus.Peek()) < 0 Then
                            If "=,kv".IndexOf(openstatus.Peek()) < 0 Then
                                openstatus.Push("k")
                                openindex.Push(i)
                            ElseIf openstatus.Peek() = "=" Then
                                os = openstatus.Pop()
                                j = openindex.Pop()
                                openstatus.Push("v")
                                openindex.Push(i)
                            End If
                        End If
                End Select
            Next

            While openstatus.Peek() <> "i"
                If openstatus.Peek() = "v" Then
                    os = openstatus.Pop()
                    j = openindex.Pop()
                    value = arg.Substring(j)
                    d.Add(key, value)
                ElseIf "]}".IndexOf(openstatus.Peek()) >= 0 Then
                    os = openstatus.Pop()
                    j = openindex.Pop()
                ElseIf openstatus.Peek() = "k" Then
                    os = openstatus.Pop()
                    j = openindex.Pop()
                    key = arg.Substring(j)
                    d.Add(key, Nothing)
                ElseIf openstatus.Peek() = "=" Then
                    os = openstatus.Pop()
                    j = openindex.Pop()
                    d.Add(key, Nothing)
                ElseIf openstatus.Peek() = "[" Then
                    os = openstatus.Pop()
                    j = openindex.Pop()
                    While (openstatus.Peek() = "[")
                        os = openstatus.Pop()
                        j = openindex.Pop()
                    End While
                    If openstatus.Peek() = "v" Then
                        value = arg.Substring(j + 1, i - j - 1)
                        os = openstatus.Pop()
                        j = openindex.Pop()
                        d.Add(key, JniPMML_DeserializeComplexValue(value))
                    End If
                ElseIf openstatus.Peek() = "{" Then
                    os = openstatus.Pop()
                    j = openindex.Pop()
                    While (openstatus.Peek() = "{")
                        os = openstatus.Pop()
                        j = openindex.Pop()
                    End While
                    If openstatus.Peek() = "v" Then
                        value = arg.Substring(j + 1, i - j - 1)
                        os = openstatus.Pop()
                        j = openindex.Pop()
                        d.Add(key, JniPMML_DeserializeComplexValue(value))
                    End If
                Else
                    os = openstatus.Pop()
                    j = openindex.Pop()
                    d.Add("check" + j, "Check deserialization " + os + " starting at index " + j)
                End If
            End While

            JniPMML_DeserializeComplexValue = d

        End Function
    End Module

'End Namespace
