<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPublishLayouts
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
        Me.components = New System.ComponentModel.Container()
        Dim GridFormatRule1 As DevExpress.XtraGrid.GridFormatRule = New DevExpress.XtraGrid.GridFormatRule()
        Dim FormatConditionRuleExpression1 As DevExpress.XtraEditors.FormatConditionRuleExpression = New DevExpress.XtraEditors.FormatConditionRuleExpression()
        Dim GridFormatRule2 As DevExpress.XtraGrid.GridFormatRule = New DevExpress.XtraGrid.GridFormatRule()
        Dim FormatConditionRuleExpression2 As DevExpress.XtraEditors.FormatConditionRuleExpression = New DevExpress.XtraEditors.FormatConditionRuleExpression()
        Me.colPublishStatus = New DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn()
        Me.RepositoryItemTextEdit1 = New DevExpress.XtraEditors.Repository.RepositoryItemTextEdit()
        Me.statusBar = New DevExpress.XtraBars.Ribbon.RibbonStatusBar()
        Me.barSheetCount = New DevExpress.XtraBars.BarStaticItem()
        Me.barSheetClass = New DevExpress.XtraBars.BarStaticItem()
        Me.ribbonMain = New DevExpress.XtraBars.Ribbon.RibbonControl()
        Me.btnSearch = New DevExpress.XtraBars.BarButtonItem()
        Me.btnSettings = New DevExpress.XtraBars.BarButtonItem()
        Me.btnPublisj = New DevExpress.XtraBars.BarButtonItem()
        Me.btnDebug = New DevExpress.XtraBars.BarButtonItem()
        Me.btnViewInDOCS = New DevExpress.XtraBars.BarButtonItem()
        Me.barchkCloseOnCompletion = New DevExpress.XtraBars.BarCheckItem()
        Me.BarStaticItem1 = New DevExpress.XtraBars.BarStaticItem()
        Me.barchkshowThumbNail = New DevExpress.XtraBars.BarCheckItem()
        Me.RibbonPage1 = New DevExpress.XtraBars.Ribbon.RibbonPage()
        Me.RibbonPageGroup1 = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonPageGroup2 = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RepositoryItemCheckedComboBoxEdit1 = New DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit()
        Me.GridControl1 = New DevExpress.XtraGrid.GridControl()
        Me.AdvBandedGridView1 = New DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView()
        Me.GridBand1 = New DevExpress.XtraGrid.Views.BandedGrid.GridBand()
        Me.colThumbNail = New DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn()
        Me.RepositoryItemPictureEdit1 = New DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit()
        Me.colTemplate = New DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn()
        Me.reposItemTemplate = New DevExpress.XtraEditors.Repository.RepositoryItemComboBox()
        Me.colLayout = New DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn()
        Me.editLayout = New DevExpress.XtraEditors.Repository.RepositoryItemComboBox()
        Me.colLayoutName = New DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn()
        Me.editDocumentName = New DevExpress.XtraEditors.Repository.RepositoryItemTextEdit()
        Me.bandPublishFormats = New DevExpress.XtraGrid.Views.BandedGrid.GridBand()
        Me.colDWG = New DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn()
        Me.RepositoryItemCheckEdit1 = New DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit()
        Me.colPDF = New DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn()
        Me.gridBand2 = New DevExpress.XtraGrid.Views.BandedGrid.GridBand()
        Me.colStatus = New DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn()
        Me.colVersion = New DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn()
        Me.gridBand3 = New DevExpress.XtraGrid.Views.BandedGrid.GridBand()
        Me.TimerRefreshData = New System.Windows.Forms.Timer(Me.components)
        Me.SplitContainerControl1 = New DevExpress.XtraEditors.SplitContainerControl()
        Me.pbThumb = New DevExpress.XtraEditors.PictureEdit()
        CType(Me.RepositoryItemTextEdit1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ribbonMain, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemCheckedComboBoxEdit1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AdvBandedGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemPictureEdit1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.reposItemTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.editLayout, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.editDocumentName, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemCheckEdit1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainerControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerControl1.SuspendLayout()
        CType(Me.pbThumb.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'colPublishStatus
        '
        Me.colPublishStatus.AutoFillDown = True
        Me.colPublishStatus.Caption = "Publishing Status"
        Me.colPublishStatus.ColumnEdit = Me.RepositoryItemTextEdit1
        Me.colPublishStatus.FieldName = "StatusDescription"
        Me.colPublishStatus.Name = "colPublishStatus"
        Me.colPublishStatus.OptionsColumn.AllowEdit = False
        Me.colPublishStatus.OptionsColumn.AllowFocus = False
        Me.colPublishStatus.OptionsColumn.ReadOnly = True
        Me.colPublishStatus.Visible = True
        Me.colPublishStatus.Width = 279
        '
        'RepositoryItemTextEdit1
        '
        Me.RepositoryItemTextEdit1.AutoHeight = False
        Me.RepositoryItemTextEdit1.ContextImageOptions.Image = Global.ExcitechDOCS.AutoCAD.Publish.My.Resources.Resources.Empty_24
        Me.RepositoryItemTextEdit1.Name = "RepositoryItemTextEdit1"
        '
        'statusBar
        '
        Me.statusBar.ItemLinks.Add(Me.barSheetCount)
        Me.statusBar.ItemLinks.Add(Me.barSheetClass)
        Me.statusBar.Location = New System.Drawing.Point(0, 555)
        Me.statusBar.Name = "statusBar"
        Me.statusBar.Ribbon = Me.ribbonMain
        Me.statusBar.Size = New System.Drawing.Size(1115, 27)
        '
        'barSheetCount
        '
        Me.barSheetCount.Caption = "Layout Count: 0"
        Me.barSheetCount.Id = 3
        Me.barSheetCount.ImageOptions.Image = Global.ExcitechDOCS.AutoCAD.Publish.My.Resources.Resources.PublishSheets_16
        Me.barSheetCount.Name = "barSheetCount"
        Me.barSheetCount.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph
        '
        'barSheetClass
        '
        Me.barSheetClass.Caption = "Selected Class: None selected"
        Me.barSheetClass.Id = 9
        Me.barSheetClass.ImageOptions.Image = Global.ExcitechDOCS.AutoCAD.Publish.My.Resources.Resources.class_16
        Me.barSheetClass.Name = "barSheetClass"
        Me.barSheetClass.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph
        '
        'ribbonMain
        '
        Me.ribbonMain.ColorScheme = DevExpress.XtraBars.Ribbon.RibbonControlColorScheme.Blue
        Me.ribbonMain.ExpandCollapseItem.Id = 0
        Me.ribbonMain.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.ribbonMain.ExpandCollapseItem, Me.ribbonMain.SearchEditItem, Me.btnSearch, Me.btnSettings, Me.btnPublisj, Me.btnDebug, Me.btnViewInDOCS, Me.barchkCloseOnCompletion, Me.BarStaticItem1, Me.barchkshowThumbNail})
        Me.ribbonMain.Location = New System.Drawing.Point(0, 0)
        Me.ribbonMain.MaxItemId = 17
        Me.ribbonMain.Name = "ribbonMain"
        Me.ribbonMain.Pages.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPage() {Me.RibbonPage1})
        Me.ribbonMain.RepositoryItems.AddRange(New DevExpress.XtraEditors.Repository.RepositoryItem() {Me.RepositoryItemCheckedComboBoxEdit1})
        Me.ribbonMain.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010
        Me.ribbonMain.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.[False]
        Me.ribbonMain.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.[False]
        Me.ribbonMain.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.[False]
        Me.ribbonMain.ShowPageHeadersInFormCaption = DevExpress.Utils.DefaultBoolean.[False]
        Me.ribbonMain.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.ShowOnMultiplePages
        Me.ribbonMain.ShowQatLocationSelector = False
        Me.ribbonMain.ShowToolbarCustomizeItem = False
        Me.ribbonMain.Size = New System.Drawing.Size(1115, 100)
        Me.ribbonMain.StatusBar = Me.statusBar
        Me.ribbonMain.Toolbar.ShowCustomizeItem = False
        Me.ribbonMain.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden
        '
        'btnSearch
        '
        Me.btnSearch.Caption = "Find Selections"
        Me.btnSearch.CloseSubMenuOnClickMode = DevExpress.Utils.DefaultBoolean.[True]
        Me.btnSearch.Id = 1
        Me.btnSearch.ImageOptions.Image = Global.ExcitechDOCS.AutoCAD.Publish.My.Resources.Resources.Browse_40
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'btnSettings
        '
        Me.btnSettings.Caption = "Settings"
        Me.btnSettings.Id = 2
        Me.btnSettings.ImageOptions.Image = Global.ExcitechDOCS.AutoCAD.Publish.My.Resources.Resources.settings_40
        Me.btnSettings.Name = "btnSettings"
        Me.btnSettings.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'btnPublisj
        '
        Me.btnPublisj.Caption = "Publish to Excitech DOCS"
        Me.btnPublisj.Id = 4
        Me.btnPublisj.ImageOptions.Image = Global.ExcitechDOCS.AutoCAD.Publish.My.Resources.Resources.Complete_40
        Me.btnPublisj.Name = "btnPublisj"
        Me.btnPublisj.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'btnDebug
        '
        Me.btnDebug.Caption = "Debug"
        Me.btnDebug.Id = 5
        Me.btnDebug.Name = "btnDebug"
        Me.btnDebug.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        Me.btnDebug.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        '
        'btnViewInDOCS
        '
        Me.btnViewInDOCS.Caption = "View in Excitech DOCS"
        Me.btnViewInDOCS.Id = 6
        Me.btnViewInDOCS.ImageOptions.Image = Global.ExcitechDOCS.AutoCAD.Publish.My.Resources.Resources.DOCS_40
        Me.btnViewInDOCS.Name = "btnViewInDOCS"
        Me.btnViewInDOCS.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'barchkCloseOnCompletion
        '
        Me.barchkCloseOnCompletion.Caption = "Close on Completion"
        Me.barchkCloseOnCompletion.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText
        Me.barchkCloseOnCompletion.Id = 8
        Me.barchkCloseOnCompletion.Name = "barchkCloseOnCompletion"
        '
        'BarStaticItem1
        '
        Me.BarStaticItem1.Id = 15
        Me.BarStaticItem1.Name = "BarStaticItem1"
        '
        'barchkshowThumbNail
        '
        Me.barchkshowThumbNail.Caption = "Show Thumbnail"
        Me.barchkshowThumbNail.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText
        Me.barchkshowThumbNail.Id = 16
        Me.barchkshowThumbNail.Name = "barchkshowThumbNail"
        '
        'RibbonPage1
        '
        Me.RibbonPage1.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() {Me.RibbonPageGroup1, Me.RibbonPageGroup2})
        Me.RibbonPage1.Name = "RibbonPage1"
        Me.RibbonPage1.Text = "RibbonPage1"
        '
        'RibbonPageGroup1
        '
        Me.RibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.[False]
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnSearch)
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnViewInDOCS)
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnPublisj)
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnSettings)
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnDebug)
        Me.RibbonPageGroup1.Name = "RibbonPageGroup1"
        Me.RibbonPageGroup1.Text = "Process document"
        '
        'RibbonPageGroup2
        '
        Me.RibbonPageGroup2.ItemLinks.Add(Me.barchkCloseOnCompletion)
        Me.RibbonPageGroup2.ItemLinks.Add(Me.barchkshowThumbNail)
        Me.RibbonPageGroup2.Name = "RibbonPageGroup2"
        Me.RibbonPageGroup2.Text = "Options"
        '
        'RepositoryItemCheckedComboBoxEdit1
        '
        Me.RepositoryItemCheckedComboBoxEdit1.AutoHeight = False
        Me.RepositoryItemCheckedComboBoxEdit1.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.RepositoryItemCheckedComboBoxEdit1.Items.AddRange(New DevExpress.XtraEditors.Controls.CheckedListBoxItem() {New DevExpress.XtraEditors.Controls.CheckedListBoxItem("1", "Show thumbnail column"), New DevExpress.XtraEditors.Controls.CheckedListBoxItem(Nothing, "Show Thumbnail panel")})
        Me.RepositoryItemCheckedComboBoxEdit1.Name = "RepositoryItemCheckedComboBoxEdit1"
        '
        'GridControl1
        '
        Me.GridControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GridControl1.Location = New System.Drawing.Point(0, 0)
        Me.GridControl1.MainView = Me.AdvBandedGridView1
        Me.GridControl1.MenuManager = Me.ribbonMain
        Me.GridControl1.Name = "GridControl1"
        Me.GridControl1.RepositoryItems.AddRange(New DevExpress.XtraEditors.Repository.RepositoryItem() {Me.reposItemTemplate, Me.RepositoryItemPictureEdit1, Me.RepositoryItemCheckEdit1, Me.RepositoryItemTextEdit1, Me.editDocumentName, Me.editLayout})
        Me.GridControl1.Size = New System.Drawing.Size(1101, 451)
        Me.GridControl1.TabIndex = 2
        Me.GridControl1.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.AdvBandedGridView1})
        '
        'AdvBandedGridView1
        '
        Me.AdvBandedGridView1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(CType(CType(216, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.AdvBandedGridView1.Appearance.FocusedRow.ForeColor = System.Drawing.Color.Black
        Me.AdvBandedGridView1.Appearance.FocusedRow.Options.UseBackColor = True
        Me.AdvBandedGridView1.Appearance.FocusedRow.Options.UseForeColor = True
        Me.AdvBandedGridView1.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.FromArgb(CType(CType(216, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.AdvBandedGridView1.Appearance.HideSelectionRow.Options.UseBackColor = True
        Me.AdvBandedGridView1.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(CType(CType(236, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.AdvBandedGridView1.Appearance.SelectedRow.ForeColor = System.Drawing.Color.Black
        Me.AdvBandedGridView1.Appearance.SelectedRow.Options.UseBackColor = True
        Me.AdvBandedGridView1.Appearance.SelectedRow.Options.UseForeColor = True
        Me.AdvBandedGridView1.Bands.AddRange(New DevExpress.XtraGrid.Views.BandedGrid.GridBand() {Me.GridBand1, Me.bandPublishFormats, Me.gridBand2, Me.gridBand3})
        Me.AdvBandedGridView1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.AdvBandedGridView1.Columns.AddRange(New DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn() {Me.colThumbNail, Me.colTemplate, Me.colLayout, Me.colLayoutName, Me.colStatus, Me.colDWG, Me.colPDF, Me.colVersion, Me.colPublishStatus})
        GridFormatRule1.ApplyToRow = True
        GridFormatRule1.Name = "formatDisabledRow"
        FormatConditionRuleExpression1.Appearance.ForeColor = System.Drawing.Color.Silver
        FormatConditionRuleExpression1.Appearance.Options.UseForeColor = True
        FormatConditionRuleExpression1.Expression = "[Disabled]"
        GridFormatRule1.Rule = FormatConditionRuleExpression1
        GridFormatRule2.Column = Me.colPublishStatus
        GridFormatRule2.Name = "Format0"
        FormatConditionRuleExpression2.Appearance.ForeColor = System.Drawing.Color.Black
        FormatConditionRuleExpression2.Appearance.Options.UseForeColor = True
        FormatConditionRuleExpression2.Expression = "true"
        GridFormatRule2.Rule = FormatConditionRuleExpression2
        Me.AdvBandedGridView1.FormatRules.Add(GridFormatRule1)
        Me.AdvBandedGridView1.FormatRules.Add(GridFormatRule2)
        Me.AdvBandedGridView1.GridControl = Me.GridControl1
        Me.AdvBandedGridView1.Name = "AdvBandedGridView1"
        Me.AdvBandedGridView1.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp
        Me.AdvBandedGridView1.OptionsCustomization.AllowFilter = False
        Me.AdvBandedGridView1.OptionsCustomization.AllowGroup = False
        Me.AdvBandedGridView1.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.AdvBandedGridView1.OptionsView.ShowBands = False
        Me.AdvBandedGridView1.OptionsView.ShowGroupPanel = False
        Me.AdvBandedGridView1.OptionsView.ShowIndicator = False
        Me.AdvBandedGridView1.RowHeight = 45
        '
        'GridBand1
        '
        Me.GridBand1.Caption = "GridBand1"
        Me.GridBand1.Columns.Add(Me.colThumbNail)
        Me.GridBand1.Columns.Add(Me.colTemplate)
        Me.GridBand1.Columns.Add(Me.colLayout)
        Me.GridBand1.Columns.Add(Me.colLayoutName)
        Me.GridBand1.Name = "GridBand1"
        Me.GridBand1.VisibleIndex = 0
        Me.GridBand1.Width = 568
        '
        'colThumbNail
        '
        Me.colThumbNail.AutoFillDown = True
        Me.colThumbNail.Caption = "Thumbnail"
        Me.colThumbNail.ColumnEdit = Me.RepositoryItemPictureEdit1
        Me.colThumbNail.FieldName = "SmallThumbnail"
        Me.colThumbNail.MinWidth = 128
        Me.colThumbNail.Name = "colThumbNail"
        Me.colThumbNail.OptionsColumn.AllowEdit = False
        Me.colThumbNail.OptionsColumn.AllowFocus = False
        Me.colThumbNail.OptionsColumn.AllowMove = False
        Me.colThumbNail.OptionsColumn.AllowShowHide = False
        Me.colThumbNail.OptionsColumn.AllowSize = False
        Me.colThumbNail.OptionsColumn.FixedWidth = True
        Me.colThumbNail.OptionsColumn.ReadOnly = True
        Me.colThumbNail.Width = 128
        '
        'RepositoryItemPictureEdit1
        '
        Me.RepositoryItemPictureEdit1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.RepositoryItemPictureEdit1.Name = "RepositoryItemPictureEdit1"
        Me.RepositoryItemPictureEdit1.PictureInterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear
        Me.RepositoryItemPictureEdit1.ShowMenu = False
        '
        'colTemplate
        '
        Me.colTemplate.AutoFillDown = True
        Me.colTemplate.Caption = "Template"
        Me.colTemplate.ColumnEdit = Me.reposItemTemplate
        Me.colTemplate.FieldName = "Template"
        Me.colTemplate.Name = "colTemplate"
        Me.colTemplate.Visible = True
        Me.colTemplate.Width = 195
        '
        'reposItemTemplate
        '
        Me.reposItemTemplate.AutoHeight = False
        Me.reposItemTemplate.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.reposItemTemplate.Name = "reposItemTemplate"
        '
        'colLayout
        '
        Me.colLayout.AutoFillDown = True
        Me.colLayout.Caption = "Layout Name"
        Me.colLayout.ColumnEdit = Me.editLayout
        Me.colLayout.FieldName = "LayoutName"
        Me.colLayout.Name = "colLayout"
        Me.colLayout.Visible = True
        Me.colLayout.Width = 181
        '
        'editLayout
        '
        Me.editLayout.AutoHeight = False
        Me.editLayout.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.editLayout.Name = "editLayout"
        '
        'colLayoutName
        '
        Me.colLayoutName.AutoFillDown = True
        Me.colLayoutName.Caption = "Document Name"
        Me.colLayoutName.ColumnEdit = Me.editDocumentName
        Me.colLayoutName.FieldName = "DocumentName"
        Me.colLayoutName.Name = "colLayoutName"
        Me.colLayoutName.Visible = True
        Me.colLayoutName.Width = 192
        '
        'editDocumentName
        '
        Me.editDocumentName.AutoHeight = False
        Me.editDocumentName.Name = "editDocumentName"
        '
        'bandPublishFormats
        '
        Me.bandPublishFormats.Caption = "Publish Formats"
        Me.bandPublishFormats.Columns.Add(Me.colDWG)
        Me.bandPublishFormats.Columns.Add(Me.colPDF)
        Me.bandPublishFormats.Name = "bandPublishFormats"
        Me.bandPublishFormats.VisibleIndex = 1
        Me.bandPublishFormats.Width = 117
        '
        'colDWG
        '
        Me.colDWG.AppearanceHeader.Options.UseTextOptions = True
        Me.colDWG.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.colDWG.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom
        Me.colDWG.AutoFillDown = True
        Me.colDWG.Caption = "DWG"
        Me.colDWG.ColumnEdit = Me.RepositoryItemCheckEdit1
        Me.colDWG.FieldName = "ExportDWG.Export"
        Me.colDWG.MinWidth = 25
        Me.colDWG.Name = "colDWG"
        Me.colDWG.OptionsColumn.AllowMove = False
        Me.colDWG.OptionsColumn.AllowShowHide = False
        Me.colDWG.OptionsColumn.AllowSize = False
        Me.colDWG.RowCount = 2
        Me.colDWG.Visible = True
        Me.colDWG.Width = 63
        '
        'RepositoryItemCheckEdit1
        '
        Me.RepositoryItemCheckEdit1.AutoHeight = False
        Me.RepositoryItemCheckEdit1.Name = "RepositoryItemCheckEdit1"
        '
        'colPDF
        '
        Me.colPDF.AppearanceHeader.Options.UseTextOptions = True
        Me.colPDF.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.colPDF.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom
        Me.colPDF.AutoFillDown = True
        Me.colPDF.Caption = "PDF"
        Me.colPDF.ColumnEdit = Me.RepositoryItemCheckEdit1
        Me.colPDF.FieldName = "ExportPDF.Export"
        Me.colPDF.MinWidth = 25
        Me.colPDF.Name = "colPDF"
        Me.colPDF.OptionsColumn.AllowMove = False
        Me.colPDF.OptionsColumn.AllowShowHide = False
        Me.colPDF.OptionsColumn.AllowSize = False
        Me.colPDF.RowCount = 2
        Me.colPDF.Visible = True
        Me.colPDF.Width = 54
        '
        'gridBand2
        '
        Me.gridBand2.Caption = "gridBand2"
        Me.gridBand2.Columns.Add(Me.colStatus)
        Me.gridBand2.Columns.Add(Me.colVersion)
        Me.gridBand2.Name = "gridBand2"
        Me.gridBand2.VisibleIndex = 2
        Me.gridBand2.Width = 200
        '
        'colStatus
        '
        Me.colStatus.Caption = "Document Status"
        Me.colStatus.FieldName = "InMfilesStatus"
        Me.colStatus.Name = "colStatus"
        Me.colStatus.OptionsColumn.AllowEdit = False
        Me.colStatus.OptionsColumn.ReadOnly = True
        Me.colStatus.Visible = True
        Me.colStatus.Width = 200
        '
        'colVersion
        '
        Me.colVersion.Caption = "Document Version"
        Me.colVersion.FieldName = "Version"
        Me.colVersion.Name = "colVersion"
        Me.colVersion.RowIndex = 1
        Me.colVersion.Visible = True
        Me.colVersion.Width = 200
        '
        'gridBand3
        '
        Me.gridBand3.Caption = "gridBand3"
        Me.gridBand3.Columns.Add(Me.colPublishStatus)
        Me.gridBand3.Name = "gridBand3"
        Me.gridBand3.VisibleIndex = 3
        Me.gridBand3.Width = 279
        '
        'TimerRefreshData
        '
        Me.TimerRefreshData.Enabled = True
        Me.TimerRefreshData.Interval = 500
        '
        'SplitContainerControl1
        '
        Me.SplitContainerControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat
        Me.SplitContainerControl1.Collapsed = True
        Me.SplitContainerControl1.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2
        Me.SplitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainerControl1.Location = New System.Drawing.Point(0, 100)
        Me.SplitContainerControl1.Name = "SplitContainerControl1"
        Me.SplitContainerControl1.Panel1.Controls.Add(Me.GridControl1)
        Me.SplitContainerControl1.Panel1.Text = "Panel1"
        Me.SplitContainerControl1.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.SplitContainerControl1.Panel2.Controls.Add(Me.pbThumb)
        Me.SplitContainerControl1.Panel2.Text = "Panel2"
        Me.SplitContainerControl1.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.[True]
        Me.SplitContainerControl1.Size = New System.Drawing.Size(1115, 455)
        Me.SplitContainerControl1.SplitterPosition = 818
        Me.SplitContainerControl1.TabIndex = 5
        '
        'pbThumb
        '
        Me.pbThumb.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbThumb.Location = New System.Drawing.Point(0, 0)
        Me.pbThumb.Name = "pbThumb"
        Me.pbThumb.Properties.AllowFocused = False
        Me.pbThumb.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.[False]
        Me.pbThumb.Properties.AllowScrollViaMouseDrag = True
        Me.pbThumb.Properties.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.[True]
        Me.pbThumb.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.[Auto]
        Me.pbThumb.Properties.ShowEditMenuItem = DevExpress.Utils.DefaultBoolean.[False]
        Me.pbThumb.Properties.ShowMenu = False
        Me.pbThumb.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom
        Me.pbThumb.Properties.ZoomingOperationMode = DevExpress.XtraEditors.Repository.ZoomingOperationMode.MouseWheel
        Me.pbThumb.Size = New System.Drawing.Size(0, 0)
        Me.pbThumb.TabIndex = 1
        Me.pbThumb.UseDirectXPaint = DevExpress.Utils.DefaultBoolean.[True]
        '
        'frmPublishLayouts
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1115, 582)
        Me.Controls.Add(Me.SplitContainerControl1)
        Me.Controls.Add(Me.statusBar)
        Me.Controls.Add(Me.ribbonMain)
        Me.IconOptions.Image = Global.ExcitechDOCS.AutoCAD.Publish.My.Resources.Resources.DOCS_16
        Me.Name = "frmPublishLayouts"
        Me.Text = "Publish Layouts to Excitech DOCS"
        CType(Me.RepositoryItemTextEdit1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ribbonMain, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemCheckedComboBoxEdit1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridControl1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AdvBandedGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemPictureEdit1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.reposItemTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.editLayout, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.editDocumentName, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemCheckEdit1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SplitContainerControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerControl1.ResumeLayout(False)
        CType(Me.pbThumb.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents statusBar As DevExpress.XtraBars.Ribbon.RibbonStatusBar
    Friend WithEvents GridControl1 As DevExpress.XtraGrid.GridControl
    Friend WithEvents reposItemTemplate As DevExpress.XtraEditors.Repository.RepositoryItemComboBox
    Friend WithEvents RepositoryItemPictureEdit1 As DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit
    Friend WithEvents RepositoryItemCheckEdit1 As DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit
    Friend WithEvents RepositoryItemTextEdit1 As DevExpress.XtraEditors.Repository.RepositoryItemTextEdit
    Friend WithEvents TimerRefreshData As Windows.Forms.Timer
    Friend WithEvents AdvBandedGridView1 As DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView
    Friend WithEvents colThumbNail As DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn
    Friend WithEvents colTemplate As DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn
    Friend WithEvents colLayoutName As DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn
    Friend WithEvents colDWG As DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn
    Friend WithEvents colPDF As DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn
    Friend WithEvents colStatus As DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn
    Friend WithEvents colVersion As DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn
    Friend WithEvents colPublishStatus As DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn
    Friend WithEvents editDocumentName As DevExpress.XtraEditors.Repository.RepositoryItemTextEdit
    Friend WithEvents SplitContainerControl1 As DevExpress.XtraEditors.SplitContainerControl
    Friend WithEvents ribbonMain As DevExpress.XtraBars.Ribbon.RibbonControl
    Friend WithEvents btnSearch As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents btnSettings As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents barSheetCount As DevExpress.XtraBars.BarStaticItem
    Friend WithEvents btnPublisj As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents btnDebug As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents btnViewInDOCS As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents barchkCloseOnCompletion As DevExpress.XtraBars.BarCheckItem
    Friend WithEvents barSheetClass As DevExpress.XtraBars.BarStaticItem
    Friend WithEvents RepositoryItemCheckedComboBoxEdit1 As DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit
    Friend WithEvents RibbonPage1 As DevExpress.XtraBars.Ribbon.RibbonPage
    Friend WithEvents RibbonPageGroup1 As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Friend WithEvents RibbonPageGroup2 As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Friend WithEvents pbThumb As DevExpress.XtraEditors.PictureEdit
    Friend WithEvents BarStaticItem1 As DevExpress.XtraBars.BarStaticItem
    Friend WithEvents colLayout As DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn
    Friend WithEvents editLayout As DevExpress.XtraEditors.Repository.RepositoryItemComboBox
    Friend WithEvents barchkshowThumbNail As DevExpress.XtraBars.BarCheckItem
    Friend WithEvents GridBand1 As DevExpress.XtraGrid.Views.BandedGrid.GridBand
    Friend WithEvents bandPublishFormats As DevExpress.XtraGrid.Views.BandedGrid.GridBand
    Friend WithEvents gridBand2 As DevExpress.XtraGrid.Views.BandedGrid.GridBand
    Friend WithEvents gridBand3 As DevExpress.XtraGrid.Views.BandedGrid.GridBand
End Class
