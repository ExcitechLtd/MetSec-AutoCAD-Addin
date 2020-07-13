Imports System.Drawing
Imports System.IO
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices

Public Class UpdateReferenceItem

    Public Enum ObjectVersionStatuses
        IsLatest
        ObjectVersionChanged
        FileVersionChanged
        NotInMFiles
        NotVersionSpecificPath
        Unknown
    End Enum

#Region "Instance Variables"
    Private m_vault As Vault
    Private m_document As Document
    Private m_dwgDatabase As Database
    Private m_acObjId As ObjectId
    Private m_referenceName As String
    Private m_objVer As ObjVer
    Private m_fileID As Integer
    Private m_namedValues As NamedValues
    Private m_versionStatus As ObjectVersionStatuses
    Private m_isArchived As Boolean
    Private m_isNested As Boolean
    Private m_isValid As Boolean = False
#End Region

#Region "Properties"
    Public ReadOnly Property ReferenceName As String
        Get
            Return m_referenceName
        End Get
    End Property

    Public ReadOnly Property IsValid As Boolean
        Get
            Return m_isValid
        End Get
    End Property

    Public ReadOnly Property ObjID As ObjID
        Get
            Return m_objVer.ObjID
        End Get
    End Property

    Public ReadOnly Property ObjVer As ObjVer
        Get
            Return m_objVer
        End Get
    End Property

    Public ReadOnly Property NamedValues As NamedValues
        Get
            Return m_namedValues
        End Get
    End Property

    Public ReadOnly Property VersionStatus As ObjectVersionStatuses
        Get
            Return m_versionStatus
        End Get
    End Property

    Public ReadOnly Property ChangeStatusColor As Color
        Get
            If VersionStatus = ObjectVersionStatuses.IsLatest Then
                Return Color.Black
            Else
                Return Color.Red
            End If
        End Get
    End Property

    Public ReadOnly Property IsCheckable As Boolean
        Get
            If m_isArchived Then Return False
            If m_isNested Then Return False
            If m_versionStatus = ObjectVersionStatuses.FileVersionChanged Then Return True
            If m_versionStatus = ObjectVersionStatuses.ObjectVersionChanged Then Return True
            Return False
        End Get
    End Property

    Public ReadOnly Property RequiresAttention As Boolean
        Get
            If m_isArchived Then Return True
            If m_versionStatus <> ObjectVersionStatuses.IsLatest Then Return True
            Return False
        End Get
    End Property

    Public ReadOnly Property IsWarning As Boolean
        Get
            If m_isArchived Then Return True
            If m_versionStatus <> ObjectVersionStatuses.IsLatest And m_isNested Then Return True
            If m_versionStatus = ObjectVersionStatuses.NotInMFiles Then Return True
            If m_versionStatus = ObjectVersionStatuses.NotVersionSpecificPath Then Return True
            Return False
        End Get
    End Property

    Public ReadOnly Property IsNested As Boolean
        Get
            Return m_isNested
        End Get
    End Property

    Public ReadOnly Property IsArchived As Boolean
        Get
            Return m_isArchived
        End Get
    End Property

    Public ReadOnly Property VersionStatusDescription() As String
        Get
            Dim status As String

            Select Case m_versionStatus
                Case ObjectVersionStatuses.IsLatest
                    status = "Latest"
                Case ObjectVersionStatuses.ObjectVersionChanged
                    status = "Metadata"
                Case ObjectVersionStatuses.FileVersionChanged
                    status = "Newer version available"
                Case ObjectVersionStatuses.NotInMFiles
                    status = "Not in vault"
                Case ObjectVersionStatuses.NotVersionSpecificPath
                    status = "Path is not to a specific version"
                Case Else
                    status = "Unknown"
            End Select

            If IsNested Then status += " (nested)"
            If IsArchived Then status += " (archived)"
            Return status
        End Get
    End Property


    Public ReadOnly Property VersionStatusHelp() As String
        Get
            Select Case m_versionStatus
                Case ObjectVersionStatuses.ObjectVersionChanged
                    Return "A property such as ‘Status’ has changed." + vbCrLf +
                           "The file itself has not changed."
                Case ObjectVersionStatuses.FileVersionChanged
                    Return "There is a newer version of this reference file." + vbCrLf +
                           "The metadata of this reference file may also have changed."
                Case ObjectVersionStatuses.NotInMFiles
                    Return "The reference is not stored in " + My.Resources.EDM_Name + vbCrLf +
                           "Detach the file, move it into " + My.Resources.EDM_Name + " and re-attach it from there"
                Case ObjectVersionStatuses.NotVersionSpecificPath
                    Return "The reference is not to a specific version of the file" + vbCrLf +
                           "Any changes to the reference file will automatically affect this drawing"
                Case Else
                    Return "This reference is up to date."
            End Select
        End Get
    End Property

    Public ReadOnly Property CanViewInMFiles() As Boolean
        Get
            Return VersionStatus <> ObjectVersionStatuses.NotInMFiles
        End Get
    End Property

    Public ReadOnly Property CanSelectVersion() As Boolean
        Get
            Return Not m_isNested And CanViewInMFiles
        End Get
    End Property
#End Region

    'Constructor 
    Public Sub New(Vault As Vault, ReferenceObjectID As ObjectId)

        m_vault = Vault
        m_acObjId = ReferenceObjectID
        m_dwgDatabase = ReferenceObjectID.Database
        m_document = Core.Application.DocumentManager.GetDocument(m_dwgDatabase)
        InitialiseReference()
    End Sub

#Region "Initialisation Methods"
    Public Sub InitialiseReference()

        Select Case m_acObjId.ObjectClass.Name
            Case "AcDbBlockTableRecord"
                initAcDbBlockTableRecord()

            Case "AcDbRasterImageDef"
                initAcDbRasterImageDef()

            Case "AcDbPdfDefinition"
                initAcDbPdfDefinition()

        End Select

    End Sub

    Private Sub initAcDbBlockTableRecord()
        Using trans As Transaction = m_dwgDatabase.TransactionManager.StartTransaction
            Dim blkTabRec As BlockTableRecord = trans.GetObject(m_acObjId, OpenMode.ForRead)
            m_referenceName = blkTabRec.Name

            'Is nested?
            Dim xrefGraph = m_dwgDatabase.GetHostDwgXrefGraph(False)
            Dim xrefNode = xrefGraph.GetXrefNode(m_acObjId)
            m_isNested = xrefNode.IsNested

            'get XREF Definition Full Path
            Dim xrefDatabase = blkTabRec.GetXrefDatabase(False)
            If xrefDatabase Is Nothing Then Return
            Dim xrefFullPath As String = xrefDatabase.Filename

            'initalise Version Status
            m_versionStatus = g_clientApplication.GetObjectVersionStatus(xrefFullPath)

            'if reference is in M-Files then 
            If m_versionStatus <> ObjectVersionStatuses.NotInMFiles Then
                Dim objVerProps = g_clientApplication.FindObjectVersionAndProperties(xrefFullPath, False)
                Dim vault = objVerProps.Vault
                m_namedValues = vault.ObjectPropertyOperations.GetPropertiesForMetadataSync(objVerProps.ObjVer, MFMetadataSyncFormat.MFMetadataSyncFormatPowerPoint)
                m_objVer = objVerProps.ObjVer
                m_isArchived = g_clientApplication.isArchived(xrefFullPath)

                Try
                    m_fileID = vault.ExcitechOperations.GetObjectFileFromPath(xrefFullPath).ID
                Catch ex As Exception
                    Return
                End Try
            End If
        End Using

        m_isValid = True
    End Sub

    Private Sub initAcDbRasterImageDef()

        Using trans As Transaction = m_dwgDatabase.TransactionManager.StartTransaction
            Dim imageDictID = RasterImageDef.GetImageDictionary(m_dwgDatabase)
            Dim imageDefDict As DBDictionary = trans.GetObject(imageDictID, OpenMode.ForRead)
            Dim imageDef As RasterImageDef = trans.GetObject(m_acObjId, OpenMode.ForRead)
            m_referenceName = imageDefDict.NameAt(m_acObjId)

            'get Image Definition Full Path
            Dim imageDefFullPath As String = imageDef.ActiveFileName
            If Not imageDef.IsLoaded Then Return

            'initalise Version Status
            m_versionStatus = g_clientApplication.GetObjectVersionStatus(imageDefFullPath)

            'if reference is in M-Files then 
            If m_versionStatus <> ObjectVersionStatuses.NotInMFiles Then
                Dim objVerProps = g_clientApplication.FindObjectVersionAndProperties(imageDefFullPath, False)
                Dim vault = objVerProps.Vault
                m_namedValues = vault.ObjectPropertyOperations.GetPropertiesForMetadataSync(objVerProps.ObjVer, MFMetadataSyncFormat.MFMetadataSyncFormatPowerPoint)
                m_objVer = objVerProps.ObjVer
                m_isArchived = g_clientApplication.isArchived(imageDefFullPath)

                Try
                    m_fileID = vault.ExcitechOperations.GetObjectFileFromPath(imageDefFullPath).ID
                Catch ex As Exception
                    Return
                End Try
            End If
        End Using

        m_isValid = True
    End Sub

    Private Sub initAcDbPdfDefinition()

        Using trans As Transaction = m_dwgDatabase.TransactionManager.StartTransaction
            Dim pdfDictKey As String = UnderlayDefinition.GetDictionaryKey(GetType(PdfDefinition))
            Dim namedObjectDict As DBDictionary = m_dwgDatabase.NamedObjectsDictionaryId.GetObject(OpenMode.ForRead)
            Dim pdfDefDict As DBDictionary = trans.GetObject(namedObjectDict.GetAt(pdfDictKey), OpenMode.ForRead)
            Dim pdfDef As PdfDefinition = trans.GetObject(m_acObjId, OpenMode.ForRead)
            m_referenceName = pdfDefDict.NameAt(m_acObjId)

            'get PDF Definition Full Path, Title and Extension
            If Not pdfDef.Loaded Then Return
            Dim pdfDefFullPath As String = pdfDef.ActiveFileName

            'initalise Version Status
            m_versionStatus = g_clientApplication.GetObjectVersionStatus(pdfDefFullPath)

            'if reference is in M-Files then 
            If m_versionStatus <> ObjectVersionStatuses.NotInMFiles Then
                Dim objVerProps = g_clientApplication.FindObjectVersionAndProperties(pdfDefFullPath, False)
                Dim vault = objVerProps.Vault
                m_namedValues = vault.ObjectPropertyOperations.GetPropertiesForMetadataSync(objVerProps.ObjVer, MFMetadataSyncFormat.MFMetadataSyncFormatPowerPoint)
                m_objVer = objVerProps.ObjVer
                m_isArchived = g_clientApplication.isArchived(pdfDefFullPath)

                Try
                    m_fileID = vault.ExcitechOperations.GetObjectFileFromPath(pdfDefFullPath).ID
                Catch ex As Exception
                    Return
                End Try
            End If
        End Using

        m_isValid = True
    End Sub
#End Region

#Region "Helper Methods"

    Public Function GetMFilesURLForFile() As String

        Return m_vault.ObjectOperations.GetMFilesURLForObjectOrFile(m_objVer.ObjID, m_objVer.Version, True, m_fileID, MFilesURLType.MFilesURLTypeShow)
    End Function

    Public Sub RepathReference(Version As Integer)
        Dim acXrefIdCol As New ObjectIdCollection

        Using trans As Transaction = m_dwgDatabase.TransactionManager.StartTransaction
            m_document.LockDocument()

            'get database Object
            Dim dbObj As DBObject = trans.GetObject(m_acObjId, OpenMode.ForWrite)

            Select Case dbObj.GetType
                Case GetType(BlockTableRecord)
                    Dim blkTabRec As BlockTableRecord = dbObj
                    acXrefIdCol.Add(blkTabRec.Id)

                    'get file extension
                    Dim fileExtension = Path.GetExtension(blkTabRec.PathName)

                    'get version specific path
                    Dim versionSpecificPath = m_vault.getVersionSpecificPath(ObjID, Version, fileExtension)

                    'make relative and update saved path
                    blkTabRec.PathName = MakeRelativePath(m_dwgDatabase.OriginalFileName, versionSpecificPath)
                Case GetType(RasterImageDef)
                    Dim imgDef As RasterImageDef = dbObj

                    'get file extension
                    Dim fileExtension = Path.GetExtension(imgDef.SourceFileName)

                    'get version specific path
                    Dim versionSpecificPath = m_vault.getVersionSpecificPath(ObjID, Version, fileExtension)

                    'try repathing to version specific relative path
                    Try
                        imgDef.SourceFileName = MakeRelativePath(m_dwgDatabase.OriginalFileName, versionSpecificPath)
                    Catch ex As Exception
                    End Try

                    'reload from selected path
                    Try
                        imgDef.ActiveFileName = versionSpecificPath
                        imgDef.Load()
                    Catch ex As Exception
                    End Try

                Case GetType(PdfDefinition)
                    Dim pdfDef As PdfDefinition = dbObj

                    'get file extension
                    Dim fileExtension = Path.GetExtension(pdfDef.SourceFileName)

                    'get version specific path
                    Dim versionSpecificPath = m_vault.getVersionSpecificPath(ObjID, Version, fileExtension)

                    'make relative and update saved path
                    pdfDef.SourceFileName = MakeRelativePath(m_dwgDatabase.OriginalFileName, versionSpecificPath)
                    pdfDef.Load("")
                Case Else
                    Return
            End Select

            trans.Commit()
        End Using

        'reload Xrefs
        If acXrefIdCol.Count > 0 Then m_dwgDatabase.ReloadXrefs(acXrefIdCol)
    End Sub
#End Region
End Class
