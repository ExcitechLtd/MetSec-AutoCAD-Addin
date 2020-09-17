Public Class Settings

#Region " Private "

#End Region

#Region " Properties "
    Public Shared ReadOnly Property Identifier As String
        Get
            Return "ExcitechDOCS.AutoCAD.Settings.Publish"
        End Get
    End Property

    Public Property DefaultTemplate As DrawingTemplate

    Public Property DefaultClass As Integer
    Public Property DefaultClassProperties As List(Of PropertyWrapper)

    Public Property LocalTemplatePath As String
    Public Property CheckDOCSForTemplates As Boolean

    Public Property ShowThumbnailColumn As Boolean = False
    Public Property CloseOnCompletion As Boolean = False
#End Region

#Region " Constructor "
    Public Sub New()
        CheckDOCSForTemplates = False
        DefaultClass = -1
    End Sub
#End Region

#Region " Methods "

    Public Shared Function ReadSettings(fileName As String) As Settings
        If Not IO.File.Exists(fileName) Then Return New Settings

        Dim str As String = ""
        Using _sr As New IO.StreamReader(fileName)
            str = _sr.ReadToEnd()
        End Using

        Dim _ret As New Settings
        _ret = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Settings)(str)
        '_ret.DefaultTemplate.InitObjVer()

        If _ret Is Nothing Then Throw New Exception("Error Reading Settings from file")
        Return _ret
    End Function

    ''save to a file
    Public Shared Function SaveSettings(fileName As String, settings As Settings) As Boolean
        Try
            Dim json As String = Newtonsoft.Json.JsonConvert.SerializeObject(settings)

            Using _sw As New IO.StreamWriter(fileName)
                _sw.WriteLine(json)
            End Using
        Catch ex As Exception

        End Try
        Return False
    End Function

    Public Shared ReadOnly Property BasePath As String
        Get
            Dim _basePath As String = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
            _basePath = IO.Path.Combine(_basePath, "Excitech DOCS\")

            Return _basePath
        End Get
    End Property

    Public Shared ReadOnly Property SettingsPath As String
        Get
            Dim setPath As String = BasePath + "AutoCAD\Settings\"

            Try
                If Not IO.Directory.Exists(setPath) Then IO.Directory.CreateDirectory(setPath)
            Catch ex As Exception

            End Try

            Return setPath
        End Get
    End Property

    Public Shared ReadOnly Property TempFilePath As String
        Get
            Dim tempPath As String = BasePath + "AutoCAD\Temp\"
            Try
                If Not IO.Directory.Exists(tempPath) Then IO.Directory.CreateDirectory(tempPath)

            Catch ex As Exception

            End Try

            Return tempPath
        End Get
    End Property

#End Region

End Class
