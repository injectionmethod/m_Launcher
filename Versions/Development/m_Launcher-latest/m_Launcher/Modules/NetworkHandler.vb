Imports System.Net
Imports System.Net.NetworkInformation

Module NetworkHandler
    Public Function DisplayProcessConnections(ByVal processName As String) As List(Of String)
        Dim connectionStrings As New List(Of String)
        Dim processIds() As Integer = Process.GetProcessesByName(processName).Select(Function(p) p.Id).ToArray()

        For Each processId As Integer In processIds
            Dim tcpConnections As IPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties()
            Dim connections As TcpConnectionInformation() = tcpConnections.GetActiveTcpConnections()

            For Each connection As TcpConnectionInformation In connections
                If connection.State = TcpState.Established AndAlso processId = processId Then
                    Dim localEndPoint As IPEndPoint = connection.LocalEndPoint
                    Dim remoteEndPoint As IPEndPoint = connection.RemoteEndPoint
                    Dim connectionString As String = $"{processName}: {localEndPoint.Address}:{localEndPoint.Port} -> {remoteEndPoint.Address}:{remoteEndPoint.Port}"
                    connectionStrings.Add(connectionString)
                End If
            Next
        Next
        If connectionStrings.Count < 1 Then
            connectionStrings.Add(processName + ": Nothing Found")
            Return connectionStrings
        Else
            Return connectionStrings
        End If
    End Function
End Module
