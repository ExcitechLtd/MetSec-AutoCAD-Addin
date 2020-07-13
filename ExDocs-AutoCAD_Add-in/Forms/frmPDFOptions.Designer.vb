<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPDFOptions
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPDFOptions))
        Me.gbPaperSize = New System.Windows.Forms.GroupBox()
        Me.cbPaperSize = New System.Windows.Forms.ComboBox()
        Me.gbLayout = New System.Windows.Forms.GroupBox()
        Me.cbLayout = New System.Windows.Forms.ComboBox()
        Me.gbLayerState = New System.Windows.Forms.GroupBox()
        Me.cbLayerState = New System.Windows.Forms.ComboBox()
        Me.gbScale = New System.Windows.Forms.GroupBox()
        Me.cbScale = New System.Windows.Forms.ComboBox()
        Me.btPreview = New System.Windows.Forms.Button()
        Me.btPublish = New System.Windows.Forms.Button()
        Me.lbWarning = New System.Windows.Forms.Label()
        Me.gbPaperSize.SuspendLayout()
        Me.gbLayout.SuspendLayout()
        Me.gbLayerState.SuspendLayout()
        Me.gbScale.SuspendLayout()
        Me.SuspendLayout()
        '
        'gbPaperSize
        '
        Me.gbPaperSize.Controls.Add(Me.cbPaperSize)
        Me.gbPaperSize.Location = New System.Drawing.Point(3, 5)
        Me.gbPaperSize.Name = "gbPaperSize"
        Me.gbPaperSize.Size = New System.Drawing.Size(581, 55)
        Me.gbPaperSize.TabIndex = 0
        Me.gbPaperSize.TabStop = False
        Me.gbPaperSize.Text = "Paper Size"
        '
        'cbPaperSize
        '
        Me.cbPaperSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbPaperSize.FormattingEnabled = True
        Me.cbPaperSize.Items.AddRange(New Object() {"ISO full bleed A0 (841.00 x 1189.00 MM)", "ISO full bleed A1 (841.00 x 594.00 MM)", "ISO full bleed A2 (594.00 x 420.00 MM)", "ISO full bleed A3 (420.00 x 297.00 MM)", "ISO full bleed A4 (210.00 x 297.00 MM)"})
        Me.cbPaperSize.Location = New System.Drawing.Point(6, 21)
        Me.cbPaperSize.Name = "cbPaperSize"
        Me.cbPaperSize.Size = New System.Drawing.Size(568, 24)
        Me.cbPaperSize.TabIndex = 0
        '
        'gbLayout
        '
        Me.gbLayout.Controls.Add(Me.cbLayout)
        Me.gbLayout.Location = New System.Drawing.Point(3, 66)
        Me.gbLayout.Name = "gbLayout"
        Me.gbLayout.Size = New System.Drawing.Size(581, 55)
        Me.gbLayout.TabIndex = 0
        Me.gbLayout.TabStop = False
        Me.gbLayout.Text = "Layout"
        '
        'cbLayout
        '
        Me.cbLayout.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbLayout.FormattingEnabled = True
        Me.cbLayout.Location = New System.Drawing.Point(6, 21)
        Me.cbLayout.Name = "cbLayout"
        Me.cbLayout.Size = New System.Drawing.Size(568, 24)
        Me.cbLayout.TabIndex = 0
        '
        'gbLayerState
        '
        Me.gbLayerState.Controls.Add(Me.cbLayerState)
        Me.gbLayerState.Location = New System.Drawing.Point(3, 127)
        Me.gbLayerState.Name = "gbLayerState"
        Me.gbLayerState.Size = New System.Drawing.Size(581, 55)
        Me.gbLayerState.TabIndex = 0
        Me.gbLayerState.TabStop = False
        Me.gbLayerState.Text = "Layer State"
        '
        'cbLayerState
        '
        Me.cbLayerState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbLayerState.FormattingEnabled = True
        Me.cbLayerState.Location = New System.Drawing.Point(6, 21)
        Me.cbLayerState.Name = "cbLayerState"
        Me.cbLayerState.Size = New System.Drawing.Size(568, 24)
        Me.cbLayerState.TabIndex = 0
        '
        'gbScale
        '
        Me.gbScale.Controls.Add(Me.cbScale)
        Me.gbScale.Location = New System.Drawing.Point(3, 188)
        Me.gbScale.Name = "gbScale"
        Me.gbScale.Size = New System.Drawing.Size(581, 55)
        Me.gbScale.TabIndex = 0
        Me.gbScale.TabStop = False
        Me.gbScale.Text = "Scale"
        '
        'cbScale
        '
        Me.cbScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbScale.FormattingEnabled = True
        Me.cbScale.Items.AddRange(New Object() {"Scaled to Fit", "1:1", "1:2", "1:10", "1:20", "1:50", "1:100"})
        Me.cbScale.Location = New System.Drawing.Point(6, 21)
        Me.cbScale.Name = "cbScale"
        Me.cbScale.Size = New System.Drawing.Size(568, 24)
        Me.cbScale.TabIndex = 0
        '
        'btPreview
        '
        Me.btPreview.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btPreview.Location = New System.Drawing.Point(268, 260)
        Me.btPreview.Name = "btPreview"
        Me.btPreview.Size = New System.Drawing.Size(155, 36)
        Me.btPreview.TabIndex = 1
        Me.btPreview.Text = "View Preview"
        Me.btPreview.UseVisualStyleBackColor = True
        '
        'btPublish
        '
        Me.btPublish.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btPublish.Enabled = False
        Me.btPublish.Location = New System.Drawing.Point(429, 260)
        Me.btPublish.Name = "btPublish"
        Me.btPublish.Size = New System.Drawing.Size(155, 36)
        Me.btPublish.TabIndex = 1
        Me.btPublish.Text = "Publish"
        Me.btPublish.UseVisualStyleBackColor = True
        '
        'lbWarning
        '
        Me.lbWarning.AutoSize = True
        Me.lbWarning.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbWarning.ForeColor = System.Drawing.Color.DarkRed
        Me.lbWarning.Location = New System.Drawing.Point(9, 268)
        Me.lbWarning.Name = "lbWarning"
        Me.lbWarning.Size = New System.Drawing.Size(304, 18)
        Me.lbWarning.TabIndex = 3
        Me.lbWarning.Text = "Please Close the Preview to continue..."
        Me.lbWarning.Visible = False
        '
        'frmPDFOptions
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(587, 308)
        Me.Controls.Add(Me.lbWarning)
        Me.Controls.Add(Me.btPublish)
        Me.Controls.Add(Me.btPreview)
        Me.Controls.Add(Me.gbScale)
        Me.Controls.Add(Me.gbLayerState)
        Me.Controls.Add(Me.gbLayout)
        Me.Controls.Add(Me.gbPaperSize)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmPDFOptions"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "PDF Publish Options"
        Me.gbPaperSize.ResumeLayout(False)
        Me.gbLayout.ResumeLayout(False)
        Me.gbLayerState.ResumeLayout(False)
        Me.gbScale.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents gbPaperSize As System.Windows.Forms.GroupBox
    Friend WithEvents cbPaperSize As System.Windows.Forms.ComboBox
    Friend WithEvents gbLayout As System.Windows.Forms.GroupBox
    Friend WithEvents cbLayout As System.Windows.Forms.ComboBox
    Friend WithEvents gbLayerState As System.Windows.Forms.GroupBox
    Friend WithEvents cbLayerState As System.Windows.Forms.ComboBox
    Friend WithEvents gbScale As System.Windows.Forms.GroupBox
    Friend WithEvents cbScale As System.Windows.Forms.ComboBox
    Friend WithEvents btPreview As System.Windows.Forms.Button
    Friend WithEvents btPublish As System.Windows.Forms.Button
    Friend WithEvents lbWarning As System.Windows.Forms.Label
End Class
