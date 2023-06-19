Imports System.IO

Public Class Form4
    Public DirectoryToBackUp As String
    Public BackupDirectory As String
    Private Sub Form4_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        Me.Text = "Backup View, Backup To Be Installed @ " + BackupDirectory
        For Each file In Directory.GetFiles(IO.Path.GetDirectoryName(DirectoryToBackUp), "*", IO.SearchOption.AllDirectories)
            Dim lvi As New ListViewItem(file)
            lvi.SubItems.Add(FileCalculations.GetFileSize(file))
            ListView1.Items.Add(lvi)
        Next
        Label1.Text = "Total Size For Backup: " + FileCalculations.GetDirSize(DirectoryToBackUp) + " | Files {" + Directory.GetFiles(IO.Path.GetDirectoryName(DirectoryToBackUp), "*", IO.SearchOption.AllDirectories).Count.ToString + "} Folders {" + Directory.GetDirectories(IO.Path.GetDirectoryName(DirectoryToBackUp), "*", IO.SearchOption.AllDirectories).Count.ToString + "}"
        ListView1.Update()
        Me.Show()
    End Sub
    Private Sub Backup()
        Me.Text = "Creating Backup!"
        Try
            Directory.CreateDirectory(IO.Path.GetDirectoryName(BackupDirectory))
            Dim SourcePath As String = IO.Path.GetDirectoryName(DirectoryToBackUp)
            Dim DestinationPath As String = IO.Path.GetDirectoryName(BackupDirectory)
            Dim newDirectory As String = System.IO.Path.Combine(DestinationPath, Path.GetFileName(SourcePath))
            If Not (Directory.Exists(newDirectory)) Then
                Directory.CreateDirectory(newDirectory)
            End If
            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(SourcePath, newDirectory)
            Me.Text = "Backup Complete!"
        Catch ex As Exception
            Me.Text = "Backup Failed!"
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Backup()
    End Sub
End Class