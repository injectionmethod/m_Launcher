Imports System.IO

Module GameImporter
    Dim GameDB As String()
    Dim GameWrite As StreamWriter
    Dim CertainExclusions As New List(Of String)

    Public Function AddExclusions()
        Dim Exclusions As String() =
        {"\python",
         "\windows\",
         "\appdata\",
         "\amd\",
         "\awd",
         "\nvidia",
         "\git\",
         "\source\repos\",
         "\system\",
         "\jetbrain\",
         "driver",
         "\bin\",
         "\adobe",
         "\after effects",
         "\sony vegas",
         "redist",
         "unins",
         "\fl-studio",
         "\drivers",
         "cheat",
         "trainer",
         "gameboost",
         "pbsvc",
         "activation",
         "installer",
         "web",
         "setup",
         "directx",
         "launcher",
         "protocol",
         "unitycrash",
         "\unity\",
         "unityex",
         "fcs_gog",
         "generator",
         "data",
         "dotnet",
         "dependencies",
         "language",
         "jre",
         "\java\",
         "crashpad",
         "vulkan",
         "\logs\",
         "server",
         "perl",
         "pip\",
         "platform",
         "\port",
         "scanner",
         "program files",
         "fraps",
         "obs",
         "streaming",
         "streamlabs",
         "errorreport",
         "crashreport",
         "overlay",
         "minidump",
         "\mods\",
         "crashsender",
         "benchmark",
         "loader",
         "install",
         "launch",
         "protected",
         "boot",
         "modding",
         "node",
         "browser",
         "\mp\",
         "objects",
         "assets",
         "sdk",
         "py\lib",
         "bash.exe",
         "cmd.exe",
         "nuget",
         "packages",
         "config"
        }
        For Each f In Exclusions
            CertainExclusions.Add(f)
        Next
    End Function
    Public Sub ImportGames()
        AddExclusions()
        GameWrite = New StreamWriter(Environment.CurrentDirectory + "/import")

        Form1.Text = "Importing Applications From Disk | Max Time To Completion: 43 minutes"
        GameWrite.AutoFlush = True

        Dim DriveList As New List(Of String)
        For Each drive As String In Directory.GetLogicalDrives()
            Dim driveInfo As New DriveInfo(drive)
            If driveInfo.DriveType <> DriveType.CDRom Or driveInfo.DriveType <> DriveType.Removable Or driveInfo.DriveType <> DriveType.Fixed Then
                DriveList.Add(drive.Replace("/", "\"))
            End If
        Next

        GameDB = IO.File.ReadAllLines(Form1.FileReadList(7))
        Dim task As Task = Task.Run(Sub() RecursiveFileSearch(DriveList))
        task.Wait()

        LoadGamesFromImport()
        Form1.Text = "m_Launcher " + "{" + Form1.ListView1.Items.Count.ToString + " Games}"
        IO.File.Delete(Environment.CurrentDirectory + "/import")
    End Sub

    Public Sub RecursiveFileSearch(driveList As List(Of String))
        For Each drive As String In driveList
            Dim rootDirectory As New DirectoryInfo(drive)

            If rootDirectory.Exists Then
                SearchFilesInDirectory(rootDirectory)
            End If
        Next
        GameWrite.Close()
    End Sub

    Private Sub SearchFilesInDirectory(directory As DirectoryInfo)
        Try
            For Each file As FileInfo In directory.GetFiles("*.exe")
                If directory.FullName.ToLower.Contains("steamapps") Or directory.FullName.ToLower.Contains("games") Or directory.FullName.ToLower.Contains("origin") Then
                    Dim ff = file.FullName
                    Dim locate_1 = False
                    For Each f In GameDB
                        If Path.GetFileNameWithoutExtension(ff).Replace(" ", "").Replace("-", "").Contains(f.ToLower) Then
                            locate_1 = True
                        End If
                    Next


                    If locate_1 = True Then
                        Dim excluded = False
                        For Each ex In CertainExclusions
                            If (ff).ToLower.Contains(ex) Then
                                excluded = True
                            End If
                        Next

                        If excluded = False Then
                            GameWrite.WriteLine(ff)
                        End If
                    End If
                End If
            Next

            For Each subDirectory As DirectoryInfo In directory.GetDirectories()
                SearchFilesInDirectory(subDirectory)
            Next
        Catch ex As Exception
            ErrorLogging.WriteToLog(ex)
        End Try
    End Sub

    Private Sub LoadGamesFromImport()
        For Each file In IO.File.ReadAllLines(Environment.CurrentDirectory + "/import")
            If FileCalculations.CheckIfExists(file, Form1.ListView1) = False Then
                If FileCalculations.IsFileFormatCorrect(file) = True Then
                    FileCalculations.LoadGameFromLocal(file, Form1.ListView1, Form1.ImgList)
                End If
            End If
        Next
    End Sub
End Module