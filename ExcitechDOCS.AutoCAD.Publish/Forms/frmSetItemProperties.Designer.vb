<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSetItemProperties
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
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.lvItemProps = New System.Windows.Forms.ListView()
        Me.colOverride = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colPropName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colPropDataType = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colPropValue = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cbTemplate = New System.Windows.Forms.ComboBox()
        Me.cbLayout = New System.Windows.Forms.ComboBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.chkDWG = New System.Windows.Forms.CheckBox()
        Me.chkPDF = New System.Windows.Forms.CheckBox()
        Me.btnLoadDefaults = New System.Windows.Forms.Button()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PictureBox1.Location = New System.Drawing.Point(12, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(128, 96)
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'lvItemProps
        '
        Me.lvItemProps.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvItemProps.CheckBoxes = True
        Me.lvItemProps.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colOverride, Me.colPropName, Me.colPropDataType, Me.colPropValue})
        Me.lvItemProps.FullRowSelect = True
        Me.lvItemProps.HideSelection = False
        Me.lvItemProps.Location = New System.Drawing.Point(12, 137)
        Me.lvItemProps.MultiSelect = False
        Me.lvItemProps.Name = "lvItemProps"
        Me.lvItemProps.ShowGroups = False
        Me.lvItemProps.Size = New System.Drawing.Size(776, 301)
        Me.lvItemProps.TabIndex = 1
        Me.lvItemProps.UseCompatibleStateImageBehavior = False
        Me.lvItemProps.View = System.Windows.Forms.View.Details
        '
        'colOverride
        '
        Me.colOverride.Text = "Override Default Value"
        Me.colOverride.Width = 128
        '
        'colPropName
        '
        Me.colPropName.Text = "Property Name"
        Me.colPropName.Width = 206
        '
        'colPropDataType
        '
        Me.colPropDataType.Text = "Data Type"
        Me.colPropDataType.Width = 206
        '
        'colPropValue
        '
        Me.colPropValue.Text = "Property Value"
        Me.colPropValue.Width = 206
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 118)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(77, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Item Properties"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(146, 15)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(51, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Template"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(146, 51)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(70, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Layout Name"
        '
        'cbTemplate
        '
        Me.cbTemplate.FormattingEnabled = True
        Me.cbTemplate.Location = New System.Drawing.Point(227, 12)
        Me.cbTemplate.Name = "cbTemplate"
        Me.cbTemplate.Size = New System.Drawing.Size(257, 21)
        Me.cbTemplate.TabIndex = 5
        '
        'cbLayout
        '
        Me.cbLayout.FormattingEnabled = True
        Me.cbLayout.Location = New System.Drawing.Point(227, 48)
        Me.cbLayout.Name = "cbLayout"
        Me.cbLayout.Size = New System.Drawing.Size(257, 21)
        Me.cbLayout.TabIndex = 6
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Button2)
        Me.Panel1.Controls.Add(Me.btnSave)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 473)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(800, 63)
        Me.Panel1.TabIndex = 7
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(414, 15)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(83, 34)
        Me.Button2.TabIndex = 1
        Me.Button2.Text = "Cancel"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(312, 15)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(83, 34)
        Me.btnSave.TabIndex = 0
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'chkDWG
        '
        Me.chkDWG.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.chkDWG.Location = New System.Drawing.Point(227, 84)
        Me.chkDWG.Name = "chkDWG"
        Me.chkDWG.Size = New System.Drawing.Size(94, 24)
        Me.chkDWG.TabIndex = 8
        Me.chkDWG.Text = "Publish DWG"
        Me.chkDWG.UseVisualStyleBackColor = True
        '
        'chkPDF
        '
        Me.chkPDF.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.chkPDF.Location = New System.Drawing.Point(390, 82)
        Me.chkPDF.Name = "chkPDF"
        Me.chkPDF.Size = New System.Drawing.Size(94, 24)
        Me.chkPDF.TabIndex = 9
        Me.chkPDF.Text = "Publish PDF"
        Me.chkPDF.UseVisualStyleBackColor = True
        '
        'btnLoadDefaults
        '
        Me.btnLoadDefaults.Location = New System.Drawing.Point(636, 444)
        Me.btnLoadDefaults.Name = "btnLoadDefaults"
        Me.btnLoadDefaults.Size = New System.Drawing.Size(152, 23)
        Me.btnLoadDefaults.TabIndex = 10
        Me.btnLoadDefaults.Text = "Load default property values"
        Me.btnLoadDefaults.UseVisualStyleBackColor = True
        '
        'frmSetItemProperties
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 536)
        Me.Controls.Add(Me.btnLoadDefaults)
        Me.Controls.Add(Me.chkPDF)
        Me.Controls.Add(Me.chkDWG)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.cbLayout)
        Me.Controls.Add(Me.cbTemplate)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.lvItemProps)
        Me.Name = "frmSetItemProperties"
        Me.Text = "frmSetItemProperties"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PictureBox1 As Windows.Forms.PictureBox
    Friend WithEvents lvItemProps As Windows.Forms.ListView
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents cbTemplate As Windows.Forms.ComboBox
    Friend WithEvents cbLayout As Windows.Forms.ComboBox
    Friend WithEvents Panel1 As Windows.Forms.Panel
    Friend WithEvents Button2 As Windows.Forms.Button
    Friend WithEvents btnSave As Windows.Forms.Button
    Friend WithEvents colPropName As Windows.Forms.ColumnHeader
    Friend WithEvents colPropDataType As Windows.Forms.ColumnHeader
    Friend WithEvents colPropValue As Windows.Forms.ColumnHeader
    Friend WithEvents chkDWG As Windows.Forms.CheckBox
    Friend WithEvents chkPDF As Windows.Forms.CheckBox
    Friend WithEvents btnLoadDefaults As Windows.Forms.Button
    Friend WithEvents colOverride As Windows.Forms.ColumnHeader
End Class
