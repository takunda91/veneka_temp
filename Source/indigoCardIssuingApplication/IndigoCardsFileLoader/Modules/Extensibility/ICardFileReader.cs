using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IndigoFileLoader.objects;

namespace IndigoFileLoader.Modules.Extensibility
{
    public interface ICardFileReader
    {
        /// <summary>
        /// Own implementation on how the batch reference should be generated.
        /// </summary>
        /// <param name="fileInfor"></param>
        /// <returns></returns>
        string generateBatchReference(FileInfo fileInfo);

        /// <summary>
        /// Own implementation on how lines in card file are decoded to populate CardFileRecord.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        List<CardFileRecord> getCardRecords(FileInfo fileInfo);
    }
}
