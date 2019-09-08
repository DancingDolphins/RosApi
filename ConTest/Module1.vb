Imports RosApi

Module Module1
    Private ROS As New RosApi.Api


    Sub Main()
        Dim Auth As New RosAuth
        Auth.IP = "192.168.10.49"
        Auth.Port = 8728
        Auth.Username = "guest"
        Auth.Password = "1"
        ROS.Log = AddressOf Log
        ROS.ShowDebug = True
        ROS.Connect(Auth)
        'ROS.LoginPre643()
        ROS.Login()
    End Sub

    Private Sub Log(ByVal Str As String)
        Console.WriteLine(Str)
    End Sub

End Module
