Imports System.Windows.Forms

Public Class frmMFilesValueChooser

#Region "Instance Variables"
    Private m_valueDictionary As Dictionary(Of Integer, String)
    Private m_currentID As Integer
#End Region

#Region "Properties"
    Public ReadOnly Property ValueID As Long
        Get
            Return lvValues.SelectedItems(0).Tag
        End Get
    End Property

    Public ReadOnly Property ValueDisplay As String
        Get
            Return lvValues.SelectedItems(0).Text
        End Get
    End Property
#End Region

    'constructor
    Public Sub New(CurrentID As Integer, ValueDictionary As Dictionary(Of Integer, String))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        m_currentID = CurrentID
        m_valueDictionary = ValueDictionary

    End Sub

#Region "Form Events"
    Private Sub frmMFilesValueChooser_Load(sender As Object, e As EventArgs) Handles Me.Load

        populateValueListView()
        lvValues.Focus()
    End Sub
#End Region

#Region "User Interface"
    Private Sub lvValues_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvValues.SelectedIndexChanged

        btnOK.Enabled = lvValues.SelectedItems.Count
    End Sub

    Private Sub butOK_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnOK.Click
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub butCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub
#End Region

#Region "Data Population"

    Private Sub populateValueListView()

        lvValues.Items.Clear()
        lvValues.BeginUpdate()

        For Each valueEntry In m_valueDictionary
            Dim lstItem = lvValues.Items.Add(valueEntry.Value, 0)
            lstItem.Tag = valueEntry.Key
            If m_currentID = valueEntry.Key Then lstItem.Selected = True
        Next
        lvValues.EndUpdate()

    End Sub

    Private Sub lvValues_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles lvValues.MouseDoubleClick
        Dim hitTestInfo = lvValues.HitTest(e.Location)
        If hitTestInfo.Item Is Nothing Then Return

        DialogResult = DialogResult.OK
        Close()
    End Sub

#End Region

End Class