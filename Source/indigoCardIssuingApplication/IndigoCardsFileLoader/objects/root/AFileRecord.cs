namespace IndigoFileLoader.objects.root
{
    public abstract class AFileRecord
    {
        public abstract bool IsHeaderRecord { get; }
        public abstract bool IsDetailedRecord { get; }
        public abstract bool IsTraileerRecord { get; }
    }
}