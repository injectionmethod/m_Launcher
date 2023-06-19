
Public Class Form1
    'ADD FILE_READ_LIST to New Form "Form3.vb"
    Public FileReadList As String()
    Public ImgList As New ImageList With {.ImageSize = New Size(24, 24)}
    Private LastPlayed As String = Nothing
    Private CurrentController As String = Nothing

    Public EnableUpdates = False

    Private Sub ListView1_DragEnter(sender As Object, e As DragEventArgs) Handles ListView1.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then e.Effect = DragDropEffects.Copy
    End Sub
    Private Sub ListView1_DragDrop(sender As Object, e As DragEventArgs) Handles ListView1.DragDrop
        Dim files As String() = CType(e.Data.GetData(DataFormats.FileDrop), String())
        Dim FileString As New List(Of String)

        For Each ff As String In files
            For Each fff As String In IO.File.ReadAllLines(FileReadList(0))
                If ff = fff Then
                    If FileCalculations.CheckIfExists(ff, ListView1) = False Then
                        If FileCalculations.IsFileFormatCorrect(ff) = True Then
                            FileCalculations.LoadGameFromLocal(ff, ListView2, ImgList)
                        End If
                    End If
                End If
            Next

            If FileCalculations.CheckIfExists(ff, ListView1) = False Then
                If FileCalculations.IsFileFormatCorrect(ff) = True Then
                    FileCalculations.LoadGameFromLocal(ff, ListView1, ImgList)
                End If
            End If
        Next
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        LoadConfig()

        Dim files As String() = IO.File.ReadAllLines(FileReadList(0))
        For Each ff As String In files
            For Each fff As String In IO.File.ReadAllLines(FileReadList(1))
                If ff = fff Then
                    If FileCalculations.CheckIfExists(ff, ListView1) = False Then
                        If FileCalculations.IsFileFormatCorrect(ff) = True Then
                            FileCalculations.LoadGameFromLocal(ff, ListView2, ImgList)
                        End If
                    End If
                End If
            Next

            If FileCalculations.CheckIfExists(ff, ListView1) = False Then
                If FileCalculations.IsFileFormatCorrect(ff) = True Then
                    FileCalculations.LoadGameFromLocal(ff, ListView1, ImgList)
                End If
            End If
        Next

        For Each webaddress In IO.File.ReadAllLines(FileReadList(3))
            Dim lv As New ListViewItem(webaddress)
            ListView3.Items.Add(lv)
            ListView3.Update()
        Next
        Me.Text = "m_Launcher {" + ListView1.Items.Count.ToString + " Games}"

        CheckLibraryCount.Start()
        GamepadInfoSet()
    End Sub
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Dim sw As New IO.StreamWriter(FileReadList(0)) : sw.AutoFlush = True
        For Each item As ListViewItem In ListView1.Items
            sw.WriteLine(IO.Path.GetFullPath(item.SubItems(2).Text))
        Next

        If ErrorLogging.LoggingEnabled = True Then
            Dim ErrorWriter As New IO.StreamWriter(Environment.CurrentDirectory + "/ExceptionLog.txt") : ErrorWriter.AutoFlush = True
            For Each ex In ErrorLogging.log
                ErrorWriter.Write(ex)
            Next
            ErrorWriter.Close()
        End If


        NotifyIcon1.Visible = False
        Form3.Close()
    End Sub
    Private Sub PlayToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PlayToolStripMenuItem.Click
        StageLaunch(ListView1)
    End Sub
    Private Sub PropertiesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PropertiesToolStripMenuItem.Click
        If ListView1.SelectedItems.Count > 0 Then
            PropertiesFunctions.MakeProperties(ListView1.SelectedItems(0).SubItems(2).Text, ListView1.SelectedItems(0).SubItems(1).Text)
        End If
    End Sub
    Private Sub AddToFavouritesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddToFavouritesToolStripMenuItem.Click
        If ListView1.SelectedItems.Count > 0 Then
            Dim IsFav = False

            For Each item As String In IO.File.ReadAllLines(FileReadList(1))
                If item.Contains(ListView1.SelectedItems(0).SubItems(2).Text) Then
                    IsFav = True
                End If
            Next

            If IsFav = False Then
                Dim GetFav As String = IO.File.ReadAllText(FileReadList(1))
                Dim WriteFav As New IO.StreamWriter(FileReadList(1)) : WriteFav.AutoFlush = True
                WriteFav.WriteLine(ListView1.SelectedItems(0).SubItems(2).Text)
                WriteFav.WriteLine(GetFav)
                WriteFav.Close()
                FileCalculations.LoadGameFromLocal(ListView1.SelectedItems(0).SubItems(2).Text, ListView2, ImgList)
            End If
        End If
    End Sub
    Private Sub PropertiesToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles PropertiesToolStripMenuItem1.Click
        If ListView2.SelectedItems.Count > 0 Then
            PropertiesFunctions.MakeProperties(ListView2.SelectedItems(0).SubItems(2).Text, ListView2.SelectedItems(0).SubItems(1).Text)
        End If
    End Sub

    Private Sub PlayToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles PlayToolStripMenuItem1.Click
        StageLaunch(ListView2)
    End Sub
    Private Function UpdateAll(l As ListView)
        PropertiesFunctions.TitleSet(l, Me, LastPlayed, CurrentController)
        Return True
    End Function
    Private Sub ListView1_Click(sender As Object, e As EventArgs) Handles ListView1.Click
        UpdateAll(ListView1)
    End Sub
    Private Sub ListView2_Click(sender As Object, e As EventArgs) Handles ListView2.Click
        UpdateAll(ListView2)
    End Sub
    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        If ListView3.SelectedItems.Count > 0 Then
            Process.Start("chrome.exe", ListView3.SelectedItems(0).SubItems(0).Text)
        End If
    End Sub
    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Me.Show()
        Me.BringToFront()
        Me.WindowState = FormWindowState.Normal
    End Sub
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Environment.Exit(0)
    End Sub
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        If ListBox1.SelectedItems.Count > 0 Then
            JoyStickTimer.Stop()
            JoyStickTimer.Start()
            Label4.ForeColor = Color.Green
            Label4.Text = "{🎮 ID-" + GetDeviceId(ListBox1.SelectedItems(0)).ToString + "}"
        End If
    End Sub
    Private Sub JoyStickTimer_Tick(sender As Object, e As EventArgs) Handles JoyStickTimer.Tick
        Try
            Label2.Text = GetJoystickState(GetDeviceId(ListBox1.SelectedItems(0))).Replace(":", ":" + vbNewLine).Replace(",", vbNewLine)
        Catch ex As Exception
            Label2.Text = "Error, Cannot Get The State Information Of Joystick {ID:" + GetDeviceId(ListBox1.SelectedItems(0)).ToString + "}"
        End Try
    End Sub
    Private Sub UseThisGamepadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UseThisGamepadToolStripMenuItem.Click
        If CurrentController IsNot Nothing Then
            ReleaseJoystickCapture(GetDeviceId(ListBox1.SelectedItems(0)))
        End If
        Try
            SetActiveDevice(ListBox1.SelectedItems(0))
            CurrentController = ListBox1.SelectedItems(0)
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        JoyStickTimer.Stop()
        PropertiesFunctions.GamepadInfoSet()
    End Sub
    Private Sub ListView1_DoubleClick(sender As Object, e As EventArgs) Handles ListView1.DoubleClick
        StageLaunch(ListView1)
    End Sub
    Private Sub ListView2_DoubleClick(sender As Object, e As EventArgs) Handles ListView2.DoubleClick
        StageLaunch(ListView2)
    End Sub
    Sub StageLaunch(l As ListView)
        If l.SelectedItems.Count > 0 Then
            LastPlayed = IO.Path.GetFileNameWithoutExtension(l.SelectedItems(0).SubItems(2).Text)
            UpdateAll(l)
            ProcessHandler.PerformProcessStart(l.SelectedItems(0).SubItems(2).Text, False, Nothing)
            l.SelectedItems.Clear()
        End If
    End Sub

    Private Sub RemoveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveToolStripMenuItem.Click
        ListView1.Items.Remove(ListView1.SelectedItems(0))
    End Sub

    Private Sub RemoveToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles RemoveToolStripMenuItem1.Click
        ListView2.Items.Remove(ListView2.SelectedItems(0))
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'Handle Radio Buttons
        If RadioButton1.Checked = True Then
            UpdateConfig("ThreadPriorityLevel=", "low")
        End If
        If RadioButton2.Checked = True Then
            UpdateConfig("ThreadPriorityLevel=", "medium")
        End If
        If RadioButton3.Checked = True Then
            UpdateConfig("ThreadPriorityLevel=", "high")
        End If
        If RadioButton4.Checked = True Then
            UpdateConfig("ThreadPriorityLevel=", "realtime")
        End If

        'Handle Checkboxes
        If CheckBox1.CheckState = CheckState.Checked Then
            UpdateConfig("RunOnStart=", "true")
        Else
            UpdateConfig("RunOnStart=", "false")
        End If
        If CheckBox2.CheckState = CheckState.Checked Then
            UpdateConfig("EnableLogging=", "true")
        Else
            ErrorLogging.LoggingEnabled = False
            UpdateConfig("EnableLogging=", "false")
        End If

        'Handle Inputs
        If TextBox1.Text.Length > 0 Then
            UpdateConfig("MaxGames=", TextBox1.Text)
        End If

        'Write And Load
        WriteConfig() : LoadConfig()
    End Sub

    Private Sub BrowseDiskForGamesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BrowseDiskForGamesToolStripMenuItem.Click
        Dim file = FileCalculations.GetSelectedFilePath()
        If FileCalculations.CheckIfExists(file, ListView1) = False Then
            If FileCalculations.IsFileFormatCorrect(file) = True Then
                FileCalculations.LoadGameFromLocal(file, ListView1, ImgList)
            End If
        End If
    End Sub
    Public Function GetSelectedFilePath() As String
        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.InitialDirectory = IO.Directory.GetDirectoryRoot("%public%")
        openFileDialog.Filter = "Application Files (*exe.*)|*.exe*"

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            Return openFileDialog.FileName
        Else
            Return String.Empty
        End If
    End Function

    Private Sub ScanDiskForGamesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ScanDiskForGamesToolStripMenuItem.Click
        Label10.Text = "Scanning For Games..."
        GameImporter.ImportGames()
    End Sub

    Private Sub CheckLibraryCount_Tick(sender As Object, e As EventArgs) Handles CheckLibraryCount.Tick
        If ListView1.Items.Count > 0 Then
            Label10.Visible = False
        Else
            Label10.Visible = True
        End If
        If ListView2.Items.Count > 0 Then
            Label11.Visible = False
        Else
            Label11.Visible = True
        End If
    End Sub
End Class