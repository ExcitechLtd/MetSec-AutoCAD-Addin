Imports Autodesk.AutoCAD.ApplicationServices

Public Class VaultStatus
    Enum VaultStatuses
        connected
        disconnected
        offline
        noVaultConnection
        noLicense
        invalidSettings
        loggedout
    End Enum

    Private Shared vault As Vault
    Private Shared connectionAttempted As Boolean

    Shared ReadOnly Property Status() As VaultStatuses
        Get
            Dim vaultConnection As VaultConnection

            'validate vault connection
            Try
                vaultConnection = g_clientApplication.GetVaultConnection(My.Settings.VaultConnectionName)
            Catch ex As Exception
                Return VaultStatuses.noVaultConnection
            End Try

            'determine vault status
            If Not vaultConnection.IsLoggedIn Then
                Return VaultStatuses.loggedout
            ElseIf vault Is Nothing Then
                Return VaultStatuses.disconnected
            Else
                If vault.ClientOperations.IsOffline Then Return VaultStatuses.offline
                Try
                    vault.TestConnectionToVaultWithTimeout(3000)
                    Dim license As New License(vault)
                    If Not license.userHasLicense(My.Resources.License_Product_Code, vault.CurrentLoggedInUserID) Then Return VaultStatuses.noLicense
                    If AutoCADVaultSettings.ReadSettings(vault) Is Nothing Then Return VaultStatuses.invalidSettings
                    Return VaultStatuses.connected
                Catch ex As Exception
                    Return VaultStatuses.disconnected
                End Try
            End If
        End Get
    End Property

    Shared Sub EchoStatus(acDoc As Document)

        Dim currentStatus = Status()
        Select Case currentStatus
            Case VaultStatuses.connected
                acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : Connected to Vault." + vbCrLf)
            Case VaultStatuses.disconnected
                acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : Unable to open Vault Connection." + vbCrLf)
            Case VaultStatuses.invalidSettings
                acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : The selected Vault is not configured for AutoCAD integration. Please contact your Administrator." + vbCrLf)
            Case VaultStatuses.noLicense
                acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : No License available" + vbCrLf)
            Case VaultStatuses.noVaultConnection
                acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : No Vault Connection configured. Please set the active Vault Connection in Settings." + vbCrLf)
            Case VaultStatuses.offline
                acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : The Vault is currently in Offline Mode." + vbCrLf)
            Case VaultStatuses.loggedout
                acWriteMessage(acDoc, vbCrLf + My.Resources.Application_Name + " : The Vault is currently Logged Out." + vbCrLf)
        End Select
    End Sub

    Shared ReadOnly Property ShortStatusDescription() As String
        Get
            Dim currentStatus = Status()
            Select Case currentStatus
                Case VaultStatuses.connected
                    Return "Connected"
                Case VaultStatuses.disconnected
                    Return "Disconnected"
                Case VaultStatuses.invalidSettings
                    Return "Vault not configured for AutoCAD integration"
                Case VaultStatuses.noLicense
                    Return "No License Available"
                Case VaultStatuses.noVaultConnection
                    Return "No Vault Connection Configured"
                Case VaultStatuses.offline
                    Return "Vault in Offline Mode"
                Case VaultStatuses.loggedout
                    Return "Vault Logged Out"
                Case Else
                    Return "Unknown"
            End Select
        End Get
    End Property


    Public Shared Function initialiseMFiles(Optional ForceReconnection As Boolean = False) As Vault

        If ForceReconnection Then vault = Nothing : connectionAttempted = False

        Try
            'if vault object exist then check the current status
            If vault IsNot Nothing Then
                If vault.ClientOperations.IsOffline Then Return vault
                Try
                    vault.TestConnectionToVaultWithTimeout(3000)
                    Return vault
                Catch ex As Exception
                    Return Nothing
                End Try
            End If

            If connectionAttempted Then Return vault

            connectionAttempted = True
            'check Vault Connection Setting
            If My.Settings.VaultConnectionName = "" Then vault = Nothing : Return Nothing
            'try getting a Vault object
            Dim vaultConnection = g_clientApplication.GetVaultConnection(My.Settings.VaultConnectionName)

            'try to get vault object
            vault = vaultConnection.BindToVault(Nothing, True, False)

            'Disable Auto Checking Message
            vault.ClientOperations.DisableCheckInReminderForCallingProcess()
            Return vault
        Catch ex As Runtime.InteropServices.COMException
        Catch ex As Exception
        End Try

        vault = Nothing
        Return Nothing
    End Function

End Class
