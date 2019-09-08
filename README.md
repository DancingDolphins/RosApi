Connect to Mikrotik RouterOs version above 6.43 using Mikrotik Api

## How to use 
  ``` VB.net
  Dim ROS As New Api 
  Dim Auth = New RosAuth
  Auth.IP = "192.168.10.50"
  Auth.Port = "8728"
  Auth.Username = "admin"
  Auth.Password = "admin"

  ROS.Connect(Auth)
  ROS.Login()
  ```


## Log?
  ``` VB.net
  ROS.Log = (Sub(ByVal Str As String) RosLog(Str))
  ```
	

## Other

  [Mikrotik RouterOs Api Wiki](https://wiki.mikrotik.com/wiki/Manual:API ) 
