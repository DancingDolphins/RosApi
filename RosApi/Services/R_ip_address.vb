Imports System.Threading
Imports RosApi

Public Class R_ip_address
    Inherits R_ServiceBase



    Public Event ReceivedPacket(ByVal PacketItem As RosApi.RosPacketItem)



    Sub New()
        Me.IsListening = True
        Command.AddCom("/ip/address/listen")

    End Sub
    Public Overrides Function CallBack(RosPacketItem As RosPacketItem, CallBackType As RosServiceCallBackType) As Boolean




        If RosPacketItem.HasTrap Then
            TrapCallBack.Invoke(Me, RosPacketItem)
            Return False
        End If
        If RosPacketItem.HasRe Then
            ReCallback.Invoke(Me, RosPacketItem)
            Return False
        End If

        Return True
        '     Return MyBase.CallBack(RosPacketItem, CallBackType)
    End Function

End Class

