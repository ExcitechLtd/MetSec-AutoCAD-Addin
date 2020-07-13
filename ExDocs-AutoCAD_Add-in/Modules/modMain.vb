Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Windows.Media.Imaging
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Windows


Module modMain

    Public g_clientApplication As New MFilesClientApplication
    Public g_hasLicense As Boolean
    Public g_excitechRibbonTab As New RibbonTab
    Public g_RibbonCommandHandler As New RibbonCommandHandler


    Public Function isMFilesDocument() As Boolean

        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim dwgFilename As String = acDoc.Database.OriginalFileName
        Dim pathInMFiles As Boolean = g_clientApplication.IsObjectPathInMFiles(dwgFilename)
        If Not pathInMFiles Then acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : Drawing not in " + My.Resources.EDM_Name + vbCrLf) : Return False
        Return True
    End Function

    Public Function autoCADWindowHandle() As Windows.Forms.IWin32Window
        Dim procAutoCAD = System.Diagnostics.Process.GetCurrentProcess
        Return New WindowWrapper(procAutoCAD.MainWindowHandle)
    End Function

    Public Sub acWriteMessage(acDoc As Document, Message As String)
        Try
            acDoc.Editor.WriteMessage(vbCrLf + Message + vbCrLf)
        Catch ex As Exception
        End Try
    End Sub

    Public Function RtnStrNotNull(varValue As Object) As String

        If IsDBNull(varValue) Then
            RtnStrNotNull = ""
        Else
            RtnStrNotNull = varValue
        End If

    End Function

    Public Function GetAllSheetBlockIds(vault As Vault) As List(Of ObjectId)

        Dim sheetBlockIds As New List(Of ObjectId)
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acDbase As Database = acDoc.Database

        Dim layoutMgr As LayoutManager = LayoutManager.Current

        Using acTrans As Transaction = acDbase.TransactionManager.StartTransaction()
            Dim layoutDic As DBDictionary = TryCast(acTrans.GetObject(acDbase.LayoutDictionaryId, OpenMode.ForRead, False), DBDictionary)
            For Each acDicEntry As DBDictionaryEntry In layoutDic
                Dim acLayoutId As ObjectId = acDicEntry.Value
                Dim shtBlockIds As List(Of ObjectId) = GetSheetBlockId(vault, acLayoutId)
                sheetBlockIds.AddRange(shtBlockIds)
            Next
        End Using

        Return sheetBlockIds

    End Function

    Public Function GetSheetBlockId(vault As Vault, LayoutId As ObjectId) As List(Of ObjectId)

        'get AutoCAD settings
        Dim settings = AutoCADVaultSettings.ReadSettings(vault, False)
        'Supported Titleblocks
        Dim titleBlockNames = settings.TitleBlockList

        Dim sheetBlockIds As New List(Of ObjectId)
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acDbase As Database = acDoc.Database

        Using doclock As DocumentLock = acDoc.LockDocument
            Using acTrans As Transaction = acDbase.TransactionManager.StartTransaction()

                ' Get the current layout id and layout name
                Dim curLayout As Layout = CType(LayoutId.GetObject(OpenMode.ForRead), Layout)
                Dim oBtr As BlockTableRecord = CType(acTrans.GetObject(curLayout.BlockTableRecordId, OpenMode.ForRead), BlockTableRecord)

                ' Get the enumerator
                Dim oBtre As BlockTableRecordEnumerator = oBtr.GetEnumerator
                ' Loop through all blocks in the block table
                While oBtre.MoveNext
                    Dim oEnt As Entity = CType(acTrans.GetObject(oBtre.Current, OpenMode.ForRead), Entity)

                    If TypeOf oEnt Is BlockReference Then
                        ' Change the entity to a block reference
                        Dim oBr As BlockReference = oEnt 'CType(oEnt, BlockReference)
                        ' Return the block reference id if the name matches
                        If titleBlockNames.Contains(oBr.Name.ToUpper) Then sheetBlockIds.Add(oBr.Id)
                    End If
                End While
            End Using
        End Using

        Return sheetBlockIds

    End Function


    Public Function GetAttributeValue(objIDTitleblock As ObjectId, AttributeName As String) As String

        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acDbase As Database = acDoc.Database

        Using doclock As DocumentLock = acDoc.LockDocument
            Using acTrans As Transaction = acDbase.TransactionManager.StartTransaction()
                Dim oBr As BlockReference = acTrans.GetObject(objIDTitleblock, OpenMode.ForRead)

                For Each objIDAtt As ObjectId In oBr.AttributeCollection
                    Dim attRef As AttributeReference = acTrans.GetObject(objIDAtt, OpenMode.ForRead)

                    If attRef.Tag.ToUpper = AttributeName.ToUpper Then Return attRef.TextString
                Next
            End Using
        End Using

        Return Nothing

    End Function

    Public Sub SetAttributeValue(objIDTitleblock As ObjectId, AttributeName As String, Value As String)

        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acDbase As Database = acDoc.Database


        Using doclock As DocumentLock = acDoc.LockDocument
            Using acTrans As Transaction = acDbase.TransactionManager.StartTransaction()
                Dim oBr As BlockReference = acTrans.GetObject(objIDTitleblock, OpenMode.ForWrite)

                For Each objIDAtt As ObjectId In oBr.AttributeCollection
                    Dim attRef As AttributeReference = acTrans.GetObject(objIDAtt, OpenMode.ForWrite)

                    If attRef.Tag.ToUpper = AttributeName.ToUpper Then
                        attRef.TextString = Value
                    End If
                Next
                acTrans.Commit()
            End Using
        End Using

    End Sub

    Public Sub updateMFilesFromAttributes(vault As Vault, objVersion As ObjectVersion, acObjectId As ObjectId)
        'get active document
        Dim acDoc = Application.DocumentManager.MdiActiveDocument

        Try
            'string to hold Sync Issues
            Dim syncIssues As String = ""

            'read Vault Settings
            Dim vaultSettings = AutoCADVaultSettings.ReadSettings(vault)

            'initialise Property Value collection
            Dim oClass As ObjectClass = vault.ClassOperations.GetObjectClass(objVersion.Class)
            Dim oPropertyValues As MFilesAPI.PropertyValues = New MFilesAPI.PropertyValues
            Dim oPropertyValue As MFilesAPI.PropertyValue
            Dim propertyMappings As Dictionary(Of String, docsPropertyMapping) = vaultSettings.propertyMappings

            'query for SyncFromFileOnly and BiDirectional Property Mappings only
            Dim filteredPropertyMappings = From propMapping As docsPropertyMapping In propertyMappings.Values Where propMapping.SyncDirection <> docsPropertyMapping.docsSyncDirections.SyncToFileOnly Select propMapping

            'iterate filtered property mappings and update M-Files
            For Each mapping In filteredPropertyMappings
                'get Attribute Value
                Dim attrValue = GetAttributeValue(acObjectId, mapping.MappingName)

                If attrValue IsNot Nothing Then
                    Dim oPropDef As PropertyDef = vault.PropertyDefOperations.GetPropertyDef(mapping.MFilesRootPropertyID)
                    Dim requiredProperty As Boolean = False
                    Dim assocPropDefResults = From associatedProp As AssociatedPropertyDef In oClass.AssociatedPropertyDefs Where associatedProp.PropertyDef = oPropDef.ID Select associatedProp
                    If assocPropDefResults.Count > 0 Then requiredProperty = assocPropDefResults.First.Required

                    'initialise Property Value
                    oPropertyValue = New MFilesAPI.PropertyValue
                    oPropertyValue.PropertyDef = mapping.MFilesRootPropertyID

                    'Validate required property status
                    If requiredProperty And attrValue.Trim = vbNullString Then syncIssues += "Unable to synchronise attribute '" + mapping.MappingName + "' to property '" + oPropDef.Name + "', no value present for a Required property." + vbCrLf : Continue For

                    Select Case oPropDef.DataType
                        Case MFDataType.MFDatatypeBoolean
                            'validate Boolean value
                            Dim boolValue As Nullable(Of Boolean) = Nothing
                            If attrValue.Trim.ToLower = "yes" Then boolValue = True
                            If attrValue.Trim.ToLower = "no" Then boolValue = False

                            If boolValue IsNot Nothing Then
                                oPropertyValue.TypedValue.SetValue(MFilesAPI.MFDataType.MFDatatypeBoolean, boolValue)
                                oPropertyValues.Add(0, oPropertyValue)
                            Else
                                syncIssues += "Unable to synchronise attribute '" + mapping.MappingName + "' to property '" + oPropDef.Name + "', value not convertable to Boolean." + vbCrLf
                            End If
                        Case MFDataType.MFDatatypeInteger
                            'Validate Integer value
                            Dim integerValue As Integer
                            If Integer.TryParse(attrValue, integerValue) Then
                                oPropertyValue.TypedValue.SetValue(MFilesAPI.MFDataType.MFDatatypeInteger, integerValue)
                                oPropertyValues.Add(0, oPropertyValue)
                            Else
                                syncIssues += "Unable to synchronise attribute '" + mapping.MappingName + "' to property '" + oPropDef.Name + "', value not convertable to Integer." + vbCrLf
                            End If
                        Case MFDataType.MFDatatypeFloating
                            'Validate Floating value
                            Dim doubleValue As Double
                            If Double.TryParse(attrValue, doubleValue) Then
                                oPropertyValue.TypedValue.SetValue(MFilesAPI.MFDataType.MFDatatypeFloating, doubleValue)
                                oPropertyValues.Add(0, oPropertyValue)
                            Else
                                syncIssues += "Unable to synchronise attribute '" + mapping.MappingName + "' to property '" + oPropDef.Name + "', value not convertable to Floating Point." + vbCrLf
                            End If
                        Case MFDataType.MFDatatypeDate
                            'Validate Date value
                            Dim dateValue As Date
                            If Date.TryParse(attrValue, dateValue) Then
                                oPropertyValue.TypedValue.SetValue(MFilesAPI.MFDataType.MFDatatypeDate, dateValue)
                                oPropertyValues.Add(0, oPropertyValue)
                            Else
                                syncIssues += "Unable to synchronise attribute '" + mapping.MappingName + "' to property '" + oPropDef.Name + "', value not convertable to Date." + vbCrLf
                            End If
                        Case MFDataType.MFDatatypeTime
                            Dim timeValue As Date
                            If Date.TryParse(attrValue, timeValue) Then
                                oPropertyValue.TypedValue.SetValue(MFilesAPI.MFDataType.MFDatatypeTime, timeValue)
                                oPropertyValues.Add(0, oPropertyValue)
                            Else
                                syncIssues += "Unable to synchronise attribute '" + mapping.MappingName + "' to property '" + oPropDef.Name + "', value not convertable to Time." + vbCrLf
                            End If
                        Case MFDataType.MFDatatypeText
                            oPropertyValue.TypedValue.SetValue(MFilesAPI.MFDataType.MFDatatypeText, attrValue)
                            oPropertyValues.Add(0, oPropertyValue)
                        Case MFDataType.MFDatatypeMultiLineText
                            oPropertyValue.PropertyDef = mapping.MFilesRootPropertyID
                            oPropertyValue.TypedValue.SetValue(MFilesAPI.MFDataType.MFDatatypeMultiLineText, attrValue)
                            oPropertyValues.Add(0, oPropertyValue)
                    End Select
                End If
            Next
            vault.ObjectPropertyOperations.SetProperties(objVersion.ObjVer, oPropertyValues)

            If syncIssues = "" Then
                acWriteMessage(acDoc, vbCrLf + "Synchronise to M-Files Complete" + vbCrLf)
            Else
                acWriteMessage(acDoc, vbCrLf + "Synchronise to M-Files Completed with Errors:" + vbCrLf)
                acWriteMessage(acDoc, syncIssues)
            End If

        Catch ex As Exception
            Dim errorArray As String() = Split(ex.Message, vbCrLf)
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + errorArray.First)
            WriteLogFile("updateMFilesFromAttributes ERROR: " + ex.Message)
        End Try

    End Sub

    Public Sub updateAttributesFromMFiles(vault As Vault, ObjVer As ObjVer, acObjectId As ObjectId)

        Try
            Dim vaultSettings = AutoCADVaultSettings.ReadSettings(vault)
            Dim propVals As MFilesAPI.PropertyValues = vault.ObjectPropertyOperations.GetProperties(ObjVer, False)
            Dim propertyMappings As Dictionary(Of String, docsPropertyMapping) = vaultSettings.propertyMappings

            'query for SyncToFileOnly and BiDirectional Property Mappings only
            Dim filteredPropertyMappings = From propMapping As docsPropertyMapping In propertyMappings.Values Where propMapping.SyncDirection <> docsPropertyMapping.docsSyncDirections.SyncFromFileOnly Select propMapping

            'Get Properties for Metadata sync
            Dim namedValues = vault.ObjectPropertyOperations.GetPropertiesForMetadataSync(ObjVer, MFMetadataSyncFormat.MFMetadataSyncFormatPowerPoint)

            'iterate filtered property mappings and update attributes
            For Each mapping In filteredPropertyMappings
                Dim propertyValue As String = ""
                If Not IsDBNull(namedValues(mapping.MFilesIdentifier)) Then propertyValue = namedValues(mapping.MFilesIdentifier)
                SetAttributeValue(acObjectId, mapping.MappingName, propertyValue)
            Next
        Catch ex As Exception
            WriteLogFile("updateAttributesFromMFiles ERROR: " + ex.Message)
        End Try

    End Sub

    Public Function GetBitmapImage(SourceImage As Bitmap) As BitmapImage

        Dim stream As MemoryStream = New MemoryStream()
        SourceImage.Save(stream, ImageFormat.Png)
        Dim bmp As BitmapImage = New BitmapImage()

        bmp.BeginInit()
        bmp.StreamSource = stream
        bmp.EndInit()

        Return bmp

    End Function

    Public Function MakeRelativePath(fromPath As [String], toPath As [String]) As [String]
        If [String].IsNullOrEmpty(fromPath) Then
            Throw New ArgumentNullException("fromPath")
        End If
        If [String].IsNullOrEmpty(toPath) Then
            Throw New ArgumentNullException("toPath")
        End If

        Dim fromUri As New Uri(fromPath)
        Dim toUri As New Uri(toPath)

        If fromUri.Scheme <> toUri.Scheme Then
            Return toPath
        End If
        ' path can't be made relative.
        Dim relativeUri As Uri = fromUri.MakeRelativeUri(toUri)
        Dim relativePath As [String] = Uri.UnescapeDataString(relativeUri.ToString())

        If toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase) Then
            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
        End If

        Return relativePath
    End Function

    Public Sub WriteLogFile(Message As String)

        'Create Log Path in ProgramData
        Dim exDocsProgramData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Excitech Docs")
        Dim exDocLogs = Path.Combine(exDocsProgramData, "Logs")
        Dim logFilename = Path.Combine(exDocLogs, "AutoCAD.log")

        'create Directory 
        Directory.CreateDirectory(exDocLogs)

        'Write Log Entry
        Dim objWriter As New System.IO.StreamWriter(logFilename, True)
        objWriter.WriteLine(Format(Now, "dd/MM/yyyy HH:mm:ss:ffff") + " - " + Message)
        objWriter.Close()
    End Sub

End Module
