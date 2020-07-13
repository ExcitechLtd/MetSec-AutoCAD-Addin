Imports System.Reflection

Public Class PluginHelper

    Public Shared Function ResolveAsmReference(sender As Object, args As ResolveEventArgs) As Assembly
        Dim folderPath As String = IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        Dim assemblyPath As String = IO.Path.Combine(folderPath, New AssemblyName(args.Name).Name & ".dll")
        If Not IO.File.Exists(assemblyPath) Then Return Nothing
        Dim assembly As Assembly = Assembly.LoadFrom(assemblyPath)
        Return assembly
    End Function

End Class
