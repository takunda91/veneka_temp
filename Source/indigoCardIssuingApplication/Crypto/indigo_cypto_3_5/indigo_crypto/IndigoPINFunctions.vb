
Imports indigo_crypto.Cryptography

    Public Enum PinBlockFormatConstants
        AnsiX98 = 0
        Diebold
        IBM_3624
    End Enum

    Public Class IndigoPINFunctions
        'TODO - Create a Local Master Key
    Private Const CONST_LMK As String = "96F3CF303347E23A0AF603EB2EBEFAE8"

    Public Shared Function GetPINFromEncryptedPINBlock(ByVal EncryptedZPK As String, ByVal Pan As String, ByVal EncryptedPINBlock As String, ByVal PINBlockType As PinBlockFormatConstants) As String
        'First perform DES Decrypt on PIN Block
        'PIN Block encrypted under ZPK
        'Dim ZPK As New HexKey(EncryptedZPK)
        'Dim strClearZPK As String = DESFunctions.TripleDESDecrypt(ZPK, CONST_LMK)
        Dim LMK As New HexKey(CONST_LMK)
        Dim strClearZPK As String = DESFunctions.TripleDESDecrypt(LMK, EncryptedZPK)
        Dim ClearZPK As New HexKey(strClearZPK)
        Dim strClearPINBlock As String = DESFunctions.TripleDESDecrypt(ClearZPK, EncryptedPINBlock)

        'Then get the clear PIN from the PIN Block
        Dim strClearPIN As String = ""
        strClearPIN = GetPINFromClearPINBlock(Pan, strClearPINBlock, PINBlockType)
        'Return the Clear PIN

        GetPINFromEncryptedPINBlock = strClearPIN
    End Function

    Private Shared Function GetPINFromClearPINBlock(ByVal Pan As String, ByVal PINBlock As String, ByVal PINBlockType As PinBlockFormatConstants) As String
        Dim strClearPIN As String = ""

        strClearPIN = ToPIN(PINBlockType, PINBlock, Pan)

        GetPINFromClearPINBlock = strClearPIN
    End Function

        ''' <summary>
        ''' Find a PIN from a PIN block.
        ''' </summary>
        ''' <param name="PBFormat"></param>
        ''' <param name="PINBlock"></param>
        ''' <param name="Account"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function ToPIN(ByVal PBFormat As PinBlockFormatConstants, ByVal PINBlock As String, ByVal Account As String) As String
            Select Case PBFormat
                Case PinBlockFormatConstants.AnsiX98
                    Return GetPIN_AnsiX98(PINBlock, Account)
                Case PinBlockFormatConstants.Diebold
                    Return GetPIN_Diebold(PINBlock)
                Case PinBlockFormatConstants.IBM_3624
                    Return GetPIN_Diebold(PINBlock)

                Case Else
                    Throw New InvalidOperationException("Invalid PIN Block Format code")
            End Select
        End Function

        ''' <summary>
        ''' Get PIN from PIN block for Ansi X9.8.
        ''' </summary>
        ''' <param name="PINBlock"></param>
        ''' <param name="Account"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetPIN_AnsiX98(ByVal PINBlock As String, ByVal Account As String) As String
            Dim unXor As String = utilities.XORHex(PINBlock, utilities.GetProperAccountDigits(Account).PadLeft(16, "0"c))
            Return unXor.Substring(2, Convert.ToInt32(unXor.Substring(0, 2)))
        End Function

        ''' <summary>
        ''' Get PIN from PIN block for Diebold.
        ''' </summary>
        ''' <param name="PINBlock"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetPIN_Diebold(ByVal PINBlock As String) As String
            Return PINBlock.Replace("F", "")
        End Function

End Class
