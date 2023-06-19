Public Class Form5
    Public ProcessName As String
    Public LastAddressList As List(Of IntPtr)
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If CheckBox1.CheckState = CheckState.Checked Then
            If TextBox1.Text.Length > 0 Then
                RichTextBox1.Text = ExportProcessBaseAddressesToString(ProcessName, Convert.ToInt32(TextBox1.Text))
            Else
                TextBox1.Text = "{default}"
                RichTextBox1.Text = ExportProcessBaseAddressesToString(ProcessName, 8)
            End If
        End If
    End Sub
    Private Sub Form5_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = $"Memory Reader - {ProcessName} | " + $"Working Set: {GetProcessMemoryUsage(ProcessName).ToString}".Replace(": 0", ": Process Not Running")
    End Sub
    Public Sub GetAddressesAndPopulateRichTextBox(ByVal processName As String, ByVal value As Integer, ByVal richTextBox As RichTextBox)
        Dim addresses = FindAddressesWithValue(processName, value) : LastAddressList = addresses

        richTextBox.Clear()
        For Each address In addresses
            richTextBox.AppendText($"Address: {address} > " + TextBox2.Text + Environment.NewLine)
        Next
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Button3.Enabled = False
        GetAddressesAndPopulateRichTextBox(ProcessName, Convert.ToInt32(TextBox2.Text), RichTextBox2)
        Button3.Enabled = True
    End Sub
    Public Sub GetAddressesAndPopulateRichTextBoxThenCompare(ByVal processName As String, ByVal value As Integer, ByVal richTextBox As RichTextBox)
        Dim addresses = FindAddressesWithValue(processName, value)

        richTextBox.Clear()
        For Each address In addresses
            If LastAddressList.Contains(address) Then
                richTextBox.AppendText($"Address: {address} > " + TextBox2.Text + Environment.NewLine)
            End If
        Next
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        GetAddressesAndPopulateRichTextBoxThenCompare(ProcessName, Convert.ToInt32(TextBox2.Text), RichTextBox2)
    End Sub
    'MAKE SURE TO FINISH THIS PART BELOW
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim address As IntPtr
        If Int64.TryParse(TextBox3.Text, address.ToInt64()) Then
            Dim value As String = TextBox4.Text
            Dim data As Byte()

            If Int32.TryParse(value, Nothing) Then
                data = BitConverter.GetBytes(Convert.ToInt32(value))
            ElseIf Single.TryParse(value, Nothing) Then
                data = BitConverter.GetBytes(Convert.ToSingle(value))
            ElseIf Double.TryParse(value, Nothing) Then
                data = BitConverter.GetBytes(Convert.ToDouble(value))
            ElseIf Boolean.TryParse(value, Nothing) Then
                data = BitConverter.GetBytes(Convert.ToBoolean(value))
            Else
                MsgBox("Invalid input value. Could not write to memory.")
                Return
            End If
            If data.Length > 0 Then
                MemoryHandling.WriteMemory(ProcessName, address, data)
            Else
                MsgBox("Data length was not correct.")
            End If
        Else
            MsgBox("Invalid address. Could not write to memory.")
        End If
    End Sub
End Class