Imports System.Runtime.InteropServices
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Windows
Imports ExcitechDOCS.AutoCAD

Public Class Ribbon
    Implements IExtensionApplication

    Private IdleEvent As New System.EventHandler(AddressOf AutoCADIdle)
    Private m_modifiedReferenceIds As New List(Of ObjectId)

    <Security.SuppressUnmanagedCodeSecurity()>
    <DllImport("accore.dll", EntryPoint:="acedCmd", CharSet:=CharSet.Unicode, CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function AcedCmd(ByVal resbuf As IntPtr) As Integer
    End Function

    Private Declare Auto Function ads_queueexpr Lib "accore.dll" (ByVal strExpr As String) As Integer

    <System.Security.SuppressUnmanagedCodeSecurity()>
    Public Sub Initialize() Implements IExtensionApplication.Initialize

        AddHandler Core.Application.Idle, IdleEvent
        AddHandler Core.Application.DocumentManager.DocumentBecameCurrent, AddressOf documentBecameCurrentEvent
        AddHandler Core.Application.DocumentManager.DocumentCreated, AddressOf documentCreatedEvent
        AddHandler Core.Application.DocumentManager.DocumentToBeDestroyed, AddressOf documentToBeDestroyedEvent
    End Sub

    Private Sub documentCreatedEvent(ByVal senderObj As Object, ByVal docColDocActEvtArgs As DocumentCollectionEventArgs)

        Dim currentDoc As Document = docColDocActEvtArgs.Document
        AddHandler currentDoc.Database.BeginSave, AddressOf beginSaveEvent
        AddHandler currentDoc.Database.SaveComplete, AddressOf saveCompleteEvent

        AddHandler currentDoc.Database.ObjectAppended, AddressOf objectAppendedEvent
        'AddHandler currentDoc.Database.ObjectModified, AddressOf objectModifiedEvent
        AddHandler currentDoc.CommandEnded, AddressOf commandEndedEvent
        AddHandler currentDoc.CommandCancelled, AddressOf commandCancelledEvent

        'fix any broken  M-Files reference links
        Dim SFDMFDHelper As New SFDMFDReferenceHelper(currentDoc)
        SFDMFDHelper.Repath()

        'Show updated references?
        If shouldShowUpdateReferenceDialog() Then g_RibbonCommandHandler.updateReferences() 'currentDoc.SendStringToExecute(ChrW(3) + "DOCSUPDATEREFS" + vbCr, False, False, False)
        If drawingInMFilesAndCheckedOut() Then g_RibbonCommandHandler.syncFromMFiles()

        Core.Application.DocumentManager.MdiActiveDocument = currentDoc
    End Sub

    Private Sub commandCancelledEvent(sender As Object, e As CommandEventArgs)
        commandEndedEvent(sender, e)
    End Sub

    Private Sub commandEndedEvent(sender As Object, e As CommandEventArgs)

        Debug.Print("commandEndedEvent: " + e.GlobalCommandName)
        If Not e.GlobalCommandName.Contains("ATTACH") Then Return
        If m_modifiedReferenceIds.Count = 0 Then Return

        'get active document
        Dim acDoc = Core.Application.DocumentManager.MdiActiveDocument
        'initialise M-Files
        Dim vault = VaultStatus.initialiseMFiles(True)
        Dim status = VaultStatus.Status()

        'Update Ribbon State
        updateRibbonState(acDoc)

        'check connection status
        If Not (status = VaultStatus.VaultStatuses.connected Or status = VaultStatus.VaultStatuses.offline) Then
            VaultStatus.EchoStatus(acDoc)
            Return
        End If
        'check Drawing is in M-Files
        If Not isMFilesDocument() Then Return

        'process all entities in the added list
        While m_modifiedReferenceIds.Count > 0
            Dim refObjectId As ObjectId
            Try
                refObjectId = m_modifiedReferenceIds.First
                Dim mFilesRef As New MFilesReferenceHelper(refObjectId)
                mFilesRef.Repath()
            Catch ex As Exception
            Finally
                m_modifiedReferenceIds.Remove(refObjectId)
            End Try
        End While

    End Sub

    Private Sub objectAppendedEvent(sender As Object, e As ObjectEventArgs)

        Select Case e.DBObject.GetType()
            Case GetType(RasterImageDef)
                m_modifiedReferenceIds.Add(e.DBObject.Id)
            Case GetType(PdfDefinition)
                Dim pdfDef As PdfDefinition = e.DBObject
                If pdfDef.Loaded Then m_modifiedReferenceIds.Add(e.DBObject.Id)
            Case GetType(BlockTableRecord)
                Dim blkTableRec = TryCast(e.DBObject, BlockTableRecord)
                If blkTableRec.XrefStatus <> XrefStatus.NotAnXref Then m_modifiedReferenceIds.Add(e.DBObject.Id)
        End Select

    End Sub

    'Private Sub objectModifiedEvent(sender As Object, e As ObjectEventArgs)

    '    Select Case e.DBObject.GetType()
    '        Case GetType(RasterImageDef)
    '            If Not m_modifiedReferenceIds.Contains(e.DBObject.Id) Then
    '                m_modifiedReferenceIds.Add(e.DBObject.Id)
    '                UpdateReferenceWithDelay(e.DBObject.Id, 5)
    '            End If
    '        Case GetType(PdfDefinition)
    '            Dim pdfDef As PdfDefinition = e.DBObject
    '            If Not pdfDef.Loaded Then Return
    '            If Not m_modifiedReferenceIds.Contains(e.DBObject.Id) Then
    '                m_modifiedReferenceIds.Add(e.DBObject.Id)
    '                UpdateReferenceWithDelay(e.DBObject.Id, 5)
    '            End If
    '        Case GetType(BlockTableRecord)
    '            Dim blkTableRec = TryCast(e.DBObject, BlockTableRecord)
    '            If blkTableRec.XrefStatus <> XrefStatus.NotAnXref Then
    '                If Not m_modifiedReferenceIds.Contains(e.DBObject.Id) Then
    '                    m_modifiedReferenceIds.Add(e.DBObject.Id)
    '                    UpdateReferenceWithDelay(e.DBObject.Id, 5)
    '                End If
    '            End If
    '    End Select

    'End Sub


    'Private Async Sub UpdateReferenceWithDelay(ObjectId As ObjectId, DelaySeconds As Integer)

    '    'delay to gain access to modified object
    '    Await Task.Delay(DelaySeconds * 1000)

    '    'attempt to repath reference
    '    Try
    '        Dim mFilesRef As New MFilesReferenceHelper(ObjectId)
    '        mFilesRef.Repath()
    '        m_modifiedReferenceIds.Remove(ObjectId)
    '    Catch ex As Exception
    '        'Retry on failure
    '        UpdateReferenceWithDelay(ObjectId, 10)
    '    End Try

    'End Sub


    Private Sub documentToBeDestroyedEvent(sender As Object, docColDocActEvtArgs As DocumentCollectionEventArgs)

        Dim currentDoc As Document = docColDocActEvtArgs.Document
        RemoveHandler currentDoc.Database.BeginSave, AddressOf beginSaveEvent
        RemoveHandler currentDoc.Database.SaveComplete, AddressOf saveCompleteEvent
        RemoveHandler currentDoc.Database.ObjectAppended, AddressOf objectAppendedEvent
        RemoveHandler currentDoc.CommandEnded, AddressOf commandEndedEvent
        RemoveHandler currentDoc.CommandCancelled, AddressOf commandCancelledEvent
    End Sub

    Private Sub beginSaveEvent(sender As Object, e As DatabaseIOEventArgs)

        'check not AutoSave file
        If IO.Path.GetExtension(e.FileName).ToUpper = ".SV$" Then Return

        If drawingInMFilesAndCheckedOut() Then
            Try
                g_RibbonCommandHandler.syncFromMFiles()
                updateReferenceRelationships(e.FileName)
            Catch ex As System.Exception
                WriteLogFile("beginSaveEvent Exception: " + ex.Message)
            End Try
        End If

    End Sub


    Private Sub saveCompleteEvent(sender As Object, e As DatabaseIOEventArgs)
        'update Ribbon State after save
        'check not AutoSave file
        If IO.Path.GetExtension(e.FileName).ToUpper <> ".SV$" Then
            updateRibbonState(Core.Application.DocumentManager.MdiActiveDocument)
        End If
    End Sub

    Private Function drawingInMFilesAndCheckedOut() As Boolean
        Try
            Dim acDoc = Core.Application.DocumentManager.MdiActiveDocument
            Dim dwgFilename As String = acDoc.Database.OriginalFileName
            'initialise M-Files
            'if in M-Files and Checked Out then return True
            If g_clientApplication.IsObjectPathInMFiles(dwgFilename) Then
                Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(dwgFilename)
                Dim vault = objVerProps.Vault
                Dim objVer As ObjVer = objVerProps.ObjVer

                'If Checked Out then synchronise properties
                If vault.ObjectOperations.IsObjectCheckedOutToThisUserOnThisComputer(objVer.ObjID) Then Return True
            End If
        Catch ex As Exception
        End Try
        Return False

    End Function

    Private Function shouldShowUpdateReferenceDialog() As Boolean

        Dim dwgDatabase = Core.Application.DocumentManager.MdiActiveDocument.Database
        Dim xrefGraph = dwgDatabase.GetHostDwgXrefGraph(False)
        If Not g_clientApplication.IsObjectPathInMFiles(dwgDatabase.OriginalFileName) Then Return False

        Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(dwgDatabase.OriginalFileName)
        Dim vault As Vault = objVerProps.Vault
        Dim objID As ObjID = objVerProps.ObjVer.ObjID
        If Not vault.ObjectOperations.IsObjectCheckedOutToThisUserOnThisComputer(objID) Then Return False

        Using trans As Transaction = dwgDatabase.TransactionManager.StartTransaction
            'check for out of date Xrefs
            Dim blkTable As BlockTable = trans.GetObject(dwgDatabase.BlockTableId, OpenMode.ForRead)
            For Each objectId As ObjectId In blkTable
                Dim blkTabRec As BlockTableRecord = trans.GetObject(objectId, OpenMode.ForRead)
                If blkTabRec.XrefStatus = XrefStatus.NotAnXref Then Continue For

                'initialise reference data
                Dim refdata As New UpdateReferenceItem(vault, objectId)
                If refdata.RequiresAttention Then Return True
            Next

            'check for out of date Images
            Dim imageDictID = RasterImageDef.GetImageDictionary(dwgDatabase)
            If Not imageDictID.IsNull Then
                Dim imageDefDict As DBDictionary = trans.GetObject(imageDictID, OpenMode.ForRead)
                For Each dictEntry As DBDictionaryEntry In imageDefDict
                    'initialise reference data
                    Dim refdata As New UpdateReferenceItem(vault, dictEntry.Value)
                    If refdata.RequiresAttention Then Return True
                Next
            End If

            'check for out of date PDFs
            Dim pdfDictKey As String = UnderlayDefinition.GetDictionaryKey(GetType(PdfDefinition))
            Dim namedObjectDict As DBDictionary = trans.GetObject(dwgDatabase.NamedObjectsDictionaryId, OpenMode.ForRead)
            If namedObjectDict.Contains(pdfDictKey) Then
                Dim pdfDefDict As DBDictionary = trans.GetObject(namedObjectDict.GetAt(pdfDictKey), OpenMode.ForRead)
                For Each dictEntry As DBDictionaryEntry In pdfDefDict
                    'initialise reference data
                    Dim refdata As New UpdateReferenceItem(vault, dictEntry.Value)
                    If refdata.RequiresAttention Then Return True
                Next
            End If
        End Using

        Return False
    End Function

    Private Sub updateReferenceRelationships(Filename As String)

        'get active document
        Dim acDoc = Core.Application.DocumentManager.MdiActiveDocument
        'initialise M-Files
        Dim vault = VaultStatus.initialiseMFiles()
        Dim status = VaultStatus.Status()

        'Update Ribbon State
        Ribbon.updateRibbonState(acDoc)

        'check connection status
        If Not (status = VaultStatus.VaultStatuses.connected Or status = VaultStatus.VaultStatuses.offline) Then
            VaultStatus.EchoStatus(acDoc)
            Return
        End If
        'check Drawing is in M-Files
        If Not isMFilesDocument() Then Return

        'get M-Files objects for current drawing
        Dim parentObjVerAndProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(Filename)
        Dim parentVault As Vault = parentObjVerAndProps.Vault
        Dim relatedObjVersionsList As New Dictionary(Of Integer, ObjectVersion)
        Dim dwgDatabase = Core.Application.DocumentManager.MdiActiveDocument.Database
        Dim xrefGraph = dwgDatabase.GetHostDwgXrefGraph(False)

        'Read Vault Settings
        Dim vaultSettings = AutoCADVaultSettings.ReadSettings(parentVault)

        Using trans As Transaction = dwgDatabase.TransactionManager.StartTransaction
            'Create Xrefs relationships
            Dim blkTable As BlockTable = trans.GetObject(dwgDatabase.BlockTableId, OpenMode.ForRead)
            For Each objID As ObjectId In blkTable
                Dim blkTabRec As BlockTableRecord = trans.GetObject(objID, OpenMode.ForRead)
                If blkTabRec.XrefStatus = XrefStatus.NotAnXref Then Continue For
                Dim xrefNode = xrefGraph.GetXrefNode(blkTabRec.Id)
                If xrefNode.IsNested Then Continue For

                'get Xref Database and Xref Full Path
                Dim xrefDatabase = blkTabRec.GetXrefDatabase(False)

                'valid xref Database
                If xrefDatabase Is Nothing Then Continue For

                Dim xrefFullPath As String = xrefDatabase.Filename
                'check M-Files with full path
                If Not g_clientApplication.IsObjectPathInMFiles(xrefFullPath) Then Continue For

                Dim childObjVerAndProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(xrefFullPath)
                If parentVault.GetGUID <> childObjVerAndProps.Vault.GetGUID Then Continue For
                'add relationship
                Dim childID = childObjVerAndProps.ObjVer.ID
                relatedObjVersionsList(childID) = childObjVerAndProps.VersionData
            Next

            'Create RasterImage relationships
            Dim imageDictID = RasterImageDef.GetImageDictionary(dwgDatabase)
            If Not imageDictID.IsNull Then
                Dim imageDefDict As DBDictionary = trans.GetObject(imageDictID, OpenMode.ForRead)
                For Each dictEntry As DBDictionaryEntry In imageDefDict
                    Dim imageDef As RasterImageDef = trans.GetObject(dictEntry.Value, OpenMode.ForRead)

                    If Not g_clientApplication.IsObjectPathInMFiles(imageDef.ActiveFileName) Then Continue For

                    Dim childObjVerAndProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(imageDef.ActiveFileName)
                    If parentVault.GetGUID <> childObjVerAndProps.Vault.GetGUID Then Continue For
                    'add relationship
                    Dim childID = childObjVerAndProps.ObjVer.ID
                    relatedObjVersionsList(childID) = childObjVerAndProps.VersionData
                Next
            End If

            'Create PDF relationships
            Dim pdfDictKey As String = UnderlayDefinition.GetDictionaryKey(GetType(PdfDefinition))
            Dim namedObjectDict As DBDictionary = trans.GetObject(dwgDatabase.NamedObjectsDictionaryId, OpenMode.ForRead)
            If namedObjectDict.Contains(pdfDictKey) Then
                Dim pdfDefDict As DBDictionary = trans.GetObject(namedObjectDict.GetAt(pdfDictKey), OpenMode.ForRead)
                For Each dictEntry As DBDictionaryEntry In pdfDefDict
                    Dim pdfDef As PdfDefinition = trans.GetObject(dictEntry.Value, OpenMode.ForRead)

                    If Not g_clientApplication.IsObjectPathInMFiles(pdfDef.ActiveFileName) Then Continue For

                    Dim childObjVerAndProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(pdfDef.ActiveFileName)
                    If parentVault.GetGUID <> childObjVerAndProps.Vault.GetGUID Then Continue For
                    'add relationship
                    Dim childID = childObjVerAndProps.ObjVer.ID
                    relatedObjVersionsList(childID) = childObjVerAndProps.VersionData
                Next
            End If
        End Using

        parentVault.ExcitechOperations.AddMultipleRelationshipsToSpecificVersion(parentObjVerAndProps.VersionData, relatedObjVersionsList.Values.ToArray, vaultSettings.xrefRelationshipPropertyID)
    End Sub

    Private Sub AutoCADIdle()

        RemoveHandler Core.Application.Idle, IdleEvent

        'get active document
        Dim acDoc As Document = Core.Application.DocumentManager.MdiActiveDocument

        'initialise M-Files
        VaultStatus.initialiseMFiles()
        VaultStatus.EchoStatus(acDoc)

        If g_excitechRibbonTab.Panels.Count = 0 Then
            AddExcitechRibbon()
            AddPlugins()
        End If
        updateRibbonState(acDoc)

    End Sub

    Public Sub documentBecameCurrentEvent(ByVal senderObj As DocumentCollection, ByVal docColDocActEvtArgs As DocumentCollectionEventArgs)

        Dim acadDoc As Document = docColDocActEvtArgs.Document
        updateRibbonState(acadDoc)
    End Sub

    Public Shared Sub updateRibbonState(acDoc As Document)

        'check ribbon populated
        If g_excitechRibbonTab.Panels.Count = 0 Then Return

        'Ensure a DWG Document is passed in
        If acDoc Is Nothing Then Return

        'initialise M-Files
        Dim vault As Vault = VaultStatus.initialiseMFiles()
        Dim status = VaultStatus.Status

        'If not connected or offline then all panel disabled
        For Each panel In g_excitechRibbonTab.Panels
            If panel.Source.Name <> "Settings" Then panel.IsEnabled = (status = VaultStatus.VaultStatuses.connected Or status = VaultStatus.VaultStatuses.offline)

            'set Settings Panel title to display configured Vault
            If panel.Source.Name = "Settings" Then
                panel.Source.Title = "Vault: " + IIf(My.Settings.VaultConnectionName = "", "Not Configured", My.Settings.VaultConnectionName)
            End If
        Next

        'enable reconnect button
        Dim btReconnect As RibbonButton = g_excitechRibbonTab.Panels.GetPanelByName("Settings").FindItem("reconnect")
        If btReconnect Is Nothing Then btReconnect = g_excitechRibbonTab.Panels.GetPanelByName("Settings").FindItem("goonline")
        Select Case status
            Case VaultStatus.VaultStatuses.connected
                btReconnect.Id = "reconnect"
                btReconnect.Text = "Reconnect"
                btReconnect.Description = "Tries reconnecting to the Vault"
                btReconnect.Image = GetBitmapImage(My.Resources.Disconnected_20)
                btReconnect.IsEnabled = False
            Case VaultStatus.VaultStatuses.disconnected, VaultStatus.VaultStatuses.loggedout
                btReconnect.Id = "reconnect"
                btReconnect.Text = "Reconnect"
                btReconnect.Description = "Tries reconnecting to the Vault"
                btReconnect.Image = GetBitmapImage(My.Resources.Disconnected_20)
                btReconnect.IsEnabled = True
                Return
            Case VaultStatus.VaultStatuses.offline
                btReconnect.Id = "goonline"
                btReconnect.Text = "Go Online"
                btReconnect.Description = "Change Vault to Online Mode"
                btReconnect.Image = GetBitmapImage(My.Resources.Offline_20)
                btReconnect.IsEnabled = True
            Case Else
                Return
        End Select

        Dim dwgFilename As String = acDoc.Database.OriginalFileName
        Dim isMFilesDocument As Boolean = g_clientApplication.IsObjectPathInMFiles(acDoc.Database.OriginalFileName)
        Dim canCheckOut As Boolean = False
        Dim canCheckIn As Boolean = False

        'reset Workflow options
        Dim btWorkflowOptions As RibbonSplitButton = g_excitechRibbonTab.Panels.GetPanelByName("Workflow").FindItem("workflowOption")
        btWorkflowOptions.Items.Clear()
        btWorkflowOptions.Text = "Not in Workflow"
        btWorkflowOptions.Current = btWorkflowOptions
        btWorkflowOptions.IsEnabled = False

        'Check Is M-Files Document
        If isMFilesDocument Then
            Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(dwgFilename)
            vault = objVerProps.Vault
            Dim objID As ObjID = objVerProps.ObjVer.ObjID
            Dim objVer As ObjVer = objVerProps.ObjVer

            If Not vault.ObjectOperations.IsObjectCheckedOut(objVer.ObjID) Then
                canCheckOut = g_clientApplication.isLatestVersion(dwgFilename)
                If canCheckOut Then updateWorkflowOptions(vault, objVer, status = VaultStatus.VaultStatuses.connected)
            ElseIf vault.ObjectOperations.IsObjectCheckedOutToThisUserOnThisComputer(objVer.ObjID) Then
                canCheckIn = g_clientApplication.isLatestVersion(dwgFilename) And Not acDoc.IsReadOnly
                updateWorkflowOptions(vault, objVer, False)
            Else
                btWorkflowOptions.Text = "Checked Out" + vbCrLf + "to Another User"
            End If
        End If

        'File Panel
        g_excitechRibbonTab.Panels.GetPanelByName("File").FindItem("newDWG").IsEnabled = (status = VaultStatus.VaultStatuses.connected)
        g_excitechRibbonTab.Panels.GetPanelByName("File").FindItem("openDWG").IsEnabled = True
        g_excitechRibbonTab.Panels.GetPanelByName("File").FindItem("saveDWG").IsEnabled = True

        'Vault Panel
        g_excitechRibbonTab.Panels.GetPanelByName("Vault").FindItem("viewInMFiles").IsEnabled = isMFilesDocument
        g_excitechRibbonTab.Panels.GetPanelByName("Vault").FindItem("browseMFiles").IsEnabled = True

        'Document Panel
        g_excitechRibbonTab.Panels.GetPanelByName("Document").FindItem("checkOut").IsEnabled = canCheckOut
        g_excitechRibbonTab.Panels.GetPanelByName("Document").FindItem("checkIn").IsEnabled = canCheckIn And status = VaultStatus.VaultStatuses.connected
        g_excitechRibbonTab.Panels.GetPanelByName("Document").FindItem("undoCheckOut").IsEnabled = canCheckIn And status = VaultStatus.VaultStatuses.connected

        'Properties Panel
        g_excitechRibbonTab.Panels.GetPanelByName("Properties").FindItem("syncToMFiles").IsEnabled = canCheckIn
        g_excitechRibbonTab.Panels.GetPanelByName("Properties").FindItem("syncFromMFiles").IsEnabled = canCheckIn
        g_excitechRibbonTab.Panels.GetPanelByName("Properties").FindItem("editProperties").IsEnabled = canCheckIn

        'Tools Panel
        g_excitechRibbonTab.Panels.GetPanelByName("Tools").FindItem("updateRefs").IsEnabled = canCheckIn And status = VaultStatus.VaultStatuses.connected
        'g_excitechRibbonTab.Panels(5).FindItem("publishPDF").IsEnabled = canCheckIn And status = VaultStatus.VaultStatuses.connected
        g_excitechRibbonTab.Panels.GetPanelByName("Tools").FindItem("showHistory").IsEnabled = isMFilesDocument And status = VaultStatus.VaultStatuses.connected

    End Sub

    Private Shared Sub updateWorkflowOptions(vault As Vault, objVer As ObjVer, enabled As Boolean)
        Dim btWorkflowOptions As RibbonSplitButton = g_excitechRibbonTab.Panels.GetPanelByName("Workflow").FindItem("workflowOption")
        btWorkflowOptions.Items.Clear()

        Try
            Dim workflowDef = vault.ObjectPropertyOperations.GetProperty(objVer, 38)
            Dim workflowState = vault.ObjectPropertyOperations.GetProperty(objVer, 39)

            Dim states As StateTransitionsForClient = vault.WorkflowOperations.GetWorkflowStateTransitionsEx(workflowDef.TypedValue.GetLookupID, workflowState.TypedValue, objVer)


            Dim btWFState As RibbonButton = New RibbonButton()
            btWFState.Text = workflowState.Value.DisplayValue + vbCrLf + "(Current State)"
            btWFState.Id = "workflow"
            btWFState.Description = "Change Workflow State"
            btWFState.ShowText = True
            btWFState.ShowImage = True
            btWFState.Image = GetBitmapImage(My.Resources.Workflow_20)
            btWFState.LargeImage = GetBitmapImage(My.Resources.Workflow_40)
            btWFState.Orientation = System.Windows.Controls.Orientation.Horizontal
            btWFState.Size = RibbonItemSize.Large
            btWFState.CommandHandler = g_RibbonCommandHandler

            btWorkflowOptions.Items.Add(btWFState)
            btWorkflowOptions.Current = btWFState

            For Each state As StateTransitionForClient In states
                btWFState = New RibbonButton()
                btWFState.Text = state.Name
                btWFState.Id = "workflow_" + state.ID.ToString
                btWFState.Tag = state.ID
                btWFState.ShowText = True
                btWFState.ShowImage = True
                btWFState.Image = GetBitmapImage(My.Resources.Workflow_20)
                btWFState.LargeImage = GetBitmapImage(My.Resources.Workflow_40)
                btWFState.Size = RibbonItemSize.Large
                btWFState.CommandHandler = New WorkflowCommandHandler()
                btWorkflowOptions.Items.Add(btWFState)
            Next

            'If current State is empty 
            If workflowState.Value.DisplayValue = "" And states.Count = 1 Then
                btWFState = btWorkflowOptions.Items(1)
                btWFState.Text += vbCrLf + "(Current State)"
                btWorkflowOptions.Current = btWFState
            End If

            'Set Enabled State
            btWorkflowOptions.IsEnabled = enabled
        Catch ex As COMException
        Catch ex As Exception
        End Try

    End Sub

    Public Sub AddExcitechRibbon()

        Dim ribbonControl As RibbonControl = ComponentManager.Ribbon
        g_excitechRibbonTab = New RibbonTab

        'Add Ribbon Tab
        g_excitechRibbonTab.Title = My.Resources.EDM_Name
        g_excitechRibbonTab.Id = "EXDOCS_RIBBON"
        ribbonControl.Tabs.Add(g_excitechRibbonTab)

        'Add File Ribbon Panel
        Dim panelSrcFiles As RibbonPanelSource = New RibbonPanelSource()
        panelSrcFiles.Title = "File"
        Dim panelFiles As RibbonPanel = New RibbonPanel()
        panelFiles.Source = panelSrcFiles
        g_excitechRibbonTab.Panels.Add(panelFiles)

        'Add File Ribbon Panel
        Dim panelSrcVault As RibbonPanelSource = New RibbonPanelSource()
        panelSrcVault.Title = "Vault"
        Dim panelVault As RibbonPanel = New RibbonPanel()
        panelVault.Source = panelSrcVault
        g_excitechRibbonTab.Panels.Add(panelVault)

        'Add Docuemnt Ribbon Panel
        Dim panelSrcDocument As RibbonPanelSource = New RibbonPanelSource()
        panelSrcDocument.Title = "Document"
        Dim panelDocument As RibbonPanel = New RibbonPanel()
        panelDocument.Source = panelSrcDocument
        g_excitechRibbonTab.Panels.Add(panelDocument)

        'Add Workflow Ribbon Panel
        Dim panelSrcWorkflow As RibbonPanelSource = New RibbonPanelSource()
        panelSrcWorkflow.Title = "Workflow"
        Dim panelWorkflow As RibbonPanel = New RibbonPanel()
        panelWorkflow.Source = panelSrcWorkflow
        g_excitechRibbonTab.Panels.Add(panelWorkflow)

        'Add Properties Ribbon Panel
        Dim panelSrcProperties As RibbonPanelSource = New RibbonPanelSource()
        panelSrcProperties.Title = "Properties"
        Dim panelProperties As RibbonPanel = New RibbonPanel()
        panelProperties.Source = panelSrcProperties
        g_excitechRibbonTab.Panels.Add(panelProperties)

        'Add Tools Ribbon Panel
        Dim panelSrcTools As RibbonPanelSource = New RibbonPanelSource()
        panelSrcTools.Title = "Tools"
        Dim panelTools As RibbonPanel = New RibbonPanel()
        panelTools.Source = panelSrcTools
        g_excitechRibbonTab.Panels.Add(panelTools)

        'Add M-Files Ribbon Panel
        Dim panelSrcMFiles As RibbonPanelSource = New RibbonPanelSource()
        panelSrcMFiles.Title = My.Resources.EDM_Name
        panelSrcMFiles.Name = "Settings"
        Dim panelMFiles As RibbonPanel = New RibbonPanel()
        panelMFiles.Source = panelSrcMFiles
        g_excitechRibbonTab.Panels.Add(panelMFiles)


        'Add New Button
        Dim btNewDWG As RibbonButton = New RibbonButton()
        btNewDWG.Text = "New"
        btNewDWG.Id = "newDWG"
        btNewDWG.Description = "Create New Drawing in " + My.Resources.EDM_Name
        btNewDWG.ShowText = True
        btNewDWG.ShowImage = True
        btNewDWG.Image = GetBitmapImage(My.Resources.New_20)
        btNewDWG.LargeImage = GetBitmapImage(My.Resources.New_40)
        btNewDWG.Orientation = System.Windows.Controls.Orientation.Vertical
        btNewDWG.Size = RibbonItemSize.Large
        btNewDWG.CommandHandler = g_RibbonCommandHandler

        panelSrcFiles.Items.Add(btNewDWG)
        panelSrcFiles.Items.Add(New RibbonSeparator)

        'Add Open Button
        Dim btOpenDWG As RibbonButton = New RibbonButton()
        btOpenDWG.Text = "Open"
        btOpenDWG.Id = "openDWG"
        btOpenDWG.Description = "Open Drawing from " + My.Resources.EDM_Name
        btOpenDWG.ShowText = True
        btOpenDWG.ShowImage = True
        btOpenDWG.Image = GetBitmapImage(My.Resources.Open_20)
        btOpenDWG.LargeImage = GetBitmapImage(My.Resources.Open_40)
        btOpenDWG.Orientation = System.Windows.Controls.Orientation.Vertical
        btOpenDWG.Size = RibbonItemSize.Large
        btOpenDWG.CommandHandler = g_RibbonCommandHandler

        panelSrcFiles.Items.Add(btOpenDWG)
        panelSrcFiles.Items.Add(New RibbonSeparator)

        'Add Save Button
        Dim btSaveDWG As RibbonButton = New RibbonButton()
        btSaveDWG.Text = "Save"
        btSaveDWG.Id = "saveDWG"
        btSaveDWG.Description = "Save Drawing in " + My.Resources.EDM_Name
        btSaveDWG.ShowText = True
        btSaveDWG.ShowImage = True
        btSaveDWG.Image = GetBitmapImage(My.Resources.Save_20)
        btSaveDWG.LargeImage = GetBitmapImage(My.Resources.Save_40)
        btSaveDWG.Orientation = System.Windows.Controls.Orientation.Vertical
        btSaveDWG.Size = RibbonItemSize.Large
        btSaveDWG.CommandHandler = g_RibbonCommandHandler

        panelSrcFiles.Items.Add(btSaveDWG)

        'Add View In M-Files Button
        Dim btViewInMFiles As RibbonButton = New RibbonButton()
        btViewInMFiles.Text = "View in" + vbCrLf + My.Resources.EDM_Name
        btViewInMFiles.Id = "viewInMFiles"
        btViewInMFiles.Description = "View current Drawing in " + My.Resources.EDM_Name
        btViewInMFiles.ShowText = True
        btViewInMFiles.ShowImage = True
        btViewInMFiles.Image = GetBitmapImage(My.Resources.DOCS_20)
        btViewInMFiles.LargeImage = GetBitmapImage(My.Resources.DOCS_40)
        btViewInMFiles.Orientation = System.Windows.Controls.Orientation.Vertical
        btViewInMFiles.Size = RibbonItemSize.Large
        btViewInMFiles.CommandHandler = g_RibbonCommandHandler

        panelSrcVault.Items.Add(btViewInMFiles)
        panelSrcVault.Items.Add(New RibbonSeparator)

        'Add Browse Button
        Dim btBrowseDWG As RibbonButton = New RibbonButton()
        btBrowseDWG.Text = "Browse" + vbCrLf + My.Resources.EDM_Name
        btBrowseDWG.Id = "browseMFiles"
        btBrowseDWG.Description = "Browse " + My.Resources.EDM_Name + " Vault"
        btBrowseDWG.ShowText = True
        btBrowseDWG.ShowImage = True
        btBrowseDWG.Image = GetBitmapImage(My.Resources.Browse_20)
        btBrowseDWG.LargeImage = GetBitmapImage(My.Resources.Browse_40)
        btBrowseDWG.Orientation = System.Windows.Controls.Orientation.Vertical
        btBrowseDWG.Size = RibbonItemSize.Large
        btBrowseDWG.CommandHandler = g_RibbonCommandHandler

        panelSrcVault.Items.Add(btBrowseDWG)

        'Add Check Out Button
        Dim btCheckOut As RibbonButton = New RibbonButton()
        btCheckOut.Text = "Check Out"
        btCheckOut.Id = "checkOut"
        btCheckOut.Description = "Check Out current Drawing"
        btCheckOut.ShowText = True
        btCheckOut.ShowImage = True
        btCheckOut.Image = GetBitmapImage(My.Resources.CheckOut_20)
        btCheckOut.LargeImage = GetBitmapImage(My.Resources.CheckOut_40)
        btCheckOut.Orientation = System.Windows.Controls.Orientation.Vertical
        btCheckOut.Size = RibbonItemSize.Large
        btCheckOut.CommandHandler = g_RibbonCommandHandler

        panelSrcDocument.Items.Add(btCheckOut)
        panelSrcDocument.Items.Add(New RibbonSeparator)

        'Add Check In Button
        Dim btCheckIn As RibbonButton = New RibbonButton()
        btCheckIn.Text = "Check In"
        btCheckIn.Id = "checkIn"
        btCheckIn.Description = "Check In current Drawing"
        btCheckIn.ShowText = True
        btCheckIn.ShowImage = True
        btCheckIn.Image = GetBitmapImage(My.Resources.CheckIn_20)
        btCheckIn.LargeImage = GetBitmapImage(My.Resources.CheckIn_40)
        btCheckIn.Orientation = System.Windows.Controls.Orientation.Vertical
        btCheckIn.Size = RibbonItemSize.Large
        btCheckIn.CommandHandler = g_RibbonCommandHandler

        panelSrcDocument.Items.Add(btCheckIn)
        panelSrcDocument.Items.Add(New RibbonSeparator)

        'Add Undo Check Out Button
        Dim btUndoCheckoOut As RibbonButton = New RibbonButton()
        btUndoCheckoOut.Text = "Undo" + vbCrLf + "Check Out"
        btUndoCheckoOut.Id = "undoCheckOut"
        btUndoCheckoOut.Description = "Undo Checked Out status of current Drawing"
        btUndoCheckoOut.ShowText = True
        btUndoCheckoOut.ShowImage = True
        btUndoCheckoOut.Image = GetBitmapImage(My.Resources.UndoCheckOut_20)
        btUndoCheckoOut.LargeImage = GetBitmapImage(My.Resources.UndoCheckOut_40)
        btUndoCheckoOut.Orientation = System.Windows.Controls.Orientation.Vertical
        btUndoCheckoOut.Size = RibbonItemSize.Large
        btUndoCheckoOut.CommandHandler = g_RibbonCommandHandler

        panelSrcDocument.Items.Add(btUndoCheckoOut)
        ' panelSrcDocument.Items.Add(New RibbonSeparator)

        'Add Workflow Button
        Dim btWorkflow As RibbonSplitButton = New RibbonSplitButton()
        btWorkflow.Text = "Not in Workflow"
        btWorkflow.Id = "workflowOption"
        btWorkflow.Description = "Change Workflow State"
        btWorkflow.ShowText = True
        btWorkflow.ShowImage = True
        btWorkflow.Image = GetBitmapImage(My.Resources.Workflow_20)
        btWorkflow.LargeImage = GetBitmapImage(My.Resources.Workflow_40)
        btWorkflow.Orientation = System.Windows.Controls.Orientation.Vertical
        btWorkflow.Size = RibbonItemSize.Large
        btWorkflow.IsSplit = False
        btWorkflow.CommandHandler = g_RibbonCommandHandler


        panelSrcWorkflow.Items.Add(btWorkflow)

        'Add Sychronise to M-Files Button
        Dim btSyncToMFiles As RibbonButton = New RibbonButton()
        btSyncToMFiles.Text = "Synchronise to" + vbCrLf + My.Resources.EDM_Name
        btSyncToMFiles.Id = "syncToMFiles"
        btSyncToMFiles.Description = "Synchronise Title block attributes to " + My.Resources.EDM_Name
        btSyncToMFiles.ShowText = True
        btSyncToMFiles.ShowImage = True
        btSyncToMFiles.Image = GetBitmapImage(My.Resources.SyncToDOCS_20)
        btSyncToMFiles.LargeImage = GetBitmapImage(My.Resources.SyncToDOCS_40)
        btSyncToMFiles.Orientation = System.Windows.Controls.Orientation.Vertical
        btSyncToMFiles.Size = RibbonItemSize.Large
        btSyncToMFiles.CommandHandler = g_RibbonCommandHandler

        panelSrcProperties.Items.Add(btSyncToMFiles)
        panelSrcProperties.Items.Add(New RibbonSeparator)

        'Add Sychronise from M-Files Button
        Dim btSyncFromMFiles As RibbonButton = New RibbonButton()
        btSyncFromMFiles.Text = "Synchronise from" + vbCrLf + My.Resources.EDM_Name
        btSyncFromMFiles.Id = "syncFromMFiles"
        btSyncFromMFiles.Description = "Synchronise " + My.Resources.EDM_Name + " metadata to Title Block"
        btSyncFromMFiles.ShowText = True
        btSyncFromMFiles.ShowImage = True
        btSyncFromMFiles.Image = GetBitmapImage(My.Resources.SyncFromDOCS_20)
        btSyncFromMFiles.LargeImage = GetBitmapImage(My.Resources.SyncFromDOCS_40)
        btSyncFromMFiles.Orientation = System.Windows.Controls.Orientation.Vertical
        btSyncFromMFiles.Size = RibbonItemSize.Large
        btSyncFromMFiles.CommandHandler = g_RibbonCommandHandler

        panelSrcProperties.Items.Add(btSyncFromMFiles)
        panelSrcProperties.Items.Add(New RibbonSeparator)

        'Add View Properties Button
        Dim btEditProps As RibbonButton = New RibbonButton()
        btEditProps.Text = "Edit" + vbCrLf + "Properties"
        btEditProps.Id = "editProperties"
        btEditProps.Description = "Edit " + My.Resources.EDM_Name + " metadata and Title Block attributes simultaneously"
        btEditProps.ShowText = True
        btEditProps.ShowImage = True
        btEditProps.Image = GetBitmapImage(My.Resources.EditProperties_20)
        btEditProps.LargeImage = GetBitmapImage(My.Resources.EditProperties_40)
        btEditProps.Orientation = System.Windows.Controls.Orientation.Vertical
        btEditProps.Size = RibbonItemSize.Large
        btEditProps.CommandHandler = g_RibbonCommandHandler

        panelSrcProperties.Items.Add(btEditProps)

        'Add Update References Button
        Dim btUpdateRef As RibbonButton = New RibbonButton()
        btUpdateRef.Text = "Update" + vbCrLf + "References"
        btUpdateRef.Id = "updateRefs"
        btUpdateRef.Description = "Update References from " + My.Resources.EDM_Name
        btUpdateRef.ShowText = True
        btUpdateRef.ShowImage = True
        btUpdateRef.Image = GetBitmapImage(My.Resources.References_20)
        btUpdateRef.LargeImage = GetBitmapImage(My.Resources.References_40)
        btUpdateRef.Orientation = System.Windows.Controls.Orientation.Vertical
        btUpdateRef.Size = RibbonItemSize.Large
        btUpdateRef.CommandHandler = g_RibbonCommandHandler

        panelSrcTools.Items.Add(btUpdateRef)
        panelSrcTools.Items.Add(New RibbonSeparator)

        ''Add Publish PDF Button
        'Dim btPublish As RibbonButton = New RibbonButton()
        'btPublish.Text = "Publish" + vbCrLf + "PDF"
        'btPublish.Id = "publishPDF"
        'btPublish.Description = "Publish PDF to " + My.Resources.EDM_Name
        'btPublish.ShowText = True
        'btPublish.ShowImage = True
        'btPublish.Image = GetBitmapImage(My.Resources.PDF_20)
        'btPublish.LargeImage = GetBitmapImage(My.Resources.PDF_40)
        'btPublish.Orientation = System.Windows.Controls.Orientation.Vertical
        'btPublish.Size = RibbonItemSize.Large
        'btPublish.CommandHandler = g_RibbonCommandHandler

        'panelSrcTools.Items.Add(btPublish)
        'panelSrcTools.Items.Add(New RibbonSeparator)

        'Add View History Button
        Dim btViewHistory As RibbonButton = New RibbonButton()
        btViewHistory.Text = "Show" + vbCrLf + "History"
        btViewHistory.Id = "showHistory"
        btViewHistory.Description = "Show " + My.Resources.EDM_Name + " Drawing History"
        btViewHistory.ShowText = True
        btViewHistory.ShowImage = True
        btViewHistory.Image = GetBitmapImage(My.Resources.History_20)
        btViewHistory.LargeImage = GetBitmapImage(My.Resources.History_40)
        btViewHistory.Orientation = System.Windows.Controls.Orientation.Vertical
        btViewHistory.Size = RibbonItemSize.Large
        btViewHistory.CommandHandler = g_RibbonCommandHandler

        panelSrcTools.Items.Add(btViewHistory)

        'Add Reconnect Button
        Dim panelMFilesRow As New RibbonRowPanel
        panelMFilesRow.IsTopJustified = True
        Dim btReconnect As RibbonButton = New RibbonButton()
        btReconnect.Text = "Reconnect"
        btReconnect.Id = "reconnect"
        btReconnect.Description = "Tries reconnecting to the Vault"
        btReconnect.ShowText = True
        btReconnect.ShowImage = True
        btReconnect.Image = GetBitmapImage(My.Resources.Disconnected_20)
        btReconnect.Orientation = System.Windows.Controls.Orientation.Horizontal
        btReconnect.Size = RibbonItemSize.Standard
        btReconnect.CommandHandler = g_RibbonCommandHandler
        btReconnect.IsEnabled = False
        panelMFilesRow.Items.Add(btReconnect)
        panelMFilesRow.Items.Add(New RibbonRowBreak)

        'Add Settings Button
        Dim btSettings As RibbonButton = New RibbonButton()
        btSettings.Text = "Settings"
        btSettings.Id = "settings"
        btSettings.Description = "Configure " + My.Resources.EDM_Name + " settings"
        btSettings.ShowText = True
        btSettings.ShowImage = True
        btSettings.Image = GetBitmapImage(My.Resources.Settings_20)
        btSettings.Orientation = System.Windows.Controls.Orientation.Horizontal
        btSettings.Size = RibbonItemSize.Standard
        btSettings.CommandHandler = g_RibbonCommandHandler
        panelMFilesRow.Items.Add(btSettings)
        panelMFilesRow.Items.Add(New RibbonRowBreak)

        'Add About Button
        Dim btAbout As RibbonButton = New RibbonButton()
        btAbout.Text = "About"
        btAbout.Id = "about"
        btAbout.Description = "Show " + My.Resources.EDM_Name + " Add-in Information"
        btAbout.ShowText = True
        btAbout.ShowImage = True
        btAbout.Image = GetBitmapImage(My.Resources.Info_20)
        btAbout.Orientation = System.Windows.Controls.Orientation.Horizontal
        btAbout.Size = RibbonItemSize.Standard
        btAbout.CommandHandler = g_RibbonCommandHandler
        panelMFilesRow.Items.Add(btAbout)
        panelMFilesRow.Items.Add(New RibbonRowBreak)
        panelMFilesRow.Items.Add(New RibbonRowBreak)
        panelSrcMFiles.Items.Add(panelMFilesRow)

    End Sub

    Public Sub AddPlugins()
        ''configure the appdomain so reference assemblies can be found
        Dim curDomain As AppDomain = AppDomain.CurrentDomain
        AddHandler curDomain.AssemblyResolve, AddressOf PluginHelper.ResolveAsmReference

        ''whereis this assembly running from
        Dim path As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly.Location)
        path = IO.Path.Combine(path, "Plugins\")

        ''find any dll's in this folder
        ''we should store the plugins some where
        If Not IO.Directory.Exists(path) Then Exit Sub
        Dim _plugin As IPlugin

        For Each fInfo As IO.FileInfo In New IO.DirectoryInfo(path).GetFiles("*.dll")
            Try
                Dim asm As Reflection.Assembly = Reflection.Assembly.LoadFile(fInfo.FullName)
                For Each _type As Type In asm.GetExportedTypes.Where(Function(_t)
                                                                         Return _t.IsClass And _t.GetInterfaces.Contains(GetType(IPlugin))
                                                                     End Function)
                    If Not _type.IsClass Then Continue For
                    If _type.GetInterfaces.Contains(GetType(IPlugin)) Then
                        _plugin = CType(Activator.CreateInstance(_type), IPlugin)
                        _plugin.VaultconnectionString = My.Settings.VaultConnectionName
                        _plugin.Initialize(ComponentManager.Ribbon)

                    End If
                Next
            Catch ex As Exception

            End Try
        Next
    End Sub

    Public Sub Terminate() Implements IExtensionApplication.Terminate

    End Sub

End Class





