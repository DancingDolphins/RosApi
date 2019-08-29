Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Public Module RosHelper
    <Extension()>
    Public Function MergeBytes(ByVal Bytes1 As Byte(), ByVal Bytes2 As Byte()) As Byte()
        If Bytes1 Is Nothing And Bytes2 Is Nothing Then Return Nothing
        If Bytes2 Is Nothing Then Return Bytes1
        If Bytes1 Is Nothing Then Return Bytes2
        Dim Bytes(Bytes1.Length - 1 + Bytes2.Length) As Byte
        Dim I As Integer = 0
        For I = 1 To Bytes1.Length
            Bytes(I - 1) = Bytes1(I - 1)

        Next
        For j = 1 To Bytes2.Length
            Bytes(I + j - 2) = Bytes2(j - 1)
        Next


        Return Bytes
    End Function

    Public Function BintoBin(ByVal Bytes() As Byte, ByVal Start As Integer, ByVal Count As Integer) As Byte()
        Dim Tobytes() As Byte
        If Bytes.Length = 0 Then
            BintoBin = Nothing
            Exit Function
        Else
            If Start > Bytes.Length Then
                BintoBin = Nothing
                Exit Function
            Else
                If Start + Count > Bytes.Length Then
                    BintoBin = Nothing
                    Exit Function
                Else
                    ReDim Tobytes(Count - 1)

                    For i = 0 To Count - 1
                        Tobytes(i) = Bytes(Start + i)
                    Next
                    Return Tobytes

                End If
            End If
        End If
    End Function



    Public Function BinReadByte(ByRef NeedBytes As Byte()) As Byte
        If NeedBytes.Length = 0 Then
            Return Nothing
        ElseIf NeedBytes.Length = 1 Then
            Return NeedBytes(0)
            NeedBytes = Nothing
        Else


            Dim RetByte As Byte = 0
            RetByte = NeedBytes(0)
            For i = 2 To NeedBytes.Length
                NeedBytes(i - 2) = NeedBytes(i - 1)
            Next
            ReDim Preserve NeedBytes(NeedBytes.Length - 2)
            Return (RetByte)
        End If




    End Function
    <Extension()>
    Public Function FindFirstbyteIndex(ByVal NeedArray() As Byte, ByVal FindByte As Byte) As Integer
        If NeedArray.Length = 0 Then Return -1
        If NeedArray.Length = 1 And NeedArray(0) <> FindByte Then Return -1
        For i = 0 To NeedArray.Length - 1
            If NeedArray(i) = FindByte Then
                Return i
                Exit Function
            End If
        Next
        Return -1
    End Function

    <Extension()>
    Public Function ToRegular(ByVal Str As String, pattern As String) As String()
        If String.IsNullOrEmpty(Str) Then
            Return Nothing
        ElseIf String.IsNullOrEmpty(pattern) Then
            Return Nothing
        Else

            Dim RetList As New List(Of String)
            For Each Match As Match In Regex.Matches(Str, pattern)
                RetList.Add(Match.Value)
            Next
            Return RetList.ToArray
        End If


    End Function
End Module
Public Class ClassCheck
    Public Shared Function IsPort(ByVal NeedPort As String) As Boolean
        If IsNumeric(NeedPort) And CInt(NeedPort) > 1 And NeedPort < 65535 Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Shared Function IsIP(ByVal IP As String) As Integer
        If IP.Length > 15 Or IP.Length < 7 Then
            IsIP = -1
            Exit Function
        End If
        For i = 1 To IP.Length
            If IsIPLetter(Mid(IP, i, 1)) = False Then
                IsIP = -2
                Exit Function
            End If
        Next
        Dim TmpStr() As String
        TmpStr = Split(IP, ".")
        Dim TmpInt As Integer
        For i = 1 To 4
            TmpInt = Val(TmpStr(i - 1))
            If TmpInt > 255 Or TmpInt < 0 Then
                IsIP = -3
                Exit Function
            End If
        Next
        IsIP = 1
    End Function
    Public Shared Function IsMAC(ByVal MAC As String) As Integer
        If MAC.Length <> 17 Then
            IsMAC = -1
            Exit Function
        End If
        For i = 1 To MAC.Length
            If IsMACLetter(Mid(MAC, i, 1)) = False Then
                IsMAC = -2
                Exit Function
            End If
        Next
        IsMAC = 1
    End Function
    Public Shared Function IsIPLetter(ByVal IPLetter As String) As Boolean
        '  MsgBox(Asc(IPLetter))
        If (Asc(IPLetter) >= 48 And Asc(IPLetter) <= 57) Or IPLetter = "." Then
            IsIPLetter = True
        Else
            IsIPLetter = False
        End If
    End Function
    Public Shared Function IsMACLetter(ByVal MACLetter As String) As Boolean

        If (Asc(MACLetter) >= 65 And Asc(MACLetter) <= 70) Or (Asc(MACLetter) >= 97 And Asc(MACLetter) <= 102) Or (Asc(MACLetter) >= 48 And Asc(MACLetter) <= 57) Or MACLetter = "-" Or MACLetter = ":" Then
            IsMACLetter = True
        Else
            IsMACLetter = False
            MsgBox(MACLetter)
        End If
    End Function


End Class
