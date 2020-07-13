Public Class frmAbout

    Private Sub frmAbout_Load(sender As Object, e As EventArgs) Handles Me.Load

        'show Assembly Version
        lbVersion.Text = "Version : " + System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString
        labCopyright.Text = "© Excitech " + Now.Year.ToString

        'Check Valid Settings
        VaultStatus.initialiseMFiles()
        lbStatus.Text = "Status : " + VaultStatus.ShortStatusDescription()
    End Sub

    Private Sub frmAbout_Click(sender As Object, e As EventArgs) Handles Me.Click
        Close()
    End Sub
End Class