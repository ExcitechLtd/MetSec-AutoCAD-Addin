Imports Autodesk.AutoCAD.DatabaseServices
Imports MFilesAPI

Public Class ViewArea

#Region " Private "
    Private _thumbnail As System.Drawing.Image
    Private _smlthumbnail As System.Drawing.Image
    Private _lgethumbnail As System.Drawing.Image
    Private _objectID As ObjectId
    Private _version As String = ""
    Private _objectVersion As ObjectVersion
    Private _customProps As List(Of PropertyWrapper)
#End Region

#Region " Enums "
    Enum StatusIcons
        None
        Information
        Success
        Warning
        Failure
        Processing
        Updating
    End Enum
#End Region

#Region " Properties "

    Public Property Thumbnail As System.Drawing.Image
        Get
            Return _thumbnail
        End Get
        Set(value As System.Drawing.Image)
            _thumbnail = value

            If value Is Nothing Then
                _smlthumbnail = Nothing
                _lgethumbnail = Nothing
            Else
                _smlthumbnail = ResizeImage(value, 128, 128)
                _lgethumbnail = ResizeImage(value, 512, 512)
            End If
        End Set
    End Property

    Public ReadOnly Property SmallThumbnail As System.Drawing.Image
        Get
            Return _smlthumbnail
        End Get
    End Property

    Public ReadOnly Property LargeThumbnail As System.Drawing.Image
        Get
            Return _lgethumbnail
        End Get
    End Property

    Public ReadOnly Property IsValid As Boolean
        Get
            Dim _ret As Boolean = ExportDWG.Export OrElse ExportPDF.Export
            _ret = _ret And Not String.IsNullOrWhiteSpace(DocumentName)
            _ret = _ret And Not String.IsNullOrWhiteSpace(Template)
            Return _ret
        End Get
    End Property

    Public Property Template As String
    Public Property DrawingTemplate As DrawingTemplate

    Public Property Scale As String

    Public Property ExportPDF As exportFiles
    Public Property ExportDWG As exportFiles

    'Public Property LayoutName As String

    Public Property DocumentName As String

    Public Property LayerName As String

    Public ReadOnly Property LayerIsModel As Boolean
        Get
            Return LayerName.ToUpperInvariant.EndsWith(My.Settings.ModelLayer.ToUpperInvariant)
        End Get
    End Property

    Public Property LayoutName As String

    Public Property Extents As Autodesk.AutoCAD.DatabaseServices.Extents3d

    Public ReadOnly Property Extents2D As Autodesk.AutoCAD.DatabaseServices.Extents2d
        Get
            Dim _ret As New Autodesk.AutoCAD.DatabaseServices.Extents2d(Extents.MinPoint.X, Extents.MinPoint.Y, Extents.MaxPoint.X, Extents.MaxPoint.Y)
            Return _ret
        End Get
    End Property

    Public Property Key As Guid

    Public Property ObjectID As ObjectId
        Get
            Return _objectID
        End Get
        Set(value As ObjectId)

            _objectID = value

            If Not _objectID = Nothing Then
                ''trya nd get the xdata
                LayoutUniqueID = AutoCADHelper.GetLayoutUniqueIDXData(_objectID)

            End If
        End Set
    End Property

    Public Property PolyLine As Polyline

    Public Property LayoutUniqueID As String

    Public ReadOnly Property InMfilesStatus As String
        Get
            If IsInMFiles Then Return "Document located in Excitech DOCS"

            Return "Document cannot be located in Excitech DOCS"
        End Get
    End Property

    Public Property IsInMFiles As Boolean

    Public Property ObjectVersion As ObjectVersion
        Get
            Return _objectVersion
        End Get
        Set(value As ObjectVersion)

            _objectVersion = value
            _version = $"Version: {value.ObjVer.Version}"
            DocumentName = _objectVersion.Title
        End Set
    End Property

    Public ReadOnly Property Version As String
        Get
            Return _version
        End Get
    End Property

    Public ReadOnly Property StatusIcon As StatusIcons

    Public ReadOnly Property StatusDescription As String

    Public ReadOnly Property StatusIconImage As Drawing.Image
        Get
            Select Case StatusIcon
                Case StatusIcons.Failure
                    Return My.Resources.failed_24_2
                Case StatusIcons.Success
                    Return My.Resources.success_24_2
                Case StatusIcons.Information
                    Return My.Resources.Info_24
                Case StatusIcons.Processing
                    Return My.Resources.processing_24
                Case StatusIcons.Warning
                    Return My.Resources.Warning_24_5
                Case StatusIcons.Updating
                    Return My.Resources.backgroundProcessing_24
                Case Else
                    Return Nothing
            End Select
        End Get
    End Property

    Public Property Disabled As Boolean

    Public Property ErrorMessage As String

    Public Property CustomObjectProperties As List(Of PropertyWrapper)
        Get
            Return _customProps
        End Get
        Set(value As List(Of PropertyWrapper))
            _customProps = value
        End Set
    End Property
#End Region

#Region " State and Status "
    Public Sub SetStatus(Description As String, Icon As StatusIcons)
        _StatusDescription = Description
        _StatusIcon = Icon
    End Sub
#End Region

#Region " Constructor "
    Public Sub New()
        Key = Guid.NewGuid
        IsInMFiles = False
        ExportDWG = New exportFiles With {.Export = False}
        ExportPDF = New exportFiles With {.Export = False}
        DrawingTemplate = New DrawingTemplate
        Disabled = False

        _customProps = New List(Of PropertyWrapper)
    End Sub
#End Region

#Region " Private "
    Function ResizeImage(source As Drawing.Image, width As Integer, height As Integer) As Drawing.Image
        Dim imgAr As Double = source.Width / source.Height
        Dim destAr As Double = width / height
        Dim scaleFctr As Double = 0

        If destAr > imgAr Then
            scaleFctr = height / source.Height
        Else
            scaleFctr = width / source.Width
        End If

        width = source.Width * scaleFctr
        height = source.Height * scaleFctr

        Dim destRect As New System.Drawing.Rectangle(0, 0, width, height)
        Dim destImage As New Drawing.Bitmap(width, height)

        destImage.SetResolution(source.HorizontalResolution, source.VerticalResolution)

        Using gr As Drawing.Graphics = Drawing.Graphics.FromImage(destImage)
            gr.CompositingMode = Drawing.Drawing2D.CompositingMode.SourceCopy
            gr.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
            gr.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
            gr.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
            gr.PixelOffsetMode = Drawing.Drawing2D.PixelOffsetMode.HighQuality

            Using wrap As New Drawing.Imaging.ImageAttributes
                wrap.SetWrapMode(Drawing.Drawing2D.WrapMode.TileFlipXY)
                gr.DrawImage(source, destRect, 0, 0, source.Width, source.Height, Drawing.GraphicsUnit.Pixel, wrap)
            End Using
        End Using

        Return destImage
    End Function
#End Region

#Region " Methods "
    Public Sub SetProperty(Name As String, Value As Object)
        Try
            If Name.Contains(".") Then
                Dim source As Object = Me
                Dim propInfo As Reflection.PropertyInfo
                Dim prop() As String = Name.Split(".")
                For i As Integer = 0 To prop.Length - 1 - 1
                    propInfo = source.GetType.GetProperty(prop(i))
                    source = propInfo.GetValue(source, Nothing)
                Next

                propInfo = source.GetType.GetProperty(prop.Last)
                propInfo.SetValue(source, Value, Nothing)

            Else
                Dim propInfo As Reflection.PropertyInfo = [GetType].GetProperty(Name)
                If propInfo Is Nothing Then Return
                '' attempt the assignment
                propInfo.SetValue(Me, Value, Nothing)
            End If
        Catch
        End Try
    End Sub

    Public Sub ViewObjectInMFiles(Vault As Vault)
        If ObjectVersion Is Nothing Then Throw New Exception("no associated M-Files object data")
        Dim objID = ObjectVersion.ObjVer.ObjID

        ''get object url
        Dim mFilesURLLink As String = Vault.ObjectOperations.GetMFilesURLForObject(objID, -1, False, MFilesURLType.MFilesURLTypeShow)
        Process.Start(mFilesURLLink)
    End Sub
#End Region

End Class
