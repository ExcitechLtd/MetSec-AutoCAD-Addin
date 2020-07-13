Public Class LoggingHelper

#Region " Private "
    Private Shared _lock As New Object
    Private Shared _fullpath As String
#End Region

#Region " Properties "
    Public Shared ReadOnly Property FullPath As String
        Get
            Return _fullpath
        End Get
    End Property


#End Region

#Region " Methods "
    Public Shared Sub WriteToLog(message As String)
        Try
            Dim logPath As String = Settings.BasePath + "AutoCAD\Log\"
            _fullpath = logPath + Now.ToString("ddMMyyyy") + ".log"

            If Not IO.Directory.Exists(logPath) Then IO.Directory.CreateDirectory(logPath)

            SyncLock _lock
                Using _sw As New IO.StreamWriter(_fullpath, True)
                    _sw.WriteLine(Format(Now, "dd/MM/yyyy HH:mm:ss:ffff") + " - " + message)
                End Using
            End SyncLock


        Catch ex As Exception

        End Try
    End Sub
#End Region
End Class
