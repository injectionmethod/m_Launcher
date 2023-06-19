Public Class Form3
    Private Declare Function SetForegroundWindow Lib "user32" (ByVal hWnd As IntPtr) As Boolean


    Private Sub Form3_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        Me.Text = "Performing Checks ."
        ProgressBar1.Value = 15
        Threading.Thread.Sleep(700)
        FileCalculations.CheckExist()
        Me.Text = "Loading {Favourites} .."
        ProgressBar1.Value = 25
        Threading.Thread.Sleep(1000)
        Me.Text = "Loading {Local} ..."
        ProgressBar1.Value = 50
        FileCalculations.ReadOnEntry()
        Threading.Thread.Sleep(1000)

        ProgressBar1.Value = 75
        If Form1.EnableUpdates = True Then
            Me.Text = "Loading {Update Check} ...."
            CheckUpdate()
            Threading.Thread.Sleep(2000)
        End If

        ProgressBar1.Value = 100
        Me.Text = "Loading {Launching} ....."
        Threading.Thread.Sleep(500)
        Me.Hide()
        Form1.Show()
    End Sub
    Private Sub CheckUpdate()
        If Form1.EnableUpdates = True Then
            NextWaitForLeave = True
            ProcessHandler.PerformProcessStart(Form1.FileReadList(2), True, "-RunUpdater")
        End If
    End Sub
    Private Function BringProcessToFront(ByVal processName As String) As Boolean
        Return Process.GetProcessesByName(processName).FirstOrDefault()?.MainWindowHandle <> IntPtr.Zero AndAlso SetForegroundWindow(Process.GetProcessesByName(processName).First().MainWindowHandle)
    End Function
    Sub CheckIfProcessIsRunning()
        If System.Diagnostics.Process.GetProcessesByName("m_Launcher").Count > 1 Then
            BringProcessToFront("m_Launcher")
            Environment.Exit(0)
        End If
    End Sub
    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckIfProcessIsRunning()
    End Sub
End Class