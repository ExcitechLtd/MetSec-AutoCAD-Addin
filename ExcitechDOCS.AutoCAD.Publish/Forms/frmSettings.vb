Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports DevExpress.Utils
Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports MFilesAPI

Public Class frmSettings

#Region " Private "
    Private _classes As List(Of ObjectClassWrapper)
    Private _properties As List(Of PropertyWrapper)
    Private _imgList As ImageList

    Private _ischecking As Boolean = False
#End Region

#Region " Constrcutor "

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _imgList = New ImageList
        _imgList.Images.Add("LOCAL", My.Resources.localdisk_16)
        _imgList.Images.Add("EXDOCS", My.Resources.DOCS_16)
    End Sub

#End Region

#Region " Form Events "

    Private Sub frmSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _classes = MFilesHelper.GetVaultClasses

        'bind combobox
        cmbClass.DisplayMember = "ToString"
        cmbClass.ValueMember = "ObjectClass"
        cmbClass.DataSource = _classes

        txtLocalTemplatePath.Text = PluginSettings.LocalTemplatePath
        chkIncludeDOCS.Checked = PluginSettings.CheckDOCSForTemplates

        PopulateTemplateListView()

        ''set the default template if there was one in the settings
        If Not PluginSettings.DefaultTemplate Is Nothing Then
            Dim _i As Integer = MFilesHelper.Templates.FindIndex(Function(dt)
                                                                     Return dt.Location = PluginSettings.DefaultTemplate.Location AndAlso dt.TemplateName = PluginSettings.DefaultTemplate.TemplateName
                                                                 End Function)

            If _i > -1 Then
                MFilesHelper.Templates(_i).Default = True

                If MFilesHelper.Templates(_i).Layouts.Contains(PluginSettings.DefaultTemplate.LayoutName) Then
                    MFilesHelper.Templates(_i).LayoutName = PluginSettings.DefaultTemplate.LayoutName
                End If
            End If


        End If

            ''set the selected class (if any)
            If Not PluginSettings.DefaultClass = -1 Then
            Dim isSet As Boolean = False
            For Each _class As ObjectClassWrapper In cmbClass.Items
                If _class.ObjectClass.ID = PluginSettings.DefaultClass Then
                    cmbClass.SelectedItem = _class
                    isSet = True
                    Exit For
                End If
            Next

            If isSet Then
                For Each propWrap As PropertyWrapper In PluginSettings.DefaultClassProperties
                    Dim item As ListViewItem = lvProperties.FindItemWithText(propWrap.PropertyDescription(MFilesHelper.Vault))
                    If Not item Is Nothing Then
                        item.SubItems(2).Text = propWrap.Value
                        item.Tag = propWrap
                    End If
                Next
            End If
        Else
            ''set this to generic document
            For Each _class As ObjectClassWrapper In cmbClass.Items
                If _class.ObjectClass.ID = 0 Then
                    cmbClass.SelectedItem = _class
                    Exit For
                End If
            Next
        End If

    End Sub

    Private Sub cmbClass_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbClass.SelectedIndexChanged

        _properties = MFilesHelper.GetClassProperties(cmbClass.SelectedItem)
        popualteListview(_properties)

    End Sub

    Private Sub chkShowAllProps_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowAllProps.CheckedChanged
        popualteListview(_properties)
    End Sub

    Private Sub lvProperties_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvProperties.SelectedIndexChanged

    End Sub

    Private Sub lvProperties_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles lvProperties.MouseDoubleClick
        Dim hitTestInfo = lvProperties.HitTest(e.Location)
        If hitTestInfo.Item Is Nothing Then Return

        Dim lstItem = hitTestInfo.Item
        Dim propWrap As PropertyWrapper = hitTestInfo.Item.Tag

        'Dim lstItem = lvPropertyMappings.SelectedItems(0)
        'Dim mapping As docsImportPropertyMapping = lstItem.Tag
        Dim propDef = MFilesHelper.Vault.PropertyDefOperations.GetPropertyDef(propWrap.PropertyID)

        Select Case propDef.DataType
            Case MFDataType.MFDatatypeText
                showTextValueEntry(lstItem, propWrap)
            Case MFDataType.MFDatatypeLookup
                showLookupValueList(lstItem, propWrap)
        End Select

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

    Private Sub btnSaveSettings_Click(sender As Object, e As EventArgs) Handles btnSaveSettings.Click
        ''validate the settings before we save and exit

        If Not ValidateSettings() Then Exit Sub

        PluginSettings.DefaultClass = DirectCast(cmbClass.SelectedItem, ObjectClassWrapper).ObjectClass.ID

        Dim tempProperties As New List(Of PropertyWrapper)
        For Each lstItem As ListViewItem In lvProperties.Items
            tempProperties.Add(lstItem.Tag)
        Next

        PluginSettings.DefaultClassProperties = tempProperties

        ''templates
        PluginSettings.CheckDOCSForTemplates = chkIncludeDOCS.Checked
        PluginSettings.LocalTemplatePath = txtLocalTemplatePath.Text

        Dim i As Integer = MFilesHelper.Templates.FindIndex(Function(_dt) _dt.Default)

        If i > -1 Then
            PluginSettings.DefaultTemplate = MFilesHelper.Templates(i)
        Else
            PluginSettings.DefaultTemplate = Nothing
        End If

        Dim settingsFile As String = IO.Path.Combine(Settings.SettingsPath, "AutoCAD.Publish.xml")
        Settings.SaveSettings(settingsFile, PluginSettings)

        Close()
    End Sub

    Private Function ValidateSettings() As Boolean
        Dim ret As Boolean = True

        ''if we dont hav any local template and and we arent search mfiles then throw an error we need toat least one of these
        If String.IsNullOrWhiteSpace(txtLocalTemplatePath.Text) And Not chkIncludeDOCS.Checked Then
            MessageBox.Show("No template locations set, please select a local path and/or searching for templates in Excitech DOCS", "Validate Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            ret = False
            Return ret
        End If

        ''if we have any items loaded in to the template list view then make sure one of them is checked
        If MFilesHelper.Templates.FindIndex(Function(dt) dt.Default) < 0 Then
            MessageBox.Show("No Default template has been selected, please select a local path and/or searching for templates in Excitech DOCS", "Validate Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            ret = False
            Return ret
        End If

        ''check that the requried properties have been set
        For Each lvItem As ListViewItem In lvProperties.Items
            Dim propWrap As PropertyWrapper = lvItem.Tag
            If propWrap.RequiredProperty Then
                If lvItem.SubItems(2).Text = "" And Not propWrap.UseDocumentName Then

                    MessageBox.Show("All required properties must have a value, please check the class properties are set and have a valid value", "Validate Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    ret = False
                    Return ret

                End If
            End If
        Next

        Return ret
    End Function

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        MFilesHelper.populateDrawingTemplates(txtLocalTemplatePath.Text, chkIncludeDOCS.Checked)

        'If Not String.IsNullOrWhiteSpace(txtLocalTemplatePath.Text) Then
        '    ''see if we have any DWT files
        '    Dim dInfo As New IO.DirectoryInfo(txtLocalTemplatePath.Text)

        '    For Each _file As IO.FileInfo In dInfo.GetFiles("*.dwt")
        '        ''get the filename
        '        Dim tmpl As New DrawingTemplate(_file.FullName)
        '        _templates.Add(tmpl)
        '    Next

        'End If

        'If chkIncludeDOCS.Checked Then
        '    ''search mfiles for tempaltes
        '    _templates.AddRange(MFilesHelper.populateDrawingTemplates())
        'End If

        PopulateTemplateListView()
    End Sub

    Private Sub cmdSelectFolder_Click(sender As Object, e As EventArgs) Handles cmdSelectFolder.Click
        With FolderBrowserDialog1
            If .ShowDialog = DialogResult.OK Then
                txtLocalTemplatePath.Text = .SelectedPath
            End If
        End With
    End Sub

    Private Sub lvTemplates_DrawSubItem(sender As Object, e As DrawListViewSubItemEventArgs)

        If Not e.ColumnIndex = 1 Then
            e.DrawDefault = True
            Return
        End If

        e.DrawBackground()
        Dim rect As New Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Height, e.Bounds.Height)

        Select Case DirectCast(e.Item.Tag, DrawingTemplate).Location
            Case DrawingTemplate.TemplateLocation.DISK
                e.Graphics.DrawImage(My.Resources.localdisk_16, rect)
            Case DrawingTemplate.TemplateLocation.MFILES
                e.Graphics.DrawImage(My.Resources.DOCS_16, rect)
        End Select

    End Sub

    Private Sub lvTemplates_DrawColumnHeader(sender As Object, e As DrawListViewColumnHeaderEventArgs)
        e.DrawDefault = True
    End Sub

    'Private Sub lvTemplates_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles lvTemplates.ItemCheck
    '    If _ischecking Then Return

    '    _ischecking = True

    '    If e.NewValue = CheckState.Checked Then
    '        For Each lvItem As ListViewItem In lvTemplates.Items
    '            If lvItem.Index = e.Index Then Continue For

    '            lvItem.Checked = False
    '        Next

    '        lvTemplates.Items(e.Index).Checked = True
    '    End If

    '    _ischecking = False
    'End Sub

    Private Sub lvTemplates_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs)
        If e.IsSelected Then
            e.Item.Selected = False
            e.Item.Focused = False
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub popualteListview(properties As List(Of PropertyWrapper))
        lvProperties.Items.Clear()
        lvProperties.BeginUpdate()

        For Each _propWrap As PropertyWrapper In _properties
            If Not chkShowAllProps.Checked And Not _propWrap.RequiredProperty Then Continue For

            Dim lstitem As New ListViewItem
            lstitem.UseItemStyleForSubItems = False
            lstitem.Tag = _propWrap
            lstitem.Text = _propWrap.PropertyDescription(MFilesHelper.Vault)
            lstitem.SubItems.Add(_propWrap.DataTypeDescription(MFilesHelper.Vault))
            lstitem.SubItems.Add("")

            If _propWrap.RequiredProperty Then lstitem.SubItems(0).Font = New Font(lvProperties.Font, FontStyle.Bold)

            lvProperties.Items.Add(lstitem)
        Next

        lvProperties.EndUpdate()
    End Sub

    Private Sub showTextValueEntry(lstItem As ListViewItem, mapping As PropertyWrapper)
        Dim value As String = ""
        value = mapping.Value
        Dim frmAddTextUI As New frmInputDialog("Add Text Value", "Value", value)

        Dim classWrap As ObjectClassWrapper = cmbClass.SelectedItem
        frmAddTextUI.UseDocumentName = mapping.PropertyID = classWrap.ObjectClass.NamePropertyDef

        frmAddTextUI.StartPosition = FormStartPosition.CenterParent
        If frmAddTextUI.ShowDialog(Me) = DialogResult.OK Then
            mapping.Value = frmAddTextUI.getTextValue
            mapping.UseDocumentName = mapping.PropertyID = classWrap.ObjectClass.NamePropertyDef

            lstItem.SubItems(2).Text = mapping.ValueDescription(MFilesHelper.Vault)
            lstItem.Tag = mapping
        End If
    End Sub

    Private Sub showLookupValueList(lstItem As ListViewItem, mapping As PropertyWrapper)

        Try
            Dim propDef = MFilesHelper.Vault.PropertyDefOperations.GetPropertyDef(mapping.PropertyID)
            Dim valueDictionary = GetValueDictionary(propDef)

            Dim lookupID As Integer = 0
            lookupID = mapping.ValueID

            Dim frmValueConfigUI As New frmMFilesValueChooser(lookupID, valueDictionary)
            frmValueConfigUI.StartPosition = FormStartPosition.CenterParent

            If frmValueConfigUI.ShowDialog(Me) = DialogResult.OK Then
                If Not frmValueConfigUI.ValueID.Equals(mapping.ValueID) Then resetSubTypeMappings(propDef)
                mapping.Value = frmValueConfigUI.ValueDisplay
                mapping.ValueID = frmValueConfigUI.ValueID

                lstItem.SubItems(2).Text = mapping.ValueDescription(MFilesHelper.Vault)
                lstItem.Tag = mapping
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

    Private Function GetValueDictionary(mFilesPropertyDef As PropertyDef) As Object

        Dim objType = MFilesHelper.Vault.ValueListOperations.GetValueList(mFilesPropertyDef.ValueList)
        If objType.RealObjectType Then
            Return objTypeDictionary(objType)
        Else
            Return valueListDictionary(objType)
        End If

        Return Nothing
    End Function

    Private Sub resetSubTypeMappings(mFilesPropertyDef As PropertyDef)

        Dim subTypeList = MFilesHelper.GetSubTypeList(mFilesPropertyDef.ID)

        For Each lstItem In lvProperties.Items
            Dim mapping As PropertyWrapper = lstItem.tag

            Try
                Dim propDef = mapping.MFilesPropertyDef(MFilesHelper.Vault)
                Dim objType = MFilesHelper.Vault.ObjectTypeOperations.GetObjectType(propDef.ValueList)
                If subTypeList.Contains(objType.OwnerType) Then
                    mapping.Value = Nothing

                    lstItem.SubItems(2).Text = mapping.ValueDescription(MFilesHelper.Vault)
                    lstItem.Tag = mapping

                End If
            Catch ex As Exception
            End Try
        Next
    End Sub

    Private Function objTypeDictionary(ObjType As ObjType)

        Dim srchConditions As New SearchConditions
        Dim oScValueListItem As SearchCondition

        If ObjType.HasOwnerType Then
            Dim ownerObjType = MFilesHelper.Vault.ObjectTypeOperations.GetObjectType(ObjType.OwnerType)

            'Dim mfilesMappings = From mapping As PropertyWrapper In PropertyMappings.Values Where mapping.PropertyType = PropertyTypes.MFiles Select mapping

            Dim results = From mapping In _properties Where mapping.MFilesPropertyDef(MFilesHelper.Vault).ValueList = ObjType.OwnerType Select mapping
            If results.Count = 0 Then Throw New Exception("Owner property '" + ObjType.NameSingular + "' not found in Class properties.")
            If results.Count > 0 AndAlso Not IsNumeric(results.First.ValueID) Then Throw New Exception("Owner property '" + ownerObjType.NameSingular + "' needs to be set to a Value Mapping.")

            'owner property
            oScValueListItem = New SearchCondition
            oScValueListItem.Expression.SetValueListItemExpression(MFValueListItemPropertyDef.MFValueListItemPropertyDefOwner, MFParentChildBehavior.MFParentChildBehaviorNone)
            oScValueListItem.ConditionType = MFConditionType.MFConditionTypeEqual
            oScValueListItem.TypedValue.SetValue(MFDataType.MFDatatypeLookup, results.First.ValueID)
            srchConditions.Add(-1, oScValueListItem)
        End If

        'not deleted
        oScValueListItem = New SearchCondition
        oScValueListItem.Expression.SetValueListItemExpression(MFValueListItemPropertyDef.MFValueListItemPropertyDefDeleted, MFParentChildBehavior.MFParentChildBehaviorNone)
        oScValueListItem.ConditionType = MFConditionType.MFConditionTypeEqual
        oScValueListItem.TypedValue.SetValue(MFDataType.MFDatatypeBoolean, False)
        srchConditions.Add(-1, oScValueListItem)

        ' Search for the value list item.
        Dim valueListItems = MFilesHelper.Vault.ValueListItemOperations.SearchForValueListItemsEx(ObjType.ID, srchConditions)

        Dim dictionary As New Dictionary(Of Integer, String)
        For Each valueListItem As ValueListItem In valueListItems
            dictionary.Add(valueListItem.ID.ToString, valueListItem.Name)
        Next

        Return dictionary
    End Function

    Private Function valueListDictionary(ObjType As ObjType)

        Dim valListItems = MFilesHelper.Vault.ValueListItemOperations.GetValueListItems(ObjType.ID)

        Dim dictionary As New Dictionary(Of Integer, String)
        For Each valueItem As ValueListItem In valListItems
            dictionary.Add(valueItem.ID.ToString, valueItem.Name)
        Next
        Return dictionary
    End Function

    Private Sub PopulateTemplateListView()
        ''
        gridTemplates.DataSource = MFilesHelper.Templates
    End Sub


#End Region

#Region " Template Methods "

    Private Sub RepositoryItemComboBox1_QueryPopUp(sender As Object, e As CancelEventArgs) Handles RepositoryItemComboBox1.QueryPopUp
        Dim comboEdit As ComboBoxEdit = sender
        comboEdit.Properties.Items.Clear()

        Dim _template As DrawingTemplate = GridView1.GetFocusedRow
        comboEdit.Properties.Items.AddRange(_template.Layouts)

    End Sub

    Private Function InSelectedCell(ByVal e As MouseEventArgs) As Boolean
        Dim hitInfo As GridHitInfo = GridView1.CalcHitInfo(e.Location)
        Return hitInfo.InRowCell
    End Function

    Private Sub GridView1_MouseDown(sender As Object, e As MouseEventArgs) Handles GridView1.MouseDown
        If InSelectedCell(e) Then
            Dim hi As GridHitInfo = GridView1.CalcHitInfo(e.Location)
            ''
            'Dim viewArea As ViewArea = AdvBandedGridView1.GetFocusedRow
            'ShowLargeThumbnail(viewArea)

            If GridView1.FocusedRowHandle = hi.RowHandle Then
                GridView1.FocusedColumn = hi.Column
                DXMouseEventArgs.GetMouseArgs(e).Handled = True
            End If
        End If
    End Sub

    Private Sub GridView1_MouseUp(sender As Object, e As MouseEventArgs) Handles GridView1.MouseUp

        Dim hi As GridHitInfo = GridView1.CalcHitInfo(e.Location)
        Dim viewArea As DrawingTemplate = GridView1.GetFocusedRow

        If InSelectedCell(e) Then

            DXMouseEventArgs.GetMouseArgs(e).Handled = True
            'advancedBandedView.ShowEditorByMouse()

            GridView1.FocusedRowHandle = hi.RowHandle
            GridView1.FocusedColumn = hi.Column
            GridView1.ShowEditor()

            Dim edit = TryCast(GridView1.ActiveEditor, CheckEdit)
            If edit IsNot Nothing Then
                edit.Toggle()
                TryCast(e, DevExpress.Utils.DXMouseEventArgs).Handled = True
            End If

            Dim combox = TryCast(GridView1.ActiveEditor, ComboBoxEdit)
            If combox IsNot Nothing Then
                combox.ShowPopup()
                TryCast(e, DevExpress.Utils.DXMouseEventArgs).Handled = True
            End If

        End If
    End Sub


    Private Sub editDefault_EditValueChanging(sender As Object, e As ChangingEventArgs) Handles editDefault.EditValueChanging
        Dim cnt As Integer = MFilesHelper.Templates.LongCount(Function(dt) dt.Default)

        If e.OldValue = True And e.NewValue = False And cnt < 0 Then
            e.Cancel = True
            Exit Sub
        End If

        If e.NewValue Then
            For Each dt In MFilesHelper.Templates.Where(Function(_dt) _dt.Default)
                dt.Default = False
            Next
        End If
    End Sub

    Private Sub GridView1_CellValueChanging(sender As Object, e As CellValueChangedEventArgs) Handles GridView1.CellValueChanging
        'get the current selection
        Dim selectedRows = GridView1.GetSelectedRows
        For Each rowHandle In selectedRows
            Dim dTemplate As DrawingTemplate = GridView1.GetRow(rowHandle)
            dTemplate.SetProperty(e.Column.FieldName, e.Value)
        Next

        GridView1.RefreshData()
    End Sub

    Private Sub editDefault_EditValueChanged(sender As Object, e As EventArgs) Handles editDefault.EditValueChanged

    End Sub



#End Region

End Class