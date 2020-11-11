Imports System.Drawing
Imports System.Windows.Forms

Public Class frmPublishProcess

#Region " Delegates "
    Public Delegate Sub ProcessDelegate(Layout As ViewArea, about As Boolean)
#End Region

#Region " Private "
    Private _layoutsForPublish As List(Of ViewArea)
    Private _abort As Boolean = False
    Private _delegate As ProcessDelegate
#End Region

#Region " Properties "

    Public Property Caption As String
        Get
            Return Text
        End Get
        Set(value As String)
            Text = value
        End Set
    End Property

    Public Property IconImage As Image
        Get
            Return pictureboxIcon.Image
        End Get
        Set(value As Image)
            pictureboxIcon.Image = value
        End Set
    End Property

#End Region

#Region " Constructor "

    Public Sub New(LayoutsForPublish As List(Of ViewArea), ProcessDelegate As ProcessDelegate)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _layoutsForPublish = LayoutsForPublish
        _delegate = ProcessDelegate

        progressBarPublishing.Properties.Maximum = _layoutsForPublish.Count
    End Sub

#End Region

#Region " Form Events "

    Private Sub frmPublishProcess_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        ProcessSheets()
    End Sub

#End Region

#Region " Buttons "

    Private Sub buttonAbort_Click(sender As Object, e As EventArgs) Handles buttonAbort.Click
        buttonAbort.Text = "Aborting..."
        buttonAbort.Enabled = False
        _abort = True
    End Sub

#End Region

#Region " Methods "

    Private Sub ProcessSheets()
        Dim pCount As Integer = 0

        For Each Layout As ViewArea In _layoutsForPublish
            'labelCurrentSheet.Text = $"Processing: {sheet.SheetNumber} - {sheet.SheetName}"
            labelCurrentSheet.Text = "Processing ..."
            Application.DoEvents()

            Try
                If Layout.Disabled Then labelCurrentSheet.Text = "Skipping ..." : Continue For
                If Not (Layout.ExportDWG.Export OrElse Layout.ExportPDF.Export) Then labelCurrentSheet.Text = "Skipping ..." : Continue For
                If Not Layout.LayerIsModel Then Continue For ''we dont want to process the non-model geometry in to the layer we do this seperatly

                'call process delegate
                _delegate(Layout, _abort)

            Catch ex As Exception
                Layout.Disabled = True
                Layout.SetStatus("Error when publishing", ViewArea.StatusIcons.Failure)
                Layout.ErrorMessage = ex.Message.ToString
            End Try

            Application.DoEvents()
            pCount += 1
            progressBarPublishing.Position = pCount
            Application.DoEvents()
        Next

        Close()
    End Sub

#End Region


End Class