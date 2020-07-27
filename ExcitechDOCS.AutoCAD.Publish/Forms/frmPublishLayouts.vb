Imports MFilesAPI
Imports Autodesk.AutoCAD.ApplicationServices
Imports System.ComponentModel
Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraEditors.ViewInfo
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports System.Windows.Forms
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.Utils
Imports DevExpress.Data

Public Class frmPublishLayouts

#Region " Private "
    Private _vault As Vault
    Private _document As Document
#End Region

#Region " Constrcutor "
    Public Sub New(Vault As Vault, acDocument As Document)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _vault = Vault
        _document = acDocument

        ''grab the templates from the vault

        LoggingHelper.WriteToLog("Connection to Vault: " + Vault.Name)
        MFilesHelper.Vault = _vault
        '_templates = MFilesHelper.populateDrawingTemplates()
        barchkshowThumbNail.Checked = PluginSettings.ShowThumbnailColumn
    End Sub
#End Region

#Region " Methods "

    Private Sub frmPublishLayouts_Load(sender As Object, e As EventArgs) Handles Me.Load
        initTemplates()
        UpdateStatusBar()
        UpdateRibbon()
    End Sub

    Private Sub btnSearch_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnSearch.ItemClick
        If PluginSettings.DefaultClass = -1 Then
            MessageBox.Show("No default Class hass been configured, please go to Settings and select a Class to import documents against", "Configuration Missing", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        ''are there any missing required properties?
        Dim lst = PluginSettings.DefaultClassProperties.Select(Function(p)
                                                                   If p.RequiredProperty And (p.ValueID = -1 Or p.Value Is Nothing) Then
                                                                       Return p.PropertyDescription(MFilesHelper.Vault)
                                                                   End If
                                                               End Function).ToList


        TimerRefreshData.Stop()

        'GridControl1.DataSource = Nothing
        ViewHelper.FindSelections()
        ViewHelper.UpdateVaultStatus()

        GridControl1.DataSource = ViewHelper.Selections '.Where(Function(va) va.LayerIsModel).ToList

        UpdateStatusBar()
        UpdateRibbon()

        Dim viewArea As ViewArea = AdvBandedGridView1.GetFocusedRow
        ShowLargeThumbnail(viewArea)

        TimerRefreshData.Start()
    End Sub

    Private Sub btnSettings_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnSettings.ItemClick
        Dim frmsettings As New frmSettings
        frmsettings.StartPosition = Windows.Forms.FormStartPosition.CenterParent
        If frmsettings.ShowDialog = Windows.Forms.DialogResult.OK Then
            UpdateStatusBar()
        End If
    End Sub

    Public Sub initTemplates()
        ''run this in the background
        'Await Task.Run(Sub() MFilesHelper.populateDrawingTemplates(PluginSettings.LocalTemplatePath, PluginSettings.CheckDOCSForTemplates))
        MFilesHelper.populateDrawingTemplates(PluginSettings.LocalTemplatePath, PluginSettings.CheckDOCSForTemplates)
    End Sub

    Private Sub BarButtonItem2_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnPublisj.ItemClick
        If PluginSettings.DefaultClass = -1 Then
            MsgBox("There is currently no document class selected, please set a default document class in Settings", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, "ExcitechDOCS")
            Exit Sub
        End If

        LoggingHelper.WriteToLog("Starting Publish...")

        ''this is a horrible bodge to fix somethigna weird error
        ''the item at the first index is being set to nothing for somereason
        ViewHelper.Selections = AdvBandedGridView1.DataSource


        '''which layouts have been set to export?
        'Dim exportViewAreas As List(Of ViewArea) = ViewHelper.Selections.Where(Function(va)
        '                                                                           Return va.ExportDWG.Export OrElse va.ExportPDF.Export
        '                                                                       End Function).ToList
        Dim exportCount As Integer = ViewHelper.Selections.Where(Function(va) va.ExportDWG.Export OrElse va.ExportPDF.Export).Count

        LoggingHelper.WriteToLog("Found: " + exportCount.ToString + " selections to publish")

        ''set the full template path
        For vi As Integer = 0 To ViewHelper.Selections.Count - 1
            Dim va As ViewArea = ViewHelper.Selections(vi)

            If Not va.ExportDWG.Export OrElse Not va.ExportPDF.Export Then Continue For

            Dim i As Integer = MFilesHelper.Templates.FindIndex(Function(t)
                                                                    Return t.TemplateName = va.Template
                                                                End Function)

            If Not i = -1 Then
                va.DrawingTemplate = MFilesHelper.Templates(i)
            Else
                LoggingHelper.WriteToLog("Unable to find template: " + va.Template)

                ''cant find the template so bail out
                va.SetStatus("Unable to find Template file", ViewArea.StatusIcons.Failure)
                va.Disabled = True
            End If

            ''check the layout name
            Dim oCl As New ObjectClassWrapper(MFilesHelper.Vault, PluginSettings.DefaultClass)

            Dim namePropIndex As Integer = PluginSettings.DefaultClassProperties.FindIndex(Function(_p) _p.MFilesPropertyDef(MFilesHelper.Vault).ID = oCl.ObjectClass.NamePropertyDef)
            If Not namePropIndex = -1 Then
                Dim nameProp = PluginSettings.DefaultClassProperties.Item(namePropIndex)
            End If

            If String.IsNullOrWhiteSpace(va.LayoutUniqueID) Then
                Dim uID As String = "ACAD_" + Guid.NewGuid.ToString
                va.LayoutUniqueID = uID
            End If
        Next

        ''
        Dim publishDelegate As New PublishLayoutDelegate(ViewHelper.Selections)
        Dim publishProgress As New frmPublishProcess(ViewHelper.Selections, AddressOf publishDelegate.PublishLayout)
        publishProgress.Caption = "Publishing"
        publishProgress.IconImage = My.Resources.PublishSheets_40
        publishProgress.ShowDialog(Me)

        UpdateRibbon()

        For Each va As ViewArea In ViewHelper.Selections
            If Not va.StatusIcon = ViewArea.StatusIcons.Failure AndAlso va.LayerIsModel Then
                AutoCADHelper.AddLayoutUniqueIDXData(va.ObjectID, va.LayoutUniqueID)
            End If
        Next
        ''
        If barchkCloseOnCompletion.Checked Then Close()

        'AutoCADHelper.ExportLayouts()
    End Sub
#End Region

#Region " Update Riboon "
    Private Sub UpdateRibbon()
        Dim va As ViewArea = AdvBandedGridView1.GetFocusedRow
        Dim selectedCount As Integer = AdvBandedGridView1.SelectedRowsCount

        ViewHelper.Selections = AdvBandedGridView1.DataSource

        btnViewInDOCS.Enabled = va IsNot Nothing AndAlso va.IsInMFiles
        btnPublisj.Enabled = va IsNot Nothing AndAlso ViewHelper.CanPublish
    End Sub

    Private Sub UpdateStatusBar()
        If Not PluginSettings.DefaultClass < 0 Then
            Dim objClass As New ObjectClassWrapper(MFilesHelper.Vault, PluginSettings.DefaultClass)
            barSheetClass.Caption = "Selected Class: " + objClass.ObjectClass.Name
        End If

        barSheetCount.Caption = "Layout Count: " + AdvBandedGridView1.RowCount.ToString
        statusBar.Refresh()
    End Sub

    Private Sub ShowLargeThumbnail(layout As ViewArea)
        If layout Is Nothing Then Exit Sub
        pbThumb.Image = layout.LargeThumbnail
    End Sub
#End Region

#Region " Grid View Editor "
    Private Sub advancedBandedView_MouseDown(sender As Object, e As MouseEventArgs) Handles AdvBandedGridView1.MouseDown
        UpdateRibbon()

        If InSelectedCell(e) Then
            Dim hi As GridHitInfo = AdvBandedGridView1.CalcHitInfo(e.Location)
            ''
            'Dim viewArea As ViewArea = AdvBandedGridView1.GetFocusedRow
            'ShowLargeThumbnail(viewArea)

            If AdvBandedGridView1.FocusedRowHandle = hi.RowHandle Then
                AdvBandedGridView1.FocusedColumn = hi.Column
                DXMouseEventArgs.GetMouseArgs(e).Handled = True
            End If
        End If
    End Sub

    Private Sub advancedBandedView_MouseUp(sender As Object, e As MouseEventArgs) Handles AdvBandedGridView1.MouseUp

        Dim hi As GridHitInfo = AdvBandedGridView1.CalcHitInfo(e.Location)
        Dim viewArea As ViewArea = AdvBandedGridView1.GetFocusedRow
        ShowLargeThumbnail(viewArea)
        If InSelectedCell(e) Then

            DXMouseEventArgs.GetMouseArgs(e).Handled = True
            'advancedBandedView.ShowEditorByMouse()

            AdvBandedGridView1.FocusedRowHandle = hi.RowHandle
            AdvBandedGridView1.FocusedColumn = hi.Column
            AdvBandedGridView1.ShowEditor()

            Dim edit = TryCast(AdvBandedGridView1.ActiveEditor, CheckEdit)
            If edit IsNot Nothing Then
                edit.Toggle()
                TryCast(e, DevExpress.Utils.DXMouseEventArgs).Handled = True
            End If

            Dim combox = TryCast(AdvBandedGridView1.ActiveEditor, ComboBoxEdit)
            If combox IsNot Nothing Then
                combox.ShowPopup()
                TryCast(e, DevExpress.Utils.DXMouseEventArgs).Handled = True
            End If

        End If
    End Sub

    Private Function InSelectedCell(ByVal e As MouseEventArgs) As Boolean
        Dim hitInfo As GridHitInfo = AdvBandedGridView1.CalcHitInfo(e.Location)
        Return hitInfo.InRowCell
    End Function

    ''set the combo box template items
    Private Sub reposItemTemplate_QueryPopUp(sender As Object, e As CancelEventArgs) Handles reposItemTemplate.QueryPopUp
        Dim comboEdit As ComboBoxEdit = sender
        comboEdit.Properties.Items.Clear()

        For Each _dt As DrawingTemplate In MFilesHelper.Templates
            comboEdit.Properties.Items.Add(_dt.TemplateName)
        Next

        Dim va As ViewArea = AdvBandedGridView1.GetFocusedRow
    End Sub

    Private Sub editLayout_QueryPopUp(sender As Object, e As CancelEventArgs) Handles editLayout.QueryPopUp
        Dim comboEdit As ComboBoxEdit = sender
        comboEdit.Properties.Items.Clear()

        Dim va As ViewArea = AdvBandedGridView1.GetFocusedRow
        comboEdit.Properties.Items.AddRange(va.DrawingTemplate.Layouts)
    End Sub

    Private Sub editLayout_CloseUp(sender As Object, e As CloseUpEventArgs) Handles editLayout.CloseUp
        Dim selectedRows = AdvBandedGridView1.GetSelectedRows
        For Each rowHandle In selectedRows
            Dim sheet As ViewArea = AdvBandedGridView1.GetRow(rowHandle)
            sheet.DrawingTemplate.LayoutName = e.Value
        Next

        UpdateRibbon()

    End Sub

    Private Sub reposItemTemplate_CloseUp(sender As Object, e As CloseUpEventArgs) Handles reposItemTemplate.CloseUp
        Dim selectedRows = AdvBandedGridView1.GetSelectedRows
        For Each rowHandle In selectedRows
            Dim sheet As ViewArea = AdvBandedGridView1.GetRow(rowHandle)
            sheet.Template = e.Value

            ''find the template and populate the sheet with it
            Dim tmpl As DrawingTemplate = MFilesHelper.Templates.Find(Function(t) t.TemplateName.ToUpperInvariant = e.Value.ToString.ToUpperInvariant)
            sheet.DrawingTemplate = tmpl
            sheet.LayoutName = tmpl.LayoutName
        Next

        UpdateRibbon()
    End Sub

    Private Sub btnDebug_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnDebug.ItemClick
        Debugger.Launch()
        Debugger.Break()
    End Sub

    Private Sub AdvBandedGridView1_CellValueChanging(sender As Object, e As CellValueChangedEventArgs) Handles AdvBandedGridView1.CellValueChanging

        'get the current selection
        Dim selectedRows = AdvBandedGridView1.GetSelectedRows
        For Each rowHandle In selectedRows
            Dim viewArea As ViewArea = AdvBandedGridView1.GetRow(rowHandle)
            viewArea.SetProperty(e.Column.FieldName, e.Value)
        Next

        AdvBandedGridView1.RefreshData()

        UpdateRibbon()
    End Sub

    Private Sub TimerRefreshData_Tick(sender As Object, e As EventArgs) Handles TimerRefreshData.Tick
        AdvBandedGridView1.RefreshData()
    End Sub

    Private Sub AdvBandedGridView1_ShowingEditor(sender As Object, e As CancelEventArgs) Handles AdvBandedGridView1.ShowingEditor
        Dim rowHandle As Integer = AdvBandedGridView1.FocusedRowHandle
        Dim viewArea As ViewArea = AdvBandedGridView1.GetRow(rowHandle)
        If viewArea.Disabled Then
            e.Cancel = True
            Return
        End If


        If AdvBandedGridView1.FocusedColumn.Name = "colLayoutName" Then
            e.Cancel = viewArea.IsInMFiles
        End If
    End Sub

    Private Sub btnViewInDOCS_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnViewInDOCS.ItemClick
        Try
            Dim viewArea As ViewArea = AdvBandedGridView1.GetFocusedRow
            viewArea.ViewObjectInMFiles(MFilesHelper.Vault)
        Catch ex As Exception

        End Try
    End Sub

#End Region

#Region " Custom Header Drawing "

    Private Sub AdvBandedGridView1_CustomDrawColumnHeader(sender As Object, e As ColumnHeaderCustomDrawEventArgs) Handles AdvBandedGridView1.CustomDrawColumnHeader
        If e.Column Is Nothing Then Return
        If e.Info.HeaderPosition = DevExpress.Utils.Drawing.HeaderPositionKind.Special Then Return

        Select Case e.Column.Name.ToUpperInvariant
            Case "COLPDF"
                e.DefaultDraw()
                e.Graphics.DrawImage(My.Resources.pdf_20, e.Bounds.Location.X + 19, e.Bounds.Location.Y + 10)
                e.Handled = True
            Case "COLDWG"
                e.DefaultDraw()
                e.Graphics.DrawImage(My.Resources.dwg_20, e.Bounds.Location.X + 19, e.Bounds.Location.Y + 10)
                e.Handled = True
        End Select
    End Sub

    Private Sub AdvBandedGridView1_CustomDrawCell(sender As Object, e As RowCellCustomDrawEventArgs) Handles AdvBandedGridView1.CustomDrawCell

        Dim view As ViewArea = AdvBandedGridView1.GetRow(e.RowHandle)

        Select Case e.Column.Name
            Case "colPublishStatus"
                Dim textEditView As TextEditViewInfo = e.Cell.viewinfo
                textEditView.ContextImage = view.StatusIconImage
        End Select

    End Sub

    Private Sub BarButtonItem1_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs)


    End Sub

    Private Sub AdvBandedGridView1_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles AdvBandedGridView1.SelectionChanged
        UpdateRibbon()
    End Sub

    Private Sub AdvBandedGridView1_DoubleClick(sender As Object, e As EventArgs) Handles AdvBandedGridView1.DoubleClick
        Dim location = GridControl1.PointToClient(MousePosition)
        Dim hitInfo As GridHitInfo = AdvBandedGridView1.CalcHitInfo(location)

        If hitInfo.InDataRow Then
            Dim sheet As ViewArea = AdvBandedGridView1.GetRow(hitInfo.RowHandle)

            'sync error
            If hitInfo.Column?.Name = "colPublishStatus" Then
                If sheet.ErrorMessage <> "" Then
                    Dim frmErrorDetailsUI As New frmErrorDetails(sheet.ErrorMessage)
                    frmErrorDetailsUI.StartPosition = FormStartPosition.CenterParent
                    frmErrorDetailsUI.ShowDialog(Me)
                End If
            End If



        End If
    End Sub

    Private Sub barchkshowThumbNail_CheckedChanged(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles barchkshowThumbNail.CheckedChanged
        colThumbNail.Visible = barchkshowThumbNail.Checked
    End Sub

    Private Sub frmPublishLayouts_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Dim settingsFile As String = System.IO.Path.Combine(Settings.SettingsPath, "AutoCAD.Publish.xml")

        PluginSettings.ShowThumbnailColumn = barchkshowThumbNail.Checked
        PluginSettings.CloseOnCompletion = barchkCloseOnCompletion.Checked
        Settings.SaveSettings(settingsFile, PluginSettings)
    End Sub

    Private Sub AdvBandedGridView1_CustomRowFilter(sender As Object, e As RowFilterEventArgs) Handles AdvBandedGridView1.CustomRowFilter
        Dim va As ViewArea = AdvBandedGridView1.GetRow(e.ListSourceRow)
        If Not va.LayerIsModel Then
            e.Visible = False
            e.Handled = True
        End If
    End Sub

#End Region

End Class