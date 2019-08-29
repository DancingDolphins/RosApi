Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Public Class RosLink
    Property ShowDebug As Boolean = False
    Public Sub New()

    End Sub

    Public Event ELog(ByVal Str As String)
    Public Event EReceivePacket(ByVal RosPacket As RosPacketItem)

    Public Sub OnLog(ByVal Str As String)
        RaiseEvent ELog(Str)
    End Sub

    Public Sub OnReceivedPacket(ByVal RosPacket As RosPacketItem)
        RaiseEvent EReceivePacket(RosPacket)
    End Sub

    Private _IsConnected As Boolean = False
    Public ReadOnly Property IsConnected() As Boolean
        Get
            Return _IsConnected
        End Get

    End Property

    Private _IsClosed As Boolean = False
    Public ReadOnly Property IsClosed() As Boolean
        Get
            Return _IsClosed
        End Get

    End Property



    ReadOnly Property Socket As Socket
    Public Sub Close()
        _IsClosed = True
        If _Socket Is Nothing Then
            Exit Sub
        Else
            _Socket.Close()
        End If
    End Sub

    Public Sub Connect(IPEND As IPEndPoint)
        _Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        Try
            OnLog("Start Connect")
            Socket.Connect(IPEND)
            _IsConnected = True
            _IsClosed = False
        Catch ex As Exception
            OnLog(ex.ToString)
            _IsConnected = False
            _IsClosed = False

        End Try


        StartReceive()
    End Sub





    Public Sub ProReceive(MS As MemoryStream)

        Static NowMem As New MemoryStream

        If NowMem Is Nothing Then
            NowMem = New MemoryStream
        End If
        Dim Tbytes As Byte() = MS.ToArray

        For i = 1 To Tbytes.Count
            NowMem.WriteByte(Tbytes(i - 1))
            If Tbytes(i - 1) = 0 Then

                NowMem.Position = 0
                Dim NowGetCount As Integer
                Dim NowItem As New RosPacketItem
                While NowMem.Length > 0 And NowMem.Position < NowMem.Length
                    NowGetCount = GetCount(NowMem)
                    If NowGetCount = 0 Then
                        'NowProcPacket.AddendPacket(NowItem)
                        Me.OnReceivedPacket(NowItem)

                        NowItem = New RosPacketItem
                    End If
                    Dim StringBuffer(NowGetCount - 1) As Byte
                    NowMem.Read(StringBuffer, 0, NowGetCount)
                    Dim NowString As String = Encoding.Default.GetString(StringBuffer)
                    Dim Rtype = NowItem.Append(NowString)


                    'BuffedMemIo.Seek(0, IO.SeekOrigin.Begin)
                    'BuffedMemIo.SetLength(0)
                End While
                NowMem = New MemoryStream
            End If
        Next

    End Sub




    Public Sub SendRosCommand(ByVal Command As RosCommand)
        If Command Is Nothing OrElse Command.IsEmpty Then
            Exit Sub
        Else
            Try


                For Each ComStr As String In Command.CommandsString
                    Dim Bytes() As Byte
                    Dim Size() As Byte
                    Bytes = System.Text.Encoding.Default.GetBytes(ComStr)
                    Size = EncodeLength(Bytes.Length)

                    _Socket.Send(Size)
                    _Socket.Send(Bytes)
                Next
                _Socket.Send({0})
            Catch ex As Exception
                OnLog(ex.ToString)
                _IsConnected = False
                _IsClosed = True
            End Try



        End If
        '    Me._Socket.Send(Bytes)
    End Sub



    Public Sub StartReceive()
        Dim SAEA As New SocketAsyncEventArgs With {.SocketFlags = SocketFlags.None}
        Dim MTU As Integer = 2048
        Dim Buffer(MTU) As Byte
        SAEA.SetBuffer(Buffer, 0, MTU)
        AddHandler SAEA.Completed, (Sub()
                                        Select Case SAEA.SocketError
                                            Case SocketError.Success

                                                Dim ReadCount As Integer = SAEA.BytesTransferred
                                                If ReadCount = 0 Then
                                                    Exit Sub
                                                Else

                                                    If ShowDebug Then
                                                        ShowReRaw(Buffer, ReadCount)
                                                    End If


                                                    Dim MS As New MemoryStream
                                                    MS.Write(Buffer, 0, ReadCount)
                                                    ProReceive(MS)

                                                End If
                                                Socket.ReceiveAsync(SAEA）
                                            Case Else
                                                OnLog("Socket:" + System.Enum.GetName(GetType(SocketError), SAEA.SocketError))
                                                _IsClosed = True
                                                _IsConnected = False


                                        End Select

                                    End Sub)

        '  While True
        Socket.ReceiveAsync(SAEA）
        '   End While

    End Sub

    Private Sub ShowReRaw(Bytes As Byte(), Count As Integer)

        Console.WriteLine(Count)
        For i = 1 To Count
            If Bytes(i - 1) = 0 Then
                Console.ForegroundColor = ConsoleColor.Red

            End If
            Console.Write(Bytes(i - 1).ToString().PadLeft(3, "_") + " ")

            Console.ForegroundColor = ConsoleColor.White
        Next
        Console.WriteLine()
        For i = 1 To Count
            If Bytes(i - 1) = 0 Then
                Console.ForegroundColor = ConsoleColor.Red

            End If
            If Bytes(i - 1) >= 32 Then
                Console.Write(ChrW(Bytes(i - 1)).ToString.PadLeft(3, " ") + " ")
            Else
                Console.Write(Bytes(i - 1).ToString().PadLeft(3, "_") + " ")
            End If

            Console.ForegroundColor = ConsoleColor.White
        Next
        Console.WriteLine()
    End Sub


    Private Function EncodeLength(ByVal l As Integer) As Byte()
        If l < &H80 Then
            Dim tmp = BitConverter.GetBytes(l)
            Return New Byte() {tmp(0)}
        ElseIf l < &H4000 Then
            Dim tmp = BitConverter.GetBytes(l Or &H8000)
            Return New Byte() {tmp(1), tmp(0)}
        ElseIf l < &H200000 Then
            Dim tmp = BitConverter.GetBytes(l Or &HC00000)
            Return New Byte() {tmp(2), tmp(1), tmp(0)}
        ElseIf l < &H10000000 Then
            Dim tmp = BitConverter.GetBytes(l Or &HE0000000)
            Return New Byte() {tmp(3), tmp(2), tmp(1), tmp(0)}
        Else
            Dim tmp = BitConverter.GetBytes(l)
            Return New Byte() {&HF0, tmp(3), tmp(2), tmp(1), tmp(0)}
        End If
    End Function


    Public Function GetCount(MS As MemoryStream) As Integer
        Dim NowProcingString As String = ""
        Dim tmp(4) As Byte
        Dim count As Long

        tmp(3) = MS.ReadByte
        Select Case tmp(3)
            Case 0
                Return 0

            Case Is < &H80
                count = tmp(3)
            Case Is < &HC0
                count = BitConverter.ToInt32(New Byte() {MS.ReadByte(), tmp(3), 0, 0}, 0) ^ &H8000
            Case Is < &HE0
                tmp(2) = MS.ReadByte()
                count = BitConverter.ToInt32(New Byte() {MS.ReadByte(), tmp(2), tmp(3), 0}, 0) ^ &HC00000
            Case Is < &HF0
                tmp(2) = MS.ReadByte()
                tmp(1) = MS.ReadByte()
                count = BitConverter.ToInt32(New Byte() {MS.ReadByte(), tmp(1), tmp(2), tmp(3)}, 0) ^ &HE0000000
            Case &HF0
                tmp(3) = MS.ReadByte()
                tmp(2) = MS.ReadByte()
                tmp(1) = MS.ReadByte()
                tmp(0) = MS.ReadByte()
                count = BitConverter.ToInt32(tmp, 0)
            Case Else
                count = -1

        End Select

        Return count

    End Function
End Class