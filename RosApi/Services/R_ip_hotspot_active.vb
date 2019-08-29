Imports System.Threading
Imports RosApi

Public Class R_ip_hotspot_active
    Inherits R_ServiceBase


    Public Event ReceivedPacket(ByVal PacketItem As RosApi.RosPacketItem)


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


    Sub New()

        Command.AddCom("/ip/hotspot/active/print")

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
    End Function


End Class

