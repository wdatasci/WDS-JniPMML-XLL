Imports ExcelDna.Integration
Imports ExcelDna.Registration.VisualBasic

Public Module AddInExcelDna

    Public Class WDSJniPMMLVBAddIn
        Implements IExcelAddIn

        Public Sub AutoOpen() Implements IExcelAddIn.AutoOpen
            Try
                PerformDefaultRegistration()
            Catch
                MsgBox("In AutoOpen")
            Finally

            End Try
            'MsgBox("In AutoOpen")
            ExcelIntegration.RegisterUnhandledExceptionHandler(AddressOf WDSExceptionHandler)
        End Sub

        Public Sub AutoClose() Implements IExcelAddIn.AutoClose
            'MsgBox("In AutoClose")
        End Sub

        Private Shared Function WDSExceptionHandler(ex As Object) As Object
            Return "ERROR, " & ex.ToString()
        End Function
    End Class

End Module
