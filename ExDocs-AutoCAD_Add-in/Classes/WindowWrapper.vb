Public Class WindowWrapper
    Implements System.Windows.Forms.IWin32Window

    Private m_hwnd As IntPtr

    Public Sub New(handle As IntPtr)
        m_hwnd = handle
    End Sub

    Public ReadOnly Property Handle As IntPtr Implements Windows.Forms.IWin32Window.Handle
        Get
            Return m_hwnd
        End Get
    End Property
End Class