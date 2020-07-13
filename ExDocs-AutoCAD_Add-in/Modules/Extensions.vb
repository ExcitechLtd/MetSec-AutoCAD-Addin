Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Autodesk.Windows
Imports ExcitechDOCS_AutoCAD_Addin.UpdateReferenceItem

Module Extensions


    <Extension()>
    Public Function GetObjectFileByName(ByVal aObjFiles As ObjectFiles, Title As String, Extension As String) As ObjectFile

        'ensure entension does not contain the period (.)
        Extension = Extension.Replace(".", "")

        'Find requested files in Multi-file object
        For Each objFile As ObjectFile In aObjFiles
            If String.Compare(Title, objFile.Title, True) = 0 And String.Compare(Extension, objFile.Extension, True) = 0 Then Return objFile
        Next
        Return Nothing

    End Function

    <Extension()>
    Public Function GetObjectFileByExtension(ByVal aObjFiles As ObjectFiles, Extension As String) As ObjectFile

        'ensure entension does not contain the period (.)
        Extension = Extension.Replace(".", "")

        'Find requested files in Multi-file object
        For Each objFile As ObjectFile In aObjFiles
            If String.Compare(Extension, objFile.Extension, True) = 0 Then Return objFile
        Next
        Return Nothing

    End Function

    <Extension()>
    Public Function isLatestVersion(aClientApp As MFilesClientApplication, VersionSpecificPath As String) As Boolean

        'get specific Version details
        Dim thisObjVersionProps As ObjectVersionAndProperties = aClientApp.FindObjectVersionAndProperties(VersionSpecificPath, False)
        Dim vault As Vault = thisObjVersionProps.Vault

        'get latest Version details
        Dim latestObjVersion As ObjectVersion = vault.ObjectOperations.GetObjectInfo(thisObjVersionProps.ObjVer, True)

        'return true if Versions are equal
        Return thisObjVersionProps.ObjVer.Version = latestObjVersion.ObjVer.Version

    End Function

    <Extension()>
    Public Function GetObjectVersionStatus(aClientApp As MFilesClientApplication, ReferencecPath As String) As ObjectVersionStatuses

        'check reference path exists in M-Files
        If Not aClientApp.IsObjectPathInMFiles(ReferencecPath) Then Return ObjectVersionStatuses.NotInMFiles

        'check reference path is version specific
        Dim rgx As New Regex("\\[0-9]+\\L\\L\\[^<>:;,?""*|\/\\]+\(ID [0-9]+\)[.\\]", RegexOptions.IgnoreCase)
        If rgx.IsMatch(ReferencecPath) Then Return ObjectVersionStatuses.NotVersionSpecificPath

        'get specific File Version details
        Dim thisObjVersionProps As ObjectVersionAndProperties = aClientApp.FindObjectVersionAndProperties(ReferencecPath, False)
        Dim thisVersion = thisObjVersionProps.ObjVer.Version
        Dim thisObjVersionFile As ObjectVersionFile = aClientApp.FindFile(ReferencecPath)
        Dim thisObjFile As ObjectFile = thisObjVersionFile.ObjectFile
        Dim thisFileID As Integer = thisObjFile.ID
        Dim thisFileVersion As Integer = thisObjFile.FileVer.Version
        Dim vault As Vault = thisObjVersionProps.Vault

        'get latest File Version details
        Dim latestObjVersion As ObjectVersion = vault.ObjectOperations.GetObjectInfo(thisObjVersionProps.ObjVer, True)
        Dim latestVersion = latestObjVersion.ObjVer.Version
        Dim latestObjFile As ObjectFile = latestObjVersion.Files.GetObjectFileByExtension(thisObjFile.Extension)
        Dim latestFileID As Integer = latestObjFile.ID
        Dim latestFileVersion As Integer = latestObjFile.Version

        'return status
        If thisVersion = latestVersion Then Return ObjectVersionStatuses.IsLatest
        If thisFileID = latestFileID AndAlso thisFileVersion < latestFileVersion Then Return ObjectVersionStatuses.FileVersionChanged
        If thisVersion < latestVersion Then Return ObjectVersionStatuses.ObjectVersionChanged

        Return ObjectVersionStatuses.Unknown

    End Function

    <Extension()>
    Public Function isArchived(aClientApp As MFilesAPI.MFilesClientApplication, VersionSpecificPath As String) As Boolean

        'get specific File Version details
        Dim thisObjVersionProps As ObjectVersionAndProperties = aClientApp.FindObjectVersionAndProperties(VersionSpecificPath, False)
        Dim vault As Vault = thisObjVersionProps.Vault

        'get latest File Version details
        Dim latestObjVer = vault.ObjectOperations.GetLatestObjVer(thisObjVersionProps.ObjVer.ObjID, False)
        Dim latestObjVersionProps As ObjectVersionAndProperties = vault.ObjectOperations.GetObjectVersionAndProperties(latestObjVer)

        Dim cdeAreaPropertyVal = latestObjVersionProps.Properties.SearchForPropertyByAlias(vault, "prCDEArea", True)
        If cdeAreaPropertyVal Is Nothing Then Return False

        Return (cdeAreaPropertyVal.Value.DisplayValue.ToUpper = "ARCHIVED")
    End Function

    <Extension()>
    Public Function getVersionSpecificPath(Vault As Vault, ObjID As ObjID, Version As Integer, FileExtension As String) As String

        'get latest File Version details
        Dim objVer As New ObjVer
        objVer.SetIDs(ObjID.Type, ObjID.ID, Version)

        Dim objVersion As ObjectVersion = Vault.ObjectOperations.GetObjectInfo(objVer, False)
        Dim objFile As ObjectFile = objVersion.Files.GetObjectFileByExtension(FileExtension)
        Dim fileVersion As Integer = objFile.Version

        'return latest Version Specific path
        Return Vault.ObjectFileOperations.GetPathInDefaultView(objVersion.ObjVer.ObjID, objVersion.ObjVer.Version, objFile.ID, fileVersion, MFLatestSpecificBehavior.MFLatestSpecificBehaviorSpecific)

    End Function

    <Extension()>
    Public Function ExcitechOperations(vault As Vault) As ExcitechOperations
        Return New ExcitechOperations(vault)
    End Function

    <Extension()>
    Public Function TruncatedMessage(Ex As Exception) As String
        Dim errorArray As String() = Split(Ex.Message, vbCrLf)
        Dim errorMessage As String = ""

        For Each errorLine In errorArray
            If errorLine = "" Then Exit For
            errorMessage += errorLine + vbCrLf
        Next

        Return errorMessage
    End Function

    <Extension()>
    Public Function GetPanelByName(Panels As RibbonPanelCollection, PanelName As String) As RibbonPanel
        If String.IsNullOrEmpty(PanelName) Then Return Nothing

        Dim _ret As RibbonPanel
        _ret = Panels.First(Function(_p)
                                If Not _p.Source.Name Is Nothing AndAlso _p.Source.Name.ToUpperInvariant = PanelName.ToUpperInvariant Then
                                    Return True
                                End If

                                If Not _p.Source.Title.ToUpperInvariant Is Nothing AndAlso _p.Source.Title.ToUpper = PanelName.ToUpperInvariant Then
                                    Return True
                                End If

                                Return False
                            End Function)

        Return _ret
    End Function
End Module


