Imports MFilesAPI

Public Class LayoutCreationSettings

#Region " Private "
    Private _selectionName As String
#End Region

#Region " Properties "
    Public Shared ReadOnly Property Identifier As String
        Get
            Return "ExcitechDOCS.AutoCAD.LayoutCreation.Settings"
        End Get
    End Property

    Public Property SelectionLayerName As String
        Get
            If String.IsNullOrWhiteSpace(_selectionName) Then
                Return "EXDOCSViews"
            End If

            Return _selectionName
        End Get
        Set(value As String)
            _selectionName = value
        End Set
    End Property

    Public Property TemplateFolder As String
    Public Property DefaultTemplate As String

    Public Property DeleteDuplicateLayer As Boolean
#End Region


#Region " Constructor "
    Public Sub New()
        DeleteDuplicateLayer = True
    End Sub
#End Region

#Region " Shared "

    Public Shared Function ReadSettings(Vault As Vault) As LayoutCreationSettings
        Try
            Dim namedValues As New NamedValues
            namedValues = Vault.NamedValueStorageOperations.GetNamedValues(MFNamedValueType.MFConfigurationValue, Identifier)
            If IsDBNull(namedValues("settings")) Then Return New LayoutCreationSettings

            Dim settings As New LayoutCreationSettings
            settings = Newtonsoft.Json.JsonConvert.DeserializeObject(Of LayoutCreationSettings)(namedValues("settings"))

            If settings Is Nothing Then Throw New Exception("Error Reading Settings From Vault")
            Return settings
        Catch ex As Exception
            Throw
        End Try

        Return New LayoutCreationSettings
    End Function

    Public Shared Function ReadSettings(filename As String) As LayoutCreationSettings
        Try
            Dim strSettings As String = ""

            If Not IO.File.Exists(filename) Then Return New LayoutCreationSettings

            Using _sr As New IO.StreamReader(filename)
                strSettings = _sr.ReadToEnd
            End Using

            Dim settings As New LayoutCreationSettings
            settings = Newtonsoft.Json.JsonConvert.DeserializeObject(Of LayoutCreationSettings)(strSettings)

            If settings Is Nothing Then Throw New Exception("Error Reading Settings From String")
            Return settings
        Catch ex As Exception
            Throw
        End Try

        Return New LayoutCreationSettings
    End Function

    Public Shared Function SaveSettings(Vault As Vault, settings As LayoutCreationSettings) As Boolean
        Try
            Dim strSettings As String = Newtonsoft.Json.JsonConvert.SerializeObject(settings)
            Dim namedValues As New NamedValues
            namedValues("settings") = Newtonsoft.Json.JsonConvert.SerializeObject(settings)
            Vault.NamedValueStorageOperations.SetNamedValues(MFNamedValueType.MFConfigurationValue, LayoutCreationSettings.Identifier, namedValues)

            Return True
        Catch ex As Exception
            Throw
        End Try
        Return False
    End Function

    Public Shared Function SaveSettings(fileName As String, settings As LayoutCreationSettings) As Boolean
        Try
            Dim _dir As String = IO.Path.GetDirectoryName(fileName)
            If Not IO.Directory.Exists(_dir) Then IO.Directory.CreateDirectory(_dir)

            Dim strSettings As String = Newtonsoft.Json.JsonConvert.SerializeObject(settings)
            Using _sw As New IO.StreamWriter(fileName)
                _sw.WriteLine(strSettings)
            End Using
            Return True
        Catch ex As Exception
            Throw
        End Try

        Return False
    End Function

#End Region

End Class
