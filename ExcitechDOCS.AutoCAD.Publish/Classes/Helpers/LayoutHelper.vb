Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry

Public Class LayoutHelper

    ''this is how the autocad help file does it which seems the most 'complete'
    Public Shared Function AddLayouttoDatabase(filename As String, layoutname As String, Optional newLayoutName As String = "") As Boolean
        If String.IsNullOrWhiteSpace(newLayoutName) Then newLayoutName = layoutname

        Dim _ret As Boolean = False
        ' Get the current document and database

        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database

        Using acDoc.LockDocument
            ' Create a new database object and open the drawing into memory
            Dim acExDb As Database = New Database(False, True)
            acExDb.ReadDwgFile(filename, FileOpenMode.OpenForReadAndAllShare, True, "")

            ' Create a transaction for the external drawing
            Using acTransEx As Transaction = acExDb.TransactionManager.StartTransaction()

                ' Get the layouts dictionary
                Dim layoutsEx As DBDictionary = acTransEx.GetObject(acExDb.LayoutDictionaryId, OpenMode.ForRead)

                ' Check to see if the layout exists in the external drawing
                If layoutsEx.Contains(layoutname) = True Then

                    ' Get the layout and block objects from the external drawing
                    Dim layEx As Layout = layoutsEx.GetAt(layoutname).GetObject(OpenMode.ForRead)
                    Dim blkBlkRecEx As BlockTableRecord = acTransEx.GetObject(layEx.BlockTableRecordId, OpenMode.ForRead)

                    ' Get the objects from the block associated with the layout
                    Dim idCol As ObjectIdCollection = New ObjectIdCollection()
                    For Each id As ObjectId In blkBlkRecEx
                        idCol.Add(id)
                    Next

                    ' Create a transaction for the current drawing
                    Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

                        ' Get the block table and create a new block
                        ' then copy the objects between drawings
                        Dim blkTbl As BlockTable = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite)

                        Using blkBlkRec As New BlockTableRecord
                            blkBlkRec.Name = "*Paper_Space" & CStr(layoutsEx.Count() - 1)
                            blkTbl.Add(blkBlkRec)
                            acTrans.AddNewlyCreatedDBObject(blkBlkRec, True)
                            acExDb.WblockCloneObjects(idCol, blkBlkRec.ObjectId, New IdMapping(), DuplicateRecordCloning.Ignore, False)

                            ' Create a new layout and then copy properties between drawings
                            Dim layouts As DBDictionary = acTrans.GetObject(acCurDb.LayoutDictionaryId, OpenMode.ForWrite)

                            Using lay As New Layout
                                lay.LayoutName = newLayoutName
                                lay.AddToLayoutDictionary(acCurDb, blkBlkRec.ObjectId)
                                acTrans.AddNewlyCreatedDBObject(lay, True)
                                lay.CopyFrom(layEx)

                                Dim plSets As DBDictionary = acTrans.GetObject(acCurDb.PlotSettingsDictionaryId, OpenMode.ForRead)

                                ' Check to see if a named page setup was assigned to the layout,
                                ' if so then copy the page setup settings
                                If lay.PlotSettingsName <> "" Then

                                    ' Check to see if the page setup exists
                                    If plSets.Contains(lay.PlotSettingsName) = False Then
                                        acTrans.GetObject(acCurDb.PlotSettingsDictionaryId, OpenMode.ForWrite)

                                        Using plSet As New PlotSettings(lay.ModelType)
                                            plSet.PlotSettingsName = lay.PlotSettingsName
                                            plSet.AddToPlotSettingsDictionary(acCurDb)
                                            acTrans.AddNewlyCreatedDBObject(plSet, True)

                                            Dim plSetsEx As DBDictionary = acTransEx.GetObject(acExDb.PlotSettingsDictionaryId, OpenMode.ForRead)

                                            Dim plSetEx As PlotSettings = plSetsEx.GetAt(lay.PlotSettingsName).GetObject(OpenMode.ForRead)

                                            plSet.CopyFrom(plSetEx)
                                        End Using
                                    End If
                                End If
                            End Using
                        End Using

                        ' Regen the drawing to get the layout tab to display
                        acDoc.Editor.Regen()

                        ' Save the changes made
                        acTrans.Commit()
                        _ret = True
                    End Using
                Else
                    ' Display a message if the layout could not be found in the specified drawing
                    acDoc.Editor.WriteMessage(vbLf & "Layout '" & layoutname & "' could not be imported from '" & filename & "'.")
                    _ret = False
                End If

                ' Discard the changes made to the external drawing file
                acTransEx.Abort()
            End Using

            ' Close the external drawing file
            acExDb.Dispose()
        End Using



        Return _ret
    End Function

    Public Shared Function AddLayouttoDatabase(filename As String, layoutname As String, acCurDb As Database, Optional newLayoutName As String = "") As Boolean
        If String.IsNullOrWhiteSpace(newLayoutName) Then newLayoutName = layoutname

        Dim _ret As Boolean = False
        Dim workingDB = HostApplicationServices.WorkingDatabase
        ' Get the current document and database

        'Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        'Dim acCurDb As Database = acDoc.Database

        'Using acDoc.LockDocument
        ' Create a new database object and open the drawing into memory
        Dim acExDb As Database = New Database(False, True)
            acExDb.ReadDwgFile(filename, FileOpenMode.OpenForReadAndAllShare, True, "")

        HostApplicationServices.WorkingDatabase = acExDb
        ' Create a transaction for the external drawing
        Using acTransEx As Transaction = acExDb.TransactionManager.StartTransaction()

                ' Get the layouts dictionary
                Dim layoutsEx As DBDictionary = acTransEx.GetObject(acExDb.LayoutDictionaryId, OpenMode.ForRead)

                ' Check to see if the layout exists in the external drawing
                If layoutsEx.Contains(layoutname) = True Then

                    ' Get the layout and block objects from the external drawing
                    Dim layEx As Layout = layoutsEx.GetAt(layoutname).GetObject(OpenMode.ForRead)
                    Dim blkBlkRecEx As BlockTableRecord = acTransEx.GetObject(layEx.BlockTableRecordId, OpenMode.ForRead)

                    ' Get the objects from the block associated with the layout
                    Dim idCol As ObjectIdCollection = New ObjectIdCollection()
                    For Each id As ObjectId In blkBlkRecEx
                        idCol.Add(id)
                    Next

                'HostApplicationServices.WorkingDatabase = acCurDb
                ' Create a transaction for the current drawing
                Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

                        ' Get the block table and create a new block
                        ' then copy the objects between drawings
                        Dim blkTbl As BlockTable = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite)

                        Using blkBlkRec As New BlockTableRecord
                            blkBlkRec.Name = "*Paper_Space" & CStr(layoutsEx.Count() - 1)
                            blkTbl.Add(blkBlkRec)
                            acTrans.AddNewlyCreatedDBObject(blkBlkRec, True)
                            acExDb.WblockCloneObjects(idCol, blkBlkRec.ObjectId, New IdMapping(), DuplicateRecordCloning.Ignore, False)

                            ' Create a new layout and then copy properties between drawings
                            Dim layouts As DBDictionary = acTrans.GetObject(acCurDb.LayoutDictionaryId, OpenMode.ForWrite)

                        HostApplicationServices.WorkingDatabase = acCurDb
                        Using lay As New Layout
                                lay.LayoutName = newLayoutName
                                lay.AddToLayoutDictionary(acCurDb, blkBlkRec.ObjectId)
                                acTrans.AddNewlyCreatedDBObject(lay, True)
                                lay.CopyFrom(layEx)

                                Dim plSets As DBDictionary = acTrans.GetObject(acCurDb.PlotSettingsDictionaryId, OpenMode.ForRead)

                                ' Check to see if a named page setup was assigned to the layout,
                                ' if so then copy the page setup settings
                                If lay.PlotSettingsName <> "" Then

                                    ' Check to see if the page setup exists
                                    If plSets.Contains(lay.PlotSettingsName) = False Then
                                        acTrans.GetObject(acCurDb.PlotSettingsDictionaryId, OpenMode.ForWrite)

                                        Using plSet As New PlotSettings(lay.ModelType)
                                            plSet.PlotSettingsName = lay.PlotSettingsName
                                            plSet.AddToPlotSettingsDictionary(acCurDb)
                                            acTrans.AddNewlyCreatedDBObject(plSet, True)

                                            Dim plSetsEx As DBDictionary = acTransEx.GetObject(acExDb.PlotSettingsDictionaryId, OpenMode.ForRead)

                                            Dim plSetEx As PlotSettings = plSetsEx.GetAt(lay.PlotSettingsName).GetObject(OpenMode.ForRead)

                                            plSet.CopyFrom(plSetEx)
                                        End Using
                                    End If
                                End If
                            End Using
                        End Using

                        ' Regen the drawing to get the layout tab to display
                        'acDoc.Editor.Regen()

                        ' Save the changes made
                        acTrans.Commit()
                        _ret = True
                    End Using
                'Else
                '' Display a message if the layout could not be found in the specified drawing
                '' acDoc.Editor.WriteMessage(vbLf & "Layout '" & layoutname & "' could not be imported from '" & filename & "'.")
                '_ret = False
            End If

                ' Discard the changes made to the external drawing file
                acTransEx.Abort()
            End Using

            ' Close the external drawing file
            acExDb.Dispose()
        'End Using

        HostApplicationServices.WorkingDatabase = workingDB

        Return _ret
    End Function

    Public Shared Function SetViewinLayout(ViewArea As ViewArea, LayoutName As String) As Boolean
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database

        Using tran As Transaction = db.TransactionManager.StartTransaction
            Using docLock As DocumentLock = doc.LockDocument
                Dim layoutMgr As LayoutManager = LayoutManager.Current
                layoutMgr.CurrentLayout = LayoutName

                'Application.SetSystemVariable("TILEMODE", 0)
                'doc.Editor.SwitchToPaperSpace()

                Dim layoutID As ObjectId = layoutMgr.GetLayoutId(LayoutName)

                ''opent his layout object for writing
                Dim lay As Layout = tran.GetObject(layoutID, OpenMode.ForWrite)

                ''get the viewports
                'Dim vps As ObjectIdCollection = db.GetViewports(True)
                Dim vps As ObjectIdCollection = lay.GetViewports
                Dim vp As Viewport
                For Each vpID As ObjectId In vps
                    Dim vp2 As Viewport = tran.GetObject(vpID, OpenMode.ForWrite)
                    ''If vp2.Number = 2 Then vp = vp2 : Exit For
                    If vp2.Layer = "ExDOCSViewPort" Then vp = vp2 : Exit For
                    'If vp2.Layer = "EXDOCSViews" Then vp = vp2 : Exit For
                    'EXDOCSViews
                Next

                '                Dim ext3D As New Extents3d(x_min, x_max)
                '                Dim ext2d As New Extents2d(x_min.X, x_min.Y, x_max.X, x_max.Y)
                Dim ext2d As Extents2d = ViewArea.Extents2D

                ''set target
                Dim ctrPt = (Point2d.Origin + (ext2d.MaxPoint - ext2d.MinPoint) * 0.5)
                vp.ViewTarget = New Point3d(ctrPt.X, ctrPt.Y, 0)

                'fit contents with matrix
                Dim asp = vp.Width * vp.Height
                'Dim mExtens As New Extents3d(x_min, x_max)
                Dim mExtens As Extents3d = ViewArea.Extents
                Dim matWCS2DCS As Matrix3d
                matWCS2DCS = Matrix3d.PlaneToWorld(vp.ViewDirection)
                matWCS2DCS = Matrix3d.Displacement(vp.ViewTarget - Point3d.Origin) * matWCS2DCS
                matWCS2DCS = Matrix3d.Rotation(-vp.TwistAngle, vp.ViewDirection, vp.ViewTarget) * matWCS2DCS
                matWCS2DCS = matWCS2DCS.Inverse
                mExtens.TransformBy(matWCS2DCS)

                Dim wid = mExtens.MaxPoint.X - mExtens.MinPoint.X
                Dim hgt = mExtens.MaxPoint.Y - mExtens.MinPoint.Y
                Dim mctrPt As New Point2d((mExtens.MaxPoint.X + mExtens.MinPoint.X) * 0.5, (mExtens.MaxPoint.Y + mExtens.MinPoint.Y) * 0.5)
                If wid > (hgt * asp) Then hgt = wid * asp

                vp.ViewHeight = hgt * 1.01
                vp.ViewCenter = mctrPt

                ''hide the view layer
                Dim idLay As ObjectId = db.LayerTableId
                Dim lt As LayerTable = tran.GetObject(idLay, OpenMode.ForRead)
                Dim idLayTblRcd As ObjectId
                If lt.Has("EXDOCSViews") Then idLayTblRcd = lt.Item("EXDOCSViews")

                Dim layIdCol As New ObjectIdCollection
                layIdCol.Add(idLayTblRcd)

                If Not vp.IsLayerFrozenInViewport(idLayTblRcd) Then
                    vp.FreezeLayersInViewport(layIdCol.GetEnumerator)
                End If

                vp.On = True
                vp.UpdateDisplay()

                'doc.Editor.SwitchToModelSpace()
            End Using


            tran.Commit()

        End Using
    End Function


End Class
