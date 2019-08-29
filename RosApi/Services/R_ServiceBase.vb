Imports System.Threading
Imports RosApi

Public MustInherit Class R_ServiceBase
    Implements IRosService
    Sub New()

    End Sub
    Private _Tag As Integer?
    Public ReadOnly Property Tag As Integer? Implements IRosService.Tag
        Get
            Return _Tag
        End Get
    End Property
    Private _Statue As RosServiceStatue = RosServiceStatue.UnStarted
    Public Property Statue As RosServiceStatue Implements IRosService.Statue
        Get
            Return _Statue
        End Get
        Set(value As RosServiceStatue)
            _Statue = value

        End Set

    End Property
    Private _Command As New RosCommand
    Public ReadOnly Property Command As RosCommand Implements IRosService.Command
        Get
            Return _Command
        End Get
    End Property
    Private _ReplyPackets As New List(Of RosPacketItem)
    Public ReadOnly Property ReplyPackets As List(Of RosPacketItem) Implements IRosService.ReplyPackets
        Get
            Return _ReplyPackets
        End Get
    End Property
    Private _WaitFinishFlag As New Threading.AutoResetEvent(False)

    Public ReadOnly Property WaitFinishFlag As AutoResetEvent Implements IRosService.WaitFinishFlag
        Get
            Return _WaitFinishFlag
        End Get
    End Property
    Public Event StatueChanged(Statue As RosServiceStatue) Implements IRosService.StatueChanged



    Public Function GetCommand(Optional Query As RosQuery = Nothing) As RosCommand Implements IRosService.GetCommand
        Dim NewCommand = Command.Clone
        If Tag IsNot Nothing Then
            NewCommand.AddCom(".tag=" + Tag.ToString)

        End If
        NewCommand.AddCom("")
        Return NewCommand
    End Function

    Delegate Sub DCallBack(Sender As Object, ByVal RosPacketItem As RosPacketItem)

    Property DoneCallBack As DCallBack = Nothing
    Property ReCallback As DCallBack = Nothing
    Property TrapCallBack As DCallBack = Nothing
    Property FatalCallBack As DCallBack = Nothing
    Protected Friend _TimeOut As Integer = 5000
    Public ReadOnly Property TimeOut As Integer Implements IRosService.TimeOut
        Get
            Return _TimeOut
        End Get
    End Property
    Private _WaitSendFlag As New AutoResetEvent(False)
    Public ReadOnly Property WaitSendFlag As AutoResetEvent Implements IRosService.WaitSendFlag
        Get
            Return _WaitSendFlag
        End Get
    End Property
    Private _IsListening As Boolean = False

    Public Property IsListening As Boolean Implements IRosService.IsListening
        Get
            Return _IsListening
        End Get
        Set(value As Boolean)
            _IsListening = value
        End Set
    End Property


    Public Sub SetTag(Tag As Integer) Implements IRosService.SetTag
        _Tag = Tag
    End Sub


    Public Overridable Function CallBack(RosPacketItem As RosPacketItem, CallBackType As RosServiceCallBackType) As Boolean Implements IRosService.CallBack
        Select Case CallBackType
            Case RosServiceCallBackType.Done
                Me.Statue = RosServiceStatue.Done
                If DoneCallBack IsNot Nothing Then
                    DoneCallBack.Invoke(Me, RosPacketItem）
                End If
                Return True
            Case RosServiceCallBackType.Re
                Me.Statue = RosServiceStatue.Running
                If ReCallback IsNot Nothing Then
                    ReCallback.Invoke(Me, RosPacketItem）
                End If
                Return True
            Case RosServiceCallBackType.Trap
                Me.Statue = RosServiceStatue.DoneWithTrap
                If TrapCallBack IsNot Nothing Then
                    TrapCallBack.Invoke(Me, RosPacketItem）
                End If
                Return True
            Case RosServiceCallBackType.Fatel
                Me.Statue = RosServiceStatue.DoneWithFatal
                If TrapCallBack IsNot Nothing Then
                    TrapCallBack.Invoke(Me, RosPacketItem）
                End If
                Return True
            Case RosServiceCallBackType.Other, RosServiceCallBackType.Err
                Return False
        End Select
        Return False
    End Function
End Class
