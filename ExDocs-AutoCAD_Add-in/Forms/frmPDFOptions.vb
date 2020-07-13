Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.PlottingServices
Imports System.IO

Public Class frmPDFOptions

    Private m_PDFPath As String
    Private m_isGeneratingPreview As Boolean

    Private Sub frmPDFOptions_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        cbPaperSize.SelectedIndex = 3
        cbScale.SelectedIndex = 0

        InitLayouts()
        InitLayerStates()

    End Sub

    Private Sub frmPDFOptions_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        'If it exists then delete existing file
        If File.Exists(m_PDFPath) Then File.Delete(m_PDFPath)
    End Sub

    Private Sub InitLayouts()

        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acDbase As Database = acDoc.Database

        Dim layoutMgr As LayoutManager = LayoutManager.Current
        Dim currentLayout As String = layoutMgr.CurrentLayout

        cbLayout.Items.Clear()
        Using acTrans As Transaction = acDbase.TransactionManager.StartTransaction()

            Dim layoutDic As DBDictionary = TryCast(acTrans.GetObject(acDbase.LayoutDictionaryId, OpenMode.ForRead, False), DBDictionary)

            For Each acDicEntry As DBDictionaryEntry In layoutDic
                Dim acLayoutId As ObjectId = acDicEntry.Value
                Dim acLayout As Layout = TryCast(acTrans.GetObject(acLayoutId, OpenMode.ForRead), Layout)
                cbLayout.Items.Add(acLayout.LayoutName)
            Next
        End Using

        cbLayout.Text = currentLayout

    End Sub

    Private Sub InitLayerStates()

        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acDbase As Database = acDoc.Database

        ' Start a transaction
        Using acTrans As Transaction = acDbase.TransactionManager.StartTransaction()

            Dim acLyrStMan As LayerStateManager
            acLyrStMan = acDbase.LayerStateManager

            Dim acDbDict As DBDictionary
            acDbDict = acTrans.GetObject(acLyrStMan.LayerStatesDictionaryId(True), OpenMode.ForRead)

            cbLayerState.Items.Clear()
            cbLayerState.Items.Add("Current Layout")
            For Each acDbDictEnt As DBDictionaryEntry In acDbDict
                cbLayerState.Items.Add(acDbDictEnt.Key)
            Next

        End Using
        cbLayerState.SelectedIndex = 0

    End Sub


    Private Sub PlotCurrentLayout(PDFPath As String)

        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim sCurrentPaperSize As String = cbPaperSize.Text.Replace(" ", "_")

        'Switch Off Background Plotting
        Application.SetSystemVariable("BACKGROUNDPLOT", 0)

        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
            '' Reference the Layout Manager
            Dim acLayoutMgr As LayoutManager
            acLayoutMgr = LayoutManager.Current

            '' Get the current layout and output its name in the Command Line window
            Dim acLayout As Layout
            acLayout = acTrans.GetObject(acLayoutMgr.GetLayoutId(acLayoutMgr.CurrentLayout), OpenMode.ForRead)

            '' Get the PlotInfo from the layout
            Dim acPlInfo As PlotInfo = New PlotInfo()
            acPlInfo.Layout = acLayout.ObjectId

            '' Get a copy of the PlotSettings from the layout
            Dim acPlSet As PlotSettings = New PlotSettings(acLayout.ModelType)
            acPlSet.CopyFrom(acLayout)

            '' Update the PlotSettings object
            Dim acPlSetVdr As PlotSettingsValidator = PlotSettingsValidator.Current

            acPlSetVdr.RefreshLists(acPlSet)

            '' Set the plot type
            acPlSetVdr.SetPlotType(acPlSet, Autodesk.AutoCAD.DatabaseServices.PlotType.Extents)


            '' Set the plot scale
            acPlSetVdr.SetUseStandardScale(acPlSet, True)
            Select Case cbScale.Text
                Case "Scaled to Fit"
                    acPlSetVdr.SetStdScaleType(acPlSet, StdScaleType.ScaleToFit)
                Case "1:1"
                    acPlSetVdr.SetStdScaleType(acPlSet, StdScaleType.StdScale1To1)
                Case "1:2"
                    acPlSetVdr.SetStdScaleType(acPlSet, StdScaleType.StdScale1To2)
                Case "1:10"
                    acPlSetVdr.SetStdScaleType(acPlSet, StdScaleType.StdScale1To10)
                Case "1:20"
                    acPlSetVdr.SetStdScaleType(acPlSet, StdScaleType.StdScale1To20)
                Case "1:50"
                    acPlSetVdr.SetStdScaleType(acPlSet, StdScaleType.StdScale1To50)
                Case "1:100"
                    acPlSetVdr.SetStdScaleType(acPlSet, StdScaleType.StdScale1To100)
            End Select

            '' Set CTB file
            acPlSetVdr.SetCurrentStyleSheet(acPlSet, "acad.ctb")
            '' Center the plot
            acPlSetVdr.SetPlotCentered(acPlSet, True)


            '' Set Orientation
            If sCurrentPaperSize = "ISO_full_bleed_A0_(841.00_x_1189.00_MM)" Then
                acPlSetVdr.SetPlotRotation(acPlSet, PlotRotation.Degrees270)
            Else
                acPlSetVdr.SetPlotRotation(acPlSet, PlotRotation.Degrees000)
            End If


            '' Set the plot device to use
            acPlSetVdr.SetPlotConfigurationName(acPlSet, "DWG To PDF.pc3", sCurrentPaperSize)

            '' Set the plot info as an override since it will
            '' not be saved back to the layout
            acPlInfo.OverrideSettings = acPlSet

            '' Validate the plot info
            Dim acPlInfoVdr As PlotInfoValidator = New PlotInfoValidator()
            acPlInfoVdr.MediaMatchingPolicy = MatchingPolicy.MatchEnabled
            acPlInfoVdr.Validate(acPlInfo)

            '' Check to see if a plot is already in progress
            If PlotFactory.ProcessPlotState = Autodesk.AutoCAD.PlottingServices.ProcessPlotState.NotPlotting Then
                Using acPlEng As PlotEngine = PlotFactory.CreatePublishEngine()

                    '' Track the plot progress with a Progress dialog
                    Dim acPlProgDlg As PlotProgressDialog = New PlotProgressDialog(False, 1, False)

                    Using (acPlProgDlg)
                        '' Define the status messages to display when plotting starts
                        acPlProgDlg.PlotMsgString(PlotMessageIndex.DialogTitle) = "Plot Progress"
                        acPlProgDlg.PlotMsgString(PlotMessageIndex.CancelJobButtonMessage) = "Cancel Job"
                        acPlProgDlg.PlotMsgString(PlotMessageIndex.CancelSheetButtonMessage) = "Cancel Sheet"
                        acPlProgDlg.PlotMsgString(PlotMessageIndex.SheetSetProgressCaption) = "Sheet Set Progress"
                        acPlProgDlg.PlotMsgString(PlotMessageIndex.SheetProgressCaption) = "Sheet Progress"

                        '' Set the plot progress range
                        acPlProgDlg.LowerPlotProgressRange = 0
                        acPlProgDlg.UpperPlotProgressRange = 100
                        acPlProgDlg.PlotProgressPos = 0

                        '' Display the Progress dialog
                        acPlProgDlg.OnBeginPlot()
                        acPlProgDlg.IsVisible = True

                        '' Start to plot the layout
                        acPlEng.BeginPlot(acPlProgDlg, Nothing)

                        '' Define the plot output
                        acPlEng.BeginDocument(acPlInfo, acDoc.Name, Nothing, 1, True, PDFPath)

                        '' Display information about the current plot
                        acPlProgDlg.PlotMsgString(PlotMessageIndex.Status) = "Plotting: " & acDoc.Name & " - " & acLayout.LayoutName

                        '' Set the sheet progress range
                        acPlProgDlg.OnBeginSheet()
                        acPlProgDlg.LowerSheetProgressRange = 0
                        acPlProgDlg.UpperSheetProgressRange = 100
                        acPlProgDlg.SheetProgressPos = 0

                        '' Plot the first sheet/layout
                        Dim acPlPageInfo As PlotPageInfo = New PlotPageInfo()
                        acPlEng.BeginPage(acPlPageInfo, acPlInfo, True, Nothing)

                        acPlEng.BeginGenerateGraphics(Nothing)
                        acPlEng.EndGenerateGraphics(Nothing)

                        '' Finish plotting the sheet/layout
                        acPlEng.EndPage(Nothing)
                        acPlProgDlg.SheetProgressPos = 100
                        acPlProgDlg.OnEndSheet()

                        '' Finish plotting the document
                        acPlEng.EndDocument(Nothing)

                        '' Finish the plot
                        acPlProgDlg.PlotProgressPos = 100
                        acPlProgDlg.OnEndPlot()
                        acPlEng.EndPlot(Nothing)
                    End Using
                End Using
            End If
        End Using
    End Sub

    Public Sub SetLayout(LayoutName As String)

        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database

        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
            Try
                Dim acLayoutMgr As LayoutManager
                acLayoutMgr = LayoutManager.Current
                acLayoutMgr.CurrentLayout = LayoutName
                acTrans.Commit()
            Catch ex As Autodesk.AutoCAD.Runtime.Exception
                Windows.Forms.MessageBox.Show("ERROR: " + ex.Message + vbCr + ex.StackTrace, My.Resources.Application_Name, Windows.Forms.MessageBoxButtons.OK, Windows.Forms.MessageBoxIcon.Error)
            End Try

        End Using

    End Sub

    Public Sub RestoreLayerState(LayerStateName As String)
        '' Get the current document
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument

        Dim acLyrStMan As LayerStateManager
        acLyrStMan = acDoc.Database.LayerStateManager

        acLyrStMan.RestoreLayerState(LayerStateName, ObjectId.Null, 1,
            LayerStateMasks.Color + LayerStateMasks.CurrentViewport + LayerStateMasks.Frozen +
            LayerStateMasks.LineType + LayerStateMasks.LineWeight + LayerStateMasks.On + LayerStateMasks.Plot +
            LayerStateMasks.PlotStyle + LayerStateMasks.Transparency)

    End Sub


    Private Sub btPreview_Click(sender As System.Object, e As System.EventArgs) Handles btPreview.Click

        If m_isGeneratingPreview Then Return
        m_isGeneratingPreview = True

        Try
            'get the current document
            Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
            Dim dwgBaseName As String = Path.GetFileNameWithoutExtension(Application.GetSystemVariable("DWGNAME"))

            Using doclock As DocumentLock = acDoc.LockDocument
                'Set Selected Layout
                SetLayout(cbLayout.Text)
                'Restore Layer State
                If cbLayerState.Text <> "Current Layout" Then RestoreLayerState(cbLayerState.Text)

                'Make Temp PDF Path
                m_PDFPath = Path.Combine(Path.GetTempPath, dwgBaseName + ".pdf")

                'If it exists then delete existing file
                If File.Exists(m_PDFPath) Then File.Delete(m_PDFPath)

                btPreview.Visible = False
                btPublish.Visible = False
                lbWarning.Visible = True

                PlotCurrentLayout(m_PDFPath)

                'Dim pAcrobat As Process = Process.Start(m_PDFPath)
                'pAcrobat.WaitForExit()
                Dim frmPDFPreviewUI As New frmPDFPreview(m_PDFPath)
                frmPDFPreviewUI.ShowDialog(Me)

            End Using
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ERROR: " + ex.Message, My.Resources.Application_Name, Windows.Forms.MessageBoxButtons.OK, Windows.Forms.MessageBoxIcon.Error)
            WriteLogFile("Generate PDF Preview ERROR: " + ex.Message + vbCrLf + vbCr + ex.StackTrace)
        End Try

        btPreview.Visible = True
        btPublish.Visible = True
        btPublish.Enabled = True
        lbWarning.Visible = False

        m_isGeneratingPreview = False

    End Sub

    Private Sub btPublish_Click(sender As System.Object, e As System.EventArgs) Handles btPublish.Click

        Try
            'get active document
            Dim acDoc = Application.DocumentManager.MdiActiveDocument
            'initialise M-Files
            Dim vault = VaultStatus.initialiseMFiles()
            Dim status = VaultStatus.Status()

            'check connection status
            If status <> VaultStatus.VaultStatuses.connected Then Throw New Exception("Unable to connect to Vault.")

            Dim vaultSettings = AutoCADVaultSettings.ReadSettings(vault)

            'Publish PDF file
            publishPdfRenditionMultiFile()

            acWriteMessage(acDoc, vbCrLf + "PDF Publish Complete" + vbCrLf)
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ERROR: " + ex.Message, My.Resources.Application_Name, Windows.Forms.MessageBoxButtons.OK, Windows.Forms.MessageBoxIcon.Error)
            WriteLogFile("PublishPDF ERROR: " + ex.Message)
        End Try

        Close()
    End Sub


    Private Sub publishPdfRenditionMultiFile()

        'initialise M-Files
        Dim dwgFilename As String = Application.DocumentManager.MdiActiveDocument.Database.Filename
        Dim oClientApp As New MFilesClientApplication
        Dim objVerProps As ObjectVersionAndProperties = oClientApp.FindObjectVersionAndProperties(dwgFilename)
        Dim vault As Vault = objVerProps.Vault
        Dim objVer As ObjVer = objVerProps.ObjVer
        Dim basePDFName As String = Path.GetFileNameWithoutExtension(objVerProps.VersionData.GetNameForFileSystem(False))

        Dim file As ObjectFile = vault.ObjectFileOperations.GetFiles(objVer).GetObjectFileByName(basePDFName, "pdf")
        If file Is Nothing Then
            vault.ObjectFileOperations.AddFile(objVer, basePDFName, "PDF", m_PDFPath)
        Else
            'replace content
            Dim mfilePDFPath = vault.ObjectFileOperations.GetPathInDefaultView(objVer.ObjID, objVer.Version, file.ID, file.Version, MFLatestSpecificBehavior.MFLatestSpecificBehaviorLatest, False)
            IO.File.Copy(m_PDFPath, mfilePDFPath, True)
            'oVault.ObjectFileOperations.RemoveFile(oObjVer, file.FileVer)
        End If

    End Sub

End Class