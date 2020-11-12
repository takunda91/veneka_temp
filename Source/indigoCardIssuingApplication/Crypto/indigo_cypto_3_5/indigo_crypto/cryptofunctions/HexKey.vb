Namespace Cryptography

    ''' <summary>
    ''' This class represents a DES/3DES key.
    ''' </summary>
    ''' <remarks></remarks>
    Friend Enum KeyLength
        ''' <summary>
        ''' Single length keys.
        ''' </summary>
        ''' <remarks>
        ''' Defines a single length hexadecimal key (16 digits).
        ''' </remarks>
        SingleLength = 0

        ''' <summary>
        ''' Double length keys.
        ''' </summary>
        ''' <remarks>
        ''' Defines a double length hexadecimal key (32 digits).
        ''' </remarks>
        DoubleLength

        ''' <summary>
        ''' Triple length keys.
        ''' </summary>
        ''' <remarks>
        ''' Defines a triple length hexadecimal key (48 digits).
        ''' </remarks>
        TripleLength
    End Enum


    Friend Class HexKey

        ''' <summary>
        ''' First 16-hex characters of key.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property PartA As String

        ''' <summary>
        ''' Second 16-hex characters of key.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property PartB As String

        ''' <summary>
        ''' Last 16-hex characters of key.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property PartC As String
        Property KeyLength As KeyLength

        ''' <summary>
        ''' Initializes a key using a hexadecimal string.
        ''' </summary>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal key As String)

            If key.Length = 16 Then
                PartA = key
                PartB = key
                PartC = key
                KeyLength = KeyLength.SingleLength
            ElseIf key.Length = 32 Then
                _PartA = key.Substring(0, 16)
                _PartB = key.Substring(16)
                _PartC = _PartA
                _KeyLength = KeyLength.DoubleLength
            Else
                _PartA = key.Substring(0, 16)
                _PartB = key.Substring(16, 16)
                _PartC = key.Substring(32)
                _KeyLength = KeyLength.TripleLength
            End If
        End Sub

        ''' <summary>
        ''' Returns key as hexadecimal characters.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Select Case KeyLength
                Case Cryptography.KeyLength.SingleLength
                    Return PartA
                Case Cryptography.KeyLength.DoubleLength
                    Return PartA + PartB
                Case Else
                    Return PartA + PartB + PartC
            End Select
        End Function

    End Class


End Namespace