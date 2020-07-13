Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.CompilerServices
Imports System.Windows.Media.Imaging
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Geometry
Imports MFilesAPI

Public Enum PolygonSelectionMode
    Crossing
    Window
End Enum

Module modMain
    Public g_clientApplication As New MFilesClientApplication

    Public PluginSettings As Settings

    Public Function GetBitmapImage(SourceImage As Bitmap) As BitmapImage

        Dim stream As IO.MemoryStream = New IO.MemoryStream()
        SourceImage.Save(stream, ImageFormat.Png)
        Dim bmp As BitmapImage = New BitmapImage()

        bmp.BeginInit()
        bmp.StreamSource = stream
        bmp.EndInit()

        Return bmp

    End Function


    <Extension()>
    Function SelectByPolyline(ByVal ed As Editor, ByVal pline As Polyline, ByVal mode As PolygonSelectionMode) As PromptSelectionResult
        If ed Is Nothing Then Throw New ArgumentNullException("ed")
        If pline Is Nothing Then Throw New ArgumentNullException("pline")
        Dim wcs As Matrix3d = ed.CurrentUserCoordinateSystem.Inverse()
        Dim polygon As Point3dCollection = New Point3dCollection()

        For i As Integer = 0 To pline.NumberOfVertices - 1
            polygon.Add(pline.GetPoint3dAt(i).TransformBy(wcs))
        Next

        Dim result As PromptSelectionResult

        Using curView As ViewTableRecord = ed.GetCurrentView()
            ed.Zoom(pline.GeometricExtents)

            If mode = PolygonSelectionMode.Crossing Then
                result = ed.SelectCrossingPolygon(polygon)
            Else
                result = ed.SelectCrossingPolygon(polygon)
            End If

            ed.SetCurrentView(curView)
        End Using

        Return result
    End Function

    <Extension()>
    Sub Zoom(ByVal ed As Editor, ByVal extents As Extents3d)
        If ed Is Nothing Then Throw New ArgumentNullException("ed")

        Using view As ViewTableRecord = ed.GetCurrentView()
            Dim worldToEye As Matrix3d = (Matrix3d.Rotation(-view.ViewTwist, view.ViewDirection, view.Target) * Matrix3d.Displacement(view.Target - Point3d.Origin) * Matrix3d.PlaneToWorld(view.ViewDirection)).Inverse()
            extents.TransformBy(worldToEye)
            view.Width = extents.MaxPoint.X - extents.MinPoint.X
            view.Height = extents.MaxPoint.Y - extents.MinPoint.Y
            view.CenterPoint = New Point2d((extents.MaxPoint.X + extents.MinPoint.X) / 2.0, (extents.MaxPoint.Y + extents.MinPoint.Y) / 2.0)
            ed.SetCurrentView(view)
        End Using
    End Sub
End Module
