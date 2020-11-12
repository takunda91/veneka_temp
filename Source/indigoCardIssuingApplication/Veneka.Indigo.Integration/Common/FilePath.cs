using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Veneka.Indigo.Integration.Common
{
    public sealed class FilePath
    {
        private static readonly object _fileCreateLock = new object();
        private static readonly object _updateLock = new object();
        private static readonly object _addLock = new object();
        private static readonly object _addLock2 = new object();

        /// <summary>
        /// Marker Keys
        /// </summary>
        public enum MarkerKeys
        {
            ISSUER_NAME_MARKER,
            ISSUER_CODE_MARKER,
            BRANCH_NAME_MARKER,
            BRANCH_CODE_MARKER,
            PRODUCT_NAME_MARKER,
            PRODUCT_CODE_MARKER,
            PRODUCT_CODE_GROUPING,
            DATE_MARKER,
            TIME_MARKER,
            BATCH_REFERENCE_NUMBER
        }

        /// <summary>
        /// Dictionary which contains the marker keys and to what value they correspond to in the parameters path.
        /// </summary>
        private static readonly Dictionary<MarkerKeys, string> MARKERS = new Dictionary<MarkerKeys, string>{
            {MarkerKeys.ISSUER_NAME_MARKER, "{issuer_name}"},
            {MarkerKeys.ISSUER_CODE_MARKER, "{issuer_code}"},
            {MarkerKeys.BRANCH_NAME_MARKER, "{branch_name}"},
            {MarkerKeys.BRANCH_CODE_MARKER, "{branch_code}"},
            {MarkerKeys.PRODUCT_NAME_MARKER, "{product_name}"},
            {MarkerKeys.PRODUCT_CODE_MARKER, "{product_code}"},
            {MarkerKeys.DATE_MARKER, @"\{date[^}]*\}"},
            {MarkerKeys.TIME_MARKER, @"\{time[^}]*\}"},
            {MarkerKeys.BATCH_REFERENCE_NUMBER, "{batch_ref}"}
        };

        public static string[] GetFilePathReferences()
        {
            return MARKERS.Values.ToArray();
        }

        //private class PathKey
        //{
        //    public string IssuerName { get; set; }
        //    public string IssuerCode { get; set; }
        //    public string BranchName { get; set; }
        //    public string BranchCode { get; set; }
        //    public string ProductName { get; set; }
        //    public string ProductCode { get; set; }
        //    public string Date { get; set; }
        //    public string Time { get; set; }
        //    public string Path { get; set; }

        //    public PathKey(string issuerName, string issuerCode, string branchName, string branchCode, 
        //                    string productName, string productCode, string date, string time, string path)
        //    {
        //        this.IssuerName = issuerName ?? String.Empty;
        //        this.IssuerCode = issuerCode ?? String.Empty;
        //        this.BranchName = branchName ?? String.Empty;
        //        this.BranchCode = branchCode ?? String.Empty;
        //        this.ProductName = productName ?? String.Empty;
        //        this.ProductCode = productCode ?? String.Empty;
        //        this.Date = date ?? String.Empty;
        //        this.Time = time ?? String.Empty;

        //        this.Path = path ?? String.Empty;
        //    }

        //    public PathKey(string path) : this(null, null, null, null, null, null, null, null, path)
        //    {
        //        if (String.IsNullOrWhiteSpace(path))
        //            throw new ArgumentNullException("path argument cannot be null or empty.");
        //    }

        //    public PathKey()
        //        : this(null, null, null, null, null, null, null, null, null)
        //    {

        //    }

        //    public bool Compare(PathKey otherKey)
        //    {
        //        if (this.IssuerName != otherKey.IssuerName)
        //            return false;

        //        if (this.IssuerCode != otherKey.IssuerCode)
        //            return false;

        //        if (this.BranchName != otherKey.BranchName)
        //            return false;

        //        if (this.BranchCode != otherKey.BranchCode)
        //            return false;

        //        if (this.ProductName != otherKey.ProductName)
        //            return false;

        //        if (this.ProductCode != otherKey.ProductCode)
        //            return false;

        //        if (this.Date != otherKey.Date)
        //            return false;

        //        if (this.Time != otherKey.Time)
        //            return false;

        //        return true;
        //    }

        //    public override bool Equals(object obj)
        //    {
        //        return this.Compare((PathKey)obj);
        //    }

        //    public static bool operator ==(PathKey c1, PathKey c2)
        //    {
        //        return c1.Compare(c2);
        //    }

        //    public static bool operator !=(PathKey c1, PathKey c2)
        //    {
        //        return !c1.Compare(c2);
        //    }
        //}

        /// <summary>
        /// Dictionary that contains paths that have already been created or exists on the file system.
        /// </summary>
        private Dictionary<string, DirectoryInfo> _cachedDirectories = new Dictionary<string, DirectoryInfo>();

        /// <summary>
        /// This method generates the path from the parameter and CardObject.
        /// It checks if the path exists on the file system, if it doesnt exist then the directory is created.
        /// 
        /// This Method should be used when files are generated from Indigo
        /// </summary>
        /// <param name="pathParameter"></param>
        /// <param name="cardObjects"></param>
        /// <param name="runDateTime"></param>
        /// <returns></returns>
        public Dictionary<string, List<Objects.CardObject>> CreateDirectory(string pathParameter, List<Objects.CardObject> cardObjects, DateTime runDateTime)
        {
            if (String.IsNullOrWhiteSpace(pathParameter))
                return null;

            Dictionary<string, List<Objects.CardObject>> allPaths = new Dictionary<string, List<Objects.CardObject>>();

            //Parallel.ForEach(cardObjects, cardObj =>
            //{
            foreach (var cardObj in cardObjects)
            {
                string realPath = pathParameter;

                foreach (var marker in MARKERS)
                {
                    string newValue = String.Empty;

                    switch (marker.Key)
                    {
                        case MarkerKeys.ISSUER_NAME_MARKER: newValue = cardObj.IssuerName; break;
                        case MarkerKeys.ISSUER_CODE_MARKER: newValue = cardObj.IssuerCode; break;
                        case MarkerKeys.BRANCH_NAME_MARKER: newValue = cardObj.BranchName; break;
                        case MarkerKeys.BRANCH_CODE_MARKER: newValue = cardObj.BranchCode; break;
                        case MarkerKeys.PRODUCT_NAME_MARKER: newValue = cardObj.ProductName; break;
                        case MarkerKeys.PRODUCT_CODE_MARKER: newValue =
                            String.IsNullOrWhiteSpace(cardObj.SubProductCode) ? cardObj.ProductCode : cardObj.SubProductCode; break;
                        case MarkerKeys.BATCH_REFERENCE_NUMBER: newValue = cardObj.DistBatchReference; break;
                        default: break;
                    }

                    if (marker.Key == MarkerKeys.DATE_MARKER || marker.Key == MarkerKeys.TIME_MARKER)
                    {
                        Match matches = System.Text.RegularExpressions.Regex.Match(realPath, marker.Value);

                        while (matches.Success)
                        {
                            foreach (Capture capture in matches.Captures)
                            {
                                newValue = runDateTime.ToString(capture.Value.Substring(5, capture.Value.Length - 6));
                                realPath = realPath.Replace(capture.Value, newValue);
                            }

                            matches = matches.NextMatch();
                        }
                    }
                    else
                    {
                        realPath = realPath.Replace(marker.Value, newValue);
                    }
                }

                if (!allPaths.ContainsKey(realPath))
                {
                    FileInfo fileAndPath = new FileInfo(realPath);

                    DirectoryInfo dirInfo;
                    if (!_cachedDirectories.TryGetValue(fileAndPath.DirectoryName.ToUpper(), out dirInfo))
                    {
                        dirInfo = fileAndPath.Directory;

                        lock (_fileCreateLock)
                        {
                            if (!dirInfo.Exists)
                                dirInfo.Create();
                        }

                        _cachedDirectories.Add(dirInfo.FullName.ToUpper(), dirInfo);
                    }

                    allPaths.Add(realPath, new List<Objects.CardObject>() { cardObj });                    
                }
                else
                {
                    var item = allPaths[realPath];
                    item.Add(cardObj);

                    allPaths[realPath] = item;
                }
            }

            return allPaths.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Takes the filename Parameter and replaces reference places with values.
        /// </summary>
        /// <param name="filenameParameter"></param>
        /// <param name="cardObject"></param>
        /// <param name="runDateTime"></param>
        /// <returns></returns>
        public string Filename(string filenameParameter, Objects.CardObject cardObject, DateTime runDateTime)
        {
            if (String.IsNullOrWhiteSpace(filenameParameter))
                return null;

            string realFilename = filenameParameter;

            foreach (var marker in MARKERS)
            {
                string newValue = String.Empty;

                switch (marker.Key)
                {
                    case MarkerKeys.ISSUER_NAME_MARKER: newValue = cardObject.IssuerName; break;
                    case MarkerKeys.ISSUER_CODE_MARKER: newValue = cardObject.IssuerCode; break;
                    case MarkerKeys.BRANCH_NAME_MARKER: newValue = cardObject.BranchName; break;
                    case MarkerKeys.BRANCH_CODE_MARKER: newValue = cardObject.BranchCode; break;
                    case MarkerKeys.PRODUCT_NAME_MARKER: newValue = cardObject.ProductName; break;
                    case MarkerKeys.PRODUCT_CODE_MARKER: newValue =
                        String.IsNullOrWhiteSpace(cardObject.SubProductCode) ? cardObject.ProductCode : cardObject.SubProductCode; break;
                    case MarkerKeys.BATCH_REFERENCE_NUMBER: newValue = cardObject.DistBatchReference; break;
                    default: break;
                }

                if (marker.Key == MarkerKeys.DATE_MARKER || marker.Key == MarkerKeys.TIME_MARKER)
                {
                    Match matches = System.Text.RegularExpressions.Regex.Match(realFilename, marker.Value);

                    while (matches.Success)
                    {
                        foreach (Capture capture in matches.Captures)
                        {
                            newValue = runDateTime.ToString(capture.Value.Substring(5, capture.Value.Length - 6));
                            realFilename = realFilename.Replace(capture.Value, newValue);
                        }

                        matches = matches.NextMatch();
                    }
                }
                else
                {
                    realFilename = realFilename.Replace(marker.Value, newValue);
                }
            }           

            return realFilename;
        }
        /// <summary>
        /// Method returns all the folders that have been specified in the path parameter argument
        /// 
        /// Should be used when folders need to be read.
        /// </summary>
        /// <param name="pathParameter"></param>
        /// <param name="runDateTime"></param>
        /// <returns></returns>
        public List<DirectoryInfo> ListDirectories(string pathParameter, DateTime runDateTime)
        {
            //if (String.IsNullOrWhiteSpace(pathParameter))
            //    return null;

            //string realPath = pathParameter;

            //foreach (var marker in MARKERS)
            //{
            //    string newValue = String.Empty;

            //    switch (marker.Key)
            //    {
            //        case MarkerKeys.ISSUER_NAME_MARKER: newValue = cardObj.IssuerName; break;
            //        case MarkerKeys.ISSUER_CODE_MARKER: newValue = cardObj.IssuerCode; break;
            //        case MarkerKeys.BRANCH_NAME_MARKER: newValue = cardObj.BranchName; break;
            //        case MarkerKeys.BRANCH_CODE_MARKER: newValue = cardObj.BranchCode; break;
            //        //case MarkerKeys.PRODUCT_NAME_MARKER: newValue = cardObj.pr
            //        case MarkerKeys.PRODUCT_CODE_MARKER: newValue =
            //            String.IsNullOrWhiteSpace(cardObj.SubProductCode) ? cardObj.ProductCode : cardObj.SubProductCode;
            //            break;
            //        case MarkerKeys.DATE_MARKER: newValue = runDateTime.ToString("yyyyMMdd"); break;
            //        case MarkerKeys.TIME_MARKER: newValue = runDateTime.ToString("hhmmss"); break;
            //        default: break;

            //    }

            //    realPath = realPath.Replace(marker.Value, newValue);
            //}


            return null;
        }
    }
}
