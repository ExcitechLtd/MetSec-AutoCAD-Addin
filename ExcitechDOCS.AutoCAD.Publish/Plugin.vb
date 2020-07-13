Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Windows
Imports ExcitechDOCS.AutoCAD

Public Class Plugin
    Implements AutoCAD.IPlugin

#Region " Properties "
    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "PublishSheets"
        End Get
    End Property

    Public Property Arguments As Dictionary(Of String, String) Implements IPlugin.Settings
    Public Property VaultConnectionString As String Implements IPlugin.VaultconnectionString
        Get
            Return VaultStatus.VaultConnectionString
        End Get
        Set(value As String)
            VaultStatus.VaultConnectionString = value
        End Set
    End Property
#End Region

#Region " Private "
    Private _ribbonControl As RibbonControl
    Private _docsTab As RibbonTab
    Private _publishPanelSrc As RibbonPanelSource
    Private _publishPanel As RibbonPanel
    Private _commandHandler As RibbonCommandHandler
    Private _overrulecopy As AutoCADOverRuleCopy
#End Region

#Region " Constructor "

#End Region

#Region " Methods "
    Public Sub Initialize(acRibbon As Global.Autodesk.Windows.RibbonControl) Implements IPlugin.Initialize
        _ribbonControl = acRibbon
        _commandHandler = New RibbonCommandHandler

        SetupRibbonBar()

        ''load the settings
        Dim settingsFile As String = IO.Path.Combine(Settings.SettingsPath, "AutoCAD.Publish.xml")
        PluginSettings = Settings.ReadSettings(settingsFile)

        ''setup the copy override
        If _overrulecopy Is Nothing Then _overrulecopy = New AutoCADOverRuleCopy
        Overrule.AddOverrule(RXObject.GetClass(GetType(Entity)), _overrulecopy, True)
        Overrule.Overruling = True
        _overrulecopy.SetXDataFilter("EXDOCS")

    End Sub

    Private Sub SetupRibbonBar()
        If _ribbonControl Is Nothing Then Exit Sub

        ''have we got a publish panel already?
        _docsTab = _ribbonControl.Tabs.FirstOrDefault(Function(_t)
                                                          Return _t.Id = "EXDOCS_RIBBON"
                                                      End Function)

        If _docsTab Is Nothing Then Throw New Exception("Unable to locate DOCS Tab")

        ''add a panel
        _publishPanelSrc = New RibbonPanelSource
        _publishPanelSrc.Title = "Publish"
        _publishPanel = New RibbonPanel
        _publishPanel.Source = _publishPanelSrc

        ''insert this panel just after the file panel if the
        Dim pnlIndex As Integer = -1

        For _i As Integer = 0 To _docsTab.Panels.Count - 1
            If _docsTab.Panels(_i).Source.Title.ToUpperInvariant = "FILE" Then pnlIndex = _i : Exit For
        Next

        If pnlIndex > -1 Then
            _docsTab.Panels.Insert(pnlIndex + 1, _publishPanel)
        Else
            _docsTab.Panels.Add(_publishPanel)
        End If

        ''add buttons
        Dim btnPublishLayouts As New RibbonButton
        btnPublishLayouts.Text = "Publish Layouts"
        btnPublishLayouts.Id = "btnPublishLayouts"
        btnPublishLayouts.Description = "Publish Layouts"
        btnPublishLayouts.ShowText = True
        btnPublishLayouts.ShowImage = True
        btnPublishLayouts.Image = GetBitmapImage(My.Resources.PublishSheets_24)
        btnPublishLayouts.LargeImage = GetBitmapImage(My.Resources.PublishSheets_40)
        btnPublishLayouts.Orientation = Windows.Controls.Orientation.Vertical
        btnPublishLayouts.Size = RibbonItemSize.Large
        btnPublishLayouts.CommandHandler = _commandHandler

        _publishPanelSrc.Items.Add(btnPublishLayouts)

        'btnPublishLayouts = New RibbonButton
        'btnPublishLayouts.Text = "FOO"
        'btnPublishLayouts.Id = "btnFOO"
        'btnPublishLayouts.Description = "FOO"
        'btnPublishLayouts.ShowText = True
        'btnPublishLayouts.ShowImage = True
        'btnPublishLayouts.Image = GetBitmapImage(My.Resources.PublishSheets_24)
        'btnPublishLayouts.LargeImage = GetBitmapImage(My.Resources.PublishSheets_40)
        'btnPublishLayouts.Orientation = Windows.Controls.Orientation.Vertical
        'btnPublishLayouts.Size = RibbonItemSize.Large
        'btnPublishLayouts.CommandHandler = _commandHandler

        'btnPublishLayouts = New RibbonButton
        'btnPublishLayouts.Text = "BAR"
        'btnPublishLayouts.Id = "btnBAR"
        'btnPublishLayouts.Description = "BAR"
        'btnPublishLayouts.ShowText = True
        'btnPublishLayouts.ShowImage = True
        'btnPublishLayouts.Image = GetBitmapImage(My.Resources.PublishSheets_24)
        'btnPublishLayouts.LargeImage = GetBitmapImage(My.Resources.PublishSheets_40)
        'btnPublishLayouts.Orientation = Windows.Controls.Orientation.Vertical
        'btnPublishLayouts.Size = RibbonItemSize.Large
        'btnPublishLayouts.CommandHandler = _commandHandler

        ''add the buttons to he panel
        '_publishPanelSrc.Items.Add(btnPublishLayouts)
    End Sub

#End Region

#Region " Events "
    Public Event Status As IPlugin.StatusEventHandler Implements IPlugin.Status
#End Region

End Class
