Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Public Class frmAcadTemplates

#Region "Win32 Functions"
    Private Const EM_SETCUEBANNER = &H1501

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As Integer, <MarshalAs(UnmanagedType.LPWStr)> lParam As String) As Int32
    End Function
#End Region

#Region "Instance Variables"
    Private m_vault As Vault
    Private m_selectedObjVersion As ObjectVersion
    Private m_vaultSettings As AutoCADVaultSettings
    Private m_sortColumnIndex As Integer = -1
    Private m_sortOrder As SortOrder = SortOrder.None
#End Region

#Region "Properties"
    Public ReadOnly Property TemplateName As String
        Get
            Dim filename As String = m_selectedObjVersion.GetNameForFileSystem(True)
            Return Path.GetFileNameWithoutExtension(filename)
        End Get
    End Property

    Public ReadOnly Property TemplatePath As String
        Get
            Dim objID As ObjID = m_selectedObjVersion.ObjVer.ObjID
            Dim objVersion As Integer = m_selectedObjVersion.ObjVer.Version

            Dim results = From file As ObjectFile In m_selectedObjVersion.Files Where file.Extension.ToLower = "dwg" Select file
            Dim dwgFile = results.First
            Dim fileID As Integer = dwgFile.ID
            Dim fileVersion As Integer = dwgFile.Version
            Dim fullPath As String = m_vault.ObjectFileOperations.GetPathInDefaultViewEx(objID, objVersion, fileID, fileVersion, MFLatestSpecificBehavior.MFLatestSpecificBehaviorLatest)

            Return fullPath
        End Get
    End Property
#End Region

    'constructor
    Public Sub New(Vault As Vault)

        InitializeComponent()

        m_vault = Vault

        'read Vault Settings
        m_vaultSettings = AutoCADVaultSettings.ReadSettings(m_vault)

    End Sub

    Private Sub frmAcadTemplates_Load(sender As Object, e As EventArgs) Handles Me.Load

        'restore template search term
        tbTemplateSearch.Text = My.Settings.TemplateSearchTerm

        'set Search placeholder
        SendMessage(tbTemplateSearch.Handle, EM_SETCUEBANNER, 0, "Search Templates")

        'initialise custom columns
        createCustomColumns()

        'populate templates
        populateDrawingTemplates()

        'restore Window placement
        If My.Settings.TemplateWindowPlacement IsNot Nothing Then My.Settings.TemplateWindowPlacement.restoreWindowPlacement(Me)

        'restore Reference Column settings
        If My.Settings.TemplateColumnCollection IsNot Nothing Then
            My.Settings.TemplateColumnCollection.restoreColumnSettings(lvDrawingTemplates)
            m_sortColumnIndex = My.Settings.TemplateColumnCollection.sortedColumnIndex
            m_sortOrder = My.Settings.TemplateColumnCollection.sortOrder
            updateSortOrder()
        End If
    End Sub

    Private Sub frmAcadTemplates_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

        'store template search term
        My.Settings.TemplateSearchTerm = tbTemplateSearch.Text

        'store Window Placement
        My.Settings.TemplateWindowPlacement = New WindowPlacement(Me)
        My.Settings.TemplateColumnCollection = New ColumnCollection(lvDrawingTemplates, m_sortColumnIndex, m_sortOrder)
        My.Settings.Save()
    End Sub

#Region "Data Population"
    Private Sub createCustomColumns()

        For Each propertyIdentifier In m_vaultSettings.templateColumns
            Dim heading As String = getPropertyName(m_vault, propertyIdentifier)
            Dim column = lvDrawingTemplates.Columns.Add(heading, 250)
            column.Name = propertyIdentifier
        Next
    End Sub

    Sub populateDrawingTemplates()

        Dim srchConditions As New SearchConditions

        'Not Deleted
        Dim srchCondition = New SearchCondition
        Dim expression = New Expression
        Dim typedValue = New TypedValue
        expression.SetStatusValueExpression(MFStatusType.MFStatusTypeDeleted)
        typedValue.SetValue(MFDataType.MFDatatypeBoolean, False)
        srchCondition.Set(expression, MFConditionType.MFConditionTypeEqual, typedValue)
        srchConditions.Add(-1, srchCondition)

        'add template user filter
        If tbTemplateSearch.Text.Trim <> "" Then
            srchCondition = New SearchCondition
            expression = New Expression
            typedValue = New TypedValue
            expression.SetAnyFieldExpression(MFFullTextSearchFlags.MFFullTextSearchFlagsLookInMetaData)
            typedValue.SetValue(MFDataType.MFDatatypeText, tbTemplateSearch.Text.Trim)
            srchCondition.Set(expression, MFConditionType.MFConditionTypeContains, typedValue)
            srchConditions.Add(-1, srchCondition)
        End If

        'Name contains .dwg
        Dim propDef As Integer = m_vault.PropertyDefOperations.GetPropertyDefIDByAlias("prDocumentNumber")
        srchCondition = New SearchCondition
        expression = New Expression
        typedValue = New TypedValue
        expression.SetFileValueExpression(MFFileValueType.MFFileValueTypeFileName)
        typedValue.SetValue(MFDataType.MFDatatypeText, ".dwg")
        srchCondition.Set(expression, MFConditionType.MFConditionTypeContains, typedValue)
        srchConditions.Add(-1, srchCondition)

        'Is Template
        srchCondition = New SearchCondition
        expression = New Expression
        typedValue = New TypedValue
        expression.SetPropertyValueExpression(37, MFParentChildBehavior.MFParentChildBehaviorNone)

        typedValue.SetValue(MFDataType.MFDatatypeBoolean, True)
        srchCondition.Set(expression, MFConditionType.MFConditionTypeEqual, typedValue)
        srchConditions.Add(-1, srchCondition)


        'disable sorting during update
        lvDrawingTemplates.ListViewItemSorter = Nothing

        'clear listview contents
        lvDrawingTemplates.Items.Clear()

        'Perform Search
        Dim oSrchResults As ObjectSearchResults
        Try
            oSrchResults = m_vault.ObjectSearchOperations.SearchForObjectsByConditions(srchConditions, MFSearchFlags.MFSearchFlagNone, True)
        Catch ex As COMException
            'show error tooltip
            Dim errorArray = Split(ex.Message, vbCrLf)
            Dim location = New Point(tbTemplateSearch.Bounds.X + 10, tbTemplateSearch.Bounds.Y + 60)
            tooltipSearchError.Hide(tbTemplateSearch)
            tooltipSearchError.ToolTipTitle = "Invalid Search"
            tooltipSearchError.ToolTipIcon = ToolTipIcon.Warning
            tooltipSearchError.Show(errorArray.First, Me, location, 5000)
            Return
        End Try

        'iterate matching templates and add to listview
        lvDrawingTemplates.BeginUpdate()
        For Each objVersionTemplates As ObjectVersion In oSrchResults
            Dim lstItem = lvDrawingTemplates.Items.Add("", 0)
            lstItem.Tag = objVersionTemplates

            Dim namedValues = m_vault.ObjectPropertyOperations.GetPropertiesForMetadataSync(objVersionTemplates.ObjVer, MFMetadataSyncFormat.MFMetadataSyncFormatPowerPoint)

            For Each column As Windows.Forms.ColumnHeader In lvDrawingTemplates.Columns
                'skip icon column
                If column.Index = 0 Then Continue For

                'add class name with Id
                If column.Index = 1 Then
                    lstItem.SubItems.Add(namedValues("P100") + " (" + objVersionTemplates.Class.ToString + ")")
                    Continue For
                End If

                Dim propertyValue As String = ""
                If IsDBNull(namedValues(column.Name)) Then
                    propertyValue = "---"
                Else
                    propertyValue = namedValues(column.Name)
                    'Empty values contain the column name in square brackets, subsitute this for ---
                    If propertyValue = "[" + column.Text + "]" Then propertyValue = "---"
                End If
                'add column value
                lstItem.SubItems.Add(propertyValue)
            Next
        Next
        lvDrawingTemplates.EndUpdate()

    End Sub
#End Region

#Region "User Interface"
    Private Sub lvDrawingTemplates_DoubleClick(sender As Object, e As EventArgs) Handles lvDrawingTemplates.DoubleClick

        If lvDrawingTemplates.SelectedItems.Count Then btOK_Click(btOK, Nothing)
    End Sub

    Private Sub btOK_Click(sender As System.Object, e As System.EventArgs) Handles btOK.Click

        m_selectedObjVersion = lvDrawingTemplates.SelectedItems(0).Tag
        DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub

    Private Sub btCancel_Click(sender As System.Object, e As System.EventArgs) Handles btCancel.Click

        DialogResult = System.Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub lvDrawingTemplates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvDrawingTemplates.SelectedIndexChanged

        btOK.Enabled = lvDrawingTemplates.SelectedItems.Count
    End Sub

    Private Sub lvDrawingTemplates_ColumnReordered(sender As Object, e As System.Windows.Forms.ColumnReorderedEventArgs) Handles lvDrawingTemplates.ColumnReordered

        If e.Header.Index = 0 Then e.Cancel = True
        If e.NewDisplayIndex = 0 Then e.Cancel = True
    End Sub

    Private Sub lvDrawingTemplates_ColumnWidthChanging(sender As Object, e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles lvDrawingTemplates.ColumnWidthChanging

        If e.ColumnIndex = 0 Then e.Cancel = True : e.NewWidth = 35
    End Sub

    Private Sub lvDrawingTemplates_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles lvDrawingTemplates.ColumnWidthChanged

        Dim column As ColumnHeader = lvDrawingTemplates.Columns(e.ColumnIndex)
        If column.Index = 0 And column.Width <> 35 Then column.Width = 35
        If column.Index > 0 And column.Width < 50 Then column.Width = 50
    End Sub

    Private Sub lvDrawingTemplates_ColumnClick(sender As Object, e As System.Windows.Forms.ColumnClickEventArgs) Handles lvDrawingTemplates.ColumnClick

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
        lvDrawingTemplates.SetSortIcon(m_sortColumnIndex, m_sortOrder)

        'validate sort column index
        If m_sortColumnIndex < 0 Then Return
        If m_sortColumnIndex + 1 > lvDrawingTemplates.Columns.Count Then Return

        'sort Column
        Dim sortedColumnHeader As System.Windows.Forms.ColumnHeader = lvDrawingTemplates.Columns(m_sortColumnIndex)
        Dim comparer As docsListViewComparer = getListViewComparerForProperty(m_vault, sortedColumnHeader.Name)
        comparer.Column = m_sortColumnIndex
        comparer.SortOrder = m_sortOrder
        lvDrawingTemplates.ListViewItemSorter = comparer
    End Sub

    Private Sub butFilterTemplates_Click(sender As Object, e As EventArgs) Handles butFilterTemplates.Click

        'check for invalid filter entry 
        If tbTemplateSearch.Text.Trim = "" Then tbTemplateSearch.Text = ""

        'populate templates
        populateDrawingTemplates()

        'update sort order
        updateSortOrder()

    End Sub

    Private Sub butClearFilter_Click(sender As Object, e As EventArgs) Handles butClearFilter.Click

        'clear filter entry
        tbTemplateSearch.Text = ""

        'populate templates
        populateDrawingTemplates()

        'update sort order
        updateSortOrder()
    End Sub

    Private Sub tbTemplateFilter_KeyDown(sender As Object, e As KeyEventArgs) Handles tbTemplateSearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            butFilterTemplates_Click(Nothing, Nothing)
        End If
    End Sub

#End Region
End Class