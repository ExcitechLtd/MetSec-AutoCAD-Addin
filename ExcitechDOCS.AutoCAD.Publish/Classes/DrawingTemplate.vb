Imports System.Drawing
Imports MFilesAPI
Imports Newtonsoft.Json

Public Class DrawingTemplate

#Region " Enum "
    Public Enum TemplateLocation
        MFILES
        DISK
    End Enum
#End Region

#Region " Private "
    Private _objVersion As ObjectVersion
    Private _fullPath As String
    Private _fileName As String
    Private _ObjNameTitle As String
    Private _objVer As ObjVer
#End Region

#Region " Properties "
    <JsonIgnore>
    Public Property objVer As ObjVer
        Get
            'If _objVer Is Nothing Then
            '    _objVer = New ObjVer()
            '    _objVer.SetLatestVersion(ObjVer_Type, ObjVer_ID)
            'End If

            Return _objVer
        End Get
        Set(value As ObjVer)
            _objVer = value

            ObjVer_ID = _objVer.ID
            ObjVer_Type = _objVer.Type
            ObjVer_Version = _objVer.Version

            '_objVer = New ObjVer()
            '_objVer.SetLatestVersion(ObjVer_Type, ObjVer_ID)

            '_objVersion = MFilesHelper.Vault.ObjectOperations.GetObjectInfo(_objVer, True)
        End Set
    End Property
    'Public Property ObjectVersion As ObjectVersion
    Public Property ObjClass As String

    Public Property ObjVer_ID As Integer
    Public Property ObjVer_Type As Integer
    Public Property ObjVer_Version As Integer


    Public Property Location As TemplateLocation

    <JsonIgnore>
    Public ReadOnly Property LocationIcon As Bitmap
        Get
            Select Case Location
                Case TemplateLocation.DISK
                    Return My.Resources.localdisk_16
                Case TemplateLocation.MFILES
                    Return My.Resources.DOCS_16
            End Select
        End Get
    End Property

    Public Property TemplateName As String
        Get
            If Me.Location = TemplateLocation.DISK Then Return _fileName
            If Me.Location = TemplateLocation.MFILES Then Return _ObjNameTitle

            Return ""
        End Get
        Set(value As String)

        End Set
    End Property

    Public Property FullPath As String
        Get
            Return _fullPath
        End Get
        Set(value As String)
            _fullPath = value
        End Set
    End Property

    Public Property ObjNameTitle As String
        Get
            Return _ObjNameTitle
        End Get
        Set(value As String)
            _ObjNameTitle = value
        End Set
    End Property

    Public Property Filename As String
        Get
            Return _fileName
        End Get
        Set(value As String)
            _fileName = value
        End Set
    End Property

    Public Property LayoutName As String

    Public Property Layouts As List(Of String)

    Public Property [Default] As Boolean
#End Region

#Region " Constrcutor "
    Public Sub New()
        Layouts = New List(Of String)
        Location = TemplateLocation.DISK
        [Default] = False
    End Sub

    Public Sub New(ObjVersion As ObjectVersion)
        _objVersion = ObjVersion
        _ObjNameTitle = _objVersion.Title
        Layouts = New List(Of String)
        Location = TemplateLocation.DISK
        [Default] = False
    End Sub

    Public Sub New(filepath As String)
        _Location = TemplateLocation.DISK
        _fullPath = filepath
        _fileName = IO.Path.GetFileNameWithoutExtension(_fullPath)
        Layouts = New List(Of String)
        Location = TemplateLocation.DISK
        [Default] = False
    End Sub
#End Region

#Region " Methods "

    Public Function GetTemplatePath(Vault As Vault) As String
        ''need to reconnect
        Dim fullPath As String = ""

        If Location = TemplateLocation.DISK Then
            fullPath = _fullPath
            Return _fullPath
        End If

        If Location = TemplateLocation.MFILES Then
            'Dim objID As ObjID = _objVersion.ObjVer.ObjID
            'Dim objVersion As Integer = _objVersion.ObjVer.Version
            Dim objID As ObjID = objVer.ObjID
            Dim objVersion As Integer = objVer.Version

            Dim results = From file As ObjectFile In _objVersion.Files Where file.Extension.ToLower = "dwg" Select file
            Dim dwgFile = results.First
            Dim fileID As Integer = dwgFile.ID
            Dim fileVersion As Integer = dwgFile.Version
            fullPath = Vault.ObjectFileOperations.GetPathInDefaultViewEx(objID, objVersion, fileID, fileVersion, MFLatestSpecificBehavior.MFLatestSpecificBehaviorLatest)
            Return fullPath
        End If

        Return ""
    End Function

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

    Public Sub InitObjVer(Vault As Vault)
        _objVer = New ObjVer()
        _objVer.SetIDs(ObjVer_Type, ObjVer_ID, ObjVer_Version)
        '_objVer.SetLatestVersion(ObjVer_Type, ObjVer_ID)

        _objVersion = Vault.ObjectOperations.GetObjectInfo(_objVer, True)
    End Sub
#End Region


End Class
