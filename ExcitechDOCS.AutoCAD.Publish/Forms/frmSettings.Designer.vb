<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSettings
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
        Me.tabSettings = New DevExpress.XtraTab.XtraTabControl()
        Me.tabClass = New DevExpress.XtraTab.XtraTabPage()
        Me.chkShowAllProps = New System.Windows.Forms.CheckBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lvProperties = New System.Windows.Forms.ListView()
        Me.colPropName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colDataType = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colMapping = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.cmbClass = New System.Windows.Forms.ComboBox()
        Me.tabTemplates = New DevExpress.XtraTab.XtraTabPage()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.gridTemplates = New DevExpress.XtraGrid.GridControl()
        Me.GridView1 = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.colLocIcon = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.RepositoryItemPictureEdit1 = New DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit()
        Me.colTemplateName = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.colLayout = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.RepositoryItemComboBox1 = New DevExpress.XtraEditors.Repository.RepositoryItemComboBox()
        Me.colTemplateDefault = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.editDefault = New DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.chkIncludeDOCS = New System.Windows.Forms.CheckBox()
        Me.cmdSelectFolder = New System.Windows.Forms.Button()
        Me.txtLocalTemplatePath = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSaveSettings = New System.Windows.Forms.Button()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        CType(Me.tabSettings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabSettings.SuspendLayout()
        Me.tabClass.SuspendLayout()
        Me.tabTemplates.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.gridTemplates, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemPictureEdit1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemComboBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.editDefault, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabSettings
        '
        Me.tabSettings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabSettings.Location = New System.Drawing.Point(0, 0)
        Me.tabSettings.Name = "tabSettings"
        Me.tabSettings.SelectedTabPage = Me.tabClass
        Me.tabSettings.Size = New System.Drawing.Size(749, 512)
        Me.tabSettings.TabIndex = 0
        Me.tabSettings.TabPages.AddRange(New DevExpress.XtraTab.XtraTabPage() {Me.tabClass, Me.tabTemplates})
        '
        'tabClass
        '
        Me.tabClass.Controls.Add(Me.chkShowAllProps)
        Me.tabClass.Controls.Add(Me.Label2)
        Me.tabClass.Controls.Add(Me.Label1)
        Me.tabClass.Controls.Add(Me.lvProperties)
        Me.tabClass.Controls.Add(Me.cmbClass)
        Me.tabClass.Name = "tabClass"
        Me.tabClass.Size = New System.Drawing.Size(747, 487)
        Me.tabClass.Text = "Publish Class"
        '
        'chkShowAllProps
        '
        Me.chkShowAllProps.AutoSize = True
        Me.chkShowAllProps.Location = New System.Drawing.Point(14, 371)
        Me.chkShowAllProps.Name = "chkShowAllProps"
        Me.chkShowAllProps.Size = New System.Drawing.Size(117, 17)
        Me.chkShowAllProps.TabIndex = 4
        Me.chkShowAllProps.Text = "Show all properties"
        Me.chkShowAllProps.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(381, 372)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(355, 27)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Properties in BOLD are required"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(11, 25)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(107, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "New Document Class"
        '
        'lvProperties
        '
        Me.lvProperties.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colPropName, Me.colDataType, Me.colMapping})
        Me.lvProperties.FullRowSelect = True
        Me.lvProperties.HideSelection = False
        Me.lvProperties.Location = New System.Drawing.Point(14, 63)
        Me.lvProperties.Name = "lvProperties"
        Me.lvProperties.Size = New System.Drawing.Size(722, 302)
        Me.lvProperties.TabIndex = 1
        Me.lvProperties.UseCompatibleStateImageBehavior = False
        Me.lvProperties.View = System.Windows.Forms.View.Details
        '
        'colPropName
        '
        Me.colPropName.Text = "Property Name"
        Me.colPropName.Width = 206
        '
        'colDataType
        '
        Me.colDataType.Text = "Data Type"
        Me.colDataType.Width = 240
        '
        'colMapping
        '
        Me.colMapping.Text = "Mapping"
        Me.colMapping.Width = 178
        '
        'cmbClass
        '
        Me.cmbClass.FormattingEnabled = True
        Me.cmbClass.Location = New System.Drawing.Point(124, 22)
        Me.cmbClass.Name = "cmbClass"
        Me.cmbClass.Size = New System.Drawing.Size(612, 21)
        Me.cmbClass.TabIndex = 0
        '
        'tabTemplates
        '
        Me.tabTemplates.Controls.Add(Me.GroupBox1)
        Me.tabTemplates.Controls.Add(Me.btnRefresh)
        Me.tabTemplates.Controls.Add(Me.Label3)
        Me.tabTemplates.Controls.Add(Me.chkIncludeDOCS)
        Me.tabTemplates.Controls.Add(Me.cmdSelectFolder)
        Me.tabTemplates.Controls.Add(Me.txtLocalTemplatePath)
        Me.tabTemplates.Name = "tabTemplates"
        Me.tabTemplates.Size = New System.Drawing.Size(747, 487)
        Me.tabTemplates.Text = "Templates"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.gridTemplates)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 77)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(724, 338)
        Me.GroupBox1.TabIndex = 9
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Available Templates"
        '
        'gridTemplates
        '
        Me.gridTemplates.Location = New System.Drawing.Point(6, 20)
        Me.gridTemplates.MainView = Me.GridView1
        Me.gridTemplates.Name = "gridTemplates"
        Me.gridTemplates.RepositoryItems.AddRange(New DevExpress.XtraEditors.Repository.RepositoryItem() {Me.editDefault, Me.RepositoryItemPictureEdit1, Me.RepositoryItemComboBox1})
        Me.gridTemplates.ShowOnlyPredefinedDetails = True
        Me.gridTemplates.Size = New System.Drawing.Size(712, 312)
        Me.gridTemplates.TabIndex = 7
        Me.gridTemplates.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.GridView1})
        '
        'GridView1
        '
        Me.GridView1.Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() {Me.colLocIcon, Me.colTemplateName, Me.colLayout, Me.colTemplateDefault})
        Me.GridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.GridView1.GridControl = Me.gridTemplates
        Me.GridView1.Name = "GridView1"
        Me.GridView1.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp
        Me.GridView1.OptionsCustomization.AllowColumnMoving = False
        Me.GridView1.OptionsCustomization.AllowFilter = False
        Me.GridView1.OptionsCustomization.AllowGroup = False
        Me.GridView1.OptionsDetail.EnableMasterViewMode = False
        Me.GridView1.OptionsDetail.ShowDetailTabs = False
        Me.GridView1.OptionsSelection.MultiSelect = True
        Me.GridView1.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never
        Me.GridView1.OptionsView.ShowGroupExpandCollapseButtons = False
        Me.GridView1.OptionsView.ShowGroupPanel = False
        Me.GridView1.OptionsView.ShowIndicator = False
        '
        'colLocIcon
        '
        Me.colLocIcon.ColumnEdit = Me.RepositoryItemPictureEdit1
        Me.colLocIcon.FieldName = "LocationIcon"
        Me.colLocIcon.MaxWidth = 20
        Me.colLocIcon.Name = "colLocIcon"
        Me.colLocIcon.OptionsColumn.AllowMove = False
        Me.colLocIcon.OptionsColumn.AllowSize = False
        Me.colLocIcon.OptionsColumn.FixedWidth = True
        Me.colLocIcon.OptionsColumn.ReadOnly = True
        Me.colLocIcon.Visible = True
        Me.colLocIcon.VisibleIndex = 0
        Me.colLocIcon.Width = 20
        '
        'RepositoryItemPictureEdit1
        '
        Me.RepositoryItemPictureEdit1.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.[False]
        Me.RepositoryItemPictureEdit1.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.[False]
        Me.RepositoryItemPictureEdit1.Name = "RepositoryItemPictureEdit1"
        Me.RepositoryItemPictureEdit1.ShowMenu = False
        Me.RepositoryItemPictureEdit1.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.[False]
        '
        'colTemplateName
        '
        Me.colTemplateName.Caption = "Template Name"
        Me.colTemplateName.FieldName = "TemplateName"
        Me.colTemplateName.Name = "colTemplateName"
        Me.colTemplateName.OptionsColumn.AllowEdit = False
        Me.colTemplateName.OptionsColumn.AllowMove = False
        Me.colTemplateName.OptionsColumn.ReadOnly = True
        Me.colTemplateName.Visible = True
        Me.colTemplateName.VisibleIndex = 1
        Me.colTemplateName.Width = 301
        '
        'colLayout
        '
        Me.colLayout.Caption = "Layout Name"
        Me.colLayout.ColumnEdit = Me.RepositoryItemComboBox1
        Me.colLayout.FieldName = "LayoutName"
        Me.colLayout.Name = "colLayout"
        Me.colLayout.Visible = True
        Me.colLayout.VisibleIndex = 2
        Me.colLayout.Width = 294
        '
        'RepositoryItemComboBox1
        '
        Me.RepositoryItemComboBox1.AutoHeight = False
        Me.RepositoryItemComboBox1.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.RepositoryItemComboBox1.Name = "RepositoryItemComboBox1"
        '
        'colTemplateDefault
        '
        Me.colTemplateDefault.Caption = "Default Template"
        Me.colTemplateDefault.ColumnEdit = Me.editDefault
        Me.colTemplateDefault.FieldName = "Default"
        Me.colTemplateDefault.MinWidth = 10
        Me.colTemplateDefault.Name = "colTemplateDefault"
        Me.colTemplateDefault.OptionsColumn.AllowMove = False
        Me.colTemplateDefault.OptionsColumn.AllowSize = False
        Me.colTemplateDefault.Visible = True
        Me.colTemplateDefault.VisibleIndex = 3
        Me.colTemplateDefault.Width = 95
        '
        'editDefault
        '
        Me.editDefault.AutoHeight = False
        Me.editDefault.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox
        Me.editDefault.Name = "editDefault"
        Me.editDefault.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(598, 50)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(139, 23)
        Me.btnRefresh.TabIndex = 8
        Me.btnRefresh.Text = "Refresh Templates"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(9, 21)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(103, 13)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "Local Template Path"
        '
        'chkIncludeDOCS
        '
        Me.chkIncludeDOCS.AutoSize = True
        Me.chkIncludeDOCS.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.chkIncludeDOCS.Location = New System.Drawing.Point(367, 54)
        Me.chkIncludeDOCS.Name = "chkIncludeDOCS"
        Me.chkIncludeDOCS.Size = New System.Drawing.Size(214, 17)
        Me.chkIncludeDOCS.TabIndex = 5
        Me.chkIncludeDOCS.Text = "Include Templates From Excitech DOCS"
        Me.chkIncludeDOCS.UseVisualStyleBackColor = True
        '
        'cmdSelectFolder
        '
        Me.cmdSelectFolder.Location = New System.Drawing.Point(707, 16)
        Me.cmdSelectFolder.Name = "cmdSelectFolder"
        Me.cmdSelectFolder.Size = New System.Drawing.Size(29, 23)
        Me.cmdSelectFolder.TabIndex = 2
        Me.cmdSelectFolder.Text = "..."
        Me.cmdSelectFolder.UseVisualStyleBackColor = True
        '
        'txtLocalTemplatePath
        '
        Me.txtLocalTemplatePath.Location = New System.Drawing.Point(118, 17)
        Me.txtLocalTemplatePath.Name = "txtLocalTemplatePath"
        Me.txtLocalTemplatePath.Size = New System.Drawing.Size(583, 21)
        Me.txtLocalTemplatePath.TabIndex = 1
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnCancel)
        Me.Panel1.Controls.Add(Me.btnSaveSettings)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 449)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(749, 63)
        Me.Panel1.TabIndex = 1
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(382, 14)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(83, 34)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnSaveSettings
        '
        Me.btnSaveSettings.Location = New System.Drawing.Point(284, 14)
        Me.btnSaveSettings.Name = "btnSaveSettings"
        Me.btnSaveSettings.Size = New System.Drawing.Size(83, 34)
        Me.btnSaveSettings.TabIndex = 0
        Me.btnSaveSettings.Text = "Save"
        Me.btnSaveSettings.UseVisualStyleBackColor = True
        '
        'frmSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(749, 512)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.tabSettings)
        Me.Name = "frmSettings"
        Me.Text = "Settings"
        CType(Me.tabSettings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabSettings.ResumeLayout(False)
        Me.tabClass.ResumeLayout(False)
        Me.tabClass.PerformLayout()
        Me.tabTemplates.ResumeLayout(False)
        Me.tabTemplates.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        CType(Me.gridTemplates, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemPictureEdit1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemComboBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.editDefault, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tabSettings As DevExpress.XtraTab.XtraTabControl
    Friend WithEvents tabClass As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents tabTemplates As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents chkShowAllProps As Windows.Forms.CheckBox
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents lvProperties As Windows.Forms.ListView
    Friend WithEvents colPropName As Windows.Forms.ColumnHeader
    Friend WithEvents colDataType As Windows.Forms.ColumnHeader
    Friend WithEvents colMapping As Windows.Forms.ColumnHeader
    Friend WithEvents cmbClass As Windows.Forms.ComboBox
    Friend WithEvents Panel1 As Windows.Forms.Panel
    Friend WithEvents btnCancel As Windows.Forms.Button
    Friend WithEvents btnSaveSettings As Windows.Forms.Button
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents btnRefresh As Windows.Forms.Button
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents chkIncludeDOCS As Windows.Forms.CheckBox
    Friend WithEvents cmdSelectFolder As Windows.Forms.Button
    Friend WithEvents txtLocalTemplatePath As Windows.Forms.TextBox
    Friend WithEvents FolderBrowserDialog1 As Windows.Forms.FolderBrowserDialog
    Friend WithEvents gridTemplates As DevExpress.XtraGrid.GridControl
    Friend WithEvents GridView1 As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents colLocIcon As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents colTemplateName As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents colLayout As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents colTemplateDefault As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents editDefault As DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit
    Friend WithEvents RepositoryItemPictureEdit1 As DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit
    Friend WithEvents RepositoryItemComboBox1 As DevExpress.XtraEditors.Repository.RepositoryItemComboBox
End Class
