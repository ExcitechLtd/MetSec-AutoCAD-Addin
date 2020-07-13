<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMFilesValueChooser
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
        Me.lvValues = New System.Windows.Forms.ListView()
        Me.chValue = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lvValues
        '
        Me.lvValues.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chValue})
        Me.lvValues.FullRowSelect = True
        Me.lvValues.GridLines = True
        Me.lvValues.HideSelection = False
        Me.lvValues.Location = New System.Drawing.Point(9, 10)
        Me.lvValues.Margin = New System.Windows.Forms.Padding(2)
        Me.lvValues.MultiSelect = False
        Me.lvValues.Name = "lvValues"
        Me.lvValues.Size = New System.Drawing.Size(420, 314)
        Me.lvValues.TabIndex = 1
        Me.lvValues.UseCompatibleStateImageBehavior = False
        Me.lvValues.View = System.Windows.Forms.View.Details
        '
        'chValue
        '
        Me.chValue.Text = "Value"
        Me.chValue.Width = 550
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(349, 335)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(268, 335)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 3
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'frmMFilesValueChooser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(436, 370)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.lvValues)
        Me.Name = "frmMFilesValueChooser"
        Me.Text = "Please Select Value"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lvValues As Windows.Forms.ListView
    Friend WithEvents chValue As Windows.Forms.ColumnHeader
    Friend WithEvents btnCancel As Windows.Forms.Button
    Friend WithEvents btnOK As Windows.Forms.Button
End Class
