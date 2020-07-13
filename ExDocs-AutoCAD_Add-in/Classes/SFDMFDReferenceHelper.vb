Imports System.Text.RegularExpressions
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports System.IO

Public Class SFDMFDReferenceHelper

#Region "Instance Variables"
    Private m_document As Document
    Private m_dwgDatabase As Database
    Private m_vault As Vault
#End Region

    Sub New(Document As Document)

        m_document = Document
        m_dwgDatabase = document.Database

    End Sub


    Public Sub Repath()

        If Not g_clientApplication.IsObjectPathInMFiles(m_dwgDatabase.OriginalFileName) Then Return

        Dim objVerProps As ObjectVersionAndProperties = g_clientApplication.FindObjectVersionAndProperties(m_dwgDatabase.OriginalFileName)
        m_vault = objVerProps.Vault
        Dim objID As ObjID = objVerProps.ObjVer.ObjID
        If Not m_vault.ObjectOperations.IsObjectCheckedOutToThisUserOnThisComputer(objID) Then Return

        'process DWG refs
        processBlockTableRecords()

        'process Images refs
        processAcDbRasterImageDef()

        'process PDF refs
        processAcDbPdfDefinition()

        'regen drawng
        m_document.Editor.Regen()

    End Sub

    Private Sub processBlockTableRecords()

        Dim acXrefIdCol As New ObjectIdCollection
        Dim xrefGraph = m_dwgDatabase.GetHostDwgXrefGraph(False)


        Using trans As Transaction = m_dwgDatabase.TransactionManager.StartTransaction
            m_document.LockDocument()

            'check for out of date Xrefs
            Dim blkTable As BlockTable = trans.GetObject(m_dwgDatabase.BlockTableId, OpenMode.ForRead)
            For Each objectId As ObjectId In blkTable

                Try
                    Dim blkTabRec As BlockTableRecord = trans.GetObject(objectId, OpenMode.ForWrite)
                    If blkTabRec.XrefStatus = XrefStatus.NotAnXref Then Continue For

                    'if nested, then skip reference
                    Dim xrefNode = xrefGraph.GetXrefNode(objectId)
                    If xrefNode.IsNested Then Continue For

                    'no need to update valid reference paths
                    If File.Exists(blkTabRec.PathName) Then Continue For

                    'get XREF Definition Full Path
                    Dim xrefDatabase = blkTabRec.GetXrefDatabase(False)
                    If xrefDatabase IsNot Nothing Then Continue For

                    'get updated path and repath reference
                    Dim newRefPath = getUpdatedReferencePath(blkTabRec.PathName)
                    If newRefPath Is Nothing Then Continue For

                    blkTabRec.PathName = MakeRelativePath(m_dwgDatabase.OriginalFileName, newRefPath)
                    acXrefIdCol.Add(blkTabRec.Id)
                Catch ex As Exception
                End Try

            Next

            trans.Commit()
        End Using

        'reload Xrefs
        If acXrefIdCol.Count > 0 Then m_dwgDatabase.ReloadXrefs(acXrefIdCol)
    End Sub

    Private Sub processAcDbRasterImageDef()


        Using trans As Transaction = m_dwgDatabase.TransactionManager.StartTransaction
            Dim imageDictID = RasterImageDef.GetImageDictionary(m_dwgDatabase)
            If imageDictID.IsNull Then Return

            Dim imageDefDict As DBDictionary = trans.GetObject(imageDictID, OpenMode.ForRead)
            For Each dictEntry As DBDictionaryEntry In imageDefDict
                Dim objectId = dictEntry.Value

                'repath Raster Image Definition with version specific path
                Dim rasterImgDef As RasterImageDef = trans.GetObject(objectId, OpenMode.ForWrite)

                'no need to update loaded image defs
                If rasterImgDef.IsLoaded Then Continue For

                'get updated path and repath reference
                Dim newRefPath = getUpdatedReferencePath(rasterImgDef.SourceFileName)
                If newRefPath Is Nothing Then Continue For

                Try
                    rasterImgDef.SourceFileName = MakeRelativePath(m_dwgDatabase.OriginalFileName, newRefPath)
                Catch ex As Exception
                End Try

                'reload from selected path
                Try
                    rasterImgDef.ActiveFileName = newRefPath
                    rasterImgDef.Load()
                Catch ex As Exception
                End Try
            Next

            trans.Commit()
        End Using

    End Sub

    Private Sub processAcDbPdfDefinition()

        Using trans As Transaction = m_dwgDatabase.TransactionManager.StartTransaction
            Dim pdfDictKey As String = UnderlayDefinition.GetDictionaryKey(GetType(PdfDefinition))
            Dim namedObjectDict As DBDictionary = m_dwgDatabase.NamedObjectsDictionaryId.GetObject(OpenMode.ForRead)
            If Not namedObjectDict.Contains(pdfDictKey) Then Return
            Dim pdfDefDict As DBDictionary = trans.GetObject(namedObjectDict.GetAt(pdfDictKey), OpenMode.ForRead)

            For Each dictEntry As DBDictionaryEntry In pdfDefDict
                Dim objectId = dictEntry.Value

                Dim pdfDef As PdfDefinition
                Try
                    'repath PDF Definition with version specific path
                    pdfDef = trans.GetObject(objectId, OpenMode.ForWrite)
                Catch ex As Exception
                    Continue For
                End Try

                'no need to update loaded pdf defs
                If pdfDef.Loaded Then Continue For

                'get updated path and repath reference
                Dim newRefPath = getUpdatedReferencePath(pdfDef.SourceFileName)
                If newRefPath Is Nothing Then Continue For

                Try
                    pdfDef.SourceFileName = MakeRelativePath(m_dwgDatabase.OriginalFileName, newRefPath)
                    pdfDef.Load("")
                Catch ex As Exception
                    Continue For
                End Try

            Next

            trans.Commit()
        End Using

    End Sub

    Private Function getUpdatedReferencePath(ReferencePath As String) As String

        'get Object ID from Reference Path
        Dim rgx As New Regex(" \(ID [0-9]+\)[.\\]", RegexOptions.IgnoreCase)
        If Not rgx.IsMatch(ReferencePath) Then Return Nothing

        Dim idSubString = rgx.Match(ReferencePath).Value

        'get Reference name without
        rgx = New Regex(" \(ID [0-9]+\)", RegexOptions.IgnoreCase)
        Dim idSuffix = rgx.Match(ReferencePath).Value
        Dim baseFilename = rgx.Replace(Path.GetFileNameWithoutExtension(ReferencePath), "")
        Dim rootPath = rgx.Replace(Path.GetDirectoryName(ReferencePath), "")
        Dim fileExt = Path.GetExtension(ReferencePath)
        Dim newFilePath As String

        'if idSubString ends in '.' then SFD path
        If idSubString.EndsWith(".") Then
            newFilePath = Path.Combine(rootPath, baseFilename + idSuffix, baseFilename + fileExt)
        ElseIf idSubString.EndsWith("\") Then
            newFilePath = rootPath + idSuffix + fileExt
        Else
            Return Nothing
        End If

        'if not absolute path, then convert to absolute
        If Not Path.IsPathRooted(newFilePath) Then
            Dim basePath = Path.GetDirectoryName(m_dwgDatabase.Filename)
            newFilePath = Path.Combine(basePath, newFilePath)
            newFilePath = Path.GetFullPath(newFilePath)
        End If

        If File.Exists(newFilePath) Then Return newFilePath

        Return Nothing

    End Function

    'Private Function getUpdatedReferencePath2(ReferencePath As String) As String

    '    Try
    '        Dim intObjId As Integer
    '        Dim intObjVersion As Integer = -1

    '        'get Object ID from Reference Path
    '        Dim rgx As New Regex("\(ID [0-9]+\)[.\\]", RegexOptions.IgnoreCase)
    '        If rgx.IsMatch(ReferencePath) Then
    '            Dim idIdentifer = rgx.Match(ReferencePath).Value
    '            rgx = New Regex("[0-9]+", RegexOptions.IgnoreCase)
    '            intObjId = CInt(rgx.Match(idIdentifer).Value)
    '        Else
    '            Return Nothing
    '        End If

    '        'verify if validate Vault Path
    '        rgx = New Regex("\\" + intObjId.ToString + "\\[LS]\\(?:[L]|v[0-9]+)\\[\w,\s-()]+\(ID " + intObjId.ToString + "\)[.\\]", RegexOptions.IgnoreCase)
    '        If Not rgx.IsMatch(ReferencePath) Then Return Nothing

    '        'is Latest or Version Specific
    '        rgx = New Regex("\\S\\v[0-9]+\\", RegexOptions.IgnoreCase)
    '        If rgx.IsMatch(ReferencePath) Then
    '            Dim versionIdentifer = rgx.Match(ReferencePath).Value
    '            rgx = New Regex("[0-9]+", RegexOptions.IgnoreCase)
    '            intObjVersion = CInt(rgx.Match(versionIdentifer).Value)
    '        End If

    '        'create ObjVer object
    '        Dim objVer As New ObjVer
    '        objVer.SetIDs(MFBuiltInObjectType.MFBuiltInObjectTypeDocument, intObjId, intObjVersion)

    '        'get object with ID and Version from Vault
    '        Dim objVersion = m_vault.ObjectOperations.GetObjectInfo(objVer, False, False)

    '        'get Reference name without ID, if present
    '        Dim baseFilename As String = Path.GetFileNameWithoutExtension(ReferencePath)
    '        rgx = New Regex(" \(ID [0-9]+\)$", RegexOptions.IgnoreCase)
    '        baseFilename = rgx.Replace(baseFilename, "")

    '        'find file
    '        Dim objFile = objVersion.Files.GetObjectFileByName(baseFilename, Path.GetExtension(ReferencePath))
    '        If objFile Is Nothing Then Throw New Exception("Cannot locate Object File")

    '        'get updated path, if version is not -1 then make path version specific
    '        Dim updatedRefPath As String
    '        If intObjVersion <> -1 Then
    '            updatedRefPath = m_vault.ObjectFileOperations.GetPathInDefaultView(objVersion.ObjVer.ObjID, objVersion.ObjVer.Version, objFile.ID, objFile.Version, MFLatestSpecificBehavior.MFLatestSpecificBehaviorSpecific)
    '        Else
    '            updatedRefPath = m_vault.ObjectFileOperations.GetPathInDefaultView(objVersion.ObjVer.ObjID, objVersion.ObjVer.Version, objFile.ID, objFile.Version, MFLatestSpecificBehavior.MFLatestSpecificBehaviorLatest)
    '        End If

    '        'return updated path
    '        Return updatedRefPath

    '    Catch ex As Exception
    '    End Try

    '    Return Nothing

    'End Function

End Class
