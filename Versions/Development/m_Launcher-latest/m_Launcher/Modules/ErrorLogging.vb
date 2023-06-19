Module ErrorLogging
    Public LoggingEnabled = False
    Public log As New List(Of String)
    Function WriteToLog(ex As Exception)
        log.Add("##EXCEPTION##" + vbNewLine + ex.Message + vbNewLine + ex.TargetSite.Name)
    End Function


End Module
