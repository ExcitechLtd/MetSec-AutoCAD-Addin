Imports System.IO
Imports System.Xml.Serialization
Imports Microsoft.Win32

Public Class CustomActions

    Const autoCADRegRoot = "SOFTWARE\Autodesk\AutoCAD"

    <CustomAction()>
    Public Shared Function CheckInstalledApplications(ByVal session As Session) As ActionResult
        session.Log("Begin CheckAutoCADInstall")

        'AutoCAD
        Dim acadKeys = getACADRegistyKeys()
        If acadKeys.Count > 0 Then session.Item("ACAD_INSTALLED") = "1"

        Return ActionResult.Success
    End Function

    <CustomAction()> Public Shared Function AddAutoCADRegistry(ByVal session As Session) As ActionResult
        session.Log("Begin AddAutoCADRegistry")

        Dim acadKeys = getACADRegistyKeys()
        For Each acadKey In acadKeys
            addACADRegistry(session, acadKey)
        Next

        Return ActionResult.Success
    End Function

    <CustomAction()> Public Shared Function RemoveAutoCADRegistry(ByVal session As Session) As ActionResult
        session.Log("Begin RemoveAutoCADRegistry")

        Dim acadKeys = getACADRegistyKeys()
        For Each acadKey In acadKeys
            removeACADRegistry(acadKey)
        Next

        Return ActionResult.Success
    End Function

#Region "AutoCAD Helper Methods"
    Private Shared Function getACADRegistyKeys() As List(Of String)

        Try
            'get version list from AutoCAD registry root
            Dim regAutoCAD = Registry.LocalMachine.OpenSubKey(autoCADRegRoot)
            Dim acadVersionList = regAutoCAD.GetSubKeyNames()
            regAutoCAD.Close()

            'Supported AutoCAD Registy Keys
            Dim validVersions = {"R20.0", "R20.1", "R21.0", "R22.0", "R23.0", "R23.1", "R24.0"}
            Dim validInstallKeys As New List(Of String)

            'validate installed AutoCAD versions
            For Each acadVersion In acadVersionList
                If validVersions.Contains(acadVersion) Then
                    updateACADValidInstallList(acadVersion, validInstallKeys)
                End If
            Next

            Return validInstallKeys
        Catch ex As Exception

        End Try

        Return New List(Of String)

    End Function

    Private Shared Sub updateACADValidInstallList(acadVersion As String, InstallKeys As List(Of String))

        'get install sub keys
        Dim regAutoCADVersion = Registry.LocalMachine.OpenSubKey(Path.Combine(autoCADRegRoot, acadVersion))
        Dim installSubKeys = regAutoCADVersion.GetSubKeyNames()
        regAutoCADVersion.Close()

        'find install keys containing 'ACAD' and a colon ':'
        Dim keysContainingColon = From keyName In installSubKeys Where keyName.Contains("ACAD") AndAlso keyName.Contains(":") Select keyName

        'check all posible installed versions (multiple language or verticals of the same acad version)
        For Each installKey In keysContainingColon
            'find acad.exe location based on registry settings
            Dim acadKeyPath = Path.Combine(autoCADRegRoot, acadVersion, installKey)
            Dim acadFullKeyPath = Path.Combine("HKEY_LOCAL_MACHINE", acadKeyPath)
            Dim acadLocationPath = Registry.GetValue(acadFullKeyPath, "AcadLocation", "")
            Dim acadExeFilename = Path.Combine(acadLocationPath, "acad.exe")

            'if acad.exe exists, then install is valid
            If File.Exists(acadExeFilename) Then InstallKeys.Add(acadKeyPath)
        Next
    End Sub

    Private Shared Sub addACADRegistry(session As Session, registryRoot As String)

        Try
            'open Applications Key
            Dim applicationsKeyPath = Path.Combine(registryRoot, "Applications")
            Dim acadApplicationKey = Registry.LocalMachine.OpenSubKey(applicationsKeyPath, True)

            'create new Excitech DOCS Key
            Dim excitechDOCSKey = acadApplicationKey.CreateSubKey("Excitech DOCS")
            excitechDOCSKey.SetValue("DESCRIPTION", "Excitech DOCS Add-in", RegistryValueKind.String)
            excitechDOCSKey.SetValue("LOADCTRLS", 2, RegistryValueKind.DWord)
            excitechDOCSKey.SetValue("LOADER", session.CustomActionData("AutoCADInstallLocation"), RegistryValueKind.String)
            excitechDOCSKey.SetValue("MANAGED", 1, RegistryValueKind.DWord)

            'close Excitech DOCS Key
            excitechDOCSKey.Close()

            'close Applications Key
            acadApplicationKey.Close()
        Catch ex As Exception
        End Try

    End Sub

    Private Shared Sub removeACADRegistry(registryRoot As String)

        Try
            'open Applications Key
            Dim applicationsKeyPath = Path.Combine(registryRoot, "Applications")
            Dim acadApplicationKey = Registry.LocalMachine.OpenSubKey(applicationsKeyPath, True)

            'remove Excitech DOCS Application entry
            acadApplicationKey.DeleteSubKey("Excitech DOCS", False)

            'close Applications Key
            acadApplicationKey.Close()
        Catch ex As Exception
        End Try
    End Sub


#End Region
End Class
