using IndigoFileLoader.objects.root;

namespace IndigoFileLoader.objects.impl
{
    internal class TrailerRecord : AFileRecord
    {
        public override bool IsHeaderRecord
        {
            get { return false; }
        }

        public override bool IsDetailedRecord
        {
            get { return false; }
        }

        public override bool IsTraileerRecord
        {
            get { return true; }
        }
    }
}