Imports System.ComponentModel
Imports System.Drawing
Imports System.Reflection
Imports System.Windows.Forms
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices

Public Class frmUpdateReferences

#Region "Shared Methods"
    Public Shared Shadows Sub DoubleBuffered(ByVal Control As Control, ByVal enable As Boolean)
        Dim doubleBufferPropertyInfo = Control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance + BindingFlags.NonPublic)
        Dim val = doubleBufferPropertyInfo.GetValue(Control, Nothing)
        doubleBufferPropertyInfo.SetValue(Control, enable, Nothing)
    End Sub
#End Region

#Region "Instance Variables"

    Private m_document As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
    Private m_dwgDatabase As Database = Core.Application.DocumentManager.MdiActiveDocument.Database
    Private m_vault As Vault
    Private m_vaultSettings As AutoCADVaultSettings
    Private m_sortColumnIndex As Integer = -1
    Private m_sortOrder As SortOrder = SortOrder.None
#End Region

    'constructor
    Public Sub New()

        InitializeComponent()

        'get active document
        Dim acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument

        'initialise M-Files
        m_vault = VaultStatus.initialiseMFiles()
        Dim status = VaultStatus.Status()

        'check connection status
        If Not (status = VaultStatus.VaultStatuses.connected Or status = VaultStatus.VaultStatuses.offline) Then Return

        'check Drawing is in M-Files
        If Not isMFilesDocument() Then Return

        'read Vault Settings
        m_vaultSettings = AutoCADVaultSettings.ReadSettings(m_vault)
    End Sub

    Private Sub frmUpdateReferences_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        'make List view double buffered to remove flicker
        DoubleBuffered(lvReferences, True)

        'update columns settings and populate references
        createCustomColumns()
        populateReferences()

        'call ItemChecked event handler to update button statuses
        lvReferences_ItemChecked(Nothing, Nothing)

        'restore Window placement
        If My.Settings.UpdateReferenceWindowPlacement IsNot Nothing Then My.Settings.UpdateReferenceWindowPlacement.restoreWindowPlacement(Me)

        'restore Reference Column settings
        If My.Settings.UpdateReferenceColumnCollection IsNot Nothing Then
            My.Settings.UpdateReferenceColumnCollection.restoreColumnSettings(lvReferences)
            m_sortColumnIndex = My.Settings.UpdateReferenceColumnCollection.sortedColumnIndex
            m_sortOrder = My.Settings.UpdateReferenceColumnCollection.sortOrder
            updateSortOrder()
        End If

    End Sub

    Private Sub frmUpdateReferences_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

        'store Window Placement
        My.Settings.UpdateReferenceWindowPlacement = New WindowPlacement(Me)
        My.Settings.UpdateReferenceColumnCollection = New ColumnCollection(lvReferences, m_sortColumnIndex, m_sortOrder)
        My.Settings.Save()
    End Sub

#Region "Properties"
    Private ReadOnly Property CheckableItemCount As Integer
        Get
            Dim count As Integer = 0
            For Each lstItem As ListViewItem In lvReferences.Items
                Dim refData As UpdateReferenceItem = lstItem.Tag
                If refData Is Nothing Then Continue For
                If refData.IsCheckable Then count += 1
            Next
            Return count
        End Get
    End Property

#End Region

#Region "Data Population"

    Private Sub createCustomColumns()

        For Each propertyIdentifier In m_vaultSettings.referenceColumns
            Dim heading As String = getPropertyName(m_vault, propertyIdentifier)
            Dim column = lvReferences.Columns.Add(heading, 250)
            column.Name = propertyIdentifier
        Next
    End Sub

    Private Sub populateReferences()

        lvReferences.Items.Clear()
        lvReferences.BeginUpdate()

        'process Xrefs
        listXrefs()

        'process Images
        listImages()

        'process PDFs
        listPDFs()

        lvReferences.EndUpdate()

    End Sub


    Private Sub listXrefs()

        Using trans As Transaction = m_dwgDatabase.TransactionManager.StartTransaction

            Dim blkTable As BlockTable = trans.GetObject(m_dwgDatabase.BlockTableId, OpenMode.ForRead)

            For Each objectId As ObjectId In blkTable
                Dim blkTabRec As BlockTableRecord = trans.GetObject(objectId, OpenMode.ForRead)
                If blkTabRec.XrefStatus = XrefStatus.NotAnXref Then Continue For

                'initialise reference data
                Dim refdata As New UpdateReferenceItem(m_vault, objectId)

                'if valid then add to listview
                If refdata.IsValid Then
                    Dim lstItem = createListViewItem("DWG")
                    refreshListViewItem(lstItem, refdata)
                End If
            Next
        End Using

    End Sub

    Private Sub listImages()

        Using trans As Transaction = m_dwgDatabase.TransactionManager.StartTransaction
            Dim imageDictID = RasterImageDef.GetImageDictionary(m_dwgDatabase)
            If imageDictID.IsNull Then Return

            Dim imageDefDict As DBDictionary = trans.GetObject(imageDictID, OpenMode.ForRead)
            For Each dictEntry As DBDictionaryEntry In imageDefDict
                'initialise reference data
                Dim refdata As New UpdateReferenceItem(m_vault, dictEntry.Value)

                'if valid then add to listview
                If refdata.IsValid Then
                    Dim lstItem = createListViewItem("IMG")
                    refreshListViewItem(lstItem, refdata)
                End If
            Next
        End Using

    End Sub

    Private Sub listPDFs()

        Using trans As Transaction = m_dwgDatabase.TransactionManager.StartTransaction
            Dim pdfDictKey As String = UnderlayDefinition.GetDictionaryKey(GetType(PdfDefinition))
            Dim namedObjectDict As DBDictionary = m_dwgDatabase.NamedObjectsDictionaryId.GetObject(OpenMode.ForRead)
            If Not namedObjectDict.Contains(pdfDictKey) Then Return
            Dim pdfDefDict As DBDictionary = trans.GetObject(namedObjectDict.GetAt(pdfDictKey), OpenMode.ForRead)
            For Each dictEntry As DBDictionaryEntry In pdfDefDict
                'initialise reference data
                Dim refdata As New UpdateReferenceItem(m_vault, dictEntry.Value)

                'if valid then add to listview
                If refdata.IsValid Then
                    Dim lstItem = createListViewItem("PDF")
                    refreshListViewItem(lstItem, refdata)
                End If
            Next
        End Using

    End Sub

    Private Function createListViewItem(iconKey As String) As ListViewItem

        Dim lstItem As New ListViewItem("", iconKey)

        For Each column As ColumnHeader In lvReferences.Columns
            lstItem.SubItems.Add("")
        Next

        'add to list view
        lvReferences.Items.Add(lstItem)

        Return lstItem

    End Function

    Private Sub refreshListViewItem(LstItem As ListViewItem, RefData As UpdateReferenceItem)

        LstItem.Tag = RefData
        LstItem.Checked = RefData.IsCheckable

        For Each column As ColumnHeader In lvReferences.Columns
            Select Case column.Name
                Case "chCheckBoxIcon"
                    Continue For
                Case "chReferenceName"
                    LstItem.Name = RefData.ReferenceName
                    LstItem.SubItems(column.Index).Text = RefData.ReferenceName
                Case "chUpdateReason"
                    LstItem.SubItems(column.Index).Text = RefData.VersionStatusDescription
                Case Else
                    Dim propertyValue As String = ""
                    If RefData.NamedValues IsNot Nothing Then
                        If Not IsDBNull(RefData.NamedValues(column.Name)) Then propertyValue = RefData.NamedValues(column.Name)
                        If propertyValue = "[" + column.Text + "]" Then propertyValue = "---"
                    End If
                    LstItem.SubItems(column.Index).Text = propertyValue
            End Select
        Next

    End Sub
#End Region

#Region "Process References"
    Private Sub updateReferences()

        For Each checkedLstItem As ListViewItem In lvReferences.CheckedItems
            'get reference data from listitem tag
            Dim refData As UpdateReferenceItem = checkedLstItem.Tag

            'repath reference and refresh UI (-1 equals latest version)
            refData.RepathReference(-1)
            refData.InitialiseReference()
            refreshListViewItem(checkedLstItem, refData)
            lvReferences.Invalidate()
        Next

        'regen drawing
        Core.Application.DocumentManager.MdiActiveDocument.Editor.Regen()
    End Sub
#End Region

#Region "User Interface"

    Private Sub lvReferences_ItemChecked(sender As Object, e As ItemCheckedEventArgs) Handles lvReferences.ItemChecked
        butUpdate.Enabled = lvReferences.CheckedItems.Count

        butSelectAll.Enabled = CheckableItemCount <> lvReferences.CheckedItems.Count
        butDeselectAll.Enabled = lvReferences.CheckedItems.Count
    End Sub

    Private Sub lvReferences_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles lvReferences.ItemCheck

        Dim lstItem = lvReferences.Items(e.Index)
        Dim refData As UpdateReferenceItem = lstItem.Tag

        If Not refData.IsCheckable Then e.NewValue = False
    End Sub

    Private Sub butSelectAll_Click(sender As System.Object, e As System.EventArgs) Handles butSelectAll.Click

        For Each lstItem As ListViewItem In lvReferences.Items
            lstItem.Checked = True
        Next

        butSelectAll.Enabled = False
    End Sub

    Private Sub butDeselectAll_Click(sender As System.Object, e As System.EventArgs) Handles butDeselectAll.Click

        For Each lstItem As ListViewItem In lvReferences.Items
            lstItem.Checked = False
        Next

        butDeselectAll.Enabled = False
    End Sub

    Private Sub butUpdate_Click(sender As System.Object, e As System.EventArgs) Handles butUpdate.Click

        'repath checked references and Regen drawing
        updateReferences()
        m_document.SendStringToExecute(ChrW(3) + "REGENALL" + vbCr, False, False, True)

        'get currently selected item
        Dim selectedRefName As String = ""
        If lvReferences.SelectedItems.Count > 0 Then selectedRefName = lvReferences.SelectedItems(0).Name

        'refresh UI
        populateReferences()

        'select previously selected item
        If selectedRefName <> "" Then
            Dim refItems = From item As ListViewItem In lvReferences.Items Where item.Name = selectedRefName Select item
            If refItems.Count Then refItems.First.Selected = True : refItems.First.EnsureVisible()
        End If

        'call SelectedIndexChanged event handler to update button statuses
        lvReferences_SelectedIndexChanged(Nothing, Nothing)
    End Sub

    Private Sub butClose_Click(sender As System.Object, e As System.EventArgs) Handles butClose.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub butViewInDocs_Click(sender As Object, e As EventArgs) Handles butViewInDocs.Click

        Try
            Dim refData As UpdateReferenceItem = lvReferences.SelectedItems(0).Tag
            Dim mFilesURLLink As String = refData.GetMFilesURLForFile
            Process.Start(mFilesURLLink)
        Catch ex As Exception
            MessageBox.Show("ERROR: " + ex.Message, My.Resources.Application_Name, MessageBoxButtons.OK, MessageBoxIcon.Error)
            WriteLogFile("ViewReferenceInMFiles ERROR: " + ex.Message)
        End Try
    End Sub

    Private Sub butShowHistory_Click(sender As Object, e As EventArgs) Handles butShowHistory.Click

        Try
            Dim refData As UpdateReferenceItem = lvReferences.SelectedItems(0).Tag
            'Show History dialogue box
            m_vault.ObjectOperations.ShowHistoryDialogModal(Core.Application.MainWindow.Handle, refData.ObjID)
        Catch ex As Exception
            MessageBox.Show("ERROR: " + ex.Message, My.Resources.Application_Name, MessageBoxButtons.OK, MessageBoxIcon.Error)
            WriteLogFile("ShowReferenceRevisions ERROR: " + ex.Message)
        End Try

    End Sub

    Private Sub butSelectVersion_Click(sender As Object, e As EventArgs) Handles butSelectVersion.Click

        Try
            Dim lstItem = lvReferences.SelectedItems(0)
            Dim refData As UpdateReferenceItem = lstItem.Tag
            'Show History dialogue box
            Dim acadWindowHandle = Core.Application.MainWindow.Handle
            Dim result = m_vault.ObjectOperations.ShowSelectObjectHistoryDialogModal(acadWindowHandle, refData.ObjID, "Please select Version")
            If result Is Nothing Then Return

            'repath reference and refresh UI
            refData.RepathReference(result.ObjVer.Version)
            populateReferences()

            'select previously selected item
            Dim refItems = From item As ListViewItem In lvReferences.Items Where item.Name = refData.ReferenceName Select item
            If refItems.Count Then
                refItems.First.Selected = True
                refItems.First.EnsureVisible()
            End If

            'call SelectedIndexChanged event handler to update button statuses
            lvReferences_SelectedIndexChanged(Nothing, Nothing)

        Catch ex As Exception
            MessageBox.Show("ERROR: " + ex.Message, My.Resources.Application_Name, MessageBoxButtons.OK, MessageBoxIcon.Error)
            WriteLogFile("SelectVersion ERROR: " + ex.Message)
        End Try

    End Sub

    Private Sub lvReferences_ColumnReordered(sender As Object, e As ColumnReorderedEventArgs) Handles lvReferences.ColumnReordered

        If e.Header.Index = 0 Then e.Cancel = True
        If e.NewDisplayIndex = 0 Then e.Cancel = True
    End Sub

    Private Sub lvReferences_ColumnWidthChanging(sender As Object, e As ColumnWidthChangingEventArgs) Handles lvReferences.ColumnWidthChanging

        If e.ColumnIndex = 0 Then e.Cancel = True : e.NewWidth = 44
    End Sub

    Private Sub lvReferences_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles lvReferences.ColumnWidthChanged

        Dim column = lvReferences.Columns(e.ColumnIndex)
        If column.Index = 0 And column.Width <> 44 Then column.Width = 44
        If column.Index > 0 And column.Width < 75 Then column.Width = 75
    End Sub

    Private Sub lvReferences_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvReferences.SelectedIndexChanged
        If lvReferences.SelectedItems.Count = 1 Then
            Dim refData As UpdateReferenceItem = lvReferences.SelectedItems(0).Tag
            butViewInDocs.Enabled = refData.CanViewInMFiles
            butShowHistory.Enabled = refData.CanViewInMFiles
            butSelectVersion.Enabled = refData.CanSelectVersion
        Else
            butViewInDocs.Enabled = False
            butShowHistory.Enabled = False
            butSelectVersion.Enabled = False
        End If
    End Sub

    Private Sub lvReferences_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles lvReferences.ColumnClick

        'don't sort first column
        If e.Column = 0 Then Return

        'toggle Ascending/Decending sort order
        If m_sortColumnIndex = e.Column Then
            If m_sortOrder = SortOrder.Ascending Then
                m_sortOrder = SortOrder.Descending
            Else
                m_sortOrder = SortOrder.Ascending
            End If
        Else
            m_sortOrder = SortOrder.Ascending
            m_sortColumnIndex = e.Column
        End If

        'update Sort Order
        updateSortOrder()

    End Sub

    Private Sub updateSortOrder()

        'set sort Icon
        lvReferences.SetSortIcon(m_sortColumnIndex, m_sortOrder)

        'validate sort column index
        If m_sortColumnIndex < 0 Then Return
        If m_sortColumnIndex + 1 > lvReferences.Columns.Count Then Return

        'sort Column
        Dim comparer As docsListViewComparer
        If m_sortColumnIndex < 3 Then
            'default columns
            comparer = New ListViewItemTextComparer
        Else
            'M-Files columns
            Dim sortedColumnHeader As ColumnHeader = lvReferences.Columns(m_sortColumnIndex)
            comparer = getListViewComparerForProperty(m_vault, sortedColumnHeader.Name)
        End If

        comparer.Column = m_sortColumnIndex
        comparer.SortOrder = m_sortOrder
        lvReferences.ListViewItemSorter = comparer

    End Sub

    Private Sub lvReferences_MouseDown(sender As Object, e As MouseEventArgs) Handles lvReferences.MouseDown

        tooltipUpdateReason.Hide(lvReferences)
        Dim hitInfo As ListViewHitTestInfo = lvReferences.HitTest(e.X, e.Y)

        If hitInfo.SubItem Is Nothing Then Return

        Dim columnIndex = hitInfo.Item.SubItems.IndexOf(hitInfo.SubItem)

        Dim refData As UpdateReferenceItem = hitInfo.Item.Tag
        If refData Is Nothing Then Return

        Dim subItem = hitInfo.SubItem

        If columnIndex = 2 Then
            tooltipUpdateReason.ToolTipTitle = refData.VersionStatusDescription
            Dim location = New Point(subItem.Bounds.X + subItem.Bounds.Width + 1, subItem.Bounds.Y + 1)
            tooltipUpdateReason.Show(refData.VersionStatusHelp, lvReferences, location, 10000)
        End If

    End Sub
#End Region

#Region "Custom ListView Drawing Code"
    Private Sub lvReferences_DrawColumnHeader(sender As Object, e As DrawListViewColumnHeaderEventArgs) Handles lvReferences.DrawColumnHeader
        e.DrawDefault = True
    End Sub

    Private Sub lvReferences_DrawItem(sender As Object, e As DrawListViewItemEventArgs) Handles lvReferences.DrawItem
        e.DrawDefault = False

        'draw selection background
        Dim lstItem As ListViewItem = e.Item

        If lstItem.Selected And e.State <> 0 Then
            Dim highlightBrush As New SolidBrush(Color.FromArgb(216, 255, 255))
            e.Graphics.FillRectangle(highlightBrush, e.Bounds)
        End If
    End Sub

    Private Sub lvReferences_DrawSubItem(sender As Object, e As DrawListViewSubItemEventArgs) Handles lvReferences.DrawSubItem

        If e.ItemState = 0 Then Return
        Dim lstItem As ListViewItem = e.Item
        Dim refData As UpdateReferenceItem = lstItem.Tag
        If refData Is Nothing Then Return

        'set text color
        Dim textColorBrush = New SolidBrush(Color.Black)
        'If refData.RequiresAttention Then textColorBrush = New SolidBrush(Color.Red)

        Select Case e.ColumnIndex
            Case 0
                'draw nested icon
                If refData.IsNested Then e.Graphics.DrawImage(imgListIcons.Images("Tree"), e.Bounds.Location.X + 2, e.Bounds.Location.Y + 6)

                'draw archived icon
                If refData.IsArchived Then e.Graphics.DrawImage(imgListIcons.Images("Archived"), e.Bounds.Location.X + 23, e.Bounds.Location.Y + 6)

                'draw check boxes
                If refData.IsCheckable Then
                    If e.Item.Checked Then
                        e.Graphics.DrawImage(imgListIcons.Images("Checked"), e.Bounds.Location.X + 2, e.Bounds.Location.Y + 6)
                    Else
                        e.Graphics.DrawImage(imgListIcons.Images("Unchecked"), e.Bounds.Location.X + 2, e.Bounds.Location.Y + 6)
                        End If
                    End If

            Case 1
                'draw sub item text
                Dim format = New StringFormat()
                format.Trimming = StringTrimming.EllipsisCharacter
                Dim boundingBox = New RectangleF(New Point(e.Bounds.X + 24, e.Bounds.Y + 7), New Size(e.Bounds.Width - 24, 17))
                e.Graphics.DrawString(e.SubItem.Text, lstItem.Font, textColorBrush, boundingBox, format)

                'draw reference type icon
                e.Graphics.DrawImage(imgListIcons.Images(e.Item.ImageKey), e.Bounds.Location.X + 5, e.Bounds.Location.Y + 6)
            Case 2
                textColorBrush = New SolidBrush(refData.ChangeStatusColor)
                Dim format = New StringFormat()
                format.Trimming = StringTrimming.EllipsisCharacter
                Dim boundingBox = New RectangleF(e.Bounds.Location.X + 24, e.Bounds.Location.Y + 7, e.Bounds.Width - 24, 17)
                e.Graphics.DrawString(e.SubItem.Text, lstItem.Font, textColorBrush, boundingBox, format)

                'draw info icon
                If refData.RequiresAttention Then
                    e.Graphics.DrawImage(imgListIcons.Images("Warning"), e.Bounds.Location.X + 5, e.Bounds.Location.Y + 6)
                Else
                    e.Graphics.DrawImage(imgListIcons.Images("Info"), e.Bounds.Location.X + 5, e.Bounds.Location.Y + 6)
                End If

            Case Else
                'draw sub item text
                Dim format = New StringFormat()
                format.Trimming = StringTrimming.EllipsisCharacter
                Dim boundingBox = New RectangleF(New Point(e.Bounds.X + 4, e.Bounds.Y + 7), New Size(e.Bounds.Width - 4, 17))
                e.Graphics.DrawString(e.SubItem.Text, lstItem.Font, textColorBrush, boundingBox, format)
        End Select
    End Sub
#End Region
End Class