Imports System.Drawing
Imports System.Windows.Forms
Imports MFilesAPI

Public Class frmSetItemProperties

#Region " Properties "
    Public Property viewArea As ViewArea

#End Region

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

#Region " Form Methods "
    Private Sub frmSetItemProperties_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        PictureBox1.Image = viewArea.SmallThumbnail

        cbTemplate.Items.Clear()
        cbTemplate.DataSource = MFilesHelper.Templates
        cbTemplate.DisplayMember = "Filename"
        cbTemplate.ValueMember = "Filename"

        Dim index As Integer = cbTemplate.FindString(viewArea.Template)
        If index = -1 Then Exit Sub
        cbTemplate.SelectedIndex = index

        chkDWG.Checked = viewArea.ExportDWG.Export
        chkPDF.Checked = viewArea.ExportPDF.Export
        ''load all the properties

        lvItemProps.Items.Clear()
        lvItemProps.BeginUpdate()

        'lvItemProps.Columns(0).DisplayIndex = lvItemProps.Columns.Count - 1

        For Each _propWrap As PropertyWrapper In MFilesHelper.ClassProperties

            Dim lstitem As New ListViewItem
            lstitem.UseItemStyleForSubItems = False
            lstitem.Tag = _propWrap
            lstitem.Text = ""
            lstitem.SubItems.Add(_propWrap.PropertyDescription(MFilesHelper.Vault))
            lstitem.SubItems.Add(_propWrap.DataTypeDescription(MFilesHelper.Vault))
            lstitem.SubItems.Add("")
            lstitem.Checked = False

            If _propWrap.RequiredProperty Then lstitem.SubItems(1).Font = New Font(lvItemProps.Font, FontStyle.Bold)

            Dim pIndex As Integer = -1
            ''have we set this to an override value? e.g. we dont want to use the 'default'
            pIndex = viewArea.CustomObjectProperties.FindIndex(Function(p) p.PropertyID = _propWrap.PropertyID)
            If pIndex > -1 Then
                ''yes we have so set this in the 'value'
                lstitem.SubItems(3).Text = viewArea.CustomObjectProperties(pIndex).Value
                lstitem.Checked = True
            Else
                ''does this property have a default value already set in the settings?
                pIndex = PluginSettings.DefaultClassProperties.FindIndex(Function(p)
                                                                             Return p.PropertyID = _propWrap.PropertyID
                                                                         End Function)
                If Not pIndex = -1 Then lstitem.SubItems(3).Text = PluginSettings.DefaultClassProperties(pIndex).Value
            End If

            If Not lstitem.Checked Then
                For Each sItem As ListViewItem.ListViewSubItem In lstitem.SubItems
                    sItem.BackColor = Color.LightGray
                    If sItem.Font.Bold Then
                        sItem.Font = New Font(lvItemProps.Font, FontStyle.Italic Or FontStyle.Bold)
                    Else
                        sItem.Font = New Font(lvItemProps.Font, FontStyle.Italic)
                    End If
                Next
            End If

            lvItemProps.Items.Add(lstitem)
        Next

        lvItemProps.EndUpdate()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        DialogResult = Windows.Forms.DialogResult.OK
    End Sub

    Private Sub cbTemplate_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbTemplate.SelectedIndexChanged
        Dim dt As DrawingTemplate = cbTemplate.SelectedItem

        cbLayout.Items.Clear()
        For Each str As String In dt.Layouts
            cbLayout.Items.Add(str)
        Next


        Dim index As Integer = cbLayout.FindString(viewArea.LayoutName)
        If index = -1 Then Exit Sub
        cbLayout.SelectedIndex = index

    End Sub

    Private Sub lvItemProps_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles lvItemProps.MouseDoubleClick
        Dim hitTestInfo = lvItemProps.HitTest(e.Location)
        If hitTestInfo.Item Is Nothing Then Return

        Dim lstItem = hitTestInfo.Item

        ''which column are we in?
        Dim colIndex As Integer = lstItem.SubItems.IndexOf(hitTestInfo.SubItem)
        If colIndex = 0 Then Exit Sub

        If Not lstItem.Checked Then Exit Sub

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

    Private Sub showTextValueEntry(lstItem As ListViewItem, mapping As PropertyWrapper)
        Dim value As String = ""
        value = mapping.Value
        Dim frmAddTextUI As New frmInputDialog("Add Text Value", "Value", value)


        Dim classWrap As ObjectClassWrapper = MFilesHelper.ObjectClassWrapper
        frmAddTextUI.UseDocumentName = mapping.PropertyID = classWrap.ObjectClass.NamePropertyDef

        frmAddTextUI.StartPosition = FormStartPosition.CenterParent
        If frmAddTextUI.ShowDialog(Me) = DialogResult.OK Then
            mapping.Value = frmAddTextUI.getTextValue
            mapping.UseDocumentName = mapping.PropertyID = classWrap.ObjectClass.NamePropertyDef

            lstItem.SubItems(3).Text = mapping.ValueDescription(MFilesHelper.Vault)
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

                lstItem.SubItems(3).Text = mapping.ValueDescription(MFilesHelper.Vault)
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

        For Each lstItem In lvItemProps.Items
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

            Dim results = From mapping In MFilesHelper.ClassProperties Where mapping.MFilesPropertyDef(MFilesHelper.Vault).ValueList = ObjType.OwnerType Select mapping
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

    Private Sub lvItemProps_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles lvItemProps.ItemCheck
        If e.NewValue Then
            Dim lvItem = lvItemProps.Items(e.Index)
            For Each sItem As ListViewItem.ListViewSubItem In lvItem.SubItems
                sItem.BackColor = Color.White

                If sItem.Font.Bold Then
                    sItem.Font = New Font(lvItemProps.Font, FontStyle.Bold)
                Else
                    sItem.Font = New Font(lvItemProps.Font, FontStyle.Regular)
                End If
            Next

        Else
            Dim lvItem = lvItemProps.Items(e.Index)
            For Each sItem As ListViewItem.ListViewSubItem In lvItem.SubItems
                sItem.BackColor = Color.LightGray
                If sItem.Font.Bold Then
                    sItem.Font = New Font(lvItemProps.Font, FontStyle.Italic Or FontStyle.Bold)
                Else
                    sItem.Font = New Font(lvItemProps.Font, FontStyle.Italic)
                End If

            Next
        End If
    End Sub


#End Region


End Class