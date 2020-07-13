Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput

Public Class AutoCADHelper

    'Public Shared Sub ExportLayouts(layoutName As String, exportFileName As String)
    '    Dim doc As Document = Application.DocumentManager.MdiActiveDocument
    '    Dim db As Database = doc.Database
    '    Dim ed As Editor = doc.Editor
    '    Dim format As String = IO.Path.Combine("c:\temp\", IO.Path.GetFileNameWithoutExtension(doc.Name)) + "_{0}.dwg"

    '    Using doc.LockDocument

    '        Dim kvp As KeyValuePair(Of String, ObjectId) = GetLayoutDetails(layoutName, doc)

    '        ''AutoCAD export engine
    '        Dim engine As Autodesk.AutoCAD.ExportLayout.Engine = Autodesk.AutoCAD.ExportLayout.Engine.Instance
    '        Dim old As String = Application.GetSystemVariable("CTAB")

    '        'Dim filename As String = String.Format(format, kvp.Key)
    '        Dim filename As String = exportFileName
    '        Application.SetSystemVariable("CTAB", kvp.Key)

    '        Using database As Database = engine.ExportLayout(kvp.Value)
    '            If engine.EngineStatus = Autodesk.AutoCAD.ExportLayout.ErrorStatus.Succeeded Then
    '                database.SaveAs(filename, DwgVersion.Newest)
    '            Else
    '                Throw New Exception("Unable to export layout")
    '            End If
    '        End Using

    '        If Not old Is Nothing Then
    '            Application.SetSystemVariable("CTAB", old)
    '        End If

    '    End Using
    'End Sub

    Public Shared Function GetLayoutDetails(layoutName As String, doc As Document) As KeyValuePair(Of String, ObjectId)
        If String.IsNullOrWhiteSpace(layoutName) Then Return Nothing
        Dim db As Database = doc.Database

        Using tr As Transaction = doc.TransactionManager.StartTransaction
            Dim dLayouts As DBDictionary = tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead)
            If dLayouts.Contains(layoutName) Then

                Dim id As ObjectId = dLayouts.GetAt(layoutName)

                Return New KeyValuePair(Of String, ObjectId)(layoutName, id)
            End If
        End Using

        Return Nothing
    End Function


    Public Shared Function GetLayersWithPrefix(db As Database) As List(Of String)
        Dim ret As New List(Of String)

        Using tr As Transaction = db.TransactionManager.StartTransaction()
            ''get the layout dictionary
            Dim lt As LayerTable = tr.GetObject(db.LayerTableId, OpenMode.ForRead)
            For Each lID As ObjectId In lt
                Dim ltr As LayerTableRecord = tr.GetObject(lID, OpenMode.ForRead)
                If ltr.Name.ToUpperInvariant.StartsWith(My.Settings.LayerPrefix.ToUpperInvariant) Then ret.Add(ltr.Name)
            Next
        End Using

        Return ret
    End Function

    'Public Shared Sub ExportLayouts()
    '    Dim doc As Document = Application.DocumentManager.MdiActiveDocument
    '    Dim db As Database = doc.Database
    '    Dim ed As Editor = doc.Editor

    '    Dim format As String = IO.Path.Combine(IO.Path.GetDirectoryName(doc.Name), IO.Path.GetFileNameWithoutExtension(doc.Name)) + "_{0}.dwg"

    '    Dim layouts As New Dictionary(Of String, ObjectId)

    '    Using doc.LockDocument
    '        Using tr As Transaction = doc.TransactionManager.StartTransaction
    '            Dim btr As BlockTableRecord = SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject(OpenMode.ForRead)
    '            Dim layout As Layout = btr.LayoutId.GetObject(OpenMode.ForRead)
    '            Dim model As String = layout.LayoutName

    '            ''open the layout dictionary
    '            Dim layoutdictionary As IDictionary = db.LayoutDictionaryId.GetObject(OpenMode.ForRead)

    '            ''get the names and ids of all paper space layouts
    '            layouts = layoutdictionary.Cast(Of DictionaryEntry).Where(Function(e) Not e.Key.ToString = model).ToDictionary(Of String, ObjectId)(Function(e) e.Key.ToString, Function(e) e.Value)

    '            tr.Commit()
    '        End Using

    '        ''AutoCAD export engine
    '        Dim engine As Autodesk.AutoCAD.ExportLayout.Engine = Autodesk.AutoCAD.ExportLayout.Engine.Instance
    '        Dim old As String = Application.GetSystemVariable("CTAB")

    '        For Each entry In layouts
    '            Dim filename As String = String.Format(format, entry.Key)
    '            Debug.WriteLine("Exporting: " + filename)
    '            Application.SetSystemVariable("CTAB", entry.Key)

    '            Using database As Database = engine.ExportLayout(entry.Value)
    '                If engine.EngineStatus = Autodesk.AutoCAD.ExportLayout.ErrorStatus.Succeeded Then
    '                    database.SaveAs(filename, DwgVersion.Newest)
    '                Else
    '                    Debug.Write("Failed to export")
    '                End If
    '            End Using
    '        Next
    '        If Not old Is Nothing Then
    '            Application.SetSystemVariable("CTAB", old)
    '        End If

    '    End Using


    'End Sub

    Public Shared Sub AddLayoutUniqueIDXData(objectID As ObjectId, uniqueID As String)
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database

        Using tr As Transaction = db.TransactionManager.StartTransaction
            Using doc.LockDocument
                Dim ent As Entity = tr.GetObject(objectID, OpenMode.ForWrite) ''open the entity for write
                Dim regTable As RegAppTable = tr.GetObject(db.RegAppTableId, OpenMode.ForRead) ''open the registerd app table

                If Not regTable.Has("EXDOCS") Then
                    regTable.UpgradeOpen()

                    ''add the appliction name
                    Dim app As New RegAppTableRecord
                    app.Name = "EXDOCS"
                    regTable.Add(app)
                    tr.AddNewlyCreatedDBObject(app, True)
                End If

                ''append the xdata
                ent.XData = New ResultBuffer(New TypedValue(DxfCode.ExtendedDataRegAppName, "EXDOCS"),
    New TypedValue(DxfCode.ExtendedDataAsciiString, uniqueID))


            End Using

            tr.Commit()
        End Using
    End Sub

    Public Shared Sub RemoveLayoutUniqueIDXData(objectID As ObjectId)

    End Sub

    Public Shared Function GetLayoutUniqueIDXData(objectID As ObjectId) As String
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database

        Using tr As Transaction = db.TransactionManager.StartTransaction
            Using doc.LockDocument
                Dim ent As Entity = tr.GetObject(objectID, OpenMode.ForWrite) ''open the entity for write
                Dim regTable As RegAppTable = tr.GetObject(db.RegAppTableId, OpenMode.ForRead) ''open the registerd app table

                If Not regTable.Has("EXDOCS") Then Return ""
                If ent.XData Is Nothing Then Return ""

                Dim _xdata As TypedValue() = ent.XData.AsArray

                If _xdata.Count - 1 < 1 Then Return ""
                Return _xdata(1).Value.ToString
            End Using



            'If Not regTable.Has("EXDOCS") Then
            '    regTable.UpgradeOpen()

            '    ''add the appliction name
            '    Dim app As New RegAppTableRecord
            '    app.Name = "EXDOCS"
            '    regTable.Add(app)
            '    tr.AddNewlyCreatedDBObject(app, True)
            'End If

            '            ''append the xdata
            '            ent.XData = New ResultBuffer(New TypedValue(DxfCode.ExtendedDataRegAppName, "EXDOCS"),
            'New TypedValue(DxfCode.ExtendedDataControlString, uniqueID))

        End Using
    End Function

    Public Shared Sub DeleteAllLayouts(keepLayouts As List(Of String))
        If keepLayouts Is Nothing Then Exit Sub
        If keepLayouts.Count <= 0 Then Exit Sub

        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim db As Database = HostApplicationServices.WorkingDatabase

        Using tr As Transaction = db.TransactionManager.StartTransaction
            Using doc.LockDocument
                Dim layoutDCT As DBDictionary = tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead)
                For Each de As DBDictionaryEntry In layoutDCT
                    Dim lName As String = de.Key

                    If keepLayouts.Contains(lName, StringComparer.InvariantCultureIgnoreCase) Then Continue For

                    LayoutManager.Current.DeleteLayout(lName)

                Next
            End Using

        End Using

    End Sub

    Public Shared Sub EraseLayout(layoutName As String)
        If String.IsNullOrWhiteSpace(layoutName) Then Exit Sub

        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database
        Dim ed As Editor = doc.Editor

        Using tr As Transaction = db.TransactionManager.StartTransaction
            Using doc.LockDocument
                Dim layoutDCT As DBDictionary = tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead)
                For Each de As DBDictionaryEntry In layoutDCT
                    If de.Key = layoutName Then
                        LayoutManager.Current.DeleteLayout(layoutName)
                        Exit For
                    End If
                Next
            End Using

        End Using

    End Sub


    Public Shared Function GetLayersWithPrefixFromFile(filename As String) As List(Of String)
        If String.IsNullOrWhiteSpace(filename) Then Return New List(Of String)
        Dim _ret As New List(Of String)

        Using db As New Database(False, True)
            db.ReadDwgFile(filename, FileOpenMode.OpenForReadAndAllShare, True, "")
            db.CloseInput(True)
            Using tr As Transaction = db.TransactionManager.StartTransaction()
                ''get the layout dictionary
                Dim lt As LayerTable = tr.GetObject(db.LayerTableId, OpenMode.ForRead)
                For Each lID As ObjectId In lt
                    Dim ltr As LayerTableRecord = tr.GetObject(lID, OpenMode.ForRead)
                    If ltr.Name.ToUpperInvariant.StartsWith(My.Settings.LayerPrefix.ToUpperInvariant) Then _ret.Add(ltr.Name)
                Next
            End Using
            db.Dispose()
        End Using

        Return _ret
    End Function

    Public Shared Function GetLayoutNamesFromFile(filename As String) As List(Of String)
        If String.IsNullOrWhiteSpace(filename) Then Return New List(Of String)

        Dim _ret As New List(Of String)

        Using dwtDB As New Database(False, True)
            dwtDB.ReadDwgFile(filename, FileOpenMode.OpenForReadAndAllShare, True, "")

            Using tr As Transaction = dwtDB.TransactionManager.StartTransaction()
                ''get the layout dictionary
                Dim layoutDCT As DBDictionary = tr.GetObject(dwtDB.LayoutDictionaryId, OpenMode.ForRead)
                For Each de As DBDictionaryEntry In layoutDCT
                    If Not de.Key.ToUpper = "MODEL" Then _ret.Add(de.Key)
                Next
            End Using
            dwtDB.Dispose()
        End Using

        Return _ret
    End Function
End Class
