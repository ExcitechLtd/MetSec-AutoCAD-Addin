<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPublishProcess
    Inherits DevExpress.XtraEditors.XtraForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPublishProcess))
        Me.labelCurrentSheet = New System.Windows.Forms.Label()
        Me.pictureboxIcon = New System.Windows.Forms.PictureBox()
        Me.buttonAbort = New System.Windows.Forms.Button()
        Me.progressBarPublishing = New DevExpress.XtraEditors.ProgressBarControl()
        CType(Me.pictureboxIcon, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.progressBarPublishing.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'labelCurrentSheet
        '
        Me.labelCurrentSheet.AutoSize = True
        Me.labelCurrentSheet.Font = New System.Drawing.Font("Tahoma", 7.8!, System.Drawing.FontStyle.Bold)
        Me.labelCurrentSheet.Location = New System.Drawing.Point(75, 37)
        Me.labelCurrentSheet.Name = "labelCurrentSheet"
        Me.labelCurrentSheet.Size = New System.Drawing.Size(68, 13)
        Me.labelCurrentSheet.TabIndex = 9
        Me.labelCurrentSheet.Text = "Processing"
        '
        'pictureboxIcon
        '
        Me.pictureboxIcon.Image = CType(resources.GetObject("pictureboxIcon.Image"), System.Drawing.Image)
        Me.pictureboxIcon.Location = New System.Drawing.Point(18, 12)
        Me.pictureboxIcon.Name = "pictureboxIcon"
        Me.pictureboxIcon.Size = New System.Drawing.Size(40, 40)
        Me.pictureboxIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.pictureboxIcon.TabIndex = 8
        Me.pictureboxIcon.TabStop = False
        '
        'buttonAbort
        '
        Me.buttonAbort.Location = New System.Drawing.Point(231, 71)
        Me.buttonAbort.Name = "buttonAbort"
        Me.buttonAbort.Size = New System.Drawing.Size(150, 30)
        Me.buttonAbort.TabIndex = 7
        Me.buttonAbort.Text = "Abort"
        Me.buttonAbort.UseVisualStyleBackColor = True
        '
        'progressBarPublishing
        '
        Me.progressBarPublishing.Location = New System.Drawing.Point(78, 12)
        Me.progressBarPublishing.Name = "progressBarPublishing"
        Me.progressBarPublishing.Properties.EndColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.progressBarPublishing.Properties.StartColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.progressBarPublishing.Properties.Step = 1
        Me.progressBarPublishing.Size = New System.Drawing.Size(516, 22)
        Me.progressBarPublishing.TabIndex = 6
        '
        'frmPublishProcess
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(612, 113)
        Me.ControlBox = False
        Me.Controls.Add(Me.labelCurrentSheet)
        Me.Controls.Add(Me.pictureboxIcon)
        Me.Controls.Add(Me.buttonAbort)
        Me.Controls.Add(Me.progressBarPublishing)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmPublishProcess"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmPublishProcess"
        CType(Me.pictureboxIcon, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.progressBarPublishing.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents labelCurrentSheet As Windows.Forms.Label
    Friend WithEvents pictureboxIcon As Windows.Forms.PictureBox
    Friend WithEvents buttonAbort As Windows.Forms.Button
    Friend WithEvents progressBarPublishing As DevExpress.XtraEditors.ProgressBarControl
End Class
