using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.FileLoader.Objects;
using Veneka.Module.VenekaFileReader;
using Veneka.Module.VenekaFileReader.Objects;

namespace Veneka.Indigo.Integration.EMP.BLL
{
    public class FileReaderBLL
    {
        private static readonly ILog _fileLoaderLog = LogManager.GetLogger(Common.General.FILE_LOADER_LOGGER);

        private Dictionary<string, string> cardRefernces;
        public CardFile ReadFile(FileInfo fileInfo, ref List<FileCommentsObject> fileComments)
        {   
            _fileLoaderLog.Info("Reading Card Request File");
            fileComments.Add(new FileCommentsObject("Reading Card Request File"));

            BulkCards cards = new BulkCards();
            
            var fileUpload = cards.ReadFile(fileInfo);

            List<CardFileRecord> cardsList = new List<CardFileRecord>();
            
            int i = 0;
            int lineNumber = 0;
            foreach (var item in fileUpload.CardDetailRecords)
            {
                string pseudoPan = String.Format("{0}{1}{2}", item.BIN, GetUniqueKey(12 - item.BIN.Length), item.LastFour);
                cardsList.Add(new CardFileRecord(lineNumber, item.BIN.Substring(0, 6), fileUpload.BranchCode, pseudoPan, pseudoPan, item.BIN.Length > 6 ? item.BIN.Substring(6) : String.Empty, item.SequenceNumber, null, item.CardId, item.ReferenceNumber));
            }

            CardFile cardRequestFile = new CardFile(fileUpload.IssuerCode, fileInfo.Name, cardsList);

            return cardRequestFile;
        }

        public static string GetUniqueKey(int size = 6, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
            using (var crypto = new RNGCryptoServiceProvider())
            {
                var data = new byte[size];

                // If chars.Length isn't a power of 2 then there is a bias if
                // we simply use the modulus operator. The first characters of
                // chars will be more probable than the last ones.

                // buffer used if we encounter an unusable random byte. We will
                // regenerate it in this buffer
                byte[] smallBuffer = null;

                // Maximum random number that can be used without introducing a
                // bias
                int maxRandom = byte.MaxValue - ((byte.MaxValue + 1) % chars.Length);

                crypto.GetBytes(data);

                var result = new char[size];

                for (int i = 0; i < size; i++)
                {
                    byte v = data[i];

                    while (v > maxRandom)
                    {
                        if (smallBuffer == null)
                        {
                            smallBuffer = new byte[1];
                        }

                        crypto.GetBytes(smallBuffer);
                        v = smallBuffer[0];
                    }

                    result[i] = chars[v % chars.Length];
                }

                return new string(result);
            }
        }
        public void WriteFile(FileInfo fileInfo)
        {

        }
        public void GenerateCardReferences(CardFile cardFile)
        {
            //cardRefernces = new Dictionary<string, string>();

            //var refs = PseudoGenerator.Generate(6, cardFile.CardFileRecords.Count, PseudoGenerator.PseudoType.AlphaNumeric);

            //for (int i = 0; i < cardFile.CardFileRecords.Count; i++)
            //{                
            //    cardFile.CardFileRecords[i].CardReference = String.Format("{0}{1}{2}",
            //            cardFile.CardFileRecords[i].BIN,
            //            refs[i],
            //            cardFile.CardFileRecords[i].CardReference);

            //    //cardRefernces.Add(pan, refs[i]);
            //}
        }
    }
}
