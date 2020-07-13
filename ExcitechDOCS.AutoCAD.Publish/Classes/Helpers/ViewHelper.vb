Imports System.Drawing
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.GraphicsSystem
Imports MFilesAPI

Public Class ViewHelper

#Region " Private "
    Private Shared _listSelections As New List(Of ViewArea)
#End Region

#Region " Public "
    Public Shared Property Selections As List(Of ViewArea)
        Get
            Return _listSelections
        End Get
        Set(value As List(Of ViewArea))
            _listSelections = value
        End Set
    End Property

    'Public Shared Property Settings As LayoutCreationSettings

    Public Shared ReadOnly Property SettingsFile As String
        Get
            Dim _settingsFile As String = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
            _settingsFile = IO.Path.Combine(_settingsFile, "Excitech DOCS\AutoCAD\Settings\settings.txt")
            Return _settingsFile
        End Get
    End Property

    Public MFilesObjectVersion As ObjectVersion

    Public Shared ReadOnly Property CanPublish As Boolean
        Get
            Dim validItemCount As Integer = 0

            For Each viewArea In _listSelections
                If viewArea.Disabled Then Continue For
                If viewArea.ExportDWG.Export Or viewArea.ExportPDF.Export Then
                    validItemCount += 1

                End If
            Next

            Return validItemCount > 0
        End Get
    End Property
#End Region

#Region " Methods "

    Public Shared Function GetThumbNail(extents As Extents3d) As System.Drawing.Image
        Dim device As Device
        Dim view As Autodesk.AutoCAD.GraphicsSystem.View
        Dim model As Model

        Dim doc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database
        Dim gsm As Manager = doc.GraphicsManager

        'LayoutManager.Current.CurrentLayout = "Model"
        ' Create a device that will draw into our panel

        Dim gsmKernelDesc As New Autodesk.AutoCAD.GraphicsSystem.KernelDescriptor()
        gsmKernelDesc.addRequirement(Autodesk.AutoCAD.UniqueString.Intern("3D Drawing")) ''what kernel do we want
        Dim gsmKernel As GraphicsKernel = Autodesk.AutoCAD.GraphicsSystem.Manager.AcquireGraphicsKernel(gsmKernelDesc) ''get the graphics kernel
        '  device = gsm.CreateAutoCADDevice(gsmKernel, ViewPanel.Handle)
        device = gsm.CreateAutoCADOffScreenDevice(gsmKernel)
        device.DeviceRenderType = RendererType.Default
        ' Create a view and add it to the device
        device.OnSize(New Drawing.Size(800, 600))

        view = New Autodesk.AutoCAD.GraphicsSystem.View
        device.Add(view)
        device.Update()

        Using tr As Transaction = db.TransactionManager.StartTransaction
            model = gsm.CreateAutoCADModel(gsmKernel)
            Dim bt As BlockTable = tr.GetObject(db.BlockTableId, OpenMode.ForRead)
            Dim ms As BlockTableRecord = tr.GetObject(bt(BlockTableRecord.ModelSpace), OpenMode.ForRead)

            view.Add(ms, model)
            view.ZoomExtents(extents.MinPoint, extents.MaxPoint)

            ' Just to make sure that not perspective but parallel
            ' mode is used
            ''can we fit the contents?
            Dim mExtens As New Extents3d(extents.MinPoint, extents.MaxPoint)
            Dim wid = mExtens.MaxPoint.X - mExtens.MinPoint.X
            Dim hgt = mExtens.MaxPoint.Y - mExtens.MinPoint.Y

            view.SetView(view.Position, view.Target, view.UpVector, wid, hgt, Projection.Parallel)

            ''hide this layer
            Dim idLay As ObjectId = db.LayerTableId
            Dim lt As LayerTable = tr.GetObject(idLay, OpenMode.ForRead)
            Dim idLayTblRcd As ObjectId
            Dim ModelLayerName As String = My.Settings.LayerPrefix + My.Settings.ModelLayer
            If lt.Has(ModelLayerName) Then idLayTblRcd = lt.Item(ModelLayerName)

            view.FreezeLayer(idLayTblRcd.OldIdPtr)
            view.Invalidate()
            view.Update()

            Dim snap = view.GetSnapshot(New Rectangle(0, 0, 800, 600))

            If Not view Is Nothing Then
                device.EraseAll()
                view.Dispose()
                view = Nothing
            End If

            If Not device Is Nothing Then
                device.Dispose()
                device = Nothing
            End If

            Return snap
        End Using
    End Function

    Public Shared Sub FindSelections()
        LoggingHelper.WriteToLog("Looking for selections...")

        Application.SetSystemVariable("TILEMODE", 1)
        ''scan the modelspace record the selections
        Dim docED As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
        _listSelections = New List(Of ViewArea)
        ''build a selectio filter
        'Dim ModelLayerName As String = My.Settings.LayerPrefix + My.Settings.ModelLayer

        ''get a list of all the layers that start with our prefix
        Dim searchLayers As List(Of String) = AutoCADHelper.GetLayersWithPrefix(Application.DocumentManager.MdiActiveDocument.Database)
        Dim selectLayers As String = String.Join(",", searchLayers)

        Dim queryArr(1) As Autodesk.AutoCAD.DatabaseServices.TypedValue
        queryArr.SetValue(New Autodesk.AutoCAD.DatabaseServices.TypedValue(DxfCode.LayerName, selectLayers), 0)
        queryArr.SetValue(New Autodesk.AutoCAD.DatabaseServices.TypedValue(DxfCode.Start, "LWPOLYLINE"), 1)

        ''assign the query to a filter
        Dim selFilter As New SelectionFilter(queryArr)

        ''request objects fromto be selecselected in the drawingarea
        Dim selPrompt As PromptSelectionResult = docED.SelectAll(selFilter)

        If selPrompt.Status = PromptStatus.OK Then
            Using tran As Transaction = docED.Document.Database.TransactionManager.StartTransaction()
                Dim selSet As SelectionSet = selPrompt.Value

                ''list the selections
                Dim i As Integer = 0
                For Each item As SelectedObject In selSet
                    If Not IsDBNull(item) Then
                        Dim ent As Entity = tran.GetObject(item.ObjectId, OpenMode.ForRead)
                        Dim va As New ViewArea With {
                            .ExportDWG = New exportFiles With {.Export = False},
                            .ExportPDF = New exportFiles With {.Export = False},
                        .DocumentName = "Layout " & _listSelections.Count + 1,
                        .Extents = ent.GeometricExtents,
                        .ObjectID = item.ObjectId,
                        .LayerName = ent.Layer
                        }

                        If Not PluginSettings.DefaultTemplate Is Nothing Then
                            va.Template = PluginSettings.DefaultTemplate.TemplateName
                            va.DrawingTemplate = PluginSettings.DefaultTemplate
                            va.LayoutName = va.DrawingTemplate.LayoutName
                        End If

                        If va.LayerIsModel Then va.Thumbnail = GetThumbNail(va.Extents)

                        _listSelections.Add(va)

                    End If
                Next

            End Using
        End If

        LoggingHelper.WriteToLog("Found: " + _listSelections.Count.ToString + " selections")
    End Sub

    Public Shared Function SetViewinLayout(ViewArea As ViewArea, LayoutName As String) As Boolean
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database
        Dim ModelLayerName As String = My.Settings.LayerPrefix + My.Settings.ModelLayer

        Using tran As Transaction = db.TransactionManager.StartTransaction
            Using docLock As DocumentLock = doc.LockDocument
                Dim layoutMgr As LayoutManager = LayoutManager.Current
                layoutMgr.CurrentLayout = LayoutName

                Dim layoutID As ObjectId = layoutMgr.GetLayoutId(LayoutName)

                ''opent his layout object for writing
                Dim lay As Layout = tran.GetObject(layoutID, OpenMode.ForWrite)

                ''get the viewports

                Dim vps As ObjectIdCollection = lay.GetViewports
                Dim vp As Viewport
                For Each vpID As ObjectId In vps
                    Dim vp2 As Viewport = tran.GetObject(vpID, OpenMode.ForWrite)
                    If vp2.Layer = ModelLayerName Then vp = vp2 : Exit For
                Next

                Dim ext2d As Extents2d = ViewArea.Extents2D

                ''set target
                Dim ctrPt = (Point2d.Origin + (ext2d.MaxPoint - ext2d.MinPoint) * 0.5)
                vp.ViewTarget = New Point3d(ctrPt.X, ctrPt.Y, 0)

                'fit contents with matrix
                Dim asp = vp.Width * vp.Height
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
                If lt.Has(ModelLayerName) Then
                    idLayTblRcd = lt.Item(ModelLayerName)
                    Dim layIdCol As New ObjectIdCollection
                    layIdCol.Add(idLayTblRcd)

                    If Not vp.IsLayerFrozenInViewport(idLayTblRcd) Then
                        vp.FreezeLayersInViewport(layIdCol.GetEnumerator)
                    End If
                End If



                vp.On = True
                vp.UpdateDisplay()

                'doc.Editor.SwitchToModelSpace()
            End Using


            tran.Commit()

        End Using
    End Function

    Public Shared Async Sub _UpdateVaultStatus()
        LoggingHelper.WriteToLog("Checking for existing MFiles Document...")

        For i As Integer = 0 To _listSelections.Count - 1
            Dim va As ViewArea = _listSelections(i)

            If String.IsNullOrWhiteSpace(va.LayoutUniqueID) Then
                va.IsInMFiles = False
                _listSelections(i) = va
                Continue For
            End If

            Await Task.Run(Sub()
                               va = MFilesHelper.FindExistingObject(va)
                               _listSelections(i) = va
                           End Sub)
        Next
    End Sub

    Public Shared Sub UpdateVaultStatus()
        LoggingHelper.WriteToLog("Checking for existing MFiles Document...")

        For i As Integer = 0 To _listSelections.Count - 1
            Dim va As ViewArea = _listSelections(i)

            If String.IsNullOrWhiteSpace(va.LayoutUniqueID) Then
                va.IsInMFiles = False
                _listSelections(i) = va
                Continue For
            End If

            Dim tVa = MFilesHelper.FindExistingObject(va)

            If Not tVa Is Nothing Then
                _listSelections(i) = tVa
            End If


        Next

    End Sub

    Private Shared Function HasDuplicate(layoutUniqueId As String) As Boolean
        If String.IsNullOrWhiteSpace(layoutUniqueId) Then Return False

        Dim cnt As Integer = _listSelections.Where(Function(va)
                                                       Return va.LayoutUniqueID = layoutUniqueId
                                                   End Function).Count

        Return cnt > 0
    End Function

    Public Shared Sub UpdateDuplicateStatus()
        'For Each _key As Guid In _listSelections.Keys.ToList
        '    Dim va As ViewArea = _listSelections(_key)
        '    va.LayoutISduplicate = HasDuplicate(va.LayoutUniqueID)
        'Next
    End Sub
#End Region
End Class
