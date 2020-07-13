Imports System.IO

Class ExcitechOperations

    Private m_vault As Vault

    Public Sub New(Vault)
        m_vault = Vault
    End Sub

    Public Function GetVersionSpecificPath(FullPath As String) As String

        Dim title = Path.GetFileNameWithoutExtension(FullPath)
        Dim extension = Path.GetExtension(FullPath)

        If g_clientApplication.IsObjectPathInMFiles(FullPath) Then
            Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(FullPath)

            Dim objFile As ObjectFile
            'if single file, return the first ObjectFile from the collection
            If objVerProps.VersionData.SingleFile Then
                objFile = objVerProps.VersionData.Files(1)
            Else
                'otherwise, find Object File based on Title and Extension
                objFile = objVerProps.VersionData.Files.GetObjectFileByName(title, extension)
            End If

            If objFile Is Nothing Then Throw New Exception("ObjectFile not found")

            'return Version Specific Path
            Return m_vault.ObjectFileOperations.GetPathInDefaultView(objVerProps.ObjVer.ObjID, objVerProps.ObjVer.Version, objFile.ID, objFile.Version, MFLatestSpecificBehavior.MFLatestSpecificBehaviorSpecific)
        Else
            Throw New Exception("Path not in M-Files")
        End If

    End Function

    Public Function GetObjectFileFromPath(FullPath As String) As ObjectFile

        Dim title = Path.GetFileNameWithoutExtension(FullPath)
        Dim extension = Path.GetExtension(FullPath)

        If g_clientApplication.IsObjectPathInMFiles(FullPath) Then
            Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(FullPath)

            Dim objFile As ObjectFile
            'if single file, return the first ObjectFile from the collection
            If objVerProps.VersionData.SingleFile Then
                objFile = objVerProps.VersionData.Files(1)
            Else
                'otherwise, find Object File based on Title and Extension
                objFile = objVerProps.VersionData.Files.GetObjectFileByName(title, extension)
            End If

            If objFile Is Nothing Then Throw New Exception("ObjectFile not found")

            'return Version Specific Path
            Return objFile
        Else
            Throw New Exception("Path not in M-Files")
        End If

    End Function

    Sub AddMultipleRelationshipsToSpecificVersion(ByVal parentObject As MFilesAPI.ObjectVersion, ByVal relatedObjects() As MFilesAPI.ObjectVersion, PropertyDef As Integer)

        Dim lookups As New MFilesAPI.Lookups

        For Each relatedObj As ObjectVersion In relatedObjects
            ' Create lookup to a specific version.
            Dim lookup As New MFilesAPI.Lookup
            lookup.Item = relatedObj.ObjVer.ID
            lookup.Version = relatedObj.ObjVer.Version

            ' Create multi-select lookup structure.
            lookups.Add(-1, lookup)
        Next

        ' Create property value that includes the previously created
        ' lookup and specifies the property definition.
        Dim propertyValue As New MFilesAPI.PropertyValue
        propertyValue.TypedValue.SetValueToMultiSelectLookup(lookups)
        propertyValue.PropertyDef = PropertyDef

        ' Set property for the object.
        m_vault.ObjectPropertyOperations.SetProperty(parentObject.ObjVer, propertyValue)
    End Sub
End Class