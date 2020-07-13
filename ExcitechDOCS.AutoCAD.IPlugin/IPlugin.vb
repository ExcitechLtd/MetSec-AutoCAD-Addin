Imports Autodesk.Windows

Public Interface IPlugin

#Region " Properties "
    ReadOnly Property Name As String
    Property Settings As Dictionary(Of String, String)
    Property VaultconnectionString As String
#End Region

#Region " Methods "
    Sub Initialize(acRibbon As RibbonControl)
#End Region

#Region " Events "
    Event Status(value As String)
#End Region

End Interface
