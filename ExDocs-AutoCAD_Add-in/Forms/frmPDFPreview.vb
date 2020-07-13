Public Class frmPDFPreview

    Public Sub New(pdfFilename As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        wbPDFPreview.Url = New Uri(pdfFilename)
    End Sub

End Class