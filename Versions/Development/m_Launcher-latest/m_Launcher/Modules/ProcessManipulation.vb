Imports System.Runtime.InteropServices

'Functions For Process Manipulation, Need To Find Something To Do With This, No Clue Why I Made It
Public Module ProcessManipulation
    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function OpenProcess(ByVal dwDesiredAccess As ProcessAccessFlags, ByVal bInheritHandle As Boolean, ByVal dwProcessId As UInteger) As IntPtr
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function CloseHandle(ByVal hObject As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function TerminateProcess(ByVal hProcess As IntPtr, ByVal uExitCode As UInteger) As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function SuspendThread(ByVal hThread As IntPtr) As UInteger
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function ResumeThread(ByVal hThread As IntPtr) As UInteger
    End Function
    Public Sub TerminateProcessByName(ByVal processName As String)
        Dim processes() As Process = Process.GetProcessesByName(processName)
        For Each process As Process In processes
            Dim processHandle As IntPtr = OpenProcess(ProcessAccessFlags.PROCESS_TERMINATE, False, CUInt(process.Id))
            If processHandle <> IntPtr.Zero Then
                TerminateProcess(processHandle, 0)
                CloseHandle(processHandle)
            End If
        Next
    End Sub

    Public Sub SuspendProcessByName(ByVal processName As String)
        Dim processes() As Process = Process.GetProcessesByName(processName)
        For Each process As Process In processes
            For Each thread As ProcessThread In process.Threads
                Dim threadHandle As IntPtr = OpenProcess(ProcessAccessFlags.PROCESS_SUSPEND_RESUME, False, CUInt(process.Id))
                If threadHandle <> IntPtr.Zero Then
                    SuspendThread(threadHandle)
                    CloseHandle(threadHandle)
                End If
            Next
        Next
    End Sub
    Public Sub ResumeProcessByName(ByVal processName As String)
        Dim processes() As Process = Process.GetProcessesByName(processName)
        For Each process As Process In processes
            For Each thread As ProcessThread In process.Threads
                Dim threadHandle As IntPtr = OpenProcess(ProcessAccessFlags.PROCESS_SUSPEND_RESUME, False, CUInt(process.Id))
                If threadHandle <> IntPtr.Zero Then
                    ResumeThread(threadHandle)
                    CloseHandle(threadHandle)
                End If
            Next
        Next
    End Sub
    Public Function GetProcessByName(ByVal processName As String) As Process
        Dim processes() As Process = Process.GetProcessesByName(processName)
        If processes.Length > 0 Then
            Return processes(0)
        Else
            Return Nothing
        End If
    End Function
    Public Function GetProcessMemoryUsage(ByVal processName As String) As Long
        Dim process As Process = GetProcessByName(processName)
        If process IsNot Nothing Then
            Return process.WorkingSet64
        Else
            Return 0
        End If
    End Function
    Public Sub SetProcessPriority(ByVal processName As String, ByVal priority As ProcessPriorityClass)
        Dim process As Process = GetProcessByName(processName)
        If process IsNot Nothing Then
            process.PriorityClass = priority
        End If
    End Sub
    Public Sub SetProcessAffinity(ByVal processName As String, ByVal affinityMask As IntPtr)
        Dim process As Process = GetProcessByName(processName)
        If process IsNot Nothing Then
            process.ProcessorAffinity = affinityMask
        End If
    End Sub
End Module