Imports System.ComponentModel

Public Class frmWorkflowComment

    Public ReadOnly Property Comment As String
        Get
            Return tbComment.Text
        End Get
    End Property

    Public ReadOnly Property ReopenDrawing As Boolean
        Get
            Return chReopenDrawing.Checked
        End Get
    End Property

    Public Sub New(TransitionName As String)

        InitializeComponent()

        labWorkflowMessage.Text = TransitionName + ". Are you sure you want to continue?"

        'restore reopen checkbox
        chReopenDrawing.Checked = My.Settings.ReopenDrawingAfterWorkflowTransition

    End Sub

    Private Sub frmWorkflowComment_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

        'save reopen checkbox
        My.Settings.ReopenDrawingAfterWorkflowTransition = chReopenDrawing.Checked
        My.Settings.Save()

    End Sub
End Class