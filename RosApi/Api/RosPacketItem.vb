Public Class RosPacketItem
    Property Tag As Integer?

    Property HasTrap As Boolean

    Property HasRe As Boolean

    Property Hasfatal As Boolean


    Property HasDone As Boolean

    Public ReadOnly Property KV As New Dictionary(Of String, String)


    Sub New()

    End Sub



    Public Function Append(RepStr As String) As RosPacketItemType
        If String.IsNullOrEmpty(RepStr) Then Return RosPacketItemType.Unkown
        Select Case RepStr
            Case "!done"
                HasDone = True
                Return RosPacketItemType.done
            Case "!re"
                HasRe = True
                Return RosPacketItemType.re
            Case "!trap"
                HasTrap = True
                Return RosPacketItemType.trap
            Case "!fatal"
                Hasfatal = True
                Return RosPacketItemType.fatal
            Case "not logged in"
                Return RosPacketItemType.fatal
            Case Else

                If RepStr.StartsWith(".tag") Then
                    Tag = CInt(GetValue(RepStr))
                    Return RosPacketItemType.tag
                Else
                    Dim ProKey As String = GetKey(RepStr)
                    If ProKey Is Nothing Then ProKey = ""
                    Dim Value As String = GetValue(RepStr)
                    If Value Is Nothing Then Value = ""
                    KV.Add(ProKey, Value)
                    Return RosPacketItemType.KV
                End If

        End Select



    End Function
    Public Shared Function GetKey(str As String) As String
        If String.IsNullOrEmpty(str) Then Return Nothing

        Return str.ToRegular("(?<=^\=).*?(?=\=)").FirstOrDefault



    End Function
    Public Shared Function GetValue(str As String) As String
        If String.IsNullOrEmpty(str) Then Return Nothing
        Return str.ToRegular("(?<=.\=).*?(?=$)").FirstOrDefault
    End Function



End Class

Public Enum RosPacketItemType
    Unkown = 0
    done = 1
    re = 2
    trap = 3
    fatal = 4
    KV = 5
    tag = 6
End Enum