Module PropertiesFunctions
    Public Function ExtractIcon(ByVal path As String, Optional ByVal index As Integer = 0) As Icon
        Dim handle As IntPtr = ExtractAssociatedIcon(IntPtr.Zero, path, index)
        If handle = IntPtr.Zero Then Throw New System.ComponentModel.Win32Exception
        Dim retval As Icon = Nothing
        Using temp As Icon = Icon.FromHandle(handle)
            retval = CType(temp.Clone(), Icon)
            DestroyIcon(handle)
        End Using
        Return retval
    End Function

    Private Declare Auto Function ExtractAssociatedIcon Lib "shell32" (
        ByVal hInst As IntPtr, ByVal path As String, ByRef index As Integer) As IntPtr
    Private Declare Auto Function DestroyIcon Lib "user32" (ByVal hIcon As IntPtr) As Boolean
    Public Function IsMenu(filename As String) As String
        Dim MenuAvailableList As String() = {
        "skate.crack", "projectzomboid64", "rdr2",
        "gta", "dbfighterz", "kenshi"
        }
        Dim MenuFound As Boolean = False
        For Each menu In MenuAvailableList
            If filename.ToLower.Contains(menu) Then
                MenuFound = True
            End If
        Next
        Return MenuFound
    End Function
    Public Function IsBackedUp(filename As String) As Boolean
        Dim IsTrue As Boolean = False
        For Each dirr As String In IO.Directory.GetDirectories(Form1.FileReadList(5))
            If dirr.ToUpper.Contains(IO.Path.GetFileNameWithoutExtension(filename).ToUpper) Then
                IsTrue = True
            End If
        Next
        Return IsTrue
    End Function
    Public Function IsModFolder(filename As String) As String
        Dim Filter As String() = {
            "MOD",
            "DROPZONE"}

        Dim IsTrue As Boolean = False
        For Each dirr As String In IO.Directory.GetDirectories(IO.Path.GetDirectoryName(filename))
            For Each f In Filter
                If dirr.ToUpper.Contains(f) Then
                    IsTrue = True
                End If
            Next
        Next
        Return IsTrue
    End Function
    Public Function IsDLCFound(filename As String) As String
        Dim Filter As String() = {
            "DOWNLOADABLE",
            "DLC",
            "EXTRA CONTENT",
            "EXTRA_CONTENT",
            "EXTRA-CONTENT"}

        Dim IsTrue As Boolean = False
        For Each dirr As String In IO.Directory.GetDirectories(IO.Path.GetDirectoryName(filename))
            For Each f In Filter
                If dirr.ToUpper.Contains(f) Then
                    IsTrue = True
                End If
            Next
        Next
        Return IsTrue
    End Function
    Public Sub MakeProperties(name As String, title As String)
        Form2.Label2.Text += " " + FileCalculations.GetFileSize(name).ToString
        Form2.Label3.Text += " " + FileCalculations.GetDirSize(name).ToString
        Form2.Label4.Text += PropertiesFunctions.IsMenu(name)
        Form2.Label5.Text += PropertiesFunctions.IsBackedUp(name).ToString
        Form2.Label7.Text += " " + IO.Path.GetExtension(name)
        Form2.Label8.Text += PropertiesFunctions.IsModFolder(name)
        Form2.Label9.Text += PropertiesFunctions.IsDLCFound(name)
        Form2.PictureBox1.Image = PropertiesFunctions.ExtractIcon(name, 3).ToBitmap
        Form2.Text = "Properties - " + title
        Form2.TextBox1.Text += name
        Form2.Show()
    End Sub
    Public Function TitleSet(ListVieww As ListView, formm As Form, LastPlayed As String, Controller As String)
        If ListVieww.SelectedItems.Count > 0 Then
            formm.Text = "m_Launcher {" + ListVieww.Items.Count.ToString + " Games} | Selected - " + IO.Path.GetFileNameWithoutExtension(ListVieww.SelectedItems(0).SubItems(2).Text)
            If LastPlayed IsNot Nothing Then
                formm.Text += " | Last Played - " + LastPlayed
            End If
            If Controller IsNot Nothing Then
                formm.Text += " | Using Controller {" + GetDeviceId(Controller).ToString + "}"
            End If
        End If
        Return True
    End Function
    Public Function GamepadInfoSet()
        Form1.Label1.Text = "Detected Gamepads: " + GetJoystickNames.Count.ToString
        Form1.ListBox1.Items.Clear()

        For Each joystick In GetJoystickNames()
            Form1.ListBox1.Items.Add(joystick)
        Next
        If Form1.ListBox1.Items.Count = 0 Then
            Form1.ListBox1.Items.Add("No Gamepads Detected")
        End If
    End Function
End Module
