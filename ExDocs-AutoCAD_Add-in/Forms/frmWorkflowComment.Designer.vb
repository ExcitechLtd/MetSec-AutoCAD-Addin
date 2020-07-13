<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmWorkflowComment
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmWorkflowComment))
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.butOk = New System.Windows.Forms.Button()
        Me.butCancel = New System.Windows.Forms.Button()
        Me.labWorkflowMessage = New System.Windows.Forms.Label()
        Me.gbComment = New System.Windows.Forms.GroupBox()
        Me.tbComment = New System.Windows.Forms.TextBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.chReopenDrawing = New System.Windows.Forms.CheckBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.gbComment.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.butOk, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.butCancel, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(394, 256)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(4)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(275, 36)
        Me.TableLayoutPanel1.TabIndex = 4
        '
        'butOk
        '
        Me.butOk.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.butOk.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.butOk.Location = New System.Drawing.Point(4, 4)
        Me.butOk.Margin = New System.Windows.Forms.Padding(4)
        Me.butOk.Name = "butOk"
        Me.butOk.Size = New System.Drawing.Size(129, 28)
        Me.butOk.TabIndex = 0
        Me.butOk.Text = "OK"
        '
        'butCancel
        '
        Me.butCancel.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.butCancel.Location = New System.Drawing.Point(142, 4)
        Me.butCancel.Margin = New System.Windows.Forms.Padding(4)
        Me.butCancel.Name = "butCancel"
        Me.butCancel.Size = New System.Drawing.Size(128, 28)
        Me.butCancel.TabIndex = 1
        Me.butCancel.Text = "Cancel"
        '
        'labWorkflowMessage
        '
        Me.labWorkflowMessage.AutoSize = True
        Me.labWorkflowMessage.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.labWorkflowMessage.Location = New System.Drawing.Point(58, 23)
        Me.labWorkflowMessage.Name = "labWorkflowMessage"
        Me.labWorkflowMessage.Size = New System.Drawing.Size(126, 17)
        Me.labWorkflowMessage.TabIndex = 6
        Me.labWorkflowMessage.Text = "Workflow Message"
        Me.labWorkflowMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'gbComment
        '
        Me.gbComment.Controls.Add(Me.tbComment)
        Me.gbComment.Location = New System.Drawing.Point(12, 66)
        Me.gbComment.Name = "gbComment"
        Me.gbComment.Size = New System.Drawing.Size(657, 183)
        Me.gbComment.TabIndex = 7
        Me.gbComment.TabStop = False
        Me.gbComment.Text = "Comment"
        '
        'tbComment
        '
        Me.tbComment.Location = New System.Drawing.Point(6, 21)
        Me.tbComment.Multiline = True
        Me.tbComment.Name = "tbComment"
        Me.tbComment.Size = New System.Drawing.Size(645, 156)
        Me.tbComment.TabIndex = 0
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.ExcitechDOCS_AutoCAD_Addin.My.Resources.Resources.Workflow_40
        Me.PictureBox1.Location = New System.Drawing.Point(12, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(40, 40)
        Me.PictureBox1.TabIndex = 5
        Me.PictureBox1.TabStop = False
        '
        'chReopenDrawing
        '
        Me.chReopenDrawing.AutoSize = True
        Me.chReopenDrawing.Checked = True
        Me.chReopenDrawing.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chReopenDrawing.Location = New System.Drawing.Point(18, 265)
        Me.chReopenDrawing.Name = "chReopenDrawing"
        Me.chReopenDrawing.Size = New System.Drawing.Size(135, 21)
        Me.chReopenDrawing.TabIndex = 8
        Me.chReopenDrawing.Text = "Reopen Drawing"
        Me.chReopenDrawing.UseVisualStyleBackColor = True
        '
        'frmWorkflowComment
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(682, 305)
        Me.Controls.Add(Me.chReopenDrawing)
        Me.Controls.Add(Me.gbComment)
        Me.Controls.Add(Me.labWorkflowMessage)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmWorkflowComment"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Change Workflow State"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.gbComment.ResumeLayout(False)
        Me.gbComment.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents butOk As System.Windows.Forms.Button
    Friend WithEvents butCancel As System.Windows.Forms.Button
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents labWorkflowMessage As System.Windows.Forms.Label
    Friend WithEvents gbComment As System.Windows.Forms.GroupBox
    Friend WithEvents tbComment As System.Windows.Forms.TextBox
    Friend WithEvents chReopenDrawing As System.Windows.Forms.CheckBox
End Class
