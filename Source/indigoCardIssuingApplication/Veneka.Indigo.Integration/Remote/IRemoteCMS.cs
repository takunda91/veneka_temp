using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Config;

namespace Veneka.Indigo.Integration.Remote
{
    [InheritedExport(typeof(IRemoteCMS))]
    public interface IRemoteCMS : IDisposable
    {
        DirectoryInfo ApplicationDirectory { get; set; }

        List<CardDetailResponse> UpdateCards(List<CardDetail> cards, External.ExternalSystemFields externalFields, IConfig config);
    }
}
