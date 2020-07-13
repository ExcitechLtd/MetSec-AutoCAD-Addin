Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.Windows
Imports System.IO
Imports System.Text.RegularExpressions

Public Class WorkflowCommandHandler
    Implements System.Windows.Input.ICommand

    Public Function CanExecute(parameter As Object) As Boolean Implements System.Windows.Input.ICommand.CanExecute
        Return True
    End Function

    Public Event CanExecuteChanged(sender As Object, e As System.EventArgs) Implements System.Windows.Input.ICommand.CanExecuteChanged

    Public Sub Execute(parameter As Object) Implements System.Windows.Input.ICommand.Execute

        If TypeOf parameter Is RibbonButton Then
            Dim button As RibbonButton = TryCast(parameter, RibbonButton)

            'get active document
            Dim acDoc = Application.DocumentManager.MdiActiveDocument
            'initialise M-Files
            Dim status = VaultStatus.Status()

            'Update Ribbon State
            Ribbon.updateRibbonState(acDoc)

            'check connection status
            If status <> VaultStatus.VaultStatuses.connected Then
                VaultStatus.EchoStatus(acDoc)
                Return
            End If

            workflow(button)
        End If
    End Sub

    Private Sub workflow(button As RibbonButton)

        Dim originalDwgFilename As String = Nothing

        Try
            Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
            originalDwgFilename = acDoc.Database.OriginalFileName

            'get refernece to Workflow button
            Dim btWorkflowOptions As RibbonSplitButton = g_excitechRibbonTab.Panels.GetPanelByName("Workflow").FindItem("workflowOption")
            Dim workflowData As String() = Split(button.Id, "_")
            Dim transitionID As Integer = CInt(workflowData(1))

            ' Check the value of DBMOD, if 0 then the drawing has not been changed
            Dim isModified As Short = Application.GetSystemVariable("DBMOD")
            If isModified > 0 Then
                If Windows.Forms.MessageBox.Show("You have made changes to the file """ + Path.GetFileName(originalDwgFilename) + """." + vbCrLf + vbCrLf + "If you check out the current drawing, you will lose these changes." + vbCrLf + vbCrLf + "Are you sure you want to continue?", "M-Files - Check Out", Windows.Forms.MessageBoxButtons.YesNo, Windows.Forms.MessageBoxIcon.Exclamation, Windows.Forms.MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then
                    btWorkflowOptions.Current = btWorkflowOptions.Items(0)
                    Return
                End If
            End If

            'show Workflow transition dialogue
            Dim frmWorkflowCommentUI As New frmWorkflowComment(button.Text)
            Dim parentWindow As New WindowWrapper(Application.MainWindow.Handle)
            If frmWorkflowCommentUI.ShowDialog(parentWindow) = Windows.Forms.DialogResult.Cancel Then
                btWorkflowOptions.Current = btWorkflowOptions.Items(0)
                Return
            End If

            'close Drawing
            DocumentExtension.CloseAndDiscard(acDoc)
            acDoc.Dispose()

            'get M-Files objects for current drawing
            Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(originalDwgFilename)
            Dim vault As Vault = objVerProps.Vault
            Dim objVer As ObjVer = objVerProps.ObjVer
            Dim objID As ObjID = objVerProps.ObjVer.ObjID

            Dim workflowDef = vault.ObjectPropertyOperations.GetProperty(objVer, 38)
            Dim workflowState = vault.ObjectPropertyOperations.GetProperty(objVer, 39)

            'Change Wrokflow State
            objVerProps = vault.ObjectPropertyOperations.SetWorkflowStateTransition(objVerProps.ObjVer, workflowDef.TypedValue.GetLookupID, transitionID, frmWorkflowCommentUI.Comment)

            If frmWorkflowCommentUI.ReopenDrawing Then
                Dim acDocMgr As DocumentCollection = Application.DocumentManager

                'remove ID from filename
                Dim rgx As New Regex(" \(ID " + objID.ID.ToString + "\)$")
                Dim basefilename = Path.GetFileNameWithoutExtension(originalDwgFilename)
                basefilename = rgx.Replace(basefilename, "")

                'find Object File base on name (without ID)
                Dim objFile = objVerProps.VersionData.Files.GetObjectFileByName(basefilename, "dwg")

                'get full path and open
                Dim updatedDwgFilename = vault.ObjectFileOperations.GetPathInDefaultView(objID, objVerProps.ObjVer.Version, objFile.ID, objFile.Version)
                DocumentCollectionExtension.Open(acDocMgr, updatedDwgFilename, False)
            End If
        Catch ex As Exception
            Dim errorMessage As String = "Unable to execute Workflow transition." + vbCrLf + vbCrLf
            errorMessage += ex.TruncatedMessage

            Windows.Forms.MessageBox.Show(errorMessage, My.Resources.Application_Name, Windows.Forms.MessageBoxButtons.OK, Windows.Forms.MessageBoxIcon.Error)
            WriteLogFile("workflow ERROR: " + ex.Message)

            'Reopen Drawing
            If originalDwgFilename Is Nothing Then Return
            Dim acDocMgr As DocumentCollection = Application.DocumentManager
            Dim acDoc As Document = DocumentCollectionExtension.Open(acDocMgr, originalDwgFilename, False)
        End Try
    End Sub

End Class
