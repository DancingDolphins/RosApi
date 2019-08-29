Imports System.Net

Public Class Api
    Property Auth As RosAuth
    ReadOnly Property Link As New RosLink
    Private ReadOnly Services As New RosServices(Link)
    Property AutoReconnect As Boolean = False
    Private AutoReconnectTD As New Threading.Thread(AddressOf AutoReLoginLoop)

    Delegate Sub DLog(ByVal Str As String)
    Property Log As DLog

    Private _IsLogin As Boolean = False
    Private _ShowDebug As Boolean = False
    Private Sub AutoReLoginLoop()
        While True
            If AutoReconnect = False Or Me.IsLogin Then
                Threading.Thread.Sleep(1000)
            Else
                If IsConnected Then

                    Login()
                Else
                    If Connect(Auth) Then
                        Login()
                    Else
                        Threading.Thread.Sleep(1000)
                    End If



                End If
            End If
        End While
    End Sub


    ReadOnly Property IsConnected As Boolean
        Get
            Return Link.IsConnected
        End Get
    End Property



    Property ShowDebug As Boolean
        Get
            Return _ShowDebug
        End Get
        Set(value As Boolean)
            _ShowDebug = value
            Link.ShowDebug = value
        End Set
    End Property

    Public Property IsLogin As Boolean
        Get
            Return _IsLogin

        End Get
        Set(value As Boolean)
            'If value = False Then
            '    Services.Reset()
            'End If
            _IsLogin = value
        End Set

    End Property

    Sub New()
        AutoReconnectTD.Start()
        AddHandler Services.Log, AddressOf OnLog
        AddHandler Link.ELog, AddressOf OnLog
        AddHandler Link.EReceivePacket, AddressOf ProcPacket
    End Sub

    Public Sub ProcPacket(P As RosPacketItem)
        Services.AppendPacket(P)

    End Sub

    Public Sub OnLog(ByVal Str As String)
        If Log IsNot Nothing Then
            Log.Invoke(Str)
        End If
    End Sub

    Public Function Connect(Auth As RosAuth) As Boolean



        Me.Auth = Auth

        If Auth Is Nothing Then
            OnLog("No Ros Auth Set!")
            Return False
        Else

            Dim IPEND As IPEndPoint
            IPEND = New System.Net.IPEndPoint(System.Net.IPAddress.Parse(_Auth.IP), _Auth.Port)

            Try
                Link.Connect(IPEND)
                IsLogin = False
            Catch ex As Exception
                OnLog("Connect To " + _Auth.IP + " Failed")
                Return False
                Exit Function
            End Try

            If Link.Socket.Connected Then

                OnLog("Connected To " + _Auth.IP)
                Return True
            Else

                OnLog("Connect To " + _Auth.IP + " Failed")
                Return False

            End If

        End If

    End Function

    Public Function Login() As Boolean
        If Link.IsConnected = False Then
            IsLogin = False
            OnLog("Login Failed! Please Connect First")
            Return False
        End If

        Dim R_Login As New R_Login

        R_Login.SetAuth(Auth)
        R_Login.DoneCallBack = (Sub(X As R_Login, Y As RosPacketItem)
                                    OnLog("Login Done")

                                    IsLogin = True

                                End Sub)
        R_Login.TrapCallBack = (Sub(X As R_Login, Y As RosPacketItem)
                                    OnLog("Login Failed! Message=" + X.ErrorMessage)
                                    IsLogin = False
                                End Sub)
        Dim ServerRep As IRosService = Services.RunServices(R_Login)

        Return IsLogin

        'Me.ShowDebug = False
        'If ServerRep.Statue < 0 Then
        '    IsLogin = False
        '    OnLog("Login Failed! Message=" + System.Enum.GetName(GetType(RosServiceStatue), ServerRep.Statue))
        '    Return False
        'End If


    End Function

    Public Sub RunService(ByVal Ser As R_ServiceBase)
        If Me.IsLogin = False Or Me.IsConnected = False Then
            Me.IsLogin = False
            Exit Sub
        End If

        Services.RunServices(Ser)
        'If Ser.Statue < 0 Then
        '    IsLogin = False
        'End If
    End Sub

    Public Sub RunLinten(ByVal Ser As R_ServiceBase)
        If Me.IsLogin = False Or Me.IsConnected = False Then
            Me.IsLogin = False
            Exit Sub
        End If

        Services.RunListen(Ser)

    End Sub

    Public Async Sub RunServiceAsync(ByVal Ser As R_ServiceBase)


        If Me.IsLogin = False Or Me.IsConnected = False Then
            Me.IsLogin = False
            Exit Sub
        End If
        Await Services.RunServicesAsync(Ser)
        'If Ser.Statue < 0 Then
        '    IsLogin = False
        'End If
    End Sub

    Public Sub Close()
        Try
            IsLogin = False

            Services.Reset()

            Link.Close()
            OnLog("Closed")
        Catch ex As Exception
        End Try
    End Sub

End Class