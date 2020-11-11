Imports MFilesAPI

Public Class PropertyWrapper

#Region " Properties "
    Public Property IsAutomatic As Boolean

    Public Property IsCalculated As Boolean
    Public Property PropertyID As Integer
    Public Property Value As Object
    Public Property RequiredProperty As Boolean = False

    Public ReadOnly Property PropertyDescription(Vault As Vault) As String
        Get

            Try
                Dim propDef = MFilesPropertyDef(Vault)
                Return propDef.Name + " (" + propDef.ID.ToString + ")"
            Catch ex As Exception
                Return Nothing
            End Try


        End Get
    End Property

    Public ReadOnly Property DataTypeDescription(Vault As Vault) As String
        Get

            Dim dataType = MFilesPropertyDef(Vault).DataType
            Return getDisplayDataType(dataType)
        End Get
    End Property

    Public ReadOnly Property MFilesPropertyDef(Vault As Vault) As PropertyDef
        Get
            Return Vault.PropertyDefOperations.GetPropertyDef(PropertyID)
        End Get
    End Property

    Public ReadOnly Property ValueDescription(Vault As Vault) As String
        Get
            Dim description As String = ""


            If IsNumeric(Value) Then
                Try
                    Dim propDef = MFilesPropertyDef(Vault)
                    Return Vault.ValueListItemOperations.GetValueListItemByID(propDef.ValueList, Value).Name
                Catch ex As Exception
                    Return "Invalid Lookup Value"
                End Try
            Else
                description = Value
                'If description = "" Then description = """"""
                'If description.Trim.Length < description.Length Then description = """" + description + """"
                Return description
                    End If

        End Get
    End Property

    Public Property UseDocumentName As Boolean

    Public Property ValueID As Integer = -1
#End Region

#Region " Constrcutor "
    Public Sub New(PropertyDefID As Integer, Required As Boolean)
        PropertyID = PropertyDefID
        RequiredProperty = Required
        UseDocumentName = False
        IsAutomatic = False
        IsCalculated = False
    End Sub
#End Region

#Region " Methods "
    Private Function getDisplayDataType(dataType As MFDataType) As String

        Select Case dataType
            Case MFDataType.MFDatatypeACL
                Return "ACL"
            Case MFDataType.MFDatatypeBoolean
                Return "Boolean"
            Case MFDataType.MFDatatypeDate
                Return "Date"
            Case MFDataType.MFDatatypeFILETIME
                Return "File Time"
            Case MFDataType.MFDatatypeFloating
                Return "Float"
            Case MFDataType.MFDatatypeInteger, MFDataType.MFDatatypeInteger64
                Return "Integer"
            Case MFDataType.MFDatatypeLookup
                Return "Lookup"
            Case MFDataType.MFDatatypeMultiLineText
                Return "Multi Line Text"
            Case MFDataType.MFDatatypeMultiSelectLookup
                Return "Multi Lookup"
            Case MFDataType.MFDatatypeText
                Return "Text"
            Case MFDataType.MFDatatypeTime
                Return "Time"
            Case MFDataType.MFDatatypeTimestamp
                Return "Time Stamp"
            Case Else
                Return "Unknown"
        End Select
    End Function
#End Region




End Class
