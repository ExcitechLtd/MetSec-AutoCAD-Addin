Imports MFilesAPI

Public Class MFilesHelper
    Public Shared Vault As Vault
    Public Shared Templates As List(Of DrawingTemplate)
    Public Shared ClassProperties As List(Of PropertyWrapper)
    Public Shared ObjectClassWrapper As ObjectClassWrapper

    Public Shared Function GetObjectClassWrapper(ClassID As Integer) As ObjectClassWrapper
        ObjectClassWrapper = New ObjectClassWrapper(Vault, ClassID)
        Return ObjectClassWrapper
    End Function

    Public Shared Sub populateDrawingTemplates(localPath As String, includeMFiles As Boolean)
        LoggingHelper.WriteToLog("Initalise templates...")

        Templates = New List(Of DrawingTemplate)

        If Not String.IsNullOrWhiteSpace(localPath) AndAlso IO.Directory.Exists(localPath) Then
            LoggingHelper.WriteToLog("Checking for local templates...")
            ''see if we have any DWT files
            Dim dInfo As New IO.DirectoryInfo(localPath)

            For Each _file As IO.FileInfo In dInfo.GetFiles("*.dwt")
                ''get the filename
                Dim tmpl As New DrawingTemplate(_file.FullName)
                tmpl.Layouts = AutoCADHelper.GetLayoutNamesFromFile(_file.FullName)
                tmpl.LayoutName = tmpl.Layouts(0)
                Templates.Add(tmpl)
            Next
        End If

        If includeMFiles Then
            LoggingHelper.WriteToLog("Checking for MFiles templates...")
            Dim srchConditions As New SearchConditions

            'Not Deleted
            Dim srchCondition = New SearchCondition
            Dim expression = New Expression
            Dim typedValue = New TypedValue
            expression.SetStatusValueExpression(MFStatusType.MFStatusTypeDeleted)
            typedValue.SetValue(MFDataType.MFDatatypeBoolean, False)
            srchCondition.Set(expression, MFConditionType.MFConditionTypeEqual, typedValue)
            srchConditions.Add(-1, srchCondition)

            ''add template user filter
            'If tbTemplateSearch.Text.Trim <> "" Then
            '    srchCondition = New SearchCondition
            '    expression = New Expression
            '    typedValue = New TypedValue
            '    expression.SetAnyFieldExpression(MFFullTextSearchFlags.MFFullTextSearchFlagsLookInMetaData)
            '    typedValue.SetValue(MFDataType.MFDatatypeText, tbTemplateSearch.Text.Trim)
            '    srchCondition.Set(expression, MFConditionType.MFConditionTypeContains, typedValue)
            '    srchConditions.Add(-1, srchCondition)
            'End If

            'Name contains .dwg
            Dim propDef As Integer = Vault.PropertyDefOperations.GetPropertyDefIDByAlias("prDocumentNumber")
            srchCondition = New SearchCondition
            expression = New Expression
            typedValue = New TypedValue
            expression.SetFileValueExpression(MFFileValueType.MFFileValueTypeFileName)
            typedValue.SetValue(MFDataType.MFDatatypeText, ".dwt")
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

            'Perform Search
            Dim oSrchResults As ObjectSearchResults
            Try
                oSrchResults = Vault.ObjectSearchOperations.SearchForObjectsByConditions(srchConditions, MFSearchFlags.MFSearchFlagNone, True)
            Catch ex As Runtime.InteropServices.COMException
                MsgBox(ex.Message)

            End Try

            'iterate matching templates and add to listview
            For Each objVersionTemplates As ObjectVersion In oSrchResults
                Dim namedValues = Vault.ObjectPropertyOperations.GetPropertiesForMetadataSync(objVersionTemplates.ObjVer, MFMetadataSyncFormat.MFMetadataSyncFormatPowerPoint)

                Dim _template As New DrawingTemplate(objVersionTemplates)
                '_template.ObjNameTitle = objVersionTemplates.Title
                _template.ObjClass = namedValues("P100") + " (" + objVersionTemplates.Class.ToString + ")"
                '_template.ObjectVersion = objVersionTemplates
                _template.objVer = objVersionTemplates.ObjVer

                ''save the template temporaily
                Dim _fpath As String = GetDefaultViewPath(_template)
                _template.Layouts = AutoCADHelper.GetLayoutNamesFromFile(_fpath)
                _template.LayoutName = _template.Layouts(0)
                _template.Location = DrawingTemplate.TemplateLocation.MFILES
                Templates.Add(_template)
            Next
        End If

        LoggingHelper.WriteToLog("Found: " + Templates.Count.ToString + " templates")
    End Sub

    Public Shared Function GetSubTypeList(PropertyID As Integer) As List(Of Integer)

        Dim subTypeList As New List(Of Integer)
        Try
            Dim propDef = Vault.PropertyDefOperations.GetPropertyDef(PropertyID)
            Dim objTypes = Vault.ObjectTypeOperations.GetObjectTypes
            linkSubTypes(objTypes, propDef.ValueList, subTypeList)
        Catch ex As Exception
        End Try
        Return subTypeList

    End Function

    Private Shared Sub linkSubTypes(objTypes As ObjTypes, objTypeID As Integer, SubTypeList As List(Of Integer))

        SubTypeList.Add(objTypeID)
        For Each objtype As ObjType In objTypes
            If objtype.HasOwnerType AndAlso objtype.OwnerType = objTypeID Then
                linkSubTypes(objTypes, objtype.ID, SubTypeList)
            End If
        Next
    End Sub

    Public Shared Function GetVaultClasses() As List(Of ObjectClassWrapper)
        Dim objClassList As New List(Of ObjectClassWrapper)
        Dim objClasses = Vault.ClassOperations.GetAllObjectClasses

        For Each objClass As ObjectClass In objClasses
            If Not Vault.ObjectTypeOperations.GetObjectType(objClass.ObjectType).CanHaveFiles Then Continue For

            If objClass.ID < 0 Then Continue For
            Dim item As New ObjectClassWrapper(Vault, objClass)

            objClassList.Add(item)
        Next

        'sort classes
        objClassList.Sort(Function(x, y) x.ToString.CompareTo(y.ToString))

        Return objClassList
    End Function

    Public Shared Function GetClassProperties(objClassWrap As ObjectClassWrapper) As List(Of PropertyWrapper)
        Dim objClass = objClassWrap.ObjectClass
        ClassProperties = New List(Of PropertyWrapper)

        'owner object if applicable
        If objClassWrap.ObjecType.OwnerType <> 0 Then
            Dim ownerObjType = Vault.ObjectTypeOperations.GetObjectType(objClassWrap.ObjecType.OwnerType)
            Dim propID = ownerObjType.OwnerPropertyDef

            Dim mapping As New PropertyWrapper(propID, True)
            If Not ClassProperties.FindIndex(Function(p)
                                                 Return p.PropertyID = propID
                                             End Function) > -1 Then
                ClassProperties.Add(mapping)
            End If

        End If


        'class property definitions
        For Each associatedPropDef As AssociatedPropertyDef In objClass.AssociatedPropertyDefs
            Dim propID = associatedPropDef.PropertyDef
            Dim propertyDef = Vault.PropertyDefOperations.GetPropertyDef(propID)

            'ignore Predefined properties, unless Object name property
            If propertyDef.Predefined And objClass.NamePropertyDef <> propID Then Continue For

            'ignore Calculated properties, Simple or VBScript 
            If propertyDef.AutomaticValueType = MFAutomaticValueType.MFAutomaticValueTypeCalculatedWithPlaceholders Or
               propertyDef.AutomaticValueType = MFAutomaticValueType.MFAutomaticValueTypeCalculatedWithVBScript Then Continue For

            'ignore Automatic Values, if Automatic Values enabled
            If (propertyDef.AutomaticValueType = MFAutomaticValueType.MFAutomaticValueTypeAutoNumberSimple Or
                propertyDef.AutomaticValueType = MFAutomaticValueType.MFAutomaticValueTypeWithVBScript) Then Continue For

            'add property to listview
            Dim mapping = New PropertyWrapper(propID, associatedPropDef.Required)
            If propertyDef.ID = objClass.NamePropertyDef Then mapping.UseDocumentName = True

            ClassProperties.Add(mapping)
            'If m_importMappings.ContainsKey(propID) Then mapping = m_importMappings(propID).Clone
            'Dim lstItem = lvPropertyMappings.Items.Add(mapping.PropertyDescription(Vault))
            'refreshListItem(m_vault, mapping, lstItem)
        Next
        Return ClassProperties
    End Function

    Public Shared Function AddNewObject(classID As Integer, filename As List(Of String), SheetUniqueID As String, layout As ViewArea) As Boolean
        Dim objClass As ObjectClass = Vault.ClassOperations.GetObjectClass(classID)
        Dim properties As New PropertyValues

        'set Class
        Dim propertyValue As New PropertyValue
        propertyValue.PropertyDef = MFBuiltInPropertyDef.MFBuiltInPropertyDefClass
        propertyValue.Value.SetValue(MFDataType.MFDatatypeLookup, objClass.ID)
        properties.Add(-1, propertyValue)

        'add Name
        Dim namePropIndex As Integer = PluginSettings.DefaultClassProperties.FindIndex(Function(_p) _p.MFilesPropertyDef(Vault).ID = objClass.NamePropertyDef)

        If namePropIndex = -1 Then
            propertyValue = New PropertyValue
            propertyValue.PropertyDef = objClass.NamePropertyDef
            propertyValue.Value.SetValue(MFDataType.MFDatatypeText, "Temporary Filename")
            properties.Add(-1, propertyValue)
        Else
            'Dim p = PluginSettings.DefaultClassProperties(namePropIndex)

            'If p.UseDocumentName Then p.Value = layout.DocumentName

            'propertyValue = New PropertyValue
            'propertyValue.PropertyDef = objClass.NamePropertyDef
            'propertyValue.Value.SetValue(MFDataType.MFDatatypeText, p.Value)
            'properties.Add(-1, propertyValue)
        End If

        ''add the defaults to the collection of custom items unless the item is already in the list
        For Each propWrap In PluginSettings.DefaultClassProperties
            Dim pIndex As Integer = layout.CustomObjectProperties.FindIndex(Function(p) p.PropertyID = propWrap.PropertyID)

            If Not pIndex > -1 Then
                If Not propWrap.Value Is Nothing Then layout.CustomObjectProperties.Add(propWrap)
            End If
        Next

        For Each propWrap In layout.CustomObjectProperties
            If propWrap.Value Is DBNull.Value Then Continue For
            If propWrap.Value Is Nothing Then Continue For

            Dim dType = propWrap.MFilesPropertyDef(Vault).DataType
            Select Case dType
                Case MFDataType.MFDatatypeLookup, MFDataType.MFDatatypeMultiSelectLookup

                    ''resolve the 
                    resolveLookupValue(propWrap.MFilesPropertyDef(Vault), propWrap)
                    propertyValue = New PropertyValue
                    propertyValue.PropertyDef = propWrap.PropertyID
                    propertyValue.Value.SetValue(dType, propWrap.ValueID)
                    properties.Add(-1, propertyValue)
                Case Else

                    If propWrap.UseDocumentName Then
                        propWrap.Value = layout.DocumentName
                    End If
                    propertyValue = New PropertyValue
                    propertyValue.PropertyDef = propWrap.PropertyID
                    propertyValue.Value.SetValue(dType, propWrap.Value)
                    properties.Add(-1, propertyValue)

            End Select
        Next

        'For Each propWrap In PluginSettings.DefaultClassProperties
        '    Dim dType = propWrap.MFilesPropertyDef(Vault).DataType
        '    Select Case dType
        '        Case MFDataType.MFDatatypeLookup, MFDataType.MFDatatypeMultiSelectLookup
        '            If propWrap.Value Is DBNull.Value Then Continue For

        '            ''resolve the 
        '            resolveLookupValue(propWrap.MFilesPropertyDef(Vault), propWrap)
        '            propertyValue = New PropertyValue
        '            propertyValue.PropertyDef = propWrap.PropertyID
        '            propertyValue.Value.SetValue(dType, propWrap.ValueID)
        '            properties.Add(-1, propertyValue)
        '        Case Else

        '            If propWrap.UseDocumentName Then
        '                propWrap.Value = layout.DocumentName
        '            End If
        '            propertyValue = New PropertyValue
        '            propertyValue.PropertyDef = propWrap.PropertyID
        '            propertyValue.Value.SetValue(dType, propWrap.Value)
        '            properties.Add(-1, propertyValue)

        '    End Select
        'Next

        'ExDOCS.Property.RvtSheetID
        ''get property from alias
        Dim SheetIDpropID As Integer = Vault.PropertyDefOperations.GetPropertyDefIDByAlias("ExDOCS.Property.RvtSheetID")
        'Dim SheetIDpropDEF As PropertyDef = Vault.PropertyDefOperations.GetPropertyDef(SheetIDpropID)
        propertyValue = New PropertyValue
        propertyValue.PropertyDef = SheetIDpropID
        propertyValue.Value.SetValue(MFDataType.MFDatatypeText, SheetUniqueID)
        properties.Add(-1, propertyValue)

        'get Source Files
        Dim sourceFiles = getSourceObjectFiles(objClass, filename)

        'create object
        Dim objVerProps As ObjectVersionAndProperties
        If sourceFiles Is Nothing Then
            objVerProps = Vault.ObjectOperations.CreateNewObject(objClass.ObjectType, properties)
        Else
            objVerProps = Vault.ObjectOperations.CreateNewObject(objClass.ObjectType, properties, sourceFiles) ', True)
        End If

        ''if Automatic Values disabled then set Object Name and remove IsTemplate property
        'If m_configuration.DisableAutomaticValues Then
        '    'set Name
        '    If m_calculatedValues.ContainsKey(objClass.NamePropertyDef) Then
        '        propertyValue = objVerProps.Properties.SearchForProperty(objClass.NamePropertyDef)
        '        propertyValue.Value.SetValue(MFDataType.MFDatatypeText, m_calculatedValues(objClass.NamePropertyDef).Value)
        '    End If
        '    'remove IsTemplate property
        '    objVerProps.Properties.Remove(2)
        'End If

        'update all properties
        'Vault.ObjectPropertyOperations.SetAllProperties(objVerProps.ObjVer, True, objVerProps.Properties)

        'check in?
        Vault.ObjectOperations.CheckIn(objVerProps.ObjVer)

    End Function

    Public Shared Function AddNewObject(classID As Integer, filename As String, SheetUniqueID As String) As Boolean
        Dim objClass As ObjectClass = Vault.ClassOperations.GetObjectClass(classID)
        Dim properties As New PropertyValues

        'set Class
        Dim propertyValue As New PropertyValue
        propertyValue.PropertyDef = MFBuiltInPropertyDef.MFBuiltInPropertyDefClass
        propertyValue.Value.SetValue(MFDataType.MFDatatypeLookup, objClass.ID)
        properties.Add(-1, propertyValue)

        'add Name
        If PluginSettings.DefaultClassProperties.FindIndex(Function(_p) _p.MFilesPropertyDef(Vault).ID = objClass.NamePropertyDef) = -1 Then
            propertyValue = New PropertyValue
            propertyValue.PropertyDef = objClass.NamePropertyDef
            propertyValue.Value.SetValue(MFDataType.MFDatatypeText, "Temporary Filename")
            properties.Add(-1, propertyValue)
        End If
        'If Not m_calculatedValues.ContainsKey(objClass.NamePropertyDef) Then
        '    propertyValue = New PropertyValue
        '    propertyValue.PropertyDef = objClass.NamePropertyDef
        '    propertyValue.Value.SetValue(MFDataType.MFDatatypeText, "Temporary Filename")
        '    properties.Add(-1, propertyValue)
        'End If

        For Each propWrap In PluginSettings.DefaultClassProperties
            Dim dType = propWrap.MFilesPropertyDef(Vault).DataType
            Select Case dType
                Case MFDataType.MFDatatypeLookup, MFDataType.MFDatatypeMultiSelectLookup
                    If propWrap.Value Is DBNull.Value Then Continue For

                    ''resolve the 
                    resolveLookupValue(propWrap.MFilesPropertyDef(Vault), propWrap)
                    propertyValue = New PropertyValue
                    propertyValue.PropertyDef = propWrap.PropertyID
                    propertyValue.Value.SetValue(dType, propWrap.ValueID)
                    properties.Add(-1, propertyValue)
                Case Else
                    propertyValue = New PropertyValue
                    propertyValue.PropertyDef = propWrap.PropertyID
                    propertyValue.Value.SetValue(dType, propWrap.Value)
                    properties.Add(-1, propertyValue)
            End Select
        Next

        'ExDOCS.Property.RvtSheetID
        ''get property from alias
        Dim SheetIDpropID As Integer = Vault.PropertyDefOperations.GetPropertyDefIDByAlias("ExDOCS.Property.RvtSheetID")
        'Dim SheetIDpropDEF As PropertyDef = Vault.PropertyDefOperations.GetPropertyDef(SheetIDpropID)
        propertyValue = New PropertyValue
        propertyValue.PropertyDef = SheetIDpropID
        propertyValue.Value.SetValue(MFDataType.MFDatatypeText, SheetUniqueID)
        properties.Add(-1, propertyValue)

        'get Source Files
        Dim sourceFiles = getSourceObjectFiles(objClass, filename)

        'create object
        Dim objVerProps As ObjectVersionAndProperties
        If sourceFiles Is Nothing Then
            objVerProps = Vault.ObjectOperations.CreateNewObject(objClass.ObjectType, properties)
        Else
            objVerProps = Vault.ObjectOperations.CreateNewObject(objClass.ObjectType, properties, sourceFiles) ', True)
        End If

        ''if Automatic Values disabled then set Object Name and remove IsTemplate property
        'If m_configuration.DisableAutomaticValues Then
        '    'set Name
        '    If m_calculatedValues.ContainsKey(objClass.NamePropertyDef) Then
        '        propertyValue = objVerProps.Properties.SearchForProperty(objClass.NamePropertyDef)
        '        propertyValue.Value.SetValue(MFDataType.MFDatatypeText, m_calculatedValues(objClass.NamePropertyDef).Value)
        '    End If
        '    'remove IsTemplate property
        '    objVerProps.Properties.Remove(2)
        'End If

        'update all properties
        Vault.ObjectPropertyOperations.SetAllProperties(objVerProps.ObjVer, True, objVerProps.Properties)

        'check in?
        Vault.ObjectOperations.CheckIn(objVerProps.ObjVer)

    End Function

    Public Shared Function AddNewObjectVersion(filename As String, item As ObjectVersion)
        If Not item.ObjectCheckedOut Then
            Dim objClass = Vault.ClassOperations.GetObjectClass(item.Class)
            Dim destObjverVersion As ObjectVersion = Vault.ObjectOperations.CheckOut(item.ObjVer.ObjID)
            Dim destObjVer = destObjverVersion.ObjVer
            Dim properties As New PropertyValues
            Dim propertyValue As New PropertyValue

            For Each propWrap In PluginSettings.DefaultClassProperties
                Dim dType = propWrap.MFilesPropertyDef(Vault).DataType
                Select Case dType
                    Case MFDataType.MFDatatypeLookup, MFDataType.MFDatatypeMultiSelectLookup
                        If propWrap.Value Is DBNull.Value Then Continue For

                        ''resolve the 
                        resolveLookupValue(propWrap.MFilesPropertyDef(Vault), propWrap)
                        propertyValue = New PropertyValue
                        propertyValue.PropertyDef = propWrap.PropertyID
                        propertyValue.Value.SetValue(dType, propWrap.ValueID)
                        properties.Add(-1, propertyValue)
                    Case Else
                        propertyValue = New PropertyValue
                        propertyValue.PropertyDef = propWrap.PropertyID
                        propertyValue.Value.SetValue(dType, propWrap.Value)
                        properties.Add(-1, propertyValue)
                End Select
            Next

            'get new  files
            Dim sourceFiles = getSourceObjectFiles(objClass, filename)
            If Not sourceFiles Is Nothing Then
                ''remove this file from the object
                For Each _file As ObjectFile In destObjverVersion.Files
                    If _file.Title = sourceFiles(1).Title And _file.Extension = sourceFiles(1).Extension Then
                        Vault.ObjectFileOperations.RemoveFile(destObjVer, _file.FileVer)
                    End If
                Next

                ''add the new file to the objver
                Vault.ObjectFileOperations.AddFile(destObjVer, sourceFiles(1).Title, sourceFiles(1).Extension, sourceFiles(1).SourceFilePath)
            End If

            'update all properties
            Vault.ObjectPropertyOperations.SetProperties(destObjVer, properties)

            'check in?

            Vault.ObjectOperations.CheckIn(destObjVer)
        End If
    End Function

    Public Shared Function AddNewObjectVersion(filename As List(Of String), item As ObjectVersion, layout As ViewArea)
        If Not item.ObjectCheckedOut Then
            Dim objClass = Vault.ClassOperations.GetObjectClass(item.Class)
            Dim destObjverVersion As ObjectVersion = Vault.ObjectOperations.CheckOut(item.ObjVer.ObjID)
            Dim destObjVer = destObjverVersion.ObjVer
            Dim properties As New PropertyValues
            Dim propertyValue As New PropertyValue

            For Each propWrap In PluginSettings.DefaultClassProperties
                If propWrap.PropertyID = MFBuiltInPropertyDef.MFBuiltInPropertyDefNameOrTitle Then Continue For
                Dim dType = propWrap.MFilesPropertyDef(Vault).DataType
                Select Case dType
                    Case MFDataType.MFDatatypeLookup, MFDataType.MFDatatypeMultiSelectLookup
                        If propWrap.Value Is DBNull.Value Then Continue For

                        ''resolve the 
                        resolveLookupValue(propWrap.MFilesPropertyDef(Vault), propWrap)
                        propertyValue = New PropertyValue
                        propertyValue.PropertyDef = propWrap.PropertyID
                        propertyValue.Value.SetValue(dType, propWrap.ValueID)
                        properties.Add(-1, propertyValue)
                    Case Else
                        propertyValue = New PropertyValue
                        propertyValue.PropertyDef = propWrap.PropertyID
                        propertyValue.Value.SetValue(dType, propWrap.Value)
                        properties.Add(-1, propertyValue)
                End Select
            Next

            'get new  files
            Dim sourceFiles = getSourceObjectFiles(objClass, filename)
            If Not sourceFiles Is Nothing Then
                ''remove this file from the object
                For Each _file As ObjectFile In destObjverVersion.Files
                    If _file.Title = sourceFiles(1).Title And _file.Extension = sourceFiles(1).Extension Then
                        Vault.ObjectFileOperations.RemoveFile(destObjVer, _file.FileVer)
                    End If
                Next

                ''add the new file to the objver
                Vault.ObjectFileOperations.AddFile(destObjVer, sourceFiles(1).Title, sourceFiles(1).Extension, sourceFiles(1).SourceFilePath)
            End If

            'update all properties
            Vault.ObjectPropertyOperations.SetProperties(destObjVer, properties)

            'check in?
            Dim destObjectVersion = Vault.ObjectOperations.CheckIn(destObjVer)
            Return destObjectVersion
        End If
    End Function

    Private Shared Sub resolveLookupValue(mFilesPropertyDef As PropertyDef, Value As PropertyWrapper)

        Dim objType = Vault.ValueListOperations.GetValueList(mFilesPropertyDef.ValueList)
        If objType.RealObjectType Then
            resolveObjTypeValue(objType, Value)
        Else
            resolveValueListEntry(objType, Value)
        End If
    End Sub

    Private Shared Sub resolveObjTypeValue(ObjType As ObjType, calculatedValue As PropertyWrapper)
        Try
            Dim srchConditions As New SearchConditions
            Dim oScValueListItem As SearchCondition

            'If ObjType.HasOwnerType Then
            '    'Dim ownerPropertyID = OwnerPropertyMappings(ObjType.OwnerType)
            '    'Dim ownerCalculatedValue = m_calculatedValues(ownerPropertyID)

            '    'owner property
            '    oScValueListItem = New SearchCondition
            '    oScValueListItem.Expression.SetValueListItemExpression(MFValueListItemPropertyDef.MFValueListItemPropertyDefOwner, MFParentChildBehavior.MFParentChildBehaviorNone)
            '    oScValueListItem.ConditionType = MFConditionType.MFConditionTypeEqual
            '    oScValueListItem.TypedValue.SetValue(MFDataType.MFDatatypeLookup, ownerCalculatedValue.ValueID)
            '    srchConditions.Add(-1, oScValueListItem)
            'End If

            'not deleted
            oScValueListItem = New SearchCondition
            oScValueListItem = New SearchCondition
            oScValueListItem.Expression.SetValueListItemExpression(MFValueListItemPropertyDef.MFValueListItemPropertyDefDeleted, MFParentChildBehavior.MFParentChildBehaviorNone)
            oScValueListItem.ConditionType = MFConditionType.MFConditionTypeEqual
            oScValueListItem.TypedValue.SetValue(MFDataType.MFDatatypeBoolean, False)
            srchConditions.Add(-1, oScValueListItem)

            ' Search for the value list item.
            Dim valueListItems = Vault.ValueListItemOperations.SearchForValueListItemsEx(ObjType.ID, srchConditions)

            For Each valueListItem As ValueListItem In valueListItems
                Dim objId As New ObjID
                objId.SetIDs(ObjType.ID, valueListItem.ID)

                'Debug.Print(valueListItem.ID.ToString + " - " + valueListItem.Name)

                'load properties
                If calculatedValue.ValueID > -1 Then
                    If valueListItem.ID = calculatedValue.ValueID Then calculatedValue.Value = valueListItem.Name : Return
                ElseIf TypeOf calculatedValue.Value Is String Then
                    Dim objVerProps = Vault.ObjectOperations.GetLatestObjectVersionAndProperties(objId, False)
                    For Each prop As PropertyValue In objVerProps.Properties
                        If String.Equals(prop.Value.DisplayValue, calculatedValue.Value, StringComparison.InvariantCultureIgnoreCase) Then
                            calculatedValue.ValueID = valueListItem.ID
                            calculatedValue.Value = valueListItem.Name
                            Return
                        End If
                    Next
                End If
            Next
        Catch ex As Exception
        End Try

    End Sub

    Private Shared Sub resolveValueListEntry(ObjType As ObjType, calculatedValue As PropertyWrapper)
        Try
            Dim valListItems = Vault.ValueListItemOperations.GetValueListItems(ObjType.ID)

            For Each valueListItem As ValueListItem In valListItems
                If calculatedValue.ValueID > -1 Then
                    If valueListItem.ID = calculatedValue.ValueID Then calculatedValue.Value = valueListItem.Name : Return
                ElseIf TypeOf calculatedValue.Value Is String Then
                    If String.Equals(valueListItem.Name, calculatedValue.Value, StringComparison.InvariantCultureIgnoreCase) Then
                        calculatedValue.ValueID = valueListItem.ID
                        calculatedValue.Value = valueListItem.Name
                        Return
                    End If
                End If
            Next
        Catch ex As Exception
        End Try

    End Sub

    Private Shared Function getSourceObjectFiles(objClass As ObjectClass, filename As String) As SourceObjectFiles

        'get Object Type from Class
        Dim objType = Vault.ObjectTypeOperations.GetObjectType(objClass.ObjectType)

        'If cannot have files then return nothing
        If Not objType.CanHaveFiles Then Return Nothing

        Dim sourceFiles As New SourceObjectFiles
        Dim sourceFile As New SourceObjectFile()

        Dim extension = IO.Path.GetExtension(filename).ToLower.Replace(".", "")
        sourceFile.SourceFilePath = filename
        'If m_calculatedValues.ContainsKey(objClass.NamePropertyDef) Then
        '    sourceFile.Title = m_calculatedValues(objClass.NamePropertyDef).Value
        'Else
        sourceFile.Title = IO.Path.GetFileNameWithoutExtension(filename)
        'End If
        sourceFile.Extension = extension
        sourceFiles.Add(-1, sourceFile)

        Return sourceFiles

    End Function

    Private Shared Function getSourceObjectFiles(objClass As ObjectClass, filename As List(Of String)) As SourceObjectFiles

        'get Object Type from Class
        Dim objType = Vault.ObjectTypeOperations.GetObjectType(objClass.ObjectType)

        'If cannot have files then return nothing
        If Not objType.CanHaveFiles Then Return Nothing

        Dim sourceFiles As New SourceObjectFiles
        Dim sourceFile As New SourceObjectFile()

        For Each file As String In filename
            Dim extension = IO.Path.GetExtension(file).ToLower.Replace(".", "")
            sourceFile.SourceFilePath = file
            'If m_calculatedValues.ContainsKey(objClass.NamePropertyDef) Then
            '    sourceFile.Title = m_calculatedValues(objClass.NamePropertyDef).Value
            'Else
            sourceFile.Title = IO.Path.GetFileNameWithoutExtension(file)
            'End If
            sourceFile.Extension = extension
            sourceFiles.Add(-1, sourceFile)
        Next



        Return sourceFiles

    End Function

    Public Shared Function FindExistingObject(viewArea As ViewArea) As ViewArea
        Try
            Dim thisCN As Vault = VaultStatus.CreateVaultObject

            Dim searchConditions As New SearchConditions
            Dim SheetIDpropID As Integer = thisCN.PropertyDefOperations.GetPropertyDefIDByAlias("ExDOCS.Property.RvtSheetID")

            'Not Deleted
            Dim srchCondition As New SearchCondition
            Dim expression As New Expression
            Dim typedValue As New TypedValue
            expression.SetStatusValueExpression(MFStatusType.MFStatusTypeDeleted)
            typedValue.SetValue(MFDataType.MFDatatypeBoolean, False)
            srchCondition.Set(expression, MFConditionType.MFConditionTypeEqual, typedValue)
            searchConditions.Add(-1, srchCondition)

            'Revit Sheet GUID
            srchCondition = New SearchCondition
            expression = New Expression
            typedValue = New TypedValue
            expression.SetPropertyValueExpression(SheetIDpropID, MFParentChildBehavior.MFParentChildBehaviorNone)
            typedValue.SetValue(MFDataType.MFDatatypeText, viewArea.LayoutUniqueID)
            srchCondition.Set(expression, MFConditionType.MFConditionTypeEqual, typedValue)
            searchConditions.Add(-1, srchCondition)

            'Is Document Object Type
            srchCondition = New SearchCondition
            expression = New Expression
            typedValue = New TypedValue
            expression.SetStatusValueExpression(MFStatusType.MFStatusTypeObjectTypeID)
            typedValue.SetValue(MFDataType.MFDatatypeLookup, 0)
            srchCondition.Set(expression, MFConditionType.MFConditionTypeEqual, typedValue)
            searchConditions.Add(-1, srchCondition)

            'Perform Search
            Dim searchResults = thisCN.ObjectSearchOperations.SearchForObjectsByConditions(searchConditions, MFSearchFlags.MFSearchFlagNone, False)

            'check if any object found with matching Sheet GUID
            If searchResults.Count = 0 Then Return Nothing

            'sort search results by object ID, then return the ObjVer with the lowest ID
            Dim results = From objVersion As ObjectVersion In searchResults Order By objVersion.ObjVer.ID Select objVersion
            viewArea.ObjectVersion = results.First
            viewArea.IsInMFiles = True

            Return viewArea

        Catch ex As Exception
            LoggingHelper.WriteToLog(ex.ToString)
            Throw
            Return Nothing
        End Try


    End Function

    Public Shared Function GetDefaultViewPath(template As DrawingTemplate) As String
        Try
            Dim objVer As New ObjVer
            'objVer = template.ObjectVersion.ObjVer
            objVer = template.objVer

            Dim objVerProp = Vault.ObjectOperations.GetObjectVersionAndProperties(objVer)
            Dim objFile = objVerProp.VersionData.Files(1)

            Dim dfPath = Vault.ObjectFileOperations.GetPathInDefaultView(objVer.ObjID, -1, objFile.ID, -1)

            Return dfPath
        Catch ex As Exception
            LoggingHelper.WriteToLog("Error getting default file path fo template from MFiles: " + template.TemplateName)
            LoggingHelper.WriteToLog(ex.ToString)

            Throw
        End Try
    End Function

    Public Shared Function DownloadTemplateFile(template As DrawingTemplate, filename As String) As String
        Try
            Dim objVer As New ObjVer
            'objVer = template.ObjectVersion.ObjVer
            'objVer = template.objVer
            objVer.SetIDs(template.ObjVer_Type, template.ObjVer_ID, template.ObjVer_Version)

            Dim objVerProp = Vault.ObjectOperations.GetObjectVersionAndProperties(objVer)
            Dim objFile = objVerProp.VersionData.Files(1)

            Dim dfPath = Vault.ObjectFileOperations.GetPathInDefaultView(objVer.ObjID, -1, objFile.ID, -1)

            IO.File.Copy(dfPath, filename)
            Dim finfo As New IO.FileInfo(filename)
            finfo.IsReadOnly = False

            Return filename
        Catch ex As Exception
            LoggingHelper.WriteToLog("Error downloading template from MFiles: " + template.TemplateName)
            LoggingHelper.WriteToLog(ex.ToString)

            Throw
        End Try


    End Function

End Class
