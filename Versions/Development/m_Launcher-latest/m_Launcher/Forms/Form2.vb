Imports System.IO
Public Class Form2
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim AskFirst As DialogResult = MessageBox.Show("Uninstall " + IO.Path.GetFileNameWithoutExtension(TextBox1.Text) + "?" + vbNewLine + "This Will Free Up " + FileCalculations.GetDirSize(TextBox1.Text) + " Of Space", TextBox1.Text, MessageBoxButtons.OKCancel)
        If AskFirst = DialogResult.OK Then
            Dim AskSecond As DialogResult = MessageBox.Show("Are You Sure You Want To Uninstall " + IO.Path.GetFileNameWithoutExtension(TextBox1.Text) + "?", TextBox1.Text, MessageBoxButtons.YesNo)
            If AskSecond = DialogResult.Yes Then
                For Each dirr As String In IO.Directory.GetFiles(IO.Path.GetDirectoryName(TextBox1.Text), "*", IO.SearchOption.AllDirectories)
                    IO.File.Delete(dirr)
                Next
                For Each dirr As String In IO.Directory.GetDirectories(IO.Path.GetDirectoryName(TextBox1.Text), "*", IO.SearchOption.AllDirectories)
                    If IO.Directory.GetFiles(dirr).Count > 0 Then
                        IO.Directory.Delete(dirr)
                    End If
                Next
                If IO.Directory.GetFiles(TextBox1.Text).Count > 0 Then
                    IO.Directory.Delete(IO.Path.GetDirectoryName(TextBox1.Text))
                End If
                MsgBox(IO.Path.GetFileNameWithoutExtension(TextBox1.Text) + " Has Been Uninstalled")
            End If
        End If
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Process.Start(IO.Path.GetDirectoryName(TextBox1.Text))
    End Sub

    Private Sub Form2_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        IO.File.WriteAllText(Form1.FileReadList(4) + IO.Path.GetFileNameWithoutExtension(TextBox1.Text), TextBox2.Text)
        Form1.Show()
        Form1.BringToFront()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        MsgBox("This Operation Will Cost You " + FileCalculations.GetDirSize(TextBox1.Text) + " Are You Sure You Want To Continue?")
        'handle backup form
        Form4.DirectoryToBackUp = TextBox1.Text
        Form4.BackupDirectory = Form1.FileReadList(5)
        Form4.Show()
    End Sub

    Private Sub StartLogFinderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StartLogFinderToolStripMenuItem.Click
        FileCalculations.SearchDirectory(TextBox1.Text, ListView1, {"*.log*", "*.dmp*", "*.dbg*", "*.crash*"})
    End Sub
    Private Sub ListView1_DoubleClick(sender As Object, e As EventArgs) Handles ListView1.DoubleClick
        If ListView1.SelectedItems.Count > 0 Then
            Process.Start(ListView1.SelectedItems(0).Text)
        End If
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        FileCalculations.SearchDirectory(TextBox1.Text, ListView2,
                                         {"*.txt*",
                                         "*.lua*",
                                         "*.png*",
                                         "*.jpg*",
                                         "*.mp4*",
                                         "*.wav*",
                                         "*.snd*",
                                         "*.ts*",
                                         "*.dts*",
                                         "*.flac*",
                                         "*.ogg*",
                                         "*.midi*",
                                         "*.mov*",
                                         "*.xml*",
                                         "*.bin*",
                                         "*.info*",
                                         "*.bk*",
                                         "*.bk2*",
                                         "*.pck*",
                                         "*.pack*",
                                         "*.cas*",
                                         "*.cab*",
                                         "*.toc*",
                                         "*.zip*",
                                         "*.bank*",
                                         "*.dat*",
                                         "*.gcm*"})
    End Sub

    Private Sub ListView2_DoubleClick(sender As Object, e As EventArgs) Handles ListView2.DoubleClick
        If ListView2.SelectedItems.Count > 0 Then
            Process.Start(ListView2.SelectedItems(0).Text)
        End If
    End Sub

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If IO.File.Exists(Form1.FileReadList(4) + IO.Path.GetFileNameWithoutExtension(TextBox1.Text)) Then
            TextBox2.Text = IO.File.ReadAllText(Form1.FileReadList(4) + IO.Path.GetFileNameWithoutExtension(TextBox1.Text))
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If Me.CheckBox1.CheckState = CheckState.Checked Then
            NextProcessRunAsAdmin = True
        Else
            NextProcessRunAsAdmin = False
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If Me.CheckBox2.CheckState = CheckState.Checked Then
            NextLaunchInternally = True
        Else
            NextLaunchInternally = False
        End If
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If Me.CheckBox2.CheckState = CheckState.Checked Then
            NextWaitForLeave = True
        Else
            NextWaitForLeave = False
        End If
    End Sub
    Public Sub GetConnections(p As String, lv As ListView)
        lv.Items.Clear()
        Dim Con As List(Of String) = NetworkHandler.DisplayProcessConnections(p)
        For Each c In Con
            lv.Items.Add(c)
        Next
    End Sub

    Private Sub SniffNetworkToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SniffNetworkToolStripMenuItem.Click
        GetConnections(Path.GetFileNameWithoutExtension(TextBox1.Text), ListView3)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Form5.ProcessName = IO.Path.GetFileNameWithoutExtension(TextBox1.Text)
        Form5.Show()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        OpenFileDialog1.Title = "Open a file to inject into " + Path.GetFileNameWithoutExtension(TextBox1.Text)
        OpenFileDialog1.Filter = "DLL Files (*.dll)|*.dll"
        OpenFileDialog1.FileName = ""
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            Dim fileName As String = OpenFileDialog1.FileName
            If InjectDLL(Path.GetFileNameWithoutExtension(TextBox1.Text), fileName) Then
                Label10.ForeColor = Color.Green
                Label10.Text = "Dll injected successfully!"
            Else
                Label10.ForeColor = Color.IndianRed
                Label10.Text = "Failed to inject dll."
            End If
        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        ProcessManipulation.SuspendProcessByName(Path.GetFileNameWithoutExtension(TextBox1.Text))
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        ProcessManipulation.ResumeProcessByName(Path.GetFileNameWithoutExtension(TextBox1.Text))
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        ProcessManipulation.TerminateProcessByName(Path.GetFileNameWithoutExtension(TextBox1.Text))
    End Sub

    Private Sub TabPage5_Click(sender As Object, e As EventArgs) Handles TabPage5.Click

    End Sub

    Private Sub HighToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HighToolStripMenuItem.Click
        ProcessManipulation.SetProcessPriority(Path.GetFileNameWithoutExtension(TextBox1.Text), ProcessPriorityClass.High)
    End Sub

    Private Sub MediumToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MediumToolStripMenuItem.Click
        ProcessManipulation.SetProcessPriority(Path.GetFileNameWithoutExtension(TextBox1.Text), ProcessPriorityClass.Normal)
    End Sub

    Private Sub LowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LowToolStripMenuItem.Click
        ProcessManipulation.SetProcessPriority(Path.GetFileNameWithoutExtension(TextBox1.Text), ProcessPriorityClass.BelowNormal)
    End Sub

    Private Sub RealtimeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RealtimeToolStripMenuItem.Click
        ProcessManipulation.SetProcessPriority(Path.GetFileNameWithoutExtension(TextBox1.Text), ProcessPriorityClass.RealTime)
    End Sub
End Class