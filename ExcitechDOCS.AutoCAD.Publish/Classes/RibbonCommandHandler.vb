Imports System.Reflection
Imports System.Windows.Forms
Imports System.Windows.Input
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Windows
Imports Application = Autodesk.AutoCAD.ApplicationServices.Application

Public Class RibbonCommandHandler
    Implements System.Windows.Input.ICommand

#Region " Events "
    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
#End Region

#Region " Methods "
    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        If TypeOf parameter Is RibbonButton Then
            Dim btn As RibbonButton = TryCast(parameter, RibbonButton)
            Dim _method As String = btn.Id

            ''call the method if we have it
            Dim _objType As Type = Me.[GetType]()
            Dim _objMethod As MethodInfo = _objType.GetMethod(_method, BindingFlags.Instance Or BindingFlags.CreateInstance Or BindingFlags.NonPublic Or BindingFlags.IgnoreCase)
            _objMethod.Invoke(Me, Nothing)

        End If

    End Sub

    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        Return True
    End Function
#End Region

#Region " Ribbon Buttons "

    <CommandMethod("DOCSPUBLISH", CommandFlags.Session + CommandFlags.Modal)>
    Private Sub btnPublishLayouts()
        Try

            ''need to pass through the settings from AutoCAD Addin

            'ViewHelper.Selections.Clear()
            Dim vault As MFilesAPI.Vault = VaultStatus.initialiseMFiles
            Dim status = VaultStatus.Status

            'check connection status
            If status <> VaultStatus.VaultStatuses.connected Then
                g_clientApplication.ShowBalloonTip(VaultStatus.ShortStatusDescription, "Excitech DOCS AutoCAD Addin")

                'Update Ribbon State
                ''TODO:: raise a request to update the ribbon event??

                ''TODO:: return failed
            End If

            ''TODO:: check that the autocad document has been saved if it needs to

            Dim acDocument As Document = Core.Application.DocumentManager.MdiActiveDocument

            Dim frmExportSheetUI As New frmPublishLayouts(vault, acDocument)
            frmExportSheetUI.ShowDialog()

            'check the value of DBMOD, if 0 then the drawing has not been changed
            Dim isModified As Short = Application.GetSystemVariable("DBMOD")
            If isModified > 0 Then
                Dim acDoc = Application.DocumentManager.MdiActiveDocument
                Dim dwgFilename As String = acDoc.Database.OriginalFileName
                System.Windows.Forms.MessageBox.Show("Changes have been made to the file:" + vbCrLf + """" + IO.Path.GetFileName(dwgFilename) + """" + vbCrLf +
                                                        vbCrLf + "Please save this document, if this document isnt saved you will lose these changes and potentially result in published layouts being orphaned", "Excitech DOCS", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation, System.Windows.Forms.MessageBoxDefaultButton.Button2)
            End If


            'If revitDocument.IsModified Then
            '    If MessageBox.Show("The Revit Model must be saved before Publishing Sheets." + vbCrLf + vbCrLf +
            '                       "Do you want to Save and Publish?", Resources.EDM_Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then Return Result.Succeeded
            '    revitDocument.Save()
            'End If

        Catch ex As Exception
            MessageBox.Show("ERROR: " + ex.Message, "Excitech DOCS AutoCAD Addin", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try


    End Sub

    Private Sub btnFOO()
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database
        Dim ed As Editor = doc.Editor

        Dim options As New PromptEntityOptions("Select Polyline: ")
        options.SetRejectMessage("Must be a polyline")
        options.AddAllowedClass(GetType(Polyline), True)
        Dim result = ed.GetEntity(options)
        If Not result.Status = PromptStatus.OK Then Exit Sub

        Using tr As Transaction = db.TransactionManager.StartTransaction
            Dim pline As Polyline = tr.GetObject(result.ObjectId, OpenMode.ForRead)
            Dim selection As PromptSelectionResult = ed.SelectByPolyline(pline, PolygonSelectionMode.Window)

            If selection.Status = PromptStatus.OK Then
                Dim objIDArray As ObjectId() = selection.Value.GetObjectIds
                Dim objIDs As New ObjectIdCollection

                For Each id As ObjectId In objIDArray
                    objIDs.Add(id)
                Next

                'Using doc.LockDocument
                '    Using newDB As Database = New Database(False, True)
                '        db.Wblock(newDB, objIDs, Autodesk.AutoCAD.Geometry.Point3d.Origin, DuplicateRecordCloning.Ignore)
                '        newDB.SaveAs("c:\temp\wblock.dwg", DwgVersion.Newest)
                '    End Using
                'End Using

                Dim destDB As New Database(True, True)
                Using doc.LockDocument

                    Dim destDbMsId As ObjectId = SymbolUtilityServices.GetBlockModelSpaceId(destDB)
                    Dim mapping As IdMapping = New IdMapping()
                    db.WblockCloneObjects(objIDs, destDbMsId, mapping, DuplicateRecordCloning.Replace, False)
                    LayoutHelper.AddLayouttoDatabase("C:\Users\Graham Haigh\Documents\DWG Files\Metsec\A1_Template.dwt", "A1", destDB, "A1")
                    destDB.SaveAs("c:\temp\CopyTest.dwg", DwgVersion.Current)
                End Using



                '''add the layout
                '' Create a new database object and open the drawing into memory
                'Dim acExDb As Database = New Database(False, True)
                'acExDb.ReadDwgFile("C:\Users\Graham Haigh\Documents\DWG Files\Metsec\A1_Template.dwt", FileOpenMode.OpenForReadAndAllShare, True, "")

                'Dim layout As New Layout
                'Dim layoutname As String = "A1"
                'Dim lytmgr As LayoutManager
                'Dim layoutid As ObjectId

                'Dim workingDB = HostApplicationServices.WorkingDatabase
                'Using tr1 As Transaction = destDB.TransactionManager.StartTransaction
                '    HostApplicationServices.WorkingDatabase = destDB

                '    ''create a layout in dest
                '    Dim newLytMgr As LayoutManager = LayoutManager.Current()
                '    Dim newLayoutId As ObjectId = newLytMgr.CreateLayout("newLayout")
                '    Dim newLayout As Layout = tr1.GetObject(newLayoutId, OpenMode.ForWrite)

                '    ''this buggers up the layout
                '    'delete all other layouts
                '    'Dim msID As ObjectId = newLytMgr.GetLayoutId("Model")
                '    'Dim layoutDCT As DBDictionary = tr1.GetObject(destDB.LayoutDictionaryId, OpenMode.ForWrite)
                '    'For Each de As DBDictionaryEntry In layoutDCT
                '    '    Dim lName As String = de.Key
                '    '    If lName = "Model" OrElse lName = "newLayout" Then Continue For
                '    '    newLytMgr.CurrentLayout = "Model"
                '    '    newLytMgr.DeleteLayout(lName)
                '    '    newLytMgr.CurrentLayout = "Model"
                '    'Next

                '    HostApplicationServices.WorkingDatabase = acExDb

                '    Using tr2 As Transaction = acExDb.TransactionManager.StartTransaction
                '        ' Get the dictionary of the original database
                '        Dim lytDict As DBDictionary = tr2.GetObject(acExDb.LayoutDictionaryId, OpenMode.ForRead)
                '        lytmgr = LayoutManager.Current()
                '        layoutid = lytmgr.GetLayoutId(layoutname)
                '        layout = tr2.GetObject(layoutid, OpenMode.ForRead)
                '        newLayout.CopyFrom(layout)

                '        'Get the block table record of the existing layout
                '        Dim blkTableRec As BlockTableRecord
                '        blkTableRec = tr2.GetObject(layout.BlockTableRecordId, OpenMode.ForRead)

                '        'Get the object ids of the objects in the existing block table record
                '        Dim objIdCol As New ObjectIdCollection()
                '        For Each objId As ObjectId In blkTableRec
                '            objIdCol.Add(objId)
                '        Next

                '        ' Clone the objects to the new layout
                '        Dim idMap As IdMapping = New IdMapping()
                '        acExDb.WblockCloneObjects(objIdCol, newLayout.BlockTableRecordId, idMap, DuplicateRecordCloning.Ignore, False)

                '        tr2.Commit()
                '    End Using


                '    tr1.Commit()
                '    destDB.SaveAs("c:\temp\CopyTest.dwg", DwgVersion.Current)
                'End Using

                ''


                ''LayoutHelper.AddLayouttoDatabase("C:\Users\Graham Haigh\Documents\DWG Files\Metsec\A1_Template.dwt", "A1", destDB, "A1")
                'HostApplicationServices.WorkingDatabase = workingDB
                '''


            End If

            tr.Commit()
        End Using
    End Sub


    Private Sub btnBAR()
        Dim filename As String = "C:\Users\Graham Haigh\Documents\DWG Files\Metsec\A1_Template.dwt"

        ''copy the select bits in to it
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database
        Dim ed As Editor = doc.Editor
        Dim destDB As New Database(False, True)

        Dim options As New PromptEntityOptions("Select Polyline: ")
        options.SetRejectMessage("Must be a polyline")
        options.AddAllowedClass(GetType(Polyline), True)
        Dim result = ed.GetEntity(options)
        If Not result.Status = PromptStatus.OK Then Exit Sub

        Dim pline As Polyline
        Using tr As Transaction = db.TransactionManager.StartTransaction
            pline = tr.GetObject(result.ObjectId, OpenMode.ForRead)
            Dim selection As PromptSelectionResult = ed.SelectByPolyline(pline, PolygonSelectionMode.Window)

            If selection.Status = PromptStatus.OK Then
                Dim objIDArray As ObjectId() = selection.Value.GetObjectIds
                Dim objIDs As New ObjectIdCollection

                For Each id As ObjectId In objIDArray
                    objIDs.Add(id)
                Next

                'Dim destDB As New Database(False, True)
                destDB.ReadDwgFile(filename, FileOpenMode.OpenForReadAndAllShare, True, "") ''this loads the template for us
                Using doc.LockDocument

                    Dim destDbMsId As ObjectId = SymbolUtilityServices.GetBlockModelSpaceId(destDB)
                    Dim mapping As IdMapping = New IdMapping()
                    db.WblockCloneObjects(objIDs, destDbMsId, mapping, DuplicateRecordCloning.Replace, False) ''copy all teh objects we need

                    'destDB.SaveAs("c:\temp\CopyTest.dwg", DwgVersion.Current)
                End Using

            End If
            tr.Commit()
        End Using

        Dim workingDB = HostApplicationServices.WorkingDatabase
        HostApplicationServices.WorkingDatabase = destDB
        Using tr As Transaction = destDB.TransactionManager.StartTransaction
            Dim layoutMgr As LayoutManager = LayoutManager.Current

            Dim layoutID As ObjectId = layoutMgr.GetLayoutId("A1")
            Dim lay As Layout = tr.GetObject(layoutID, OpenMode.ForWrite)

            Dim vps As ObjectIdCollection = lay.GetViewports
            Dim vp As Viewport
            For Each vpID As ObjectId In vps
                Dim vp2 As Viewport = tr.GetObject(vpID, OpenMode.ForWrite)
                If vp2.Layer = "ExDOCSViewPort" Then vp = vp2 : Exit For
            Next

            Dim ext3d As Extents3d = pline.GeometricExtents
            Dim ext2d As New Extents2d(ext3d.MinPoint.X, ext3d.MinPoint.Y, ext3d.MaxPoint.X, ext3d.MaxPoint.Y)
            Dim ctrPt = (Autodesk.AutoCAD.Geometry.Point2d.Origin + (ext2d.MaxPoint - ext2d.MinPoint) * 0.5)
            vp.ViewTarget = New Autodesk.AutoCAD.Geometry.Point3d(ctrPt.X, ctrPt.Y, 0)

            Dim asp = vp.Width * vp.Height
            'Dim mExtens As New Extents3d(x_min, x_max)
            Dim mExtens As Extents3d = pline.GeometricExtents
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

        HostApplicationServices.WorkingDatabase = workingDB
        destDB.SaveAs("c:\temp\CopyTest.dwg", DwgVersion.Current)

    End Sub
#End Region


End Class
