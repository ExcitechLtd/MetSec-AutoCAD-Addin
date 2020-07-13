<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAbout
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAbout))
        Me.lbVersion = New System.Windows.Forms.Label()
        Me.lbStatus = New System.Windows.Forms.Label()
        Me.labCopyright = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lbVersion
        '
        Me.lbVersion.BackColor = System.Drawing.Color.Black
        Me.lbVersion.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbVersion.ForeColor = System.Drawing.Color.White
        Me.lbVersion.Location = New System.Drawing.Point(30, 125)
        Me.lbVersion.Name = "lbVersion"
        Me.lbVersion.Size = New System.Drawing.Size(185, 18)
        Me.lbVersion.TabIndex = 13
        Me.lbVersion.Text = "Version : 0.0.0.0"
        Me.lbVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lbStatus
        '
        Me.lbStatus.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbStatus.BackColor = System.Drawing.Color.Black
        Me.lbStatus.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbStatus.ForeColor = System.Drawing.Color.White
        Me.lbStatus.Location = New System.Drawing.Point(324, 125)
        Me.lbStatus.Name = "lbStatus"
        Me.lbStatus.Size = New System.Drawing.Size(250, 18)
        Me.lbStatus.TabIndex = 13
        Me.lbStatus.Text = "Status : "
        Me.lbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'labCopyright
        '
        Me.labCopyright.BackColor = System.Drawing.Color.Transparent
        Me.labCopyright.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.labCopyright.ForeColor = System.Drawing.Color.White
        Me.labCopyright.Location = New System.Drawing.Point(12, 566)
        Me.labCopyright.Name = "labCopyright"
        Me.labCopyright.Size = New System.Drawing.Size(185, 18)
        Me.labCopyright.TabIndex = 14
        Me.labCopyright.Text = "© Excitech"
        Me.labCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'frmAbout
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.ClientSize = New System.Drawing.Size(600, 600)
        Me.Controls.Add(Me.labCopyright)
        Me.Controls.Add(Me.lbStatus)
        Me.Controls.Add(Me.lbVersion)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmAbout"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "About"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lbVersion As System.Windows.Forms.Label
    Friend WithEvents lbStatus As Windows.Forms.Label
    Friend WithEvents labCopyright As Windows.Forms.Label
End Class
