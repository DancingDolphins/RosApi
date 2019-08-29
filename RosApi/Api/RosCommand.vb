Public Class RosCommand
    Private _IsEmpty As Boolean = True

    ReadOnly Property IsEmpty As Boolean
        Get
            Return _IsEmpty
        End Get
    End Property

    Public ReadOnly CommandsString As New List(Of String)

    Sub New()

    End Sub

    Public Function AddCom(ByVal str As String) As RosCommand
        If String.IsNullOrEmpty(str) = False Then
            CommandsString.Add(str)
            _IsEmpty = False
        Else

        End If
        Return Me
    End Function

    Public Sub SetCom(ByVal Command As RosCommand)
        CommandsString.Clear()
        CommandsString.AddRange(Command.CommandsString)
        Me._IsEmpty = Command.IsEmpty
    End Sub

    Public Function Clone() As RosCommand
        Dim NewRosCommand As New RosCommand
        For Each str As String In Me.CommandsString
            NewRosCommand.AddCom(str)
        Next
        Return NewRosCommand
    End Function

    Public Sub Clear()
        CommandsString.Clear()
    End Sub

End Class
