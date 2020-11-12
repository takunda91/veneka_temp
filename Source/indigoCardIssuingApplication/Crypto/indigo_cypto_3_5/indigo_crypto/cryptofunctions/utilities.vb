Namespace Cryptography

    Friend Class utilities

        ''' <summary>
        ''' Converts a string from hexadecimal to binary form.
        ''' </summary>
        ''' <param name="hexString"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function HexToBinary(ByVal hexString As String) As String
            Dim r As String = ""
            For i As Integer = 0 To hexString.Length - 1
                r = r + Convert.ToString(Convert.ToInt32(hexString.Substring(i, 1), 16), 2).PadLeft(4, "0"c)
            Next
            Return r
        End Function

        ''' <summary>
        ''' Converts a string from binary to hexadecimal form.
        ''' </summary>
        ''' <param name="binaryString"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function BinaryToHex(ByVal binaryString As String) As String
            Dim r As String = ""
            For i As Integer = 0 To binaryString.Length - 1 Step 4
                r = r + Convert.ToByte(binaryString.Substring(i, 4), 2).ToString("X1")
            Next
            Return r
        End Function

        ''' <summary>
        ''' Converts a hexadecimal string to a byte array.
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function HexToByteArray(ByVal s As String) As Byte()
            Dim i As Integer = 0, j As Integer = 0
            Dim bData() As Byte
            ReDim bData(s.Length \ 2 - 1)

            While i <= s.Length - 1
                bData(j) = Convert.ToByte(s.Substring(i, 2), 16)
                i += 2
                j += 1
            End While

            Return bData
        End Function

        ''' <summary>
        ''' Converts a byte array to a hexadecimal string.
        ''' </summary>
        ''' <param name="bData"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ByteArrayToHex(ByVal bData() As Byte) As String
            Dim sb As New System.Text.StringBuilder

            For i As Integer = 0 To bData.GetUpperBound(0)
                sb.AppendFormat("{0:X2}", bData(i))
            Next
            Return sb.ToString
        End Function


        ''' <summary>
        ''' Performs a XOR operation on two hexadecimal strings.
        ''' </summary>
        ''' <param name="sourceHexString"></param>
        ''' <param name="xorHexString"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function XORHex(ByVal sourceHexString As String, ByVal xorHexString As String) As String
            Dim s As String = ""

            For i As Integer = 0 To sourceHexString.Length - 1
                s = s + (Convert.ToInt32(sourceHexString.Substring(i, 1), 16) Xor Convert.ToInt32(xorHexString.Substring(i, 1), 16)).ToString("X")
            Next

            Return s
        End Function

        ''' <summary>
        ''' Performs an AND operation on two hexadecimal strings.
        ''' </summary>
        ''' <param name="sourceHexString"></param>
        ''' <param name="xorHexString"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ANDHex(ByVal sourceHexString As String, ByVal xorHexString As String) As String
            Dim s As String = ""

            For i As Integer = 0 To sourceHexString.Length - 1
                s = s + (Convert.ToInt32(sourceHexString.Substring(i, 1), 16) And Convert.ToInt32(xorHexString.Substring(i, 1), 16)).ToString("X")
            Next

            Return s
        End Function

        ''' <summary>
        ''' Performs an OR operation on two hexadecimal strings.
        ''' </summary>
        ''' <param name="sourceHexString"></param>
        ''' <param name="xorHexString"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ORHex(ByVal sourceHexString As String, ByVal xorHexString As String) As String
            Dim s As String = ""

            For i As Integer = 0 To sourceHexString.Length - 1
                s = s + (Convert.ToInt32(sourceHexString.Substring(i, 1), 16) Or Convert.ToInt32(xorHexString.Substring(i, 1), 16)).ToString("X")
            Next

            Return s
        End Function

        ''' <summary>
        ''' Return last 12 PAN digits, excluding the check digit.
        ''' </summary>
        ''' <param name="account"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetProperAccountDigits(ByVal account As String) As String
            Return account.Substring(account.Length - 12 - 1, 12)
        End Function

    End Class

End Namespace
