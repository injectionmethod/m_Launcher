Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Text

Public Module MemoryHandling

    Public Enum ProcessAccessFlags As UInteger
        PROCESS_TERMINATE = &H1
        PROCESS_CREATE_THREAD = &H2
        PROCESS_SET_SESSIONID = &H4
        PROCESS_VM_OPERATION = &H8
        PROCESS_VM_READ = &H10
        PROCESS_VM_WRITE = &H20
        PROCESS_DUP_HANDLE = &H40
        PROCESS_CREATE_PROCESS = &H80
        PROCESS_SET_QUOTA = &H100
        PROCESS_SET_INFORMATION = &H200
        PROCESS_QUERY_INFORMATION = &H400
        PROCESS_SUSPEND_RESUME = &H800
        PROCESS_ALL_ACCESS = &H1F0FFF
    End Enum

    Public Enum MemoryProtectionConstants As UInteger
        PAGE_NOACCESS = &H1
        PAGE_READONLY = &H2
        PAGE_READWRITE = &H4
        PAGE_WRITECOPY = &H8
        PAGE_EXECUTE = &H10
        PAGE_EXECUTE_READ = &H20
        PAGE_EXECUTE_READWRITE = &H40
        PAGE_EXECUTE_WRITECOPY = &H80
        PAGE_GUARD = &H100
        PAGE_NOCACHE = &H200
        PAGE_WRITECOMBINE = &H400
    End Enum

    <StructLayout(LayoutKind.Sequential)>
    Public Structure MEMORY_BASIC_INFORMATION
        Public BaseAddress As IntPtr
        Public AllocationBase As IntPtr
        Public AllocationProtect As UInteger
        Public RegionSize As IntPtr
        Public State As UInteger
        Public Protect As UInteger
        Public Type As UInteger
    End Structure

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function OpenProcess(ByVal dwDesiredAccess As ProcessAccessFlags, ByVal bInheritHandle As Boolean, ByVal dwProcessId As UInteger) As IntPtr
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function CloseHandle(ByVal hObject As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function ReadProcessMemory(ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, <Out()> ByVal lpBuffer As Byte(), ByVal nSize As Integer, <Out()> ByRef lpNumberOfBytesRead As Integer) As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function WriteProcessMemory(ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer As Byte(), ByVal nSize As Integer, <Out()> ByRef lpNumberOfBytesWritten As Integer) As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function VirtualQueryEx(ByVal hProcess As IntPtr, ByVal lpAddress As IntPtr, <Out()> ByRef lpBuffer As MEMORY_BASIC_INFORMATION, ByVal dwLength As UInteger) As UInteger
    End Function

    Public Function DoReadMemory(ByVal processName As String, ByVal address As IntPtr, ByVal size As Integer) As Byte()
        Dim processHandle As IntPtr = IntPtr.Zero
        Dim bytesRead As Integer = 0
        Dim buffer(size - 1) As Byte
        Dim proc As Process = Process.GetProcessesByName(processName)(0)

        processHandle = OpenProcess(ProcessAccessFlags.PROCESS_VM_READ, False, proc.Id)
        ReadProcessMemory(processHandle, address, buffer, size, bytesRead)
        CloseHandle(processHandle)

        Return buffer
    End Function

    Public Function FindAddresses(ByVal processName As String) As List(Of IntPtr)
        Dim addressList As New List(Of IntPtr)
        Try
            Dim proc = Process.GetProcessesByName(processName)(0)
            Dim address As IntPtr = proc.MainModule.BaseAddress
            Dim memoryInfo As MEMORY_BASIC_INFORMATION
            While VirtualQueryEx(proc.Handle, address, memoryInfo, CType(Marshal.SizeOf(memoryInfo), UInteger)) <> 0
                If memoryInfo.Protect <> MemoryProtectionConstants.PAGE_NOACCESS AndAlso memoryInfo.State = &H1000 Then
                    addressList.Add(address)
                End If

                address = New IntPtr(address.ToInt64() + memoryInfo.RegionSize.ToInt64())
                Return addressList
            End While
        Catch ex As Exception
            WriteToLog(ex)
            Return addressList
        End Try
    End Function

    Public Function BytesToString(ByVal bytes() As Byte) As String
        Return Encoding.ASCII.GetString(bytes)
    End Function

    Public Function ExportProcessBaseAddressesToString(ByVal processName As String, ByVal size As Integer) As String
        Dim addressList As List(Of IntPtr) = FindAddresses(processName)

        If addressList.Count = 0 Then
            Return "Process {" + processName + "} not found"
        End If

        Dim output As New StringBuilder()
        For Each address In addressList
            Dim data As Byte() = DoReadMemory(processName, address, size)
            Dim strData As String = BitConverter.ToString(data).Replace("-", " ")
            output.AppendLine($"Address: {address} - Data: {strData}" + vbNewLine + vbNewLine)
        Next

        Return output.ToString()
    End Function

    Public Sub WriteMemory(ByVal processName As String, ByVal address As IntPtr, ByVal value As Byte())
        Dim processHandle As IntPtr = IntPtr.Zero
        Dim bytesWritten As Integer = 0
        Dim proc As Process = Process.GetProcessesByName(processName)(0)

        processHandle = OpenProcess(ProcessAccessFlags.PROCESS_VM_WRITE Or ProcessAccessFlags.PROCESS_VM_OPERATION, False, proc.Id)
        WriteProcessMemory(processHandle, address, value, value.Length, bytesWritten)
        CloseHandle(processHandle)
    End Sub

    Public Function FindAddressesWithValue(ByVal processName As String, ByVal value As Integer) As List(Of IntPtr)
        Dim addressList As New List(Of IntPtr)

        Dim proc = Process.GetProcessesByName(processName).FirstOrDefault()
        If proc Is Nothing Then
            Return addressList
        End If

        Dim memoryRanges = FindMemoryRanges(proc)

        For Each range In memoryRanges
            Dim buffer = DoReadMemory(processName, range.BaseAddress, range.RegionSize.ToInt32())
            Dim valueAddresses = FindValueAddresses(buffer, value, range.BaseAddress)

            addressList.AddRange(valueAddresses)
        Next

        Return addressList
    End Function

    Private Function FindMemoryRanges(ByVal process As Process) As List(Of MEMORY_BASIC_INFORMATION)
        Dim memoryRanges As New List(Of MEMORY_BASIC_INFORMATION)()

        Dim address As IntPtr = process.MainModule.BaseAddress
        Dim memoryInfo As MEMORY_BASIC_INFORMATION

        While VirtualQueryEx(process.Handle, address, memoryInfo, CType(Marshal.SizeOf(memoryInfo), UInteger)) <> 0
            If memoryInfo.Protect <> MemoryProtectionConstants.PAGE_NOACCESS AndAlso memoryInfo.State = &H1000 Then
                memoryRanges.Add(memoryInfo)
            End If

            address = New IntPtr(address.ToInt64() + memoryInfo.RegionSize.ToInt64())
        End While

        Return memoryRanges
    End Function

    Private Function FindValueAddresses(ByVal buffer As Byte(), ByVal value As Integer, ByVal baseAddress As IntPtr) As List(Of IntPtr)
        Dim valueAddresses As New List(Of IntPtr)()

        Dim dataSize As Integer = buffer.Length
        For i As Integer = 0 To dataSize - 4 Step 4 ' Step 4 for Int32
            Dim intValue As Integer = BitConverter.ToInt32(buffer, i)
            If intValue = value Then
                Dim offset As Integer = i
                Dim address As IntPtr = New IntPtr(baseAddress.ToInt64() + offset)
                valueAddresses.Add(address)
            End If
        Next

        Return valueAddresses
    End Function
End Module