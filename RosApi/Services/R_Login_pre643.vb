Imports System.Threading

Public Class R_Login_pre643
    Inherits R_ServiceBase
    Sub New()
        Me._TimeOut = 10000
        Command.AddCom("/login")
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



    Public Overrides Function CallBack(RosPacketItem As RosPacketItem, CallBackType As RosServiceCallBackType) As Boolean
        Select Case CallBackType
            Case RosServiceCallBackType.Err

                _IsError = 1
                _ErrorMessage = System.Enum.GetName(GetType(RosServiceStatue), CallBackType)
                Me.TrapCallBack.Invoke(Me, RosPacketItem）

            Case RosServiceCallBackType.Done
                Me.ReplyPackets.Add(RosPacketItem)
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
End Class

