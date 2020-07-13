Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.PlottingServices

Public Class AutoCADPlotHelper

    Private Shared Function GetLayoutDetails(layoutName As String, doc As Document) As Layout

        Dim db As Database = doc.Database

        Using tr As Transaction = doc.TransactionManager.StartTransaction
            Dim dLayouts As DBDictionary = tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead)
            If dLayouts.Contains(layoutName) Then

                Dim id As ObjectId = dLayouts.GetAt(layoutName)

                Return tr.GetObject(id, OpenMode.ForRead)
            End If
        End Using

        Return Nothing
    End Function

    Public Shared Function GetLayout(layoutName As String, database As Database) As Layout
        Using tr As Transaction = database.TransactionManager.StartTransaction
            Dim dLayouts As DBDictionary = tr.GetObject(database.LayoutDictionaryId, OpenMode.ForRead)
            If dLayouts.Contains(layoutName) Then
                Dim id As ObjectId = dLayouts.GetAt(layoutName)

                Return tr.GetObject(id, OpenMode.ForRead)
            End If
        End Using

        Return Nothing
    End Function

    Private Shared Function validatePlot(pSet As PlotSettings) As PlotSettings
        Dim psv As PlotSettingsValidator = PlotSettingsValidator.Current
        Dim plotDevList As New List(Of String)
        For Each str As String In psv.GetPlotDeviceList
            plotDevList.Add(str)
        Next

        Dim pdIndex As Integer = plotDevList.FindIndex(Function(pdv)
                                                           Return pdv.ToUpperInvariant = pSet.PlotConfigurationName
                                                       End Function)

        ''are the layout plot settigns valid?
        Dim layoutIsValid As Boolean = False
        If Not pdIndex = -1 Then
            ''the plot device does exist so lets check the paper size does
            ''set teh psv so that we can update the media settings

            Using ps As New PlotSettings(True)
                psv.SetPlotConfigurationName(ps, plotDevList(pdIndex), Nothing)
                psv.RefreshLists(ps)

                ''now get the media names
                Dim mediaList As New List(Of String)
                For Each _str As String In psv.GetCanonicalMediaNameList(ps)
                    mediaList.Add(_str)
                Next

                Dim mlIndex As Integer = mediaList.FindIndex(Function(ml)
                                                                 Return ml.ToUpperInvariant = pSet.CanonicalMediaName
                                                             End Function)

                ''if we get here then the plotsettings are good 
                Return pSet
            End Using
        End If

        Dim acLayoutIsValid As Boolean = False
        Using ps As New PlotSettings(True)
            psv.SetPlotConfigurationName(ps, "DWG to PDF.pc3", Nothing)
            psv.RefreshLists(ps)

            Dim mediaList As New List(Of String)
            For Each str As String In psv.GetCanonicalMediaNameList(ps)
                'mediaList.Add(str.Replace("_", " "))
                mediaList.Add(str)
            Next

            Dim paperSz As Autodesk.AutoCAD.Geometry.Point2d = pSet.PlotPaperSize
            Dim szX As Integer = Math.Abs(paperSz.X)
            Dim szY As Integer = Math.Abs(paperSz.Y)
            '(?:ISO).+(250.00 x 176.00)
            Dim rgEx As New Text.RegularExpressions.Regex("(?:ISO).+(" + szX.ToString + ".00 x " + szY.ToString + ".00)")
            Dim paperList = mediaList.Where(Function(mSz)
                                                Return rgEx.IsMatch(mSz.Replace("_", " "))
                                            End Function).ToList

            If paperList Is Nothing OrElse paperList.Count = 0 Then
                rgEx = New Text.RegularExpressions.Regex("(" + szX.ToString + "|" + szY.ToString + ")")
                paperList = mediaList.Where(Function(mSz)
                                                Return rgEx.IsMatch(mSz.Replace("_", " "))
                                            End Function).ToList
            End If
            paperList.Sort()

            If paperList Is Nothing OrElse paperList.Count <= 0 Then
                Throw New System.Exception("Invalid plot settings Unable to find paper to fit media name '" + pSet.CanonicalMediaName + "' size (" + paperSz.X.ToString + "," + paperSz.Y.ToString + ")")
            End If

            psv.SetPlotConfigurationName(pSet, "DWG to PDF.pc3", paperList(0))
            Return pSet

        End Using

    End Function

    Public Shared Function PlotPDF(dbfilename As String, pdfFilename As String) As Boolean
        Dim _ret As Boolean = False

        Dim acDocMgr As DocumentCollection = Application.DocumentManager
        Dim doc As Document = DocumentCollectionExtension.Open(acDocMgr, dbfilename)
        Dim db As Database = doc.Database
        Dim obj As Integer

        Try
            Using doc.LockDocument

                Using tr As Transaction = db.TransactionManager.StartTransaction
                    obj = Application.GetSystemVariable("BACKGROUNDPLOT")
                    Autodesk.AutoCAD.ApplicationServices.Core.Application.SetSystemVariable("BACKGROUNDPLOT", 0)


                    Dim lm As LayoutManager = LayoutManager.Current
                    Dim layoutID As ObjectId = lm.GetLayoutId(lm.CurrentLayout)
                    Dim layout As Layout = tr.GetObject(layoutID, OpenMode.ForRead)

                    Dim pi As New PlotInfo
                    pi.Layout = layout.ObjectId

                    Dim ps As PlotSettings = New PlotSettings(layout.ModelType)
                    ps.CopyFrom(layout)

                    ''check our plot settings
                    ps = validatePlot(ps)
                    ''
                    Dim psv As PlotSettingsValidator = PlotSettingsValidator.Current
                    psv.SetPlotType(ps, Autodesk.AutoCAD.DatabaseServices.PlotType.Extents)
                    psv.SetUseStandardScale(ps, True)
                    psv.SetStdScaleType(ps, StdScaleType.ScaleToFit)
                    psv.SetPlotCentered(ps, True)

                    pi.OverrideSettings = ps ''wont be saved to the layout but will be used
                    Dim piv As PlotInfoValidator = New PlotInfoValidator()
                    piv.MediaMatchingPolicy = MatchingPolicy.MatchEnabled
                    piv.Validate(pi)


                    Dim pe As PlotEngine = PlotFactory.CreatePublishEngine
                    Using pe
                        pe.BeginPlot(Nothing, Nothing)
                        pe.BeginDocument(pi, IO.Path.GetFileName(dbfilename), Nothing, 1, True, pdfFilename)

                        Dim ppi As New PlotPageInfo
                        pe.BeginPage(ppi, pi, True, Nothing)
                        pe.BeginGenerateGraphics(Nothing)
                        pe.EndGenerateGraphics(Nothing)
                        pe.EndPage(Nothing)
                        pe.EndDocument(Nothing)
                        pe.EndPlot(Nothing)
                    End Using

                    tr.Commit()

                    'Autodesk.AutoCAD.ApplicationServices.Core.Application.SetSystemVariable("BACKGROUNDPLOT", obj)
                End Using
            End Using

            _ret = True
        Catch ex As Exception
            LoggingHelper.WriteToLog("Error plotting to PDF")
            LoggingHelper.WriteToLog(ex.ToString)

            Throw
        Finally
            Autodesk.AutoCAD.ApplicationServices.Core.Application.SetSystemVariable("BACKGROUNDPLOT", obj)
            DocumentExtension.CloseAndDiscard(doc)
            doc.Dispose()
            _ret = False
        End Try

        Return _ret
    End Function

    Public Shared Function _PlotPDF(LayoutName As String) As PreviewEndPlotStatus
        Dim _ret As PreviewEndPlotStatus

        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = doc.Editor
        Dim db As Database = doc.Database
        Dim _layout As Layout = GetLayoutDetails(LayoutName, doc)

        Using tr As Transaction = db.TransactionManager.StartTransaction
            Using doc.LockDocument
                Dim pi As PlotInfo = New PlotInfo()
                pi.Layout = _layout.ObjectId

                Dim ps As PlotSettings = New PlotSettings(_layout.ModelType)
                ps.CopyFrom(_layout)
                Dim psv As PlotSettingsValidator = PlotSettingsValidator.Current
                psv.SetPlotType(ps, Autodesk.AutoCAD.DatabaseServices.PlotType.Layout)
                psv.SetUseStandardScale(ps, True)
                psv.SetStdScaleType(ps, StdScaleType.ScaleToFit)
                'psv.SetPlotCentered(ps, True)
                psv.SetPlotConfigurationName(ps, "DWG to PDF.pc3", _layout.CanonicalMediaName)
                pi.OverrideSettings = ps
                Dim piv As PlotInfoValidator = New PlotInfoValidator()
                piv.MediaMatchingPolicy = MatchingPolicy.MatchEnabled
                piv.Validate(pi)

                Dim pe As PlotEngine = PlotFactory.CreatePublishEngine
                Dim ppd As PlotProgressDialog = New PlotProgressDialog(False, 1, True)

                Using ppd
                    'ppd.set_PlotMsgString(PlotMessageIndex.DialogTitle, "Custom Preview Progress")
                    'ppd.set_PlotMsgString(PlotMessageIndex.SheetName, doc.Name.Substring(doc.Name.LastIndexOf("\") + 1))
                    'ppd.set_PlotMsgString(PlotMessageIndex.CancelJobButtonMessage, "Cancel Job")
                    'ppd.set_PlotMsgString(PlotMessageIndex.CancelSheetButtonMessage, "Cancel Sheet")
                    'ppd.set_PlotMsgString(PlotMessageIndex.SheetSetProgressCaption, "Sheet Set Progress")
                    'ppd.set_PlotMsgString(PlotMessageIndex.SheetProgressCaption, "Sheet Progress")

                    'ppd.LowerPlotProgressRange = 0
                    'ppd.UpperPlotProgressRange = 100
                    'ppd.PlotProgressPos = 0
                    ppd.OnBeginPlot()
                    ppd.IsVisible = False
                    pe.BeginPlot(ppd, Nothing)


                    pe.BeginDocument(pi, doc.Name, Nothing, 1, True, "c:\temp\foo.pdf")
                    ppd.OnBeginSheet()
                    ppd.LowerSheetProgressRange = 0
                    ppd.UpperSheetProgressRange = 100
                    ppd.SheetProgressPos = 0
                    Dim ppi As PlotPageInfo = New PlotPageInfo()
                    pe.BeginPage(ppi, pi, True, Nothing)
                    pe.BeginGenerateGraphics(Nothing)
                    ppd.SheetProgressPos = 50
                    pe.EndGenerateGraphics(Nothing)
                    Dim pepi As PreviewEndPlotInfo = New PreviewEndPlotInfo()
                    pe.EndPage(pepi)
                    _ret = pepi.Status
                    ppd.SheetProgressPos = 100
                    ppd.OnEndSheet()
                    pe.EndDocument(Nothing)
                    ppd.PlotProgressPos = 100
                    ppd.OnEndPlot()
                    pe.EndPlot(Nothing)
                End Using

                tr.Commit()
            End Using


        End Using

        Return _ret
    End Function

    Public Shared Function GetPLOTDeviceList() As List(Of String)
        Dim psv As PlotSettingsValidator = PlotSettingsValidator.Current
        Dim devList As Specialized.StringCollection = psv.GetPlotDeviceList

        Dim ret As New List(Of String)
        ret.AddRange(devList)
        Return ret
    End Function

    Public Shared Sub printDWG(ByVal dwg As String)
        Dim acDocMgr As DocumentCollection = Application.DocumentManager
        Dim doc As Document = DocumentCollectionExtension.Open(acDocMgr, dwg)
        'Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Application.SetSystemVariable("BACKGROUNDPLOT", 0)
        Using doc.LockDocument
            Using db As Database = New Database(False, True)
                db.ReadDwgFile(dwg, IO.FileShare.ReadWrite, False, Nothing)

                HostApplicationServices.WorkingDatabase = db

                Using tr As Transaction = db.TransactionManager.StartTransaction()
                    Dim btr As BlockTableRecord = CType(tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead), BlockTableRecord)
                    Dim lo As Layout = CType(tr.GetObject(btr.LayoutId, OpenMode.ForRead), Layout)
                    Dim pi As PlotInfo = New PlotInfo()
                    pi.Layout = btr.LayoutId
                    Dim ps As PlotSettings = New PlotSettings(lo.ModelType)
                    ps.CopyFrom(lo)
                    Dim psv As PlotSettingsValidator = PlotSettingsValidator.Current
                    psv.SetPlotType(ps, Autodesk.AutoCAD.DatabaseServices.PlotType.Layout)
                    'psv.SetPlotWindowArea(ps, New Extents2d(New Point2d(0.01625, 0.01625), New Point2d(41.9375, 29.9375)))

                    psv.SetUseStandardScale(ps, True)
                    psv.SetStdScaleType(ps, StdScaleType.StdScale1To1)
                    'psv.SetPlotCentered(ps, True)
                    psv.SetPlotConfigurationName(ps, "DWG To PDF.pc3", "ARCH_E1_(30.00_x_42.00_Inches)")
                    'psv.SetPlotPaperUnits(ps, PlotPaperUnit.Inches)
                    'psv.SetPlotRotation(ps, PlotRotation.Degrees090)
                    psv.RefreshLists(ps)
                    'psv.SetCurrentStyleSheet(ps, "Stellar FullSize Mono.ctb")
                    pi.OverrideSettings = ps
                    Dim piv As PlotInfoValidator = New PlotInfoValidator()
                    piv.MediaMatchingPolicy = MatchingPolicy.MatchEnabled
                    piv.Validate(pi)

                    If PlotFactory.ProcessPlotState = ProcessPlotState.NotPlotting Then
                        Dim pe As PlotEngine = PlotFactory.CreatePublishEngine()

                        Using pe
                            Dim ppd As PlotProgressDialog = New PlotProgressDialog(False, 1, True)

                            Using ppd
                                ppd.PlotMsgString(PlotMessageIndex.DialogTitle) = "Custom Plot Progress"
                                ppd.PlotMsgString(PlotMessageIndex.CancelJobButtonMessage) = "Cancel Job"
                                ppd.PlotMsgString(PlotMessageIndex.CancelSheetButtonMessage) = "Cancel Sheet"
                                ppd.PlotMsgString(PlotMessageIndex.SheetSetProgressCaption) = "Sheet Set Progress"
                                ppd.PlotMsgString(PlotMessageIndex.SheetProgressCaption) = "Sheet Progress"
                                ppd.LowerPlotProgressRange = 0
                                ppd.UpperPlotProgressRange = 100
                                ppd.PlotProgressPos = 0
                                ppd.OnBeginPlot()
                                ppd.IsVisible = True
                                pe.BeginPlot(ppd, Nothing)
                                pe.BeginDocument(pi, dwg, Nothing, 1, True, "c:\temp\foo.pdf")
                                ppd.OnBeginSheet()
                                ppd.LowerSheetProgressRange = 0
                                ppd.UpperSheetProgressRange = 100
                                ppd.SheetProgressPos = 0
                                Dim ppi As PlotPageInfo = New PlotPageInfo()
                                pe.BeginPage(ppi, pi, True, Nothing)
                                pe.BeginGenerateGraphics(Nothing)
                                pe.EndGenerateGraphics(Nothing)
                                pe.EndPage(Nothing)
                                ppd.SheetProgressPos = 100
                                ppd.OnEndSheet()
                                pe.EndDocument(Nothing)
                                ppd.PlotProgressPos = 100
                                ppd.OnEndPlot()
                                pe.EndPlot(Nothing)
                            End Using
                        End Using
                    Else
                        MsgBox("error")
                    End If
                End Using
            End Using
        End Using


    End Sub

End Class
