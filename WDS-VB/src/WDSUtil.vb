Imports ExcelDna.Integration
Imports Microsoft.Office.Interop.Excel

'Namespace com.WDataSci.WDS

Public Module Util

        <ExcelFunction(Name:="Concatenate_Dlm",
                Category:="WDS",
                Description:="Concatenates the string values of a range of cells into a delimited string, empty values are not included",
                ExplicitRegistration:=True,
                IsVolatile:=False
                )>
        Public Function Concatenate_Range(
                                         <ExcelArgument(Name:="Values", Description:="A contiguous range to concatenate")>
                                         ByVal Values(,) As Object,
                                         <ExcelArgument(Name:="dlm", Description:="A delimiter")>
                                         ByVal dlm As String) As String
            Dim rv As String = ""
            Dim s As String
            Dim i, j, k As Integer
            k = 0
            For i = LBound(Values, 1) To UBound(Values, 1)
                For j = LBound(Values, 2) To UBound(Values, 2)
                    s = Values(i, j).ToString().Trim()
                    If (Not IsNothing(Values(i, j))) And (TypeOf (Values(i, j)) IsNot ExcelDna.Integration.ExcelEmpty) And (s.Length > 0) Then
                        k = k + 1
                        If k = 1 Then
                            rv = Values(i, j).ToString()
                        Else
                            rv = rv & dlm & Values(i, j).ToString()
                        End If
                    End If
                Next
            Next
            Concatenate_Range = rv
        End Function

    <ExcelFunction(Name:="bIn", Category:="WDS", Description:="Returns true if first argument value is any of the optional arguments", ExplicitRegistration:=True)>
    Public Function bIn(<ExcelArgument(Name:="PrimarySubject", Description:="Compares string value against all other arguments", AllowReference:=False)> ByVal arg As String,
            <ExcelArgument(Name:="CompareValues", Description:="Compares each against the first agument", AllowReference:=False)>
            ParamArray ByVal arglist() As Object) As Boolean
        bIn = False
        For i = LBound(arglist) To UBound(arglist)
            If arglist(i).ToString() = arg Then
                bIn = True
                Exit For
            End If
        Next i
    End Function

End Module

'End Namespace