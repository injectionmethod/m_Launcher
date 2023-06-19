Imports System.IO
Module ProcessHandler
    Public NextProcessRunAsAdmin As Boolean = False
    Public NextLaunchInternally As Boolean = False
    Public NextWaitForLeave As Boolean = False
    Public Function PerformProcessStart(filename As String, Hidden As Boolean, args As String)
        Dim Ran As Boolean = False
        Dim Arguments As String = args

        Try
            For Each arguments_file In IO.Directory.GetFiles(Form1.FileReadList(4))
                If arguments_file.Contains(IO.Path.GetFileNameWithoutExtension(filename)) Then
                    Arguments = IO.File.ReadAllText(arguments_file)
                End If
            Next
        Catch ex As Exception
            WriteToLog(ex)
        End Try

        Using StartTheProcess As New Process

            If Hidden = True Then
                StartTheProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            End If

            If NextProcessRunAsAdmin = True Then
                StartTheProcess.StartInfo.Verb = "runAs"
                NextProcessRunAsAdmin = False
            End If

            If NextLaunchInternally = True Then
                NextLaunchInternally = False
                StartTheProcess.StartInfo.WorkingDirectory = Environment.CurrentDirectory
                StartTheProcess.StartInfo.UseShellExecute = True
            Else
                StartTheProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(filename)
                StartTheProcess.StartInfo.UseShellExecute = False
            End If

            StartTheProcess.StartInfo.FileName = filename
            StartTheProcess.StartInfo.Arguments = Arguments
            Try
                StartTheProcess.Start()
            Catch ex As Exception
                WriteToLog(ex)
                MsgBox(ex.ToString)
            End Try

            If NextWaitForLeave = True Then
                StartTheProcess.WaitForExit()
                NextWaitForLeave = False
            End If
            Ran += 1
        End Using
        Return Ran
    End Function
End Module
