Imports MFilesAPI

Public Class Form1

    Private m_vault As Vault
    Private m_clientApp As New MFilesClientApplication

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim vaultCN = m_clientApp.GetVaultConnection("HESIMM")
        m_vault = vaultCN.BindToVault(Me.Handle, True, False)


        ''create a new object with automatic name property
        Dim objClass = m_vault.ClassOperations.GetObjectClass(68)
        Dim properties As New PropertyValues

        'set Class
        Dim propertyValue As New PropertyValue
        propertyValue.PropertyDef = MFBuiltInPropertyDef.MFBuiltInPropertyDefClass
        propertyValue.Value.SetValue(MFDataType.MFDatatypeLookup, objClass.ID)
        properties.Add(-1, propertyValue)

        ''set Is Template (ignores automatic values)
        'propertyValue = New PropertyValue
        'propertyValue.PropertyDef = MFBuiltInPropertyDef.MFBuiltInPropertyDefIsTemplate
        '    propertyValue.Value.SetValue(MFDataType.MFDatatypeBoolean, True)
        '    properties.Add(-1, propertyValue)

        ''add Name

        propertyValue = New PropertyValue
        propertyValue.PropertyDef = objClass.NamePropertyDef
        propertyValue.Value.SetValue(MFDataType.MFDatatypeText, "Foo")
        properties.Add(-1, propertyValue)

        Dim objVerProps As ObjectVersionAndProperties

        objVerProps = m_vault.ObjectOperations.CreateNewObject(objClass.ObjectType, properties)

        'remove IsTemplate property
        'objVerProps.Properties.Remove(2)

        ''    m_vault.ObjectPropertyOperations.SetAllProperties(objVerProps.ObjVer, True, objVerProps.Properties)

        m_vault.ObjectOperations.CheckIn(objVerProps.ObjVer)
    End Sub
End Class
