Public Interface IRosService

    ReadOnly Property Tag As Integer?
    ReadOnly Property Command As RosCommand
    Property Statue As RosServiceStatue

    Property IsListening As Boolean

    ReadOnly Property TimeOut As Integer
    ReadOnly Property WaitSendFlag As Threading.AutoResetEvent
    ReadOnly Property WaitFinishFlag As Threading.AutoResetEvent

    Function CallBack(ByVal RosPacketItem As RosPacketItem, CallBackType As RosServiceCallBackType) As Boolean

    ReadOnly Property ReplyPackets As List(Of RosPacketItem)

    Sub SetTag(Tag As Integer)

    Function GetCommand(Optional Query As RosQuery = Nothing) As RosCommand

    Event StatueChanged(Statue As RosServiceStatue)



End Interface


Public Enum RosServiceStatue As Short
    UnStarted = 0
    Added = 1
    Sended = 2
    Waiting = 3
    Running = 4
    Linsting = 5
    Done = 100
    DoneWithTrap = 101
    DoneWithFatal = 102

    SendFailed = -1

    ReveiveErr = -2
    NoTag = -3
    NoConnectiong = -20
    NoLogin = -21
    NoCommand = -22
    PoolFull = -23
    Timeout = -100
    SendTimeOut = -102
    Stoped = -101
End Enum