using IndigoFileLoader.objects.root;

namespace IndigoFileLoader.objects.impl
{
    internal class HeaderRecord : AFileRecord
    {
        public override bool IsHeaderRecord
        {
            get { return true; }
        }

        public override bool IsDetailedRecord
        {
            get { return false; }
        }

        public override bool IsTraileerRecord
        {
            get { return false; }
        }
    }
}