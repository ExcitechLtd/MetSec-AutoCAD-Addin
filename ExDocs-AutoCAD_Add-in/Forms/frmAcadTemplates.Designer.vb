<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmAcadTemplates
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAcadTemplates))
        Me.btOK = New System.Windows.Forms.Button()
        Me.btCancel = New System.Windows.Forms.Button()
        Me.lvDrawingTemplates = New System.Windows.Forms.ListView()
        Me.chIcon = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chClass = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.imgIcons = New System.Windows.Forms.ImageList(Me.components)
        Me.tbTemplateSearch = New System.Windows.Forms.TextBox()
        Me.butFilterTemplates = New System.Windows.Forms.Button()
        Me.butClearFilter = New System.Windows.Forms.Button()
        Me.tooltipSearchError = New System.Windows.Forms.ToolTip(Me.components)
        Me.SuspendLayout()
        '
        'btOK
        '
        Me.btOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btOK.Enabled = False
        Me.btOK.Location = New System.Drawing.Point(308, 314)
        Me.btOK.Margin = New System.Windows.Forms.Padding(4)
        Me.btOK.Name = "btOK"
        Me.btOK.Size = New System.Drawing.Size(130, 28)
        Me.btOK.TabIndex = 0
        Me.btOK.Text = "OK"
        '
        'btCancel
        '
        Me.btCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btCancel.Location = New System.Drawing.Point(446, 314)
        Me.btCancel.Margin = New System.Windows.Forms.Padding(4)
        Me.btCancel.Name = "btCancel"
        Me.btCancel.Size = New System.Drawing.Size(130, 28)
        Me.btCancel.TabIndex = 1
        Me.btCancel.Text = "Cancel"
        '
        'lvDrawingTemplates
        '
        Me.lvDrawingTemplates.AllowColumnReorder = True
        Me.lvDrawingTemplates.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvDrawingTemplates.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chIcon, Me.chClass})
        Me.lvDrawingTemplates.FullRowSelect = True
        Me.lvDrawingTemplates.GridLines = True
        Me.lvDrawingTemplates.HideSelection = False
        Me.lvDrawingTemplates.Location = New System.Drawing.Point(5, 44)
        Me.lvDrawingTemplates.MultiSelect = False
        Me.lvDrawingTemplates.Name = "lvDrawingTemplates"
        Me.lvDrawingTemplates.Size = New System.Drawing.Size(572, 260)
        Me.lvDrawingTemplates.SmallImageList = Me.imgIcons
        Me.lvDrawingTemplates.TabIndex = 2
        Me.lvDrawingTemplates.UseCompatibleStateImageBehavior = False
        Me.lvDrawingTemplates.View = System.Windows.Forms.View.Details
        '
        'chIcon
        '
        Me.chIcon.Text = ""
        Me.chIcon.Width = 30
        '
        'chClass
        '
        Me.chClass.Text = "Class"
        Me.chClass.Width = 150
        '
        'imgIcons
        '
        Me.imgIcons.ImageStream = CType(resources.GetObject("imgIcons.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imgIcons.TransparentColor = System.Drawing.Color.Transparent
        Me.imgIcons.Images.SetKeyName(0, "dwg_30.png")
        '
        'tbTemplateSearch
        '
        Me.tbTemplateSearch.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbTemplateSearch.Location = New System.Drawing.Point(5, 12)
        Me.tbTemplateSearch.Name = "tbTemplateSearch"
        Me.tbTemplateSearch.Size = New System.Drawing.Size(500, 22)
        Me.tbTemplateSearch.TabIndex = 3
        '
        'butFilterTemplates
        '
        Me.butFilterTemplates.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butFilterTemplates.Image = Global.ExcitechDOCS_AutoCAD_Addin.My.Resources.Resources.MagnifyingGlass_20
        Me.butFilterTemplates.Location = New System.Drawing.Point(511, 8)
        Me.butFilterTemplates.Name = "butFilterTemplates"
        Me.butFilterTemplates.Size = New System.Drawing.Size(30, 30)
        Me.butFilterTemplates.TabIndex = 4
        Me.butFilterTemplates.UseVisualStyleBackColor = True
        '
        'butClearFilter
        '
        Me.butClearFilter.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butClearFilter.Image = Global.ExcitechDOCS_AutoCAD_Addin.My.Resources.Resources.Delete_20
        Me.butClearFilter.Location = New System.Drawing.Point(547, 8)
        Me.butClearFilter.Name = "butClearFilter"
        Me.butClearFilter.Size = New System.Drawing.Size(30, 30)
        Me.butClearFilter.TabIndex = 4
        Me.butClearFilter.UseVisualStyleBackColor = True
        '
        'tooltipSearchError
        '
        Me.tooltipSearchError.AutoPopDelay = 5000
        Me.tooltipSearchError.InitialDelay = 0
        Me.tooltipSearchError.ReshowDelay = 0
        Me.tooltipSearchError.UseAnimation = False
        Me.tooltipSearchError.UseFading = False
        '
        'frmAcadTemplates
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(582, 355)
        Me.Controls.Add(Me.butClearFilter)
        Me.Controls.Add(Me.butFilterTemplates)
        Me.Controls.Add(Me.tbTemplateSearch)
        Me.Controls.Add(Me.btOK)
        Me.Controls.Add(Me.lvDrawingTemplates)
        Me.Controls.Add(Me.btCancel)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(600, 400)
        Me.Name = "frmAcadTemplates"
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "AutoCAD Templates"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btOK As System.Windows.Forms.Button
    Friend WithEvents btCancel As System.Windows.Forms.Button
    Friend WithEvents lvDrawingTemplates As System.Windows.Forms.ListView
    Friend WithEvents chIcon As System.Windows.Forms.ColumnHeader
    Friend WithEvents imgIcons As System.Windows.Forms.ImageList
    Friend WithEvents chClass As Windows.Forms.ColumnHeader
    Friend WithEvents tbTemplateSearch As Windows.Forms.TextBox
    Friend WithEvents butFilterTemplates As Windows.Forms.Button
    Friend WithEvents butClearFilter As Windows.Forms.Button
    Friend WithEvents tooltipSearchError As Windows.Forms.ToolTip
End Class
