using System.Collections.Generic;

namespace IndigoFileLoader.objects.root
{
    public abstract class ACardsFile
    {
        public abstract FileExtensionType CardsFileType { get; }
        public abstract List<BatchRecord> GetBatchRecords();
        public abstract List<string[]> ReadFile();
        public abstract List<AFileRecord> GetFileRecords();
        public abstract bool IsHeaderRecord(AFileRecord record);
        public abstract bool IsDetailedRecord(AFileRecord record);
        public abstract bool IsTrailerRecord(AFileRecord record);
    }
}