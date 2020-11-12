
Imports System.Security.Cryptography
Imports System.IO

Namespace Cryptography


    ''' <summary>
    ''' Utility class to perform Triple DES operations.
    ''' </summary>
    ''' <remarks>
    ''' This class can be used to perform 3D operations using <see cref="HexKey"/> hexadecimal keys.
    ''' </remarks>
    Friend Class DESFunctions

        ''' <summary>
        ''' Performs an encryption operation.
        ''' </summary>
        ''' <remarks>
        ''' Performs an encryption operation.
        ''' </remarks>
        ''' 

        Public Shared Function TripleDESEncrypt(ByVal key As HexKey, ByVal data As String) As String

            If (data Is Nothing) OrElse (data.Length <> 16 AndAlso data.Length <> 32 AndAlso data.Length <> 48) Then Throw New InvalidOperationException("Invalid data for 3DEncrypt")

            Dim result As String
            If data.Length = 16 Then
                result = Encrypt(key, data)
            ElseIf data.Length = 32 Then
                result = Encrypt(key, data.Substring(0, 16)) + _
                         Encrypt(key, data.Substring(16, 16))
            Else
                result = Encrypt(key, data.Substring(0, 16)) + _
                         Encrypt(key, data.Substring(16, 16)) + _
                         Encrypt(key, data.Substring(32, 16))
            End If
            Return result
        End Function

        ''' <summary>
        ''' Performs a decrypt operation.
        ''' </summary>
        ''' <remarks>
        ''' Performs a decryption operation.
        ''' </remarks>
        Public Shared Function TripleDESDecrypt(ByVal key As HexKey, ByVal data As String) As String
            If (data Is Nothing) OrElse (data.Length <> 16 AndAlso data.Length <> 32 AndAlso data.Length <> 48) Then Throw New InvalidOperationException("Invalid data for 3DEncrypt")

            Dim result As String
            If data.Length = 16 Then
                result = Decrypt(key, data)
            ElseIf data.Length = 32 Then
                result = Decrypt(key, data.Substring(0, 16)) + _
                         Decrypt(key, data.Substring(16, 16))
            Else
                result = Decrypt(key, data.Substring(0, 16)) + _
                         Decrypt(key, data.Substring(16, 16)) + _
                         Decrypt(key, data.Substring(32, 16))
            End If
            Return result
        End Function

        ''' <summary>
        ''' Determines whether a key is weak or semi-weak.
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function IsWeakKey(ByVal key As HexKey) As Boolean
            Dim bKey() As Byte = utilities.HexToByteArray(key.ToString)
            If key.KeyLength = KeyLength.SingleLength Then
                Return (DESCryptoServiceProvider.IsWeakKey(bKey)) Or (DESCryptoServiceProvider.IsSemiWeakKey(bKey))
            Else
                Return TripleDESCryptoServiceProvider.IsWeakKey(bKey)
            End If
        End Function

        ''' <summary>
        ''' Returns a key check value.
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetCheckValue(ByVal key As HexKey) As String
            Return TripleDESEncrypt(key, "0000000000000000")
        End Function

        ''' <summary>
        ''' Encrypts 16-hex data.
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function Encrypt(ByVal key As HexKey, ByVal data As String) As String
            Dim result As String = ""
            result = DESEncrypt(key.PartA, data)
            result = DESDecrypt(key.PartB, result)
            result = DESEncrypt(key.PartC, result)
            Return result
        End Function

        ''' <summary>
        ''' Decrypts 16-hex data.
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function Decrypt(ByVal key As HexKey, ByVal data As String) As String
            Dim result As String
            result = DESDecrypt(key.PartC, data)
            result = DESEncrypt(key.PartB, result)
            result = DESDecrypt(key.PartA, result)
            Return result
        End Function


        ''' <summary>
        ''' Encrypts a byte array.
        ''' </summary>
        ''' <remarks>
        ''' The method encrypts a byte array of 16 bytes.
        ''' </remarks>
        Private Shared Sub byteDESEncrypt(ByVal bKey() As Byte, ByVal bData() As Byte, ByRef bResult() As Byte)
            ReDim bResult(7)

            Using outStream As MemoryStream = New MemoryStream(bResult)
                Using desProvider As DESCryptoServiceProvider = New DESCryptoServiceProvider
                    Dim bNullVector() As Byte = {0, 0, 0, 0, 0, 0, 0, 0}

                    desProvider.Mode = CipherMode.ECB
                    desProvider.Key = bKey
                    desProvider.IV = bNullVector
                    desProvider.Padding = PaddingMode.None

                    Using cStream As CryptoStream = New CryptoStream(outStream, desProvider.CreateEncryptor(bKey, bNullVector), CryptoStreamMode.Write)
                        cStream.Write(bData, 0, 8)
                        cStream.Close()
                    End Using
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Decrypts a byte array.
        ''' </summary>
        ''' <remarks>
        ''' This method decrypts a byte array of 16 bytes.
        ''' </remarks>
        Public Shared Sub byteDESDecrypt(ByVal bKey() As Byte, ByVal bData() As Byte, ByRef bResult() As Byte)
            Using desProvider As DESCryptoServiceProvider = New DESCryptoServiceProvider
                Dim bNullVector() As Byte = {0, 0, 0, 0, 0, 0, 0, 0}

                desProvider.Mode = CipherMode.ECB
                desProvider.Key = bKey
                desProvider.IV = bNullVector
                desProvider.Padding = PaddingMode.None

                ReDim bResult(7)

                Using outStream As MemoryStream = New MemoryStream(bResult)
                    Using cStream As CryptoStream = New CryptoStream(outStream, desProvider.CreateDecryptor(bKey, bNullVector), CryptoStreamMode.Write)
                        cStream.Write(bData, 0, 8)
                        cStream.Close()
                    End Using
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Encrypts a hex string.
        ''' </summary>
        ''' <remarks>
        ''' This method encrypts hex data under a hex key and returns the result.
        ''' </remarks>
        Public Shared Function DESEncrypt(ByVal sKey As String, ByVal sData As String) As String
            Dim bOutput() As Byte = {}, sResult As String = ""

            byteDESEncrypt(utilities.HexToByteArray(sKey), utilities.HexToByteArray(sData), bOutput)
            Return utilities.ByteArrayToHex(bOutput)
        End Function

        'DES-decrypt a 16-hex block using a 16-hex key
        ''' <summary>
        ''' Decrypts a hex string.
        ''' </summary>
        ''' <remarks>
        ''' This method decrypts hex data using a hex key and returns the result.
        ''' </remarks>
        Public Shared Function DESDecrypt(ByVal sKey As String, ByVal sData As String) As String
            Dim bOutput() As Byte = {}, sResult As String = ""

            byteDESDecrypt(utilities.HexToByteArray(sKey), utilities.HexToByteArray(sData), bOutput)
            Return utilities.ByteArrayToHex(bOutput)
        End Function

        ''' <summary>
        ''' Determines whether a key is weak or semi-weak.
        ''' </summary>
        ''' <param name="hexKey"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function IsWeakKey(ByVal hexKey As String) As Boolean
            Dim bKey() As Byte = utilities.HexToByteArray(hexKey)
            Return (DESCryptoServiceProvider.IsWeakKey(bKey)) Or (DESCryptoServiceProvider.IsSemiWeakKey(bKey))
        End Function


    End Class

End Namespace
