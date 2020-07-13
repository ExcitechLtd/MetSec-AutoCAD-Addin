<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmUpdateReferences
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUpdateReferences))
        Me.butSelectAll = New System.Windows.Forms.Button()
        Me.butDeselectAll = New System.Windows.Forms.Button()
        Me.butUpdate = New System.Windows.Forms.Button()
        Me.butClose = New System.Windows.Forms.Button()
        Me.lvReferences = New System.Windows.Forms.ListView()
        Me.chCheckBoxIcon = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chReferenceName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chUpdateReason = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.imgRowHeight = New System.Windows.Forms.ImageList(Me.components)
        Me.imgListIcons = New System.Windows.Forms.ImageList(Me.components)
        Me.butViewInDocs = New System.Windows.Forms.Button()
        Me.butShowHistory = New System.Windows.Forms.Button()
        Me.tooltipUpdateReason = New System.Windows.Forms.ToolTip(Me.components)
        Me.butSelectVersion = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'butSelectAll
        '
        Me.butSelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butSelectAll.Image = Global.ExcitechDOCS_AutoCAD_Addin.My.Resources.Resources.Checked_16
        Me.butSelectAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.butSelectAll.Location = New System.Drawing.Point(590, 3)
        Me.butSelectAll.Name = "butSelectAll"
        Me.butSelectAll.Size = New System.Drawing.Size(190, 30)
        Me.butSelectAll.TabIndex = 1
        Me.butSelectAll.Text = " Select All"
        Me.butSelectAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.butSelectAll.UseVisualStyleBackColor = True
        '
        'butDeselectAll
        '
        Me.butDeselectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butDeselectAll.Image = Global.ExcitechDOCS_AutoCAD_Addin.My.Resources.Resources.Unchecked_16
        Me.butDeselectAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.butDeselectAll.Location = New System.Drawing.Point(590, 39)
        Me.butDeselectAll.Name = "butDeselectAll"
        Me.butDeselectAll.Size = New System.Drawing.Size(190, 30)
        Me.butDeselectAll.TabIndex = 2
        Me.butDeselectAll.Text = " Deselect All"
        Me.butDeselectAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.butDeselectAll.UseVisualStyleBackColor = True
        '
        'butUpdate
        '
        Me.butUpdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butUpdate.Image = Global.ExcitechDOCS_AutoCAD_Addin.My.Resources.Resources.Refresh_20
        Me.butUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.butUpdate.Location = New System.Drawing.Point(590, 183)
        Me.butUpdate.Name = "butUpdate"
        Me.butUpdate.Size = New System.Drawing.Size(190, 30)
        Me.butUpdate.TabIndex = 5
        Me.butUpdate.Text = "Update Checked Items"
        Me.butUpdate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.butUpdate.UseVisualStyleBackColor = True
        '
        'butClose
        '
        Me.butClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butClose.Location = New System.Drawing.Point(590, 313)
        Me.butClose.Name = "butClose"
        Me.butClose.Size = New System.Drawing.Size(190, 30)
        Me.butClose.TabIndex = 6
        Me.butClose.Text = "Close"
        Me.butClose.UseVisualStyleBackColor = True
        '
        'lvReferences
        '
        Me.lvReferences.AllowColumnReorder = True
        Me.lvReferences.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvReferences.CheckBoxes = True
        Me.lvReferences.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chCheckBoxIcon, Me.chReferenceName, Me.chUpdateReason})
        Me.lvReferences.FullRowSelect = True
        Me.lvReferences.GridLines = True
        Me.lvReferences.HideSelection = False
        Me.lvReferences.Location = New System.Drawing.Point(3, 3)
        Me.lvReferences.MultiSelect = False
        Me.lvReferences.Name = "lvReferences"
        Me.lvReferences.OwnerDraw = True
        Me.lvReferences.ShowItemToolTips = True
        Me.lvReferences.Size = New System.Drawing.Size(581, 340)
        Me.lvReferences.SmallImageList = Me.imgRowHeight
        Me.lvReferences.TabIndex = 0
        Me.lvReferences.UseCompatibleStateImageBehavior = False
        Me.lvReferences.View = System.Windows.Forms.View.Details
        '
        'chCheckBoxIcon
        '
        Me.chCheckBoxIcon.Name = "chCheckBoxIcon"
        Me.chCheckBoxIcon.Text = ""
        Me.chCheckBoxIcon.Width = 44
        '
        'chReferenceName
        '
        Me.chReferenceName.Name = "chReferenceName"
        Me.chReferenceName.Text = "Reference Name"
        Me.chReferenceName.Width = 250
        '
        'chUpdateReason
        '
        Me.chUpdateReason.Name = "chUpdateReason"
        Me.chUpdateReason.Text = "Change Status"
        Me.chUpdateReason.Width = 200
        '
        'imgRowHeight
        '
        Me.imgRowHeight.ImageStream = CType(resources.GetObject("imgRowHeight.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imgRowHeight.TransparentColor = System.Drawing.Color.Transparent
        Me.imgRowHeight.Images.SetKeyName(0, "ReferenceFileDWG_16.png")
        Me.imgRowHeight.Images.SetKeyName(1, "ReferenceFileIMG_16.png")
        Me.imgRowHeight.Images.SetKeyName(2, "ReferenceFilePDF_16.png")
        Me.imgRowHeight.Images.SetKeyName(3, "Checked_16.png")
        Me.imgRowHeight.Images.SetKeyName(4, "Unchecked_16.png")
        Me.imgRowHeight.Images.SetKeyName(5, "Warning_16.png")
        Me.imgRowHeight.Images.SetKeyName(6, "Info_16.png")
        '
        'imgListIcons
        '
        Me.imgListIcons.ImageStream = CType(resources.GetObject("imgListIcons.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imgListIcons.TransparentColor = System.Drawing.Color.Transparent
        Me.imgListIcons.Images.SetKeyName(0, "DWG")
        Me.imgListIcons.Images.SetKeyName(1, "IMG")
        Me.imgListIcons.Images.SetKeyName(2, "PDF")
        Me.imgListIcons.Images.SetKeyName(3, "Checked")
        Me.imgListIcons.Images.SetKeyName(4, "Unchecked")
        Me.imgListIcons.Images.SetKeyName(5, "Warning")
        Me.imgListIcons.Images.SetKeyName(6, "Info")
        Me.imgListIcons.Images.SetKeyName(7, "Tree")
        Me.imgListIcons.Images.SetKeyName(8, "Archived")
        '
        'butViewInDocs
        '
        Me.butViewInDocs.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butViewInDocs.Enabled = False
        Me.butViewInDocs.Image = Global.ExcitechDOCS_AutoCAD_Addin.My.Resources.Resources.DOCS_20
        Me.butViewInDocs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.butViewInDocs.Location = New System.Drawing.Point(590, 75)
        Me.butViewInDocs.Name = "butViewInDocs"
        Me.butViewInDocs.Size = New System.Drawing.Size(190, 30)
        Me.butViewInDocs.TabIndex = 3
        Me.butViewInDocs.Text = " View in Excitech DOCS"
        Me.butViewInDocs.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.butViewInDocs.UseVisualStyleBackColor = True
        '
        'butShowHistory
        '
        Me.butShowHistory.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butShowHistory.Enabled = False
        Me.butShowHistory.Image = Global.ExcitechDOCS_AutoCAD_Addin.My.Resources.Resources.History_16
        Me.butShowHistory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.butShowHistory.Location = New System.Drawing.Point(590, 111)
        Me.butShowHistory.Name = "butShowHistory"
        Me.butShowHistory.Size = New System.Drawing.Size(190, 30)
        Me.butShowHistory.TabIndex = 4
        Me.butShowHistory.Text = " Show History"
        Me.butShowHistory.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.butShowHistory.UseVisualStyleBackColor = True
        '
        'tooltipUpdateReason
        '
        Me.tooltipUpdateReason.AutoPopDelay = 5000
        Me.tooltipUpdateReason.InitialDelay = 0
        Me.tooltipUpdateReason.ReshowDelay = 0
        Me.tooltipUpdateReason.UseAnimation = False
        Me.tooltipUpdateReason.UseFading = False
        '
        'butSelectVersion
        '
        Me.butSelectVersion.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butSelectVersion.Enabled = False
        Me.butSelectVersion.Image = Global.ExcitechDOCS_AutoCAD_Addin.My.Resources.Resources.Properties_16
        Me.butSelectVersion.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.butSelectVersion.Location = New System.Drawing.Point(590, 147)
        Me.butSelectVersion.Name = "butSelectVersion"
        Me.butSelectVersion.Size = New System.Drawing.Size(190, 30)
        Me.butSelectVersion.TabIndex = 7
        Me.butSelectVersion.Text = " Select Version"
        Me.butSelectVersion.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.butSelectVersion.UseVisualStyleBackColor = True
        '
        'frmUpdateReferences
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(782, 355)
        Me.Controls.Add(Me.butSelectVersion)
        Me.Controls.Add(Me.lvReferences)
        Me.Controls.Add(Me.butClose)
        Me.Controls.Add(Me.butShowHistory)
        Me.Controls.Add(Me.butViewInDocs)
        Me.Controls.Add(Me.butDeselectAll)
        Me.Controls.Add(Me.butUpdate)
        Me.Controls.Add(Me.butSelectAll)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 400)
        Me.Name = "frmUpdateReferences"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Update References"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents butSelectAll As System.Windows.Forms.Button
    Friend WithEvents butDeselectAll As System.Windows.Forms.Button
    Friend WithEvents butUpdate As System.Windows.Forms.Button
    Friend WithEvents butClose As System.Windows.Forms.Button
    Friend WithEvents imgListIcons As System.Windows.Forms.ImageList
    Friend WithEvents lvReferences As System.Windows.Forms.ListView
    Friend WithEvents chCheckBoxIcon As Windows.Forms.ColumnHeader
    Friend WithEvents butViewInDocs As Windows.Forms.Button
    Friend WithEvents butShowHistory As Windows.Forms.Button
    Friend WithEvents chReferenceName As Windows.Forms.ColumnHeader
    Friend WithEvents chChangeStatus As Windows.Forms.ColumnHeader
    Friend WithEvents imgRowHeight As Windows.Forms.ImageList
    Friend WithEvents tooltipUpdateReason As Windows.Forms.ToolTip
    Friend WithEvents chUpdateReason As Windows.Forms.ColumnHeader
    Friend WithEvents butSelectVersion As Windows.Forms.Button
End Class
