
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports ExcitechDOCS.AutoCAD.Publish.ViewArea

Public Class PublishLayoutDelegate

#Region " Private "
    Private _layouts As List(Of ViewArea)
    Private _layout As ViewArea
    Private _templateFilename As String
#End Region

#Region " Constrcutor "
    Public Sub New(Layouts As List(Of ViewArea))

        _layouts = Layouts
        _templateFilename = ""
    End Sub

    Public Sub PublishLayout(Layout As ViewArea, Abort As Boolean)

        Layout.SetStatus("Processing", StatusIcons.Processing)

        If Abort Then
            Layout.SetStatus("Aborted", StatusIcons.Warning)
        Else
            'Dim publishHelper As New PublishHelper(_layouts)
            'publishHelper.publish
            _layout = Layout

            Publish()
        End If
    End Sub
#End Region

#Region " Methods "
    Private Sub Publish()
        Try
            ''clear temp folder
            ClearTempfolder()

            ''generate the export files
            ProcessExports()

        Catch ex As Exception
            LoggingHelper.WriteToLog(ex.ToString)
            Throw
        End Try
    End Sub

    Private Sub ClearTempfolder()
        Dim dInfo As New IO.DirectoryInfo(Settings.TempFilePath)

        For Each _file As IO.FileInfo In dInfo.GetFiles
            _file.IsReadOnly = False
            Try
                _file.Delete()
            Catch ex As Exception

            End Try

        Next
    End Sub

    Private Function ReplaceInvalidChars(filename As String)
        Return String.Join(" ", filename.Split(IO.Path.GetInvalidFileNameChars))
    End Function


    Private Function GetTemplate() As String
        If _layout.DrawingTemplate.Location = DrawingTemplate.TemplateLocation.MFILES Then
            Dim tempName As String = ReplaceInvalidChars(_layout.DrawingTemplate.TemplateName + ".dwt")
            tempName = IO.Path.Combine(Settings.TempFilePath, tempName)


            MFilesHelper.DownloadTemplateFile(_layout.DrawingTemplate, tempName)
            _layout.DrawingTemplate.FullPath = tempName
        End If

        Return _layout.DrawingTemplate.FullPath
    End Function

    Private Function FindLayersInModelspace() As List(Of String)
        ''find all the layers we have that match the layer prefix in the current model space
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database
        'Dim ed As Editor = doc.Editor
        Dim ret As New List(Of String)

        'Using tr As Transaction = db.TransactionManager.StartTransaction()
        '    ''get the layout dictionary
        '    Dim layoutDCT As DBDictionary = tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead)
        '    For Each de As DBDictionaryEntry In layoutDCT
        '        If Not de.Key.ToUpper.StartsWith(My.Settings.LayerPrefix) Then ret.Add(de.Key)
        '    Next
        'End Using

        'Return ret

        Using tr As Transaction = db.TransactionManager.StartTransaction()
            ''get the layout dictionary
            Dim lt As LayerTable = tr.GetObject(db.LayerTableId, OpenMode.ForRead)
            For Each lID As ObjectId In lt
                Dim ltr As LayerTableRecord = tr.GetObject(lID, OpenMode.ForRead)
                If ltr.Name.ToUpperInvariant.StartsWith(My.Settings.LayerPrefix.ToUpperInvariant) Then ret.Add(ltr.Name)
            Next
        End Using

        Return ret
    End Function

    Private Sub ProcessExports()
        Dim workingDB As Database = Nothing

        Try
            _templateFilename = GetTemplate()
            ''get the layouts from the current drawing
            Dim currLayouts As List(Of String) = FindLayersInModelspace()
            Dim tmpLayouts As List(Of String) = AutoCADHelper.GetLayersWithPrefixFromFile(_templateFilename)
            Dim foundLayouts As New List(Of String)

            ''compare both lists we only want items that are both in this drawing
            ''and are in the template as we are going to copy the geometry
            For Each item As String In currLayouts
                If Not tmpLayouts.FindIndex(Function(tmpitem) tmpitem.ToUpperInvariant = item.ToUpperInvariant) = -1 Then
                    ''layout is in both so keep its name
                    foundLayouts.Add(item)
                End If
            Next

            ''if we have no matching layers then raise an error with this item
            If foundLayouts.Count <= 0 Then
                LoggingHelper.WriteToLog("No matching layers found in model and template export will be skipped...")
                LoggingHelper.WriteToLog("Layout: " & _layout.LayoutName + " Template: " & _layout.Template)
                _layout.SetStatus("No matching layers found in model and template", StatusIcons.Failure)
                Exit Sub
            End If

            ''copy the geometry in to the new drawing
            LoggingHelper.WriteToLog("Creating Drawing...")

            Dim tempFile As String = ""
            tempFile = ReplaceInvalidChars(_layout.DocumentName) + ".dwg"
            tempFile = IO.Path.Combine(Settings.TempFilePath, tempFile)
            _layout.SetStatus("Creating new drawing", ViewArea.StatusIcons.Processing)
            LoggingHelper.WriteToLog("Tempfile: " + tempFile)

            Dim doc As Document = Application.DocumentManager.MdiActiveDocument
            Dim db As Database = doc.Database
            Dim ed As Editor = doc.Editor

            'Dim filename As String = GetTemplate()
            LoggingHelper.WriteToLog("Template Path: " + _templateFilename)

            ''create our new database
            LoggingHelper.WriteToLog("Creating new drawing from template...")
            Dim destDB As New Database(False, True)
            destDB.ReadDwgFile(_templateFilename, FileOpenMode.OpenForReadAndAllShare, True, "")
            destDB.CloseInput(True)


            For Each layerName As String In foundLayouts
                Dim srcObjID As ObjectId = Nothing
                If layerName.ToUpperInvariant.EndsWith(My.Settings.ModelLayer.ToLowerInvariant) Then
                    srcObjID = _layout.ObjectID
                Else
                    ''look for the non model layer
                    Dim thisVA As ViewArea = ViewHelper.Selections.FirstOrDefault(Function(va) va.LayerName.ToUpperInvariant = layerName.ToUpperInvariant)
                    If thisVA Is Nothing Then

                    Else
                        srcObjID = thisVA.ObjectID
                    End If
                End If

                ''do geometry copy here
                Try
                    LoggingHelper.WriteToLog("Finding objects in selected area...")
                    Dim pLine As Polyline
                    Using tr As Transaction = db.TransactionManager.StartTransaction
                        pLine = tr.GetObject(srcObjID, OpenMode.ForRead) ''get the polyline that indicates the boundary

                        Dim selection As PromptSelectionResult = ed.SelectByPolyline(pLine, PolygonSelectionMode.Window) ''get everything that crosses the boundary 

                        If selection.Status = PromptStatus.OK Then
                            Dim objIDArray As ObjectId() = selection.Value.GetObjectIds
                            Dim objIDs As New ObjectIdCollection

                            ''create a list of all the objects that we need to copy
                            For Each id As ObjectId In objIDArray
                                objIDs.Add(id)
                            Next

                            LoggingHelper.WriteToLog("Cloning objects in to new drawing...")
                            Using doc.LockDocument
                                Dim destDbMsId As ObjectId = SymbolUtilityServices.GetBlockModelSpaceId(destDB)
                                Dim mapping As IdMapping = New IdMapping()
                                db.WblockCloneObjects(objIDs, destDbMsId, mapping, DuplicateRecordCloning.Replace, False)
                            End Using

                        End If

                        tr.Commit()
                    End Using
                    ''
                Catch ex As Exception
                    Debugger.Launch()
                    Debugger.Break()
                    Throw
                End Try
            Next

            LoggingHelper.WriteToLog("Creating view in viewport...")
            _layout.SetStatus("Creating view in viewport", ViewArea.StatusIcons.Processing)
            ''now we have a document with a layout that matches out tempalte and models space that only has this particular selection
            ''next is to set the viewport
            workingDB = HostApplicationServices.WorkingDatabase ''store the working db as we need to change it
            HostApplicationServices.WorkingDatabase = destDB
            AutoCADHelper.DeleteAllLayouts(New List(Of String)({"Model", _layout.LayoutName}))            ''clear all but the layout we need

            Using tr As Transaction = destDB.TransactionManager.StartTransaction
                Dim layoutMgr As LayoutManager = LayoutManager.Current

                ''we need to do this for each layer
                For Each layerName As String In foundLayouts

                    Dim layoutID As ObjectId = layoutMgr.GetLayoutId(_layout.LayoutName) ''("A1") ''
                    Dim lay As Layout = tr.GetObject(layoutID, OpenMode.ForWrite)

                    Dim vps As ObjectIdCollection = lay.GetViewports
                    Dim vp As Viewport = Nothing
                    For Each vpID As ObjectId In vps
                        Dim vp2 As Viewport = tr.GetObject(vpID, OpenMode.ForWrite)
                        If vp2.Layer = layerName Then vp = vp2 : Exit For
                    Next

                    If vp Is Nothing Then Throw New Exception("Unable to find viewport on layer: " + layerName)

                    'get our pLine object extents
                    'Dim srcObjID As ObjectId = Nothing
                    Dim pLineExtents As Extents3d
                    If layerName.ToUpperInvariant.EndsWith(My.Settings.ModelLayer.ToLowerInvariant) Then
                        'srcObjID = _layout.ObjectID
                        pLineExtents = _layout.Extents
                    Else
                        ''look for the non model layer
                        Dim thisVA As ViewArea = ViewHelper.Selections.FirstOrDefault(Function(va) va.LayerName.ToUpperInvariant = layerName.ToUpperInvariant)
                        If thisVA Is Nothing Then

                        Else
                            ' srcObjID = thisVA.ObjectID
                            pLineExtents = thisVA.Extents
                        End If
                    End If

                    'Dim pLine As Polyline = tr.GetObject(srcObjID, OpenMode.ForRead)

                    ''calculate our viewport and try to make the geometry fit in to the viewport as best as possible
                    Dim ext3d As Extents3d = pLineExtents 'pLine.GeometricExtents
                    Dim ext2d As New Extents2d(ext3d.MinPoint.X, ext3d.MinPoint.Y, ext3d.MaxPoint.X, ext3d.MaxPoint.Y)
                    Dim ctrPt = (Autodesk.AutoCAD.Geometry.Point2d.Origin + (ext2d.MaxPoint - ext2d.MinPoint) * 0.5)
                    vp.ViewTarget = New Autodesk.AutoCAD.Geometry.Point3d(ctrPt.X, ctrPt.Y, 0)

                    Dim asp = vp.Width * vp.Height
                    Dim mExtens As Extents3d = pLineExtents 'pLine.GeometricExtents
                    Dim matWCS2DCS As Autodesk.AutoCAD.Geometry.Matrix3d
                    matWCS2DCS = Autodesk.AutoCAD.Geometry.Matrix3d.PlaneToWorld(vp.ViewDirection)
                    matWCS2DCS = Autodesk.AutoCAD.Geometry.Matrix3d.Displacement(vp.ViewTarget - Autodesk.AutoCAD.Geometry.Point3d.Origin) * matWCS2DCS
                    matWCS2DCS = Autodesk.AutoCAD.Geometry.Matrix3d.Rotation(-vp.TwistAngle, vp.ViewDirection, vp.ViewTarget) * matWCS2DCS
                    matWCS2DCS = matWCS2DCS.Inverse
                    mExtens.TransformBy(matWCS2DCS)

                    Dim wid = mExtens.MaxPoint.X - mExtens.MinPoint.X
                    Dim hgt = mExtens.MaxPoint.Y - mExtens.MinPoint.Y
                    Dim mctrPt As New Autodesk.AutoCAD.Geometry.Point2d((mExtens.MaxPoint.X + mExtens.MinPoint.X) * 0.5, (mExtens.MaxPoint.Y + mExtens.MinPoint.Y) * 0.5)
                    If wid > (hgt * asp) Then hgt = wid * asp

                    vp.ViewHeight = hgt * 1.01
                    vp.ViewCenter = mctrPt

                    ''hide the view layer
                    Dim idLay As ObjectId = destDB.LayerTableId
                    Dim lt As LayerTable = tr.GetObject(idLay, OpenMode.ForRead)
                    Dim idLayTblRcd As ObjectId
                    If lt.Has(layerName) Then
                        idLayTblRcd = lt.Item(layerName)
                        Dim layIdCol As New ObjectIdCollection
                        layIdCol.Add(idLayTblRcd)

                        If Not vp.IsLayerFrozenInViewport(idLayTblRcd) Then
                            vp.FreezeLayersInViewport(layIdCol.GetEnumerator)
                        End If
                    End If

                    vp.On = True
                    vp.UpdateDisplay()
                Next
                tr.Commit()
            End Using

            LoggingHelper.WriteToLog("Saving temp drawing...")
            _layout.SetStatus("Saving temp drawing", ViewArea.StatusIcons.Processing)
            HostApplicationServices.WorkingDatabase = workingDB
            destDB.SaveAs(tempFile, DwgVersion.Current)

            If Not destDB Is Nothing Then destDB.Dispose()
            _layout.ExportDWG.Filepath = tempFile

            If _layout.ExportPDF.Export Then
                LoggingHelper.WriteToLog("Creating PDF...")
                _layout.SetStatus("Creating PDF", ViewArea.StatusIcons.Processing)
                Dim pdftempFile = IO.Path.ChangeExtension(tempFile, ".pdf")
                AutoCADPlotHelper.PlotPDF(tempFile, pdftempFile)
                _layout.ExportPDF.Filepath = pdftempFile
            End If

            If _layout.IsInMFiles Then
                LoggingHelper.WriteToLog("Layout is in MFiles, Document will be up versioned...")
                _layout.SetStatus("Publishing new document version", ViewArea.StatusIcons.Processing)
                Dim _files As New List(Of String)
                If _layout.ExportDWG.Export Then _files.Add(_layout.ExportDWG.Filepath)
                If _layout.ExportPDF.Export Then _files.Add(_layout.ExportPDF.Filepath)

                MFilesHelper.AddNewObjectVersion(_files, _layout.ObjectVersion, _layout) ''returnreturn the object version
                _layout.SetStatus("Publishing Successful", ViewArea.StatusIcons.Success)
                LoggingHelper.WriteToLog("Publishing Successful...")
            Else
                LoggingHelper.WriteToLog("Layout not in MFiles, a New document will be created...")
                _layout.SetStatus("Publishing new document", ViewArea.StatusIcons.Processing)
                Dim _files As New List(Of String)
                If _layout.ExportDWG.Export Then _files.Add(_layout.ExportDWG.Filepath)
                If _layout.ExportPDF.Export Then _files.Add(_layout.ExportPDF.Filepath)

                MFilesHelper.AddNewObject(PluginSettings.DefaultClass, _files, _layout.LayoutUniqueID, _layout) ''returnreturn the object version
                _layout.SetStatus("Publishing Successful", ViewArea.StatusIcons.Success)
                LoggingHelper.WriteToLog("Publishing Successful...")
            End If

        Catch ex As Exception
            _layout.SetStatus("Publishing Failed", ViewArea.StatusIcons.Failure)
            _layout.ErrorMessage = ex.Message
            Throw
        Finally
            If Not workingDB Is Nothing Then
                If Not HostApplicationServices.WorkingDatabase = workingDB Then
                    HostApplicationServices.WorkingDatabase = workingDB
                End If
            End If
        End Try

        MFilesHelper.FindExistingObject(_layout)

    End Sub



    Private Sub _ProcessExports()

        LoggingHelper.WriteToLog("Creating Drawing...")

        Dim tempFile As String = ""
        tempFile = ReplaceInvalidChars(_layout.DocumentName) + ".dwg"
        'tempFile = IO.Path.GetFileNameWithoutExtension(IO.Path.GetTempFileName) + ".dwg"
        tempFile = IO.Path.Combine(Settings.TempFilePath, tempFile)
        ''if export pdf then 
        _layout.SetStatus("Creating new drawing", ViewArea.StatusIcons.Processing)
        LoggingHelper.WriteToLog("Tempfile: " + tempFile)

        'Dim filename As String = "C:\Users\Graham Haigh\Documents\DWG Files\Metsec\A1_Template.dwt"
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim workingDB
        Dim filename As String = GetTemplate() ''_layout.DrawingTemplate.FullPath
        LoggingHelper.WriteToLog("Template Path: " + filename)

        Try
            ''find the objects we need to copy in the polyline
            Dim db As Database = doc.Database
            Dim ed As Editor = doc.Editor
            Dim destDB As New Database(False, True)

            LoggingHelper.WriteToLog("Finding objects in selected area from modelspace...")
            Dim pLine As Polyline
            Using tr As Transaction = db.TransactionManager.StartTransaction
                pLine = tr.GetObject(_layout.ObjectID, OpenMode.ForRead) ''get the polyline that indicates teh boundary of the selection
                Dim selection As PromptSelectionResult = ed.SelectByPolyline(pLine, PolygonSelectionMode.Window) ''get everything that crosses the boundary of the polyline

                If selection.Status = PromptStatus.OK Then
                    Dim objIDArray As ObjectId() = selection.Value.GetObjectIds
                    Dim objIDs As New ObjectIdCollection

                    ''create a list of all the objects that we need to copy
                    For Each id As ObjectId In objIDArray
                        objIDs.Add(id)
                    Next

                    ''create a new document from our tempalte
                    ''and copy all the objects from the selection in to the model space
                    LoggingHelper.WriteToLog("Creating new drawing from template...")
                    destDB.ReadDwgFile(filename, FileOpenMode.OpenForReadAndAllShare, True, "")
                    destDB.CloseInput(True)
                    LoggingHelper.WriteToLog("Cloning objects in to new drawing...")
                    Using doc.LockDocument
                        Dim destDbMsId As ObjectId = SymbolUtilityServices.GetBlockModelSpaceId(destDB)
                        Dim mapping As IdMapping = New IdMapping()
                        db.WblockCloneObjects(objIDs, destDbMsId, mapping, DuplicateRecordCloning.Replace, False)
                    End Using

                End If

                tr.Commit()
            End Using

            LoggingHelper.WriteToLog("Creating view in viewport...")
            _layout.SetStatus("Creating view in viewport", ViewArea.StatusIcons.Processing)
            ''now we have a document with a layout that matches out tempalte and models space that only has this particular selection
            ''next is to set the viewport
            workingDB = HostApplicationServices.WorkingDatabase ''store the working db as we need to change it
            HostApplicationServices.WorkingDatabase = destDB
            AutoCADHelper.DeleteAllLayouts(New List(Of String)({"Model", _layout.LayoutName}))            ''clear all but the layout we need

            Using tr As Transaction = destDB.TransactionManager.StartTransaction
                Dim layoutMgr As LayoutManager = LayoutManager.Current
                'Dim thisLayout As String = layoutMgr.CurrentLayout

                '''get the first non model layout
                'Dim dLayouts As DBDictionary = tr.GetObject(destDB.LayoutDictionaryId, OpenMode.ForRead)
                'For Each entry As DBDictionaryEntry In dLayouts
                '    If Not entry.Key = "Model" Then thisLayout = entry.Key
                'Next

                Dim layoutID As ObjectId = layoutMgr.GetLayoutId(_layout.LayoutName) ''("A1") ''
                Dim lay As Layout = tr.GetObject(layoutID, OpenMode.ForWrite)

                Dim vps As ObjectIdCollection = lay.GetViewports
                Dim vp As Viewport = Nothing
                For Each vpID As ObjectId In vps
                    Dim vp2 As Viewport = tr.GetObject(vpID, OpenMode.ForWrite)
                    If vp2.Layer = "ExDOCSViewPort" Then vp = vp2 : Exit For
                Next

                If vp Is Nothing Then Throw New Exception("Unable to find viewport on layer: ExDOCSViewPort")

                Dim ext3d As Extents3d = pLine.GeometricExtents
                Dim ext2d As New Extents2d(ext3d.MinPoint.X, ext3d.MinPoint.Y, ext3d.MaxPoint.X, ext3d.MaxPoint.Y)
                Dim ctrPt = (Autodesk.AutoCAD.Geometry.Point2d.Origin + (ext2d.MaxPoint - ext2d.MinPoint) * 0.5)
                vp.ViewTarget = New Autodesk.AutoCAD.Geometry.Point3d(ctrPt.X, ctrPt.Y, 0)

                Dim asp = vp.Width * vp.Height
                'Dim mExtens As New Extents3d(x_min, x_max)
                Dim mExtens As Extents3d = pLine.GeometricExtents
                Dim matWCS2DCS As Autodesk.AutoCAD.Geometry.Matrix3d
                matWCS2DCS = Autodesk.AutoCAD.Geometry.Matrix3d.PlaneToWorld(vp.ViewDirection)
                matWCS2DCS = Autodesk.AutoCAD.Geometry.Matrix3d.Displacement(vp.ViewTarget - Autodesk.AutoCAD.Geometry.Point3d.Origin) * matWCS2DCS
                matWCS2DCS = Autodesk.AutoCAD.Geometry.Matrix3d.Rotation(-vp.TwistAngle, vp.ViewDirection, vp.ViewTarget) * matWCS2DCS
                matWCS2DCS = matWCS2DCS.Inverse
                mExtens.TransformBy(matWCS2DCS)

                Dim wid = mExtens.MaxPoint.X - mExtens.MinPoint.X
                Dim hgt = mExtens.MaxPoint.Y - mExtens.MinPoint.Y
                Dim mctrPt As New Autodesk.AutoCAD.Geometry.Point2d((mExtens.MaxPoint.X + mExtens.MinPoint.X) * 0.5, (mExtens.MaxPoint.Y + mExtens.MinPoint.Y) * 0.5)
                If wid > (hgt * asp) Then hgt = wid * asp

                vp.ViewHeight = hgt * 1.01
                vp.ViewCenter = mctrPt

                ''hide the view layer
                Dim idLay As ObjectId = destDB.LayerTableId
                Dim lt As LayerTable = tr.GetObject(idLay, OpenMode.ForRead)
                Dim idLayTblRcd As ObjectId
                If lt.Has("EXDOCSViews") Then idLayTblRcd = lt.Item("EXDOCSViews")

                Dim layIdCol As New ObjectIdCollection
                layIdCol.Add(idLayTblRcd)

                If Not vp.IsLayerFrozenInViewport(idLayTblRcd) Then
                    vp.FreezeLayersInViewport(layIdCol.GetEnumerator)
                End If

                vp.On = True
                vp.UpdateDisplay()

                tr.Commit()
            End Using

            ''process any other views/layers here



            LoggingHelper.WriteToLog("Saving temp drawing...")
            _layout.SetStatus("Saving temp drawing", ViewArea.StatusIcons.Processing)
            HostApplicationServices.WorkingDatabase = workingDB
            destDB.SaveAs(tempFile, DwgVersion.Current)
            'doc = Application.DocumentManager.GetDocument(destDB)
            'DocumentExtension.CloseAndSave(doc, tempFile)

            If Not destDB Is Nothing Then destDB.Dispose()

            _layout.ExportDWG.Filepath = tempFile

            If _layout.ExportPDF.Export Then
                LoggingHelper.WriteToLog("Creating PDF...")
                _layout.SetStatus("Creating PDF", ViewArea.StatusIcons.Processing)
                Dim pdftempFile = IO.Path.ChangeExtension(tempFile, ".pdf")
                AutoCADPlotHelper.PlotPDF(tempFile, pdftempFile)
                _layout.ExportPDF.Filepath = pdftempFile
            End If

            If _layout.IsInMFiles Then
                LoggingHelper.WriteToLog("Layout is in MFiles, Document will be up versioned...")
                _layout.SetStatus("Publishing new document version", ViewArea.StatusIcons.Processing)
                Dim _files As New List(Of String)
                If _layout.ExportDWG.Export Then _files.Add(_layout.ExportDWG.Filepath)
                If _layout.ExportPDF.Export Then _files.Add(_layout.ExportPDF.Filepath)

                MFilesHelper.AddNewObjectVersion(_files, _layout.ObjectVersion, _layout) ''returnreturn the object version
                _layout.SetStatus("Publishing Successful", ViewArea.StatusIcons.Success)
                LoggingHelper.WriteToLog("Publishing Successful...")
            Else
                LoggingHelper.WriteToLog("Layout not in MFiles, a New document will be created...")
                _layout.SetStatus("Publishing new document", ViewArea.StatusIcons.Processing)
                Dim _files As New List(Of String)
                If _layout.ExportDWG.Export Then _files.Add(_layout.ExportDWG.Filepath)
                If _layout.ExportPDF.Export Then _files.Add(_layout.ExportPDF.Filepath)

                MFilesHelper.AddNewObject(PluginSettings.DefaultClass, _files, _layout.LayoutUniqueID, _layout) ''returnreturn the object version
                _layout.SetStatus("Publishing Successful", ViewArea.StatusIcons.Success)
                LoggingHelper.WriteToLog("Publishing Successful...")
            End If

        Catch ex As Exception
            _layout.SetStatus("Publishing Failed", ViewArea.StatusIcons.Failure)
            _layout.ErrorMessage = ex.Message
            Throw
        Finally
            If Not workingDB Is Nothing Then
                If Not HostApplicationServices.WorkingDatabase = workingDB Then
                    HostApplicationServices.WorkingDatabase = workingDB
                End If
            End If


        End Try

        MFilesHelper.FindExistingObject(_layout)
    End Sub

#End Region

End Class
