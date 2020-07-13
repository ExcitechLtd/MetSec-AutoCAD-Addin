<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmErrorDetails
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
        Me.buttonCopyClip = New System.Windows.Forms.Button()
        Me.buttonClose = New System.Windows.Forms.Button()
        Me.textboxErrorDetails = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'buttonCopyClip
        '
        Me.buttonCopyClip.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.buttonCopyClip.Location = New System.Drawing.Point(11, 262)
        Me.buttonCopyClip.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.buttonCopyClip.Name = "buttonCopyClip"
        Me.buttonCopyClip.Size = New System.Drawing.Size(120, 24)
        Me.buttonCopyClip.TabIndex = 4
        Me.buttonCopyClip.Text = "Copy to Clipboard"
        Me.buttonCopyClip.UseVisualStyleBackColor = True
        '
        'buttonClose
        '
        Me.buttonClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.buttonClose.Location = New System.Drawing.Point(265, 262)
        Me.buttonClose.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.buttonClose.Name = "buttonClose"
        Me.buttonClose.Size = New System.Drawing.Size(69, 24)
        Me.buttonClose.TabIndex = 3
        Me.buttonClose.Text = "Close"
        Me.buttonClose.UseVisualStyleBackColor = True
        '
        'textboxErrorDetails
        '
        Me.textboxErrorDetails.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.textboxErrorDetails.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.textboxErrorDetails.Location = New System.Drawing.Point(11, 11)
        Me.textboxErrorDetails.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.textboxErrorDetails.Multiline = True
        Me.textboxErrorDetails.Name = "textboxErrorDetails"
        Me.textboxErrorDetails.ReadOnly = True
        Me.textboxErrorDetails.Size = New System.Drawing.Size(322, 244)
        Me.textboxErrorDetails.TabIndex = 5
        Me.textboxErrorDetails.TabStop = False
        '
        'frmErrorDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(345, 297)
        Me.ControlBox = False
        Me.Controls.Add(Me.buttonCopyClip)
        Me.Controls.Add(Me.buttonClose)
        Me.Controls.Add(Me.textboxErrorDetails)
        Me.Name = "frmErrorDetails"
        Me.Text = "Error Details"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents buttonCopyClip As Windows.Forms.Button
    Friend WithEvents buttonClose As Windows.Forms.Button
    Friend WithEvents textboxErrorDetails As Windows.Forms.TextBox
End Class
