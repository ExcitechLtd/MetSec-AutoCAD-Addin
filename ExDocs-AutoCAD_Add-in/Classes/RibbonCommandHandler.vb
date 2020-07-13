Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Windows
Imports System.IO
Imports Autodesk.AutoCAD.Windows
Imports Autodesk.AutoCAD.EditorInput
Imports System.Text.RegularExpressions
Imports System.Runtime.InteropServices


Public Class RibbonCommandHandler
    Implements System.Windows.Input.ICommand

    Public Function CanExecute(parameter As Object) As Boolean Implements System.Windows.Input.ICommand.CanExecute

        Return True
    End Function

    Public Event CanExecuteChanged(sender As Object, e As System.EventArgs) Implements System.Windows.Input.ICommand.CanExecuteChanged


#Region "Ribbon Menu Entry Point"

    ''' <summary>
    ''' ICommand Implementation of Execute.
    ''' Called on Ribbon Button press.
    ''' </summary>
    ''' <param name="parameter"></param>
    ''' <remarks></remarks>
    Public Sub Execute(parameter As Object) Implements System.Windows.Input.ICommand.Execute

        'Get Active Drawing and Database
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acDbase As Database = acDoc.Database

        'check event raised by a RibbonButton
        If TypeOf parameter Is RibbonButton Then
            Dim button As RibbonButton = TryCast(parameter, RibbonButton)

            'Direct to sub routine based on Button Id
            Select Case button.Id
                Case "newDWG"
                    acDoc.SendStringToExecute(ChrW(3) + "DOCSNEW" + vbCr, False, False, True)
                Case "openDWG"
                    openDrawing()
                Case "saveDWG"
                    saveDrawing()
                Case "viewInMFiles"
                    viewInMFiles()
                Case "browseMFiles"
                    browseMFiles()
                Case "checkOut"
                    acDoc.SendStringToExecute(ChrW(3) + "DOCSCHECKOUT" + vbCr, False, False, True)
                Case "checkIn"
                    acDoc.SendStringToExecute(ChrW(3) + "DOCSCHECKIN" + vbCr, False, False, True)
                Case "undoCheckOut"
                    acDoc.SendStringToExecute(ChrW(3) + "DOCSUNDOCHECKOUT" + vbCr, False, False, True)
                Case "syncToMFiles"
                    syncToMFiles()
                Case "syncFromMFiles"
                    syncFromMFiles()
                Case "editProperties"
                    editProperties()
                Case "updateRefs"
                    updateReferences()
                Case "publishPDF"
                    publishPDF()
                Case "showHistory"
                    showHistory()
                Case "reconnect"
                    reconnect()
                Case "goonline"
                    goOnline()
                Case "settings"
                    showSettings()
                Case "about"
                    showAboutBox()
            End Select
        End If
    End Sub
#End Region



#Region "File Button Handlers"

    ''' <summary>
    ''' Create new Drawing in M-File from configured DWG Templates. 
    ''' Show the M-File Document creation wizard
    ''' </summary>
    ''' <remarks></remarks>
    <CommandMethod("DOCSNEW", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub newDrawing()
        'get active document
        Dim acDoc = Application.DocumentManager.MdiActiveDocument

        Try
            'initialise M-Files
            Dim vault = VaultStatus.initialiseMFiles()
            Dim status = VaultStatus.Status()

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            'check connection status
            If status <> VaultStatus.VaultStatuses.connected Then
                VaultStatus.EchoStatus(acDoc)
                acWriteMessage(acDoc, vbCrLf + "Unable to create new Drawing." + vbCrLf)
                Return
            End If

            'Prompt user to select Template
            Dim frmAcadTemplatesUI As New frmAcadTemplates(vault)
            If frmAcadTemplatesUI.ShowDialog(autoCADWindowHandle) = System.Windows.Forms.DialogResult.Cancel Then Return

            'init Creation Info
            Dim oCreationInfo As New MFilesAPI.ObjectCreationInfo()
            oCreationInfo.SetObjectType(0, True)
            oCreationInfo.SetMetadataCardTitle("Create New Drawing")
            oCreationInfo.SetSingleFileDocument(True, True)

            Dim oTemplateObjFileVer As ObjectVersionFile = g_clientApplication.FindFile(frmAcadTemplatesUI.TemplatePath)
            Dim oTemplateObjId As ObjID = oTemplateObjFileVer.ObjectVersion.ObjVer.ObjID
            Dim oTemplateObjVer = vault.ObjectOperations.GetLatestObjVer(oTemplateObjId, False, True)
            oCreationInfo.SetTemplate(oTemplateObjVer)

            Dim objWindowResult As ObjectWindowResult = vault.ObjectOperations.ShowNewObjectWindow(autoCADWindowHandle.Handle, MFilesAPI.MFObjectWindowMode.MFObjectWindowModeInsertSourceFiles, oCreationInfo)

            If objWindowResult.Result = MFObjectWindowResultCode.MFObjectWindowResultCodeOk Then
                Dim objVer = objWindowResult.ObjVer
                Dim oCheckedOutObjVersion = vault.ObjectOperations.GetObjectInfo(objVer, True, False)

                'Get File and Path
                Dim results = From file As ObjectFile In oCheckedOutObjVersion.Files Where file.Extension.ToLower = "dwg"
                Dim dwgFile As ObjectFile = results.First
                Dim vltPath As String = vault.ObjectFileOperations.GetPathInDefaultView(objVer.ObjID, -1, dwgFile.ID, dwgFile.Version, MFLatestSpecificBehavior.MFLatestSpecificBehaviorNormal, False)

                Dim acDocMgr As DocumentCollection = Application.DocumentManager
                DocumentCollectionExtension.Open(acDocMgr, vltPath, False)

                'Sync Properties from MFiles
                syncFromMFiles()
            End If
        Catch ex As Exception
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("NewDrawing ERROR:  " + ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Allow the user to Open a Drawing from with M-Files
    ''' </summary>
    ''' <remarks></remarks>
    <CommandMethod("DOCSOPEN", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub openDrawing()
        'get active document
        Dim acDoc = Application.DocumentManager.MdiActiveDocument

        Try
            'initialise M-Files
            Dim vault = VaultStatus.initialiseMFiles()
            Dim status = VaultStatus.Status()

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            'check connection status
            If Not (status = VaultStatus.VaultStatuses.connected Or status = VaultStatus.VaultStatuses.offline) Then
                VaultStatus.EchoStatus(acDoc)
                acWriteMessage(acDoc, vbCrLf + "Unable to open a Drawing." + vbCrLf)
                Return
            End If

            'build Root Vault Path and show Open File Dialogue
            Dim rootVaultPath As String = g_clientApplication.GetDriveLetter() + ":\" + My.Settings.VaultConnectionName
            Dim ofdBrowseDWG As New OpenFileDialog("Select File", rootVaultPath, "dwg", "OpenDrawing", OpenFileDialog.OpenFileDialogFlags.DoNotTransferRemoteFiles Or OpenFileDialog.OpenFileDialogFlags.DefaultIsFolder Or OpenFileDialog.OpenFileDialogFlags.ForceDefaultFolder)

            If ofdBrowseDWG.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Dim fiDrawing As New FileInfo(ofdBrowseDWG.Filename)

                If fiDrawing.IsReadOnly Then
                    If System.Windows.Forms.MessageBox.Show(ofdBrowseDWG.Filename + " is currently in use or is read-only." + vbCrLf + "Would you like to open the file read-only?", "AutoCAD Alert", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Error) = System.Windows.Forms.DialogResult.No Then openDrawing() : Return
                End If

                Dim acDocMgr As DocumentCollection = Application.DocumentManager
                DocumentCollectionExtension.Open(acDocMgr, ofdBrowseDWG.Filename, False)
            End If
        Catch ex As Exception
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("OpenDrawing ERROR: " + ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Save Active Drawing into M-File Vault
    ''' </summary>
    ''' <remarks></remarks>
    <CommandMethod("DOCSSAVE", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub saveDrawing()
        'get active document
        Dim acDoc = Application.DocumentManager.MdiActiveDocument

        Try
            Dim dwgFilename As String = acDoc.Database.OriginalFileName

            'initialise M-Files
            Dim vault = VaultStatus.initialiseMFiles()
            Dim status As VaultStatus.VaultStatuses = VaultStatus.Status()

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            'check connection status
            If Not (status = VaultStatus.VaultStatuses.connected Or status = VaultStatus.VaultStatuses.offline) Then
                VaultStatus.EchoStatus(acDoc)
                acWriteMessage(acDoc, vbCrLf + "Unable to save Drawing." + vbCrLf)
                Return
            End If

            'If Drawing in MFiles and Checked Out to current user then perform QSAVE, else save as new Document
            If g_clientApplication.IsObjectPathInMFiles(dwgFilename) Then
                Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(dwgFilename)
                vault = objVerProps.Vault
                Dim objID As ObjID = objVerProps.ObjVer.ObjID
                Dim objVer As ObjVer = objVerProps.ObjVer

                'If not Checked Out then save as new Document, else perform QSAVE
                If Not vault.ObjectOperations.IsObjectCheckedOut(objVer.ObjID) Then
                    If status = VaultStatus.VaultStatuses.connected Then
                        saveAsNewDrawing()
                    Else
                        acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : Unable save as new document in Offline Mode." + vbCrLf)
                    End If
                ElseIf vault.ObjectOperations.IsObjectCheckedOutToThisUserOnThisComputer(objVer.ObjID) Then
                    acDoc.SendStringToExecute(ChrW(3) + "_QSAVE" + vbCr, False, False, True)
                End If
            Else
                If status = VaultStatus.VaultStatuses.connected Then
                    saveAsNewDrawing()
                Else
                    acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : Unable save as new document in Offline Mode." + vbCrLf)
                End If
            End If
        Catch ex As Exception
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("SaveDrawing ERROR: " + ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' save Active Drawing as new file in M-Files, Propmpt user for metadata
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub saveAsNewDrawing()
        'get active document
        Dim acDoc = Application.DocumentManager.MdiActiveDocument

        Try
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

            Dim oCreationInfo As New MFilesAPI.ObjectCreationInfo()

            oCreationInfo.SetObjectType(0, True)
            oCreationInfo.SetMetadataCardTitle("Create New Drawing")
            oCreationInfo.SetSingleFileDocument(True, True)

            'add empty file initally
            Dim oSourceFiles As MFilesAPI.SourceObjectFiles = New MFilesAPI.SourceObjectFiles
            Dim oSourceFile1 As MFilesAPI.SourceObjectFile = New MFilesAPI.SourceObjectFile
            oSourceFile1.Extension = "dwg"
            oSourceFiles.Add(0, oSourceFile1)
            oCreationInfo.SetSourceFiles(oSourceFiles)

            Dim objWindowResult As ObjectWindowResult = vault.ObjectOperations.ShowNewObjectWindow(autoCADWindowHandle.Handle, MFilesAPI.MFObjectWindowMode.MFObjectWindowModeInsertSourceFiles, oCreationInfo)

            If objWindowResult.Result = MFObjectWindowResultCode.MFObjectWindowResultCodeOk Then
                Dim objVer = objWindowResult.ObjVer
                Dim oCheckedOutObjVersion = vault.ObjectOperations.GetObjectInfo(objVer, True, False)

                'Get File and Path
                Dim results = From file As ObjectFile In oCheckedOutObjVersion.Files Where file.Extension.ToLower = "dwg"
                Dim dwgFile As ObjectFile = results.First
                Dim vltPath As String = vault.ObjectFileOperations.GetPathInDefaultView(objVer.ObjID, -1, dwgFile.ID, dwgFile.Version, MFLatestSpecificBehavior.MFLatestSpecificBehaviorNormal, False)

                'save drawing
                Using dwgDatabase = acDoc.Database
                    Using trans As Transaction = dwgDatabase.TransactionManager.StartTransaction
                        dwgDatabase.SaveAs(vltPath, DwgVersion.Newest)
                    End Using
                End Using

                'open newly saved drawing
                Dim acDocMgr As DocumentCollection = Application.DocumentManager
                DocumentCollectionExtension.Open(acDocMgr, vltPath, False)
            End If
        Catch ex As System.Runtime.InteropServices.COMException
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("SaveDrawing ERROR: Unable to open Vault Connection '" + My.Settings.VaultConnectionName + "'." + vbCrLf + vbCrLf + "Please ensure the Vault Connection exists and is configured correctly.")
        Catch ex As Exception
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("SaveDrawing ERROR: " + ex.Message)
        End Try
    End Sub
#End Region

#Region "Vault Button Handlers"

    ''' <summary>
    ''' Open M-Files Client and goto the current Drawing 
    ''' </summary>
    ''' <remarks></remarks>
    <CommandMethod("DOCSVIEW", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub viewInMFiles()
        'get active document
        Dim acDoc = Application.DocumentManager.MdiActiveDocument

        Try
            Dim dwgFilename As String = acDoc.Database.OriginalFileName

            'initialise M-Files
            Dim vault = VaultStatus.initialiseMFiles()
            Dim status = VaultStatus.Status()

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            'check connection status
            If Not (status = VaultStatus.VaultStatuses.connected Or status = VaultStatus.VaultStatuses.offline) Then
                VaultStatus.EchoStatus(acDoc)
                acWriteMessage(acDoc, vbCrLf + "Unable to view Drawing in " + My.Resources.EDM_Name + "." + vbCrLf)
                Return
            End If

            'check Drawing is in M-Files
            If Not isMFilesDocument() Then Return

            'get M-Files objects for current drawing
            Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(dwgFilename)
            Dim objID As ObjID = objVerProps.ObjVer.ObjID
            Dim objVer As New ObjVer
            vault = objVerProps.Vault
            objVer.SetIDs(0, objID.ID, -1)
            'get drawing URL and launch Link
            Dim mFilesURLLink As String = vault.ObjectOperations.GetMFilesURLForObjectOrFile(objVer.ObjID, -1, False, -1, MFilesURLType.MFilesURLTypeShow)
            Process.Start(mFilesURLLink)
        Catch ex As Exception
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("ViewInMFiles ERROR: " + ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open M-Files Client for the active Vault Connection
    ''' </summary>
    ''' <remarks></remarks>
    <CommandMethod("DOCSBROWSE", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub browseMFiles()
        'get active document
        Dim acDoc = Application.DocumentManager.MdiActiveDocument

        Try
            'initialise M-Files
            Dim vault = VaultStatus.initialiseMFiles()
            Dim status = VaultStatus.Status()

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            'check connection status
            If Not (status = VaultStatus.VaultStatuses.connected Or status = VaultStatus.VaultStatuses.offline) Then
                VaultStatus.EchoStatus(acDoc)
                acWriteMessage(acDoc, vbCrLf + "Unable to browse " + My.Resources.EDM_Name + "." + vbCrLf)
                Return
            End If

            'Launch M-Files
            Dim rootVaultPath As String = g_clientApplication.GetDriveLetter() + ":\" + My.Settings.VaultConnectionName
            Process.Start("""" + rootVaultPath + """")
        Catch ex As Exception
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("BrowseMFiles ERROR: " + ex.Message)
        End Try
    End Sub
#End Region

#Region "Document Button Handlers"
    ''' <summary>
    ''' Check Out active Drawing in M-Files.
    ''' This forces the Drawing to be reopened.
    ''' </summary>
    ''' <remarks></remarks>
    <CommandMethod("DOCSCHECKOUT", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub checkOut()
        'get active document
        Dim acDoc = Application.DocumentManager.MdiActiveDocument

        Try
            Dim acDocMgr As DocumentCollection = Application.DocumentManager
            Dim dwgFilename As String = acDoc.Database.OriginalFileName
            'initialise M-Files
            Dim vault = VaultStatus.initialiseMFiles()
            Dim status = VaultStatus.Status()

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            'check connection status
            If Not (status = VaultStatus.VaultStatuses.connected Or status = VaultStatus.VaultStatuses.offline) Then
                VaultStatus.EchoStatus(acDoc)
                acWriteMessage(acDoc, vbCrLf + "Unable to Check Out." + vbCrLf)
                Return
            End If
            'check Drawing is in M-Files
            If Not isMFilesDocument() Then Return

            'get Object Version Properties from MFiles
            Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(dwgFilename)
            vault = objVerProps.Vault
            Dim objID As ObjID = objVerProps.ObjVer.ObjID
            Dim objVer As ObjVer = objVerProps.ObjVer

            'check the value of DBMOD, if 0 then the drawing has not been changed
            Dim isModified As Short = Application.GetSystemVariable("DBMOD")

            'Check if Drawing can be Checked Out
            If vault.ObjectOperations.IsObjectCheckedOut(objVer.ObjID) Then
                If isModified = 0 Then 'reopen checked out version
                    DocumentExtension.CloseAndDiscard(acDoc)
                    acDoc.Dispose()
                    DocumentCollectionExtension.Open(acDocMgr, dwgFilename, False)
                    Return
                Else
                    acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : Cannot Check Out the Current Drawing, it is already checked out" + vbCrLf)
                    Return
                End If
            End If

            'if Checkedin Drawing has been modified, warn the user before checking out
            If isModified > 0 Then
                If System.Windows.Forms.MessageBox.Show("You have made changes to the file """ + Path.GetFileName(dwgFilename) + """." + vbCrLf + vbCrLf + "If you 'Check Out' the current drawing, you will lose these changes." + vbCrLf + vbCrLf + "Are you sure you want to continue?", My.Resources.Application_Name, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Exclamation, System.Windows.Forms.MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then Return
            End If

            'close Drawing
            DocumentExtension.CloseAndDiscard(acDoc)
            acDoc.Dispose()

            'check Out and Reopen
            Dim comException As COMException = Nothing
            Try
                'try checking out object
                Dim oCheckedOutObjVersion As ObjectVersion = vault.ObjectOperations.CheckOut(objID)
            Catch ex As COMException
                'store exception
                comException = ex
            Finally
                'reopen drawing
                acDoc = DocumentCollectionExtension.Open(acDocMgr, dwgFilename, False)
                If comException IsNot Nothing Then
                    acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + comException.TruncatedMessage)
                End If
            End Try
        Catch ex As Exception
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("CheckOut ERROR: " + ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Check In active Drawing in M-Files.
    ''' This forces the Drawing to be reopened.
    ''' </summary>
    ''' <remarks></remarks>
    <CommandMethod("DOCSCHECKIN", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub checkIn()

        'get active document
        Dim acDoc = Core.Application.DocumentManager.MdiActiveDocument

        Try
            Dim originalDwgFilename = acDoc.Database.OriginalFileName
            'initialise M-Files
            Dim vault = VaultStatus.initialiseMFiles()
            Dim status = VaultStatus.Status()

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            'check connection status
            If status <> VaultStatus.VaultStatuses.connected Then
                VaultStatus.EchoStatus(acDoc)
                acWriteMessage(acDoc, vbCrLf + "Unable to Check In." + vbCrLf)
                Return
            End If
            'check Drawing is in M-Files
            If Not isMFilesDocument() Then Return

            'get Object Version Properties from MFiles
            Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(originalDwgFilename)
            vault = objVerProps.Vault
            Dim objID As ObjID = objVerProps.ObjVer.ObjID
            Dim objVer As ObjVer = objVerProps.ObjVer

            'Check if Drawing can be Checked In
            If Not vault.ObjectOperations.IsObjectCheckedOutToThisUserOnThisComputer(objVer.ObjID) Then acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : Cannot Check In the Current Drawing" + vbCrLf) : Return

            'close Drawing and Save Drawing
            DocumentExtension.CloseAndSave(acDoc, originalDwgFilename)
            acDoc.Dispose()

            'get M-Files objects for current version
            objVerProps = g_clientApplication.FindObjectVersionAndProperties(originalDwgFilename)
            vault = objVerProps.Vault
            objVer = objVerProps.ObjVer

            'check In
            Dim checkedInObjVersion As ObjectVersion = vault.ObjectOperations.CheckIn(objVer)

            'get full path and open
            Dim acDocMgr As DocumentCollection = Core.Application.DocumentManager

            'remove ID from filename
            Dim rgx As New Regex(" \(ID " + objID.ID.ToString + "\)$")
            Dim basefilename = Path.GetFileNameWithoutExtension(originalDwgFilename)
            basefilename = rgx.Replace(basefilename, "")

            'find Object File base on name (without ID)
            objVer = checkedInObjVersion.ObjVer
            Dim objFile = checkedInObjVersion.Files.GetObjectFileByName(basefilename, "dwg")

            'get full path and open
            Dim updatedDwgFilename = vault.ObjectFileOperations.GetPathInDefaultView(objID, objVer.Version, objFile.ID, objFile.Version)
            DocumentCollectionExtension.Open(acDocMgr, updatedDwgFilename, False)

        Catch ex As Runtime.InteropServices.COMException
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("CheckIn ERROR: " + ex.Message)
        Catch ex As Exception
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("CheckIn ERROR: " + ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Undo Check Out of the active Drawing in M-Files.
    ''' This forces the Drawing to be reopened.
    ''' </summary>
    ''' <remarks></remarks>
    <CommandMethod("DOCSUNDOCHECKOUT", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub undoCheckOut()
        'get active document
        Dim acDoc = Application.DocumentManager.MdiActiveDocument

        Try
            Dim dwgFilename As String = acDoc.Database.OriginalFileName
            'initialise M-Files
            Dim vault = VaultStatus.initialiseMFiles()
            Dim status = VaultStatus.Status()

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            'check connection status
            If status <> VaultStatus.VaultStatuses.connected Then
                VaultStatus.EchoStatus(acDoc)
                acWriteMessage(acDoc, vbCrLf + "Unable to Undo Check Out." + vbCrLf)
                Return
            End If
            'check Drawing is in M-Files
            If Not isMFilesDocument() Then Return

            'get Object Version Properties from MFiles
            Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(dwgFilename)
            vault = objVerProps.Vault
            Dim objID As ObjID = objVerProps.ObjVer.ObjID
            Dim objVer As ObjVer = objVerProps.ObjVer

            'Check if Drawing can be Checked In
            If Not vault.ObjectOperations.IsObjectCheckedOutToThisUserOnThisComputer(objVer.ObjID) Then acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : Cannot Undo Check Out for the Current Drawing" + vbCrLf) : Return

            'check the value of DBMOD, if 0 then the drawing has not been changed
            Dim isModified As Short = Application.GetSystemVariable("DBMOD")
            If isModified > 0 Then If System.Windows.Forms.MessageBox.Show("You have made changes to the file:" + vbCrLf + """" + Path.GetFileName(dwgFilename) + """" + vbCrLf + vbCrLf + "If you 'Undo Check Out', you will lose these changes." + vbCrLf + vbCrLf + "Are you sure you want to continue?", My.Resources.Application_Name, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Exclamation, System.Windows.Forms.MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then Return

            'get M-Files objects for current version
            objVerProps = g_clientApplication.FindObjectVersionAndProperties(dwgFilename)
            vault = objVerProps.Vault
            objVer = objVerProps.ObjVer

            'Check if Initial Version, if it is Warn the User that Undoing Check Out will delete the Drawing
            If objVer.Version = 1 Then
                If System.Windows.Forms.MessageBox.Show("This is the Initial Version of the file:" + vbCrLf + """" + Path.GetFileName(dwgFilename) + """" + vbCrLf + vbCrLf + "If you 'Undo Check Out', the Drawing will be Deleted." + vbCrLf + vbCrLf + "Are you sure you want to continue?", My.Resources.Application_Name, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Exclamation, System.Windows.Forms.MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then Return
                dwgFilename = Nothing
            End If

            'close Drawing
            DocumentExtension.CloseAndDiscard(acDoc)
            acDoc.Dispose()

            'undo Check Out and Reopen
            Dim oCheckedOutObjVersion As ObjectVersion = vault.ObjectOperations.UndoCheckout(objVer)
            If dwgFilename IsNot Nothing Then
                Dim acDocMgr As DocumentCollection = Application.DocumentManager
                DocumentCollectionExtension.Open(acDocMgr, dwgFilename, True)
            End If
        Catch ex As Exception
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("UndoCheckOut ERROR: " + ex.Message)
        End Try
    End Sub
#End Region

#Region "Properties Button Handlers"

    ''' <summary>
    ''' Synchronise Metadata from AutoCAD to MFiles  
    ''' </summary>
    <CommandMethod("DOCSSYNCTOMFILES", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub syncToMFiles()
        'get active document
        Dim acDoc = Application.DocumentManager.MdiActiveDocument

        Try
            Dim dwgFilename As String = acDoc.Database.OriginalFileName
            'initialise M-Files
            Dim vault = VaultStatus.initialiseMFiles()
            Dim status = VaultStatus.Status()

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            'check connection status
            If Not (status = VaultStatus.VaultStatuses.connected Or status = VaultStatus.VaultStatuses.offline) Then
                VaultStatus.EchoStatus(acDoc)
                acWriteMessage(acDoc, vbCrLf + "Unable to synchronise properties to " + My.Resources.EDM_Name + "." + vbCrLf)
                Return
            End If
            'check Drawing is in M-Files
            If Not isMFilesDocument() Then Return

            'get M-Files objects for current drawing
            Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(dwgFilename)
            vault = objVerProps.Vault
            Dim objVer As ObjVer = objVerProps.ObjVer
            Dim objVersion As ObjectVersion = objVerProps.VersionData

            'get Titleblock object ID's
            Dim acObjIds As List(Of ObjectId) = GetAllSheetBlockIds(vault)

            'check Drawing is Checked Out to current User on this computer
            If Not vault.ObjectOperations.IsObjectCheckedOutToThisUserOnThisComputer(objVer.ObjID) Then acWriteMessage(acDoc, My.Resources.Application_Name + " : Current Drawing not Checked Out") : Return

            'check the number of matching Titleblocks found
            Select Case acObjIds.Count
                Case 0
                    acWriteMessage(acDoc, vbCrLf + "No matching Titleblocks found." + vbCrLf)
                Case 1
                    updateMFilesFromAttributes(vault, objVersion, acObjIds.First)
                Case Else
                    Dim promptEntRes As PromptEntityResult = acDoc.Editor.GetEntity("Multiple matching Titleblocks found. Please select the Titlebock to synchronise from:")
                    If promptEntRes.Status = PromptStatus.OK Then
                        If acObjIds.Contains(promptEntRes.ObjectId) Then
                            updateMFilesFromAttributes(vault, objVersion, promptEntRes.ObjectId)
                        Else
                            acWriteMessage(acDoc, vbCrLf + "Not a supported Titleblock." + vbCrLf)
                        End If
                    End If
            End Select
        Catch ex As Exception
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("SyncToMFiles ERROR: " + ex.Message)
        End Try
        Ribbon.updateRibbonState(acDoc)
    End Sub

    ''' <summary>
    ''' Synchronise Metadata from MFiles to AutoCAD  
    ''' </summary>
    <CommandMethod("DOCSSYNCFROMMFILES", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub syncFromMFiles()
        'get active document
        Dim acDoc = Application.DocumentManager.MdiActiveDocument

        Try
            Dim dwgFilename As String = acDoc.Database.OriginalFileName
            'initialise M-Files
            Dim vault = VaultStatus.initialiseMFiles()
            Dim status = VaultStatus.Status()

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            'check connection status
            If Not (status = VaultStatus.VaultStatuses.connected Or status = VaultStatus.VaultStatuses.offline) Then
                VaultStatus.EchoStatus(acDoc)
                acWriteMessage(acDoc, vbCrLf + "Unable to synchronise properties from " + My.Resources.EDM_Name + "." + vbCrLf)
                Return
            End If
            'check Drawing is in M-Files
            If Not isMFilesDocument() Then Return

            'get M-Files objects for current drawing
            Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(dwgFilename)
            vault = objVerProps.Vault
            Dim objVer As ObjVer = objVerProps.ObjVer

            'check Drawing is Checked Out to current User on this computer
            If Not vault.ObjectOperations.IsObjectCheckedOutToThisUserOnThisComputer(objVer.ObjID) Then acWriteMessage(acDoc, My.Resources.Application_Name + " : Current Drawing not Checked Out") : Return

            'update attribute values of matching Titleblock
            Dim acObjIds As List(Of ObjectId) = GetAllSheetBlockIds(vault)
            For Each acObjId As ObjectId In acObjIds
                updateAttributesFromMFiles(vault, objVer, acObjId)
            Next
            acWriteMessage(acDoc, vbCrLf + "Synchronise from M-Files Complete" + vbCrLf)
        Catch ex As Exception
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("SyncFromMFiles ERROR: " + ex.Message)
        End Try
        Ribbon.updateRibbonState(acDoc)
    End Sub


    ''' <summary>
    ''' Edit MFiles metadata and AutoCAD title block simualtaniously
    ''' </summary>
    <CommandMethod("DOCSEDITPROPS", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub editProperties()
        'get active document
        Dim acDoc = Application.DocumentManager.MdiActiveDocument

        Try
            Dim dwgFilename As String = acDoc.Database.OriginalFileName
            'initialise M-Files
            Dim vault = VaultStatus.initialiseMFiles()
            Dim status = VaultStatus.Status()

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            'check connection status
            If Not (status = VaultStatus.VaultStatuses.connected Or status = VaultStatus.VaultStatuses.offline) Then
                VaultStatus.EchoStatus(acDoc)
                acWriteMessage(acDoc, vbCrLf + "Unable to edit properties in " + My.Resources.EDM_Name + "." + vbCrLf)
                Return
            End If

            'check Drawing is in M-Files
            If Not isMFilesDocument() Then Return

            'get M-Files objects for current drawing
            Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(dwgFilename)
            vault = objVerProps.Vault
            Dim objID As ObjID = objVerProps.ObjVer.ObjID
            Dim objVer = objVerProps.ObjVer

            'check Drawing is Checked Out to current User on this computer
            If Not vault.ObjectOperations.IsObjectCheckedOutToThisUserOnThisComputer(objVer.ObjID) Then acWriteMessage(acDoc, My.Resources.Application_Name + " : Current Drawing not Checked Out") : Return

            'display M-Files metadata edit window
            Dim objWindowResult = vault.ObjectOperations.ShowEditObjectWindow(Application.MainWindow.Handle, MFObjectWindowMode.MFObjectWindowModeEdit, objVer)
            If objWindowResult.Result = MFObjectWindowResultCode.MFObjectWindowResultCodeOk Then
                'update attribute values of matching Titleblock
                Dim acObjIds As List(Of ObjectId) = GetAllSheetBlockIds(vault)
                For Each acObjId As ObjectId In acObjIds
                    updateAttributesFromMFiles(vault, objVer, acObjId)
                Next
            End If

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            acWriteMessage(acDoc, vbCrLf + "Edit Properties Complete" + vbCrLf)
        Catch ex As Exception
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("EditProperties ERROR: " + ex.Message)
        End Try
    End Sub
#End Region

#Region "Tools Button Handlers"

    ''' <summary>
    ''' Shows Publish PDF dialogue box
    ''' </summary>
    <CommandMethod("DOCSPUBLISH", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub publishPDF()
        'get active document
        Dim acDoc = Application.DocumentManager.MdiActiveDocument

        Try
            Dim dwgFilename As String = acDoc.Database.OriginalFileName
            'initialise M-Files
            Dim vault = VaultStatus.initialiseMFiles()
            Dim status = VaultStatus.Status()

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            'check connection status
            If status <> VaultStatus.VaultStatuses.connected Then
                VaultStatus.EchoStatus(acDoc)
                acWriteMessage(acDoc, vbCrLf + "Unable to publish PDF rendition." + vbCrLf)
                Return
            End If
            'check Drawing is in M-Files
            If Not isMFilesDocument() Then Return

            'get M-Files objects for current drawing
            Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(dwgFilename)
            vault = objVerProps.Vault
            Dim objVer As ObjVer = objVerProps.ObjVer

            'check Drawing is Checked Out to current User on this computer
            If Not vault.ObjectOperations.IsObjectCheckedOutToThisUserOnThisComputer(objVer.ObjID) Then acWriteMessage(acDoc, My.Resources.Application_Name + " : Current Drawing not Checked Out") : Return

            'check user has saved recent changes
            Dim isModified As Short = Application.GetSystemVariable("DBMOD")
            If isModified > 0 Then System.Windows.Forms.MessageBox.Show("Please save your changes before Publishing a PDF.", My.Resources.Application_Name, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation) : Return

            'show publishing interface
            Dim frmPDFOptionsUI As New frmPDFOptions
            Dim parentWindow As New WindowWrapper(Application.MainWindow.Handle)
            frmPDFOptionsUI.ShowDialog(parentWindow)
        Catch ex As Exception
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("PublishPDF ERROR: " + ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Shows Revision History of current Drawing in MFiles
    ''' </summary>
    <CommandMethod("DOCSHISTORY", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub showHistory()
        'get active document
        Dim acDoc = Application.DocumentManager.MdiActiveDocument

        Try
            Dim dwgFilename As String = acDoc.Database.OriginalFileName
            'initialise M-Files
            Dim vault = VaultStatus.initialiseMFiles()
            Dim status = VaultStatus.Status()

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            'check connection status
            If status <> VaultStatus.VaultStatuses.connected Then
                VaultStatus.EchoStatus(acDoc)
                acWriteMessage(acDoc, vbCrLf + "Unable to show drawing history in " + My.Resources.EDM_Name + "." + vbCrLf)
                Return
            End If
            'check Drawing is in M-Files
            If Not isMFilesDocument() Then Return

            'get M-Files objects for current drawing
            Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(dwgFilename)
            vault = objVerProps.Vault
            Dim objID As ObjID = objVerProps.ObjVer.ObjID

            'Show History dialogue box
            vault.ObjectOperations.ShowHistoryDialogModal(Application.MainWindow.Handle, objID)

        Catch ex As COMException
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.TruncatedMessage)
            WriteLogFile("ShowHistory ERROR: " + ex.Message)
        Catch ex As Exception
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("ShowHistory ERROR: " + ex.Message)
        End Try
    End Sub
#End Region

#Region "Reconnect\Settings\About Button Handlers"

    ''' <summary>
    ''' Show the Client Settings Dialogue Box.
    ''' </summary>
    ''' <remarks></remarks>
    <CommandMethod("DOCSRECONNECT", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub reconnect()

        'get active document
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument


        'Reinitialise M-Files
        VaultStatus.initialiseMFiles(True)
        Dim status = VaultStatus.Status()
        VaultStatus.EchoStatus(acDoc)

        'Update Ribbon State
        Ribbon.updateRibbonState(Application.DocumentManager.MdiActiveDocument)
    End Sub

    <CommandMethod("DOCSGOONLINE", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub goOnline()

        'get active document
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument

        'Reinitialise M-Files
        Dim vault = VaultStatus.initialiseMFiles()
        Dim status As VaultStatus.VaultStatuses = VaultStatus.Status()
        If status = VaultStatus.VaultStatuses.offline Then
            vault.ClientOperations.SetVaultToOnline(0)
        End If

        'Update Ribbon State
        Ribbon.updateRibbonState(Application.DocumentManager.MdiActiveDocument)
    End Sub

    ''' <summary>
    ''' Show the Client Settings Dialogue Box.
    ''' </summary>
    ''' <remarks></remarks>
    <CommandMethod("DOCSSETTINGS", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub showSettings()

        Dim frmSettingsUI As New frmClientSettings()
        Dim parentWindow As New WindowWrapper(Application.MainWindow.Handle)
        If frmSettingsUI.ShowDialog(parentWindow) = System.Windows.Forms.DialogResult.OK Then
            VaultStatus.initialiseMFiles(True)
        End If
        'Update Ribbon State
        Ribbon.updateRibbonState(Application.DocumentManager.MdiActiveDocument)
    End Sub

    ''' <summary>
    ''' Show the Add-in About Dialogue Box.
    ''' </summary>
    ''' <remarks></remarks>
    <CommandMethod("DOCSABOUT", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub showAboutBox()

        Dim frmAboutUI As New frmAbout
        Dim parentWindow As New WindowWrapper(Application.MainWindow.Handle)
        frmAboutUI.ShowDialog(parentWindow)
        'Update Ribbon State
        Ribbon.updateRibbonState(Application.DocumentManager.MdiActiveDocument)
    End Sub
#End Region

#Region "References"
    ''' <summary>
    ''' Shows Update XRef dialogue box
    ''' </summary>
    <CommandMethod("DOCSUPDATEREFS", CommandFlags.Session + CommandFlags.Modal)>
    Public Sub updateReferences()
        'get active document
        Dim acDoc = Application.DocumentManager.MdiActiveDocument

        Try
            Dim dwgFilename As String = acDoc.Database.OriginalFileName
            'initialise M-Files
            Dim vault = VaultStatus.initialiseMFiles()
            Dim status = VaultStatus.Status()

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            'check connection status
            If status <> VaultStatus.VaultStatuses.connected Then
                VaultStatus.EchoStatus(acDoc)
                acWriteMessage(acDoc, vbCrLf + "Unable to update references." + vbCrLf)
                Return
            End If
            'check Drawing is in M-Files
            If Not isMFilesDocument() Then Return

            'get M-Files objects for current drawing
            Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(dwgFilename)
            vault = objVerProps.Vault
            Dim objVer As ObjVer = objVerProps.ObjVer

            'check Drawing is Checked Out to current User on this computer
            If Not vault.ObjectOperations.IsObjectCheckedOutToThisUserOnThisComputer(objVer.ObjID) Then acWriteMessage(acDoc, My.Resources.Application_Name + " : Current Drawing not Checked Out") : Return

            'show Update References dialogue
            Dim frmUpdateRefUI As New frmUpdateReferences
            Dim parentWindow As New WindowWrapper(Application.MainWindow.Handle)
            'Application.ShowModalDialog(frmUpdateRefUI)
            frmUpdateRefUI.ShowDialog(parentWindow)
        Catch ex As Exception
            acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : ERROR - " + ex.Message)
            WriteLogFile("UpdateReferences ERROR: " + ex.Message)
        End Try
    End Sub
#End Region
End Class
