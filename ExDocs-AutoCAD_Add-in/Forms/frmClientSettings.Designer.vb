<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmClientSettings
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmClientSettings))
        Me.labVaultConnection = New System.Windows.Forms.Label()
        Me.cbVaultConnection = New System.Windows.Forms.ComboBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.butSave = New System.Windows.Forms.Button()
        Me.butCancel = New System.Windows.Forms.Button()
        Me.ofdBrowseDWGTemplate = New System.Windows.Forms.OpenFileDialog()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'labVaultConnection
        '
        Me.labVaultConnection.AutoSize = True
        Me.labVaultConnection.Location = New System.Drawing.Point(12, 19)
        Me.labVaultConnection.Name = "labVaultConnection"
        Me.labVaultConnection.Size = New System.Drawing.Size(115, 17)
        Me.labVaultConnection.TabIndex = 5
        Me.labVaultConnection.Text = "Vault Connection"
        '
        'cbVaultConnection
        '
        Me.cbVaultConnection.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbVaultConnection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbVaultConnection.FormattingEnabled = True
        Me.cbVaultConnection.Location = New System.Drawing.Point(195, 16)
        Me.cbVaultConnection.Name = "cbVaultConnection"
        Me.cbVaultConnection.Size = New System.Drawing.Size(340, 24)
        Me.cbVaultConnection.TabIndex = 0
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.butSave, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.butCancel, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(340, 56)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(4)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(195, 36)
        Me.TableLayoutPanel1.TabIndex = 5
        '
        'butSave
        '
        Me.butSave.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.butSave.Location = New System.Drawing.Point(4, 4)
        Me.butSave.Margin = New System.Windows.Forms.Padding(4)
        Me.butSave.Name = "butSave"
        Me.butSave.Size = New System.Drawing.Size(89, 28)
        Me.butSave.TabIndex = 0
        Me.butSave.Text = "Save"
        '
        'butCancel
        '
        Me.butCancel.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.butCancel.Location = New System.Drawing.Point(101, 4)
        Me.butCancel.Margin = New System.Windows.Forms.Padding(4)
        Me.butCancel.Name = "butCancel"
        Me.butCancel.Size = New System.Drawing.Size(89, 28)
        Me.butCancel.TabIndex = 1
        Me.butCancel.Text = "Cancel"
        '
        'ofdBrowseDWGTemplate
        '
        Me.ofdBrowseDWGTemplate.DefaultExt = "pdf"
        Me.ofdBrowseDWGTemplate.Filter = "AutoCAD Drawing Files|*.dwg"
        Me.ofdBrowseDWGTemplate.Title = "Please select DWG Template"
        '
        'frmClientSettings
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(542, 105)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.labVaultConnection)
        Me.Controls.Add(Me.cbVaultConnection)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmClientSettings"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "M-Files Settings"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents labVaultConnection As System.Windows.Forms.Label
    Friend WithEvents cbVaultConnection As System.Windows.Forms.ComboBox
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents butSave As System.Windows.Forms.Button
    Friend WithEvents butCancel As System.Windows.Forms.Button
    Friend WithEvents ofdBrowseDWGTemplate As System.Windows.Forms.OpenFileDialog
End Class
