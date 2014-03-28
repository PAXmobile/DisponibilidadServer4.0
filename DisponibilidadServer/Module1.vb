Imports System.Management
Imports System
Imports System.IO
Imports System.Net

Module Module1

    Sub Main()

        Dim ipServer = getServer()
        Dim FreeSpace As String = Nothing
        Dim FreePhysicalMemory As String = Nothing
        Dim FreeVirtualMemory As String = Nothing

        Try
            Dim searcher As New ManagementObjectSearcher( _
                "root\CIMV2", _
                "SELECT * FROM Win32_LogicalDisk WHERE Caption = 'C:'")

            For Each queryObj As ManagementObject In searcher.Get()

                Console.WriteLine("-----------------------------------")
                Console.WriteLine("Win32_LogicalDisk instance")
                Console.WriteLine("-----------------------------------")
                Console.WriteLine("FreeSpace: {0}", queryObj("FreeSpace"))
                FreeSpace = queryObj("FreeSpace")
            Next
        Catch err As ManagementException
            Console.WriteLine("An error occurred while querying for WMI data: " & err.Message)
        End Try

        'Disponibilidad de la memoria

        Try
            Dim searcher As New ManagementObjectSearcher( _
                "root\CIMV2", _
                "SELECT * FROM Win32_OperatingSystem")

            For Each queryObj As ManagementObject In searcher.Get()

                Console.WriteLine("-----------------------------------")
                Console.WriteLine("Win32_OperatingSystem instance")
                Console.WriteLine("-----------------------------------")
                Console.WriteLine("FreePhysicalMemory: {0}", queryObj("FreePhysicalMemory"))
                FreePhysicalMemory = queryObj("FreePhysicalMemory")
            Next
        Catch err As ManagementException
            Console.WriteLine("An error occurred while querying for WMI data: " & err.Message)
        End Try

        'Espacio disponible de la memoria virtual
        Try
            Dim searcher As New ManagementObjectSearcher( _
                "root\CIMV2", _
                "SELECT * FROM Win32_OperatingSystem")

            For Each queryObj As ManagementObject In searcher.Get()

                Console.WriteLine("-----------------------------------")
                Console.WriteLine("Win32_OperatingSystem instance")
                Console.WriteLine("-----------------------------------")
                Console.WriteLine("FreeVirtualMemory: {0}", queryObj("FreeVirtualMemory"))
                FreeVirtualMemory = queryObj("FreeVirtualMemory")
            Next
        Catch err As ManagementException
            Console.WriteLine("An error occurred while querying for WMI data: " & err.Message)
        End Try

        'Consulta el porcentaje de uso del cpu
        Dim obj_WMI As Object, obj_INFO_CPU As Object, Porcentaje As Integer

        obj_WMI = GetObject("winmgmts:").InstancesOf("Win32_Processor")
        For Each obj_INFO_CPU In obj_WMI
            If obj_WMI.Count > 1 Then
                Porcentaje = Porcentaje + obj_INFO_CPU.LoadPercentage
            Else
                Porcentaje = obj_INFO_CPU.LoadPercentage
            End If
        Next

        If obj_WMI.Count > 1 Then
            Porcentaje = Porcentaje \ obj_WMI.Count
            obj_WMI = Nothing
            obj_INFO_CPU = Nothing
        End If

        Dim USO_CPU = Porcentaje




        'Envio los datos al dashboard
        Dim sURL As String = "http://dashboard.paxmobile.com/ping.php?server=" & ipServer & "&FreeSpace=" & FreeSpace & "&FreePhysicalMemory=" & FreePhysicalMemory & "&FreeVirtualMemory=" & FreeVirtualMemory & "&CPU=" & USO_CPU

        insertURL(sURL)
        'Dim wrGETURL As WebRequest
        'wrGETURL = WebRequest.Create(sURL)

        'Dim myProxy As New WebProxy("myproxy", 80)
        'myProxy.BypassProxyOnLocal = True

        ''wrGETURL.Proxy = myProxy
        'wrGETURL.Proxy = WebProxy.GetDefaultProxy()

        'Dim objStream As Stream
        'objStream = wrGETURL.GetResponse.GetResponseStream()

        'Dim objReader As New StreamReader(objStream)
        'Dim sLine As String = ""
        'Dim i As Integer = 0

        'Do While Not sLine Is Nothing
        '    i += 1
        '    sLine = objReader.ReadLine
        '    If Not sLine Is Nothing Then
        '        Console.WriteLine("{0}:{1}", i, sLine)
        '    End If
        'Loop
        'Console.ReadLine()
    End Sub

    'Funcion que me sirve para que la lectura del XML no me tome los tags HTML como un tag del XML
    Public Function getServer()
        Dim objReader As New StreamReader("C:\Disponibilidad\IPServer.txt") 'Probar con ruta relativa
        Dim sLine As String = Nothing
        Dim ipServer As String = Nothing
        Do
            sLine = objReader.ReadLine()
            If Not sLine Is Nothing Then
                ipServer = sLine
                Return ipServer
            End If
        Loop Until sLine Is Nothing

        objReader.Close()
        Return "no hay cadena"
    End Function


    Public Sub insertURL(ByVal laUrl)
        ' Cear la solicitud de la URL.
        Dim request As WebRequest = WebRequest.Create(laUrl)

        ' Obtener la respuesta.
        Dim response As WebResponse = request.GetResponse()

        ' Abrir el stream de la respuesta recibida.
        Dim reader As New StreamReader(response.GetResponseStream())

        ' Leer el contenido.
        Dim res As String = reader.ReadToEnd()

        ' Mostrarlo.
        Console.WriteLine(res)

        ' Cerrar los streams abiertos.
        reader.Close()
        response.Close()
    End Sub

End Module
