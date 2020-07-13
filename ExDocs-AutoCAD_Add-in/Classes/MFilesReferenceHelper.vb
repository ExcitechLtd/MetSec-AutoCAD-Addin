Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices

Public Class MFilesReferenceHelper

    Private m_vault As Vault
    Private m_objectId As ObjectId

    Public Sub New(ObjectId As ObjectId)

        m_vault = VaultStatus.initialiseMFiles(True)
        m_objectId = ObjectId

    End Sub

    Public Sub Repath()

        'check vault status
        If VaultStatus.Status <> VaultStatus.VaultStatuses.connected Then
            Return
        End If

        'check database is valid
        If m_objectId.Database Is Nothing Then Return

        'lock document
        Dim acDoc = Core.Application.DocumentManager.GetDocument(m_objectId.Database)
        acDoc.LockDocument()

        'process reference based on class type
        Select Case m_objectId.ObjectClass.Name
            Case "AcDbBlockTableRecord"
                processBlockTableRecord()
            Case "AcDbRasterImageDef"
                processAcDbRasterImageDef()
            Case "AcDbPdfDefinition"
                processAcDbPdfDefinition()
        End Select

    End Sub

    Private Sub processBlockTableRecord()

        Dim dwgDatabase = m_objectId.Database
        Dim xrefGraph = dwgDatabase.GetHostDwgXrefGraph(False)
        Dim objCollection As New ObjectIdCollection

        Using trans As Transaction = dwgDatabase.TransactionManager.StartTransaction

            'repath Block Table Record with version specific path
            Dim blkTabRec As BlockTableRecord = trans.GetObject(m_objectId, OpenMode.ForWrite)

            'check if nested the skip
            Dim xrefNode = xrefGraph.GetXrefNode(blkTabRec.Id)
            If xrefNode.IsNested Then Return

            'get Xref Database and Xref Full Path
            Dim xrefDatabase = blkTabRec.GetXrefDatabase(False)
            If xrefDatabase Is Nothing Then Return
            Dim xrefFullPath As String = xrefDatabase.Filename

            'try repathing to version specific relative path
            Try
                Dim versionSpecificPath = m_vault.ExcitechOperations.GetVersionSpecificPath(xrefFullPath)
                blkTabRec.PathName = MakeRelativePath(dwgDatabase.OriginalFileName, versionSpecificPath)
                objCollection.Add(blkTabRec.Id)
            Catch ex As Exception
            End Try

            trans.Commit()
        End Using

        'reload any repathed Xrefs
        If objCollection.Count Then dwgDatabase.ReloadXrefs(objCollection)

    End Sub

    Private Sub processAcDbRasterImageDef()

        Dim dwgDatabase = m_objectId.Database

        Using trans As Transaction = dwgDatabase.TransactionManager.StartTransaction

            'repath Raster Image Definition with version specific path
            Dim rasterImgDef As RasterImageDef = trans.GetObject(m_objectId, OpenMode.ForWrite)
            'get Raster Image Full Path
            Dim rasterImgFullPath As String = rasterImgDef.ActiveFileName
            Dim versionSpecificPath As String = Nothing

            'try repathing to version specific relative path
            Try
                versionSpecificPath = m_vault.ExcitechOperations.GetVersionSpecificPath(rasterImgFullPath)
                rasterImgDef.SourceFileName = MakeRelativePath(dwgDatabase.OriginalFileName, versionSpecificPath)
            Catch ex As Exception
            End Try

            'reload from selected path
            Try
                rasterImgDef.ActiveFileName = versionSpecificPath
                rasterImgDef.Load()
            Catch ex As Exception
            End Try

            trans.Commit()
        End Using

    End Sub

    Private Sub processAcDbPdfDefinition()

        Dim dwgDatabase = m_objectId.Database

        Using trans As Transaction = dwgDatabase.TransactionManager.StartTransaction

            Dim pdfDef As PdfDefinition
            Try
                'repath PDF Definition with version specific path
                pdfDef = trans.GetObject(m_objectId, OpenMode.ForWrite)
            Catch ex As Exception
                Return
            End Try

            'try repathing to version specific relative path
            Try
                'get PDF Definition Full Path
                Dim pdfDefFullPath As String = pdfDef.ActiveFileName
                Dim versionSpecificPath = m_vault.ExcitechOperations.GetVersionSpecificPath(pdfDefFullPath)
                pdfDef.SourceFileName = MakeRelativePath(dwgDatabase.OriginalFileName, versionSpecificPath)
                pdfDef.Load("")
            Catch ex As Exception
                Return
            End Try

            trans.Commit()
        End Using

    End Sub


End Class
