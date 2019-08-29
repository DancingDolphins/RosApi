Imports System.Threading
Imports System.Threading.Tasks
Public Class RosServices
    Property Services As New Dictionary(Of Integer, IRosService)
    Public Event ServicesDone(IRosService As IRosService)
    Private ReadOnly SendThread As New Thread(AddressOf SendTD)
    Private SendThreadIsRuning As Boolean = False
    Private ReadOnly CommandList As New Queue(Of IRosService)
    Private Link As RosLink
    Public Event Log(ByVal Str As String)

    Public Sub OnLog(ByVal str As String)
        RaiseEvent Log(str)
    End Sub
    Sub New(Link As RosLink)
        Me.Link = Link
        StartSendThread()
        For i = 1 To 10
            Services.Add(i, Nothing)
        Next
    End Sub

    Public Sub Reset()
        SyncLock Services
            Services.Clear()
            For i = 1 To 10
                Services.Add(i, Nothing)
            Next
        End SyncLock

    End Sub


    Public Function AddServices(ByVal Service As IRosService) As Boolean
        If Service Is Nothing Then Return False
        If Service.Command.IsEmpty Then
            Service.Statue = RosServiceStatue.NoCommand
            Return False
        End If



        SyncLock Services


            For Each KV In Services


                If IsCanReplace(KV.Value) Then
                    Services(KV.Key) = Service
                    Service.SetTag(KV.Key)
                    OnLog("Replaced " + Service.Tag.ToString + " From pool")
                    Return True
                End If
            Next
        End SyncLock

        Service.Statue = RosServiceStatue.PoolFull
        OnLog("Remining=0")
        Thread.Sleep(500)
        Return False


    End Function

    Private Function IsCanReplace(Service As IRosService) As Boolean
        If Service Is Nothing Then Return True
        If Service.IsListening Then Return False
        Select Case Service.Statue
            Case RosServiceStatue.Done, RosServiceStatue.DoneWithFatal, RosServiceStatue.DoneWithTrap
                Return True
            Case < 0
                Return True
            Case Else
                Return False


        End Select
    End Function

    Public Async Function RunServicesAsync(ByVal Service As IRosService) As Task(Of IRosService)


        If AddServices(Service) = False Then
            Return Service
        End If
        Dim SendTimeOut As Boolean
        SendCommand(Service)
        Await (Task.Run(Sub()

                            SendTimeOut = Service.WaitSendFlag.WaitOne(Service.TimeOut)
                        End Sub))
        If SendTimeOut = False Then
            Service.Statue = RosServiceStatue.SendTimeOut
            Service.CallBack(Nothing, RosServiceCallBackType.Err)
            Return Service
        End If

        Dim ReceiveTimeout As Boolean
        Await (Task.Run(Sub()
                            ReceiveTimeout = Service.WaitFinishFlag.WaitOne(Service.TimeOut)
                        End Sub))
        If ReceiveTimeout = False Then
            Service.Statue = RosServiceStatue.Timeout
            Service.CallBack(Nothing, RosServiceCallBackType.Err)
            Return Service
        End If

        Return Service
    End Function

    Public Function RunListen(ByVal Service As IRosService) As IRosService
        If AddServices(Service) = False Then
            Return Service
        End If
        SendCommand(Service)
        Dim SendTimeOut As Boolean
        SendTimeOut = Service.WaitSendFlag.WaitOne(Service.TimeOut)
        If SendTimeOut = False Then
            Service.Statue = RosServiceStatue.SendTimeOut
            Service.CallBack(Nothing, RosServiceCallBackType.Err)
            Return Service
        End If
        Service.IsListening = True
        Return Service
    End Function


    Public Function RunServices(ByVal Service As IRosService) As IRosService

        If AddServices(Service) = False Then
            Return Service
        End If
        SendCommand(Service)
        Dim SendTimeOut As Boolean
        SendTimeOut = Service.WaitSendFlag.WaitOne(Service.TimeOut)
        If SendTimeOut = False Then
            Service.Statue = RosServiceStatue.SendTimeOut
            Service.CallBack(Nothing, RosServiceCallBackType.Err)
            Return Service
        End If

        Dim ReceiveTimeout As Boolean

        ReceiveTimeout = Service.WaitFinishFlag.WaitOne(Service.TimeOut)

        If ReceiveTimeout = False Then
            Service.Statue = RosServiceStatue.Timeout
            Service.CallBack(Nothing, RosServiceCallBackType.Err)
            Return Service
        End If

        'Service.Statue = RosServiceStatue.Done
        'FinishServices(Service)
        Return Service

    End Function



    Public Function AppendPacket(ByVal PackertItem As RosPacketItem) As IRosService



        If PackertItem.Tag Is Nothing Then Return Nothing


        Dim FirstFound As IRosService = Nothing

        FirstFound = Services(PackertItem.Tag)

        If FirstFound IsNot Nothing Then
            FirstFound.CallBack(PackertItem, GetPacketType(PackertItem))
            '   FirstFound.ReplyPackets.Add(PackertItem)


            If PackertItem.HasDone Then
                FirstFound.Statue = RosServiceStatue.Done
                FirstFound.WaitFinishFlag.Set()
                Return FirstFound

            End If
            If PackertItem.HasTrap Then
                FirstFound.Statue = RosServiceStatue.DoneWithTrap
                FirstFound.WaitFinishFlag.Set()
                Return FirstFound
            End If
            If PackertItem.Hasfatal Then
                FirstFound.Statue = RosServiceStatue.DoneWithFatal
                FirstFound.WaitFinishFlag.Set()
                Return FirstFound
            End If
        End If
        Return FirstFound
                                         End Function

    Public Function GetPacketType(x As RosPacketItem) As RosServiceCallBackType
        If x.HasDone Then Return RosServiceCallBackType.Done
        If x.Hasfatal Then Return RosServiceCallBackType.Fatel
        If x.HasTrap Then Return RosServiceCallBackType.Trap
        If x.HasRe Then Return RosServiceCallBackType.Re
        Return RosServiceCallBackType.Other
    End Function

    Private Sub StartSendThread()
        If SendThreadIsRuning Then Exit Sub
        SendThread.Start()
    End Sub
    Private Sub SendTD()
        If SendThreadIsRuning Then Exit Sub
        SendThreadIsRuning = True
        While True


            If CommandList.Count = 0 Then
                Thread.Sleep(100)
            Else
                Dim DSend As IRosService = CommandList.Dequeue
                Try


                    Link.SendRosCommand(DSend.GetCommand)
                Catch ex As Exception
                    OnLog(ex.ToString)
                    DSend.Statue = RosServiceStatue.SendFailed

                End Try
                DSend.WaitSendFlag.Set()

            End If
        End While
    End Sub

    Public Sub SendCommand(ByVal Command As IRosService)

        CommandList.Enqueue(Command)
    End Sub
End Class
