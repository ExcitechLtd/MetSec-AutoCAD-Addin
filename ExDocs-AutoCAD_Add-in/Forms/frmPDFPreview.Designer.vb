<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPDFPreview
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPDFPreview))
        Me.wbPDFPreview = New System.Windows.Forms.WebBrowser()
        Me.SuspendLayout()
        '
        'wbPDFPreview
        '
        Me.wbPDFPreview.Dock = System.Windows.Forms.DockStyle.Fill
        Me.wbPDFPreview.Location = New System.Drawing.Point(0, 0)
        Me.wbPDFPreview.MinimumSize = New System.Drawing.Size(20, 20)
        Me.wbPDFPreview.Name = "wbPDFPreview"
        Me.wbPDFPreview.Size = New System.Drawing.Size(782, 555)
        Me.wbPDFPreview.TabIndex = 0
        '
        'frmPDFPreview
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(782, 555)
        Me.Controls.Add(Me.wbPDFPreview)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "frmPDFPreview"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "PDF Preview"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents wbPDFPreview As Windows.Forms.WebBrowser
End Class
