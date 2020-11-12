using IndigoFileLoader.objects.root;

namespace IndigoFileLoader.objects.impl
{
    internal class DetailRecord : AFileRecord
    {
        public override bool IsHeaderRecord
        {
            get { return false; }
        }

        public override bool IsDetailedRecord
        {
            get { return true; }
        }

        public override bool IsTraileerRecord
        {
            get { return false; }
        }
    }
}