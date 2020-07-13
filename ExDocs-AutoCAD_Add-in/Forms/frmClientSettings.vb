Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms

Public Class frmClientSettings

    Private m_addinLocation As String = Assembly.GetExecutingAssembly().Location
    Private m_settingsPath As String = Path.Combine(Path.GetDirectoryName(m_addinLocation), "settings.config")
    Private m_vaultConnections As VaultConnections



    Private Sub frmMFilesSettings_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        initVaultConnections()

    End Sub

    Private Sub initVaultConnections()

        Dim selectedIndex As Integer = -1

        m_vaultConnections = g_clientApplication.GetVaultConnections()

        For Each oVltConnection As VaultConnection In m_vaultConnections
            If My.Settings.VaultConnectionName = oVltConnection.Name Then selectedIndex = cbVaultConnection.Items.Count
            cbVaultConnection.Items.Add(oVltConnection.Name)
        Next
        cbVaultConnection.SelectedIndex = selectedIndex

    End Sub

    Private Sub butSave_Click(sender As System.Object, e As System.EventArgs) Handles butSave.Click

        If dataValid() Then
            My.Settings.VaultConnectionName = cbVaultConnection.Text
            My.Settings.Save()

            DialogResult = Windows.Forms.DialogResult.OK
        End If
    End Sub

    Private Sub butCancel_Click(sender As System.Object, e As System.EventArgs) Handles butCancel.Click

        DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub


    Private Function dataValid() As Boolean

        If cbVaultConnection.SelectedIndex = -1 Then
            MessageBox.Show("Please select a Vault Connection.", My.Resources.Application_Name, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return False
        End If

        Dim vault As Vault = Nothing
        Try
            vault = m_vaultConnections(cbVaultConnection.SelectedIndex + 1).BindToVault(Me.Handle, True, True)
            If vault Is Nothing Then Throw New Exception("Unable to Login to the selected Vault Connection.")
        Catch ex As Exception
            MessageBox.Show(ex.TruncatedMessage, My.Resources.Application_Name, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return False
        End Try

        Dim vaultSettings = AutoCADVaultSettings.ReadSettings(vault, False)
        If Not vaultSettings.isConfigured Then
            MessageBox.Show("The selected Vault is not configured for AutoCAD integration." + vbCrLf + vbCrLf + "Please contact your Administrator.", My.Resources.Application_Name, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True
    End Function

End Class