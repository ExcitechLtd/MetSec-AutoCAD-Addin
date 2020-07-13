<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmInputDialog
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
        Me.labCaption = New System.Windows.Forms.Label()
        Me.tbTextEntry = New System.Windows.Forms.TextBox()
        Me.btnOk = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.chkUseDocName = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'labCaption
        '
        Me.labCaption.AutoSize = True
        Me.labCaption.Location = New System.Drawing.Point(11, 11)
        Me.labCaption.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.labCaption.Name = "labCaption"
        Me.labCaption.Size = New System.Drawing.Size(54, 13)
        Me.labCaption.TabIndex = 7
        Me.labCaption.Text = "<caption>"
        '
        'tbTextEntry
        '
        Me.tbTextEntry.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.tbTextEntry.Location = New System.Drawing.Point(14, 35)
        Me.tbTextEntry.Margin = New System.Windows.Forms.Padding(2)
        Me.tbTextEntry.Name = "tbTextEntry"
        Me.tbTextEntry.Size = New System.Drawing.Size(264, 20)
        Me.tbTextEntry.TabIndex = 6
        '
        'btnOk
        '
        Me.btnOk.Location = New System.Drawing.Point(120, 96)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(75, 23)
        Me.btnOk.TabIndex = 8
        Me.btnOk.Text = "OK"
        Me.btnOk.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(201, 96)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 9
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'chkUseDocName
        '
        Me.chkUseDocName.AutoSize = True
        Me.chkUseDocName.Location = New System.Drawing.Point(14, 60)
        Me.chkUseDocName.Name = "chkUseDocName"
        Me.chkUseDocName.Size = New System.Drawing.Size(128, 17)
        Me.chkUseDocName.TabIndex = 10
        Me.chkUseDocName.Text = "Use Document Name"
        Me.chkUseDocName.UseVisualStyleBackColor = True
        Me.chkUseDocName.Visible = False
        '
        'frmInputDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(288, 131)
        Me.Controls.Add(Me.chkUseDocName)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.labCaption)
        Me.Controls.Add(Me.tbTextEntry)
        Me.Name = "frmInputDialog"
        Me.Text = "<heading>"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents labCaption As Windows.Forms.Label
    Friend WithEvents tbTextEntry As Windows.Forms.TextBox
    Friend WithEvents btnOk As Windows.Forms.Button
    Friend WithEvents btnCancel As Windows.Forms.Button
    Friend WithEvents chkUseDocName As Windows.Forms.CheckBox
End Class
