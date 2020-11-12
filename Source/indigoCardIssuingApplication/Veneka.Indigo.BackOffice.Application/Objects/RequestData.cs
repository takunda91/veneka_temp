using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.BackOffice.API;

namespace Veneka.Indigo.BackOffice.Application.Objects
{
    public enum PrintingStatus
    {
        PRINTING=0,
        PRINTED = 1,
        RE_PRINT = 2,
        CARD_SPOILED = 3,
       
    }
    public class RequestData : RequestDetails, INotifyPropertyChanged
    {
        public RequestData(RequestDetails requestdetails, string _printingProgress, bool _isSelected)
        {

            base.RequestId = requestdetails.RequestId;
            base.RequestReference = requestdetails.RequestReference;
            base.RequestStatuesId = requestdetails.RequestStatuesId;
            base.RequestStatus = requestdetails.RequestStatus;
            this.printingProgress = _printingProgress;
            base.ProdTemplateList = requestdetails.ProdTemplateList;
            this.isSelected = _isSelected;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private string printingProgress;
        public string PrintingProgress
        {
            get { return printingProgress; }
            set
            {
                if (printingProgress != value)
                {
                    printingProgress = value;
                        if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("PrintingProgress"));
                }
            }
        }
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                }
            }
        }
    }
}
