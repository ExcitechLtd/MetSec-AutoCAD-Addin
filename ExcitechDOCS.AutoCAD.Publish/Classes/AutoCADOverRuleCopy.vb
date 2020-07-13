Imports Autodesk.AutoCAD.DatabaseServices

Public Class AutoCADOverRuleCopy
    Inherits ObjectOverrule



    Public Overrides Function DeepClone(dbObject As DBObject, ownerObject As DBObject, idMap As IdMapping, isPrimary As Boolean) As DBObject
        'Return MyBase.DeepClone(dbObject, ownerObject, idMap, isPrimary)

        Dim ret As DBObject = MyBase.DeepClone(dbObject, ownerObject, idMap, isPrimary)

        ''check for our Xdata
        Using tr As Transaction = dbObject.Database.TransactionManager.StartTransaction
            Dim ent As Entity = tr.GetObject(ret.ObjectId, OpenMode.ForRead)
            Dim xdata As ResultBuffer = ent.GetXDataForApplication("EXDOCS")

            If Not xdata Is Nothing Then
                ent.UpgradeOpen()
                ent.XData = New ResultBuffer(New TypedValue(DxfCode.ExtendedDataRegAppName, "EXDOCS"))
                xdata.Dispose()
            End If

            tr.Commit()
        End Using

        Return ret
    End Function
End Class
