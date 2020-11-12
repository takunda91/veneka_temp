using System;
using System.Collections.Generic;
using IndigoCardIssuanceService.COMS;
using IndigoCardIssuanceService.DataContracts;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.COMS.DataSource;

namespace IndigoCardIssuanceService.bll
{
    public class LoadCardController
    {
        private readonly CardMangementService _cardManService = new CardMangementService(new LocalDataSource());


    }
}