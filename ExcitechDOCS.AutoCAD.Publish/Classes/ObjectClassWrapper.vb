Imports MFilesAPI

Public Class ObjectClassWrapper

#Region "Instance Variables"
    Private m_objectClass As ObjectClass
    Private m_objType As ObjType
    Private m_vault As Vault
#End Region

    'constructor
    Public Sub New(ObjectClass As ObjectClass)

        m_objectClass = ObjectClass
    End Sub

    Public Sub New(Vault As Vault, ObjectClassID As Integer)
        m_vault = Vault

        m_objectClass = m_vault.ClassOperations.GetObjectClass(ObjectClassID)
        m_objType = m_vault.ObjectTypeOperations.GetObjectType(m_objectClass.ObjectType)
    End Sub

    Public Sub New(Vault As Vault, ObjectClass As ObjectClass)
        m_vault = Vault
        m_objectClass = ObjectClass
        m_objType = m_vault.ObjectTypeOperations.GetObjectType(ObjectClass.ObjectType)
    End Sub

#Region "Properties"

    Public ReadOnly Property ObjectClass As ObjectClass
        Get
            Return m_objectClass
        End Get
    End Property

    Public ReadOnly Property ObjecType As ObjType
        Get
            Return m_objType
        End Get
    End Property

    Public ReadOnly Property CanHaveFiles As Boolean
        Get
            If Not m_objType Is Nothing Then
                Return m_objType.CanHaveFiles
            End If

            Return False
        End Get
    End Property

#End Region

    Public Overrides Function ToString() As String
        Return m_objectClass.Name + " (" + m_objectClass.ID.ToString + ")"
    End Function

End Class

