Imports System.Windows

Public Class frmErrorDetails
    Sub New(ErrorMessage As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        textboxErrorDetails.Text = ErrorMessage
    End Sub

    Private Sub buttonCopyClip_Click(sender As Object, e As EventArgs) Handles buttonCopyClip.Click

        Clipboard.Clear()
        Clipboard.SetText(textboxErrorDetails.Text)
    End Sub

    Private Sub buttonClose_Click(sender As Object, e As EventArgs) Handles buttonClose.Click
        Close()
    End Sub
End Class