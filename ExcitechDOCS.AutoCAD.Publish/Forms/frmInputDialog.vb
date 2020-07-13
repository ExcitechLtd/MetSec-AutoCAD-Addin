Imports System.Windows.Forms

Public Class frmInputDialog

#Region "Instance Variables"
    Private m_inputValidationCallback As Func(Of Char, Boolean)
    Private m_outputValidationCallback As Func(Of String, Tuple(Of Boolean, String))
    Private _useDocName As Boolean
#End Region

#Region "Properties"
    Public ReadOnly Property getTextValue As String
        Get
            If Not chkUseDocName.Checked Then
                Return tbTextEntry.Text
            End If
            Return ""
        End Get
    End Property

    Public WriteOnly Property InputValidationCallback As Func(Of Char, Boolean)
        Set(value As Func(Of Char, Boolean))
            m_inputValidationCallback = value
        End Set
    End Property

    Public WriteOnly Property OutputValidationCallback As Func(Of String, Tuple(Of Boolean, String))
        Set(value As Func(Of String, Tuple(Of Boolean, String)))
            m_outputValidationCallback = value
        End Set
    End Property

    Public Property UseDocumentName As Boolean
        Get
            Return _useDocName
        End Get
        Set(value As Boolean)
            _useDocName = value
            chkUseDocName.Visible = value
            chkUseDocName.Checked = value
        End Set
    End Property
#End Region

    'Constructor
    Public Sub New(Heading As String, Caption As String, DefaultValue As String, Optional casing As CharacterCasing = CharacterCasing.Normal)

        InitializeComponent()

        Text = Heading
        labCaption.Text = Caption
        tbTextEntry.Text = DefaultValue
        tbTextEntry.CharacterCasing = casing

        _useDocName = False
    End Sub

#Region "User Interface"
    Private Sub tbTextEntry_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tbTextEntry.KeyPress

        If m_inputValidationCallback Is Nothing Then Return

        e.Handled = Not m_inputValidationCallback(e.KeyChar)
    End Sub

    Private Sub tbTextEntry_KeyDown(sender As Object, e As KeyEventArgs) Handles tbTextEntry.KeyDown

        If e.KeyCode = Keys.Enter AndAlso dataValid() Then DialogResult = DialogResult.OK
    End Sub

#End Region

#Region "Data Validation"

    Private Function dataValid() As Boolean

        If m_outputValidationCallback Is Nothing Then Return True

        Dim validationResult = m_outputValidationCallback(tbTextEntry.Text)

        If Not validationResult.Item1 Then
            MsgBox(validationResult.Item2)
            Return False
        End If

        Return True
    End Function

    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        If dataValid() Then
            DialogResult = System.Windows.Forms.DialogResult.OK
            Close()
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        DialogResult = System.Windows.Forms.DialogResult.Cancel
        Close()
    End Sub

    Private Sub chkUseDocName_CheckedChanged(sender As Object, e As EventArgs) Handles chkUseDocName.CheckedChanged
        If chkUseDocName.Checked Then
            tbTextEntry.Enabled = False
            tbTextEntry.Text = "Use Document Name"
            tbTextEntry.Font = New Drawing.Font(tbTextEntry.Font, Drawing.FontStyle.Italic)
        Else
            tbTextEntry.Enabled = True
            tbTextEntry.Text = ""
            tbTextEntry.Font = New Drawing.Font(tbTextEntry.Font, Drawing.FontStyle.Regular)
        End If

    End Sub
#End Region

End Class