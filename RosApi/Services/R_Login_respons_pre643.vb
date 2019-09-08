Imports System.Threading

Public Class R_Login_respons_pre643
    Inherits R_ServiceBase
    Sub New()
        Me._TimeOut = 10000
    End Sub
    Private _IsError As Boolean = False
    Public ReadOnly Property IsError() As Boolean
        Get
            Return _IsError
        End Get

    End Property

    Private _ErrorMessage As String = ""
    Public ReadOnly Property ErrorMessage() As String
        Get
            Return _ErrorMessage
        End Get

    End Property

    Sub SetAuth(Auth As RosAuth, challange As String)

        Command.AddCom("/login") _
            .AddCom("=name=" + Auth.Username) _
            .AddCom("=response=00" & EncodePassword(Auth.Password, challange))
    End Sub

    Public Overrides Function CallBack(RosPacketItem As RosPacketItem, CallBackType As RosServiceCallBackType) As Boolean
        Select Case CallBackType
            Case RosServiceCallBackType.Err

                _IsError = 1
                _ErrorMessage = System.Enum.GetName(GetType(RosServiceStatue), CallBackType)
                Me.TrapCallBack.Invoke(Me, RosPacketItem）

            Case RosServiceCallBackType.Done
                Me.DoneCallBack.Invoke(Me, RosPacketItem）
            Case RosServiceCallBackType.Fatel, RosServiceCallBackType.Trap
                _IsError = 1
                _ErrorMessage = RosPacketItem.KV("message")
                Me.TrapCallBack.Invoke(Me, RosPacketItem）
            Case Else
                _IsError = 1
                _ErrorMessage = RosPacketItem.KV("Unknown Statue")
                Me.TrapCallBack.Invoke(Me, RosPacketItem）
        End Select






        Return True
    End Function

    Function EncodePassword(ByVal pass As String, ByVal challange As String) As String
        Dim hash_byte(challange.Length / 2 - 1) As Byte
        For i = 0 To challange.Length - 2 Step 2
            hash_byte(i / 2) = Byte.Parse(challange.Substring(i, 2), Globalization.NumberStyles.HexNumber)
        Next
        Dim response(pass.Length + hash_byte.Length) As Byte
        response(0) = 0
        Text.Encoding.ASCII.GetBytes(pass.ToCharArray()).CopyTo(response, 1)
        hash_byte.CopyTo(response, 1 + pass.Length)


        Dim md5 = New System.Security.Cryptography.MD5CryptoServiceProvider()

        Dim hash = md5.ComputeHash(response)

        Dim hashStr As New Text.StringBuilder()
        For Each h In hash
            hashStr.Append(h.ToString("x2"))
        Next
        Return hashStr.ToString()
    End Function
End Class

