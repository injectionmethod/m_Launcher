Imports System.Net.Http
Imports Microsoft.Win32

Module ConfigHandler
    Dim ConfigEnable = False
    Public TotalGamesInLibraryLimit As Integer = 500
    Public ConfigFull As New List(Of String)
    Public ConfigNew As String

    Function UpdateConfig(entry As String, data As String)
        Dim TempList As New List(Of String)
        For Each f In ConfigFull
            If f.StartsWith(entry) Then
                TempList.Add(f.Replace(f.Split("=")(1), data))
            Else
                TempList.Add(f)
            End If
        Next

        ConfigFull.Clear()

        For Each f In TempList
            ConfigFull.Add(f)
        Next
        TempList.Clear()
    End Function
    Sub WriteConfig()
        Dim sw As New IO.StreamWriter(Form1.FileReadList(6)) : sw.AutoFlush = True
        For Each f In ConfigFull
            sw.WriteLine(f)
        Next
        sw.Close()
    End Sub
    Sub LoadConfig()
        Dim cfg As String() = IO.File.ReadAllLines(Form1.FileReadList(6))
        For Each f In cfg
            If Not f.Contains("#") Then
                HandleConfig(f)
            End If
            ConfigFull.Add(f)
        Next
    End Sub
    Function HandleConfig(configVar As String)
        If configVar.StartsWith("ConfigEnable=1") Then
            ConfigEnable = True
        End If

        If ConfigEnable = True Then
            If configVar.StartsWith("RunOnStart=true") Then
                If IsStartupRegistryKeyPresent() = False Then
                    Form1.CheckBox1.Checked = True
                    WriteStartupRegistryKey()
                End If
            End If
            If configVar.StartsWith("ThreadPriorityLevel=") Then
                HandleThreadPriority(configVar.Split("=")(1))
            End If
            If configVar.StartsWith("MaxGames=") Then
                Form1.TextBox1.Text = configVar.Split("=")(1)
                SetGamesLimit(Convert.ToInt32(configVar.Split("=")(1)))
            End If
            If configVar.StartsWith("EnableLogging=true") Then
                Form1.CheckBox2.CheckState = CheckState.Checked
                ErrorLogging.LoggingEnabled = True
            End If

        End If
        Return Nothing
    End Function

    'Handle GamesLimit Subs
    Public Sub SetGamesLimit(NewLimit As Integer)
        TotalGamesInLibraryLimit = NewLimit
    End Sub

    'Handle ThreadPriority Subs
    Public Sub HandleThreadPriority(level As String)
        If level = "realtime" Or level = "highest" Or level = "1" Then
            Form1.RadioButton1.Checked = False : Form1.RadioButton2.Checked = False : Form1.RadioButton3.Checked = False : Form1.RadioButton4.Checked = True
            ProcessManipulation.SetProcessPriority(GetApplicationName, ProcessPriorityClass.RealTime)
        End If
        If level = "high" Or level = "2" Then
            Form1.RadioButton1.Checked = False : Form1.RadioButton2.Checked = False : Form1.RadioButton3.Checked = True : Form1.RadioButton4.Checked = False
            ProcessManipulation.SetProcessPriority(GetApplicationName, ProcessPriorityClass.High)
        End If
        If level = "medium" Or level = "3" Then
            Form1.RadioButton1.Checked = False : Form1.RadioButton2.Checked = True : Form1.RadioButton3.Checked = False : Form1.RadioButton4.Checked = False
            ProcessManipulation.SetProcessPriority(GetApplicationName, ProcessPriorityClass.AboveNormal)
        End If
        If level = "low" Or level = "4" Then
            Form1.RadioButton1.Checked = True : Form1.RadioButton2.Checked = False : Form1.RadioButton3.Checked = False : Form1.RadioButton4.Checked = False
            ProcessManipulation.SetProcessPriority(GetApplicationName, ProcessPriorityClass.BelowNormal)
        End If
    End Sub


    'Handle Startup Registry Functions/Subs
    Public Function IsStartupRegistryKeyPresent() As Boolean
        Dim applicationName As String = GetApplicationName()

        Dim key As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", False)

        If key IsNot Nothing Then
            Dim value As Object = key.GetValue(applicationName)
            If value IsNot Nothing Then
                Return True
            End If
        End If

        Return False
    End Function
    Public Sub WriteStartupRegistryKey()
        Dim applicationName As String = GetApplicationName()
        Dim applicationPath As String = Process.GetCurrentProcess().MainModule.FileName

        Dim key As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)

        key.SetValue(applicationName, applicationPath)
    End Sub

    Public Function GetApplicationName() As String
        Dim currentProcess As Process = Process.GetCurrentProcess()
        Dim applicationName As String = currentProcess.ProcessName
        Return applicationName
    End Function
End Module
